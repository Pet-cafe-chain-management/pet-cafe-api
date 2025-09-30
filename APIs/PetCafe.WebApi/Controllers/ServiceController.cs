using Microsoft.AspNetCore.Mvc;
using PetCafe.Application.Models.ServiceModels;
using PetCafe.Application.Models.ShareModels;
using PetCafe.Application.Services;

namespace PetCafe.WebApi.Controllers;

public class ServiceController(
        IServService _service,
        ISlotService _slotService
    ) : BaseController
{

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var product = await _service.GetByIdAsync(id);
        return Ok(product);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllPaging([FromQuery] FilterQuery query)
    {
        var products = await _service.GetAllPagingAsync(query);
        return Ok(products);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, ServiceUpdateModel model)
    {
        await _service.UpdateAsync(id, model);
        return NoContent();
    }
    [HttpPost]
    public async Task<IActionResult> Create(ServiceCreateModel model)
    {
        var product = await _service.CreateAsync(model);
        return Ok(product);
    }

    [HttpDelete("{id:guid}")]
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
}