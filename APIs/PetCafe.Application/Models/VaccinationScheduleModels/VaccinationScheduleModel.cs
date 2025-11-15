using PetCafe.Application.Models.ShareModels;

namespace PetCafe.Application.Models.VaccinationScheduleModels;

public class VaccinationScheduleCreateModel
{
    public Guid PetId { get; set; }
    public Guid VaccineTypeId { get; set; }
    public DateTime ScheduledDate { get; set; }
    public string? Notes { get; set; }
    public Guid TeamId { get; set; }
}

public class VaccinationScheduleUpdateModel : VaccinationScheduleCreateModel
{
    public string Status { get; set; } = "PENDING";
}


public class VaccinationScheduleScheduleFilterQuery : FilterQuery
{
    public Guid? PetId { get; set; }
    public string? VaccineType { get; set; } = string.Empty;
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string? Status { get; set; }
}