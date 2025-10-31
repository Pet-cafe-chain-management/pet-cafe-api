using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PetCafe.Domain.Constants;

namespace PetCafe.Domain.Entities;

[Table("tasks")]
public class Task : BaseEntity
{
    [Column("title")]
    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = default!;

    [Column("image_url")]
    public string? ImageUrl { get; set; }

    [Column("description")]
    public string? Description { get; set; }

    [Column("priority")]
    [MaxLength(10)]
    public string Priority { get; set; } = TaskPriorityConstant.MEDIUM;

    [Column("status")]
    [MaxLength(20)]
    public string Status { get; set; } = TaskStatusConstant.ACTIVE;

    [Column("is_public", TypeName = "boolean")]
    [DefaultValue(false)]
    public bool IsPublic { get; set; } = false;


    [Column("estimated_hours")]
    public int? EstimatedHours { get; set; }

    [Column("work_type_id")]
    [ForeignKey("WorkType")]
    public Guid WorkTypeId { get; set; }

    [Column("service_id")]
    public Guid? ServiceId { get; set; }

    // Navigation properties
    public virtual WorkType WorkType { get; set; } = default!;
    public virtual Service? Service { get; set; } = default!;

    public virtual ICollection<Slot> Slots { get; set; } = [];
    public virtual ICollection<DailyTask> DailyTasks { get; set; } = [];
}