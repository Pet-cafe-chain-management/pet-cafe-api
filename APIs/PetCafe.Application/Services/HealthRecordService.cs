using Microsoft.EntityFrameworkCore;
using PetCafe.Application.GlobalExceptionHandling.Exceptions;
using PetCafe.Application.Models.HealthRecordModels;
using PetCafe.Application.Models.ShareModels;
using PetCafe.Domain.Entities;

namespace PetCafe.Application.Services;

public interface IHealthRecordService
{
    Task<HealthRecord> GetByIdAsync(Guid id);
    Task<BasePagingResponseModel<HealthRecord>> GetAllPagingAsync(Guid petId, FilterQuery query);
    Task<HealthRecord> CreateAsync(HealthRecordCreateModel model);
    Task<HealthRecord> UpdateAsync(Guid id, HealthRecordUpdateModel model);
    Task<bool> DeleteAsync(Guid id);
}

public class HealthRecordService(
    IUnitOfWork _unitOfWork
) : IHealthRecordService
{
    public async Task<HealthRecord> CreateAsync(HealthRecordCreateModel model)
    {
        var healthRecord = _unitOfWork.Mapper.Map<HealthRecord>(model);
        await _unitOfWork.HealthRecordRepository.AddAsync(healthRecord);
        await _unitOfWork.SaveChangesAsync();
        return healthRecord;
    }

    public async Task<HealthRecord> UpdateAsync(Guid id, HealthRecordUpdateModel model)
    {
        var healthRecord = await _unitOfWork.HealthRecordRepository.GetByIdAsync(id) ?? throw new BadRequestException("Không tìm thấy thông tin!");
        _unitOfWork.Mapper.Map(model, healthRecord);
        _unitOfWork.HealthRecordRepository.Update(healthRecord);
        await _unitOfWork.SaveChangesAsync();
        return healthRecord;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var healthRecord = await _unitOfWork.HealthRecordRepository.GetByIdAsync(id) ?? throw new BadRequestException("Không tìm thấy thông tin!");
        _unitOfWork.HealthRecordRepository.SoftRemove(healthRecord);
        return await _unitOfWork.SaveChangesAsync();
    }

    public async Task<HealthRecord> GetByIdAsync(Guid id)
    {
        return await _unitOfWork.HealthRecordRepository.GetByIdAsync(id, includeFunc: x => x.Include(x => x.Pet)) ?? throw new BadRequestException("Không tìm thấy thông tin!");
    }

    public async Task<BasePagingResponseModel<HealthRecord>> GetAllPagingAsync(Guid petId, FilterQuery query)
    {

        var (Pagination, Entities) = await _unitOfWork.HealthRecordRepository.ToPagination(
             pageIndex: query.Page ?? 0,
            pageSize: query.Limit ?? 10,
            filter: x => x.PetId == petId,
            searchTerm: query.Q,
            searchFields: ["Name", "Description"],
            sortOrders: query.OrderBy?.ToDictionary(
                    k => k.OrderColumn ?? "CreatedAt",
                    v => (v.OrderDir ?? "ASC").Equals("ASC", StringComparison.CurrentCultureIgnoreCase)
                ) ?? new Dictionary<string, bool> { { "CreatedAt", false } },
            includeFunc: x => x.Include(x => x.Pet)
        );
        return BasePagingResponseModel<HealthRecord>.CreateInstance(Entities, Pagination);
    }
}