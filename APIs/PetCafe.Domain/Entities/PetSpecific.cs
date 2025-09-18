using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetCafe.Domain.Entities;

[Table("pet_specifics")]
public class PetSpecific : BaseEntity
{
    [Column("pet_id")]
    [ForeignKey("Pet")]
    public Guid PetId { get; set; }

    [Column("attribute_name")]
    [Required]
    [MaxLength(50)]
    public string AttributeName { get; set; } = default!; // Favorite_Food, Preferred_Activity, etc.

    [Column("attribute_value")]
    [Required]
    [MaxLength(200)]
    public string AttributeValue { get; set; } = default!;

    [Column("notes")]
    [MaxLength(500)]
    public string? Notes { get; set; }

    // Navigation properties
    public virtual Pet Pet { get; set; } = default!;
}