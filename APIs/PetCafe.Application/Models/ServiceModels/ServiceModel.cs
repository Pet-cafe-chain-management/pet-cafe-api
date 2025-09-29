namespace PetCafe.Application.Models.ServiceModels;

public class ServiceCreateModel
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }

    public int DurationMinutes { get; set; }
    public double BasePrice { get; set; }
    public int MaxParticipants { get; set; } = 1;

    public string ServiceType { get; set; } = default!;
    public bool RequiresArea { get; set; } = true;
    public string? ImageUrl { get; set; }
    public List<string> Thumbnails { get; set; } = [];

}


public class ServiceUpdateModel : ServiceCreateModel
{
}