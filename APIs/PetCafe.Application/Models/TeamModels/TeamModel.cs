namespace PetCafe.Application.Models.TeamModels;

public class TeamCreateModel
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public string TeamType { get; set; } = default!; // Cleaning, Training, Care, Sales
    public Guid? LeaderId { get; set; }
}

public class TeamUpdateModel : TeamCreateModel
{
    public bool IsActive { get; set; } = true;
}



public class MemberCreateModel
{
    public Guid EmployeeId { get; set; }
}

public class MemberUpdateModel : MemberCreateModel
{
    public bool IsActive { get; set; } = true;
}

