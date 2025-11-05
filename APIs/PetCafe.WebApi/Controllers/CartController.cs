using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCafe.Application.Models.OrderModels;
using PetCafe.Application.Services;
using PetCafe.Application.Services.Commons;
using PetCafe.Domain.Constants;

namespace PetCafe.WebApi.Controllers;

[ApiController]
[Route("api/carts")]
public class CartController(
    ICartService _cartService,
    IClaimsService _claimsService
) : BaseController
{
    [HttpGet]
    [Authorize(Roles = RoleConstants.CUSTOMER)]
    public async Task<IActionResult> GetCart()
    {
        var customerId = _claimsService.GetCurrentUser;
        var cart = await _cartService.GetCartAsync(customerId);

        return Ok(cart);
    }

    [HttpPost]
    [Authorize(Roles = RoleConstants.CUSTOMER)]
    public async Task<IActionResult> AddOrUpdateCart([FromBody] OrderCreateModel model)
    {
        var customerId = _claimsService.GetCurrentUser;
        var cart = await _cartService.AddOrUpdateCartAsync(customerId, model);
        return Ok(cart);
    }

    [HttpPut("info")]
    [Authorize(Roles = RoleConstants.CUSTOMER)]
    public async Task<IActionResult> UpdateCartInfo([FromBody] CartInfoUpdateModel model)
    {
        var customerId = _claimsService.GetCurrentUser;
        var cart = await _cartService.UpdateCartInfoAsync(customerId, model);
        return Ok(cart);
    }

    [HttpDelete]
    [Authorize(Roles = RoleConstants.CUSTOMER)]
    public async Task<IActionResult> ClearCart()
    {
        var customerId = _claimsService.GetCurrentUser;
        var result = await _cartService.ClearCartAsync(customerId);
        return Ok(new { success = result });
    }

    [HttpPost("products")]
    [Authorize(Roles = RoleConstants.CUSTOMER)]
    public async Task<IActionResult> AddProduct([FromBody] ProductOrderModel product)
    {
        var customerId = _claimsService.GetCurrentUser;
        var cart = await _cartService.AddProductToCartAsync(customerId, product);
        return Ok(cart);
    }

    [HttpDelete("products/{productId:guid}")]
    [Authorize(Roles = RoleConstants.CUSTOMER)]
    public async Task<IActionResult> RemoveProduct([FromRoute] Guid productId)
    {
        var customerId = _claimsService.GetCurrentUser;
        var cart = await _cartService.RemoveProductFromCartAsync(customerId, productId);
        return Ok(cart);
    }

    [HttpPost("services")]
    [Authorize(Roles = RoleConstants.CUSTOMER)]
    public async Task<IActionResult> AddService([FromBody] ServiceOrderModel service)
    {
        var customerId = _claimsService.GetCurrentUser;
        var cart = await _cartService.AddServiceToCartAsync(customerId, service);
        return Ok(cart);
    }

    [HttpDelete("services/{slotId:guid}")]
    [Authorize(Roles = RoleConstants.CUSTOMER)]
    public async Task<IActionResult> RemoveService([FromRoute] Guid slotId)
    {
        var customerId = _claimsService.GetCurrentUser;
        var cart = await _cartService.RemoveServiceFromCartAsync(customerId, slotId);
        return Ok(cart);
    }
}

