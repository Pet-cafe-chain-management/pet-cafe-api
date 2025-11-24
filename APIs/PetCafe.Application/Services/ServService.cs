using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PetCafe.Application.GlobalExceptionHandling.Exceptions;
using PetCafe.Application.Models.ServiceModels;
using PetCafe.Application.Models.ShareModels;
using PetCafe.Application.Models.SlotModels;
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

    Task<BasePagingResponseModel<Slot>> GetSlotsByServiceIdAsync(Guid serviceId, SlotFilterQuery query);

}

public class ServService(IUnitOfWork _unitOfWork, IClaimsService _claimsService) : IServService
{
    public async Task<Service> CreateAsync(ServiceCreateModel model)
    {

        var task = await _unitOfWork.TaskRepository.FirstOrDefaultAsync(x => x.Id == model.TaskId && x.IsPublic) ?? throw new BadRequestException("Không tìm thấy thông tin công việc!");
        var serviceExist = await _unitOfWork.ServiceRepository.FirstOrDefaultAsync(x => x.TaskId == model.TaskId);
        if (serviceExist != null) throw new BadRequestException("Dịch vụ cho công việc này đã tồn tại!");
        var service = _unitOfWork.Mapper.Map<Service>(model);
        task.ServiceId = service.Id;
        _unitOfWork.TaskRepository.Update(task);
        await _unitOfWork.ServiceRepository.AddAsync(service);

        var slots = await _unitOfWork.SlotRepository.WhereAsync(x => x.TaskId == model.TaskId);
        if (slots.Count > 0)
        {
            foreach (var slot in slots)
            {
                slot.ServiceId = service.Id;
                slot.ServiceStatus = SlotStatusConstant.UNAVAILABLE;
                _unitOfWork.SlotRepository.Update(slot);
            }
        }
        await _unitOfWork.SaveChangesAsync();
        return service;
    }

    public async Task<Service> UpdateAsync(Guid id, ServiceUpdateModel model)
    {
        var task = await _unitOfWork.TaskRepository.FirstOrDefaultAsync(x => x.Id == model.TaskId && x.IsPublic) ?? throw new BadRequestException("Không tìm thấy thông tin công việc!");
        var service = await _unitOfWork.ServiceRepository.GetByIdAsync(id) ?? throw new BadRequestException("Không tìm thấy thông tin!");
        if (model.IsActive)
        {
            var slots = await _unitOfWork.SlotRepository.WhereAsync(x => x.ServiceId == service.Id && x.ServiceStatus == SlotStatusConstant.AVAILABLE);
            if (slots.Count <= 0) throw new BadRequestException("Không có khung giờ nào được kích hoạt!");
        }
        _unitOfWork.Mapper.Map(model, service);
        task.ServiceId = service.Id;
        _unitOfWork.TaskRepository.Update(task);
        _unitOfWork.ServiceRepository.Update(service);
        await _unitOfWork.SaveChangesAsync();
        return service;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var service = await _unitOfWork.ServiceRepository.GetByIdAsync(id) ?? throw new BadRequestException("Không tìm thấy thông tin!");
        var task = await _unitOfWork.TaskRepository.GetByIdAsync(service.TaskId) ?? throw new BadRequestException("Không tìm thấy thông tin!");
        task.ServiceId = null;
        _unitOfWork.TaskRepository.Update(task);
        _unitOfWork.ServiceRepository.SoftRemove(service);
        return await _unitOfWork.SaveChangesAsync();
    }

    public async Task<Service> GetByIdAsync(Guid id)
    {
        return await _unitOfWork.ServiceRepository.GetByIdAsync(id,
            includeFunc: x => x.Include(x => x.Slots.Where(x => !x.IsDeleted)).ThenInclude(x => x.PetGroup!)
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


        if (query.SearchDate.HasValue)
        {
            var dayOfWeek = query.SearchDate.Value.DayOfWeek.ToString().ToUpper();

            Expression<Func<Service, bool>> tmp_filter = x => x.Slots.Any(slot => slot.DayOfWeek == dayOfWeek);
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
               query.PetBreedIds.Contains(slot.PetGroup!.PetBreedId!.Value));
            filter = filter != null ? FilterCustoms.CombineFilters(filter, tmp_filter) : tmp_filter;
        }

        if (query.PetSpeciesIds != null && query.PetSpeciesIds.Count > 0)
        {
            Expression<Func<Service, bool>> tmp_filter = x => x.Slots.Any(slot =>
               query.PetSpeciesIds.Contains(slot.PetGroup!.PetSpeciesId!.Value));
            filter = filter != null ? FilterCustoms.CombineFilters(filter, tmp_filter) : tmp_filter;
        }

        if (query.AreaIds != null && query.AreaIds.Count > 0)
        {
            Expression<Func<Service, bool>> tmp_filter = x => x.Slots.Any(slot =>
               query.AreaIds.Contains(slot.AreaId));
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
            includeFunc: x => x
                    .Include(x => x.Slots.Where(x => !x.IsDeleted)).ThenInclude(x => x.PetGroup)
                    .Include(x => x.Task)
        );
        return BasePagingResponseModel<Service>.CreateInstance(Entities, Pagination);
    }

    public async Task<BasePagingResponseModel<Slot>> GetSlotsByServiceIdAsync(Guid serviceId, SlotFilterQuery query)
    {
        Expression<Func<Slot, bool>> filter = x => x.ServiceId == serviceId;
        if (query.DayOfWeek != null)
        {
            Expression<Func<Slot, bool>> tmp_filter = x => x.DayOfWeek == query.DayOfWeek;
            filter = filter != null ? FilterCustoms.CombineFilters(filter, tmp_filter) : tmp_filter;
        }
        if (query.StartTime != null)
        {
            Expression<Func<Slot, bool>> tmp_filter = x => x.StartTime >= query.StartTime;
            filter = filter != null ? FilterCustoms.CombineFilters(filter, tmp_filter) : tmp_filter;
        }
        if (query.EndTime != null)
        {
            Expression<Func<Slot, bool>> tmp_filter = x => x.EndTime <= query.EndTime;
            filter = filter != null ? FilterCustoms.CombineFilters(filter, tmp_filter) : tmp_filter;
        }
        var (Pagination, Entities) = await _unitOfWork.SlotRepository.ToPagination(
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
                .Include(x => x.Area)
                .Include(x => x.Service)
                .Include(x => x.PetGroup).ThenInclude(x => x!.PetSpecies!)
                .Include(x => x.PetGroup).ThenInclude(x => x!.PetBreed!)
                .Include(x => x.Team)
        );
        return BasePagingResponseModel<Slot>.CreateInstance(Entities, Pagination);
    }
}