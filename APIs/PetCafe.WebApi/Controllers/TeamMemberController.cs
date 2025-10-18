using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCafe.Application.Services;
using PetCafe.Domain.Constants;

namespace PetCafe.WebApi.Controllers;

[ApiController]
[Route("api/team-members")]
public class TeamMemberController(ITeamService _teamService) : ControllerBase
{
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = RoleConstants.MANAGER)]
    public async Task<IActionResult> RemoveMembersFromTeam(Guid id)
    {
        await _teamService.RemoveMemberFromTeam(id);
        return NoContent();
    }
}
