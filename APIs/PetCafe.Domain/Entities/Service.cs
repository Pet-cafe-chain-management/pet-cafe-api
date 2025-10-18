using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetCafe.Domain.Entities;

[Table("services")]
public class Service : BaseEntity
{
    [Column("name")]
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = default!;

    [Column("description")]
    [MaxLength(500)]
    public string? Description { get; set; }

    [Column("duration_minutes")]
    public int DurationMinutes { get; set; }

    [Column("base_price")]
    public double BasePrice { get; set; }

    [Column("image_url")]
    public string? ImageUrl { get; set; }
    [Column("thumbnails")]
    public List<string> Thumbnails { get; set; } = [];

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("work_type_id")]
    [ForeignKey("WorkType")]
    public Guid WorkTypeId { get; set; }

    // Navigation properties
    public virtual WorkType WorkType { get; set; } = default!;

    public virtual ICollection<Slot> Slots { get; set; } = [];
    public virtual ICollection<ServiceOrderDetail> OrderDetails { get; set; } = [];
    public virtual ICollection<CustomerBooking> Bookings { get; set; } = [];
    public virtual ICollection<ServicePetGroup> ServicePetGroups { get; set; } = [];
}

