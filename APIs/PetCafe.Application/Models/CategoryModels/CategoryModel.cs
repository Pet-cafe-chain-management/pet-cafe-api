using FluentValidation;
using PetCafe.Application.Models.ShareModels;

namespace PetCafe.Application.Models.CategoryModels;

public class CategoryCreateModel
{
    public required string Name { get; set; } = default!;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
}

public class CategoryUpdateModel : CategoryCreateModel
{
    public bool IsActive { get; set; }
}

public class CategoryFilterQuery : FilterQuery
{
    public bool IsActive { get; set; } = true;

}

public class CategoryCreateModelValidator : AbstractValidator<CategoryCreateModel>
{
    public CategoryCreateModelValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Tên danh mục không được để trống")
            .MaximumLength(100).WithMessage("Tên danh mục không được vượt quá 100 ký tự");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Mô tả không được vượt quá 500 ký tự")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.ImageUrl)
            .MaximumLength(1000).WithMessage("Đường dẫn ảnh không được vượt quá 1000 ký tự")
            .When(x => !string.IsNullOrEmpty(x.ImageUrl));
    }
}

public class CategoryUpdateModelValidator : AbstractValidator<CategoryUpdateModel>
{
    public CategoryUpdateModelValidator()
    {
        Include(new CategoryCreateModelValidator());
    }
}