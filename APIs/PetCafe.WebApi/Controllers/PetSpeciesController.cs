using Microsoft.AspNetCore.Mvc;
using PetCafe.Application.Models.PetSpeciesModels;
using PetCafe.Application.Models.ShareModels;
using PetCafe.Application.Services;

namespace PetCafe.WebApi.Controllers;

public class PetSpeciesController(IPetSpeciesService _petSpeciesService) : BaseController
{

    [HttpPost]
    public async Task<IActionResult> CreatePetSpecies([FromBody] PetSpeciesCreateModel model)
    {
        var petSpecies = await _petSpeciesService.CreatePetSpeciesAsync(model);
        return Ok(petSpecies);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdatePetSpecies([FromRoute] Guid id, [FromBody] PetSpeciesUpdateModel model)
    {
        await _petSpeciesService.UpdatePetSpeciesAsync(id, model);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeletePetSpecies([FromRoute] Guid id)
    {
        await _petSpeciesService.DeletePetSpeciesAsync(id);
        return NoContent();
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetPetSpeciesById([FromRoute] Guid id)
    {
        var petSpecies = await _petSpeciesService.GetPetSpeciesByIdAsync(id);
        return Ok(petSpecies);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllPetSpecies([FromQuery] FilterQuery query)
    {
        var petSpeciesList = await _petSpeciesService.GetAllPetSpeciesAsync(query);
        return Ok(petSpeciesList);
    }
}