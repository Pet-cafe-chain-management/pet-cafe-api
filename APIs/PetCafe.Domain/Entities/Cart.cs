using PetCafe.Domain.Constants;

namespace PetCafe.Domain.Entities;

public class Cart : BaseEntity
{
    public Guid CustomerId { get; set; }

    public string? FullName { get; set; }

    public string? Address { get; set; }

    public string? Phone { get; set; }

    public string? Notes { get; set; }

    public List<CartProductItem> Products { get; set; } = [];

    public List<CartServiceItem> Services { get; set; } = [];

    public string PaymentMethod { get; set; } = PaymentMethodConstant.ONLINE;
}

public class CartProductItem
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public string? Notes { get; set; }
}

public class CartServiceItem
{
    public Guid SlotId { get; set; }
    public string? Notes { get; set; }
    public DateTime BookingDate { get; set; }
}

