using FluentValidation;

namespace PetCafe.Application.Models.HealthRecordModels;

public class HealthRecordCreateModel
{
    public Guid PetId { get; set; }
    public DateTime CheckDate { get; set; } = DateTime.UtcNow;
    public double? Weight { get; set; } = 0;
    public double? Temperature { get; set; }
    public string HealthStatus { get; set; } = default!; // Healthy, Sick, Recovering
    public string? Symptoms { get; set; }
    public string? Treatment { get; set; }
    public string? Veterinarian { get; set; }
    public DateTime? NextCheckDate { get; set; }
    public string? Notes { get; set; }

}

public class HealthRecordUpdateModel : HealthRecordCreateModel
{
}
public class HealthRecordCreateModelValidator : AbstractValidator<HealthRecordCreateModel>
{
    public HealthRecordCreateModelValidator()
    {
        RuleFor(x => x.PetId)
            .NotEmpty().WithMessage("ID thú cưng không được để trống");

        RuleFor(x => x.CheckDate)
            .NotEmpty().WithMessage("Ngày kiểm tra không được để trống")
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Ngày kiểm tra không được lớn hơn ngày hiện tại");

        RuleFor(x => x.Weight)
            .GreaterThan(0).WithMessage("Cân nặng phải lớn hơn 0")
            .When(x => x.Weight.HasValue);

        RuleFor(x => x.Temperature)
            .GreaterThan(0).WithMessage("Nhiệt độ phải lớn hơn 0")
            .When(x => x.Temperature.HasValue);

        RuleFor(x => x.HealthStatus)
            .NotEmpty().WithMessage("Tình trạng sức khỏe không được để trống")
            .Must(status => status == "Healthy" || status == "Sick" || status == "Recovering")
            .WithMessage("Tình trạng sức khỏe phải là một trong các giá trị: Healthy, Sick, Recovering");

        RuleFor(x => x.Symptoms)
            .MaximumLength(500).WithMessage("Triệu chứng không được vượt quá 500 ký tự")
            .When(x => !string.IsNullOrEmpty(x.Symptoms));

        RuleFor(x => x.Treatment)
            .MaximumLength(500).WithMessage("Phương pháp điều trị không được vượt quá 500 ký tự")
            .When(x => !string.IsNullOrEmpty(x.Treatment));

        RuleFor(x => x.Veterinarian)
            .MaximumLength(100).WithMessage("Tên bác sĩ thú y không được vượt quá 100 ký tự")
            .When(x => !string.IsNullOrEmpty(x.Veterinarian));

        RuleFor(x => x.NextCheckDate)
            .GreaterThan(x => x.CheckDate).WithMessage("Ngày kiểm tra tiếp theo phải sau ngày kiểm tra hiện tại")
            .When(x => x.NextCheckDate.HasValue);

        RuleFor(x => x.Notes)
            .MaximumLength(1000).WithMessage("Ghi chú không được vượt quá 1000 ký tự")
            .When(x => !string.IsNullOrEmpty(x.Notes));
    }
}

public class HealthRecordUpdateModelValidator : AbstractValidator<HealthRecordUpdateModel>
{
    public HealthRecordUpdateModelValidator()
    {
        Include(new HealthRecordCreateModelValidator());
    }
}