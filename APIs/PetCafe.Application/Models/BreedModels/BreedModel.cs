namespace PetCafe.Application.Models.BreedModels;

public class BreedCreateModel
{
    public string Name { get; set; } = default!;
    public Guid SpeciesId { get; set; }
    public string? Description { get; set; }
    public double AverageWeight { get; set; }
    public int? AverageLifespan { get; set; }

}

public class BreedUpdateModel : BreedCreateModel
{
}