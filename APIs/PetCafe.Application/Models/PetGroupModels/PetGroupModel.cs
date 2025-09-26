namespace PetCafe.Application.Models.PetGroupModels;

public class PetGroupCreateModel
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public int MaxCapacity { get; set; }
    public Guid? PetSpeciesId { get; set; }
    public Guid? PetBreedId { get; set; }
}

public class PetGroupUpdateModel : PetGroupCreateModel
{
}