using PetCafe.Application.Models.ShareModels;

namespace PetCafe.Application.Models.ServiceModels;

public class ServiceCreateModel
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }

    public int DurationMinutes { get; set; }
    public double BasePrice { get; set; }
    public string ServiceType { get; set; } = default!;
    public bool RequiresArea { get; set; } = true;
    public string? ImageUrl { get; set; }
    public List<string> Thumbnails { get; set; } = [];

}


public class ServiceUpdateModel : ServiceCreateModel
{
}

public class ServiceFilterQuery : FilterQuery
{
    public DateTime? SearchDate { get; set; }
    public TimeSpan? StartTime { get; set; }
    public TimeSpan? EndTime { get; set; }
    public List<Guid>? PetSpeciesIds { get; set; } = [];
    public List<Guid>? PetBreedIds { get; set; } = [];
    public List<Guid>? AreaIds { get; set; } = [];
    public List<string>? ServiceTypes { get; set; } = [];
    public double? MaxPrice { get; set; }
    public double? MinPrice { get; set; }
    public bool IsActive { get; set; } = true;

}