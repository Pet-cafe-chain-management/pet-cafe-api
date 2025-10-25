namespace PetCafe.WebApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PetCafe.Application.Services;
using PetCafe.Application.Models.TaskModels;
using PetCafe.Application.Models.ShareModels;
using PetCafe.Domain.Constants;
using PetCafe.Application.Models.SlotModels;

public class TaskController(ITaskService _taskService) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAllPaging([FromQuery] TaskFilterQuery query)
    {
        return Ok(await _taskService.GetAllTasksAsync(query));
    }
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        return Ok(await _taskService.GetTaskByIdAsync(id));
    }
    [HttpGet("{id:guid}/slots")]
    public async Task<IActionResult> GetSlots(Guid id, [FromQuery] SlotFilterQuery query)
    {
        return Ok(await _taskService.GetSlotsByTaskIdAsync(id, query));
    }
    [HttpPost]
    [Authorize(Roles = RoleConstants.MANAGER)]
    public async Task<IActionResult> Create(TaskCreateModel model)
    {
        return Ok(await _taskService.CreateTaskAsync(model));
    }
    [HttpPut("{id:guid}")]
    [Authorize(Roles = RoleConstants.MANAGER)]
    public async Task<IActionResult> Update(Guid id, TaskUpdateModel model)
    {
        await _taskService.UpdateTaskAsync(id, model);
        return NoContent();
    }
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = RoleConstants.MANAGER)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _taskService.DeleteTaskAsync(id);
        return NoContent();
    }
}