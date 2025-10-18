using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetCafe.Domain.Entities;

[Table("service_pet_groups")]
public class ServicePetGroup : BaseEntity
{
    [Column("service_id")]
    public Guid ServiceId { get; set; }

    [Column("pet_group_id")]
    public Guid PetGroupId { get; set; }



    [ForeignKey("ServiceId")]
    public virtual Service Service { get; set; } = default!;

    [ForeignKey("PetGroupId")]
    public virtual PetGroup PetGroup { get; set; } = default!;
}