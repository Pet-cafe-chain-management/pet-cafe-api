using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetCafe.Domain.Entities;

[Table("work_shifts")]
public class WorkShift : BaseEntity
{
    [Column("name")]
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = default!;

    [Column("start_time")]
    public TimeSpan StartTime { get; set; }

    [Column("end_time")]
    public TimeSpan EndTime { get; set; }

    [Column("description")]
    [MaxLength(200)]
    public string? Description { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    public virtual ICollection<EmployeeSchedule> Schedules { get; set; } = [];
    public virtual ICollection<Task> Tasks { get; set; } = [];
}