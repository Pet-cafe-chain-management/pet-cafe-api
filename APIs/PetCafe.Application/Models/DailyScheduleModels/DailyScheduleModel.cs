using PetCafe.Application.Models.ShareModels;
using PetCafe.Domain.Constants;
using FluentValidation;

namespace PetCafe.Application.Models.DailyScheduleModels;

public class DailyScheduleFilterQuery : FilterQuery
{
    public Guid? TeamId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string? Status { get; set; }
}

public class DailyScheduleUpdateModel
{
    public Guid Id { get; set; }
    public string Status { get; set; } = default!;
    public string? Notes { get; set; }
}

public class DailyScheduleUpdateModelValidator : AbstractValidator<DailyScheduleUpdateModel>
{
    public DailyScheduleUpdateModelValidator()
    {
        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Trạng thái không được để trống")
            .Must(status => DailyScheduleStatusConstant.ALL_STATUSES.Contains(status)).WithMessage("Trạng thái không hợp lệ");

        RuleFor(x => x.Notes)
            .MaximumLength(500).WithMessage("Ghi chú không được vượt quá 500 ký tự")
            .When(x => !string.IsNullOrEmpty(x.Notes));
    }
}