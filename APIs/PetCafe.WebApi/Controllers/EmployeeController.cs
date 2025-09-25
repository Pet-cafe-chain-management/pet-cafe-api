using Microsoft.AspNetCore.Mvc;
using PetCafe.Application.Models.EmployeeModels;
using PetCafe.Application.Models.ShareModels;
using PetCafe.Application.Services;

namespace PetCafe.WebApi.Controllers;

public class EmployeeController(IEmployeeService _employeeService) : BaseController
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] EmployeeCreateModel model)
    {
        return Ok(await _employeeService.CreateAsync(model));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] EmployeeUpdateModel model)
    {
        await _employeeService.UpdateAsync(id, model);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        await _employeeService.DeleteAsync(id);
        return NoContent();
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        return Ok(await _employeeService.GetByIdAsync(id));
    }

    [HttpGet]
    public async Task<IActionResult> GetAllPaging([FromQuery] FilterQuery query)
    {
        return Ok(await _employeeService.GetAllPagingAsync(query));
    }
}