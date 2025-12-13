using Microsoft.EntityFrameworkCore;
using PetCafe.Application.Models.ShareModels;
using PetCafe.Application.Models.TransactionModels;
using PetCafe.Domain.Entities;

namespace PetCafe.Application.Services;

public interface ITransactionService
{
    Task<BasePagingResponseModel<Transaction>> GetAllPagingAsync(TransactionFilterQuery query);
}

public class TransactionService(IUnitOfWork _unitOfWork) : ITransactionService
{
    public async Task<BasePagingResponseModel<Transaction>> GetAllPagingAsync(TransactionFilterQuery query)
    {
        var (Pagination, Entities) = await _unitOfWork.TransactionRepository.ToPagination(
            pageIndex: query.Page ?? 0,
            pageSize: query.Limit ?? 10,
            searchTerm: query.Q,
            searchFields: ["OrderCode", "PaymentMethod", "Status"],
            sortOrders: query.OrderBy?.ToDictionary(
                k => k.OrderColumn ?? "CreatedAt",
                v => (v.OrderDir ?? "ASC").Equals("ASC", StringComparison.CurrentCultureIgnoreCase)
            ) ?? new Dictionary<string, bool> { { "CreatedAt", false } },
            includeFunc: x => x.Include(x => x.Order)
        );


        return BasePagingResponseModel<Transaction>.CreateInstance(Entities, Pagination);
    }
}