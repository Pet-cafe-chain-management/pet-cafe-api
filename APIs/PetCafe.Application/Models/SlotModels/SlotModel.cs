using FluentValidation;
using PetCafe.Domain.Constants;

namespace PetCafe.Application.Models.SlotModels;

public class SlotCreateModel
{
    public Guid ServiceId { get; set; }
    public Guid AreaId { get; set; }
    public Guid TeamId { get; set; }
    public Guid PetGroupId { get; set; }
    public List<string> ApplicableDays { get; set; } = DayConstant.ALLDAYS;
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public int MaxCapacity { get; set; }
    public double Price { get; set; }
    public string? SpecialNotes { get; set; }

}

public class SlotUpdateModel : SlotCreateModel
{
    public bool IsActive { get; set; } = true;
    public string? Status { get; set; }
}

public class SlotCreateModelValidator : AbstractValidator<SlotCreateModel>
{
    public SlotCreateModelValidator()
    {
        RuleFor(x => x.ServiceId)
            .NotEmpty().WithMessage("ID dịch vụ không được để trống");

        RuleFor(x => x.AreaId)
            .NotEmpty().WithMessage("ID khu vực không được để trống");

        RuleFor(x => x.TeamId)
            .NotEmpty().WithMessage("ID đội không được để trống");

        RuleFor(x => x.PetGroupId)
            .NotEmpty().WithMessage("ID nhóm thú cưng không được để trống");

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

        RuleFor(x => x.MaxCapacity)
            .GreaterThan(0).WithMessage("Sức chứa tối đa phải lớn hơn 0");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0).WithMessage("Giá phải lớn hơn hoặc bằng 0");

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

        RuleFor(x => x.Status)
            .Must(status => string.IsNullOrEmpty(status) ||
                           status == SlotStatusConstant.AVAILABLE ||
                           status == SlotStatusConstant.CANCELLED ||
                           status == SlotStatusConstant.MAINTENANCE ||
                           status == SlotStatusConstant.FULL)
            .WithMessage("Trạng thái không hợp lệ")
            .When(x => x.Status != null);
    }
}