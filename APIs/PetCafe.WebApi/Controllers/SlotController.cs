using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCafe.Application.Models.SlotModels;
using PetCafe.Application.Services;
using PetCafe.Domain.Constants;

namespace PetCafe.WebApi.Controllers;

public class SlotController(ISlotService _slotService) : BaseController
{

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var product = await _slotService.GetByIdAsync(id);
        return Ok(product);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = RoleConstants.MANAGER)]
    public async Task<IActionResult> Update(Guid id, SlotUpdateModel model)
    {
        await _slotService.UpdateAsync(id, model);
        return NoContent();
    }
    [HttpPost]
    [Authorize(Roles = RoleConstants.MANAGER)]
    public async Task<IActionResult> Create(SlotCreateModel model)
    {
        var product = await _slotService.CreateAsync(model);
        return Ok(product);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = RoleConstants.MANAGER)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _slotService.DeleteAsync(id);
        return NoContent();
    }
}