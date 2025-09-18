using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetCafe.Domain.Entities;

[Table("products")]
public class Product : BaseEntity
{
    [Column("name")]
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = default!;

    [Column("product_category_id")]
    [ForeignKey("ProductCategory")]
    public Guid ProductCategoryId { get; set; }

    [Column("description")]
    [MaxLength(500)]
    public string? Description { get; set; }

    [Column("price")]
    public double Price { get; set; }

    [Column("cost")]
    public double? Cost { get; set; }

    [Column("stock_quantity")]
    public int StockQuantity { get; set; } = 0;

    [Column("min_stock_level")]
    public int MinStockLevel { get; set; } = 0;

    [Column("image_url")]
    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("is_for_pets")]
    public bool IsForPets { get; set; } = false;

    // Navigation properties
    public virtual ProductCategory ProductCategory { get; set; } = default!;
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = [];
}