using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCafe.Application.Services;
using PetCafe.Domain.Constants;

namespace PetCafe.WebApi.Controllers;

[ApiController]
[Route("api/team-work-types")]
public class TeamWorkTypeController(ITeamService _teamService) : ControllerBase
{

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = $"{RoleConstants.MANAGER},{RoleConstants.EMPLOYEE}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        await _teamService.DeleteWorkTypeAsync(id);
        return NoContent();
    }
}