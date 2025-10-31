using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetCafe.Domain.Entities;

[Table("service_feedbacks")]
public class ServiceFeedback : BaseEntity
{
    [Column("customer_booking_id")]
    [ForeignKey("CustomerBooking")]
    public Guid CustomerBookingId { get; set; }

    [Column("service_id")]
    [ForeignKey("Service")]
    public Guid ServiceId { get; set; }

    [Column("customer_id")]
    [ForeignKey("Customer")]
    public Guid CustomerId { get; set; }

    [Column("rating")]
    public int Rating { get; set; } // 1-5 stars

    [Column("comment")]
    [MaxLength(1000)]
    public string? Comment { get; set; }

    [Column("feedback_date")]
    public DateTime FeedbackDate { get; set; }

    // Navigation properties
    public virtual CustomerBooking CustomerBooking { get; set; } = default!;
    public virtual Service Service { get; set; } = default!;
    public virtual Customer Customer { get; set; } = default!;
}

