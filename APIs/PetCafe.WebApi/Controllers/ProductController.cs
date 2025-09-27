using Microsoft.AspNetCore.Mvc;
using PetCafe.Application.Models.ProductModels;
using PetCafe.Application.Models.ShareModels;
using PetCafe.Application.Services;

namespace PetCafe.WebApi.Controllers;

public class ProductController(IProductService _productService) : BaseController
{
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var product = await _productService.GetByIdAsync(id);
        return Ok(product);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllPaging([FromQuery] ProductFilterQuery query)
    {
        var products = await _productService.GetAllPagingAsync(query);
        return Ok(products);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, ProductUpdateModel model)
    {
        await _productService.UpdateAsync(id, model);
        return NoContent();
    }
    [HttpPost]
    public async Task<IActionResult> Create(ProductCreateModel model)
    {
        var product = await _productService.CreateAsync(model);
        return Ok(product);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _productService.DeleteAsync(id);
        return NoContent();
    }
}