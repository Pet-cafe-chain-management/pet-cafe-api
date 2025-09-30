using Microsoft.AspNetCore.Mvc;
using PetCafe.Application.Models.ShareModels;
using PetCafe.Application.Models.WorkShiftModels;
using PetCafe.Application.Services;

namespace PetCafe.WebApi.Controllers;

[ApiController]
[Route("api/work-shifts")]
public class WorkShiftController(IWorkShiftService _workShiftService) : ControllerBase
{

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var workShift = await _workShiftService.GetByIdAsync(id);
        return Ok(workShift);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllPaging([FromQuery] FilterQuery query)
    {
        var workShifts = await _workShiftService.GetAllPagingAsync(query);
        return Ok(workShifts);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] WorkShiftCreateModel model)
    {
        var workShift = await _workShiftService.CreateAsync(model);
        return Ok(workShift);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] WorkShiftUpdateModel model)
    {
        await _workShiftService.UpdateAsync(id, model);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var result = await _workShiftService.DeleteAsync(id);
        return NoContent();
    }
}