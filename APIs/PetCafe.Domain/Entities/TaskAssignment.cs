using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetCafe.Domain.Entities;

[Table("task_assignments")]
public class TaskAssignment : BaseEntity
{
    [Column("task_id")]
    [ForeignKey("Task")]
    public Guid TaskId { get; set; }

    [Column("employee_id")]
    [ForeignKey("Employee")]
    public Guid EmployeeId { get; set; }

    [Column("assigned_date")]
    public DateTime AssignedDate { get; set; }

    [Column("start_date")]
    public DateTime? StartDate { get; set; }

    [Column("completion_date")]
    public DateTime? CompletionDate { get; set; }

    [Column("status")]
    [MaxLength(20)]
    public string Status { get; set; } = "Assigned"; // Assigned, InProgress, Completed

    [Column("notes")]
    [MaxLength(500)]
    public string? Notes { get; set; }

    // Navigation properties
    public virtual Task Task { get; set; } = default!;
    public virtual Employee Employee { get; set; } = default!;
}