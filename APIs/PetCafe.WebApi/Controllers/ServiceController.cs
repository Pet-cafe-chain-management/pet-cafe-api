using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCafe.Application.Models.ServiceModels;
using PetCafe.Application.Models.SlotModels;
using PetCafe.Application.Services;
using PetCafe.Domain.Constants;

namespace PetCafe.WebApi.Controllers;

public class ServiceController(
        IServService _service,
        ISlotService _slotService
    ) : BaseController
{

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var service = await _service.GetByIdAsync(id);
        return Ok(service);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllPaging([FromQuery] ServiceFilterQuery query)
    {
        var services = await _service.GetAllPagingAsync(query);
        return Ok(services);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = RoleConstants.MANAGER)]
    public async Task<IActionResult> Update(Guid id, ServiceUpdateModel model)
    {
        await _service.UpdateAsync(id, model);
        return NoContent();
    }
    [HttpPost]
    [Authorize(Roles = RoleConstants.MANAGER)]
    public async Task<IActionResult> Create(ServiceCreateModel model)
    {
        var service = await _service.CreateAsync(model);
        return Ok(service);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = RoleConstants.MANAGER)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }

    [HttpGet("{id:guid}/slots")]
    public async Task<IActionResult> GetServiceSlots([FromRoute] Guid id, SlotFilterQuery query)
    {
        return Ok(await _slotService.GetAllPagingByServiceAsync(id, query));
    }

}