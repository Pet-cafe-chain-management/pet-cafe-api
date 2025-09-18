
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

    [Column("species")]
    [Required]
    [MaxLength(30)]
    public string Species { get; set; } = default!;

    [Column("duration_months")]
    public int DurationMonths { get; set; } // Validity period in months

    [Column("is_mandatory")]
    public bool IsMandatory { get; set; } = false;

    // Navigation properties
    public virtual ICollection<VaccinationRecord> VaccinationRecords { get; set; } = new List<VaccinationRecord>();
    public virtual ICollection<VaccinationSchedule> VaccinationSchedules { get; set; } = new List<VaccinationSchedule>();
}