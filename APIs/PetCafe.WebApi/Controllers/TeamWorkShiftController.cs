using PetCafe.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCafe.Domain.Constants;

namespace PetCafe.WebApi.Controllers;

[ApiController]
[Route("api/team-work-shifts")]
public class TeamWorkShiftController(ITeamWorkShiftService _teamWorkShiftService) : BaseController
{
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = RoleConstants.MANAGER)]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        await _teamWorkShiftService.RemoveWorkShift(id);
        return NoContent();
    }
}