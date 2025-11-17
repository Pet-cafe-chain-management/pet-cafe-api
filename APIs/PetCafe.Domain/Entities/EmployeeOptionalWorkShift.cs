using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetCafe.Domain.Entities;

[Table("employee_optional_work_shifts")]
public class EmployeeOptionalWorkShift : BaseEntity
{
    [Column("employee_id")]
    [ForeignKey("Employee")]
    public Guid EmployeeId { get; set; }

    [Column("work_shift_id")]
    [ForeignKey("WorkShift")]
    public Guid WorkShiftId { get; set; }

    [Column("priority")]
    public int Priority { get; set; } = 1; // Default priority, higher = more preferred

    [Column("is_available")]
    public bool IsAvailable { get; set; } = true;

    // Navigation properties
    public virtual Employee Employee { get; set; } = default!;
    public virtual WorkShift WorkShift { get; set; } = default!;
}

