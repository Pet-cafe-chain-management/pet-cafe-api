using PetCafe.Application.Models.TeamWorkShiftModels;
using PetCafe.Domain.Entities;
using PetCafe.Application.GlobalExceptionHandling.Exceptions;
using Microsoft.EntityFrameworkCore;
using PetCafe.Application.Models.ShareModels;
using PetCafe.Domain.Constants;

namespace PetCafe.Application.Services;

public interface ITeamWorkShiftService
{
    Task<bool> AssignWorkShift(Guid teamId, TeamWorkShiftCreateModel model);
    Task<BasePagingResponseModel<WorkShift>> GetTeamWorkShift(Guid teamId, FilterQuery query);

    Task<bool> RemoveWorkShift(Guid teamWorkShiftId);
}

public class TeamWorkShiftService(
    IUnitOfWork _unitOfWork
) : ITeamWorkShiftService
{
    #region  AssignWorkShift
    public async Task<bool> AssignWorkShift(Guid teamId, TeamWorkShiftCreateModel model)
    {
        // 1. Kiểm tra xem team có tồn tại không
        var team = await _unitOfWork.TeamRepository.GetByIdAsync(
            teamId,
            includeFunc: q => q.Include(t => t.TeamMembers)
                              .ThenInclude(tm => tm.Employee)
        ) ?? throw new NotFoundException($"Team với ID {teamId} không tồn tại");

        // 2. Kiểm tra xem các work shifts có tồn tại không
        var workShiftIds = model.WorkShiftIds;
        if (workShiftIds.Count == 0)
        {
            throw new BadRequestException("Danh sách ca làm việc không được để trống");
        }

        var workShifts = await _unitOfWork.WorkShiftRepository.WhereAsync(
            ws => workShiftIds.Contains(ws.Id) && ws.IsActive
        );

        if (workShifts.Count != workShiftIds.Count)
        {
            throw new BadRequestException("Một hoặc nhiều ca làm việc không tồn tại hoặc không còn hoạt động");
        }

        // Kiểm tra xem team đã có work shift nào chưa và có trùng với work shift mới không
        var existingTeamWorkShifts = await _unitOfWork.TeamWorkShiftRepository.WhereAsync(
            tws => tws.TeamId == teamId,
            includeFunc: q => q.Include(tws => tws.WorkShift)
        );

        if (existingTeamWorkShifts.Count != 0)
        {
            // Kiểm tra xem có work shift nào trong danh sách mới trùng với work shift đã tồn tại không
            var existingWorkShiftIds = existingTeamWorkShifts.Select(tws => tws.WorkShiftId).ToList();
            var duplicateWorkShiftIds = workShiftIds.Intersect(existingWorkShiftIds).ToList();

            if (duplicateWorkShiftIds.Count != 0)
            {
                var duplicateWorkShiftNames = existingTeamWorkShifts
                    .Where(tws => duplicateWorkShiftIds.Contains(tws.WorkShiftId))
                    .Select(tws => tws.WorkShift.Name)
                    .ToList();

                throw new BadRequestException($"Các ca làm việc sau đã được gán cho nhóm: {string.Join(", ", duplicateWorkShiftNames)}");
            }

        }

        // 3. Kiểm tra xem các work shifts có bị trùng thời gian không
        CheckWorkShiftTimeOverlap(workShifts);

        // 4. Kiểm tra trùng lịch của các thành viên trước khi assign (kiểm tra qua tất cả các team)
        var teamMembers = team.TeamMembers.Where(tm => tm.IsActive).ToList();
        if (teamMembers.Count != 0)
        {
            // Lấy tất cả các work shifts hiện tại của nhân viên trong tất cả các team
            foreach (var member in teamMembers)
            {
                var employeeId = member.EmployeeId;

                // Lấy tất cả các DailySchedule hiện tại của nhân viên (từ tất cả các team)
                var existingDailySchedules = await _unitOfWork.DailyScheduleRepository.WhereAsync(
                    ds => ds.EmployeeId == employeeId,
                    includeFunc: q => q.Include(ds => ds.WorkShift).Include(ds => ds.TeamMember)
                );

                foreach (var workShift in workShifts)
                {
                    // Với mỗi ngày trong tháng mà work shift có hiệu lực
                    var today = DateTime.UtcNow.Date;
                    var endOfMonth = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));

                    for (var date = today; date <= endOfMonth; date = date.AddDays(1))
                    {
                        var dayOfWeek = date.DayOfWeek.ToString().ToUpper();

                        // Kiểm tra xem work shift có áp dụng cho ngày này không
                        if (!workShift.ApplicableDays.Contains(dayOfWeek))
                        {
                            continue;
                        }

                        // Kiểm tra xem nhân viên đã có lịch vào ngày này chưa (từ bất kỳ team nào)
                        var hasScheduleOnDate = existingDailySchedules
                            .Any(ds => ds.Date.Date == date);

                        if (hasScheduleOnDate)
                        {
                            // Lấy work shift của lịch hiện tại trong ngày đó
                            var existingSchedulesOnDate = existingDailySchedules
                                .Where(ds => ds.Date.Date == date)
                                .Select(ds => ds.WorkShift)
                                .ToList();

                            // Kiểm tra xem có bị trùng thời gian không
                            if (IsTimeOverlap(existingSchedulesOnDate, workShift))
                            {
                                throw new BadRequestException($"Nhân viên {member.Employee.FullName} đã có lịch trùng với ca làm việc {workShift.Name} vào ngày {date:dd/MM/yyyy}");
                            }
                        }
                    }
                }
            }
        }

        // 5. Tạo các TeamWorkShift mới
        var newTeamWorkShifts = workShifts.Select(ws => new TeamWorkShift
        {
            TeamId = teamId,
            WorkShiftId = ws.Id
        }).ToList();

        await _unitOfWork.TeamWorkShiftRepository.AddRangeAsync(newTeamWorkShifts);

        // 6. Tạo DailySchedule cho tất cả các thành viên trong team đến cuối tháng
        if (teamMembers.Count != 0)
        {
            var dailySchedules = new List<DailySchedule>();
            var today = DateTime.UtcNow.Date;
            var endOfMonth = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));

            // Lấy tất cả các team member IDs
            var teamMemberIds = teamMembers.Select(tm => tm.Id).ToList();

            // Lấy tất cả các work shift IDs
            var workShiftIdsToAssign = workShifts.Select(ws => ws.Id).ToList();

            // Lấy tất cả các daily schedules hiện có cho team members này
            var existingSchedules = await _unitOfWork.DailyScheduleRepository.WhereAsync(
                ds => teamMemberIds.Contains(ds.TeamMemberId)
                    && workShiftIdsToAssign.Contains(ds.WorkShiftId)
                    && ds.Date >= today
                    && ds.Date <= endOfMonth
            );

            foreach (var member in teamMembers)
            {
                foreach (var workShift in workShifts)
                {
                    // Tạo lịch cho mỗi ngày từ hôm nay đến cuối tháng
                    for (var date = today; date <= endOfMonth; date = date.AddDays(1))
                    {
                        var dayOfWeek = date.DayOfWeek.ToString().ToUpper();

                        // Kiểm tra xem work shift có áp dụng cho ngày này không
                        if (!workShift.ApplicableDays.Contains(dayOfWeek))
                        {
                            continue;
                        }

                        // Kiểm tra xem đã có lịch chưa (để tránh duplicate)
                        var existingSchedule = existingSchedules
                            .FirstOrDefault(ds => ds.TeamMemberId == member.Id
                                && ds.WorkShiftId == workShift.Id
                                && ds.Date.Date == date);

                        if (existingSchedule == null)
                        {
                            dailySchedules.Add(new DailySchedule
                            {
                                TeamMemberId = member.Id,
                                EmployeeId = member.EmployeeId,
                                WorkShiftId = workShift.Id,
                                Date = date,
                                Status = DailyScheduleStatusConstant.PENDING
                            });
                        }
                    }
                }
            }

            // Thêm daily schedules vào database nếu có
            if (dailySchedules.Count != 0)
            {
                await _unitOfWork.DailyScheduleRepository.AddRangeAsync(dailySchedules);
            }
        }

        // Lưu thay đổi
        return await _unitOfWork.SaveChangesAsync();
    }

    private static void CheckWorkShiftTimeOverlap(List<WorkShift> workShifts)
    {
        for (int i = 0; i < workShifts.Count - 1; i++)
        {
            for (int j = i + 1; j < workShifts.Count; j++)
            {
                var shift1 = workShifts[i];
                var shift2 = workShifts[j];
                // Kiểm tra ngày áp dụng có trùng nhau không
                var commonDays = shift1.ApplicableDays.Intersect(shift2.ApplicableDays).ToList();
                if (commonDays.Count == 0)
                {
                    continue; // Không có ngày nào trùng nhau
                }

                // Kiểm tra thời gian có trùng nhau không
                if (IsTimeOverlapping(shift1.StartTime, shift1.EndTime, shift2.StartTime, shift2.EndTime))
                {
                    throw new BadRequestException($"Các ca làm việc {shift1.Name} và {shift2.Name} bị trùng thời gian vào các ngày: {string.Join(", ", commonDays)}");
                }
            }
        }
    }

    private static bool IsTimeOverlapping(TimeSpan start1, TimeSpan end1, TimeSpan start2, TimeSpan end2)
    {
        // Nếu một ca kết thúc trước khi ca kia bắt đầu, thì không trùng
        if (end1 <= start2 || end2 <= start1)
        {
            return false;
        }
        return true;
    }

    private static bool IsTimeOverlap(List<WorkShift> existingWorkShifts, WorkShift newWorkShift)
    {
        foreach (var existingShift in existingWorkShifts)
        {
            // Kiểm tra ngày áp dụng có trùng nhau không
            var commonDays = existingShift.ApplicableDays.Intersect(newWorkShift.ApplicableDays).ToList();
            if (commonDays.Count == 0)
            {
                continue; // Không có ngày nào trùng nhau
            }

            // Kiểm tra thời gian có trùng nhau không
            if (IsTimeOverlapping(existingShift.StartTime, existingShift.EndTime,
                                 newWorkShift.StartTime, newWorkShift.EndTime))
            {
                return true;
            }
        }
        return false;
    }


    #endregion

    public async Task<BasePagingResponseModel<WorkShift>> GetTeamWorkShift(Guid teamId, FilterQuery query)
    {

        var (Pagination, Entities) = await _unitOfWork.WorkShiftRepository.ToPagination(
            pageIndex: query.Page ?? 0,
            pageSize: query.Limit ?? 10,
            filter: x => x.TeamWorkShifts.Any(x => x.TeamId == teamId && !x.IsDeleted),
            searchTerm: query.Q,
            searchFields: ["Name", "Description"],
            sortOrders: query.OrderBy?.ToDictionary(
                    k => k.OrderColumn ?? "CreatedAt",
                    v => (v.OrderDir ?? "ASC").Equals("ASC", StringComparison.CurrentCultureIgnoreCase)
                ) ?? new Dictionary<string, bool> { { "CreatedAt", false } }
            );
        return BasePagingResponseModel<WorkShift>.CreateInstance(Entities, Pagination);
    }

    public async Task<bool> RemoveWorkShift(Guid teamWorkShiftId)
    {
        var teamWorkShift = await _unitOfWork.TeamWorkShiftRepository.FirstOrDefaultAsync(x => x.Id == teamWorkShiftId) ?? throw new NotFoundException("Không tìm thấy ca làm việc của nhóm");
        _unitOfWork.TeamWorkShiftRepository.SoftRemove(teamWorkShift);
        return await _unitOfWork.SaveChangesAsync();
    }
}