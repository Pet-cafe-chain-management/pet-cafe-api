using FluentValidation;

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

public class PetGroupCreateModelValidator : AbstractValidator<PetGroupCreateModel>
{
    public PetGroupCreateModelValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Tên nhóm thú cưng không được để trống")
            .MaximumLength(100).WithMessage("Tên nhóm thú cưng không được vượt quá 100 ký tự");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Mô tả không được vượt quá 500 ký tự")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.MaxCapacity)
            .GreaterThan(0).WithMessage("Sức chứa tối đa phải lớn hơn 0");

        RuleFor(x => x.PetBreedId)
            .Must((model, breedId) => !breedId.HasValue || model.PetSpeciesId.HasValue)
            .WithMessage("Phải chọn loài thú cưng khi chọn giống thú cưng");
    }
}

public class PetGroupUpdateModelValidator : AbstractValidator<PetGroupUpdateModel>
{
    public PetGroupUpdateModelValidator()
    {
        Include(new PetGroupCreateModelValidator());
    }
}