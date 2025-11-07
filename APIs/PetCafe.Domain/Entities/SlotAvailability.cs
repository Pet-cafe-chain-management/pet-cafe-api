using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetCafe.Domain.Entities;

[Table("slot_availabilities")]
public class SlotAvailability : BaseEntity
{
    [Column("slot_id")]
    [Required]
    public Guid SlotId { get; set; }

    [Column("booking_date")]
    [Required]
    public DateOnly BookingDate { get; set; }

    [Column("booked_count")]
    [Required]
    public int BookedCount { get; set; } = 0;

    [Column("max_capacity")]
    [Required]
    public int MaxCapacity { get; set; }

    // Navigation property
    [ForeignKey("SlotId")]
    public virtual Slot Slot { get; set; } = null!;
}

