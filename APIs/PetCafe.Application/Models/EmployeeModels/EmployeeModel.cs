namespace PetCafe.Application.Models.EmployeeModels;

public class EmployeeCreateModel
{
    public required string FullName { get; set; } = default!;
    public required string Phone { get; set; }
    public string? Address { get; set; }
    public double Salary { get; set; } = 0;
    public List<string> Skills { get; set; } = [];
    public Guid? AreaId { get; set; }
    public required string Email { get; set; } = default!;
    public required string AvatarUrl { get; set; } = default!;
    public required string Password { get; set; }
}

public class EmployeeUpdateModel : EmployeeCreateModel
{
}