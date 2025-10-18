namespace PetCafe.Application.Models.WorkTypeModels;

public class WorkTypeCreateModel
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }

}

public class WorkTypeUpdateModel : WorkTypeCreateModel
{
    public bool IsActive { get; set; } = true;

}