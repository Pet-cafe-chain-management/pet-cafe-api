namespace PetCafe.Application.Models.VaccineTypeModels;

public class VaccineTypeCreateModel
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public Guid? SpeciesId { get; set; }
    public int IntervalMonths { get; set; }
    public bool IsRequired { get; set; } = true;
}

public class VaccineTypeUpdateModel : VaccineTypeCreateModel
{
}