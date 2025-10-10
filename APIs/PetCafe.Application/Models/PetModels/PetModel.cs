using FluentValidation;

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

public class PetCreateModelValidator : AbstractValidator<PetCreateModel>
{
    public PetCreateModelValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Tên thú cưng không được để trống")
            .MaximumLength(100).WithMessage("Tên thú cưng không được vượt quá 100 ký tự");

        RuleFor(x => x.Age)
            .GreaterThanOrEqualTo(0).WithMessage("Tuổi thú cưng phải lớn hơn hoặc bằng 0");

        RuleFor(x => x.SpeciesId)
            .NotEmpty().WithMessage("ID loài không được để trống");

        RuleFor(x => x.BreedId)
            .NotEmpty().WithMessage("ID giống không được để trống");

        RuleFor(x => x.Color)
            .MaximumLength(50).WithMessage("Màu sắc không được vượt quá 50 ký tự")
            .When(x => !string.IsNullOrEmpty(x.Color));

        RuleFor(x => x.Weight)
            .GreaterThan(0).WithMessage("Cân nặng phải lớn hơn 0")
            .When(x => x.Weight.HasValue);

        RuleFor(x => x.Preferences)
            .MaximumLength(500).WithMessage("Sở thích không được vượt quá 500 ký tự")
            .When(x => !string.IsNullOrEmpty(x.Preferences));

        RuleFor(x => x.SpecialNotes)
            .MaximumLength(500).WithMessage("Ghi chú đặc biệt không được vượt quá 500 ký tự")
            .When(x => !string.IsNullOrEmpty(x.SpecialNotes));

        RuleFor(x => x.ImageUrl)
            .MaximumLength(1000).WithMessage("Đường dẫn ảnh không được vượt quá 1000 ký tự")
            .When(x => !string.IsNullOrEmpty(x.ImageUrl));

        RuleFor(x => x.ArrivalDate)
            .NotEmpty().WithMessage("Ngày đến không được để trống")
            .LessThanOrEqualTo(DateTime.Now).WithMessage("Ngày đến không được lớn hơn ngày hiện tại");

        RuleFor(x => x.Gender)
            .NotEmpty().WithMessage("Giới tính không được để trống")
            .Must(gender => gender == "Male" || gender == "Female")
            .WithMessage("Giới tính phải là 'Male' hoặc 'Female'");
    }
}

public class PetUpdateModelValidator : AbstractValidator<PetUpdateModel>
{
    public PetUpdateModelValidator()
    {
        Include(new PetCreateModelValidator());
    }
}