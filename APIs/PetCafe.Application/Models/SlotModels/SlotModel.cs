using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using PetCafe.Application.Models.ShareModels;
using PetCafe.Domain.Constants;

namespace PetCafe.Application.Models.SlotModels;

public class SlotCreateModel
{
    public Guid TaskId { get; set; }
    public Guid AreaId { get; set; }
    public Guid PetGroupId { get; set; }
    public Guid TeamId { get; set; }
    public Guid? PetId { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public int MaxCapacity { get; set; }
    public string? SpecialNotes { get; set; }
    public bool IsRecurring { get; set; } = false;
    public DateTime? SpecificDate { get; set; }
    public string? DayOfWeek { get; set; }

}


public class SlotUpdateModel : SlotCreateModel
{
    public double Price { get; set; }
    public string ServiceStatus { get; set; } = SlotStatusConstant.AVAILABLE;
    public bool IsUpdateRelatedData { get; set; } = false;
}

public class SlotCreateModelValidator : AbstractValidator<SlotCreateModel>
{
    public SlotCreateModelValidator()
    {
        RuleFor(x => x.AreaId)
            .NotEmpty().WithMessage("ID khu vực không được để trống");

        RuleFor(x => x.PetGroupId)
            .NotEmpty().WithMessage("ID nhóm thú cưng không được để trống");

        RuleFor(x => x.StartTime)
            .NotEmpty().WithMessage("Thời gian bắt đầu không được để trống");

        RuleFor(x => x.EndTime)
            .NotEmpty().WithMessage("Thời gian kết thúc không được để trống")
            .GreaterThan(x => x.StartTime).WithMessage("Thời gian kết thúc phải sau thời gian bắt đầu");

        RuleFor(x => x.MaxCapacity)
            .GreaterThan(0).WithMessage("Sức chứa tối đa phải lớn hơn 0");

        RuleFor(x => x.IsRecurring)
            .NotNull().WithMessage("IsRecurring không được để trống");

        RuleFor(x => x.SpecificDate)
            .NotNull().WithMessage("SpecificDate không được để trống")
            .When(x => x.IsRecurring == false);

        RuleFor(x => x.DayOfWeek)
            .NotEmpty().WithMessage("Ngày trong tuần không được để trống")
            .Must(day => DayConstant.ALLDAYS.Contains(day!)).WithMessage("Ngày trong tuần không hợp lệ: " + string.Join(", ", DayConstant.ALLDAYS))
            .When(x => x.IsRecurring == true);

        RuleFor(x => x.SpecialNotes)
            .MaximumLength(500).WithMessage("Ghi chú đặc biệt không được vượt quá 500 ký tự")
            .When(x => !string.IsNullOrEmpty(x.SpecialNotes));

    }
}

public class SlotUpdateModelValidator : AbstractValidator<SlotUpdateModel>
{
    public SlotUpdateModelValidator()
    {
        Include(new SlotCreateModelValidator());

        RuleFor(x => x.Price)
             .GreaterThanOrEqualTo(0).WithMessage("Giá phải lớn hơn hoặc bằng 0");

        RuleFor(x => x.ServiceStatus)
            .Must(status => string.IsNullOrEmpty(status) ||
                           status == SlotStatusConstant.AVAILABLE ||
                           status == SlotStatusConstant.CANCELLED ||
                           status == SlotStatusConstant.MAINTENANCE)
            .WithMessage("Trạng thái không hợp lệ")
            .When(x => x.ServiceStatus != null);
    }
}


public class SlotFilterQuery : FilterQuery
{
    [FromQuery(Name = "day_of_week")]
    public string? DayOfWeek { get; set; }
    [FromQuery(Name = "start_time")]
    public TimeSpan? StartTime { get; set; }
    [FromQuery(Name = "end_time")]
    public TimeSpan? EndTime { get; set; }
    [FromQuery(Name = "is_recurring")]
    public bool? IsRecurring { get; set; }
    [FromQuery(Name = "specific_date")]
    public DateTime? SpecificDate { get; set; }
}

