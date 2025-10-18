using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetCafe.Domain.Entities;

[Table("daily_tasks")]
public class DailyTask : BaseEntity
{
    [Column("team_id")]
    [ForeignKey("Team")]
    public Guid TeamId { get; set; }

    [Column("status")]
    public string Status { get; set; } = default!;

    [Column("task_id")]
    [ForeignKey("Task")]
    public Guid TaskId { get; set; }

    // Navigation properties
    public virtual Task Task { get; set; } = default!;
    public virtual Team Team { get; set; } = default!;
}