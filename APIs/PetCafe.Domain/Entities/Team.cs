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
    public string TeamType { get; set; } = default!;

    [Column("leader_id")]
    [ForeignKey("Leader")]
    public Guid LeaderId { get; set; }

    [Column("area_id")]
    [ForeignKey("Area")]
    public Guid AreaId { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("work_type_id")]
    [ForeignKey("WorkType")]
    public Guid WorkTypeId { get; set; }

    // Navigation properties
    public virtual WorkType WorkType { get; set; } = default!;
    public virtual Employee Leader { get; set; } = default!;
    public virtual Area Area { get; set; } = default!;
    public virtual ICollection<TeamMember> TeamMembers { get; set; } = [];
    public virtual ICollection<CustomerBooking> Bookings { get; set; } = [];
    public virtual ICollection<Task> Tasks { get; set; } = [];
    public virtual ICollection<DailyTask> DailyTasks { get; set; } = [];
    public virtual ICollection<TeamWorkShift> TeamWorkShifts { get; set; } = [];

}
