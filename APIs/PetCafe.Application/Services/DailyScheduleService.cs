using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PetCafe.Application.Models.DailyScheduleModels;
using PetCafe.Application.Models.ShareModels;
using PetCafe.Application.Utilities;
using PetCafe.Domain.Constants;
using PetCafe.Domain.Entities;
using PetCafe.Domain.Models;
using Hangfire;
using Task = System.Threading.Tasks.Task;
using PetCafe.Application.Services.Commons;
using PetCafe.Application.GlobalExceptionHandling.Exceptions;

namespace PetCafe.Application.Services;

public interface IDailyScheduleService
{
    Task AutoAssignSchedulesAsync();
    Task<BasePagingResponseModel<DailySchedule>> GetDailySchedulesAsync(DailyScheduleFilterQuery query);
    Task<List<DailySchedule>> CreateDailySchedulesForMembersAsync(List<TeamMember> teamMembers, List<WorkShift> workShifts, DateTime startDate, DateTime endDate, bool checkTimeOverlap = false);
    Task CreateDailySchedulesForMembersBackgroundAsync(List<Guid> teamMemberIds, List<Guid> workShiftIds, DateTime startDate, DateTime endDate, bool checkTimeOverlap);
    Task<List<DailySchedule>> UpdateAsync(Guid teamId, List<DailyScheduleUpdateModel> models);
    // Task AutoChangeStatusAsync();

}


public class DailyScheduleService(IUnitOfWork _unitOfWork, IClaimsService _claimsService) : IDailyScheduleService
{

    public async Task<List<DailySchedule>> UpdateAsync(Guid teamId, List<DailyScheduleUpdateModel> models)
    {

        var team = await _unitOfWork.TeamRepository.GetByIdAsync(teamId) ?? throw new BadRequestException("Team không tồn tại");
        if (team.LeaderId != _claimsService.GetCurrentUser && _claimsService.GetCurrentUserRole != RoleConstants.MANAGER)
        {
            throw new BadRequestException("Bạn không có quyền cập nhật lịch làm việc của nhóm này!");
        }
        var dailySchedules = await _unitOfWork.DailyScheduleRepository.WhereAsync(
            ds => models.Select(m => m.Id).Contains(ds.Id)
        );
        foreach (var model in models)
        {
            var dailySchedule = dailySchedules.FirstOrDefault(ds => ds.Id == model.Id);
            if (dailySchedule != null)
            {
                _unitOfWork.Mapper.Map(model, dailySchedule);
                _unitOfWork.DailyScheduleRepository.Update(dailySchedule);
            }
        }
        await _unitOfWork.SaveChangesAsync();
        return [.. dailySchedules];
    }

    public async Task AutoAssignSchedulesAsync()
    {
        // Lấy ngày bắt đầu của tháng kế tiếp
        var today = DateTime.UtcNow.Date;
        var nextMonth = today.AddMonths(1);
        var startOfNextMonth = new DateTime(nextMonth.Year, nextMonth.Month, 1);
        var endOfNextMonth = new DateTime(nextMonth.Year, nextMonth.Month, DateTime.DaysInMonth(nextMonth.Year, nextMonth.Month));

        // Lấy tất cả các TeamWorkShift (chỉ kiểm tra IsDeleted vì TeamWorkShift không có IsActive)
        var teamWorkShifts = await _unitOfWork.TeamWorkShiftRepository.WhereAsync(
            tws => !tws.IsDeleted,
            includeFunc: q => q.Include(tws => tws.WorkShift)
                              .Include(tws => tws.Team)
                                  .ThenInclude(t => t.TeamMembers.Where(tm => !tm.IsDeleted))
                                      .ThenInclude(tm => tm.Employee)
        );

        var dailySchedules = new List<DailySchedule>();

        foreach (var teamWorkShift in teamWorkShifts)
        {
            var team = teamWorkShift.Team;
            var workShift = teamWorkShift.WorkShift;

            // Lấy tất cả các team members active trong team này
            var teamMembers = team.TeamMembers
                .Where(tm => !tm.IsDeleted)
                .ToList();

            if (teamMembers.Count == 0)
                continue;

            // Tạo lịch cho từng team member
            foreach (var member in teamMembers)
            {
                // Tạo lịch cho mỗi ngày trong tháng kế tiếp
                for (var date = startOfNextMonth; date <= endOfNextMonth; date = date.AddDays(1))
                {
                    var dayOfWeek = date.DayOfWeek.ToString().ToUpper();

                    // Kiểm tra xem work shift có áp dụng cho ngày này không
                    if (!workShift.ApplicableDays.Contains(dayOfWeek))
                    {
                        continue;
                    }

                    // Kiểm tra xem nhân viên đã có lịch vào ngày này từ team khác chưa
                    var existingSchedule = await _unitOfWork.DailyScheduleRepository.FirstOrDefaultAsync(
                        ds => ds.EmployeeId == member.EmployeeId
                            && ds.Date.Date == date,
                        includeFunc: q => q.Include(ds => ds.WorkShift)
                    );

                    if (existingSchedule != null && existingSchedule.WorkShift != null)
                    {
                        // Kiểm tra xem có bị trùng thời gian không
                        var isTimeOverlapping = existingSchedule.WorkShift.StartTime < workShift.EndTime
                            && existingSchedule.WorkShift.EndTime > workShift.StartTime;

                        if (isTimeOverlapping)
                        {
                            // Nhân viên đã có lịch trùng thời gian, bỏ qua
                            continue;
                        }
                    }

                    // Kiểm tra xem đã có lịch trong team này chưa (để tránh duplicate)
                    var existingScheduleInTeam = await _unitOfWork.DailyScheduleRepository.FirstOrDefaultAsync(
                        ds => ds.TeamMemberId == member.Id
                            && ds.WorkShiftId == workShift.Id
                            && ds.Date.Date == date
                    );

                    if (existingScheduleInTeam != null)
                    {
                        continue;
                    }

                    // Tạo daily schedule mới
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

        // Thêm tất cả daily schedules vào database
        if (dailySchedules.Count != 0)
        {
            await _unitOfWork.DailyScheduleRepository.AddRangeAsync(dailySchedules);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task<BasePagingResponseModel<DailySchedule>> GetDailySchedulesAsync(DailyScheduleFilterQuery query)
    {
        // Lấy tất cả các team members của team
        Expression<Func<DailySchedule, bool>> filter = x => x.Employee.IsActive;

        // Note: Không thể filter ApplicableDays trong EF Core expression vì Contains với DayOfWeek.ToString() không được translate
        // Sẽ filter sau khi load dữ liệu

        if (query.TeamId.HasValue)
        {
            filter = filter != null ? FilterCustoms.CombineFilters(filter, x => x.TeamMember.TeamId == query.TeamId.Value) : x => x.TeamMember.TeamId == query.TeamId.Value;
        }

        if (query.FromDate.HasValue)
        {
            filter = filter != null ? FilterCustoms.CombineFilters(filter, x => x.Date >= query.FromDate.Value) : x => x.Date >= query.FromDate.Value;
        }
        if (query.ToDate.HasValue)
        {
            filter = filter != null ? FilterCustoms.CombineFilters(filter, x => x.Date <= query.ToDate.Value) : x => x.Date <= query.ToDate.Value;
        }
        if (!string.IsNullOrEmpty(query.Status))
        {
            filter = filter != null ? FilterCustoms.CombineFilters(filter, x => x.Status == query.Status) : x => x.Status == query.Status;
        }

        // Load tất cả records matching các filter (không có ApplicableDays filter)
        var allEntities = await _unitOfWork.DailyScheduleRepository.WhereAsync(
            filter,
            includeFunc: q => q.Include(ds => ds.WorkShift)
                              .Include(ds => ds.Employee)
                              .Include(ds => ds.TeamMember)
        );

        // Filter theo ApplicableDays trong memory
        var filteredEntities = allEntities
            .Where(ds => ds.WorkShift != null &&
                        ds.WorkShift.ApplicableDays.Contains(ds.Date.DayOfWeek.ToString().ToUpper()))
            .ToList();

        // Apply pagination thủ công
        var pageIndex = query.Page ?? 0;
        var pageSize = query.Limit ?? 10;
        var totalCount = filteredEntities.Count;
        var paginatedEntities = filteredEntities
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToList();

        var pagination = new Pagination
        {
            PageIndex = pageIndex,
            PageSize = pageSize,
            TotalItemsCount = totalCount
        };

        return BasePagingResponseModel<DailySchedule>.CreateInstance(paginatedEntities, pagination);
    }
    public async Task<List<DailySchedule>> CreateDailySchedulesForMembersAsync(
        List<TeamMember> teamMembers,
        List<WorkShift> workShifts,
        DateTime startDate,
        DateTime endDate,
        bool checkTimeOverlap = false)
    {
        if (teamMembers.Count == 0 || workShifts.Count == 0)
            return [];

        var dailySchedules = new List<DailySchedule>();

        // Lấy tất cả các team member IDs
        var teamMemberIds = teamMembers.Select(tm => tm.Id).ToList();

        // Lấy tất cả các work shift IDs
        var workShiftIds = workShifts.Select(ws => ws.Id).ToList();

        // Lấy tất cả các daily schedules hiện có cho team members này trong khoảng thời gian
        var existingSchedules = await _unitOfWork.DailyScheduleRepository.WhereAsync(
            ds => teamMemberIds.Contains(ds.TeamMemberId)
                && workShiftIds.Contains(ds.WorkShiftId)
                && ds.Date >= startDate
                && ds.Date <= endDate
        );

        foreach (var member in teamMembers)
        {
            var employeeId = member.EmployeeId;

            // Nếu cần kiểm tra trùng thời gian, lấy tất cả các DailySchedule của nhân viên
            List<DailySchedule>? employeeExistingSchedules = null;
            if (checkTimeOverlap)
            {
                employeeExistingSchedules = await _unitOfWork.DailyScheduleRepository.WhereAsync(
                    ds => ds.EmployeeId == employeeId,
                    includeFunc: q => q.Include(ds => ds.WorkShift)
                );
            }

            foreach (var workShift in workShifts)
            {
                // Tạo lịch cho mỗi ngày trong khoảng thời gian
                for (var date = startDate; date <= endDate; date = date.AddDays(1))
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

                    if (existingSchedule != null)
                    {
                        continue;
                    }

                    // Nếu cần kiểm tra trùng thời gian với các schedule từ team khác
                    if (checkTimeOverlap && employeeExistingSchedules != null)
                    {
                        // Kiểm tra xem nhân viên đã có lịch vào ngày này từ team khác chưa
                        var hasScheduleOnDate = employeeExistingSchedules
                            .Any(ds => ds.Date.Date == date);

                        if (hasScheduleOnDate)
                        {
                            // Lấy work shift của lịch hiện tại trong ngày đó
                            var existingSchedulesOnDate = employeeExistingSchedules
                                .Where(ds => ds.Date.Date == date)
                                .Select(ds => ds.WorkShift)
                                .ToList();

                            // Kiểm tra xem có bị trùng thời gian không
                            var isTimeOverlapping = existingSchedulesOnDate.Any(existingShift =>
                            {
                                var commonDays = existingShift.ApplicableDays.Intersect(workShift.ApplicableDays).ToList();
                                if (commonDays.Count == 0) return false;

                                return existingShift.StartTime < workShift.EndTime
                                    && existingShift.EndTime > workShift.StartTime;
                            });

                            if (isTimeOverlapping)
                            {
                                // Nhân viên đã có lịch trùng thời gian, bỏ qua
                                continue;
                            }
                        }
                    }

                    // Tạo daily schedule mới
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

        return dailySchedules;
    }

    [AutomaticRetry(Attempts = 3)]
    public async Task CreateDailySchedulesForMembersBackgroundAsync(
        List<Guid> teamMemberIds,
        List<Guid> workShiftIds,
        DateTime startDate,
        DateTime endDate,
        bool checkTimeOverlap)
    {
        if (teamMemberIds.Count == 0 || workShiftIds.Count == 0)
            return;

        // Lấy team members và work shifts từ database
        var teamMembers = await _unitOfWork.TeamMemberRepository.WhereAsync(
            tm => teamMemberIds.Contains(tm.Id)
        );

        var workShifts = await _unitOfWork.WorkShiftRepository.WhereAsync(
            ws => workShiftIds.Contains(ws.Id)
        );

        if (teamMembers.Count == 0 || workShifts.Count == 0)
            return;

        // Sử dụng helper method để tạo DailySchedule
        var dailySchedules = await CreateDailySchedulesForMembersAsync(
            teamMembers,
            workShifts,
            startDate,
            endDate,
            checkTimeOverlap
        );

        // Thêm daily schedules vào database nếu có
        if (dailySchedules.Count != 0)
        {
            await _unitOfWork.DailyScheduleRepository.AddRangeAsync(dailySchedules);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}