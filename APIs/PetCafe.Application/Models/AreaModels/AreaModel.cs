using FluentValidation;
using PetCafe.Application.Models.ShareModels;

namespace PetCafe.Application.Models.AreaModels;

public class AreaCreateModel
{
    public required string Name { get; set; } = default!;
    public string? Description { get; set; }
    public string? Location { get; set; }
    public int MaxCapacity { get; set; } = 0;
    public string? ImageUrl { get; set; }
    public List<Guid>? WorkTypeIds { get; set; } = [];
}

public class AreaUpdateModel : AreaCreateModel
{
    public bool IsActive { get; set; }
}

public class AreaFilterQuery : FilterQuery
{
    public bool IsActive { get; set; } = true;
    public Guid? WorkTypeId { get; set; }

}
public class AreaCreateModelValidator : AbstractValidator<AreaCreateModel>
{
    public AreaCreateModelValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Tên khu vực không được để trống")
            .MaximumLength(100).WithMessage("Tên khu vực không được vượt quá 100 ký tự");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Mô tả không được vượt quá 500 ký tự")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.Location)
            .MaximumLength(200).WithMessage("Vị trí không được vượt quá 200 ký tự")
            .When(x => !string.IsNullOrEmpty(x.Location));

        RuleFor(x => x.MaxCapacity)
            .GreaterThanOrEqualTo(0).WithMessage("Sức chứa tối đa phải lớn hơn hoặc bằng 0");

        RuleFor(x => x.ImageUrl)
            .MaximumLength(1000).WithMessage("Đường dẫn ảnh không được vượt quá 1000 ký tự")
            .When(x => !string.IsNullOrEmpty(x.ImageUrl));
    }
}

public class AreaUpdateModelValidator : AbstractValidator<AreaUpdateModel>
{
    public AreaUpdateModelValidator()
    {
        Include(new AreaCreateModelValidator());
    }
}