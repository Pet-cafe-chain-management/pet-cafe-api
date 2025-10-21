using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PetCafe.Application.GlobalExceptionHandling.Exceptions;
using PetCafe.Application.Models.ServiceModels;
using PetCafe.Application.Models.ShareModels;
using PetCafe.Application.Services.Commons;
using PetCafe.Application.Utilities;
using PetCafe.Domain.Constants;
using PetCafe.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace PetCafe.Application.Services;


public interface IServService
{

    Task<Service> GetByIdAsync(Guid id);
    Task<BasePagingResponseModel<Service>> GetAllPagingAsync(ServiceFilterQuery query);
    Task<Service> CreateAsync(ServiceCreateModel model);
    Task<Service> UpdateAsync(Guid id, ServiceUpdateModel model);
    Task<bool> DeleteAsync(Guid id);

    Task<bool> AssignPetGroupToService(Guid id, ServicePetGroupCreateModel model);
    Task<bool> RemovePetGroupFromService(Guid serviceId, Guid petGroupId);
    Task<BasePagingResponseModel<PetGroup>> GetPetGroupsByServiceId(Guid serviceId, FilterQuery query);
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

        Expression<Func<Service, bool>>? filter = x => x.IsActive == true;

        if (_claimsService.GetCurrentUserRole != RoleConstants.CUSTOMER)
        {
            filter = x => x.IsActive == query.IsActive;
        }

        // if (_claimsService.GetCurrentUserRole != RoleConstants.MANAGER)
        // {
        //     Expression<Func<Service, bool>> tmp_filter = x => x.Slots.Any(slot =>
        //          slot.IsActive &&
        //          slot.Status == SlotStatusConstant.AVAILABLE &&
        //          slot.AvailableCapacity > 0);
        //     filter = filter != null ? FilterCustoms.CombineFilters(filter, tmp_filter) : tmp_filter;
        // }

        if (query.SearchDate.HasValue)
        {
            var dayOfWeek = query.SearchDate.Value.DayOfWeek.ToString().ToUpper();

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


        if (query.WorkTypeId != null && query.WorkTypeId != Guid.Empty)
        {
            Expression<Func<Service, bool>> additional_filter = x => x.WorkTypeId == query.WorkTypeId;
            filter = filter != null ? FilterCustoms.CombineFilters(filter, additional_filter) : additional_filter;
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
            includeFunc: x => x
                    .Include(x => x.Slots.Where(x => !x.IsDeleted)).ThenInclude(x => x.PetGroup)
                    .Include(x => x.WorkType)
        );
        return BasePagingResponseModel<Service>.CreateInstance(Entities, Pagination);
    }

    public async Task<bool> AssignPetGroupToService(Guid id, ServicePetGroupCreateModel model)
    {
        var service = await _unitOfWork.ServiceRepository.GetByIdAsync(id) ?? throw new BadRequestException("Không tìm thấy thông tin!");

        // Kiểm tra trùng lặp pet_group_id
        await ValidateDuplicatePetGroups(id, model.PetGroupIds);

        foreach (var pet_group_id in model.PetGroupIds)
        {
            await _unitOfWork.ServicePetGroupRepository.AddAsync(new ServicePetGroup
            {
                PetGroupId = pet_group_id,
                ServiceId = service.Id
            });
        }

        return await _unitOfWork.SaveChangesAsync();
    }

    private async Task ValidateDuplicatePetGroups(Guid serviceId, List<Guid> petGroupIds)
    {
        var existingPetGroups = await _unitOfWork.ServicePetGroupRepository.WhereAsync(
            filter: spg => spg.ServiceId == serviceId,
            withDeleted: false
        );

        var existingPetGroupIds = existingPetGroups.Select(spg => spg.PetGroupId).ToList();

        var duplicatePetGroupIds = petGroupIds.Where(id => existingPetGroupIds.Contains(id)).ToList();

        if (duplicatePetGroupIds.Count != 0)
        {
            var duplicatePetGroups = await _unitOfWork.PetGroupRepository.WhereAsync(
                filter: pg => duplicatePetGroupIds.Contains(pg.Id),
                withDeleted: false
            );

            var petGroupNames = duplicatePetGroups.Select(pg => pg.Name).ToList();
            string duplicateNames = string.Join(", ", petGroupNames);

            throw new BadRequestException($"Các nhóm thú cưng sau đây đã được gán cho dịch vụ này: {duplicateNames}");
        }

        foreach (var petGroupId in petGroupIds)
        {
            var petGroup = await _unitOfWork.PetGroupRepository.GetByIdAsync(petGroupId) ?? throw new BadRequestException($"Không tìm thấy nhóm thú cưng với ID: {petGroupId}");
        }
    }

    public async Task<bool> RemovePetGroupFromService(Guid serviceId, Guid petGroupId)
    {
        var servicePetGroup = await _unitOfWork.ServicePetGroupRepository.FirstOrDefaultAsync(x => x.PetGroupId == petGroupId && x.ServiceId == serviceId) ?? throw new BadRequestException($"Nhóm thú cưng này không tồn tại");
        _unitOfWork.ServicePetGroupRepository.SoftRemove(servicePetGroup);
        return await _unitOfWork.SaveChangesAsync();
    }

    public async Task<BasePagingResponseModel<PetGroup>> GetPetGroupsByServiceId(Guid serviceId, FilterQuery query)
    {
        var existingPetGroups = await _unitOfWork.ServicePetGroupRepository.WhereAsync(
           filter: spg => spg.ServiceId == serviceId,
           withDeleted: false
       );
        var petGroupIds = existingPetGroups.Select(spg => spg.PetGroupId).ToList();


        var (Pagination, Entities) = await _unitOfWork.PetGroupRepository.ToPagination(
             pageIndex: query.Page ?? 0,
            pageSize: query.Limit ?? 10,
            filter: x => petGroupIds.Contains(x.Id),
            searchTerm: query.Q,
            searchFields: ["Name", "Description"],
            sortOrders: query.OrderBy?.ToDictionary(
                    k => k.OrderColumn ?? "CreatedAt",
                    v => (v.OrderDir ?? "ASC").Equals("ASC", StringComparison.CurrentCultureIgnoreCase)
                ) ?? new Dictionary<string, bool> { { "CreatedAt", false } }
        );
        return BasePagingResponseModel<PetGroup>.CreateInstance(Entities, Pagination);
    }
}
