using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetCafe.Domain.Entities;

[Table("pet_species")]
public class PetSpecies : BaseEntity
{
    [Column("name")]
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = null!; // "Dog", "Cat", "Bird", "Rabbit", etc.

    [Column("description")]
    [MaxLength(500)]
    public string? Description { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public virtual ICollection<PetBreed> PetBreeds { get; set; } = [];
    public virtual ICollection<Pet> Pets { get; set; } = [];
}
