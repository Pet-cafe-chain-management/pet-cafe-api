using PetCafe.Application.GlobalExceptionHandling.Exceptions;
using PetCafe.Application.Models.ShareModels;
using PetCafe.Application.Models.WorkShiftModels;
using PetCafe.Domain.Entities;

namespace PetCafe.Application.Services;

public interface IWorkShiftService
{
    Task<WorkShift> GetByIdAsync(Guid id);
    Task<BasePagingResponseModel<WorkShift>> GetAllPagingAsync(FilterQuery query);
    Task<WorkShift> CreateAsync(WorkShiftCreateModel model);
    Task<WorkShift> UpdateAsync(Guid id, WorkShiftUpdateModel model);
    Task<bool> DeleteAsync(Guid id);
}

public class WorkShiftService(
    IUnitOfWork _unitOfWork
) : IWorkShiftService
{
    public async Task<WorkShift> CreateAsync(WorkShiftCreateModel model)
    {
        var workShift = _unitOfWork.Mapper.Map<WorkShift>(model);
        await _unitOfWork.WorkShiftRepository.AddAsync(workShift);
        await _unitOfWork.SaveChangesAsync();
        return workShift;
    }

    public async Task<WorkShift> UpdateAsync(Guid id, WorkShiftUpdateModel model)
    {
        var workShift = await _unitOfWork.WorkShiftRepository.GetByIdAsync(id) ?? throw new BadRequestException("Không tìm thấy thông tin!");
        _unitOfWork.Mapper.Map(model, workShift);
        _unitOfWork.WorkShiftRepository.Update(workShift);
        await _unitOfWork.SaveChangesAsync();
        return workShift;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var workShift = await _unitOfWork.WorkShiftRepository.GetByIdAsync(id) ?? throw new BadRequestException("Không tìm thấy thông tin!");
        _unitOfWork.WorkShiftRepository.SoftRemove(workShift);
        return await _unitOfWork.SaveChangesAsync();
    }

    public async Task<WorkShift> GetByIdAsync(Guid id)
    {
        return await _unitOfWork.WorkShiftRepository.GetByIdAsync(id) ?? throw new BadRequestException("Không tìm thấy thông tin!");
    }

    public async Task<BasePagingResponseModel<WorkShift>> GetAllPagingAsync(FilterQuery query)
    {

        var (Pagination, Entities) = await _unitOfWork.WorkShiftRepository.ToPagination(
            pageIndex: query.Page ?? 0,
           pageSize: query.Limit ?? 10,
           searchTerm: query.Q,
           searchFields: ["Name", "Description"],
           sortOrders: query.OrderBy?.ToDictionary(
                   k => k.OrderColumn ?? "CreatedAt",
                   v => (v.OrderDir ?? "ASC").Equals("ASC", StringComparison.CurrentCultureIgnoreCase)
               ) ?? new Dictionary<string, bool> { { "CreatedAt", false } }
            );
        return BasePagingResponseModel<WorkShift>.CreateInstance(Entities, Pagination);
    }
}