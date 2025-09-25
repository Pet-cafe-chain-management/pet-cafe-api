using Microsoft.EntityFrameworkCore;
using PetCafe.Application.GlobalExceptionHandling.Exceptions;
using PetCafe.Application.Models.CategoryModels;
using PetCafe.Application.Models.ShareModels;
using PetCafe.Domain.Entities;

namespace PetCafe.Application.Services;

public interface ICategoryService
{
    Task<ProductCategory> CreateAsync(CategoryCreateModel model);
    Task<ProductCategory> UpdateAsync(Guid id, CategoryUpdateModel model);
    Task<bool> DeleteAsync(Guid id);
    Task<ProductCategory> GetByIdAsync(Guid id);
    Task<BasePagingResponseModel<ProductCategory>> GetAllPagingAsync(FilterQuery query);
}

public class CategoryService(
    IUnitOfWork _unitOfWork
) : ICategoryService
{
    public async Task<ProductCategory> CreateAsync(CategoryCreateModel model)
    {
        var category = _unitOfWork.Mapper.Map<ProductCategory>(model);
        await _unitOfWork.ProductCategoryRepository.AddAsync(category);
        await _unitOfWork.SaveChangesAsync();
        return category;
    }

    public async Task<ProductCategory> UpdateAsync(Guid id, CategoryUpdateModel model)
    {
        var category = await _unitOfWork.ProductCategoryRepository.GetByIdAsync(id) ?? throw new BadRequestException("Không tìm thấy thông tin!");
        _unitOfWork.Mapper.Map(model, category);
        _unitOfWork.ProductCategoryRepository.Update(category);
        await _unitOfWork.SaveChangesAsync();
        return category;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var category = await _unitOfWork.ProductCategoryRepository.GetByIdAsync(id) ?? throw new BadRequestException("Không tìm thấy thông tin!");
        _unitOfWork.ProductCategoryRepository.SoftRemove(category);
        return await _unitOfWork.SaveChangesAsync();
    }

    public async Task<ProductCategory> GetByIdAsync(Guid id)
    {
        return await _unitOfWork.ProductCategoryRepository.GetByIdAsync(id) ?? throw new BadRequestException("Không tìm thấy thông tin!");
    }

    public async Task<BasePagingResponseModel<ProductCategory>> GetAllPagingAsync(FilterQuery query)
    {
        var (Pagination, Entities) = await _unitOfWork.ProductCategoryRepository.ToPagination(
            pageIndex: query.Page ?? 0,
            pageSize: query.Limit ?? 10,
            searchTerm: query.Q,
            searchFields: ["FullName", "Phone", "Address"],
            sortOrders: query.OrderBy?.ToDictionary(
                    k => k.OrderColumn ?? "CreatedAt",
                    v => (v.OrderDir ?? "ASC").Equals("ASC", StringComparison.CurrentCultureIgnoreCase)
                ) ?? new Dictionary<string, bool> { { "CreatedAt", false } },
            includeFunc: x => x.Include(x => x.Products.Where(p => !p.IsDeleted))
        );

        return BasePagingResponseModel<ProductCategory>.CreateInstance(Entities, Pagination); ;

    }
}