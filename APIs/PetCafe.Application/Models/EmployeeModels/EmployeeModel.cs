using FluentValidation;
using PetCafe.Domain.Constants;

namespace PetCafe.Application.Models.EmployeeModels;

public class EmployeeCreateModel
{
    public required string FullName { get; set; } = default!;
    public required string Phone { get; set; }
    public string? Address { get; set; }
    public double Salary { get; set; } = 0;
    public List<string> Skills { get; set; } = [];
    public Guid? AreaId { get; set; }
    public required string Email { get; set; } = default!;
    public required string AvatarUrl { get; set; } = default!;
    public required string Password { get; set; }
    public string SubRole { get; set; } = SubRoleConstants.WORKING_STAFF;
}

public class EmployeeUpdateModel : EmployeeCreateModel
{
}

public class EmployeeCreateModelValidator : AbstractValidator<EmployeeCreateModel>
{
    public EmployeeCreateModelValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Họ tên không được để trống")
            .MaximumLength(100).WithMessage("Họ tên không được vượt quá 100 ký tự");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Số điện thoại không được để trống")
            .Matches(@"^(0|\+84)(\s|\.)?((3[2-9])|(5[689])|(7[06-9])|(8[1-689])|(9[0-46-9]))(\d)(\s|\.)?(\d{3})(\s|\.)?(\d{3})$")
            .WithMessage("Số điện thoại không đúng định dạng");

        RuleFor(x => x.Address)
            .MaximumLength(500).WithMessage("Địa chỉ không được vượt quá 500 ký tự")
            .When(x => !string.IsNullOrEmpty(x.Address));

        RuleFor(x => x.Salary)
            .GreaterThanOrEqualTo(0).WithMessage("Lương phải lớn hơn hoặc bằng 0");

        RuleForEach(x => x.Skills)
            .NotEmpty().WithMessage("Kỹ năng không được để trống")
            .MaximumLength(100).WithMessage("Kỹ năng không được vượt quá 100 ký tự");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email không được để trống")
            .EmailAddress().WithMessage("Email không đúng định dạng")
            .MaximumLength(100).WithMessage("Email không được vượt quá 100 ký tự");

        RuleFor(x => x.AvatarUrl)
            .NotEmpty().WithMessage("Đường dẫn ảnh đại diện không được để trống")
            .MaximumLength(1000).WithMessage("Đường dẫn ảnh đại diện không được vượt quá 1000 ký tự");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Mật khẩu không được để trống")
            .MinimumLength(6).WithMessage("Mật khẩu phải có ít nhất 6 ký tự")
            .MaximumLength(100).WithMessage("Mật khẩu không được vượt quá 100 ký tự")
            .Matches("[A-Z]").WithMessage("Mật khẩu phải có ít nhất 1 chữ cái viết hoa")
            .Matches("[a-z]").WithMessage("Mật khẩu phải có ít nhất 1 chữ cái viết thường")
            .Matches("[0-9]").WithMessage("Mật khẩu phải có ít nhất 1 chữ số")
            .Matches("[^a-zA-Z0-9]").WithMessage("Mật khẩu phải có ít nhất 1 ký tự đặc biệt");

        RuleFor(x => x.SubRole)
            .NotEmpty().WithMessage("Vui trò nhập vai trò của nhân viên")
            .Must(subRole => subRole == SubRoleConstants.WORKING_STAFF || subRole == SubRoleConstants.SALE_STAFF)
            .WithMessage($"Vai trò phụ phải là một trong các giá trị: {SubRoleConstants.WORKING_STAFF}, {SubRoleConstants.SALE_STAFF}");
    }
}

public class EmployeeUpdateModelValidator : AbstractValidator<EmployeeUpdateModel>
{
    public EmployeeUpdateModelValidator()
    {
        Include(new EmployeeCreateModelValidator());
    }
}