using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCafe.Application.Models.ServiceModels;
using PetCafe.Application.Models.ShareModels;
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
    public async Task<IActionResult> GetServiceSlots([FromRoute] Guid id, FilterQuery query)
    {
        return Ok(await _slotService.GetAllPagingByServiceAsync(id, query));
    }

    [HttpPost("{id:guid}/pet-groups")]
    [Authorize(Roles = RoleConstants.MANAGER)]
    public async Task<IActionResult> AssignPetGroupToService(Guid id, ServicePetGroupCreateModel model)
    {
        await _service.AssignPetGroupToService(id, model);
        return Ok();
    }

    [HttpDelete("{id:guid}/pet-groups/{petGroupId:guid}")]
    [Authorize(Roles = RoleConstants.MANAGER)]
    public async Task<IActionResult> RemovePetGroupFromService(Guid id, Guid petGroupId)
    {
        await _service.RemovePetGroupFromService(id, petGroupId);
        return NoContent();
    }

    [HttpGet("{id:guid}/pet-groups")]
    public async Task<IActionResult> GetPetGroupsByServiceId([FromRoute] Guid id, FilterQuery query)
    {
        return Ok(await _service.GetPetGroupsByServiceId(id, query));
    }
}