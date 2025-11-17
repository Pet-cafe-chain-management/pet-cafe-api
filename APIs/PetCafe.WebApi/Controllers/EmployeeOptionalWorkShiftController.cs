using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCafe.Application.Models.EmployeeOptionalWorkShiftModels;
using PetCafe.Application.Services;
using PetCafe.Domain.Constants;

namespace PetCafe.WebApi.Controllers;

[ApiController]
[Route("api/employees/{employeeId:guid}/optional-work-shifts")]
public class EmployeeOptionalWorkShiftController(IEmployeeOptionalWorkShiftService _optionalWorkShiftService) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = $"{RoleConstants.MANAGER},{RoleConstants.EMPLOYEE}")]
    public async Task<IActionResult> Add([FromRoute] Guid employeeId, [FromBody] EmployeeOptionalWorkShiftCreateModel model)
    {
        model.EmployeeId = employeeId; // Ensure employeeId from route is used
        var result = await _optionalWorkShiftService.AddAsync(model);
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = $"{RoleConstants.MANAGER},{RoleConstants.EMPLOYEE}")]
    public async Task<IActionResult> Update([FromRoute] Guid employeeId, [FromRoute] Guid id, [FromBody] EmployeeOptionalWorkShiftUpdateModel model)
    {
        var result = await _optionalWorkShiftService.UpdateAsync(id, model);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = $"{RoleConstants.MANAGER},{RoleConstants.EMPLOYEE}")]
    public async Task<IActionResult> Remove([FromRoute] Guid employeeId, [FromRoute] Guid id)
    {
        var result = await _optionalWorkShiftService.RemoveAsync(id);
        return Ok(new { success = result });
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetByEmployeeId([FromRoute] Guid employeeId)
    {
        var result = await _optionalWorkShiftService.GetByEmployeeIdAsync(employeeId);
        return Ok(result);
    }
}

