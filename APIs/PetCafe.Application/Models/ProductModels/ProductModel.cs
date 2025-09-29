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