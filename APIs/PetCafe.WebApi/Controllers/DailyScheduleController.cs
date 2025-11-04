namespace PetCafe.WebApi.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCafe.Application.Models.DailyScheduleModels;
using PetCafe.Application.Services;
using PetCafe.Domain.Constants;

[ApiController]
[Route("api/daily-schedules")]
public class DailyScheduleController(IDailyScheduleService _dailyScheduleService) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = $"{RoleConstants.MANAGER},{RoleConstants.EMPLOYEE}")]
    public async Task<IActionResult> GetAllPaging([FromQuery] DailyScheduleFilterQuery query)
    {
        return Ok(await _dailyScheduleService.GetDailySchedulesAsync(query));
    }


}