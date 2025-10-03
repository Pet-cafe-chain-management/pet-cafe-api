using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PetCafe.Domain.Constants;

namespace PetCafe.Domain.Entities;

[Table("product_orders")]
public class ProductOrder : BaseEntity
{
    [Column("order_id")]
    [ForeignKey("Order")]
    public Guid OrderId { get; set; }

    [Column("total_amount")]
    public double TotalAmount { get; set; }

    [Column("discount_amount")]
    public double DiscountAmount { get; set; } = 0;

    [Column("final_amount")]
    public double FinalAmount { get; set; }

    [Column("notes")]
    [MaxLength(200)]
    public string? Notes { get; set; }

    // Navigation properties
    public virtual Order Order { get; set; } = default!;
    public virtual ICollection<ProductOrderDetail> OrderDetails { get; set; } = [];

    [Column("status")]
    [MaxLength(20)]
    public string Status { get; set; } = OrderStatusConstant.PENDING;

}
