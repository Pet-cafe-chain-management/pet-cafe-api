using FluentValidation;
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
    public int RequiredDoses { get; set; } = 1;

}

public class VaccineTypeUpdateModel : VaccineTypeCreateModel
{
}

public class VaccineTypeCreateModelValidator : AbstractValidator<VaccineTypeCreateModel>
{
    public VaccineTypeCreateModelValidator()
    {
        RuleFor(x => x.RequiredDoses)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Số liều vaccine yêu cầu phải lớn hơn hoặc bằng 1");
    }
}

public class VaccineTypeUpdateModelValidator : AbstractValidator<VaccineTypeUpdateModel>
{
    public VaccineTypeUpdateModelValidator()
    {
        Include(new VaccineTypeCreateModelValidator());
    }
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