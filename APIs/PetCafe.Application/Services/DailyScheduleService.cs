using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PetCafe.Application.Models.DailyScheduleModels;
using PetCafe.Application.Models.ShareModels;
using PetCafe.Application.Utilities;
using PetCafe.Domain.Constants;
using PetCafe.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace PetCafe.Application.Services;

public interface IDailyScheduleService
{
    Task AutoAssignSchedulesAsync();
    Task<BasePagingResponseModel<DailySchedule>> GetDailySchedulesAsync(DailyScheduleFilterQuery query);
    // Task AutoChangeStatusAsync();
}


public class DailyScheduleService(IUnitOfWork _unitOfWork) : IDailyScheduleService
{
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
                                  .ThenInclude(t => t.TeamMembers.Where(tm => tm.IsActive && !tm.IsDeleted))
                                      .ThenInclude(tm => tm.Employee)
        );

        var dailySchedules = new List<DailySchedule>();

        foreach (var teamWorkShift in teamWorkShifts)
        {
            var team = teamWorkShift.Team;
            var workShift = teamWorkShift.WorkShift;

            // Lấy tất cả các team members active trong team này
            var teamMembers = team.TeamMembers
                .Where(tm => tm.IsActive && !tm.IsDeleted)
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
        Expression<Func<DailySchedule, bool>> filter = x => true;
        if (query.TeamId.HasValue)
        {
            filter = filter != null ? FilterCustoms.CombineFilters(filter, x => x.TeamMemberId == query.TeamId.Value) : x => x.TeamMemberId == query.TeamId.Value;
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
        var (Pagination, Entities) = await _unitOfWork.DailyScheduleRepository.ToPagination(
            pageIndex: query.Page ?? 0,
            pageSize: query.Limit ?? 10,
            filter: filter,
            includeFunc: q => q.Include(ds => ds.WorkShift)
                              .Include(ds => ds.Employee)
                              .Include(ds => ds.TeamMember)
        );
        return BasePagingResponseModel<DailySchedule>.CreateInstance(Entities, Pagination);
    }
}