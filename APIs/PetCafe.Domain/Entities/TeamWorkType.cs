using System.ComponentModel.DataAnnotations.Schema;

namespace PetCafe.Domain.Entities;

[Table("team_work_types")]
public class TeamWorkType : BaseEntity
{
    [Column("team_id")]
    [ForeignKey("Team")]
    public Guid TeamId { get; set; }

    [Column("work_type_id")]
    [ForeignKey("WorkType")]
    public Guid WorkTypeId { get; set; }

    [Column("description")]
    public string? Description { get; set; }

    // Navigation properties
    public virtual Team Team { get; set; } = default!;
    public virtual WorkType WorkType { get; set; } = default!;
}