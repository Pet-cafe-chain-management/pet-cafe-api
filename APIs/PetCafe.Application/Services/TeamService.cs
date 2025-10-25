using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PetCafe.Application.GlobalExceptionHandling.Exceptions;
using PetCafe.Application.Models.ShareModels;
using PetCafe.Application.Models.SlotModels;
using PetCafe.Application.Models.TeamModels;
using PetCafe.Application.Utilities;
using PetCafe.Domain.Entities;

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

    Task<List<TeamMember>> GetMembersByTeamIdAsync(Guid teamId);
    Task<bool> AddMemeberToTeam(List<MemberCreateModel> models, Guid id);
    Task<bool> UpdateMemberInTeam(List<MemberUpdateModel> models, Guid id);
    Task<bool> RemoveMemberFromTeam(Guid teamMemberId);
}


public class TeamService(
    IUnitOfWork _unitOfWork
) : ITeamService
{
    public async Task<List<WorkType>> GetWorkTypeNotInTeamAsync(Guid teamId)
    {
        return await _unitOfWork.WorkTypeRepository.WhereAsync(x => !x.TeamWorkTypes.Any(y => y.TeamId == teamId));
    }

    public async Task<Team> CreateAsync(TeamCreateModel model)
    {
        var team = _unitOfWork.Mapper.Map<Team>(model);
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
        var team = await _unitOfWork.TeamRepository.GetByIdAsync(id) ?? throw new BadRequestException("Không tìm thấy thông tin!");
        _unitOfWork.Mapper.Map(model, team);
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
        var team = await _unitOfWork.TeamRepository.GetByIdAsync(id) ?? throw new BadRequestException("Không tìm thấy thông tin!");
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
        var team = await _unitOfWork.TeamRepository.GetByIdAsync(id) ?? throw new BadRequestException("Không tìm thấy thông tin!");

        var teamMembers = models.Select(x => new TeamMember
        {
            TeamId = id,
            EmployeeId = x.EmployeeId,
            IsActive = true
        }).ToList();

        await _unitOfWork.TeamMemberRepository.AddRangeAsync(teamMembers);
        return await _unitOfWork.SaveChangesAsync();
    }

    public async Task<bool> UpdateMemberInTeam(List<MemberUpdateModel> models, Guid id)
    {
        var team = await _unitOfWork.TeamRepository.GetByIdAsync(id) ?? throw new BadRequestException("Không tìm thấy thông tin!");
        var memberIds = models.Select(x => x.EmployeeId).ToList();
        var existingMembers = await _unitOfWork.TeamMemberRepository.WhereAsync(x => x.TeamId == id && memberIds.Contains(x.EmployeeId));

        if (existingMembers.Count == 0) throw new BadRequestException("Không tìm thấy thành viên trong đội!");

        foreach (var member in existingMembers)
        {
            var updateModel = models.FirstOrDefault(x => x.EmployeeId == member.EmployeeId);
            if (updateModel != null)
            {
                member.IsActive = updateModel.IsActive;
                _unitOfWork.TeamMemberRepository.Update(member);
            }
        }
        return await _unitOfWork.SaveChangesAsync();
    }

    public async Task<bool> RemoveMemberFromTeam(Guid teamMemberId)
    {
        var member = await _unitOfWork.TeamMemberRepository.GetByIdAsync(teamMemberId)
            ?? throw new BadRequestException("Không tìm thấy thông tin!");

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
}