using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetCafe.Domain.Entities;

[Table("vaccination_records")]
public class VaccinationRecord : BaseEntity
{
    [Column("pet_id")]
    [ForeignKey("Pet")]
    public Guid PetId { get; set; }

    [Column("vaccine_type_id")]
    [ForeignKey("VaccineType")]
    public Guid VaccineTypeId { get; set; }

    [Column("vaccination_date")]
    public DateTime VaccinationDate { get; set; }

    [Column("next_due_date")]
    public DateTime? NextDueDate { get; set; }

    [Column("veterinarian")]
    [MaxLength(100)]
    public string? Veterinarian { get; set; }

    [Column("clinic_name")]
    [MaxLength(100)]
    public string? ClinicName { get; set; }

    [Column("batch_number")]
    [MaxLength(50)]
    public string? BatchNumber { get; set; }

    [Column("notes")]
    [MaxLength(500)]
    public string? Notes { get; set; }

    // Navigation properties
    public virtual Pet Pet { get; set; } = default!;
    public virtual VaccineType VaccineType { get; set; } = default!;
}
