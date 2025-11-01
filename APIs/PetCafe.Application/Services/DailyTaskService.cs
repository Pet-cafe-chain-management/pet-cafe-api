using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PetCafe.Application.GlobalExceptionHandling.Exceptions;
using PetCafe.Application.Models.DailyTaskModels;
using PetCafe.Application.Models.ShareModels;
using PetCafe.Application.Services.Commons;
using PetCafe.Application.Utilities;
using PetCafe.Domain.Constants;
using PetCafe.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace PetCafe.Application.Services;
public interface IDailyTaskService
{
    Task<BasePagingResponseModel<DailyTask>> GetAllPagingAsync(DailyTaskFilterQuery query);
    Task<DailyTask> CreateAsync(DailyTaskCreateModel model);
    Task<DailyTask> UpdateAsync(Guid id, DailyTaskUpdateModel model);
    Task<bool> DeleteAsync(Guid id);

    Task AutoAssignTasksAsync();

    Task AutoChangeStatusAsync();

    Task CreateDailyTasksFromSlotAsync(Slot slot, Domain.Entities.Task task, List<DateTime> dates, List<DailyTask>? existingDailyTasks = null);

    Task CreateDailyTasksFromSpecificDateAsync(Slot slot, Domain.Entities.Task task, List<DailyTask>? existingDailyTasks = null);

}


public class DailyTaskService(
    IUnitOfWork _unitOfWork,
    IClaimsService _claimsService) : IDailyTaskService
{

    public async Task<BasePagingResponseModel<DailyTask>> GetAllPagingAsync(DailyTaskFilterQuery query)
    {
        Expression<Func<DailyTask, bool>> filter = x => true;
        if (query.TeamId.HasValue)
        {
            filter = filter != null ? FilterCustoms.CombineFilters(filter, x => x.TeamId == query.TeamId.Value) : x => x.TeamId == query.TeamId.Value;
        }
        if (query.FromDate.HasValue)
        {
            filter = filter != null ? FilterCustoms.CombineFilters(filter, x => x.AssignedDate >= query.FromDate.Value) : x => x.AssignedDate >= query.FromDate.Value;
        }
        if (query.ToDate.HasValue)
        {
            filter = filter != null ? FilterCustoms.CombineFilters(filter, x => x.AssignedDate <= query.ToDate.Value) : x => x.AssignedDate <= query.ToDate.Value;
        }
        if (!string.IsNullOrEmpty(query.Status))
        {
            filter = filter != null ? FilterCustoms.CombineFilters(filter, x => x.Status == query.Status) : x => x.Status == query.Status;
        }
        var (Pagination, Entities) = await _unitOfWork.DailyTaskRepository.ToPagination(
            pageIndex: query.Page ?? 0,
            pageSize: query.Limit ?? 10,
            filter: filter,
            searchTerm: query.Q,
            searchFields: ["Task.Title", "Slot.Name", "Slot.Time", "Slot.DayOfWeek", "Slot.Area", "Slot.Team", "Slot.PetGroup", "Slot.Pet", "Slot.SpecialNotes"],
            sortOrders: query.OrderBy?.ToDictionary(
                    k => k.OrderColumn ?? "CreatedAt",
                    v => (v.OrderDir ?? "ASC").Equals("ASC", StringComparison.CurrentCultureIgnoreCase)
                ) ?? new Dictionary<string, bool> { { "CreatedAt", false } },
            includeFunc: x => x.Include(x => x.Task).Include(x => x.Slot).Include(x => x.Team)
        );
        return BasePagingResponseModel<DailyTask>.CreateInstance(Entities, Pagination);
    }

    public async Task AutoAssignTasksAsync()
    {
        // Lấy ngày đầu tuần (Thứ 2)
        var today = DateTime.UtcNow.Date;
        var startOfWeek = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);

        // Nếu hôm nay là Chủ nhật, lấy thứ 2 tuần sau
        if (today.DayOfWeek == DayOfWeek.Sunday)
        {
            startOfWeek = startOfWeek.AddDays(7);
        }

        var endOfWeek = startOfWeek.AddDays(6);

        // Lấy tất cả slots recurring một lần
        var allSlots = await _unitOfWork.SlotRepository.WhereAsync(
            x => x.IsRecurring && !x.IsDeleted,
            includeFunc: x => x.Include(s => s.Task)
        );

        // Lấy tất cả DailyTasks đã tồn tại trong tuần này
        var existingDailyTasks = await _unitOfWork.DailyTaskRepository.WhereAsync(
            x => x.AssignedDate.Date >= startOfWeek &&
                 x.AssignedDate.Date <= endOfWeek
        );

        var dailyTasksToAdd = new List<DailyTask>();

        // Lặp qua 7 ngày trong tuần
        for (int i = 0; i < 7; i++)
        {
            var currentDate = startOfWeek.AddDays(i);
            var dayOfWeek = currentDate.DayOfWeek.ToString().ToUpper();

            // Lọc slots cho ngày hiện tại
            var slotsForDay = allSlots.Where(x => x.DayOfWeek == dayOfWeek);

            foreach (var slot in slotsForDay)
            {
                // Kiểm tra xem đã tồn tại DailyTask cho slot này trong ngày chưa
                var exists = existingDailyTasks.Any(x =>
                    x.SlotId == slot.Id &&
                    x.AssignedDate.Date == currentDate.Date
                );

                if (!exists)
                {
                    dailyTasksToAdd.Add(new DailyTask
                    {
                        Title = slot.Task.Title,
                        Description = slot.Task.Description,
                        Priority = slot.Task.Priority,
                        TeamId = slot.TeamId,
                        Status = DailyTaskStatusConstant.SCHEDULED,
                        AssignedDate = currentDate,
                        StartTime = slot.StartTime,
                        EndTime = slot.EndTime,
                        TaskId = slot.TaskId,
                        SlotId = slot.Id,
                    });
                }
            }
        }

        // Bulk insert
        if (dailyTasksToAdd.Count != 0)
        {
            await _unitOfWork.DailyTaskRepository.AddRangeAsync(dailyTasksToAdd);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task<DailyTask> CreateAsync(DailyTaskCreateModel model)
    {
        var dailyTask = _unitOfWork.Mapper.Map<DailyTask>(model);
        await _unitOfWork.DailyTaskRepository.AddAsync(dailyTask);
        await _unitOfWork.SaveChangesAsync();
        return dailyTask;
    }

    public async Task<DailyTask> UpdateAsync(Guid id, DailyTaskUpdateModel model)
    {
        var dailyTask = await _unitOfWork.DailyTaskRepository.GetByIdAsync(id) ?? throw new BadRequestException("Không tìm thấy thông tin!");
        if (dailyTask.Status == DailyTaskStatusConstant.COMPLETED || dailyTask.Status == DailyTaskStatusConstant.CANCELLED || dailyTask.Status == DailyTaskStatusConstant.MISSED)
        {
            throw new BadRequestException("Không thể cập nhật trạng thái của nhiệm vụ đã hoàn thành hoặc bị hủy!");
        }

        var team = await _unitOfWork.TeamRepository
            .GetByIdAsync(dailyTask.TeamId, includeFunc: x => x.Include(x => x.Leader))
             ?? throw new BadRequestException("Không tìm thấy thông tin nhóm!");

        if (model.Status == DailyTaskStatusConstant.COMPLETED && team.LeaderId != _claimsService.GetCurrentUser)
        {
            throw new BadRequestException("Bạn không có quyền cập nhật trạng thái của nhiệm vụ này!");
        }
        if (model.Status == DailyTaskStatusConstant.CANCELLED && team.LeaderId != _claimsService.GetCurrentUser && _claimsService.GetCurrentUserRole != RoleConstants.MANAGER)
        {
            throw new BadRequestException("Bạn không có quyền hủy nhiệm vụ này!");
        }
        if (model.Status == DailyTaskStatusConstant.MISSED && _claimsService.GetCurrentUserRole != RoleConstants.MANAGER)
        {
            throw new BadRequestException("Bạn không có quyền báo cáo nhiệm vụ này!");
        }

        if (model.Status == DailyTaskStatusConstant.IN_PROGRESS && team.Status == TeamStatusConstant.INACTIVE)
        {
            throw new BadRequestException("Không thể cập nhật trạng thái của nhiệm vụ khi nhóm chưa hoàn thành điểm danh!");
        }

        _unitOfWork.Mapper.Map(model, dailyTask);
        _unitOfWork.DailyTaskRepository.Update(dailyTask);
        await _unitOfWork.SaveChangesAsync();
        return dailyTask;
    }
    public async Task<bool> DeleteAsync(Guid id)
    {
        var dailyTask = await _unitOfWork.DailyTaskRepository.GetByIdAsync(id) ?? throw new BadRequestException("Không tìm thấy thông tin!");
        _unitOfWork.DailyTaskRepository.SoftRemove(dailyTask);
        return await _unitOfWork.SaveChangesAsync();
    }

    public async Task AutoChangeStatusAsync()
    {
        var dailyTasks = await _unitOfWork
            .DailyTaskRepository
            .WhereAsync(x =>
                (x.Status == DailyTaskStatusConstant.SCHEDULED || x.Status == DailyTaskStatusConstant.IN_PROGRESS) &&
                x.AssignedDate < DateTime.UtcNow.Date
            );
        if (dailyTasks.Count != 0)
        {
            foreach (var dailyTask in dailyTasks)
            {
                dailyTask.Status = DailyTaskStatusConstant.MISSED;
            }
        }
        _unitOfWork.DailyTaskRepository.UpdateRange(dailyTasks);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task CreateDailyTasksFromSlotAsync(Slot slot, Domain.Entities.Task task, List<DateTime> dates, List<DailyTask>? existingDailyTasks = null)
    {
        var dailyTasksToAdd = new List<DailyTask>();

        foreach (var date in dates)
        {
            var dayOfWeek = date.DayOfWeek.ToString().ToUpper();

            // Nếu ngày trong tuần của slot khớp với ngày được chỉ định
            if (slot.DayOfWeek == dayOfWeek)
            {
                bool exists = false;

                // Nếu có danh sách existingDailyTasks, check trong memory
                if (existingDailyTasks != null)
                {
                    exists = existingDailyTasks.Any(x =>
                        x.SlotId == slot.Id &&
                        x.AssignedDate.Date == date.Date
                    );
                }
                else
                {
                    // Nếu không có, query từ database
                    var existingDailyTask = await _unitOfWork.DailyTaskRepository.FirstOrDefaultAsync(x =>
                        x.SlotId == slot.Id &&
                        x.AssignedDate.Date == date.Date
                    );
                    exists = existingDailyTask != null;
                }

                if (!exists)
                {
                    dailyTasksToAdd.Add(new DailyTask
                    {
                        Title = task.Title,
                        Description = task.Description,
                        Priority = task.Priority,
                        TeamId = slot.TeamId,
                        Status = DailyTaskStatusConstant.SCHEDULED,
                        AssignedDate = date,
                        StartTime = slot.StartTime,
                        EndTime = slot.EndTime,
                        TaskId = task.Id,
                        SlotId = slot.Id,
                    });
                }
            }
        }

        // Bulk insert các DailyTasks
        if (dailyTasksToAdd.Count > 0)
        {
            await _unitOfWork.DailyTaskRepository.AddRangeAsync(dailyTasksToAdd);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task CreateDailyTasksFromSpecificDateAsync(Slot slot, Domain.Entities.Task task, List<DailyTask>? existingDailyTasks = null)
    {
        if (slot.IsRecurring || slot.SpecificDate == null) return;
        var dailyTask = new DailyTask
        {
            Title = task.Title,
            Description = task.Description,
            Priority = task.Priority,
            TeamId = slot.TeamId,
            Status = DailyTaskStatusConstant.SCHEDULED,
            AssignedDate = slot.SpecificDate.Value,
            StartTime = slot.StartTime,
            EndTime = slot.EndTime,
            TaskId = task.Id,
            SlotId = slot.Id,
        };
        await _unitOfWork.DailyTaskRepository.AddAsync(dailyTask);
        await _unitOfWork.SaveChangesAsync();
    }


}
