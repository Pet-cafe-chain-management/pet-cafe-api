using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCafe.Application.Models.VaccinationRecordModels;
using PetCafe.Application.Services;
using PetCafe.Domain.Constants;

namespace PetCafe.WebApi.Controllers;

[ApiController]
[Route("api/vaccination-records")]
public class VaccinationRecordController(IVaccinationRecordService _vaccinationRecordService) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = $"{RoleConstants.MANAGER},{RoleConstants.EMPLOYEE}")]
    public async Task<IActionResult> Create([FromBody] VaccinationRecordCreateModel model)
    {
        var result = await _vaccinationRecordService.CreateAsync(model);
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = $"{RoleConstants.MANAGER},{RoleConstants.EMPLOYEE}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] VaccinationRecordUpdateModel model)
    {
        await _vaccinationRecordService.UpdateAsync(id, model);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = $"{RoleConstants.MANAGER},{RoleConstants.EMPLOYEE}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        await _vaccinationRecordService.DeleteAsync(id);
        return NoContent();
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var result = await _vaccinationRecordService.GetByIdAsync(id);
        return Ok(result);
    }
}