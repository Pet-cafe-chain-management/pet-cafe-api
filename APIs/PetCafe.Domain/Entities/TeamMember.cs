using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetCafe.Domain.Entities;

[Table("team_members")]
public class TeamMember : BaseEntity
{
    [Column("team_id")]
    [ForeignKey("Team")]
    public Guid TeamId { get; set; }

    [Column("employee_id")]
    [ForeignKey("Employee")]
    public Guid EmployeeId { get; set; }


    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public virtual Team Team { get; set; } = default!;
    public virtual Employee Employee { get; set; } = default!;
}