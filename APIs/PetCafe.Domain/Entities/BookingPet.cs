using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetCafe.Domain.Entities;

[Table("booking_pets")]
public class BookingPet : BaseEntity
{
    [Column("service_booking_id")]
    [ForeignKey("ServiceBooking")]
    public Guid ServiceBookingId { get; set; }

    [Column("pet_id")]
    [ForeignKey("Pet")]
    public Guid PetId { get; set; }

    [Column("notes")]
    [MaxLength(500)]
    public string? Notes { get; set; }

    // Navigation properties
    public virtual ServiceBooking ServiceBooking { get; set; } = default!;
    public virtual Pet Pet { get; set; } = default!;
}