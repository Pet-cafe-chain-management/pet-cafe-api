namespace PetCafe.Application.Models.VaccinationRecordModels;

public class VaccinationRecordCreateModel
{
    public Guid PetId { get; set; }
    public Guid VaccineTypeId { get; set; }
    public DateTime VaccinationDate { get; set; }
    public DateTime? NextDueDate { get; set; }
    public string? Veterinarian { get; set; }
    public string? ClinicName { get; set; }
    public string? BatchNumber { get; set; }
    public string? Notes { get; set; }
    public Guid ScheduleId { get; set; }
}


public class VaccinationRecordUpdateModel : VaccinationRecordCreateModel
{
}