namespace PetCafe.WebApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using PetCafe.Application.Services;
using PetCafe.Application.Models.DailyTaskModels;

[ApiController]
[Route("api/daily-tasks")]
public class DailyTaskController(IDailyTaskService _dailyTaskService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllPaging([FromQuery] DailyTaskFilterQuery query)
    {
        return Ok(await _dailyTaskService.GetAllPagingAsync(query));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] DailyTaskCreateModel model)
    {
        return Ok(await _dailyTaskService.CreateAsync(model));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, DailyTaskUpdateModel model)
    {
        await _dailyTaskService.UpdateAsync(id, model);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _dailyTaskService.DeleteAsync(id);
        return NoContent();
    }

}