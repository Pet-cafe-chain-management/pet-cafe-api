namespace PetCafe.Application.Models.OrderModels;


public class OrderCreateModel
{
    public string? FullName { get; set; } = default;
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Notes { get; set; }
    public List<ProductOrderModel>? Products { get; set; } = [];
    public List<ServiceOrderModel>? Services { get; set; } = [];

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

}