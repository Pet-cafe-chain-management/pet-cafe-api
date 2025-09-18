using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetCafe.Domain.Entities;

[Table("pets")]
public class Pet : BaseEntity
{
    [Column("name")]
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = default!;

    [Column("pet_breed_id")]
    [ForeignKey("PetBreed")]
    public Guid PetBreedId { get; set; }

    [Column("pet_status_id")]
    [ForeignKey("PetStatus")]
    public Guid PetStatusId { get; set; }

    [Column("age")]
    public int Age { get; set; }

    [Column("gender")]
    [MaxLength(10)]
    public string? Gender { get; set; }

    [Column("weight")]
    public double? Weight { get; set; }

    [Column("color")]
    [MaxLength(30)]
    public string? Color { get; set; }

    [Column("personality")]
    [MaxLength(500)]
    public string? Personality { get; set; }

    [Column("special_notes")]
    [MaxLength(1000)]
    public string? SpecialNotes { get; set; }

    [Column("arrival_date")]
    public DateTime ArrivalDate { get; set; }

    [Column("image_url")]
    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    // Navigation properties
    public virtual PetBreed PetBreed { get; set; } = default!;
    public virtual PetStatus PetStatus { get; set; } = default!;
    public virtual ICollection<HealthRecord> HealthRecords { get; set; } = [];
    public virtual ICollection<VaccinationRecord> VaccinationRecords { get; set; } = [];
    public virtual ICollection<VaccinationSchedule> VaccinationSchedules { get; set; } = [];
    public virtual ICollection<BookingPet> BookingPets { get; set; } = [];
    public virtual ICollection<PetSpecific> PetSpecifics { get; set; } = [];
}
