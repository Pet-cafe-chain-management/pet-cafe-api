using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetCafe.Domain.Entities;

[Table("product_order_details")]
public class ProductOrderDetail : BaseEntity
{

    [Column("product_order_id")]
    [ForeignKey("ProductOrder")]
    public Guid ProductOrderId { get; set; }

    [Column("product_id")]
    [ForeignKey("Product")]
    public Guid? ProductId { get; set; }

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

    [Column("booking_date")]
    public DateTime? BookingDate { get; set; }

    // Navigation properties
    public virtual ProductOrder ProductOrder { get; set; } = default!;
    public virtual Product? Product { get; set; } = default!;


}
