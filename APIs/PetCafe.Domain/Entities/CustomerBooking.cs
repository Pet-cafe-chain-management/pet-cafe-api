using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetCafe.Domain.Entities;

[Table("customer_bookings")]
public class CustomerBooking : BaseEntity
{
    [Column("customer_id")]
    [ForeignKey("Customer")]
    public Guid? CustomerId { get; set; }

    [Column("service_id")]
    [ForeignKey("Service")]
    public Guid ServiceId { get; set; }

    [Column("slot_id")]
    [ForeignKey("Slot")]
    public Guid SlotId { get; set; }

    [Column("pet_group_id")]
    [ForeignKey("PetGroup")]
    public Guid PetGroupId { get; set; }

    [Column("order_detail_id")]
    public Guid OrderDetailId { get; set; }

    [Column("booking_date")]
    public DateTime BookingDate { get; set; }

    [Column("start_time")]
    public TimeSpan StartTime { get; set; }

    [Column("end_time")]
    public TimeSpan EndTime { get; set; }

    [Column("booking_status")]
    [MaxLength(20)]
    public string BookingStatus { get; set; } = "Pending"; // Pending, Confirmed, InProgress, Completed, Cancelled

    [Column("feedback_rating")]
    public int? FeedbackRating { get; set; } // 1-5 stars

    [Column("feedback_comment")]
    [MaxLength(1000)]
    public string? FeedbackComment { get; set; }

    [Column("feedback_date")]
    public DateTime? FeedbackDate { get; set; }

    // Navigation properties
    public virtual Customer? Customer { get; set; } = default!;
    public virtual Service Service { get; set; } = default!;
    public virtual PetGroup PetGroup { get; set; } = default!;
    public virtual ServiceOrderDetail OrderDetail { get; set; } = default!;
    public virtual Slot Slot { get; set; } = default!;
}

