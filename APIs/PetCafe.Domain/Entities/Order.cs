using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using PetCafe.Domain.Constants;

namespace PetCafe.Domain.Entities;

[Table("orders")]
public class Order : BaseEntity
{
    [Column("customer_id")]
    [ForeignKey("Customer")]
    public Guid? CustomerId { get; set; }

    [Column("employee_id")]
    [ForeignKey("Employee")]
    public Guid? EmployeeId { get; set; }

    [Column("full_name")]
    [Required]
    [MaxLength(100)]
    public string? FullName { get; set; } = default!;

    [Column("address")]
    [MaxLength(200)]
    public string? Address { get; set; }

    [Column("order_number")]
    [Required]
    [MaxLength(20)]
    public string OrderNumber { get; set; } = default!;

    [Column("phone")]
    [MaxLength(15)]
    public string? Phone { get; set; }

    [Column("order_date")]
    public DateTime OrderDate { get; set; }

    [Column("total_amount")]
    public double TotalAmount { get; set; } = 0;

    [Column("discount_amount")]
    public double DiscountAmount { get; set; } = 0;

    [Column("final_amount")]
    public double FinalAmount { get; set; } = 0;

    [Column("payment_status")]
    [MaxLength(20)]
    public string PaymentStatus { get; set; } = PaymentStatusConstant.PENDING;

    [Column("payment_method")]
    [MaxLength(20)]
    public string? PaymentMethod { get; set; } = PaymentMethodConstant.QR_CODE;

    [Column("status")]
    [MaxLength(20)]
    public string Status { get; set; } = OrderStatusConstant.PENDING;
    [Column("notes")]
    [MaxLength(500)]
    public string? Notes { get; set; }

    [Column("payment_data")]
    public string? PaymentDataJson { get; set; }

    [NotMapped]
    public PaymentInfo? PaymentInfo
    {
        get => PaymentDataJson == null ? null : JsonSerializer.Deserialize<PaymentInfo>(PaymentDataJson);
        set => PaymentDataJson = value == null ? null : JsonSerializer.Serialize(value);
    }

    [Column("type")]
    public string Type { get; set; } = OrderTypeConstant.CUSTOMER;
    // Navigation properties
    public virtual Customer? Customer { get; set; } = default!;
    public virtual Employee? Employee { get; set; } = default!;
    public virtual ProductOrder? ProductOrder { get; set; } = default!;
    public virtual ServiceOrder? ServiceOrder { get; set; } = default!;
    public virtual ICollection<Transaction> Transactions { get; set; } = [];


}

public class PaymentInfo
{
    public string? Bin { get; set; }
    public string? AccountNumber { get; set; }
    public string? AccountName { get; set; }
    public string? Currency { get; set; }
    public string? PaymentLinkId { get; set; }
    public double Amount { get; set; }
    public string? Description { get; set; }
    public int OrderCode { get; set; }
    public long ExpiredAt { get; set; }
    public string? Status { get; set; }
    public string? CheckoutUrl { get; set; }
    public string? QrCode { get; set; }
}