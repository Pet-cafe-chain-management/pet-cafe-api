using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetCafe.Domain.Entities;

[Table("tasks")]
public class Task : BaseEntity
{
    [Column("title")]
    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = default!;

    [Column("description")]
    [MaxLength(500)]
    public string? Description { get; set; }

    [Column("priority")]
    [MaxLength(10)]
    public string Priority { get; set; } = "Medium"; // Low, Medium, High, Urgent

    [Column("status")]
    [MaxLength(20)]
    public string Status { get; set; } = "Pending"; // Pending, InProgress, Completed, Cancelled

    [Column("due_date")]
    public DateTime? DueDate { get; set; }

    [Column("estimated_hours")]
    public int? EstimatedHours { get; set; }

    [Column("actual_hours")]
    public int? ActualHours { get; set; }

    [Column("team_id")]
    [ForeignKey("Team")]
    public Guid TeamId { get; set; }

    [Column("area_id")]
    [ForeignKey("Area")]
    public Guid? AreaId { get; set; }

    [Column("pet_group_id")]
    [ForeignKey("PetGroup")]
    public Guid? PetGroupId { get; set; }

    [Column("pet_id")]
    [ForeignKey("Pet")]
    public Guid? PetId { get; set; }

    [Column("work_shift_id")]
    [ForeignKey("WorkShift")]
    public Guid? WorkShiftId { get; set; }

    // Navigation properties
    public virtual Team Team { get; set; } = default!;
    public virtual Area? Area { get; set; } = default!;
    public virtual PetGroup? PetGroup { get; set; } = default!;
    public virtual Pet? Pet { get; set; } = default!;
    public virtual WorkShift WorkShift { get; set; } = default!;
    public virtual ICollection<TaskAssignment> TaskAssignments { get; set; } = [];
    public virtual ICollection<DailyTask> DailyTasks { get; set; } = [];
}