using PetCafe.Application.GlobalExceptionHandling.Exceptions;
using PetCafe.Application.Models.ShareModels;
using PetCafe.Application.Models.WorkShiftModels;
using PetCafe.Domain.Entities;
using Task = System.Threading.Tasks.Task;

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
        await CheckDuplicateWorkShift(model.StartTime, model.EndTime, model.ApplicableDays);
        var workShift = _unitOfWork.Mapper.Map<WorkShift>(model);
        await _unitOfWork.WorkShiftRepository.AddAsync(workShift);
        await _unitOfWork.SaveChangesAsync();
        return workShift;
    }

    public async Task<WorkShift> UpdateAsync(Guid id, WorkShiftUpdateModel model)
    {
        await CheckDuplicateWorkShift(model.StartTime, model.EndTime, model.ApplicableDays);
        var workShift = await _unitOfWork.WorkShiftRepository.GetByIdAsync(id) ?? throw new BadRequestException("Không tìm thấy thông tin!");
        _unitOfWork.Mapper.Map(model, workShift);
        _unitOfWork.WorkShiftRepository.Update(workShift);
        await _unitOfWork.SaveChangesAsync();
        return workShift;
    }
    private async Task CheckDuplicateWorkShift(TimeSpan startTime, TimeSpan endTime, List<string> applicableDays, Guid? excludeId = null)
    {
        var existingWorkShifts = await _unitOfWork.WorkShiftRepository.WhereAsync(
            filter: ws => excludeId == null || ws.Id != excludeId,
            withDeleted: false
        );

        foreach (var existingShift in existingWorkShifts)
        {
            bool hasOverlappingDays = existingShift.ApplicableDays.Intersect(applicableDays).Any();

            if (hasOverlappingDays)
            {
                bool timeOverlap =
                    (startTime >= existingShift.StartTime && startTime < existingShift.EndTime) ||
                    (endTime > existingShift.StartTime && endTime <= existingShift.EndTime) ||
                    (startTime <= existingShift.StartTime && endTime >= existingShift.EndTime);

                if (timeOverlap)
                {
                    string overlappingDays = string.Join(", ", existingShift.ApplicableDays.Intersect(applicableDays));
                    throw new BadRequestException(
                        $"Ca làm việc bị trùng thời gian ({existingShift.StartTime.ToString(@"hh\:mm")} - {existingShift.EndTime.ToString(@"hh\:mm")}) " +
                        $"vào các ngày: {overlappingDays} với ca làm việc '{existingShift.Name}'");
                }
            }
        }
    }
    public async Task<bool> DeleteAsync(Guid id)
    {
        var workShift = await _unitOfWork.WorkShiftRepository.GetByIdAsync(id) ?? throw new BadRequestException("Không tìm thấy thông tin!");

        // Xóa DailySchedule liên quan tới work shift này
        var dailySchedules = await _unitOfWork.DailyScheduleRepository.WhereAsync(
            ds => ds.WorkShiftId == id
        );
        if (dailySchedules.Count > 0)
        {
            _unitOfWork.DailyScheduleRepository.SoftRemoveRange(dailySchedules);
        }

        // Xóa TeamWorkShift liên quan tới work shift này
        var teamWorkShifts = await _unitOfWork.TeamWorkShiftRepository.WhereAsync(
            tws => tws.WorkShiftId == id
        );

        if (teamWorkShifts.Count > 0)
            _unitOfWork.TeamWorkShiftRepository.SoftRemoveRange(teamWorkShifts);

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