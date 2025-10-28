using Microsoft.EntityFrameworkCore;
using PetCafe.Application.GlobalExceptionHandling.Exceptions;
using PetCafe.Application.Models.ShareModels;
using PetCafe.Application.Models.SlotModels;
using PetCafe.Domain.Constants;
using PetCafe.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace PetCafe.Application.Services;

public interface ISlotService
{
    Task<Slot> GetByIdAsync(Guid id);
    Task<BasePagingResponseModel<Slot>> GetAllPagingByServiceAsync(Guid serviceId, FilterQuery query);
    Task<Slot> CreateAsync(SlotCreateModel model);
    Task<Slot> UpdateAsync(Guid id, SlotUpdateModel model);
    Task<bool> DeleteAsync(Guid id);
}

public class SlotService(IUnitOfWork _unitOfWork, IDailyTaskService _dailyTaskService) : ISlotService
{
    public async Task<Slot> CreateAsync(SlotCreateModel model)
    {
        var task = await _unitOfWork.TaskRepository.GetByIdAsync(model.TaskId) ?? throw new BadRequestException("Không tìm thấy thông tin công việc!");
        await ValidateSlot(task, model);
        var slot = _unitOfWork.Mapper.Map<Slot>(model);
        await CheckDuplicateSlot(model);
        if (task.ServiceId != null)
        {
            slot.ServiceId = task.ServiceId;
            slot.ServiceStatus = SlotStatusConstant.UNAVAILABLE;
        }
        await _unitOfWork.SlotRepository.AddAsync(slot);
        await _unitOfWork.SaveChangesAsync();

        // Tạo DailyTasks cho các ngày còn lại trong tuần nếu task là recurring
        if (task.IsRecurring)
        {
            var today = DateTime.UtcNow.Date;
            var currentDayOfWeek = today.DayOfWeek;

            // Tính số ngày còn lại từ hôm nay đến Chủ nhật
            var daysUntilSunday = 7 - (int)currentDayOfWeek;

            // Tạo danh sách các ngày còn lại trong tuần (từ ngày mai đến Chủ nhật)
            var remainingDates = new List<DateTime>();
            for (int i = 1; i <= daysUntilSunday; i++)
            {
                remainingDates.Add(today.AddDays(i));
            }

            // Sử dụng method dùng chung để tạo DailyTasks
            if (remainingDates.Count > 0)
            {
                await _dailyTaskService.CreateDailyTasksFromSlotAsync(slot, task, remainingDates);
            }
        }

        return slot;
    }

    public async Task<Slot> UpdateAsync(Guid id, SlotUpdateModel model)
    {
        var task = await _unitOfWork.TaskRepository.GetByIdAsync(model.TaskId) ?? throw new BadRequestException("Không tìm thấy thông tin công việc!");
        await ValidateSlot(task, model);
        var slot = await _unitOfWork.SlotRepository.GetByIdAsync(id) ?? throw new BadRequestException("Không tìm thấy thông tin!");
        await CheckDuplicateSlot(model, slot.Id);
        _unitOfWork.Mapper.Map(model, slot);
        _unitOfWork.SlotRepository.Update(slot);
        await _unitOfWork.SaveChangesAsync();
        return slot;
    }

    public async Task ValidateSlot(Domain.Entities.Task task, SlotCreateModel model)
    {

        var area = await _unitOfWork
            .AreaWorkTypeRepository
            .FirstOrDefaultAsync(x =>
                x.WorkTypeId == task.WorkTypeId &&
                x.AreaId == model.AreaId
        ) ?? throw new BadRequestException("Khu vực không cùng chung công việc!");

        var team = await _unitOfWork
            .TeamWorkTypeRepository
            .FirstOrDefaultAsync(x =>
                x.WorkTypeId == task.WorkTypeId &&
                x.TeamId == model.TeamId
            ) ?? throw new BadRequestException("Nhóm không cùng chung công việc!");

        var team_work_shifts = await _unitOfWork
            .TeamWorkShiftRepository
            .WhereAsync(
                x => x.TeamId == model.TeamId,
                includeFunc: x => x.Include(x => x.WorkShift)
            );

        var validTeamWorkShift = team_work_shifts.FirstOrDefault(x =>
            x.WorkShift.ApplicableDays != null &&
            x.WorkShift.ApplicableDays.Contains(model.DayOfWeek) &&
            model.StartTime >= x.WorkShift.StartTime &&
            model.EndTime <= x.WorkShift.EndTime)
            ?? throw new BadRequestException("Nhóm không hoạt động trong khoảng thời gian này!");
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var slot = await _unitOfWork.SlotRepository.GetByIdAsync(id) ?? throw new BadRequestException("Không tìm thấy thông tin!");
        _unitOfWork.SlotRepository.SoftRemove(slot);
        return await _unitOfWork.SaveChangesAsync();
    }

    public async Task<Slot> GetByIdAsync(Guid id)
    {
        return await _unitOfWork.SlotRepository.GetByIdAsync(id,
            includeFunc: x => x
                .Include(x => x.Area)
                .Include(x => x.Service)
                .Include(x => x.PetGroup).ThenInclude(x => x!.Pets.Where(x => !x.IsDeleted))
                .Include(x => x.PetGroup).ThenInclude(x => x!.PetSpecies!)
                .Include(x => x.PetGroup).ThenInclude(x => x!.PetBreed!)
        ) ?? throw new BadRequestException("Không tìm thấy thông tin!");
    }

    public async Task<BasePagingResponseModel<Slot>> GetAllPagingByServiceAsync(Guid serviceId, FilterQuery query)
    {
        var (Pagination, Entities) = await _unitOfWork.SlotRepository.ToPagination(
             pageIndex: query.Page ?? 0,
            pageSize: query.Limit ?? 10,
            filter: x => x.ServiceId == serviceId,
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

    private async Task CheckDuplicateSlot(SlotCreateModel model, Guid? excludeId = null)
    {
        var existingSlot = await _unitOfWork
            .SlotRepository
            .FirstOrDefaultAsync(x =>
                (excludeId == null || x.Id != excludeId) &&
                (x.AreaId == model.AreaId || x.TeamId == model.TeamId || x.PetGroupId == model.PetGroupId) &&
                x.DayOfWeek == model.DayOfWeek &&
                (
                    (model.StartTime >= x.StartTime && model.StartTime < x.EndTime) ||
                    (model.EndTime > x.StartTime && model.EndTime <= x.EndTime) ||
                    (model.StartTime <= x.StartTime && model.EndTime >= x.EndTime)
                )
            );

        if (existingSlot != null)
            throw new BadRequestException("Thông tin slot trùng với thông tin hiện có!");
    }
}
