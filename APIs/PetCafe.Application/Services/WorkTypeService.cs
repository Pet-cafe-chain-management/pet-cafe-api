using PetCafe.Application.GlobalExceptionHandling.Exceptions;
using PetCafe.Application.Models.ShareModels;
using PetCafe.Application.Models.WorkTypeModels;
using PetCafe.Domain.Entities;

namespace PetCafe.Application.Services;

public interface IWorkTypeService
{
    Task<WorkType> GetByIdAsync(Guid id);
    Task<BasePagingResponseModel<WorkType>> GetAllPagingAsync(FilterQuery query);
    Task<WorkType> CreateAsync(WorkTypeCreateModel model);
    Task<WorkType> UpdateAsync(Guid id, WorkTypeUpdateModel model);
    Task<bool> DeleteAsync(Guid id);
}

public class WorkTypeService(
    IUnitOfWork _unitOfWork
) : IWorkTypeService
{
    public async Task<WorkType> CreateAsync(WorkTypeCreateModel model)
    {
        var work_type = _unitOfWork.Mapper.Map<WorkType>(model);
        await _unitOfWork.WorkTypeRepository.AddAsync(work_type);
        await _unitOfWork.SaveChangesAsync();
        return work_type;
    }

    public async Task<WorkType> UpdateAsync(Guid id, WorkTypeUpdateModel model)
    {
        var work_type = await _unitOfWork.WorkTypeRepository.GetByIdAsync(id) ?? throw new BadRequestException("Không tìm thấy thông tin!");
        _unitOfWork.Mapper.Map(model, work_type);
        _unitOfWork.WorkTypeRepository.Update(work_type);
        await _unitOfWork.SaveChangesAsync();
        return work_type;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var work_type = await _unitOfWork.WorkTypeRepository.GetByIdAsync(id) ?? throw new BadRequestException("Không tìm thấy thông tin!");
        _unitOfWork.WorkTypeRepository.SoftRemove(work_type);
        return await _unitOfWork.SaveChangesAsync();
    }

    public async Task<WorkType> GetByIdAsync(Guid id)
    {
        return await _unitOfWork.WorkTypeRepository.GetByIdAsync(id) ?? throw new BadRequestException("Không tìm thấy thông tin!");
    }

    public async Task<BasePagingResponseModel<WorkType>> GetAllPagingAsync(FilterQuery query)
    {

        var (Pagination, Entities) = await _unitOfWork.WorkTypeRepository.ToPagination(
            pageIndex: query.Page ?? 0,
            pageSize: query.Limit ?? 10,
            searchTerm: query.Q,
            searchFields: ["Name", "Description"],
            sortOrders: query.OrderBy?.ToDictionary(
                   k => k.OrderColumn ?? "CreatedAt",
                   v => (v.OrderDir ?? "ASC").Equals("ASC", StringComparison.CurrentCultureIgnoreCase)
               ) ?? new Dictionary<string, bool> { { "CreatedAt", false } }
        );
        return BasePagingResponseModel<WorkType>.CreateInstance(Entities, Pagination);
    }

}