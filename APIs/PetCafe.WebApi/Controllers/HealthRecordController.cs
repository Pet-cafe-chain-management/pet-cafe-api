using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCafe.Application.Models.HealthRecordModels;
using PetCafe.Application.Models.ShareModels;
using PetCafe.Application.Services;
using PetCafe.Domain.Constants;

namespace PetCafe.WebApi.Controllers;

[ApiController]
[Route("api/health-records")]
public class HealthRecordController(IHealthRecordService _healthRecordService) : ControllerBase
{

    [HttpGet("{id:guid}")]
    [Authorize(Roles = $"{RoleConstants.MANAGER},{RoleConstants.EMPLOYEE}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var record = await _healthRecordService.GetByIdAsync(id);
        return Ok(record);
    }


    [HttpPost]
    [Authorize(Roles = $"{RoleConstants.MANAGER},{RoleConstants.EMPLOYEE}")]
    public async Task<IActionResult> Create([FromBody] HealthRecordCreateModel model)
    {
        var createdRecord = await _healthRecordService.CreateAsync(model);
        return Ok(createdRecord);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = $"{RoleConstants.MANAGER},{RoleConstants.EMPLOYEE}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] HealthRecordUpdateModel model)
    {
        await _healthRecordService.UpdateAsync(id, model);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = $"{RoleConstants.MANAGER},{RoleConstants.EMPLOYEE}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        await _healthRecordService.DeleteAsync(id);
        return NoContent();
    }
}