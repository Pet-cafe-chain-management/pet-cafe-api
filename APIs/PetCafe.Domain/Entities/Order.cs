using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetCafe.Domain.Entities;

[Table("orders")]
public class Order : BaseEntity
{
    [Column("customer_id")]
    [ForeignKey("Customer")]
    public Guid CustomerId { get; set; }

    [Column("order_number")]
    [Required]
    [MaxLength(20)]
    public string OrderNumber { get; set; } = default!;

    [Column("order_date")]
    public DateTime OrderDate { get; set; }

    [Column("total_amount")]
    public double TotalAmount { get; set; }

    [Column("discount_amount")]
    public double DiscountAmount { get; set; } = 0;

    [Column("final_amount")]
    public double FinalAmount { get; set; }

    [Column("payment_status")]
    [MaxLength(20)]
    public string PaymentStatus { get; set; } = "Pending"; // Pending, Paid, Refunded

    [Column("payment_method")]
    [MaxLength(20)]
    public string? PaymentMethod { get; set; } // Cash, Card, Mobile

    [Column("status")]
    [MaxLength(20)]
    public string Status { get; set; } = "Processing"; // Processing, Completed, Cancelled

    [Column("notes")]
    [MaxLength(500)]
    public string? Notes { get; set; }

    // Navigation properties
    public virtual Customer Customer { get; set; } = default!;
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = [];
    public virtual ICollection<StaffAssignment> StaffAssignments { get; set; } = [];
}