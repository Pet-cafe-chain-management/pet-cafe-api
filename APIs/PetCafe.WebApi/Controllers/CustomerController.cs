using Microsoft.AspNetCore.Mvc;
using PetCafe.Application.Models.CustomerModels;
using PetCafe.Application.Models.OrderModels;
using PetCafe.Application.Models.ShareModels;
using PetCafe.Application.Services;

namespace PetCafe.WebApi.Controllers;

public class CustomerController(
    ICustomerService _customerService,
    IOrderService _orderService
) : BaseController
{

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CustomerCreateModel model)
    {
        return Ok(await _customerService.CreateAsync(model));
    }
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] CustomerUpdateModel model)
    {
        await _customerService.UpdateAsync(id, model);
        return NoContent();
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        return Ok(await _customerService.GetByIdAsync(id));
    }

    [HttpGet("{id:guid}/orders")]
    public async Task<IActionResult> GetAllAsync([FromRoute] Guid id, [FromQuery] OrderFilterQuery query)
    {
        return Ok(await _orderService.GetAllPagingAsync(query, id));
    }

    [HttpGet]
    public async Task<IActionResult> GetAllPaging([FromQuery] FilterQuery query)
    {
        return Ok(await _customerService.GetAllPagingAsync(query));
    }
}