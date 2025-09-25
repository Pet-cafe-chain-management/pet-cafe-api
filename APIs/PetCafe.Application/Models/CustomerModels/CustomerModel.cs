namespace PetCafe.Application.Models.CustomerModels;

public class CustomerCreateModel
{
    public required string FullName { get; set; } = default!;
    public required string Phone { get; set; }
    public string? Address { get; set; }
    public string? AvatarUrl { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public required string Password { get; set; }
}

public class CustomerUpdateModel : CustomerCreateModel
{
}