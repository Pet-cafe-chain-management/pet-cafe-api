using Microsoft.AspNetCore.Mvc;
using PetCafe.Application.Models.ShareModels;

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

public class VaccineTypeFilterQuery : FilterQuery
{
    [FromQuery(Name = "is_required")]
    public bool? IsRequired { get; set; }
    [FromQuery(Name = "name")]
    public string? Name { get; set; }
    [FromQuery(Name = "species_id")]
    public Guid? SpeciesId { get; set; }
}