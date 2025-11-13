using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PetCafe.Application.GlobalExceptionHandling.Exceptions;
using PetCafe.Application.Models.ShareModels;
using PetCafe.Application.Models.VaccineTypeModels;
using PetCafe.Application.Utilities;
using PetCafe.Domain.Entities;

namespace PetCafe.Application.Services;

public interface IVaccineTypeService
{
    Task<VaccineType> GetByIdAsync(Guid id);
    Task<BasePagingResponseModel<VaccineType>> GetAllPagingAsync(VaccineTypeFilterQuery query);
    Task<VaccineType> CreateAsync(VaccineTypeCreateModel model);
    Task<VaccineType> UpdateAsync(Guid id, VaccineTypeUpdateModel model);
    Task<bool> DeleteAsync(Guid id);
}

public class VaccineTypeService(
    IUnitOfWork _unitOfWork
) : IVaccineTypeService
{
    public async Task<VaccineType> CreateAsync(VaccineTypeCreateModel model)
    {
        var vaccineType = _unitOfWork.Mapper.Map<VaccineType>(model);
        await _unitOfWork.VaccineTypeRepository.AddAsync(vaccineType);
        await _unitOfWork.SaveChangesAsync();
        return vaccineType;
    }

    public async Task<VaccineType> UpdateAsync(Guid id, VaccineTypeUpdateModel model)
    {
        var vaccineType = await _unitOfWork.VaccineTypeRepository.GetByIdAsync(id) ?? throw new BadRequestException("Không tìm thấy thông tin!");
        _unitOfWork.Mapper.Map(model, vaccineType);
        _unitOfWork.VaccineTypeRepository.Update(vaccineType);
        await _unitOfWork.SaveChangesAsync();
        return vaccineType;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var vaccineType = await _unitOfWork.VaccineTypeRepository.GetByIdAsync(id) ?? throw new BadRequestException("Không tìm thấy thông tin!");
        _unitOfWork.VaccineTypeRepository.SoftRemove(vaccineType);
        return await _unitOfWork.SaveChangesAsync();
    }

    public async Task<VaccineType> GetByIdAsync(Guid id)
    {
        return await _unitOfWork.VaccineTypeRepository.GetByIdAsync(id) ?? throw new BadRequestException("Không tìm thấy thông tin!");
    }

    public async Task<BasePagingResponseModel<VaccineType>> GetAllPagingAsync(VaccineTypeFilterQuery query)
    {
        Expression<Func<VaccineType, bool>>? filter = null;


        if (query.SpeciesId.HasValue)
        {
            Expression<Func<VaccineType, bool>> tmp_filter = x => x.SpeciesId == query.SpeciesId.Value;
            filter = filter != null ? FilterCustoms.CombineFilters(filter, tmp_filter) : tmp_filter;

        }

        if (string.IsNullOrEmpty(query.Name) == false)
        {
            Expression<Func<VaccineType, bool>> tmp_filter = x => x.Name.Contains(query.Name);
            filter = filter != null ? FilterCustoms.CombineFilters(filter, tmp_filter) : tmp_filter;
        }

        if (query.IsRequired.HasValue)
        {
            Expression<Func<VaccineType, bool>> tmp_filter = x => x.IsRequired == query.IsRequired.Value;
            filter = filter != null ? FilterCustoms.CombineFilters(filter, tmp_filter) : tmp_filter;
        }


        var (Pagination, Entities) = await _unitOfWork.VaccineTypeRepository.ToPagination(
            pageIndex: query.Page ?? 0,
           pageSize: query.Limit ?? 10,
            filter: filter,
           searchTerm: query.Q,
           searchFields: ["Name", "Description"],
           sortOrders: query.OrderBy?.ToDictionary(
                   k => k.OrderColumn ?? "CreatedAt",
                   v => (v.OrderDir ?? "ASC").Equals("ASC", StringComparison.CurrentCultureIgnoreCase)
               ) ?? new Dictionary<string, bool> { { "CreatedAt", false } },
           includeFunc: x => x.Include(x => x.Species!)
       );
        return BasePagingResponseModel<VaccineType>.CreateInstance(Entities, Pagination);
    }
}