
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetCafe.Domain.Entities;

[Table("vaccine_types")]
public class VaccineType : BaseEntity
{
    [Column("name")]
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = default!;

    [Column("description")]
    [MaxLength(500)]
    public string? Description { get; set; }

    [Column("species_id")]
    public Guid? SpeciesId { get; set; }

    [Column("interval_months")]
    public int IntervalMonths { get; set; }

    [Column("is_required")]
    public bool IsRequired { get; set; } = true;

    // Navigation properties
    [ForeignKey("SpeciesId")]
    public virtual PetSpecies? Species { get; set; }
    public virtual ICollection<VaccinationRecord> VaccinationRecords { get; set; } = [];
    public virtual ICollection<VaccinationSchedule> VaccinationSchedules { get; set; } = [];
}