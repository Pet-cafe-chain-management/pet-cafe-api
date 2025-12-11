using System.Linq.Expressions;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using PetCafe.Application.GlobalExceptionHandling.Exceptions;
using PetCafe.Application.Models.DailyTaskModels;
using PetCafe.Application.Models.ShareModels;
using PetCafe.Application.Models.SlotModels;
using PetCafe.Application.Models.TeamModels;
using PetCafe.Application.Utilities;
using PetCafe.Domain.Entities;
using PetCafe.Domain.Constants;

namespace PetCafe.Application.Services;

public interface ITeamService
{
    Task<Team> GetByIdAsync(Guid id);
    Task<BasePagingResponseModel<Team>> GetAllPagingAsync(TeamFilterQuery query);
    Task<Team> CreateAsync(TeamCreateModel model);
    Task<Team> UpdateAsync(Guid id, TeamUpdateModel model);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> DeleteWorkTypeAsync(Guid id);
    Task<List<WorkType>> GetWorkTypeNotInTeamAsync(Guid teamId);

    Task<BasePagingResponseModel<Slot>> GetSlotsByTeamIdAsync(Guid teamId, SlotFilterQuery query);
    Task<BasePagingResponseModel<DailyTask>> GetDailyTasksByTeamIdAsync(Guid teamId, DailyTaskFilterQuery query);
    Task<List<TeamMember>> GetMembersByTeamIdAsync(Guid teamId);
    Task<bool> AddMemeberToTeam(List<MemberCreateModel> models, Guid id);
    Task<bool> RemoveMemberFromTeam(Guid teamMemberId);
}


public class TeamService(
    IUnitOfWork _unitOfWork,
    IBackgroundJobClient _backgroundJobClient,
    IDailyScheduleService _dailyScheduleService
) : ITeamService
{
    public async Task<List<WorkType>> GetWorkTypeNotInTeamAsync(Guid teamId)
    {
        return await _unitOfWork.WorkTypeRepository.WhereAsync(x => !x.TeamWorkTypes.Any(y => y.TeamId == teamId));
    }

    public async Task<Team> CreateAsync(TeamCreateModel model)
    {
        // Validate LeaderId nếu có
        if (model.LeaderId.HasValue)
        {
            var leader = await _unitOfWork.EmployeeRepository.GetByIdAsync(model.LeaderId.Value)
                ?? throw new BadRequestException("Không tìm thấy thông tin leader!");
            if (!leader.IsActive)
            {
                throw new BadRequestException($"Nhân viên {leader.FullName} hiện không đang hoạt động!");
            }
        }

        var team = _unitOfWork.Mapper.Map<Team>(model);

        // Tạo TeamMember cho Leader nếu LeaderId không null
        if (model.LeaderId.HasValue)
        {
            await _unitOfWork.TeamMemberRepository.AddAsync(new TeamMember
            {
                TeamId = team.Id,
                EmployeeId = model.LeaderId.Value
            });
        }

        foreach (var workTypeId in model.WorkTypeIds ?? [])
        {
            await _unitOfWork.TeamWorkTypeRepository.AddAsync(new TeamWorkType
            {
                TeamId = team.Id,
                WorkTypeId = workTypeId
            });
        }
        await _unitOfWork.TeamRepository.AddAsync(team);
        await _unitOfWork.SaveChangesAsync();
        return team;
    }

    public async Task<Team> UpdateAsync(Guid id, TeamUpdateModel model)
    {
        var team = await _unitOfWork.TeamRepository.GetByIdAsync(
            id,
            includeFunc: x => x.Include(t => t.TeamMembers.Where(tm => !tm.IsDeleted))
        ) ?? throw new BadRequestException("Không tìm thấy thông tin!");

        var oldLeaderId = team.LeaderId;
        _unitOfWork.Mapper.Map(model, team);

        // Kiểm tra nếu LeaderId bị thay đổi
        if (model.LeaderId.HasValue && model.LeaderId.Value != oldLeaderId)
        {
            // Validate LeaderId mới
            var newLeader = await _unitOfWork.EmployeeRepository.GetByIdAsync(model.LeaderId.Value)
                ?? throw new BadRequestException("Không tìm thấy thông tin leader!");
            if (!newLeader.IsActive)
            {
                throw new BadRequestException($"Nhân viên {newLeader.FullName} hiện không đang hoạt động!");
            }

            // Kiểm tra xem LeaderId mới đã có trong TeamMembers chưa
            var existingMember = team.TeamMembers.FirstOrDefault(tm => tm.EmployeeId == model.LeaderId.Value);
            if (existingMember == null)
            {
                // Nếu chưa có thì thêm vào TeamMembers
                await _unitOfWork.TeamMemberRepository.AddAsync(new TeamMember
                {
                    TeamId = team.Id,
                    EmployeeId = model.LeaderId.Value
                });
            }
        }

        foreach (var workTypeId in model.WorkTypeIds ?? [])
        {
            var existingWorkType = await _unitOfWork.TeamWorkTypeRepository.FirstOrDefaultAsync(x => x.WorkTypeId == workTypeId && x.TeamId == team.Id);
            if (existingWorkType != null) continue;
            await _unitOfWork.TeamWorkTypeRepository.AddAsync(new TeamWorkType
            {
                TeamId = team.Id,
                WorkTypeId = workTypeId
            });
        }

        _unitOfWork.TeamRepository.Update(team);
        await _unitOfWork.SaveChangesAsync();
        return team;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var slots = await _unitOfWork.SlotRepository.WhereAsync(x => x.TeamId == id);
        if (slots.Count > 0) throw new BadRequestException("Không thể xóa team có ca làm việc!");

        var team = await _unitOfWork.TeamRepository.GetByIdAsync(
            id,
            includeFunc: x => x.Include(t => t.TeamMembers.Where(tm => !tm.IsDeleted))
        ) ?? throw new BadRequestException("Không tìm thấy thông tin!");

        // Xóa DailySchedule của các thành viên liên quan tới team này
        var teamMemberIds = team.TeamMembers.Select(tm => tm.Id).ToList();
        if (teamMemberIds.Count > 0)
        {
            var dailySchedules = await _unitOfWork.DailyScheduleRepository.WhereAsync(
                ds => teamMemberIds.Contains(ds.TeamMemberId)
            );

            if (dailySchedules.Count > 0)
                _unitOfWork.DailyScheduleRepository.SoftRemoveRange(dailySchedules);
        }

        _unitOfWork.TeamRepository.SoftRemove(team);
        return await _unitOfWork.SaveChangesAsync();
    }

    public async Task<Team> GetByIdAsync(Guid id)
    {
        return await _unitOfWork.TeamRepository
            .GetByIdAsync(
                id,
                includeFunc: x => x
                    .Include(x => x.Leader!)
                    .Include(x => x.TeamMembers.Where(x => !x.IsDeleted)).ThenInclude(x => x.Employee)
                    .Include(x => x.TeamWorkTypes.Where(x => !x.IsDeleted)).ThenInclude(twt => twt.WorkType))
                ?? throw new BadRequestException("Không tìm thấy thông tin!");
    }

    public async Task<BasePagingResponseModel<Team>> GetAllPagingAsync(TeamFilterQuery query)
    {
        Expression<Func<Team, bool>> filter = x => x.IsActive == query.IsActive;

        if (query.StartWorkingTime != null && query.EndWorkingTime != null && query.WorkingDay != null && query.WorkTypeId != null)
        {
            Expression<Func<Team, bool>> tmp_filter = x =>
                x.TeamWorkShifts.Any(x => x.WorkShift.StartTime >= query.StartWorkingTime) &&
                x.TeamWorkShifts.Any(x => x.WorkShift.EndTime <= query.EndWorkingTime) &&
                x.TeamWorkShifts.Any(x => x.WorkShift.ApplicableDays.Contains(query.WorkingDay)) &&
                x.TeamWorkTypes.Any(x => x.WorkTypeId == query.WorkTypeId);
            filter = filter != null ? FilterCustoms.CombineFilters(filter, tmp_filter) : tmp_filter;
        }

        var (Pagination, Entities) = await _unitOfWork.TeamRepository.ToPagination(
            pageIndex: query.Page ?? 0,
           pageSize: query.Limit ?? 10,
           searchTerm: query.Q,
           searchFields: ["Name", "Description"],
           sortOrders: query.OrderBy?.ToDictionary(
                   k => k.OrderColumn ?? "CreatedAt",
                   v => (v.OrderDir ?? "ASC").Equals("ASC", StringComparison.CurrentCultureIgnoreCase)
               ) ?? new Dictionary<string, bool> { { "CreatedAt", false } },
           includeFunc: x => x.Include(x => x.Leader).Include(x => x.TeamWorkTypes.Where(x => !x.IsDeleted)).ThenInclude(twt => twt.WorkType)
       );
        return BasePagingResponseModel<Team>.CreateInstance(Entities, Pagination);
    }

    public async Task<bool> AddMemeberToTeam(List<MemberCreateModel> models, Guid id)
    {
        var team = await _unitOfWork.TeamRepository.GetByIdAsync(
            id,
            includeFunc: x => x
                .Include(t => t.TeamWorkShifts.Where(tws => !tws.IsDeleted))
                              .ThenInclude(tws => tws.WorkShift)
        ) ?? throw new BadRequestException("Không tìm thấy thông tin!");

        foreach (var model in models)
        {
            var employee = await _unitOfWork.EmployeeRepository.GetByIdAsync(model.EmployeeId) ?? throw new BadRequestException("Không tìm thấy nhân viên!");
            if (!employee.IsActive)
            {
                throw new BadRequestException($"Nhân viên {employee.FullName} hiện không đang hoạt động!");
            }
            if (team.TeamMembers.Any(x => x.EmployeeId == model.EmployeeId))
            {
                throw new BadRequestException($"Nhân viên {employee.FullName} đã tồn tại trong team!");
            }
        }

        var teamMembers = models.Select(x => new TeamMember
        {
            TeamId = id,
            EmployeeId = x.EmployeeId,
        }).ToList();

        await _unitOfWork.TeamMemberRepository.AddRangeAsync(teamMembers);
        await _unitOfWork.SaveChangesAsync();

        // Tạo DailySchedule cho các ngày còn lại trong tháng (chạy background)
        if (team.TeamWorkShifts.Count > 0 && teamMembers.Count > 0)
        {
            var today = DateTime.UtcNow.Date;

            // Tính ngày cuối tháng
            var endOfMonth = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));

            // Lấy tất cả các work shift IDs của team
            var workShiftIds = team.TeamWorkShifts.Select(tws => tws.WorkShiftId).ToList();
            var teamMemberIds = teamMembers.Select(tm => tm.Id).ToList();

            // Enqueue background job để tạo DailySchedule (từ hôm nay đến cuối tháng)
            _backgroundJobClient.Enqueue(() => _dailyScheduleService.CreateDailySchedulesForMembersBackgroundAsync(
                teamMemberIds,
                workShiftIds,
                today,
                endOfMonth,
                true
            ));
        }

        return true;
    }



    public async Task<bool> RemoveMemberFromTeam(Guid teamMemberId)
    {
        var member = await _unitOfWork.TeamMemberRepository.GetByIdAsync(teamMemberId)
            ?? throw new BadRequestException("Không tìm thấy thông tin!");

        // Xóa DailySchedule của member này
        var dailySchedules = await _unitOfWork.DailyScheduleRepository.WhereAsync(
            ds => ds.TeamMemberId == teamMemberId
        );

        if (dailySchedules.Count > 0)
            _unitOfWork.DailyScheduleRepository.SoftRemoveRange(dailySchedules);

        _unitOfWork.TeamMemberRepository.SoftRemove(member);
        return await _unitOfWork.SaveChangesAsync();
    }

    public async Task<List<TeamMember>> GetMembersByTeamIdAsync(Guid teamId)
    {
        return await _unitOfWork.TeamMemberRepository.WhereAsync(x => x.TeamId == teamId, includeFunc: x => x.Include(x => x.Employee));
    }


    public async Task<bool> DeleteWorkTypeAsync(Guid id)
    {
        var team_work_type = await _unitOfWork.TeamWorkTypeRepository.GetByIdAsync(id) ?? throw new BadRequestException("Không tìm thấy thông tin!");
        _unitOfWork.TeamWorkTypeRepository.SoftRemove(team_work_type);
        return await _unitOfWork.SaveChangesAsync();
    }


    public async Task<BasePagingResponseModel<Slot>> GetSlotsByTeamIdAsync(Guid teamId, SlotFilterQuery query)
    {

        Expression<Func<Slot, bool>> filter = x => x.TeamId == teamId;
        if (query.DayOfWeek != null)
        {
            Expression<Func<Slot, bool>> tmp_filter = x => x.DayOfWeek == query.DayOfWeek;
            filter = filter != null ? FilterCustoms.CombineFilters(filter, tmp_filter) : tmp_filter;
        }
        if (query.StartTime != null)
        {
            Expression<Func<Slot, bool>> tmp_filter = x => x.StartTime >= query.StartTime;
            filter = filter != null ? FilterCustoms.CombineFilters(filter, tmp_filter) : tmp_filter;
        }
        if (query.EndTime != null)
        {
            Expression<Func<Slot, bool>> tmp_filter = x => x.EndTime <= query.EndTime;
            filter = filter != null ? FilterCustoms.CombineFilters(filter, tmp_filter) : tmp_filter;
        }
        var (Pagination, Entities) = await _unitOfWork.SlotRepository.ToPagination(
            pageIndex: query.Page ?? 0,
            pageSize: query.Limit ?? 10,
            filter: filter,
            searchTerm: query.Q,
            searchFields: ["Name", "Description"],
            sortOrders: query.OrderBy?.ToDictionary(
                    k => k.OrderColumn ?? "CreatedAt",
                    v => (v.OrderDir ?? "ASC").Equals("ASC", StringComparison.CurrentCultureIgnoreCase)
                ) ?? new Dictionary<string, bool> { { "CreatedAt", false } },
            includeFunc: x => x
                .Include(x => x.Area)
                .Include(x => x.Service)
                .Include(x => x.PetGroup).ThenInclude(x => x!.PetSpecies!)
                .Include(x => x.PetGroup).ThenInclude(x => x!.PetBreed!)
                .Include(x => x.Team)
        );
        return BasePagingResponseModel<Slot>.CreateInstance(Entities, Pagination);
    }

    public async Task<BasePagingResponseModel<DailyTask>> GetDailyTasksByTeamIdAsync(Guid teamId, DailyTaskFilterQuery query)
    {
        Expression<Func<DailyTask, bool>> filter = x => x.TeamId == teamId;
        if (query.FromDate.HasValue)
        {
            filter = filter != null ? FilterCustoms.CombineFilters(filter, x => x.AssignedDate >= query.FromDate.Value) : x => x.AssignedDate >= query.FromDate.Value;
        }
        if (query.ToDate.HasValue)
        {
            filter = filter != null ? FilterCustoms.CombineFilters(filter, x => x.AssignedDate <= query.ToDate.Value) : x => x.AssignedDate <= query.ToDate.Value;
        }
        if (!string.IsNullOrEmpty(query.Status))
        {
            filter = filter != null ? FilterCustoms.CombineFilters(filter, x => x.Status == query.Status) : x => x.Status == query.Status;
        }
        var (Pagination, Entities) = await _unitOfWork.DailyTaskRepository.ToPagination(
            pageIndex: query.Page ?? 0,
            pageSize: query.Limit ?? 10,
            filter: filter,
            searchTerm: query.Q,
            searchFields: ["Task.Title", "Slot.Name", "Slot.Time", "Slot.DayOfWeek", "Slot.Area", "Slot.Team", "Slot.PetGroup", "Slot.Pet", "Slot.SpecialNotes"],
            sortOrders: query.OrderBy?.ToDictionary(
                    k => k.OrderColumn ?? "CreatedAt",
                    v => (v.OrderDir ?? "ASC").Equals("ASC", StringComparison.CurrentCultureIgnoreCase)
                ) ?? new Dictionary<string, bool> { { "CreatedAt", false } },
            includeFunc: x => x.Include(x => x.Task).Include(x => x.Slot).Include(x => x.Team)
        );
        return BasePagingResponseModel<DailyTask>.CreateInstance(Entities, Pagination);
    }
}