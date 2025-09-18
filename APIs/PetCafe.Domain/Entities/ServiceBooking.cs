using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetCafe.Domain.Entities;

[Table("service_bookings")]
public class ServiceBooking : BaseEntity
{
    [Column("customer_id")]
    [ForeignKey("Customer")]
    public Guid CustomerId { get; set; }

    [Column("service_id")]
    [ForeignKey("Service")]
    public Guid ServiceId { get; set; }

    [Column("area_id")]
    [ForeignKey("Area")]
    public Guid? AreaId { get; set; } // Khu vực được chọn cho service

    [Column("booking_number")]
    [Required]
    [MaxLength(20)]
    public string BookingNumber { get; set; } = default!;

    [Column("booking_date")]
    public DateTime BookingDate { get; set; }

    [Column("scheduled_date")]
    public DateTime ScheduledDate { get; set; }

    [Column("scheduled_time")]
    public TimeSpan ScheduledTime { get; set; }

    [Column("participants")]
    public int Participants { get; set; } = 1;

    [Column("total_amount")]
    public double TotalAmount { get; set; }

    [Column("base_amount")]
    public double BaseAmount { get; set; } // Giá gốc của service

    [Column("area_adjustment")]
    public double AreaAdjustment { get; set; } = 0; // Điều chỉnh giá theo khu vực

    [Column("payment_status")]
    [MaxLength(20)]
    public string PaymentStatus { get; set; } = "Pending"; // Pending, Paid, Refunded

    [Column("booking_status")]
    [MaxLength(20)]
    public string BookingStatus { get; set; } = "Pending"; // Pending, Confirmed, InProgress, Completed, Cancelled

    [Column("special_requests")]
    [MaxLength(500)]
    public string? SpecialRequests { get; set; }

    [Column("completion_date")]
    public DateTime? CompletionDate { get; set; }

    [Column("feedback_rating")]
    public int? FeedbackRating { get; set; } // 1-5 stars

    [Column("feedback_comment")]
    [MaxLength(1000)]
    public string? FeedbackComment { get; set; }

    [Column("feedback_date")]
    public DateTime? FeedbackDate { get; set; }

    // Navigation properties
    public virtual Customer Customer { get; set; } = default!;
    public virtual Service Service { get; set; } = default!;
    public virtual Area? Area { get; set; }
    public virtual ICollection<BookingPet> BookingPets { get; set; } = [];
}

