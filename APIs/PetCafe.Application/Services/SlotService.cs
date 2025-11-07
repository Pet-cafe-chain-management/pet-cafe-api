using System.Linq.Expressions;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using PetCafe.Application.GlobalExceptionHandling.Exceptions;
using PetCafe.Application.Models.ShareModels;
using PetCafe.Application.Models.SlotModels;
using PetCafe.Application.Utilities;
using PetCafe.Domain.Constants;
using PetCafe.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace PetCafe.Application.Services;

public interface ISlotService
{
    Task<Slot> GetByIdAsync(Guid id, DateOnly? bookingDate);
    Task<BasePagingResponseModel<Slot>> GetAllPagingByServiceAsync(Guid serviceId, FilterQuery query, DateOnly? bookingDateTo, DateOnly? bookingDateFrom);
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

        // Lưu lại giá trị IsRecurring cũ để kiểm tra thay đổi
        var oldIsRecurring = slot.IsRecurring;
        var oldSpecificDate = slot.SpecificDate;

        _unitOfWork.Mapper.Map(model, slot);
        _unitOfWork.SlotRepository.Update(slot);
        await _unitOfWork.SaveChangesAsync();

        // Xử lý khi chuyển đổi recurring ↔ non-recurring
        var isRecurringChanged = oldIsRecurring != slot.IsRecurring;

        if (isRecurringChanged)
        {
            // Chuyển từ non-recurring → recurring: Tạo DailyTask mới cho các ngày còn lại trong tuần
            if (!oldIsRecurring && slot.IsRecurring && slot.DayOfWeek != null)
            {
                _backgroundJobClient.Enqueue(() => CreateDailyTasksFromSlotAsync(slot, task));
            }
            // Chuyển từ recurring → non-recurring: Xóa DailyTask tương lai không phù hợp
            else if (oldIsRecurring && !slot.IsRecurring && slot.SpecificDate.HasValue)
            {
                var today = DateTime.UtcNow.Date;
                var futureDailyTasks = await _unitOfWork.DailyTaskRepository.WhereAsync(
                    dt => dt.SlotId == slot.Id
                        && dt.Status == DailyTaskStatusConstant.SCHEDULED
                        && dt.AssignedDate >= today
                        && dt.AssignedDate.Date != slot.SpecificDate.Value.Date
                );

                if (futureDailyTasks.Count > 0)
                {
                    _unitOfWork.DailyTaskRepository.SoftRemoveRange(futureDailyTasks);
                    await _unitOfWork.SaveChangesAsync();
                }

                // Tạo DailyTask cho SpecificDate nếu chưa có
                var existingDailyTaskForDate = await _unitOfWork.DailyTaskRepository.FirstOrDefaultAsync(
                    dt => dt.SlotId == slot.Id
                        && dt.AssignedDate.Date == slot.SpecificDate.Value.Date
                );

                if (existingDailyTaskForDate == null)
                {
                    await _dailyTaskService.CreateDailyTasksFromSpecificDateAsync(slot, task);
                }
            }
        }

        // Xử lý update DailyTask hiện có nếu cần
        if (model.IsUpdateRelatedData)
        {
            _backgroundJobClient.Enqueue(() => UpdateDailyTasksFromSlotAsync(slot));
        }

        return slot;
    }

    public async Task UpdateDailyTasksFromSlotAsync(Slot slot)
    {
        var today = DateTime.UtcNow.Date;

        // Tính ngày đầu tuần (Thứ 2) và cuối tuần (Chủ nhật)
        var startOfWeek = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);
        if (today.DayOfWeek == DayOfWeek.Sunday)
        {
            startOfWeek = startOfWeek.AddDays(7);
        }
        var endOfWeek = startOfWeek.AddDays(6);

        // Chỉ lấy các DailyTask trong tuần hiện tại (từ Thứ 2 đến Chủ nhật)
        var dailyTasks = await _unitOfWork.DailyTaskRepository.WhereAsync(x =>
            x.SlotId == slot.Id &&
            x.Status == DailyTaskStatusConstant.SCHEDULED &&
            x.AssignedDate >= startOfWeek &&
            x.AssignedDate <= endOfWeek
        );
        if (dailyTasks.Count == 0) return;

        // Load Task để có thông tin cập nhật
        var task = await _unitOfWork.TaskRepository.GetByIdAsync(slot.TaskId);
        if (task == null) return;

        foreach (var dailyTask in dailyTasks)
        {
            if (slot.IsRecurring && slot.DayOfWeek != null)
            {
                // Tính ngày trong tuần hiện tại khớp với DayOfWeek của slot
                var targetDayOfWeek = Enum.Parse<DayOfWeek>(slot.DayOfWeek, true);
                var daysFromStartOfWeek = ((int)targetDayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
                var newDate = startOfWeek.AddDays(daysFromStartOfWeek);

                // Đảm bảo ngày không trong quá khứ (ít nhất là hôm nay)
                if (newDate < today)
                {
                    // Nếu ngày đã qua trong tuần, không cập nhật (giữ nguyên hoặc để job AutoAssignTasksAsync xử lý)
                    continue;
                }

                dailyTask.AssignedDate = newDate;
            }
            else if (!slot.IsRecurring && slot.SpecificDate.HasValue)
            {
                // Nếu SpecificDate nằm trong tuần và không trong quá khứ, cập nhật
                if (slot.SpecificDate.Value >= startOfWeek &&
                    slot.SpecificDate.Value <= endOfWeek &&
                    slot.SpecificDate.Value >= today)
                {
                    dailyTask.AssignedDate = slot.SpecificDate.Value;
                }
            }

            dailyTask.StartTime = slot.StartTime;
            dailyTask.EndTime = slot.EndTime;
            dailyTask.TaskId = slot.TaskId;
            dailyTask.SlotId = slot.Id;
            dailyTask.TeamId = slot.TeamId;
            dailyTask.Status = DailyTaskStatusConstant.SCHEDULED;
            dailyTask.Title = task.Title;
            dailyTask.Description = task.Description;
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

    public async Task<Slot> GetByIdAsync(Guid id, DateOnly? bookingDate)
    {
        var booking_date = bookingDate ?? DateOnly.FromDateTime(DateTime.UtcNow.Date);
        return await _unitOfWork.SlotRepository.GetByIdAsync(id,
            includeFunc: x => x
                .Include(x => x.Area)
                .Include(x => x.Service)
                .Include(x => x.PetGroup).ThenInclude(x => x!.Pets.Where(x => !x.IsDeleted))
                .Include(x => x.PetGroup).ThenInclude(x => x!.PetSpecies!)
                .Include(x => x.SlotAvailabilities.Where(x => x.BookingDate == booking_date))
                .Include(x => x.PetGroup).ThenInclude(x => x!.PetBreed!)
        ) ?? throw new BadRequestException("Không tìm thấy thông tin!");
    }

    public async Task<BasePagingResponseModel<Slot>> GetAllPagingByServiceAsync(Guid serviceId, FilterQuery query, DateOnly? bookingDateTo, DateOnly? bookingDateFrom)
    {

        Expression<Func<Slot, bool>> filter = x => x.ServiceId == serviceId;

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
                .Include(x => x.SlotAvailabilities.Where(x => x.BookingDate >= bookingDateFrom && x.BookingDate <= bookingDateTo))
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
