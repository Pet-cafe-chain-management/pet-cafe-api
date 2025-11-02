using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PetCafe.Domain.Constants;

namespace PetCafe.Domain.Entities;

[Table("pets")]
public class Pet : BaseEntity
{

    [Column("name")]
    [MaxLength(50)]
    public string Name { get; set; } = null!;

    [Column("species_id")]
    public Guid SpeciesId { get; set; }

    [Column("breed_id")]
    public Guid BreedId { get; set; }

    [Column("group_id")]
    public Guid? GroupId { get; set; }


    [Column("age")]
    public int Age { get; set; }

    [Column("gender")]
    [MaxLength(10)]
    public string Gender { get; set; } = null!;

    [Column("color")]
    [MaxLength(30)]
    public string? Color { get; set; }

    [Column("weight")]
    public double? Weight { get; set; }

    [Column("preferences")]
    [MaxLength(500)]
    public string? Preferences { get; set; }

    [Column("special_notes")]
    [MaxLength(1000)]
    public string? SpecialNotes { get; set; }

    [Column("image_url")]
    [MaxLength(255)]
    public string? ImageUrl { get; set; }

    [Column("health_status")]
    [MaxLength(20)]
    public string HealthStatus { get; set; } = HealthStatusConstant.HEALTHY;

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("arrival_date")]
    public DateTime ArrivalDate { get; set; }

    // Navigation properties
    [ForeignKey("SpeciesId")]
    public virtual PetSpecies Species { get; set; } = null!;

    [ForeignKey("BreedId")]
    public virtual PetBreed Breed { get; set; } = null!;

    [ForeignKey("GroupId")]
    public virtual PetGroup? Group { get; set; }
    public virtual ICollection<HealthRecord> HealthRecords { get; set; } = [];
    public virtual ICollection<VaccinationRecord> VaccinationRecords { get; set; } = [];
    public virtual ICollection<VaccinationSchedule> VaccinationSchedules { get; set; } = [];
    public virtual ICollection<Slot> Slots { get; set; } = [];
}
