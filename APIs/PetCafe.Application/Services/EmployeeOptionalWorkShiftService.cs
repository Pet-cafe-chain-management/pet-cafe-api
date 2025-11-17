using Microsoft.EntityFrameworkCore;
using PetCafe.Application.GlobalExceptionHandling.Exceptions;
using PetCafe.Application.Models.EmployeeOptionalWorkShiftModels;
using PetCafe.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace PetCafe.Application.Services;

public interface IEmployeeOptionalWorkShiftService
{
    Task<EmployeeOptionalWorkShift> AddAsync(EmployeeOptionalWorkShiftCreateModel model);
    Task<EmployeeOptionalWorkShift> UpdateAsync(Guid id, EmployeeOptionalWorkShiftUpdateModel model);
    Task<bool> RemoveAsync(Guid id);
    Task<List<EmployeeOptionalWorkShift>> GetByEmployeeIdAsync(Guid employeeId);
    Task<bool> ValidateEmployeeHasOptionalShiftsAsync(Guid employeeId);
}

public class EmployeeOptionalWorkShiftService(
    IUnitOfWork _unitOfWork
) : IEmployeeOptionalWorkShiftService
{
    public async Task<EmployeeOptionalWorkShift> AddAsync(EmployeeOptionalWorkShiftCreateModel model)
    {
        // Validate employee exists
        var employee = await _unitOfWork.EmployeeRepository.GetByIdAsync(model.EmployeeId)
            ?? throw new BadRequestException("Không tìm thấy nhân viên!");

        if (!employee.IsActive)
        {
            throw new BadRequestException("Nhân viên hiện không đang hoạt động!");
        }

        // Validate work shift exists
        var workShift = await _unitOfWork.WorkShiftRepository.GetByIdAsync(model.WorkShiftId)
            ?? throw new BadRequestException("Không tìm thấy ca làm việc!");

        // Check if already exists
        var existing = await _unitOfWork.EmployeeOptionalWorkShiftRepository.FirstOrDefaultAsync(
            eows => eows.EmployeeId == model.EmployeeId
                && eows.WorkShiftId == model.WorkShiftId
        );

        if (existing != null)
        {
            throw new BadRequestException("Nhân viên đã có ca làm việc linh hoạt này!");
        }

        var optionalWorkShift = _unitOfWork.Mapper.Map<EmployeeOptionalWorkShift>(model);
        await _unitOfWork.EmployeeOptionalWorkShiftRepository.AddAsync(optionalWorkShift);
        await _unitOfWork.SaveChangesAsync();

        return optionalWorkShift;
    }

    public async Task<EmployeeOptionalWorkShift> UpdateAsync(Guid id, EmployeeOptionalWorkShiftUpdateModel model)
    {
        var optionalWorkShift = await _unitOfWork.EmployeeOptionalWorkShiftRepository.GetByIdAsync(id)
            ?? throw new NotFoundException("Không tìm thấy ca làm việc linh hoạt!");

        _unitOfWork.Mapper.Map(model, optionalWorkShift);
        _unitOfWork.EmployeeOptionalWorkShiftRepository.Update(optionalWorkShift);
        await _unitOfWork.SaveChangesAsync();

        return optionalWorkShift;
    }

    public async Task<bool> RemoveAsync(Guid id)
    {
        var optionalWorkShift = await _unitOfWork.EmployeeOptionalWorkShiftRepository.GetByIdAsync(id)
            ?? throw new NotFoundException("Không tìm thấy ca làm việc linh hoạt!");

        _unitOfWork.EmployeeOptionalWorkShiftRepository.SoftRemove(optionalWorkShift);
        return await _unitOfWork.SaveChangesAsync();
    }

    public async Task<List<EmployeeOptionalWorkShift>> GetByEmployeeIdAsync(Guid employeeId)
    {
        return await _unitOfWork.EmployeeOptionalWorkShiftRepository.WhereAsync(
            eows => eows.EmployeeId == employeeId,
            includeFunc: q => q
                .Include(eows => eows.WorkShift)
                .Include(eows => eows.Employee)
        );
    }

    public async Task<bool> ValidateEmployeeHasOptionalShiftsAsync(Guid employeeId)
    {
        var optionalShifts = await _unitOfWork.EmployeeOptionalWorkShiftRepository.WhereAsync(
            eows => eows.EmployeeId == employeeId && eows.IsAvailable
        );

        return optionalShifts.Count > 0;
    }
}

