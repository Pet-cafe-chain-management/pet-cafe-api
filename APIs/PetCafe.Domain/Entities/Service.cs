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

    [Column("service_type")]
    [Required]
    [MaxLength(30)]
    public string ServiceType { get; set; } = default!;

    [Column("requires_area")]
    public bool RequiresArea { get; set; } = true;

    [Column("image_url")]
    public string? ImageUrl { get; set; }
    [Column("thumbnails")]
    public List<string> Thumbnails { get; set; } = [];


    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public virtual ICollection<Slot> Slots { get; set; } = [];
    public virtual ICollection<Task> Tasks { get; set; } = [];
    public virtual ICollection<ServiceOrderDetail> OrderDetails { get; set; } = [];
    public virtual ICollection<CustomerBooking> Bookings { get; set; } = [];
    public virtual ICollection<AreaService> AreaServices { get; set; } = [];
}

