using FluentValidation;
using PetCafe.Application.Models.ShareModels;

namespace PetCafe.Application.Models.ProductModels;

public class ProductCreateModel
{
    public string Name { get; set; } = default!;
    public Guid CategoryId { get; set; }
    public string? Description { get; set; }
    public double Price { get; set; }
    public double? Cost { get; set; }
    public int StockQuantity { get; set; } = 0;
    public int MinStockLevel { get; set; } = 0;
    public string? ImageUrl { get; set; }
    public bool IsForPets { get; set; } = false;
    public List<string> Thumbnails { get; set; } = [];

}


public class ProductUpdateModel : ProductCreateModel
{
    public bool IsActive { get; set; } = true;
}

public class ProductFilterQuery : FilterQuery
{
    public bool? IsActive { get; set; }
    public int? MinPrice { get; set; }
    public int? MaxPrice { get; set; }

    public int? MinCost { get; set; }
    public int? MaxCost { get; set; }

    public bool? IsForPets { get; set; }
    public int MinStockQuantity { get; set; } = 0;
    public int MaxStockQuantity { get; set; } = int.MaxValue;

}

public class ProductCreateModelValidator : AbstractValidator<ProductCreateModel>
{
    public ProductCreateModelValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Tên sản phẩm không được để trống")
            .MaximumLength(100).WithMessage("Tên sản phẩm không được vượt quá 100 ký tự");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("ID danh mục không được để trống");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Mô tả không được vượt quá 1000 ký tự")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0).WithMessage("Giá bán phải lớn hơn hoặc bằng 0");

        RuleFor(x => x.Cost)
            .GreaterThanOrEqualTo(0).WithMessage("Giá vốn phải lớn hơn hoặc bằng 0")
            .When(x => x.Cost.HasValue);

        RuleFor(x => x)
            .Must(x => !x.Cost.HasValue || x.Price >= x.Cost.Value)
            .WithMessage("Giá bán phải lớn hơn hoặc bằng giá vốn")
            .When(x => x.Cost.HasValue);

        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("Số lượng tồn kho phải lớn hơn hoặc bằng 0");

        RuleFor(x => x.MinStockLevel)
            .GreaterThanOrEqualTo(0).WithMessage("Mức tồn kho tối thiểu phải lớn hơn hoặc bằng 0");

        RuleFor(x => x.ImageUrl)
            .MaximumLength(1000).WithMessage("Đường dẫn ảnh không được vượt quá 1000 ký tự")
            .When(x => !string.IsNullOrEmpty(x.ImageUrl));

        RuleForEach(x => x.Thumbnails)
            .NotEmpty().WithMessage("Đường dẫn ảnh thumbnail không được để trống")
            .MaximumLength(1000).WithMessage("Đường dẫn ảnh thumbnail không được vượt quá 1000 ký tự");
    }
}

public class ProductUpdateModelValidator : AbstractValidator<ProductUpdateModel>
{
    public ProductUpdateModelValidator()
    {
        Include(new ProductCreateModelValidator());
    }
}