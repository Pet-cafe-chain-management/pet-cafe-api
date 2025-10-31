using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PetCafe.Domain.Constants;

namespace PetCafe.Domain.Entities;

[Table("slots")]
public class Slot : BaseEntity
{

    [Column("service_id")]
    public Guid? ServiceId { get; set; }

    [Column("task_id")]
    public Guid TaskId { get; set; }

    [Column("area_id")]
    public Guid AreaId { get; set; }

    [Column("team_id")]
    public Guid TeamId { get; set; }

    [Column("pet_group_id")]
    public Guid? PetGroupId { get; set; }

    [Column("pet_id")]
    public Guid? PetId { get; set; }

    [Column("start_time")]
    public TimeSpan StartTime { get; set; }

    [Column("end_time")]
    public TimeSpan EndTime { get; set; }

    [Column("max_capacity")]
    public int MaxCapacity { get; set; }

    [Column("price")]
    public double Price { get; set; }

    [Column("is_recurring")]
    public bool IsRecurring { get; set; } = false;

    [Column("specific_date")]
    public DateTime? SpecificDate { get; set; }

    [Column("day_of_week")]
    public string? DayOfWeek { get; set; }

    [Column("is_active")]
    public string ServiceStatus { get; set; } = SlotStatusConstant.UNAVAILABLE;

    [Column("special_notes")]
    [MaxLength(500)]
    public string? SpecialNotes { get; set; }

    [ForeignKey("PetGroupId")]
    public virtual PetGroup? PetGroup { get; set; } = null!;

    [ForeignKey("ServiceId")]
    public virtual Service? Service { get; set; } = null!;

    [ForeignKey("PetId")]
    public virtual Pet? Pet { get; set; } = null!;

    [ForeignKey("AreaId")]
    public virtual Area Area { get; set; } = null!;

    [ForeignKey("TeamId")]
    public virtual Team Team { get; set; } = null!;

    [ForeignKey("TaskId")]
    public virtual Task Task { get; set; } = null!;
    public virtual ICollection<CustomerBooking> CustomerBookings { get; set; } = [];
    public virtual ICollection<ServiceOrderDetail> OrderDetails { get; set; } = [];
    public virtual ICollection<DailyTask> DailyTasks { get; set; } = [];
}
