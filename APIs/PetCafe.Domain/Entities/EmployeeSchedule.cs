using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetCafe.Domain.Entities;

[Table("employee_schedules")]
public class EmployeeSchedule : BaseEntity
{
    [Column("employee_id")]
    [ForeignKey("Employee")]
    public Guid EmployeeId { get; set; }

    [Column("work_shift_id")]
    [ForeignKey("WorkShift")]
    public Guid WorkShiftId { get; set; }

    // Navigation properties
    public virtual Employee Employee { get; set; } = default!;
    public virtual WorkShift WorkShift { get; set; } = default!;
}