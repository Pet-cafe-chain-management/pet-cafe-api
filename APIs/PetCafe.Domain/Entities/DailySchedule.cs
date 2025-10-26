using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PetCafe.Domain.Constants;

namespace PetCafe.Domain.Entities;

[Table("daily_schedules")]
public class DailySchedule : BaseEntity
{
    [Column("team_member_id")]
    [ForeignKey("TeamMember")]
    public Guid TeamMemberId { get; set; }

    [Column("employee_id")]
    [ForeignKey("Employee")]
    public Guid EmployeeId { get; set; }

    [Column("work_shift_id")]
    [ForeignKey("WorkShift")]
    public Guid WorkShiftId { get; set; }


    [Column("date")]
    public DateTime Date { get; set; }

    [Column("status")]
    public string Status { get; set; } = DailyScheduleStatusConstant.PENDING;

    [Column("notes")]
    [MaxLength(500)]
    public string? Notes { get; set; }

    // Navigation properties
    public virtual TeamMember TeamMember { get; set; } = default!;
    public virtual Employee Employee { get; set; } = default!;
    public virtual WorkShift WorkShift { get; set; } = default!;

}