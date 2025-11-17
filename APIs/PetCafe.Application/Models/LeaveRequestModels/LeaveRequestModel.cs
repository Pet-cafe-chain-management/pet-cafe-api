using FluentValidation;
using PetCafe.Application.Models.ShareModels;
using PetCafe.Domain.Constants;

namespace PetCafe.Application.Models.LeaveRequestModels;

public class LeaveRequestCreateModel
{
    public Guid EmployeeId { get; set; }
    public Guid? ReplacementEmployeeId { get; set; } // Bắt buộc cho ADVANCE, null cho EMERGENCY
    public DateTime LeaveDate { get; set; }
    public string Reason { get; set; } = default!;
    public string LeaveType { get; set; } = LeaveRequestTypeConstant.ADVANCE;
}

public class LeaveRequestUpdateModel : LeaveRequestCreateModel
{
}

public class LeaveRequestReviewModel
{
    public string Status { get; set; } = LeaveRequestStatusConstant.APPROVED;
    public string? ReviewNotes { get; set; }
}

public class LeaveRequestFilterQuery : FilterQuery
{
    public Guid? EmployeeId { get; set; }
    public Guid? ReviewedBy { get; set; }
    public string? Status { get; set; }
    public string? LeaveType { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

public class LeaveRequestCreateModelValidator : AbstractValidator<LeaveRequestCreateModel>
{
    public LeaveRequestCreateModelValidator()
    {
        RuleFor(x => x.EmployeeId)
            .NotEmpty().WithMessage("ID nhân viên không được để trống");

        RuleFor(x => x.LeaveDate)
            .NotEmpty().WithMessage("Ngày nghỉ phép không được để trống");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Lý do nghỉ phép không được để trống")
            .MaximumLength(500).WithMessage("Lý do nghỉ phép không được vượt quá 500 ký tự");

        RuleFor(x => x.LeaveType)
            .NotEmpty().WithMessage("Loại nghỉ phép không được để trống")
            .Must(type => LeaveRequestTypeConstant.ALL_TYPES.Contains(type))
            .WithMessage($"Loại nghỉ phép phải là một trong: {string.Join(", ", LeaveRequestTypeConstant.ALL_TYPES)}");

        RuleFor(x => x.ReplacementEmployeeId)
            .NotEmpty().WithMessage("Nhân viên thay thế là bắt buộc cho nghỉ phép có kế hoạch")
            .When(x => x.LeaveType == LeaveRequestTypeConstant.ADVANCE);

        RuleFor(x => x.ReplacementEmployeeId)
            .Empty().WithMessage("Nghỉ phép đột xuất không cần nhân viên thay thế")
            .When(x => x.LeaveType == LeaveRequestTypeConstant.EMERGENCY);
    }
}

public class LeaveRequestUpdateModelValidator : AbstractValidator<LeaveRequestUpdateModel>
{
    public LeaveRequestUpdateModelValidator()
    {
        Include(new LeaveRequestCreateModelValidator());
    }
}

public class LeaveRequestReviewModelValidator : AbstractValidator<LeaveRequestReviewModel>
{
    public LeaveRequestReviewModelValidator()
    {
        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Trạng thái không được để trống")
            .Must(status => LeaveRequestStatusConstant.ALL_STATUSES.Contains(status))
            .WithMessage($"Trạng thái phải là một trong: {string.Join(", ", LeaveRequestStatusConstant.ALL_STATUSES)}");

        RuleFor(x => x.ReviewNotes)
            .MaximumLength(500).WithMessage("Ghi chú đánh giá không được vượt quá 500 ký tự")
            .When(x => !string.IsNullOrEmpty(x.ReviewNotes));
    }
}

