using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PetCafe.Domain.Constants;

namespace PetCafe.Domain.Entities;

[Table("slots")]
public class Slot : BaseEntity
{

    [Column("service_id")]
    public Guid ServiceId { get; set; }

    [Column("area_id")]
    public Guid AreaId { get; set; }

    [Column("team_id")]
    public Guid TeamId { get; set; }

    [Column("pet_gourp_id")]
    public Guid PetGroupId { get; set; }

    [Column("applicable_days")]
    public List<string> ApplicableDays { get; set; } = DayConstant.ALLDAYS;

    [Column("start_time")]
    public TimeSpan StartTime { get; set; }

    [Column("end_time")]
    public TimeSpan EndTime { get; set; }

    [Column("max_capacity")]
    public int MaxCapacity { get; set; }

    [Column("available_capacity")]
    public int AvailableCapacity { get; set; }

    [Column("price")]
    public double Price { get; set; }

    [Column("status")]
    [MaxLength(20)]
    public string Status { get; set; } = SlotStatusConstant.AVAILABLE; 

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("special_notes")]
    [MaxLength(500)]
    public string? SpecialNotes { get; set; }

    [ForeignKey("PetGroupId")]
    public virtual PetGroup PetGroup { get; set; } = null!;

    [ForeignKey("TeamId")]
    public virtual Team Team { get; set; } = null!;

    [ForeignKey("ServiceId")]
    public virtual Service Service { get; set; } = null!;

    [ForeignKey("AreaId")]
    public virtual Area Area { get; set; } = null!;

    public virtual ICollection<CustomerBooking> CustomerBookings { get; set; } = [];
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = [];
}
