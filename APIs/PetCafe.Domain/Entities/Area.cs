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
    public string? Location { get; set; } // Vị trí trong cửa hàng

    [Column("area_code")]
    [Required]
    [MaxLength(10)]
    public string AreaCode { get; set; } = default!; // Mã khu vực: A, B, C, etc.

    [Column("max_capacity")]
    public int MaxCapacity { get; set; } = 0; // Sức chứa tối đa của khu vực

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("operating_hours_start")]
    public TimeSpan? OperatingHoursStart { get; set; }

    [Column("operating_hours_end")]
    public TimeSpan? OperatingHoursEnd { get; set; }

    [Column("image_url")]
    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    // Navigation properties
    public virtual ICollection<AreaService> AreaServices { get; set; } = new List<AreaService>();
    public virtual ICollection<ServiceBooking> ServiceBookings { get; set; } = new List<ServiceBooking>();
}