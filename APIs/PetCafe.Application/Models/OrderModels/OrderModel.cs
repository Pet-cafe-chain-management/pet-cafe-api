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
    public string PaymentMethod { get; set; } = PaymentMethodConstant.QR_CODE;


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