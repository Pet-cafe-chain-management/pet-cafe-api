using PetCafe.Application.Models.ShareModels;

namespace PetCafe.Application.Models.AreaModels;

public class AreaCreateModel
{
    public required string Name { get; set; } = default!;
    public string? Description { get; set; }
    public string? Location { get; set; }
    public int MaxCapacity { get; set; } = 0;
    public string? ImageUrl { get; set; }
}

public class AreaUpdateModel : AreaCreateModel
{
    public bool IsActive { get; set; }
}

public class AreaFilterQuery : FilterQuery
{
    public bool IsActive { get; set; } = true;

}