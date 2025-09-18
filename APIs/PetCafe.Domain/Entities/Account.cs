using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetCafe.Domain.Entities;

[Table("accounts")]
public class Account : BaseEntity
{
    [Column("username")]
    [Required]
    [MaxLength(50)]
    public string Username { get; set; } = default!;

    [Column("email")]
    [Required]
    [MaxLength(100)]
    public string Email { get; set; } = default!;

    [Column("password_hash")]
    [Required]
    public string PasswordHash { get; set; } = default!;

    [Column("role")]
    [Required]
    [MaxLength(20)]
    public string Role { get; set; } = default!;

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public virtual Customer? Customer { get; set; }
    public virtual Employee? Employee { get; set; }
    public virtual ICollection<Notification> Notifications { get; set; } = [];
}
