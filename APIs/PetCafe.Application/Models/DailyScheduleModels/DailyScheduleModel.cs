using PetCafe.Application.Models.ShareModels;

namespace PetCafe.Application.Models.DailyScheduleModels;

public class DailyScheduleFilterQuery : FilterQuery
{
    public Guid? TeamId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string? Status { get; set; }
}