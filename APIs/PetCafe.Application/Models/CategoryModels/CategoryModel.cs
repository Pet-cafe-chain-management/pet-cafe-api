namespace PetCafe.Application.Models.CategoryModels;

public class CategoryCreateModel
{
    public required string Name { get; set; } = default!;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
}

public class CategoryUpdateModel : CategoryCreateModel
{
    public bool IsActive { get; set; }
}