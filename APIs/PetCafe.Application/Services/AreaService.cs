using Microsoft.EntityFrameworkCore;
using PetCafe.Application.GlobalExceptionHandling.Exceptions;
using PetCafe.Application.Models.AreaModels;
using PetCafe.Application.Models.ShareModels;
using PetCafe.Domain.Entities;

namespace PetCafe.Application.Services;

public interface IAreaService
{
    Task<Area> GetByIdAsync(Guid id);
    Task<BasePagingResponseModel<Area>> GetAllPagingAsync(FilterQuery query);
    Task<Area> CreateAsync(AreaCreateModel model);
    Task<Area> UpdateAsync(Guid id, AreaUpdateModel model);
    Task<bool> DeleteAsync(Guid id);
}

public class AreaService(
    IUnitOfWork _unitOfWork
) : IAreaService
{
    public async Task<Area> CreateAsync(AreaCreateModel model)
    {
        var area = _unitOfWork.Mapper.Map<Area>(model);
        await _unitOfWork.AreaRepository.AddAsync(area);
        await _unitOfWork.SaveChangesAsync();
        return area;
    }

    public async Task<Area> UpdateAsync(Guid id, AreaUpdateModel model)
    {
        var area = await _unitOfWork.AreaRepository.GetByIdAsync(id) ?? throw new BadRequestException("Không tìm thấy thông tin!");
        _unitOfWork.Mapper.Map(model, area);
        _unitOfWork.AreaRepository.Update(area);
        await _unitOfWork.SaveChangesAsync();
        return area;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var area = await _unitOfWork.AreaRepository.GetByIdAsync(id) ?? throw new BadRequestException("Không tìm thấy thông tin!");
        _unitOfWork.AreaRepository.SoftRemove(area);
        return await _unitOfWork.SaveChangesAsync();
    }

    public async Task<Area> GetByIdAsync(Guid id)
    {
        return await _unitOfWork.AreaRepository.GetByIdAsync(id) ?? throw new BadRequestException("Không tìm thấy thông tin!");
    }

    public async Task<BasePagingResponseModel<Area>> GetAllPagingAsync(FilterQuery query)
    {

        var (Pagination, Entities) = await _unitOfWork.AreaRepository.ToPagination(
             pageIndex: query.Page ?? 0,
            pageSize: query.Limit ?? 10,
            searchTerm: query.Q,
            searchFields: ["FullName", "Phone", "Address"],
            sortOrders: query.OrderBy?.ToDictionary(
                    k => k.OrderColumn ?? "CreatedAt",
                    v => (v.OrderDir ?? "ASC").Equals("ASC", StringComparison.CurrentCultureIgnoreCase)
                ) ?? new Dictionary<string, bool> { { "CreatedAt", false } },
            includeFunc: x => x.Include(x => x.Employees.Where(e => !e.IsDeleted))
        );
        return BasePagingResponseModel<Area>.CreateInstance(Entities, Pagination); ;
    }
}