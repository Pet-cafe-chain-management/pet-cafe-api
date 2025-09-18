using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetCafe.Domain.Entities;

[Table("employees")]
public class Employee : BaseEntity
{
    [Column("account_id")]
    [ForeignKey("Account")]
    public Guid AccountId { get; set; }

    [Column("employee_code")]
    [Required]
    [MaxLength(20)]
    public string EmployeeCode { get; set; } = default!;

    [Column("full_name")]
    [Required]
    [MaxLength(100)]
    public string FullName { get; set; } = default!;

    [Column("phone")]
    [MaxLength(15)]
    public string? Phone { get; set; }

    [Column("address")]
    [MaxLength(200)]
    public string? Address { get; set; }

    [Column("hire_date")]
    public DateTime HireDate { get; set; }

    [Column("salary")]
    public double? Salary { get; set; } = 0;

    [Column("position")]
    [MaxLength(50)]
    public string? Position { get; set; }

    // Navigation properties
    public virtual Account Account { get; set; } = default!;
    public virtual ICollection<TeamMember> TeamMembers { get; set; } = [];
    public virtual ICollection<TaskAssignment> TaskAssignments { get; set; } = [];
    public virtual ICollection<EmployeeSchedule> EmployeeSchedules { get; set; } = [];
    public virtual ICollection<StaffAssignment> StaffAssignments { get; set; } = [];
}