using System.ComponentModel.DataAnnotations.Schema;

namespace PetCafe.Domain.Entities;

[Table("transactions")]
public class Transaction : BaseEntity
{
    [Column("order_code")]
    public double OrderCode { get; set; }
    [Column("amount")]
    public double Amount { get; set; }
    [Column("description")]
    public string? Description { get; set; }
    [Column("account_number")]
    public string? AccountNumber { get; set; }
    [Column("reference")]
    public string? Reference { get; set; }
    [Column("transaction_date_time")]
    public string? TransactionDateTime { get; set; }
    [Column("currency")]
    public string? Currency { get; set; }
    [Column("payment_link_id")]
    public string? PaymentLinkId { get; set; }
    [Column("code")]
    public string? Code { get; set; }
    [Column("desc")]
    public string? Desc { get; set; }
    [Column("order_id")]
    public Guid OrderId { get; set; }
    [ForeignKey("OrderId")]
    public Order Order { get; set; } = null!;

}

