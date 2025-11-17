using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PetCafe.Domain.Constants;

namespace PetCafe.Domain.Entities;

[Table("leave_requests")]
public class LeaveRequest : BaseEntity
{
    [Column("employee_id")]
    [ForeignKey("Employee")]
    public Guid EmployeeId { get; set; }

    [Column("replacement_employee_id")]
    [ForeignKey("ReplacementEmployee")]
    public Guid? ReplacementEmployeeId { get; set; }

    [Column("leave_date")]
    public DateTime LeaveDate { get; set; }

    [Column("reason")]
    [Required]
    [MaxLength(500)]
    public string Reason { get; set; } = default!;

    [Column("leave_type")]
    [MaxLength(20)]
    public string LeaveType { get; set; } = LeaveRequestTypeConstant.ADVANCE;

    [Column("status")]
    [MaxLength(20)]
    public string Status { get; set; } = LeaveRequestStatusConstant.PENDING;

    [Column("reviewed_by")]
    [ForeignKey("Reviewer")]
    public Guid? ReviewedBy { get; set; }

    [Column("reviewed_at")]
    public DateTime? ReviewedAt { get; set; }

    [Column("review_notes")]
    [MaxLength(500)]
    public string? ReviewNotes { get; set; }

    // Navigation properties
    public virtual Employee Employee { get; set; } = default!;
    public virtual Employee? ReplacementEmployee { get; set; }
    public virtual Employee? Reviewer { get; set; }
}

