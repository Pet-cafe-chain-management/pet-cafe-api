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

    [Column("work_date")]
    public DateTime WorkDate { get; set; }

    [Column("status")]
    [MaxLength(20)]
    public string Status { get; set; } = "Scheduled"; // Scheduled, Present, Absent, Late

    [Column("check_in_time")]
    public DateTime? CheckInTime { get; set; }

    [Column("check_out_time")]
    public DateTime? CheckOutTime { get; set; }

    [Column("notes")]
    [MaxLength(500)]
    public string? Notes { get; set; }

    // Navigation properties
    public virtual Employee Employee { get; set; } = default!;
    public virtual WorkShift WorkShift { get; set; } = default!;
}