using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PetCafe.Domain.Constants;

namespace PetCafe.Domain.Entities;

[Table("daily_tasks")]
public class DailyTask : BaseEntity
{
    [Column("team_id")]
    [ForeignKey("Team")]
    public Guid TeamId { get; set; }

    [Column("title")]
    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = default!;

    [Column("description")]
    public string? Description { get; set; }

    [Column("priority")]
    [MaxLength(10)]
    public string Priority { get; set; } = TaskPriorityConstant.MEDIUM;

    [Column("status")]
    public string Status { get; set; } = DailyTaskStatusConstant.SCHEDULED;

    [Column("assigned_date")]
    public DateTime AssignedDate { get; set; }

    [Column("start_time")]
    public TimeSpan StartTime { get; set; }

    [Column("end_time")]
    public TimeSpan? EndTime { get; set; }

    [Column("completion_date")]
    public DateTime? CompletionDate { get; set; }

    [Column("task_id")]
    [ForeignKey("Task")]
    public Guid? TaskId { get; set; }

    [Column("slot_id")]
    [ForeignKey("Slot")]
    public Guid? SlotId { get; set; }

    [Column("vaccination_schedule_id")]
    [ForeignKey("VaccinationSchedule")]
    public Guid? VaccinationScheduleId { get; set; }

    [Column("notes")]
    [MaxLength(500)]
    public string? Notes { get; set; }

    // Navigation properties

    public virtual VaccinationSchedule? VaccinationSchedule { get; set; } = null!;
    public virtual Task? Task { get; set; } = null!;
    public virtual Slot? Slot { get; set; } = null!;
    public virtual Team Team { get; set; } = null!;
}