using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCafe.Application.Models.AreaModels;
using PetCafe.Application.Models.ShareModels;
using PetCafe.Application.Models.WorkTypeModels;
using PetCafe.Application.Services;

namespace PetCafe.WebApi.Controllers;

[ApiController]
[Route("api/work-types")]
public class WorkTypeController(IWorkTypeService _workTypeService) : ControllerBase
{
    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> GetById(Guid id)
    {
        var workType = await _workTypeService.GetByIdAsync(id);
        return Ok(workType);
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAllPaging([FromQuery] FilterQuery query)
    {
        var workTypes = await _workTypeService.GetAllPagingAsync(query);
        return Ok(workTypes);
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Update(Guid id, WorkTypeUpdateModel model)
    {
        await _workTypeService.UpdateAsync(id, model);
        return NoContent();
    }


    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create(WorkTypeCreateModel model)
    {
        var workType = await _workTypeService.CreateAsync(model);
        return Ok(workType);
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _workTypeService.DeleteAsync(id);
        return NoContent();
    }

}