using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCafe.Application.Models.AreaModels;
using PetCafe.Application.Models.ShareModels;
using PetCafe.Application.Services;
using PetCafe.Domain.Constants;

namespace PetCafe.WebApi.Controllers;

public class AreaController(IAreaService _areaService) : BaseController
{
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var area = await _areaService.GetByIdAsync(id);
        return Ok(area);
    }

    /// <summary>
    /// Lấy danh sách WorkType không thuộc Area đã chọn
    /// </summary>
    /// <param name="id">ID của Area</param>
    /// <returns>Danh sách WorkType không thuộc Area đã chọn</returns>
    [HttpGet("{id:guid}/work-types")]
    public async Task<IActionResult> GetWorkTypeNotInArea(Guid id)
    {
        var workTypes = await _areaService.GetWorkTypeNotInAreaAsync(id);
        return Ok(workTypes);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllPaging([FromQuery] AreaFilterQuery query)
    {
        var areas = await _areaService.GetAllPagingAsync(query);
        return Ok(areas);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = RoleConstants.MANAGER)]

    public async Task<IActionResult> Update(Guid id, AreaUpdateModel model)
    {
        await _areaService.UpdateAsync(id, model);
        return NoContent();
    }


    [HttpPost]
    [Authorize(Roles = RoleConstants.MANAGER)]
    public async Task<IActionResult> Create(AreaCreateModel model)
    {
        var area = await _areaService.CreateAsync(model);
        return Ok(area);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = RoleConstants.MANAGER)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _areaService.DeleteAsync(id);
        return NoContent();
    }

}