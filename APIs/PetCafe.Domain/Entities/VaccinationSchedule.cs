using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PetCafe.Domain.Constants;

namespace PetCafe.Domain.Entities;

[Table("vaccination_schedules")]
public class VaccinationSchedule : BaseEntity
{
    [Column("pet_id")]
    [ForeignKey("Pet")]
    public Guid PetId { get; set; }

    [Column("vaccine_type_id")]
    [ForeignKey("VaccineType")]
    public Guid VaccineTypeId { get; set; }

    [Column("scheduled_date")]
    public DateTime ScheduledDate { get; set; }

    [Column("status")]
    public string Status { get; set; } = VaccinationScheduleStatus.PENDING;

    [Column("completed_date")]
    public DateTime? CompletedDate { get; set; }

    [Column("notes")]
    [MaxLength(500)]
    public string? Notes { get; set; }

    [Column("record_id")]
    public Guid? RecordId { get; set; }
    // Navigation properties
    public virtual VaccinationRecord? Record { get; set; }
    public virtual Pet Pet { get; set; } = default!;
    public virtual VaccineType VaccineType { get; set; } = default!;
}
