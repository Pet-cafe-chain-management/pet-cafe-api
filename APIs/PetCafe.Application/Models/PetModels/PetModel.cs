namespace PetCafe.Application.Models.PetModels;

public class PetCreateModel
{
    public string Name { get; set; } = null!;
    public int Age { get; set; }
    public Guid SpeciesId { get; set; }
    public Guid BreedId { get; set; }
    public string? Color { get; set; }
    public double? Weight { get; set; }
    public string? Preferences { get; set; }
    public string? SpecialNotes { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime ArrivalDate { get; set; }
    public string Gender { get; set; } = null!;

}

public class PetUpdateModel : PetCreateModel
{
    public Guid? GroupId { get; set; }
}