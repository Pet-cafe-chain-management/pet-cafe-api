using Microsoft.EntityFrameworkCore;
using PetCafe.Application.GlobalExceptionHandling.Exceptions;
using PetCafe.Application.Models.ServiceModels;
using PetCafe.Application.Models.ShareModels;
using PetCafe.Domain.Entities;

namespace PetCafe.Application.Services;


public interface IServService
{

    Task<Service> GetByIdAsync(Guid id);
    Task<BasePagingResponseModel<Service>> GetAllPagingAsync(FilterQuery query);
    Task<Service> CreateAsync(ServiceCreateModel model);
    Task<Service> UpdateAsync(Guid id, ServiceUpdateModel model);
    Task<bool> DeleteAsync(Guid id);
}

public class ServService(IUnitOfWork _unitOfWork) : IServService
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
        return await _unitOfWork.ServiceRepository.GetByIdAsync(id) ?? throw new BadRequestException("Không tìm thấy thông tin!");
    }

    public async Task<BasePagingResponseModel<Service>> GetAllPagingAsync(FilterQuery query)
    {

        var (Pagination, Entities) = await _unitOfWork.ServiceRepository.ToPagination(
             pageIndex: query.Page ?? 0,
            pageSize: query.Limit ?? 10,
            searchTerm: query.Q,
            searchFields: ["Name", "Description"],
            sortOrders: query.OrderBy?.ToDictionary(
                    k => k.OrderColumn ?? "CreatedAt",
                    v => (v.OrderDir ?? "ASC").Equals("ASC", StringComparison.CurrentCultureIgnoreCase)
                ) ?? new Dictionary<string, bool> { { "CreatedAt", false } },
            includeFunc: x => x.Include(x => x.Slots.Where(x => !x.IsDeleted))
        );
        return BasePagingResponseModel<Service>.CreateInstance(Entities, Pagination);
    }
}
