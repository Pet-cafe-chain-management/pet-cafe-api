using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PetCafe.Domain.Constants;

namespace PetCafe.Domain.Entities;

[Table("health_records")]
public class HealthRecord : BaseEntity
{
    [Column("pet_id")]
    [ForeignKey("Pet")]
    public Guid PetId { get; set; }

    [Column("check_date")]
    public DateTime CheckDate { get; set; }

    [Column("weight")]
    public double? Weight { get; set; } = 0;

    [Column("temperature")]
    public double? Temperature { get; set; }

    [Column("health_status")]
    [Required]
    [MaxLength(20)]
    public string HealthStatus { get; set; } = HealthStatusConstant.HEALTHY;

    [Column("symptoms")]
    [MaxLength(500)]
    public string? Symptoms { get; set; }

    [Column("treatment")]
    [MaxLength(1000)]
    public string? Treatment { get; set; }

    [Column("veterinarian")]
    [MaxLength(100)]
    public string? Veterinarian { get; set; }

    [Column("next_check_date")]
    public DateTime? NextCheckDate { get; set; }

    [Column("notes")]
    [MaxLength(1000)]
    public string? Notes { get; set; }

    // Navigation properties
    public virtual Pet Pet { get; set; } = default!;
}
