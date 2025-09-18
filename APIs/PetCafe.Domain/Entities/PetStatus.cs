using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetCafe.Domain.Entities;

[Table("pet_status")]
public class PetStatus : BaseEntity
{
    [Column("name")]
    [Required]
    [MaxLength(30)]
    public string Name { get; set; } = default!; // Available, Resting, UnderCare, Sick

    [Column("description")]
    [MaxLength(200)]
    public string? Description { get; set; }

    [Column("color_code")]
    [MaxLength(7)]
    public string? ColorCode { get; set; } // For UI display

    // Navigation properties
    public virtual ICollection<Pet> Pets { get; set; } = new List<Pet>();
}