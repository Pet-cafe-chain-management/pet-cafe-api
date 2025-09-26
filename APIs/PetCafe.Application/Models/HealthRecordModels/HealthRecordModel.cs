namespace PetCafe.Application.Models.HealthRecordModels;

public class HealthRecordCreateModel
{
    public Guid PetId { get; set; }
    public DateTime CheckDate { get; set; } = DateTime.UtcNow;
    public double? Weight { get; set; } = 0;
    public double? Temperature { get; set; }
    public string HealthStatus { get; set; } = default!; // Healthy, Sick, Recovering
    public string? Symptoms { get; set; }
    public string? Treatment { get; set; }
    public string? Veterinarian { get; set; }
    public DateTime? NextCheckDate { get; set; }
    public string? Notes { get; set; }

}

public class HealthRecordUpdateModel : HealthRecordCreateModel
{
}