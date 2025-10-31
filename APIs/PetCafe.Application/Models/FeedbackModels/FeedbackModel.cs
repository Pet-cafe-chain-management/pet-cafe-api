using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using PetCafe.Application.Models.ShareModels;

namespace PetCafe.Application.Models.FeedbackModels;

public class FeedbackCreateModel
{
    public Guid CustomerBookingId { get; set; }
    public Guid ServiceId { get; set; }
    public int Rating { get; set; } // 1-5 stars
    public string? Comment { get; set; }
}

public class FeedbackUpdateModel : FeedbackCreateModel
{
}

public class FeedbackFilterQuery : FilterQuery
{
    [FromQuery(Name = "service_id")]
    public Guid? ServiceId { get; set; }

    [FromQuery(Name = "customer_id")]
    public Guid? CustomerId { get; set; }

    [FromQuery(Name = "min_rating")]
    public int? MinRating { get; set; }

    [FromQuery(Name = "max_rating")]
    public int? MaxRating { get; set; }
}

public class FeedbackCreateModelValidator : AbstractValidator<FeedbackCreateModel>
{
    public FeedbackCreateModelValidator()
    {
        RuleFor(x => x.CustomerBookingId)
            .NotEmpty().WithMessage("ID booking không được để trống");

        RuleFor(x => x.ServiceId)
            .NotEmpty().WithMessage("ID dịch vụ không được để trống");

        RuleFor(x => x.Rating)
            .GreaterThanOrEqualTo(1).WithMessage("Đánh giá phải từ 1 sao trở lên")
            .LessThanOrEqualTo(5).WithMessage("Đánh giá không được vượt quá 5 sao");

        RuleFor(x => x.Comment)
            .MaximumLength(1000).WithMessage("Bình luận không được vượt quá 1000 ký tự")
            .When(x => !string.IsNullOrEmpty(x.Comment));
    }
}

public class FeedbackUpdateModelValidator : AbstractValidator<FeedbackUpdateModel>
{
    public FeedbackUpdateModelValidator()
    {
        Include(new FeedbackCreateModelValidator());
    }
}

