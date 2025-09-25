using Microsoft.EntityFrameworkCore;
using PetCafe.Application.GlobalExceptionHandling.Exceptions;
using PetCafe.Application.Models.CustomerModels;
using PetCafe.Application.Models.ShareModels;
using PetCafe.Domain.Entities;

namespace PetCafe.Application.Services;

public interface ICustomerService
{
    Task<Customer> GetByIdAsync(Guid id);
    Task<BasePagingResponseModel<Customer>> GetAllPagingAsync(FilterQuery query);
    Task<Customer> UpdateAsync(Guid id, CustomerUpdateModel model);

}

public class CustomerService(
    IUnitOfWork _unitOfWork
) : ICustomerService
{
    public async Task<Customer> GetByIdAsync(Guid id)
    {
        return await _unitOfWork.CustomerRepository
            .GetByIdAsync(id) ?? throw new BadRequestException("Không tìm thấy thông tin!");
    }

    public async Task<BasePagingResponseModel<Customer>> GetAllPagingAsync(FilterQuery query)
    {
        var (Pagination, Entities) = await _unitOfWork.CustomerRepository.ToPagination(
             pageIndex: query.Page ?? 0,
            pageSize: query.Limit ?? 10,
            searchTerm: query.Q,
            searchFields: ["FullName", "Phone", "Address"],
            sortOrders: query.OrderBy?.ToDictionary(
                    k => k.OrderColumn ?? "CreatedAt",
                    v => (v.OrderDir ?? "ASC").Equals("ASC", StringComparison.CurrentCultureIgnoreCase)
                ) ?? new Dictionary<string, bool> { { "CreatedAt", false } },
            includeFunc: x => x.Include(x => x.Account)
        );
        return BasePagingResponseModel<Customer>.CreateInstance(Entities, Pagination); ;
    }

    public async Task<Customer> UpdateAsync(Guid id, CustomerUpdateModel model)
    {
        var customer = await _unitOfWork.CustomerRepository.GetByIdAsync(id) ?? throw new BadRequestException("Không tìm thấy thông tin");
        _unitOfWork.Mapper.Map(model, customer);
        _unitOfWork.CustomerRepository.Update(customer);
        await _unitOfWork.SaveChangesAsync();
        return customer;
    }
}