using Microsoft.EntityFrameworkCore;
using PetCafe.Application.GlobalExceptionHandling.Exceptions;
using PetCafe.Application.Models.ProductModels;
using PetCafe.Application.Models.ShareModels;
using PetCafe.Domain.Entities;

namespace PetCafe.Application.Services;

public interface IProductService
{
    Task<Product> GetByIdAsync(Guid id);
    Task<BasePagingResponseModel<Product>> GetAllPagingAsync(FilterQuery query);
    Task<Product> CreateAsync(ProductCreateModel model);
    Task<Product> UpdateAsync(Guid id, ProductUpdateModel model);
    Task<bool> DeleteAsync(Guid id);
}


public class ProductService(
    IUnitOfWork _unitOfWork
) : IProductService
{
    public async Task<Product> CreateAsync(ProductCreateModel model)
    {
        var product = _unitOfWork.Mapper.Map<Product>(model);
        await _unitOfWork.ProductRepository.AddAsync(product);
        await _unitOfWork.SaveChangesAsync();
        return product;
    }

    public async Task<Product> UpdateAsync(Guid id, ProductUpdateModel model)
    {
        var product = await _unitOfWork.ProductRepository.GetByIdAsync(id) ?? throw new BadRequestException("Không tìm thấy thông tin!");
        _unitOfWork.Mapper.Map(model, product);
        _unitOfWork.ProductRepository.Update(product);
        await _unitOfWork.SaveChangesAsync();
        return product;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var product = await _unitOfWork.ProductRepository.GetByIdAsync(id) ?? throw new BadRequestException("Không tìm thấy thông tin!");
        _unitOfWork.ProductRepository.SoftRemove(product);
        return await _unitOfWork.SaveChangesAsync();
    }

    public async Task<Product> GetByIdAsync(Guid id)
    {
        return await _unitOfWork.ProductRepository.GetByIdAsync(id) ?? throw new BadRequestException("Không tìm thấy thông tin!");
    }

    public async Task<BasePagingResponseModel<Product>> GetAllPagingAsync(FilterQuery query)
    {

        var (Pagination, Entities) = await _unitOfWork.ProductRepository.ToPagination(
             pageIndex: query.Page ?? 0,
            pageSize: query.Limit ?? 10,
            searchTerm: query.Q,
            searchFields: ["Name", "Description"],
            sortOrders: query.OrderBy?.ToDictionary(
                    k => k.OrderColumn ?? "CreatedAt",
                    v => (v.OrderDir ?? "ASC").Equals("ASC", StringComparison.CurrentCultureIgnoreCase)
                ) ?? new Dictionary<string, bool> { { "CreatedAt", false } },
            includeFunc: x => x.Include(x => x.Category)
        );
        return BasePagingResponseModel<Product>.CreateInstance(Entities, Pagination); ;
    }
}