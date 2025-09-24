using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetCafe.Domain.Entities;

[Table("pet_breeds")]
public class PetBreed : BaseEntity
{
    [Column("name")]
    [MaxLength(50)]
    public string Name { get; set; } = null!;

    [Column("species_id")]
    [ForeignKey("Species")]
    public Guid SpeciesId { get; set; }

    [Column("description")]
    [MaxLength(500)]
    public string? Description { get; set; }

    [Column("average_weight")]
    public double AverageWeight { get; set; }

    [Column("average_lifespan")]
    public int? AverageLifespan { get; set; }

    // Navigation properties
    public virtual PetSpecies Species { get; set; } = null!;
    public virtual ICollection<Pet> Pets { get; set; } = [];
}
