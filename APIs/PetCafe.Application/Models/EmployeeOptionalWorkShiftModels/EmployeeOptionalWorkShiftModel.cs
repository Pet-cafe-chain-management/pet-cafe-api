using FluentValidation;

namespace PetCafe.Application.Models.EmployeeOptionalWorkShiftModels;

public class EmployeeOptionalWorkShiftCreateModel
{
    public Guid EmployeeId { get; set; }
    public Guid WorkShiftId { get; set; }
    public int Priority { get; set; } = 1;
    public bool IsAvailable { get; set; } = true;
}

public class EmployeeOptionalWorkShiftUpdateModel
{
    public int Priority { get; set; }
    public bool IsAvailable { get; set; }
}

public class EmployeeOptionalWorkShiftCreateModelValidator : AbstractValidator<EmployeeOptionalWorkShiftCreateModel>
{
    public EmployeeOptionalWorkShiftCreateModelValidator()
    {
        RuleFor(x => x.EmployeeId)
            .NotEmpty().WithMessage("ID nhân viên không được để trống");

        RuleFor(x => x.WorkShiftId)
            .NotEmpty().WithMessage("ID ca làm việc không được để trống");

        RuleFor(x => x.Priority)
            .GreaterThan(0).WithMessage("Độ ưu tiên phải lớn hơn 0")
            .LessThanOrEqualTo(10).WithMessage("Độ ưu tiên không được vượt quá 10");
    }
}

public class EmployeeOptionalWorkShiftUpdateModelValidator : AbstractValidator<EmployeeOptionalWorkShiftUpdateModel>
{
    public EmployeeOptionalWorkShiftUpdateModelValidator()
    {
        RuleFor(x => x.Priority)
            .GreaterThan(0).WithMessage("Độ ưu tiên phải lớn hơn 0")
            .LessThanOrEqualTo(10).WithMessage("Độ ưu tiên không được vượt quá 10");
    }
}

