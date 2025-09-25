using Microsoft.AspNetCore.Mvc;
using PetCafe.Application.Models.PetModels;
using PetCafe.Application.Models.ShareModels;
using PetCafe.Application.Services;

namespace PetCafe.WebApi.Controllers;

public class PetController(IPetService _petService) : BaseController
{

    [HttpPost]
    public async Task<IActionResult> CreatePet([FromBody] PetCreateModel model)
    {
        var pet = await _petService.CreateAsync(model);
        return Ok(pet);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdatePet([FromRoute] Guid id, [FromBody] PetUpdateModel model)
    {
        var pet = await _petService.UpdateAsync(id, model);
        return Ok(pet);
    }

    [HttpDelete("{id:guid}")]
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
}