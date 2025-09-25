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
}


public class ProductUpdateModel : ProductCreateModel
{
    public bool IsActive { get; set; } = true;
}