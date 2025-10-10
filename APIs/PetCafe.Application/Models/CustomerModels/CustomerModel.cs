using FluentValidation;

namespace PetCafe.Application.Models.CustomerModels;

public class CustomerCreateModel
{
    public required string FullName { get; set; } = default!;
    public required string Phone { get; set; }
    public string? Address { get; set; }
    public string? AvatarUrl { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public required string Password { get; set; }
}

public class CustomerUpdateModel : CustomerCreateModel
{
}

public class CustomerCreateModelValidator : AbstractValidator<CustomerCreateModel>
{
    public CustomerCreateModelValidator()
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

        RuleFor(x => x.AvatarUrl)
            .MaximumLength(1000).WithMessage("Đường dẫn ảnh đại diện không được vượt quá 1000 ký tự")
            .When(x => !string.IsNullOrEmpty(x.AvatarUrl));

        RuleFor(x => x.DateOfBirth)
            .LessThan(DateTime.Now).WithMessage("Ngày sinh phải nhỏ hơn ngày hiện tại")
            .When(x => x.DateOfBirth.HasValue);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Mật khẩu không được để trống")
            .MinimumLength(6).WithMessage("Mật khẩu phải có ít nhất 6 ký tự")
            .MaximumLength(100).WithMessage("Mật khẩu không được vượt quá 100 ký tự")
            .Matches("[A-Z]").WithMessage("Mật khẩu phải có ít nhất 1 chữ cái viết hoa")
            .Matches("[a-z]").WithMessage("Mật khẩu phải có ít nhất 1 chữ cái viết thường")
            .Matches("[0-9]").WithMessage("Mật khẩu phải có ít nhất 1 chữ số")
            .Matches("[^a-zA-Z0-9]").WithMessage("Mật khẩu phải có ít nhất 1 ký tự đặc biệt");
    }
}

public class CustomerUpdateModelValidator : AbstractValidator<CustomerUpdateModel>
{
    public CustomerUpdateModelValidator()
    {
        Include(new CustomerCreateModelValidator());
    }
}