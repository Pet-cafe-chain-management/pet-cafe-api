using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using PetCafe.Application.Models.ShareModels;

namespace PetCafe.Application.Models.ServiceModels;

public class ServiceCreateModel
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public int DurationMinutes { get; set; }
    public double BasePrice { get; set; }
    public string? ImageUrl { get; set; }
    public List<string> Thumbnails { get; set; } = [];
    public Guid TaskId { get; set; }

}


public class ServiceUpdateModel : ServiceCreateModel
{
    public bool IsActive { get; set; } = true;
}


public class ServiceFilterQuery : FilterQuery
{
    [FromQuery(Name = "task_id")]
    public DateTime? SearchDate { get; set; }

    [FromQuery(Name = "start_time")]
    public TimeSpan? StartTime { get; set; }

    [FromQuery(Name = "end_time")]
    public TimeSpan? EndTime { get; set; }

    [FromQuery(Name = "pet_species_ids")]
    public List<Guid>? PetSpeciesIds { get; set; } = [];

    [FromQuery(Name = "pet_breed_ids")]
    public List<Guid>? PetBreedIds { get; set; } = [];

    [FromQuery(Name = "area_ids")]
    public List<Guid>? AreaIds { get; set; } = [];

    [FromQuery(Name = "max_price")]
    public double? MaxPrice { get; set; }

    [FromQuery(Name = "min_price")]
    public double? MinPrice { get; set; }

    [FromQuery(Name = "is_active")]
    public bool IsActive { get; set; } = true;

}

public class ServiceCreateModelValidator : AbstractValidator<ServiceCreateModel>
{
    public ServiceCreateModelValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Tên dịch vụ không được để trống")
            .MaximumLength(100).WithMessage("Tên dịch vụ không được vượt quá 100 ký tự");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Mô tả không được vượt quá 1000 ký tự")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.DurationMinutes)
            .GreaterThan(0).WithMessage("Thời gian thực hiện phải lớn hơn 0 phút");

        RuleFor(x => x.BasePrice)
            .GreaterThanOrEqualTo(0).WithMessage("Giá cơ bản phải lớn hơn hoặc bằng 0");


        RuleFor(x => x.ImageUrl)
            .MaximumLength(1000).WithMessage("Đường dẫn ảnh không được vượt quá 1000 ký tự")
            .When(x => !string.IsNullOrEmpty(x.ImageUrl));

        RuleForEach(x => x.Thumbnails)
            .NotEmpty().WithMessage("Đường dẫn ảnh thumbnail không được để trống")
            .MaximumLength(1000).WithMessage("Đường dẫn ảnh thumbnail không được vượt quá 1000 ký tự");
    }
}

public class ServiceUpdateModelValidator : AbstractValidator<ServiceUpdateModel>
{
    public ServiceUpdateModelValidator()
    {
        Include(new ServiceCreateModelValidator());
    }
}