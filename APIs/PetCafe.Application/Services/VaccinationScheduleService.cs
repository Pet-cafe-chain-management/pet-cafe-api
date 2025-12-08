using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PetCafe.Application.GlobalExceptionHandling.Exceptions;
using PetCafe.Application.Models.ShareModels;
using PetCafe.Application.Models.VaccinationScheduleModels;
using PetCafe.Application.Utilities;
using PetCafe.Domain.Entities;
using PetCafe.Domain.Constants;
using Task = System.Threading.Tasks.Task;
using Hangfire;

namespace PetCafe.Application.Services;


public interface IVaccinationScheduleService
{
    Task<VaccinationSchedule> CreateAsync(VaccinationScheduleCreateModel model);
    Task<VaccinationSchedule> UpdateAsync(Guid id, VaccinationScheduleUpdateModel model);
    Task<bool> DeleteAsync(Guid id);
    Task<VaccinationSchedule> GetByIdAsync(Guid id);

    Task<BasePagingResponseModel<VaccinationSchedule>> GetAllAsync(VaccinationScheduleScheduleFilterQuery query);
    Task CreateOrUpdateDailyTaskAsync(Guid scheduleId, Guid teamId);
}


public class VaccinationScheduleService(
    IUnitOfWork _unitOfWork,
    IBackgroundJobClient _backgroundJobClient) : IVaccinationScheduleService
{
    public async Task<VaccinationSchedule> CreateAsync(VaccinationScheduleCreateModel model)
    {
        var schedule = _unitOfWork.Mapper.Map<VaccinationSchedule>(model);
        await _unitOfWork.VaccinationScheduleRepository.AddAsync(schedule);
        await _unitOfWork.SaveChangesAsync();

        // Load Pet and VaccineType for DailyTask creation
        schedule = await _unitOfWork.VaccinationScheduleRepository.GetByIdAsync(
            schedule.Id,
            includeFunc: x => x
                .Include(x => x.Pet)
        ) ?? throw new BadRequestException("Không tìm thấy thông tin!");

        _backgroundJobClient.Enqueue(() => CreateOrUpdateDailyTaskAsync(schedule.Id, model.TeamId));

        return schedule;
    }

    public async Task<VaccinationSchedule> UpdateAsync(Guid id, VaccinationScheduleUpdateModel model)
    {
        var schedule = await _unitOfWork.VaccinationScheduleRepository.GetByIdAsync(
            id,
            includeFunc: x => x
                .Include(x => x.Pet)
        ) ?? throw new BadRequestException("Không tìm thấy thông tin!");

        _unitOfWork.Mapper.Map(model, schedule);
        _unitOfWork.VaccinationScheduleRepository.Update(schedule);
        await _unitOfWork.SaveChangesAsync();

        // Update or create DailyTask for the vaccination schedule
        _backgroundJobClient.Enqueue(() => CreateOrUpdateDailyTaskAsync(schedule.Id, model.TeamId));

        return schedule;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var schedule = await _unitOfWork.VaccinationScheduleRepository.GetByIdAsync(id) ?? throw new BadRequestException("Không tìm thấy thông tin!");

        // Find and cancel/delete related DailyTask
        var dailyTask = await _unitOfWork.DailyTaskRepository.FirstOrDefaultAsync(
            x => x.VaccinationScheduleId == id && !x.IsDeleted
        );

        if (dailyTask != null)
        {
            // Cancel the DailyTask if it's not completed
            if (dailyTask.Status != DailyTaskStatusConstant.COMPLETED)
            {
                dailyTask.Status = DailyTaskStatusConstant.CANCELLED;
                _unitOfWork.DailyTaskRepository.Update(dailyTask);
            }
            else
            {
                // Soft delete if already completed
                _unitOfWork.DailyTaskRepository.SoftRemove(dailyTask);
            }
        }

        _unitOfWork.VaccinationScheduleRepository.SoftRemove(schedule);
        return await _unitOfWork.SaveChangesAsync();
    }

    public async Task<VaccinationSchedule> GetByIdAsync(Guid id)
    {
        return await _unitOfWork
            .VaccinationScheduleRepository
            .GetByIdAsync(
                id,
                includeFunc: x => x
                    .Include(x => x.Pet)
                    .Include(x => x.Record!)
                    .Include(x => x.DailyTask!).ThenInclude(x => x.Team)
                ) ?? throw new BadRequestException("Không tìm thấy thông tin!");
    }

    public async Task<BasePagingResponseModel<VaccinationSchedule>> GetAllAsync(VaccinationScheduleScheduleFilterQuery query)
    {
        Expression<Func<VaccinationSchedule, bool>>? filter = null;

        if (query.PetId != null && query.PetId != Guid.Empty)
        {
            Expression<Func<VaccinationSchedule, bool>> filter_pet = x => x.PetId == query.PetId;
            filter = filter != null ? FilterCustoms.CombineFilters(filter, filter_pet) : filter_pet;
        }

        if (query.FromDate != null && query.ToDate != null)
        {
            Expression<Func<VaccinationSchedule, bool>> filter_range = x => x.ScheduledDate >= query.FromDate && x.ScheduledDate <= query.ToDate;
            filter = filter != null ? FilterCustoms.CombineFilters(filter, filter_range) : filter_range;
        }
        else if (query.ToDate != null)
        {
            Expression<Func<VaccinationSchedule, bool>> filter_to = x => x.ScheduledDate <= query.ToDate;
            filter = filter != null ? FilterCustoms.CombineFilters(filter, filter_to) : filter_to;
        }
        else if (query.FromDate != null)
        {
            Expression<Func<VaccinationSchedule, bool>> filter_from = x => x.ScheduledDate >= query.FromDate;
            filter = filter != null ? FilterCustoms.CombineFilters(filter, filter_from) : filter_from;
        }

        if (!string.IsNullOrEmpty(query.Status))
        {
            Expression<Func<VaccinationSchedule, bool>> filter_status = x => x.Status == query.Status;
            filter = filter != null ? FilterCustoms.CombineFilters(filter, filter_status) : filter_status;
        }
        if (!string.IsNullOrEmpty(query.VaccineType))
        {
            Expression<Func<VaccinationSchedule, bool>> addtional_filter = x => x.Record!.Name.Contains(query.VaccineType);
            filter = filter != null ? FilterCustoms.CombineFilters(filter, addtional_filter) : addtional_filter;
        }

        var (Pagination, Entities) = await _unitOfWork.VaccinationScheduleRepository.ToPagination(
                    pageIndex: query.Page ?? 0,
                    pageSize: query.Limit ?? 10,
                    filter: filter,
                    sortOrders: query.OrderBy?.ToDictionary(
                            k => k.OrderColumn ?? "CreatedAt",
                            v => (v.OrderDir ?? "ASC").Equals("ASC", StringComparison.CurrentCultureIgnoreCase)
                        ) ?? new Dictionary<string, bool> { { "CreatedAt", false } },
                    includeFunc: x => x
                        .Include(x => x.Pet)
                        .Include(x => x.Record!)
                        .Include(x => x.DailyTask!).ThenInclude(x => x.Team)
                );

        return BasePagingResponseModel<VaccinationSchedule>.CreateInstance(Entities, Pagination); ;
    }

    public async Task CreateOrUpdateDailyTaskAsync(Guid scheduleId, Guid teamId)
    {
        // Reload schedule with necessary includes for background job
        var schedule = await _unitOfWork.VaccinationScheduleRepository.GetByIdAsync(
            scheduleId,
            includeFunc: x => x
                .Include(x => x.Pet)
        ) ?? throw new BadRequestException("Không tìm thấy thông tin lịch tiêm!");

        var existingDailyTask = await _unitOfWork.DailyTaskRepository.FirstOrDefaultAsync(
            x => x.VaccinationScheduleId == schedule.Id && !x.IsDeleted
        );

        var team = await _unitOfWork.TeamRepository.GetByIdAsync(teamId) ?? throw new BadRequestException("Không tìm thấy team!");

        var title = $"Tiêm vaccine cho {schedule.Pet.Name}";
        var description = $"Lịch tiêm vaccine cho thú cưng {schedule.Pet.Name}";

        var scheduledTime = schedule.ScheduledDate.TimeOfDay;
        if (scheduledTime == TimeSpan.Zero)
        {
            scheduledTime = new TimeSpan(9, 0, 0);
        }

        var endTime = scheduledTime.Add(new TimeSpan(1, 0, 0));

        if (existingDailyTask != null)
        {
            existingDailyTask.Title = title;
            existingDailyTask.Description = description;
            existingDailyTask.AssignedDate = schedule.ScheduledDate.Date;
            existingDailyTask.StartTime = scheduledTime;
            existingDailyTask.EndTime = endTime;
            existingDailyTask.Priority = TaskPriorityConstant.HIGH;
            existingDailyTask.Notes = schedule.Notes;

            if (schedule.Status == VaccinationScheduleStatus.COMPLETED)
            {
                existingDailyTask.Status = DailyTaskStatusConstant.COMPLETED;
                existingDailyTask.CompletionDate = schedule.CompletedDate ?? DateTime.UtcNow;
            }
            else if (schedule.Status == VaccinationScheduleStatus.CANCELLED)
            {
                existingDailyTask.Status = DailyTaskStatusConstant.CANCELLED;
            }
            else if (schedule.Status == VaccinationScheduleStatus.IN_PROGRESS)
            {
                existingDailyTask.Status = DailyTaskStatusConstant.IN_PROGRESS;
            }
            else
            {
                existingDailyTask.Status = DailyTaskStatusConstant.SCHEDULED;
            }

            _unitOfWork.DailyTaskRepository.Update(existingDailyTask);
            schedule.DailyTaskId = existingDailyTask.Id;
        }
        else
        {
            var dailyTask = new DailyTask
            {
                TeamId = team.Id,
                Title = title,
                Description = description,
                Priority = TaskPriorityConstant.HIGH,
                Status = schedule.Status == VaccinationScheduleStatus.COMPLETED
                    ? DailyTaskStatusConstant.COMPLETED
                    : DailyTaskStatusConstant.SCHEDULED,
                AssignedDate = schedule.ScheduledDate.Date,
                StartTime = scheduledTime,
                EndTime = endTime,
                VaccinationScheduleId = schedule.Id,
                Notes = schedule.Notes,
                CompletionDate = schedule.Status == VaccinationScheduleStatus.COMPLETED
                    ? (schedule.CompletedDate ?? DateTime.UtcNow)
                    : null
            };

            await _unitOfWork.DailyTaskRepository.AddAsync(dailyTask);
            await _unitOfWork.SaveChangesAsync();
            schedule.DailyTaskId = dailyTask.Id;
        }

        _unitOfWork.VaccinationScheduleRepository.Update(schedule);
        await _unitOfWork.SaveChangesAsync();
    }
}