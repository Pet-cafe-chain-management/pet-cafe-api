using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetCafe.Domain.Entities;

[Table("notifications")]
public class Notification : BaseEntity
{
    [Column("account_id")]
    [ForeignKey("Account")]
    public Guid AccountId { get; set; }

    [Column("title")]
    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = default!;

    [Column("message")]
    [Required]
    [MaxLength(500)]
    public string Message { get; set; } = default!;

    [Column("notification_type")]
    [Required]
    [MaxLength(30)]
    public string NotificationType { get; set; } = default!; // Task, Vaccination, Booking, Promotion, System

    [Column("priority")]
    [MaxLength(10)]
    public string Priority { get; set; } = "Normal"; // Low, Normal, High, Urgent

    [Column("is_read")]
    public bool IsRead { get; set; } = false;

    [Column("read_date")]
    public DateTime? ReadDate { get; set; }

    [Column("reference_id")]
    public Guid? ReferenceId { get; set; } // ID of related entity (Task, Booking, etc.)

    [Column("reference_type")]
    [MaxLength(30)]
    public string? ReferenceType { get; set; } // Task, ServiceBooking, VaccinationSchedule, etc.

    [Column("scheduled_send_date")]
    public DateTime? ScheduledSendDate { get; set; }

    [Column("sent_date")]
    public DateTime? SentDate { get; set; }

    // Navigation properties
    public virtual Account Account { get; set; } = default!;
}