using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCafe.Application.Models.CategoryModels;
using PetCafe.Application.Models.ShareModels;
using PetCafe.Application.Services;
using PetCafe.Domain.Constants;

namespace PetCafe.WebApi.Controllers;

[ApiController]
[Route("api/product-categories")]
public class CategoryController(
    ICategoryService _categoryService
) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = RoleConstants.MANAGER)]
    public async Task<IActionResult> CreateAsync([FromBody] CategoryCreateModel model)
    {
        var category = await _categoryService.CreateAsync(model);
        return Ok(category);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = RoleConstants.MANAGER)]
    public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] CategoryUpdateModel model)
    {
        var category = await _categoryService.UpdateAsync(id, model);
        return Ok(category);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = RoleConstants.MANAGER)]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid id)
    {
        var result = await _categoryService.DeleteAsync(id);
        return Ok(new { Success = result });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id)
    {
        var category = await _categoryService.GetByIdAsync(id);
        if (category == null)
        {
            return NotFound();
        }
        return Ok(category);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllPagingAsync([FromQuery] CategoryFilterQuery query)
    {
        var categories = await _categoryService.GetAllPagingAsync(query);
        return Ok(categories);
    }
}