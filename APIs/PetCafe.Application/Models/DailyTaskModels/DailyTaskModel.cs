using PetCafe.Domain.Constants;
using PetCafe.Application.Models.ShareModels;

namespace PetCafe.Application.Models.DailyTaskModels;

public class DailyTaskCreateModel
{
    public Guid TeamId { get; set; }
    public Guid? TaskId { get; set; }
    public Guid? SlotId { get; set; }
    public string Status { get; set; } = DailyTaskStatusConstant.SCHEDULED;
    public DateTime AssignedDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan? EndTime { get; set; }
    public string Title { get; set; } = default!;
    public string Priority { get; set; } = TaskPriorityConstant.MEDIUM;
    public string? Description { get; set; }
    public string? Notes { get; set; }


}

public class DailyTaskUpdateModel : DailyTaskCreateModel
{
}

public class DailyTaskFilterQuery : FilterQuery
{
    public Guid? TeamId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string? Status { get; set; }
}