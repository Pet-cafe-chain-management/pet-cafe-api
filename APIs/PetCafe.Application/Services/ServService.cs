using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PetCafe.Application.GlobalExceptionHandling.Exceptions;
using PetCafe.Application.Models.ServiceModels;
using PetCafe.Application.Models.ShareModels;
using PetCafe.Application.Services.Commons;
using PetCafe.Application.Utilities;
using PetCafe.Domain.Constants;
using PetCafe.Domain.Entities;

namespace PetCafe.Application.Services;


public interface IServService
{

    Task<Service> GetByIdAsync(Guid id);
    Task<BasePagingResponseModel<Service>> GetAllPagingAsync(ServiceFilterQuery query);
    Task<Service> CreateAsync(ServiceCreateModel model);
    Task<Service> UpdateAsync(Guid id, ServiceUpdateModel model);
    Task<bool> DeleteAsync(Guid id);
}

public class ServService(IUnitOfWork _unitOfWork, IClaimsService _claimsService) : IServService
{
    public async Task<Service> CreateAsync(ServiceCreateModel model)
    {
        var service = _unitOfWork.Mapper.Map<Service>(model);
        await _unitOfWork.ServiceRepository.AddAsync(service);
        await _unitOfWork.SaveChangesAsync();
        return service;
    }

    public async Task<Service> UpdateAsync(Guid id, ServiceUpdateModel model)
    {
        var service = await _unitOfWork.ServiceRepository.GetByIdAsync(id) ?? throw new BadRequestException("Không tìm thấy thông tin!");
        _unitOfWork.Mapper.Map(model, service);
        _unitOfWork.ServiceRepository.Update(service);
        await _unitOfWork.SaveChangesAsync();
        return service;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var service = await _unitOfWork.ServiceRepository.GetByIdAsync(id) ?? throw new BadRequestException("Không tìm thấy thông tin!");
        _unitOfWork.ServiceRepository.SoftRemove(service);
        return await _unitOfWork.SaveChangesAsync();
    }

    public async Task<Service> GetByIdAsync(Guid id)
    {
        return await _unitOfWork.ServiceRepository.GetByIdAsync(id,
            includeFunc: x => x.Include(x => x.Slots.Where(x => !x.IsDeleted)).ThenInclude(x => x.PetGroup)
        ) ?? throw new BadRequestException("Không tìm thấy thông tin!");
    }

    public async Task<BasePagingResponseModel<Service>> GetAllPagingAsync(ServiceFilterQuery query)
    {
        #region Filter

        Expression<Func<Service, bool>>? filter = null;

        if (_claimsService.GetCurrentUserRole != RoleConstants.MANAGER)
        {
            Expression<Func<Service, bool>> tmp_filter = x => x.Slots.Any(slot =>
                 slot.IsActive &&
                 slot.Status == SlotStatusConstant.AVAILABLE &&
                 slot.AvailableCapacity > 0);
            filter = filter != null ? FilterCustoms.CombineFilters(filter, tmp_filter) : tmp_filter;
        }

        if (query.SearchDate.HasValue)
        {
            var dayOfWeek = query.SearchDate.Value.DayOfWeek.ToString();

            Expression<Func<Service, bool>> tmp_filter = x => x.Slots.Any(slot => slot.ApplicableDays.Contains(dayOfWeek));
            filter = filter != null ? FilterCustoms.CombineFilters(filter, tmp_filter) : tmp_filter;

        }

        if (query.StartTime.HasValue)
        {
            Expression<Func<Service, bool>> tmp_filter = x => x.Slots.Any(slot =>
                slot.StartTime <= query.StartTime.Value &&
                slot.EndTime >= query.StartTime.Value);
            filter = filter != null ? FilterCustoms.CombineFilters(filter, tmp_filter) : tmp_filter;
        }

        if (query.EndTime.HasValue)
        {
            Expression<Func<Service, bool>> tmp_filter = x => x.Slots.Any(slot =>
                slot.StartTime <= query.EndTime.Value &&
                slot.EndTime >= query.EndTime.Value);
            filter = filter != null ? FilterCustoms.CombineFilters(filter, tmp_filter) : tmp_filter;
        }

        if (query.MaxPrice.HasValue && query.MinPrice.HasValue)
        {
            Expression<Func<Service, bool>> tmp_filter = x => x.Slots.Any(slot =>
                slot.Price >= query.MinPrice && slot.Price <= query.MaxPrice);
            filter = filter != null ? FilterCustoms.CombineFilters(filter, tmp_filter) : tmp_filter;
        }

        if (query.PetBreedIds != null && query.PetBreedIds.Count > 0)
        {
            Expression<Func<Service, bool>> tmp_filter = x => x.Slots.Any(slot =>
               query.PetBreedIds.Contains(slot.PetGroup.PetBreedId!.Value));
            filter = filter != null ? FilterCustoms.CombineFilters(filter, tmp_filter) : tmp_filter;
        }

        if (query.PetSpeciesIds != null && query.PetSpeciesIds.Count > 0)
        {
            Expression<Func<Service, bool>> tmp_filter = x => x.Slots.Any(slot =>
               query.PetSpeciesIds.Contains(slot.PetGroup.PetSpeciesId!.Value));
            filter = filter != null ? FilterCustoms.CombineFilters(filter, tmp_filter) : tmp_filter;
        }

        if (query.AreaIds != null && query.AreaIds.Count > 0)
        {
            Expression<Func<Service, bool>> tmp_filter = x => x.Slots.Any(slot =>
               query.AreaIds.Contains(slot.AreaId));
            filter = filter != null ? FilterCustoms.CombineFilters(filter, tmp_filter) : tmp_filter;
        }

        if (query.ServiceTypes != null && query.ServiceTypes.Count > 0)
        {
            Expression<Func<Service, bool>> tmp_filter = x => query.ServiceTypes.Contains(x.ServiceType);
            filter = filter != null ? FilterCustoms.CombineFilters(filter, tmp_filter) : tmp_filter;
        }

        if (_claimsService.GetCurrentUserRole == RoleConstants.MANAGER)
        {
            Expression<Func<Service, bool>> tmp_filter = x => x.IsActive == query.IsActive;
            filter = filter != null ? FilterCustoms.CombineFilters(filter, tmp_filter) : tmp_filter;
        }

        #endregion

        var (Pagination, Entities) = await _unitOfWork.ServiceRepository.ToPagination(
             pageIndex: query.Page ?? 0,
            pageSize: query.Limit ?? 10,
            filter: filter,
            searchTerm: query.Q,
            searchFields: ["Name", "Description"],
            sortOrders: query.OrderBy?.ToDictionary(
                    k => k.OrderColumn ?? "CreatedAt",
                    v => (v.OrderDir ?? "ASC").Equals("ASC", StringComparison.CurrentCultureIgnoreCase)
                ) ?? new Dictionary<string, bool> { { "CreatedAt", false } },
            includeFunc: x => x.Include(x => x.Slots.Where(x => !x.IsDeleted)).ThenInclude(x => x.PetGroup)
        );
        return BasePagingResponseModel<Service>.CreateInstance(Entities, Pagination);
    }
}
