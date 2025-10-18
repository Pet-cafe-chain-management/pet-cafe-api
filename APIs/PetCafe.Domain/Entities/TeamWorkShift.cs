using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetCafe.Domain.Entities;

[Table("team_work_shifts")]
public class TeamWorkShift : BaseEntity
{
    [Column("team_id")]
    public Guid TeamId { get; set; }

    [Column("work_shift_id")]
    public Guid WorkShiftId { get; set; }

    [Column("description")]
    public string? Description { get; set; }

    [ForeignKey("TeamId")]
    public virtual Team Team { get; set; } = default!;

    [ForeignKey("WorkShiftId")]
    public virtual WorkShift WorkShift { get; set; } = default!;
}