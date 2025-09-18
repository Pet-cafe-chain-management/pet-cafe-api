using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetCafe.Domain.Entities;

[Table("staff_assignments")]
public class StaffAssignment : BaseEntity
{
    [Column("order_id")]
    [ForeignKey("Order")]
    public Guid OrderId { get; set; }

    [Column("employee_id")]
    [ForeignKey("Employee")]
    public Guid EmployeeId { get; set; }

    [Column("assignment_type")]
    [Required]
    [MaxLength(20)]
    public string AssignmentType { get; set; } = default!;
    [Column("assigned_date")]
    public DateTime AssignedDate { get; set; }

    [Column("completion_date")]
    public DateTime? CompletionDate { get; set; }

    [Column("status")]
    [MaxLength(20)]
    public string Status { get; set; } = "Assigned";
    [Column("notes")]
    [MaxLength(500)]
    public string? Notes { get; set; }

    // Navigation properties
    public virtual Order Order { get; set; } = default!;
    public virtual Employee Employee { get; set; } = default!;
}