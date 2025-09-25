namespace PetCafe.Application.Models.PetSpeciesModels;

public class PetSpeciesCreateModel
{
    public string Name { get; set; } = null!; // "Dog", "Cat", "Bird", "Rabbit", etc.
    public string? Description { get; set; }
}
public class PetSpeciesUpdateModel : PetSpeciesCreateModel
{
    public bool IsActive { get; set; }
}