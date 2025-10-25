using System.ComponentModel.DataAnnotations.Schema;

namespace PetCafe.Domain.Entities;

[Table("area_work_types")]
public class AreaWorkType : BaseEntity
{
    [Column("area_id")]
    [ForeignKey("Area")]
    public Guid AreaId { get; set; }

    [Column("work_type_id")]
    [ForeignKey("WorkType")]
    public Guid WorkTypeId { get; set; }

    [Column("description")]
    public string? Description { get; set; }

    // Navigation properties
    public virtual Area Area { get; set; } = default!;
    public virtual WorkType WorkType { get; set; } = default!;
}