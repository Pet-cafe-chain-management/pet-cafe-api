using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCafe.Application.Models.DailyScheduleModels;
using PetCafe.Application.Models.DailyTaskModels;
using PetCafe.Application.Models.ShareModels;
using PetCafe.Application.Models.SlotModels;
using PetCafe.Application.Models.TeamModels;
using PetCafe.Application.Models.TeamWorkShiftModels;
using PetCafe.Application.Services;
using PetCafe.Domain.Constants;

namespace PetCafe.WebApi.Controllers;

public class TeamController(
    ITeamService _teamService,
    ITeamWorkShiftService _teamWorkShiftService,
    IDailyScheduleService _dailyScheduleService
) : BaseController
{

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll([FromQuery] TeamFilterQuery query)
    {
        {
            var teams = await _teamService.GetAllPagingAsync(query);
            return Ok(teams);
        }
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> GetById(Guid id)
    {
        var team = await _teamService.GetByIdAsync(id);
        return Ok(team);
    }
    /// <summary>
    /// Lấy danh sách WorkType không thuộc Team đã chọn
    /// </summary>
    /// <param name="id">ID của Team</param>
    /// <returns>Danh sách WorkType không thuộc Team đã chọn</returns>
    [HttpGet("{id:guid}/work-types")]
    public async Task<IActionResult> GetWorkTypeNotInTeam(Guid id)
    {
        var workTypes = await _teamService.GetWorkTypeNotInTeamAsync(id);
        return Ok(workTypes);
    }

    [HttpPost]
    [Authorize(Roles = RoleConstants.MANAGER)]
    public async Task<IActionResult> Create([FromBody] TeamCreateModel model)
    {
        var createdTeam = await _teamService.CreateAsync(model);
        return Ok(createdTeam);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = RoleConstants.MANAGER)]
    public async Task<IActionResult> Update(Guid id, [FromBody] TeamUpdateModel model)
    {
        await _teamService.UpdateAsync(id, model);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = RoleConstants.MANAGER)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _teamService.DeleteAsync(id);
        return NoContent();
    }

    [HttpPost("{id:guid}/members")]
    [Authorize(Roles = RoleConstants.MANAGER)]
    public async Task<IActionResult> AddMembersToTeam(Guid id, [FromBody] List<MemberCreateModel> models)
    {
        await _teamService.AddMemeberToTeam(models, id);
        return StatusCode(201);
    }

    // [HttpPut("{id:guid}/members")]
    // [Authorize(Roles = RoleConstants.MANAGER)]
    // public async Task<IActionResult> UpdateMembersInTeam(Guid id, [FromBody] List<MemberUpdateModel> models)
    // {
    //     await _teamService.UpdateMemberInTeam(models, id);
    //     return NoContent();
    // }

    [HttpGet("{id:guid}/members")]
    [Authorize]

    public async Task<IActionResult> GetMembersByTeamId(Guid id)
    {
        var members = await _teamService.GetMembersByTeamIdAsync(id);
        return Ok(members);
    }

    [HttpPost("{id:guid}/work-shifts")]
    [Authorize(Roles = RoleConstants.MANAGER)]
    public async Task<IActionResult> AssignWorkShift(Guid id, [FromBody] TeamWorkShiftCreateModel models)
    {
        await _teamWorkShiftService.AssignWorkShift(id, models);
        return StatusCode(201);
    }

    // [HttpDelete("{id:guid}/work-shifts")]
    // [Authorize(Roles = RoleConstants.MANAGER)]
    // public async Task<IActionResult> RemoveWorkShift(Guid id)
    // {
    //     await _teamWorkShiftService.RemoveWorkShift(id);
    //     return NoContent();
    // }

    [HttpGet("{id:guid}/work-shifts")]
    [Authorize]

    public async Task<IActionResult> GetTeamWorkShift(Guid id, FilterQuery query)
    {
        var members = await _teamWorkShiftService.GetTeamWorkShift(id, query);
        return Ok(members);
    }

    [HttpGet("{id:guid}/slots")]
    [Authorize]
    public async Task<IActionResult> GetSlots(Guid id, [FromQuery] SlotFilterQuery query)
    {
        var slots = await _teamService.GetSlotsByTeamIdAsync(id, query);
        return Ok(slots);
    }

    [HttpGet("{id:guid}/daily-tasks")]
    [Authorize]
    public async Task<IActionResult> GetDailyTasks(Guid id, [FromQuery] DailyTaskFilterQuery query)
    {
        var dailyTasks = await _teamService.GetDailyTasksByTeamIdAsync(id, query);
        return Ok(dailyTasks);
    }

    [HttpPut("{id:guid}/daily-schedules")]
    [Authorize(Roles = $"{RoleConstants.MANAGER},{RoleConstants.EMPLOYEE}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] List<DailyScheduleUpdateModel> models)
    {
        return Ok(await _dailyScheduleService.UpdateAsync(id, models));
    }


    [HttpGet("{id:guid}/daily-schedules")]
    [Authorize(Roles = $"{RoleConstants.MANAGER},{RoleConstants.EMPLOYEE}")]
    public async Task<IActionResult> GetAllPaging([FromRoute] Guid id, [FromQuery] DailyScheduleFilterQuery query)
    {
        return Ok(await _dailyScheduleService.GetDailySchedulesAsync(id, query));
    }

}