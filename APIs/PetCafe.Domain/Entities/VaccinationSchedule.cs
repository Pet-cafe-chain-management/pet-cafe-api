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

    [Column("daily_task_id")]
    public Guid? DailyTaskId { get; set; }

    // Navigation properties
    public virtual DailyTask? DailyTask { get; set; } = null!;
    public virtual VaccinationRecord? Record { get; set; }
    public virtual Pet Pet { get; set; } = default!;
}
