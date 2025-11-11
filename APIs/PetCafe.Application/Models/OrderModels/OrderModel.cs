using FluentValidation;
using PetCafe.Application.Models.ShareModels;
using PetCafe.Domain.Constants;

namespace PetCafe.Application.Models.OrderModels;


public class OrderCreateModel
{
    public string? FullName { get; set; } = default;
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Notes { get; set; }
    public List<ProductOrderModel>? Products { get; set; } = [];
    public List<ServiceOrderModel>? Services { get; set; } = [];
    public string PaymentMethod { get; set; } = PaymentMethodConstant.ONLINE;


}

public class CartInfoUpdateModel
{
    public string? FullName { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Notes { get; set; }
    public string? PaymentMethod { get; set; }
}


public class ProductOrderModel
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public string? Notes { get; set; }

}

public class ServiceOrderModel
{
    public Guid SlotId { get; set; }
    public string? Notes { get; set; }
    public DateTime BookingDate { get; set; }

}

public class OrderFilterQuery : FilterQuery
{
    public string? Type { get; set; }
    public string? Status { get; set; }
    public string? PaymentMethod { get; set; }
    public int? MinPrice { get; set; }
    public int? MaxPrice { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public Guid? EmployeeId { get; set; }
}

public class ProductOrderModelValidator : AbstractValidator<ProductOrderModel>
{
    public ProductOrderModelValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("ID sản phẩm không được để trống");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Số lượng phải lớn hơn 0");

        RuleFor(x => x.Notes)
            .MaximumLength(500).WithMessage("Ghi chú không được vượt quá 500 ký tự")
            .When(x => !string.IsNullOrEmpty(x.Notes));
    }
}

public class ServiceOrderModelValidator : AbstractValidator<ServiceOrderModel>
{
    public ServiceOrderModelValidator()
    {
        RuleFor(x => x.SlotId)
            .NotEmpty().WithMessage("ID khung giờ không được để trống");

        RuleFor(x => x.BookingDate)
            .NotEmpty().WithMessage("Ngày đặt lịch không được để trống")
            .GreaterThanOrEqualTo(DateTime.Today).WithMessage("Ngày đặt lịch phải từ ngày hiện tại trở đi");

        RuleFor(x => x.Notes)
            .MaximumLength(500).WithMessage("Ghi chú không được vượt quá 500 ký tự")
            .When(x => !string.IsNullOrEmpty(x.Notes));
    }
}

public class OrderCreateModelValidator : AbstractValidator<OrderCreateModel>
{
    public OrderCreateModelValidator()
    {
        RuleFor(x => x.FullName)
            .MaximumLength(100).WithMessage("Họ tên không được vượt quá 100 ký tự")
            .When(x => !string.IsNullOrEmpty(x.FullName));

        RuleFor(x => x.Address)
            .MaximumLength(500).WithMessage("Địa chỉ không được vượt quá 500 ký tự")
            .When(x => !string.IsNullOrEmpty(x.Address));

        RuleFor(x => x.Phone)
            .Matches(@"^(0|\+84)(\s|\.)?((3[2-9])|(5[689])|(7[06-9])|(8[1-689])|(9[0-46-9]))(\d)(\s|\.)?(\d{3})(\s|\.)?(\d{3})$")
            .WithMessage("Số điện thoại không đúng định dạng")
            .When(x => !string.IsNullOrEmpty(x.Phone));

        RuleFor(x => x.Notes)
            .MaximumLength(1000).WithMessage("Ghi chú không được vượt quá 1000 ký tự")
            .When(x => !string.IsNullOrEmpty(x.Notes));

        RuleFor(x => x.PaymentMethod)
            .NotEmpty().WithMessage("Phương thức thanh toán không được để trống")
            .Must(method => method == PaymentMethodConstant.ONLINE ||
                           method == PaymentMethodConstant.AT_COUNTER)
            .WithMessage("Phương thức thanh toán không hợp lệ");

        RuleFor(x => x)
            .Must(order => (order.Products != null && order.Products.Count > 0) ||
                          (order.Services != null && order.Services.Count > 0))
            .WithMessage("Đơn hàng phải có ít nhất một sản phẩm hoặc một dịch vụ");

        RuleForEach(x => x.Products)
            .SetValidator(new ProductOrderModelValidator())
            .When(x => x.Products != null && x.Products.Count > 0);

        RuleForEach(x => x.Services)
            .SetValidator(new ServiceOrderModelValidator())
            .When(x => x.Services != null && x.Services.Count > 0);
    }
}