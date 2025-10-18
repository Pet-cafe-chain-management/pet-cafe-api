using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCafe.Application.Models.PetModels;
using PetCafe.Application.Models.ShareModels;
using PetCafe.Application.Services;
using PetCafe.Domain.Constants;

namespace PetCafe.WebApi.Controllers;

public class PetController(
    IPetService _petService,
    IHealthRecordService _healthRecordService,
    IVaccinationRecordService _vaccinationRecordService) : BaseController
{

    [HttpPost]
    [Authorize(Roles = RoleConstants.MANAGER)]
    public async Task<IActionResult> CreatePet([FromBody] PetCreateModel model)
    {
        var pet = await _petService.CreateAsync(model);
        return Ok(pet);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = RoleConstants.MANAGER)]
    public async Task<IActionResult> UpdatePet([FromRoute] Guid id, [FromBody] PetUpdateModel model)
    {
        var pet = await _petService.UpdateAsync(id, model);
        return Ok(pet);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = RoleConstants.MANAGER)]
    public async Task<IActionResult> DeletePet([FromRoute] Guid id)
    {
        await _petService.DeleteAsync(id);
        return NoContent();
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetPetById([FromRoute] Guid id)
    {
        var pet = await _petService.GetByIdAsync(id);
        return Ok(pet);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllPets([FromQuery] FilterQuery query)
    {
        var petList = await _petService.GetAllPagingAsync(query);
        return Ok(petList);
    }

    [HttpGet("{id:guid}/health-records")]
    public async Task<IActionResult> GetAllByPetId([FromRoute] Guid id, [FromQuery] FilterQuery query)
    {
        var records = await _healthRecordService.GetAllPagingAsync(id, query);
        return Ok(records);
    }

    [HttpGet("{id:guid}/vaccination-records")]
    public async Task<IActionResult> GetAllVaccinationRecordsByPetId([FromRoute] Guid id, [FromQuery] FilterQuery query)
    {
        var records = await _vaccinationRecordService.GetAllByPetIdAsync(id, query);
        return Ok(records);
    }

}