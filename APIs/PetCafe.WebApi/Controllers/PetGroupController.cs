using Microsoft.AspNetCore.Mvc;
using PetCafe.Application.Models.PetGroupModels;
using PetCafe.Application.Models.ShareModels;
using PetCafe.Application.Services;

namespace PetCafe.WebApi.Controllers;

[ApiController]
[Route("api/pet-groups")]
public class PetGroupController(IPetGroupService _petGroupService) : ControllerBase
{

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var petGroup = await _petGroupService.GetByIdAsync(id);
        return Ok(petGroup);
    }

    [HttpGet("paging")]
    public async Task<IActionResult> GetAllPaging([FromQuery] FilterQuery query)
    {
        var petGroups = await _petGroupService.GetAllPagingAsync(query);
        return Ok(petGroups);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PetGroupCreateModel model)
    {
        var petGroup = await _petGroupService.CreateAsync(model);
        return Ok(petGroup);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] PetGroupUpdateModel model)
    {
        await _petGroupService.UpdateAsync(id, model);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        await _petGroupService.DeleteAsync(id);
        return NoContent();
    }
}