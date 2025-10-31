using Hangfire;
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

public class SlotService(
    IUnitOfWork _unitOfWork,
    IDailyTaskService _dailyTaskService,
    IBackgroundJobClient _backgroundJobClient) : ISlotService
{
    [AutomaticRetry(Attempts = 0)]
    public async Task CreateDailyTasksFromSlotAsync(Slot slot, Domain.Entities.Task task)
    {
        if (slot.IsRecurring)
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
        else
        {
            await _dailyTaskService.CreateDailyTasksFromSpecificDateAsync(slot, task);
        }
    }

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
        _backgroundJobClient.Enqueue(() => CreateDailyTasksFromSlotAsync(slot, task));

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
        if (model.IsUpdateRelatedData)
        {
            _backgroundJobClient.Enqueue(() => UpdateDailyTasksFromSlotAsync(slot));
        }
        return slot;
    }

    public async Task UpdateDailyTasksFromSlotAsync(Slot slot)
    {
        var dailyTasks = await _unitOfWork.DailyTaskRepository.WhereAsync(x =>
            x.SlotId == slot.Id &&
            x.Status == DailyTaskStatusConstant.SCHEDULED
        );
        if (dailyTasks.Count == 0) return;

        foreach (var dailyTask in dailyTasks)
        {
            if (slot.IsRecurring && slot.DayOfWeek != null &&
                !slot.DayOfWeek.Equals(dailyTask.AssignedDate.DayOfWeek.ToString(), StringComparison.CurrentCultureIgnoreCase))
            {
                // Tính ngày kế tiếp gần nhất với ngày hiện tại khớp với DayOfWeek của slot
                var today = DateTime.UtcNow.Date;
                var targetDayOfWeek = Enum.Parse<DayOfWeek>(slot.DayOfWeek, true);
                var daysUntilTarget = ((int)targetDayOfWeek - (int)today.DayOfWeek + 7) % 7;

                // Nếu cùng ngày, lấy ngày kế tiếp (tuần sau)
                if (daysUntilTarget == 0)
                {
                    daysUntilTarget = 7;
                }

                dailyTask.AssignedDate = today.AddDays(daysUntilTarget);
            }
            else
            {
                dailyTask.AssignedDate = slot.SpecificDate!.Value;
            }
            dailyTask.StartTime = slot.StartTime;
            dailyTask.EndTime = slot.EndTime;
            dailyTask.TaskId = slot.TaskId;
            dailyTask.SlotId = slot.Id;
            dailyTask.TeamId = slot.TeamId;
            dailyTask.Status = DailyTaskStatusConstant.SCHEDULED;
            dailyTask.Title = slot.Task.Title;
            dailyTask.Description = slot.Task.Description;
        }

        _unitOfWork.DailyTaskRepository.UpdateRange(dailyTasks);
        await _unitOfWork.SaveChangesAsync();
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
            model.StartTime >= x.WorkShift.StartTime &&
            model.EndTime <= x.WorkShift.EndTime)
            ?? throw new BadRequestException("Nhóm không hoạt động trong khoảng thời gian này!");

        if (model.IsRecurring && model.DayOfWeek != null && validTeamWorkShift.WorkShift.ApplicableDays.Contains(model.DayOfWeek))
        {
            throw new BadRequestException("Ngày trong tuần không cùng chung ca làm việc!");
        }

        if (!model.IsRecurring && model.SpecificDate != null && validTeamWorkShift.WorkShift.ApplicableDays.Contains(model.SpecificDate.Value.DayOfWeek.ToString()))
        {
            throw new BadRequestException("Ngày trong tuần không cùng chung ca làm việc!");
        }

    }

    public async Task<bool> DeleteAsync(Guid id)
    {

        var slot = await _unitOfWork.SlotRepository.GetByIdAsync(id) ?? throw new BadRequestException("Không tìm thấy thông tin!");
        var dailyTasks = await _unitOfWork.DailyTaskRepository.WhereAsync(x => x.SlotId == id && x.Status == DailyTaskStatusConstant.SCHEDULED);
        _unitOfWork.DailyTaskRepository.SoftRemoveRange(dailyTasks);
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
