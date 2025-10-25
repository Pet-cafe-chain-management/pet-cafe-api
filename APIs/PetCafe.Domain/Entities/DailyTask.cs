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

    [Column("status")]
    public string Status { get; set; } = DailyTaskStatusConstant.SCHEDULED;

    [Column("assigned_date")]
    public DateTime AssignedDate { get; set; }

    [Column("start_date")]
    public DateTime? StartDate { get; set; }

    [Column("completion_date")]
    public DateTime? CompletionDate { get; set; }

    [Column("task_id")]
    [ForeignKey("Task")]
    public Guid TaskId { get; set; }

    [Column("slot_id")]
    [ForeignKey("Slot")]
    public Guid SlotId { get; set; }

    [Column("notes")]
    [MaxLength(500)]
    public string? Notes { get; set; }

    // Navigation properties
    public virtual Task Task { get; set; } = default!;
    public virtual Slot Slot { get; set; } = default!;
    public virtual Team Team { get; set; } = default!;
}