using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCafe.Application.Models.EmployeeModels;
using PetCafe.Application.Models.ShareModels;
using PetCafe.Application.Services;
using PetCafe.Domain.Constants;

namespace PetCafe.WebApi.Controllers;

public class EmployeeController(IEmployeeService _employeeService) : BaseController
{
    [HttpPost]
    [Authorize(Roles = RoleConstants.MANAGER)]
    public async Task<IActionResult> Create([FromBody] EmployeeCreateModel model)
    {
        return Ok(await _employeeService.CreateAsync(model));
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] EmployeeUpdateModel model)
    {
        await _employeeService.UpdateAsync(id, model);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = RoleConstants.MANAGER)]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        await _employeeService.DeleteAsync(id);
        return NoContent();
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        return Ok(await _employeeService.GetByIdAsync(id));
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAllPaging([FromQuery] EmployeeFilterQuery query)
    {
        return Ok(await _employeeService.GetAllPagingAsync(query));
    }
}