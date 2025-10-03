using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetCafe.Domain.Entities;

[Table("service_order_details")]
public class ServiceOrderDetail : BaseEntity
{
    [Column("service_order_id")]
    [ForeignKey("ServiceOrder")]
    public Guid ServiceOrderId { get; set; }

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

    [Column("notes")]
    [MaxLength(200)]
    public string? Notes { get; set; }

    [Column("booking_date")]
    public DateTime? BookingDate { get; set; }

    // Navigation properties
    public virtual ServiceOrder ServiceOrder { get; set; } = default!;
    public virtual CustomerBooking? Booking { get; set; } = default!;
    public virtual Slot? Slot { get; set; } = default!;
    public virtual Service? Service { get; set; } = default!;

}
