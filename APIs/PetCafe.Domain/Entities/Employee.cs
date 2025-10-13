using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PetCafe.Domain.Constants;

namespace PetCafe.Domain.Entities;

[Table("employees")]
public class Employee : BaseEntity
{
    [Column("account_id")]
    [ForeignKey("Account")]
    public Guid AccountId { get; set; }

    [Column("full_name")]
    [Required]
    [MaxLength(100)]
    public string FullName { get; set; } = default!;

    [Column("avatar_url")]
    [Required]
    public string AvatarUrl { get; set; } = default!;

    [Column("email")]
    [Required]
    [MaxLength(100)]
    public string Email { get; set; } = default!;

    [Column("phone")]
    [MaxLength(15)]
    public string? Phone { get; set; }

    [Column("address")]
    [MaxLength(200)]
    public string? Address { get; set; }

    [Column("skills")]
    public List<string> Skills { get; set; } = [];

    [Column("salary")]
    public double? Salary { get; set; } = 0;

    [Column("area_id")]
    public Guid? AreaId { get; set; }

    [Column("sub_role")]
    public string SubRole { get; set; } = SubRoleConstants.WORKING_STAFF;

    // Navigation properties

    [ForeignKey("AreaId")]
    public virtual Area? Area { get; set; } = default!;
    public virtual Account Account { get; set; } = default!;
    public virtual ICollection<TeamMember> TeamMembers { get; set; } = [];
    public virtual ICollection<TaskAssignment> TaskAssignments { get; set; } = [];
    public virtual ICollection<Order> Orders { get; set; } = [];
    public virtual ICollection<EmployeeSchedule> Schedules { get; set; } = [];
}