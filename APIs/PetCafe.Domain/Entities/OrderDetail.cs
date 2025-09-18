using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetCafe.Domain.Entities;

[Table("order_details")]
public class OrderDetail : BaseEntity
{
    [Column("order_id")]
    [ForeignKey("Order")]
    public Guid OrderId { get; set; }

    [Column("product_id")]
    [ForeignKey("Product")]
    public Guid ProductId { get; set; }

    [Column("quantity")]
    public int Quantity { get; set; }

    [Column("unit_price")]
    public double UnitPrice { get; set; }

    [Column("total_price")]
    public double TotalPrice { get; set; }

    [Column("is_for_feeding")]
    public bool IsForFeeding { get; set; } = false;

    [Column("notes")]
    [MaxLength(200)]
    public string? Notes { get; set; }

    // Navigation properties
    public virtual Order Order { get; set; } = default!;
    public virtual Product Product { get; set; } = default!;
}
