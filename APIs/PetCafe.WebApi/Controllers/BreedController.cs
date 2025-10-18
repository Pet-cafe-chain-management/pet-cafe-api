using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCafe.Application.Models.BreedModels;
using PetCafe.Application.Models.ShareModels;
using PetCafe.Application.Services;
using PetCafe.Domain.Constants;

namespace PetCafe.WebApi.Controllers;

[ApiController]
[Route("api/pet-breeds")]
public class BreedController(
    IBreedService _breedService
) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = RoleConstants.MANAGER)]
    public async Task<IActionResult> CreateAsync([FromBody] BreedCreateModel model)
    {
        return Ok(await _breedService.CreateAsync(model));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = RoleConstants.MANAGER)]
    public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] BreedUpdateModel model)
    {
        var breed = await _breedService.UpdateAsync(id, model);
        return Ok(breed);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = RoleConstants.MANAGER)]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid id)
    {
        await _breedService.DeleteAsync(id);
        return NoContent();
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id)
    {

        return Ok(await _breedService.GetByIdAsync(id));
    }

    [HttpGet]
    public async Task<IActionResult> GetAllPagingAsync([FromQuery] FilterQuery query)
    {
        return Ok(await _breedService.GetAllPagingAsync(query));
    }
}