using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetCafe.Domain.Entities;

[Table("pet_breeds")]
public class PetBreed : BaseEntity
{
    [Column("name")]
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = default!;

    [Column("description")]
    [MaxLength(500)]
    public string? Description { get; set; }

    [Column("species")]
    [Required]
    [MaxLength(30)]
    public string Species { get; set; } = default!; // Cat, Dog, Rabbit, etc.

    // Navigation properties
    public virtual ICollection<Pet> Pets { get; set; } = new List<Pet>();
}