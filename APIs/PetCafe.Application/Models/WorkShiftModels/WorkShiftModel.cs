namespace PetCafe.Application.Models.WorkShiftModels;

public class WorkShiftCreateModel
{
    public string Name { get; set; } = default!;
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string? Description { get; set; }
}

public class WorkShiftUpdateModel : WorkShiftCreateModel
{
    public bool IsActive { get; set; } = true;

}