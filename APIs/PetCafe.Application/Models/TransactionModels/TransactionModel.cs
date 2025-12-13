using PetCafe.Application.Models.ShareModels;

namespace PetCafe.Application.Models.TransactionModels;

public class TransactionFilterQuery : FilterQuery
{
    public string? OrderCode { get; set; }
    public string? PaymentMethod { get; set; }
    public string? Status { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
