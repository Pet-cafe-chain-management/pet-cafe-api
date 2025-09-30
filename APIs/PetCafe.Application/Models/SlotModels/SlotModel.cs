using PetCafe.Domain.Constants;

namespace PetCafe.Application.Models.SlotModels;

public class SlotCreateModel
{
    public Guid ServiceId { get; set; }
    public Guid AreaId { get; set; }
    public Guid TeamId { get; set; }
    public Guid PetGroupId { get; set; }
    public List<string> ApplicableDays { get; set; } = DayConstant.ALLDAYS;
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public int MaxCapacity { get; set; }
    public double Price { get; set; }
    public string? SpecialNotes { get; set; }

}

public class SlotUpdateModel:SlotCreateModel
{
    public bool IsActive { get; set; } = true;
    public string? Status { get; set; }
}