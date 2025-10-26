namespace PetCafe.WebApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using PetCafe.Application.Models.DailyScheduleModels;
using PetCafe.Application.Services;

[ApiController]
[Route("api/daily-schedules")]
public class DailyScheduleController(IDailyScheduleService _dailyScheduleService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllPaging([FromQuery] DailyScheduleFilterQuery query)
    {
        return Ok(await _dailyScheduleService.GetDailySchedulesAsync(query));
    }
}