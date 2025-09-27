using Microsoft.AspNetCore.Mvc;
using PetCafe.Application.Models.VaccinationScheduleModels;
using PetCafe.Application.Services;

namespace PetCafe.WebApi.Controllers;

[ApiController]
[Route("api/vaccination-schedules")]
public class VaccinationScheduleController(IVaccinationScheduleService _vaccinationScheduleService) : ControllerBase
{

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] VaccinationScheduleCreateModel model)
    {
        var result = await _vaccinationScheduleService.CreateAsync(model);
        return Ok(result);
    }
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] VaccinationScheduleUpdateModel model)
    {
        await _vaccinationScheduleService.UpdateAsync(id, model);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        await _vaccinationScheduleService.DeleteAsync(id);
        return NoContent();
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var result = await _vaccinationScheduleService.GetByIdAsync(id);
        return Ok(result);
    }
}