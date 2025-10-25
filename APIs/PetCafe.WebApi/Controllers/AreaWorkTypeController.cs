using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCafe.Application.Services;
using PetCafe.Domain.Constants;

namespace PetCafe.WebApi.Controllers;

[ApiController]
[Route("api/area-work-types")]
public class AreaWorkTypeController(IAreaService _areaService) : ControllerBase
{

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = $"{RoleConstants.MANAGER},{RoleConstants.EMPLOYEE}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        await _areaService.DeleteWorkTypeAsync(id);
        return NoContent();
    }
}