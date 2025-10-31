using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCafe.Application.Models.FeedbackModels;
using PetCafe.Application.Services;
using PetCafe.Domain.Constants;

namespace PetCafe.WebApi.Controllers;

public class FeedbackController(
    IFeedbackService _feedbackService
) : BaseController
{
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var feedback = await _feedbackService.GetByIdAsync(id);
        return Ok(feedback);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllPaging([FromQuery] FeedbackFilterQuery query)
    {
        var feedbacks = await _feedbackService.GetAllPagingAsync(query);
        return Ok(feedbacks);
    }


    [HttpPost]
    [Authorize(Roles = RoleConstants.CUSTOMER)]
    public async Task<IActionResult> Create([FromBody] FeedbackCreateModel model)
    {
        var feedback = await _feedbackService.CreateAsync(model);
        return Ok(feedback);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = RoleConstants.CUSTOMER)]
    public async Task<IActionResult> Update(Guid id, [FromBody] FeedbackUpdateModel model)
    {
        var feedback = await _feedbackService.UpdateAsync(id, model);
        return Ok(feedback);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = RoleConstants.CUSTOMER + "," + RoleConstants.MANAGER)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _feedbackService.DeleteAsync(id);
        return NoContent();
    }
}

