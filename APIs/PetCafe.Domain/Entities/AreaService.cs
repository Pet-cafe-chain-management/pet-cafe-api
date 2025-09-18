using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetCafe.Domain.Entities;

[Table("area_services")]
public class AreaService : BaseEntity
{
    [Column("area_id")]
    [ForeignKey("Area")]
    public Guid AreaId { get; set; }

    [Column("service_id")]
    [ForeignKey("Service")]
    public Guid ServiceId { get; set; }

    [Column("max_capacity")]
    public int MaxCapacity { get; set; } // Số lượng service tối đa có thể phục vụ trong khu vực này

    [Column("current_bookings")]
    public int CurrentBookings { get; set; } = 0; // Số lượng booking hiện tại

    [Column("price_adjustment")]
    public double PriceAdjustment { get; set; } = 0; // Điều chỉnh giá theo khu vực (có thể âm hoặc dương)

    [Column("is_available")]
    public bool IsAvailable { get; set; } = true;

    [Column("special_notes")]
    [MaxLength(500)]
    public string? SpecialNotes { get; set; }

    // Navigation properties
    public virtual Area Area { get; set; } = default!;
    public virtual Service Service { get; set; } = default!;
}
