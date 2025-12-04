using Microsoft.AspNetCore.Mvc;
using PetCafe.Application.Models.OrderModels;
using PetCafe.Application.Models.PayOsModels;
using PetCafe.Application.Services;

namespace PetCafe.WebApi.Controllers;


public class OrderController(IOrderService _orderService, IPayOsService payOsService) : BaseController
{

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id)
    {
        return Ok(await _orderService.GetByIdAsync(id));
    }

    [HttpGet("{order_code:double}")]
    public async Task<IActionResult> GetByOrderCodeAsync([FromRoute] double order_code)
    {
        return Ok(await _orderService.GetByOrderCodeAsync(order_code));
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync([FromQuery] OrderFilterQuery query)
    {
        return Ok(await _orderService.GetAllPagingAsync(query, null));
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(OrderCreateModel model)
    {
        var order = await _orderService.CreateAsync(model);
        return Ok(order);
    }
    [HttpPut("{id:guid}/confirm")]
    public async Task<IActionResult> ConfirmAsync([FromRoute] Guid id)
    {
        var order = await _orderService.ConfirmOrderAsync(id);
        return Ok(order);
    }



    [HttpPost("/api/webhook-event-handler")]
    public async Task<IActionResult> PayOsWebHook([FromBody] WebhookResponseModel model)
    {
        // await _orderService.HandleWebhookAsync(model);
        return Ok(new { success = true });
    }

    [HttpGet("payos")]
    public async Task<IActionResult> CreatePayOsPayment()
    {

        var num = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds().ToString();

        var paymentResponse = await payOsService.CreatePaymentAsync(200000, Double.Parse(num));
        return Ok(paymentResponse);
    }
}