using FluentValidation;

namespace PetCafe.Application.Models.VaccinationRecordModels;

public class VaccinationRecordCreateModel
{
    public Guid PetId { get; set; }
    public DateTime VaccinationDate { get; set; }
    public DateTime? NextDueDate { get; set; }
    public string Name { get; set; } = default!;
    public string? Veterinarian { get; set; }
    public string? ClinicName { get; set; }
    public string? BatchNumber { get; set; }
    public string? Notes { get; set; }
    public Guid ScheduleId { get; set; }
}


public class VaccinationRecordUpdateModel : VaccinationRecordCreateModel
{
}

public class VaccinationRecordCreateModelValidator : AbstractValidator<VaccinationRecordCreateModel>
{
    public VaccinationRecordCreateModelValidator()
    {
        RuleFor(x => x.PetId)
            .NotEmpty().WithMessage("ID thú cưng không được để trống");

        RuleFor(x => x.VaccinationDate)
            .NotEmpty().WithMessage("Ngày tiêm chủng không được để trống")
            .Must(date => date.Date <= DateTime.UtcNow.Date)
            .WithMessage("Ngày tiêm chủng không được lớn hơn ngày hiện tại");

        RuleFor(x => x.NextDueDate)
            .GreaterThan(x => x.VaccinationDate).WithMessage("Ngày tiêm chủng tiếp theo phải sau ngày tiêm chủng hiện tại")
            .When(x => x.NextDueDate.HasValue);

        RuleFor(x => x.Veterinarian)
            .MaximumLength(100).WithMessage("Tên bác sĩ thú y không được vượt quá 100 ký tự")
            .When(x => !string.IsNullOrEmpty(x.Veterinarian));

        RuleFor(x => x.ClinicName)
            .MaximumLength(200).WithMessage("Tên phòng khám không được vượt quá 200 ký tự")
            .When(x => !string.IsNullOrEmpty(x.ClinicName));

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Tên vaccine không được để trống")
            .MaximumLength(100).WithMessage("Tên vaccine không được vượt quá 100 ký tự")
            .When(x => !string.IsNullOrEmpty(x.Name));

        RuleFor(x => x.BatchNumber)
            .MaximumLength(50).WithMessage("Số lô không được vượt quá 50 ký tự")
            .When(x => !string.IsNullOrEmpty(x.BatchNumber));

        RuleFor(x => x.Notes)
            .MaximumLength(500).WithMessage("Ghi chú không được vượt quá 500 ký tự")
            .When(x => !string.IsNullOrEmpty(x.Notes));

        RuleFor(x => x.ScheduleId)
            .NotEmpty().WithMessage("ID lịch tiêm chủng không được để trống");
    }
}

public class VaccinationRecordUpdateModelValidator : AbstractValidator<VaccinationRecordUpdateModel>
{
    public VaccinationRecordUpdateModelValidator()
    {
        Include(new VaccinationRecordCreateModelValidator());
    }
}