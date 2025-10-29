using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCafe.Application.Models.BookingModels;
using PetCafe.Application.Services;
using PetCafe.Domain.Constants;

namespace PetCafe.WebApi.Controllers;


public class BookingController(
    IBookingService _bookingService
) : BaseController
{
    [HttpGet("{id:guid}")]
    [Authorize(Roles = $"{RoleConstants.MANAGER},{RoleConstants.EMPLOYEE}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        return Ok(await _bookingService.GetByIdAsync(id));
    }

    [HttpGet]
    [Authorize(Roles = $"{RoleConstants.MANAGER},{RoleConstants.EMPLOYEE}")]
    public async Task<IActionResult> GetAllPaging([FromQuery] BookingFilterQuery query)
    {
        return Ok(await _bookingService.GetAllPagingAsync(query));
    }
    [HttpPut("{id:guid}")]
    [Authorize(Roles = $"{RoleConstants.MANAGER},{RoleConstants.EMPLOYEE}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] BookingUpdateModel model)
    {
        return Ok(await _bookingService.UpdateAsync(id, model));
    }
}