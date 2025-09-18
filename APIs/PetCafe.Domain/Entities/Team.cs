using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetCafe.Domain.Entities;

[Table("teams")]
public class Team : BaseEntity
{
    [Column("name")]
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = default!;

    [Column("description")]
    [MaxLength(200)]
    public string? Description { get; set; }

    [Column("team_type")]
    [Required]
    [MaxLength(30)]
    public string TeamType { get; set; } = default!; // Cleaning, Training, Care, Sales

    [Column("leader_id")]
    [ForeignKey("Leader")]
    public Guid? LeaderId { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public virtual Employee? Leader { get; set; }
    public virtual ICollection<TeamMember> TeamMembers { get; set; } = [];
    public virtual ICollection<Task> Tasks { get; set; } = [];
}
