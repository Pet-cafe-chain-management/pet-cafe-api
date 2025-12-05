using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PetCafe.Application.GlobalExceptionHandling.Exceptions;
using PetCafe.Application.Models.ProductModels;
using PetCafe.Application.Models.ShareModels;
using PetCafe.Application.Utilities;
using PetCafe.Domain.Entities;

namespace PetCafe.Application.Services;

public interface IProductService
{
    Task<Product> GetByIdAsync(Guid id);
    Task<BasePagingResponseModel<Product>> GetAllPagingAsync(ProductFilterQuery query);
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

    public async Task<BasePagingResponseModel<Product>> GetAllPagingAsync(ProductFilterQuery query)
    {
        Expression<Func<Product, bool>>? filter = null;

        if (query.IsActive != null)
        {
            Expression<Func<Product, bool>> filter_status = x => x.IsActive == query.IsActive;
            filter = filter != null ? FilterCustoms.CombineFilters(filter, filter_status) : filter_status;
        }

        if (query.IsForPets != null)
        {
            Expression<Func<Product, bool>> filter_isForPets = x => x.IsForPets == query.IsForPets;
            filter = filter != null ? FilterCustoms.CombineFilters(filter, filter_isForPets) : filter_isForPets;
        }

        if (query.MinPrice != null || query.MaxPrice != null)
        {
            int min = query.MinPrice ?? 0;
            int max = query.MaxPrice ?? int.MaxValue;
            var filter_price = FilterByPrice(max, min);
            filter = filter != null ? FilterCustoms.CombineFilters(filter, filter_price) : filter_price;
        }

        if (query.MinStockQuantity != 0 || query.MaxStockQuantity != int.MaxValue)
        {
            int min = query.MinStockQuantity;
            int max = query.MaxStockQuantity;
            var filter_quantity = FilterByQuantity(max, min);
            filter = filter != null ? FilterCustoms.CombineFilters(filter, filter_quantity) : filter_quantity;
        }

        if (query.MinCost != null || query.MaxCost != null)
        {
            int min = query.MinCost ?? 0;
            int max = query.MaxCost ?? int.MaxValue;
            var filter_cost = FilterByCost(max, min);
            filter = filter != null ? FilterCustoms.CombineFilters(filter, filter_cost) : filter_cost;
        }

        var (Pagination, Entities) = await _unitOfWork.ProductRepository.ToPagination(
            pageIndex: query.Page ?? 0,
            pageSize: query.Limit ?? 10,
            filter: filter,
            searchTerm: query.Q,
            searchFields: ["Name", "Description"],
            sortOrders: query.OrderBy?.ToDictionary(
                    k => k.OrderColumn ?? "CreatedAt",
                    v => (v.OrderDir ?? "ASC").Equals("ASC", StringComparison.CurrentCultureIgnoreCase)
                ) ?? new Dictionary<string, bool> { { "CreatedAt", false } },
            includeFunc: x => x.Include(x => x.Category)
        );
        return BasePagingResponseModel<Product>.CreateInstance(Entities, Pagination);
    }

    private static Expression<Func<Product, bool>> FilterByPrice(int max, int min)
    {
        Expression<Func<Product, bool>> filter = x => x.Price >= min && x.Price <= max;
        return filter;
    }
    private static Expression<Func<Product, bool>> FilterByQuantity(int max, int min)
    {
        Expression<Func<Product, bool>> filter = x => x.StockQuantity >= min && x.StockQuantity <= max;
        return filter;
    }
    private static Expression<Func<Product, bool>> FilterByCost(int max, int min)
    {
        Expression<Func<Product, bool>> filter = x => x.Cost >= min && x.Cost <= max;
        return filter;
    }

}