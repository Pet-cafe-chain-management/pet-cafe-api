using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCafe.Application.Models.ShareModels;
using PetCafe.Application.Models.TeamModels;
using PetCafe.Application.Services;
using PetCafe.Domain.Constants;

namespace PetCafe.WebApi.Controllers;

public class TeamController(ITeamService _teamService) : BaseController
{

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll([FromQuery] FilterQuery query)
    {
        {
            var teams = await _teamService.GetAllPagingAsync(query);
            return Ok(teams);
        }
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> GetById(Guid id)
    {
        var team = await _teamService.GetByIdAsync(id);
        return Ok(team);
    }

    [HttpPost]
    [Authorize(Roles = RoleConstants.MANAGER)]
    public async Task<IActionResult> Create([FromBody] TeamCreateModel model)
    {
        var createdTeam = await _teamService.CreateAsync(model);
        return Ok(createdTeam);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = RoleConstants.MANAGER)]
    public async Task<IActionResult> Update(Guid id, [FromBody] TeamUpdateModel model)
    {
        await _teamService.UpdateAsync(id, model);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = RoleConstants.MANAGER)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _teamService.DeleteAsync(id);
        return NoContent();
    }

    [HttpPost("{id:guid}/members")]
    [Authorize(Roles = RoleConstants.MANAGER)]
    public async Task<IActionResult> AddMembersToTeam(Guid id, [FromBody] List<MemberCreateModel> models)
    {
        await _teamService.AddMemeberToTeam(models, id);
        return StatusCode(201);
    }

    [HttpPut("{id:guid}/members")]
    [Authorize(Roles = RoleConstants.MANAGER)]
    public async Task<IActionResult> UpdateMembersInTeam(Guid id, [FromBody] List<MemberUpdateModel> models)
    {
        await _teamService.UpdateMemberInTeam(models, id);
        return NoContent();
    }
    [HttpDelete("{id:guid}/members")]
    [Authorize(Roles = RoleConstants.MANAGER)]
    public async Task<IActionResult> RemoveMembersFromTeam(Guid id, [FromBody] List<Guid> memberIds)
    {
        await _teamService.RemoveMemberFromTeam(memberIds, id);
        return NoContent();
    }
    [HttpGet("{id:guid}/members")]
    [Authorize]

    public async Task<IActionResult> GetMembersByTeamId(Guid id)
    {
        var members = await _teamService.GetMembersByTeamIdAsync(id);
        return Ok(members);
    }

}