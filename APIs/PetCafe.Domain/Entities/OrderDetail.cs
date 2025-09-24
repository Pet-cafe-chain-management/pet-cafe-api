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
    public Guid? ProductId { get; set; }

    [Column("service_id")]
    [ForeignKey("Service")]
    public Guid? ServiceId { get; set; }

    [Column("slot_id")]
    [ForeignKey("Slot")]
    public Guid? SlotId { get; set; }

    [Column("booking_id")]
    public Guid? BookingId { get; set; }

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
    public virtual Product? Product { get; set; } = default!;
    public virtual CustomerBooking? Booking { get; set; } = default!;
    public virtual Slot? Slot { get; set; } = default!;
    public virtual Service? Service { get; set; } = default!;

}
