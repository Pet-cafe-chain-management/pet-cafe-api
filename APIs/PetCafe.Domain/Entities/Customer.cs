using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetCafe.Domain.Entities;

[Table("customers")]
public class Customer : BaseEntity
{
    [Column("account_id")]
    [ForeignKey("Account")]
    public Guid AccountId { get; set; }

    [Column("full_name")]
    [Required]
    [MaxLength(100)]
    public string FullName { get; set; } = default!;

    [Column("phone")]
    [MaxLength(15)]
    public string? Phone { get; set; }

    [Column("address")]
    [MaxLength(200)]
    public string? Address { get; set; }

    [Column("date_of_birth")]
    public DateTime? DateOfBirth { get; set; }

    [Column("loyalty_points")]
    public int LoyaltyPoints { get; set; } = 0;

    // Navigation properties
    public virtual Account Account { get; set; } = default!;
    public virtual ICollection<Order> Orders { get; set; } = [];
    public virtual ICollection<ServiceBooking> ServiceBookings { get; set; } = [];
}