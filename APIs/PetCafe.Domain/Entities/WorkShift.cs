using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PetCafe.Domain.Constants;

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

    [Column("applicable_days")]
    public List<string> ApplicableDays { get; set; } = DayConstant.ALLDAYS;
    public virtual ICollection<EmployeeSchedule> Schedules { get; set; } = [];
    public virtual ICollection<TeamWorkShift> TeamWorkShifts { get; set; } = [];

}