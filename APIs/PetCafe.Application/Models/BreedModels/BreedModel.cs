using FluentValidation;

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

public class BreedCreateModelValidator : AbstractValidator<BreedCreateModel>
{
    public BreedCreateModelValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Tên giống không được để trống")
            .MaximumLength(100).WithMessage("Tên giống không được vượt quá 100 ký tự");

        RuleFor(x => x.SpeciesId)
            .NotEmpty().WithMessage("ID loài không được để trống");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Mô tả không được vượt quá 500 ký tự")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.AverageWeight)
            .GreaterThan(0).WithMessage("Cân nặng trung bình phải lớn hơn 0");

        RuleFor(x => x.AverageLifespan)
            .GreaterThan(0).WithMessage("Tuổi thọ trung bình phải lớn hơn 0")
            .When(x => x.AverageLifespan.HasValue);
    }
}

public class BreedUpdateModelValidator : AbstractValidator<BreedUpdateModel>
{
    public BreedUpdateModelValidator()
    {
        Include(new BreedCreateModelValidator());
    }
}