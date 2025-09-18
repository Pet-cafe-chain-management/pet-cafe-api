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

    [Column("team_id")]
    [ForeignKey("Team")]
    public Guid TeamId { get; set; }

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

    // Navigation properties
    public virtual Team Team { get; set; } = default!;
    public virtual ICollection<TaskAssignment> TaskAssignments { get; set; } = [];
}