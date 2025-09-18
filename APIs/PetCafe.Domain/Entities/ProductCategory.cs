using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetCafe.Domain.Entities;

[Table("product_categories")]
public class ProductCategory : BaseEntity
{
    [Column("name")]
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = default!;

    [Column("description")]
    [MaxLength(200)]
    public string? Description { get; set; }

    [Column("image_url")]
    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public virtual ICollection<Product> Products { get; set; } = [];
}
