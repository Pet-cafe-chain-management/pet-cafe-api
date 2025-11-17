using Microsoft.EntityFrameworkCore;
using PetCafe.Application.GlobalExceptionHandling.Exceptions;
using PetCafe.Domain.Constants;
using PetCafe.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace PetCafe.Application.Services;

public interface ILeaveRequestAssignmentService
{
    Task ProcessAdvanceLeaveAsync(LeaveRequest leaveRequest);
    Task ProcessEmergencyLeaveAsync(LeaveRequest leaveRequest);
    Task<Employee?> AutoAssignReplacementAsync(Guid teamId, Guid workShiftId, DateTime leaveDate, Guid excludeEmployeeId);
}

public class LeaveRequestAssignmentService(
    IUnitOfWork _unitOfWork,
    INotificationService _notificationService
) : ILeaveRequestAssignmentService
{
    public async Task ProcessAdvanceLeaveAsync(LeaveRequest leaveRequest)
    {
        if (leaveRequest.ReplacementEmployeeId == null)
        {
            throw new BadRequestException("Nhân viên thay thế là bắt buộc cho nghỉ phép có kế hoạch!");
        }

        // Load leaveRequest with Employee and ReplacementEmployee
        leaveRequest = await _unitOfWork.LeaveRequestRepository.GetByIdAsync(
            leaveRequest.Id,
            includeFunc: q => q
                .Include(lr => lr.Employee)
                .Include(lr => lr.ReplacementEmployee!)
        ) ?? throw new BadRequestException("Không tìm thấy đơn nghỉ phép!");

        // Lấy tất cả DailySchedules của employee nghỉ cho ngày đó
        var employeeSchedules = await _unitOfWork.DailyScheduleRepository.WhereAsync(
            ds => ds.EmployeeId == leaveRequest.EmployeeId
                && ds.Date.Date == leaveRequest.LeaveDate.Date
                && ds.Status != DailyScheduleStatusConstant.EXCUSED,
            includeFunc: q => q.Include(ds => ds.TeamMember)
        );

        if (employeeSchedules.Count == 0)
        {
            return; // Không có lịch nào cần xử lý
        }

        // Lấy replacement employee và team members
        var replacementEmployee = await _unitOfWork.EmployeeRepository.GetByIdAsync(leaveRequest.ReplacementEmployeeId!.Value)
            ?? throw new BadRequestException("Không tìm thấy nhân viên thay thế!");

        var replacementTeamMembers = await _unitOfWork.TeamMemberRepository.WhereAsync(
            tm => tm.EmployeeId == replacementEmployee.Id && !tm.IsDeleted
        );

        // Xử lý từng schedule
        foreach (var schedule in employeeSchedules)
        {
            // Disable schedule của employee nghỉ
            schedule.Status = DailyScheduleStatusConstant.EXCUSED;
            schedule.Notes = $"Nghỉ phép - Thay thế bởi {replacementEmployee.FullName}";
            _unitOfWork.DailyScheduleRepository.Update(schedule);

            // Tìm team member của replacement employee trong cùng team
            var replacementTeamMember = replacementTeamMembers.FirstOrDefault(
                tm => tm.TeamId == schedule.TeamMember.TeamId
            );

            if (replacementTeamMember == null)
            {
                // Replacement employee không trong cùng team, bỏ qua
                continue;
            }

            // Kiểm tra xem replacement employee đã có schedule cho ca này chưa
            var existingReplacementSchedule = await _unitOfWork.DailyScheduleRepository.FirstOrDefaultAsync(
                ds => ds.EmployeeId == replacementEmployee.Id
                    && ds.WorkShiftId == schedule.WorkShiftId
                    && ds.Date.Date == leaveRequest.LeaveDate.Date
            );

            if (existingReplacementSchedule == null)
            {
                // Tạo schedule mới cho replacement employee
                var newSchedule = new DailySchedule
                {
                    TeamMemberId = replacementTeamMember.Id,
                    EmployeeId = replacementEmployee.Id,
                    WorkShiftId = schedule.WorkShiftId,
                    Date = leaveRequest.LeaveDate,
                    Status = DailyScheduleStatusConstant.PENDING,
                    Notes = $"Thay thế cho {leaveRequest.Employee.FullName}"
                };

                await _unitOfWork.DailyScheduleRepository.AddAsync(newSchedule);

                // Send notification to replacement employee
                var replacementEmployeeWithAccount = await _unitOfWork.EmployeeRepository.GetByIdAsync(
                    replacementEmployee.Id,
                    includeFunc: q => q.Include(e => e.Account)
                );

                if (replacementEmployeeWithAccount?.AccountId != null)
                {
                    await _notificationService.SendNotificationAsync(
                        replacementEmployeeWithAccount.AccountId,
                        "Được phân công thay thế",
                        $"Bạn đã được phân công thay thế cho {leaveRequest.Employee.FullName} vào ngày {leaveRequest.LeaveDate:dd/MM/yyyy}",
                        "LeaveRequest",
                        "Normal",
                        leaveRequest.Id,
                        "LeaveRequest"
                    );
                }
            }
        }

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task ProcessEmergencyLeaveAsync(LeaveRequest leaveRequest)
    {
        // Load leaveRequest with Employee
        leaveRequest = await _unitOfWork.LeaveRequestRepository.GetByIdAsync(
            leaveRequest.Id,
            includeFunc: q => q.Include(lr => lr.Employee)
        ) ?? throw new BadRequestException("Không tìm thấy đơn nghỉ phép!");

        // Lấy tất cả DailySchedules của employee nghỉ cho ngày đó
        var employeeSchedules = await _unitOfWork.DailyScheduleRepository.WhereAsync(
            ds => ds.EmployeeId == leaveRequest.EmployeeId
                && ds.Date.Date == leaveRequest.LeaveDate.Date
                && ds.Status != DailyScheduleStatusConstant.EXCUSED,
            includeFunc: q => q
                .Include(ds => ds.TeamMember)
                    .ThenInclude(tm => tm.Team)
                .Include(ds => ds.WorkShift)
        );

        if (employeeSchedules.Count == 0)
        {
            return; // Không có lịch nào cần xử lý
        }

        // Xử lý từng schedule
        foreach (var schedule in employeeSchedules)
        {
            // Disable schedule của employee nghỉ
            schedule.Status = DailyScheduleStatusConstant.EXCUSED;
            schedule.Notes = "Nghỉ phép đột xuất";
            _unitOfWork.DailyScheduleRepository.Update(schedule);

            // Auto-assign replacement
            var replacementEmployee = await AutoAssignReplacementAsync(
                schedule.TeamMember.TeamId,
                schedule.WorkShiftId,
                leaveRequest.LeaveDate,
                leaveRequest.EmployeeId
            );

            if (replacementEmployee != null)
            {
                // Tìm team member của replacement employee
                var replacementTeamMember = await _unitOfWork.TeamMemberRepository.FirstOrDefaultAsync(
                    tm => tm.EmployeeId == replacementEmployee.Id
                        && tm.TeamId == schedule.TeamMember.TeamId
                        && !tm.IsDeleted
                );

                if (replacementTeamMember != null)
                {
                    // Kiểm tra xem đã có schedule chưa
                    var existingSchedule = await _unitOfWork.DailyScheduleRepository.FirstOrDefaultAsync(
                        ds => ds.EmployeeId == replacementEmployee.Id
                            && ds.WorkShiftId == schedule.WorkShiftId
                            && ds.Date.Date == leaveRequest.LeaveDate.Date
                    );

                    if (existingSchedule == null)
                    {
                        // Tạo schedule mới cho replacement employee
                        var newSchedule = new DailySchedule
                        {
                            TeamMemberId = replacementTeamMember.Id,
                            EmployeeId = replacementEmployee.Id,
                            WorkShiftId = schedule.WorkShiftId,
                            Date = leaveRequest.LeaveDate,
                            Status = DailyScheduleStatusConstant.PENDING,
                            Notes = $"Thay thế đột xuất cho {leaveRequest.Employee.FullName}"
                        };

                        await _unitOfWork.DailyScheduleRepository.AddAsync(newSchedule);

                        // Send notification to replacement employee
                        var replacementEmployeeWithAccount = await _unitOfWork.EmployeeRepository.GetByIdAsync(
                            replacementEmployee.Id,
                            includeFunc: q => q.Include(e => e.Account)
                        );

                        if (replacementEmployeeWithAccount?.AccountId != null)
                        {
                            await _notificationService.SendNotificationAsync(
                                replacementEmployeeWithAccount.AccountId,
                                "Được phân công thay thế đột xuất",
                                $"Bạn đã được tự động phân công thay thế cho {leaveRequest.Employee.FullName} vào ngày {leaveRequest.LeaveDate:dd/MM/yyyy}",
                                "LeaveRequest",
                                "High",
                                leaveRequest.Id,
                                "LeaveRequest"
                            );
                        }
                    }
                }
            }
        }

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<Employee?> AutoAssignReplacementAsync(
        Guid teamId,
        Guid workShiftId,
        DateTime leaveDate,
        Guid excludeEmployeeId)
    {
        // Lấy work shift cần thay thế
        var workShift = await _unitOfWork.WorkShiftRepository.GetByIdAsync(workShiftId)
            ?? throw new BadRequestException("Không tìm thấy ca làm việc!");

        // Lấy tất cả team members trong team (trừ employee nghỉ)
        var teamMembers = await _unitOfWork.TeamMemberRepository.WhereAsync(
            tm => tm.TeamId == teamId
                && tm.EmployeeId != excludeEmployeeId
                && !tm.IsDeleted,
            includeFunc: q => q
                .Include(tm => tm.Employee)
        );

        if (teamMembers.Count == 0)
        {
            return null; // Không có nhân viên nào để thay thế
        }

        var candidates = new List<Candidate>();

        foreach (var member in teamMembers)
        {
            var employee = member.Employee;
            if (employee == null || !employee.IsActive)
            {
                continue;
            }

            // Lấy optional work shifts của employee
            var optionalShifts = await _unitOfWork.EmployeeOptionalWorkShiftRepository.WhereAsync(
                eows => eows.EmployeeId == employee.Id
                    && eows.IsAvailable,
                includeFunc: q => q.Include(eows => eows.WorkShift)
            );

            foreach (var optShift in optionalShifts)
            {
                var shift = optShift.WorkShift;
                if (shift == null)
                {
                    continue;
                }

                // Kiểm tra compatibility:
                // 1. Cùng ngày áp dụng (ApplicableDays)
                var dayOfWeek = leaveDate.DayOfWeek.ToString().ToUpper();
                if (!shift.ApplicableDays.Contains(dayOfWeek))
                {
                    continue;
                }

                // 2. Thời gian overlap hoặc bao phủ
                var isTimeCompatible = shift.StartTime < workShift.EndTime
                    && shift.EndTime > workShift.StartTime;

                if (!isTimeCompatible)
                {
                    continue;
                }

                // 3. Employee chưa có lịch trùng thời gian
                var existingSchedules = await _unitOfWork.DailyScheduleRepository.WhereAsync(
                    ds => ds.EmployeeId == employee.Id
                        && ds.Date.Date == leaveDate.Date
                        && ds.Status != DailyScheduleStatusConstant.EXCUSED,
                    includeFunc: q => q.Include(ds => ds.WorkShift)
                );

                var hasConflict = existingSchedules.Any(ds =>
                {
                    var existingShift = ds.WorkShift;
                    if (existingShift == null) return false;

                    return existingShift.StartTime < shift.EndTime
                        && existingShift.EndTime > shift.StartTime;
                });

                if (hasConflict)
                {
                    continue;
                }

                // Tính workload (số ca đã assign trong ngày/tuần)
                var workload = existingSchedules.Count;

                candidates.Add(new Candidate
                {
                    EmployeeId = employee.Id,
                    Employee = employee,
                    WorkShiftId = optShift.WorkShiftId,
                    Priority = optShift.Priority,
                    Workload = workload
                });
            }
        }

        if (candidates.Count == 0)
        {
            return null; // Không tìm thấy candidate phù hợp
        }

        // Chọn employee tốt nhất:
        // 1. Ưu tiên Priority cao
        // 2. Ưu tiên Workload thấp
        var bestCandidate = candidates
            .OrderByDescending(c => c.Priority)
            .ThenBy(c => c.Workload)
            .FirstOrDefault();

        return bestCandidate?.Employee;
    }

    private class Candidate
    {
        public Guid EmployeeId { get; set; }
        public Employee? Employee { get; set; }
        public Guid WorkShiftId { get; set; }
        public int Priority { get; set; }
        public int Workload { get; set; }
    }
}

