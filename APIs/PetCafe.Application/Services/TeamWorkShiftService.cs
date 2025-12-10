using PetCafe.Application.Models.TeamWorkShiftModels;
using PetCafe.Domain.Entities;
using PetCafe.Application.GlobalExceptionHandling.Exceptions;
using Microsoft.EntityFrameworkCore;
using PetCafe.Application.Models.ShareModels;
using PetCafe.Domain.Constants;
using Hangfire;

namespace PetCafe.Application.Services;

public interface ITeamWorkShiftService
{
    Task<bool> AssignWorkShift(Guid teamId, TeamWorkShiftCreateModel model);
    Task<BasePagingResponseModel<TeamWorkShift>> GetTeamWorkShift(Guid teamId, FilterQuery query);

    Task<bool> RemoveWorkShift(Guid teamWorkShiftId);
}

public class TeamWorkShiftService(
    IUnitOfWork _unitOfWork,
    IDailyScheduleService _dailyScheduleService,
    IBackgroundJobClient _backgroundJobClient
) : ITeamWorkShiftService
{
    #region  AssignWorkShift
    public async Task<bool> AssignWorkShift(Guid teamId, TeamWorkShiftCreateModel model)
    {
        // 1. Kiểm tra xem team có tồn tại không
        var team = await _unitOfWork.TeamRepository.GetByIdAsync(
            teamId,
            includeFunc: q => q.Include(t => t.TeamMembers.Where(tm => !tm.IsDeleted))
                              .ThenInclude(tm => tm.Employee)
        ) ?? throw new NotFoundException($"Nhóm với ID {teamId} không tồn tại");

        // 1.1. Kiểm tra team có đang active không
        if (!team.IsActive)
        {
            throw new BadRequestException($"Nhóm {team.Name} hiện không đang hoạt động");
        }

        var activeMembers = team.TeamMembers
            .Where(tm => !tm.IsDeleted && tm.Employee != null && tm.Employee.IsActive)
            .ToList();

        if (activeMembers.Count == 0)
        {
            throw new BadRequestException("Nhóm hiện không có thành viên nào đang hoạt động");
        }

        // 2. Kiểm tra xem các work shifts có tồn tại không
        var modelWorkShiftIds = model.WorkShiftIds;
        if (modelWorkShiftIds.Count == 0)
        {
            throw new BadRequestException("Danh sách ca làm việc không được để trống");
        }

        // 2.1. Kiểm tra duplicate IDs trong input
        var distinctWorkShiftIds = modelWorkShiftIds.Distinct().ToList();
        if (distinctWorkShiftIds.Count != modelWorkShiftIds.Count)
        {
            throw new BadRequestException("Danh sách ca làm việc chứa ID trùng lặp");
        }

        var workShifts = await _unitOfWork.WorkShiftRepository.WhereAsync(
            ws => modelWorkShiftIds.Contains(ws.Id)
        );

        if (workShifts.Count != modelWorkShiftIds.Count)
        {
            throw new BadRequestException("Một hoặc nhiều ca làm việc không tồn tại hoặc không còn hoạt động");
        }

        // Kiểm tra xem team đã có work shift nào chưa và có trùng với work shift mới không
        var existingTeamWorkShifts = await _unitOfWork.TeamWorkShiftRepository.WhereAsync(
            tws => tws.TeamId == teamId && !tws.IsDeleted,
            includeFunc: q => q.Include(tws => tws.WorkShift)
        );

        if (existingTeamWorkShifts.Count != 0)
        {
            // Kiểm tra xem có work shift nào trong danh sách mới trùng với work shift đã tồn tại không
            var existingWorkShiftIds = existingTeamWorkShifts.Select(tws => tws.WorkShiftId).ToList();
            var duplicateWorkShiftIds = modelWorkShiftIds.Intersect(existingWorkShiftIds).ToList();

            if (duplicateWorkShiftIds.Count != 0)
            {
                var duplicateWorkShiftNames = existingTeamWorkShifts
                    .Where(tws => duplicateWorkShiftIds.Contains(tws.WorkShiftId))
                    .Select(tws => tws.WorkShift.Name)
                    .ToList();

                throw new BadRequestException($"Các ca làm việc sau đã được gán cho nhóm: {string.Join(", ", duplicateWorkShiftNames)}");
            }

        }

        // 2.2. Validate các work shifts trước khi sử dụng
        foreach (var workShift in workShifts)
        {
            if (workShift.StartTime >= workShift.EndTime)
            {
                throw new BadRequestException($"Ca làm việc {workShift.Name} có thời gian không hợp lệ (StartTime phải nhỏ hơn EndTime)");
            }

            if (workShift.ApplicableDays == null || workShift.ApplicableDays.Count == 0)
            {
                throw new BadRequestException($"Ca làm việc {workShift.Name} không có ngày áp dụng");
            }
        }

        // 3. Kiểm tra xem các work shifts có bị trùng thời gian không
        CheckWorkShiftTimeOverlap(workShifts);

        // 4. Kiểm tra trùng lịch của các thành viên trước khi assign (kiểm tra qua tất cả các team)
        var teamMembers = team.TeamMembers.ToList();

        // 4.0. Lọc ra các team members có employee không bị deleted
        teamMembers = teamMembers.Where(tm => tm.Employee != null && !tm.Employee.IsDeleted).ToList();

        if (teamMembers.Count != 0)
        {
            // Lấy tất cả các work shifts hiện tại của nhân viên trong tất cả các team
            foreach (var member in teamMembers)
            {
                var employeeId = member.EmployeeId;

                // Lấy tất cả các DailySchedule hiện tại của nhân viên (từ tất cả các team)
                var existingDailySchedules = await _unitOfWork.DailyScheduleRepository.WhereAsync(
                    ds => ds.EmployeeId == employeeId && !ds.IsDeleted,
                    includeFunc: q => q.Include(ds => ds.WorkShift).Include(ds => ds.TeamMember)
                );

                foreach (var workShift in workShifts)
                {
                    // Với mỗi ngày trong tuần mà work shift có hiệu lực
                    var today = DateTime.UtcNow.Date;

                    // Tính ngày đầu tuần (Thứ 2) và cuối tuần (Chủ nhật)
                    // Nếu hôm nay là Chủ nhật, lấy thứ 2 tuần này (không phải tuần sau)
                    var daysFromMonday = ((int)today.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
                    var startOfWeek = today.AddDays(-daysFromMonday);
                    var endOfWeek = startOfWeek.AddDays(6);

                    for (var date = today; date <= endOfWeek; date = date.AddDays(1))
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
                                .Where(ws => ws != null)
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
        await _unitOfWork.SaveChangesAsync();

        // 6. Tạo DailySchedule cho tất cả các thành viên trong team đến cuối tháng (chạy background)
        if (teamMembers.Count != 0)
        {
            var today = DateTime.UtcNow.Date;

            // Tính ngày cuối tháng
            var endOfMonth = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));

            // Lấy các IDs để truyền vào background job
            var teamMemberIds = teamMembers.Select(tm => tm.Id).ToList();
            var workShiftIdsForJob = workShifts.Select(ws => ws.Id).ToList();

            // Enqueue background job để tạo DailySchedule (từ hôm nay đến cuối tháng)
            _backgroundJobClient.Enqueue(() => _dailyScheduleService.CreateDailySchedulesForMembersBackgroundAsync(
                teamMemberIds,
                workShiftIdsForJob,
                today,
                endOfMonth,
                false
            ));
        }

        return true;
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

    public async Task<BasePagingResponseModel<TeamWorkShift>> GetTeamWorkShift(Guid teamId, FilterQuery query)
    {

        var (Pagination, Entities) = await _unitOfWork.TeamWorkShiftRepository.ToPagination(
            pageIndex: query.Page ?? 0,
            pageSize: query.Limit ?? 10,
            filter: x => x.TeamId == teamId,
            searchTerm: query.Q,
            searchFields: ["Name", "Description"],
            sortOrders: query.OrderBy?.ToDictionary(
                    k => k.OrderColumn ?? "CreatedAt",
                    v => (v.OrderDir ?? "ASC").Equals("ASC", StringComparison.CurrentCultureIgnoreCase)
                ) ?? new Dictionary<string, bool> { { "CreatedAt", false } }
            , includeFunc: x => x.Include(x => x.WorkShift)
        );
        return BasePagingResponseModel<TeamWorkShift>.CreateInstance(Entities, Pagination);
    }

    public async Task<bool> RemoveWorkShift(Guid teamWorkShiftId)
    {
        var teamWorkShift = await _unitOfWork.TeamWorkShiftRepository.FirstOrDefaultAsync(
            x => x.Id == teamWorkShiftId,
            includeFunc: q => q.Include(tws => tws.Team)
                              .ThenInclude(t => t.TeamMembers.Where(tm => !tm.IsDeleted))
        ) ?? throw new NotFoundException("Không tìm thấy ca làm việc của nhóm");

        // Xóa DailySchedule của các thành viên trong team đó liên quan tới work shift này
        var teamMemberIds = teamWorkShift.Team.TeamMembers.Select(tm => tm.Id).ToList();
        if (teamMemberIds.Count > 0)
        {
            var dailySchedules = await _unitOfWork.DailyScheduleRepository.WhereAsync(
                ds => teamMemberIds.Contains(ds.TeamMemberId)
                    && ds.WorkShiftId == teamWorkShift.WorkShiftId
            );
            if (dailySchedules.Count > 0)
                _unitOfWork.DailyScheduleRepository.SoftRemoveRange(dailySchedules);

        }

        _unitOfWork.TeamWorkShiftRepository.SoftRemove(teamWorkShift);
        return await _unitOfWork.SaveChangesAsync();
    }
}