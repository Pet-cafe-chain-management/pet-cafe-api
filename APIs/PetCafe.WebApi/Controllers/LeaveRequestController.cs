using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCafe.Application.Models.LeaveRequestModels;
using PetCafe.Application.Services;
using PetCafe.Domain.Constants;

namespace PetCafe.WebApi.Controllers;

[ApiController]
[Route("api/leave-requests")]
public class LeaveRequestController(ILeaveRequestService _leaveRequestService) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = $"{RoleConstants.MANAGER},{RoleConstants.EMPLOYEE}")]
    public async Task<IActionResult> Create([FromBody] LeaveRequestCreateModel model)
    {
        var result = await _leaveRequestService.CreateAsync(model);
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = $"{RoleConstants.MANAGER},{RoleConstants.EMPLOYEE}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] LeaveRequestUpdateModel model)
    {
        var result = await _leaveRequestService.UpdateAsync(id, model);
        return Ok(result);
    }

    [HttpPut("{id:guid}/approve")]
    [Authorize(Roles = $"{RoleConstants.MANAGER},{RoleConstants.EMPLOYEE}")]
    public async Task<IActionResult> Approve([FromRoute] Guid id, [FromBody] LeaveRequestReviewModel model)
    {
        var result = await _leaveRequestService.ApproveAsync(id, model);
        return Ok(result);
    }

    [HttpPut("{id:guid}/reject")]
    [Authorize(Roles = $"{RoleConstants.MANAGER},{RoleConstants.EMPLOYEE}")]
    public async Task<IActionResult> Reject([FromRoute] Guid id, [FromBody] LeaveRequestReviewModel model)
    {
        var result = await _leaveRequestService.RejectAsync(id, model);
        return Ok(result);
    }

    [HttpPut("{id:guid}/cancel")]
    [Authorize(Roles = $"{RoleConstants.MANAGER},{RoleConstants.EMPLOYEE}")]
    public async Task<IActionResult> Cancel([FromRoute] Guid id)
    {
        var result = await _leaveRequestService.CancelAsync(id);
        return Ok(new { success = result });
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var result = await _leaveRequestService.GetByIdAsync(id);
        return Ok(result);
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll([FromQuery] LeaveRequestFilterQuery query)
    {
        var result = await _leaveRequestService.GetAllAsync(query);
        return Ok(result);
    }
}

