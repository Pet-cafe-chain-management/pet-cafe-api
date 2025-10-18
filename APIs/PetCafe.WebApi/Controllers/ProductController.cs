using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCafe.Application.Models.ProductModels;
using PetCafe.Application.Models.ShareModels;
using PetCafe.Application.Services;
using PetCafe.Domain.Constants;

namespace PetCafe.WebApi.Controllers;

public class ProductController(IProductService _productService) : BaseController
{
    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> GetById(Guid id)
    {
        var product = await _productService.GetByIdAsync(id);
        return Ok(product);
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAllPaging([FromQuery] ProductFilterQuery query)
    {
        var products = await _productService.GetAllPagingAsync(query);
        return Ok(products);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = RoleConstants.MANAGER)]
    public async Task<IActionResult> Update(Guid id, ProductUpdateModel model)
    {
        await _productService.UpdateAsync(id, model);
        return NoContent();
    }
    [HttpPost]
    [Authorize(Roles = RoleConstants.MANAGER)]
    public async Task<IActionResult> Create(ProductCreateModel model)
    {
        var product = await _productService.CreateAsync(model);
        return Ok(product);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = RoleConstants.MANAGER)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _productService.DeleteAsync(id);
        return NoContent();
    }
}