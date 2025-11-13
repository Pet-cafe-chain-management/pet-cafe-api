using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCafe.Application.Models.ShareModels;
using PetCafe.Application.Models.VaccineTypeModels;
using PetCafe.Application.Services;
using PetCafe.Domain.Constants;

namespace PetCafe.WebApi.Controllers;

[ApiController]
[Route("api/vaccine-types")]
public class VaccineTypeController(IVaccineTypeService _vaccineTypeService) : ControllerBase
{

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var vaccineType = await _vaccineTypeService.GetByIdAsync(id);
        return Ok(vaccineType);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllPaging([FromQuery] VaccineTypeFilterQuery query)
    {
        var result = await _vaccineTypeService.GetAllPagingAsync(query);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = RoleConstants.MANAGER)]

    public async Task<IActionResult> Create([FromBody] VaccineTypeCreateModel model)
    {
        var vaccineType = await _vaccineTypeService.CreateAsync(model);
        return Ok(vaccineType);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = RoleConstants.MANAGER)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] VaccineTypeUpdateModel model)
    {
        await _vaccineTypeService.UpdateAsync(id, model);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = RoleConstants.MANAGER)]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var success = await _vaccineTypeService.DeleteAsync(id);
        return NoContent();
    }
}