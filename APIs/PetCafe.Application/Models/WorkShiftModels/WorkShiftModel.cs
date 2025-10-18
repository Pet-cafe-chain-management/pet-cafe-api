using FluentValidation;
using PetCafe.Domain.Constants;

namespace PetCafe.Application.Models.WorkShiftModels;

public class WorkShiftCreateModel
{
    public string Name { get; set; } = default!;
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string? Description { get; set; }
    public List<string> ApplicableDays { get; set; } = DayConstant.ALLDAYS;

}

public class WorkShiftUpdateModel : WorkShiftCreateModel
{
    public bool IsActive { get; set; } = true;

}

public class WorkShiftCreateModelValidator : AbstractValidator<WorkShiftCreateModel>
{
    public WorkShiftCreateModelValidator()
    {

        RuleFor(x => x.ApplicableDays)
            .NotEmpty().WithMessage("Danh sách ngày áp dụng không được để trống");

        RuleForEach(x => x.ApplicableDays)
            .Must(day => DayConstant.ALLDAYS.Contains(day))
            .WithMessage("Ngày áp dụng phải là một trong các giá trị: " + string.Join(", ", DayConstant.ALLDAYS));

        RuleFor(x => x.StartTime)
            .NotEmpty().WithMessage("Thời gian bắt đầu không được để trống");

        RuleFor(x => x.EndTime)
            .NotEmpty().WithMessage("Thời gian kết thúc không được để trống")
            .GreaterThan(x => x.StartTime).WithMessage("Thời gian kết thúc phải sau thời gian bắt đầu");

    }
}

public class WorkShiftUpdateModelValidator : AbstractValidator<WorkShiftUpdateModel>
{
    public WorkShiftUpdateModelValidator()
    {
        Include(new WorkShiftCreateModelValidator());
    }
}