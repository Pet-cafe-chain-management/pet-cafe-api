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

    [Column("leader_id")]
    [ForeignKey("Leader")]
    public Guid LeaderId { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public virtual Employee Leader { get; set; } = default!;
    public virtual ICollection<TeamMember> TeamMembers { get; set; } = [];
    public virtual ICollection<CustomerBooking> Bookings { get; set; } = [];
    public virtual ICollection<Slot> Slots { get; set; } = [];
    public virtual ICollection<DailyTask> DailyTasks { get; set; } = [];
    public virtual ICollection<TeamWorkShift> TeamWorkShifts { get; set; } = [];
    public virtual ICollection<TeamWorkType> TeamWorkTypes { get; set; } = [];

}
