using Microsoft.AspNetCore.Mvc;
using PetCafe.Application.Models.NotificationModels;
using PetCafe.Application.Models.ShareModels;
using PetCafe.Application.Services;

namespace PetCafe.WebApi.Controllers;

public class NotificationController(INotificationService _notificationService) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAllPaging([FromQuery] FilterQuery query, [FromQuery(Name = "account_id")] Guid accountId)
    {
        return Ok(await _notificationService.GetAllPagingAsync(accountId, query));
    }

    [HttpPut]
    public async Task<IActionResult> UpdateNotification([FromBody] NotificationUpdateModel model)
    {
        await _notificationService.UpdateNotificationAsync(model);
        return Ok();
    }
}