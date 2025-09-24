using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetCafe.Domain.Entities;

[Table("pet_groups")]
public class PetGroup : BaseEntity
{

    [Column("name")]
    [MaxLength(50)]
    public string Name { get; set; } = null!;

    [Column("description")]
    [MaxLength(255)]
    public string? Description { get; set; }

    [Column("max_capacity")]
    public int MaxCapacity { get; set; }

    [Column("pet_species_id")]
    public Guid? PetSpeciesId { get; set; }

    [ForeignKey("PetSpeciesId")]
    public virtual PetSpecies? PetSpecies { get; set; }
    [Column("pet_breed_id")]
    public Guid? PetBreedId { get; set; }

    [ForeignKey("PetBreedId")]
    public virtual PetBreed? PetBreed { get; set; }
    public virtual ICollection<Pet> Pets { get; set; } = [];
    public virtual ICollection<Slot> Slots { get; set; } = [];
    public virtual ICollection<Task> Tasks { get; set; } = [];
}