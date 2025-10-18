using Microsoft.EntityFrameworkCore;
using PetCafe.Application.GlobalExceptionHandling.Exceptions;
using PetCafe.Application.Models.ShareModels;
using PetCafe.Application.Models.TeamModels;
using PetCafe.Domain.Entities;

namespace PetCafe.Application.Services;

public interface ITeamService
{
    Task<Team> GetByIdAsync(Guid id);
    Task<BasePagingResponseModel<Team>> GetAllPagingAsync(FilterQuery query);
    Task<Team> CreateAsync(TeamCreateModel model);
    Task<Team> UpdateAsync(Guid id, TeamUpdateModel model);
    Task<bool> DeleteAsync(Guid id);


    Task<List<TeamMember>> GetMembersByTeamIdAsync(Guid teamId);
    Task<bool> AddMemeberToTeam(List<MemberCreateModel> models, Guid id);
    Task<bool> UpdateMemberInTeam(List<MemberUpdateModel> models, Guid id);
    Task<bool> RemoveMemberFromTeam(List<Guid> memberIds, Guid id);
}


public class TeamService(
    IUnitOfWork _unitOfWork
) : ITeamService
{
    public async Task<Team> CreateAsync(TeamCreateModel model)
    {
        var area = await _unitOfWork.AreaRepository
            .FirstOrDefaultAsync(x =>
                x.WorkTypeId == model.WorkTypeId &&
                x.Id == model.AreaId
            ) ?? throw new BadRequestException("Không tìm thấy khu vực tương ứng với loại công việc");
        var team = _unitOfWork.Mapper.Map<Team>(model);
        await _unitOfWork.TeamRepository.AddAsync(team);
        await _unitOfWork.SaveChangesAsync();
        return team;
    }

    public async Task<Team> UpdateAsync(Guid id, TeamUpdateModel model)
    {
        var area = await _unitOfWork.AreaRepository
            .FirstOrDefaultAsync(x =>
                x.WorkTypeId == model.WorkTypeId &&
                x.Id == model.AreaId
            ) ?? throw new BadRequestException("Không tìm thấy khu vực tương ứng với loại công việc");
        var team = await _unitOfWork.TeamRepository.GetByIdAsync(id) ?? throw new BadRequestException("Không tìm thấy thông tin!");
        _unitOfWork.Mapper.Map(model, team);
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
                    .Include(x => x.TeamMembers.Where(x => !x.IsDeleted)).ThenInclude(x => x.Employee))
                ?? throw new BadRequestException("Không tìm thấy thông tin!");
    }

    public async Task<BasePagingResponseModel<Team>> GetAllPagingAsync(FilterQuery query)
    {

        var (Pagination, Entities) = await _unitOfWork.TeamRepository.ToPagination(
            pageIndex: query.Page ?? 0,
           pageSize: query.Limit ?? 10,
           searchTerm: query.Q,
           searchFields: ["Name", "Description"],
           sortOrders: query.OrderBy?.ToDictionary(
                   k => k.OrderColumn ?? "CreatedAt",
                   v => (v.OrderDir ?? "ASC").Equals("ASC", StringComparison.CurrentCultureIgnoreCase)
               ) ?? new Dictionary<string, bool> { { "CreatedAt", false } },
           includeFunc: x => x.Include(x => x.Leader).Include(x => x.Area).Include(x => x.WorkType)
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

    public async Task<bool> RemoveMemberFromTeam(List<Guid> memberIds, Guid id)
    {
        var team = await _unitOfWork.TeamRepository.GetByIdAsync(id) ?? throw new BadRequestException("Không tìm thấy thông tin!");
        var existingMembers = await _unitOfWork.TeamMemberRepository.WhereAsync(x => x.TeamId == id && memberIds.Contains(x.EmployeeId));

        if (existingMembers.Count == 0) throw new BadRequestException("Không tìm thấy thành viên trong đội!");

        foreach (var member in existingMembers)
        {
            _unitOfWork.TeamMemberRepository.SoftRemove(member);
        }
        return await _unitOfWork.SaveChangesAsync();
    }

    public async Task<List<TeamMember>> GetMembersByTeamIdAsync(Guid teamId)
    {
        return await _unitOfWork.TeamMemberRepository.WhereAsync(x => x.TeamId == teamId, includeFunc: x => x.Include(x => x.Employee));
    }
}