using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetCafe.Domain.Entities;

[Table("areas")]
public class Area : BaseEntity
{
    [Column("name")]
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = default!;

    [Column("description")]
    [MaxLength(500)]
    public string? Description { get; set; }

    [Column("location")]
    [MaxLength(200)]
    public string? Location { get; set; }

    [Column("max_capacity")]
    public int MaxCapacity { get; set; } = 0;

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("image_url")]
    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    [Column("work_type_id")]
    [ForeignKey("WorkType")]
    public Guid WorkTypeId { get; set; }
    // Navigation properties
    public virtual WorkType WorkType { get; set; } = default!;
    public virtual ICollection<Slot> Slots { get; set; } = [];
    public virtual ICollection<Task> Tasks { get; set; } = [];
    public virtual ICollection<Team> Teams { get; set; } = [];
}