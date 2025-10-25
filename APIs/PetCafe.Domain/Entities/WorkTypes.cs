using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetCafe.Domain.Entities;

[Table("work_types")]
public class WorkType : BaseEntity
{
    [Column("name")]
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = default!;

    [Column("description")]
    [MaxLength(500)]
    public string? Description { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public virtual ICollection<Task> Tasks { get; set; } = [];
    public virtual ICollection<AreaWorkType> AreaWorkTypes { get; set; } = [];
    public virtual ICollection<TeamWorkType> TeamWorkTypes { get; set; } = [];
}