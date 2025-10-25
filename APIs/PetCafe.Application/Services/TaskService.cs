using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PetCafe.Application.GlobalExceptionHandling.Exceptions;
using PetCafe.Application.Models.ShareModels;
using PetCafe.Application.Models.SlotModels;
using PetCafe.Application.Models.TaskModels;
using PetCafe.Application.Utilities;
using PetCafe.Domain.Entities;
using Task = PetCafe.Domain.Entities.Task;

namespace PetCafe.Application.Services;

public interface ITaskService
{
    Task<Task> CreateTaskAsync(TaskCreateModel model);
    Task<Task> UpdateTaskAsync(Guid id, TaskUpdateModel model);
    Task<Task> GetTaskByIdAsync(Guid id);
    Task<BasePagingResponseModel<Task>> GetAllTasksAsync(TaskFilterQuery query);
    Task<bool> DeleteTaskAsync(Guid id);
    Task<BasePagingResponseModel<Slot>> GetSlotsByTaskIdAsync(Guid taskId, SlotFilterQuery query);
}

public class TaskService(
    IUnitOfWork _unitOfWork
) : ITaskService
{

    public async Task<BasePagingResponseModel<Slot>> GetSlotsByTaskIdAsync(Guid taskId, SlotFilterQuery query)
    {
        Expression<Func<Slot, bool>> filter = x => x.TaskId == taskId;
        if (query.DayOfWeek != null)
        {
            Expression<Func<Slot, bool>> tmp_filter = x => x.DayOfWeek == query.DayOfWeek;
            filter = filter != null ? FilterCustoms.CombineFilters(filter, tmp_filter) : tmp_filter;
        }
        if (query.StartTime != null)
        {
            Expression<Func<Slot, bool>> tmp_filter = x => x.StartTime >= query.StartTime;
            filter = filter != null ? FilterCustoms.CombineFilters(filter, tmp_filter) : tmp_filter;
        }
        if (query.EndTime != null)
        {
            Expression<Func<Slot, bool>> tmp_filter = x => x.EndTime <= query.EndTime;
            filter = filter != null ? FilterCustoms.CombineFilters(filter, tmp_filter) : tmp_filter;
        }
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
                .Include(x => x.Area)
                .Include(x => x.Service)
                .Include(x => x.PetGroup).ThenInclude(x => x!.PetSpecies!)
                .Include(x => x.PetGroup).ThenInclude(x => x!.PetBreed!)
                .Include(x => x.Team)
        );
        return BasePagingResponseModel<Slot>.CreateInstance(Entities, Pagination);
    }

    public async Task<Task> CreateTaskAsync(TaskCreateModel model)
    {
        var workType = await _unitOfWork.WorkTypeRepository.GetByIdAsync(model.WorkTypeId) ??
            throw new BadRequestException("Không tìm thấy thông tin loại công việc!");

        var task = _unitOfWork.Mapper.Map<Task>(model);
        await _unitOfWork.TaskRepository.AddAsync(task);
        await _unitOfWork.SaveChangesAsync();

        return task;
    }

    public async Task<Task> UpdateTaskAsync(Guid id, TaskUpdateModel model)
    {
        var workType = await _unitOfWork.WorkTypeRepository.GetByIdAsync(model.WorkTypeId) ??
            throw new BadRequestException("Không tìm thấy thông tin loại công việc!");

        var task = await _unitOfWork.TaskRepository.GetByIdAsync(id) ?? throw new BadRequestException($"không tìm thấy thông tin!");
        _unitOfWork.Mapper.Map(model, task);
        _unitOfWork.TaskRepository.Update(task);
        await _unitOfWork.SaveChangesAsync();
        return task;
    }

    public async Task<Task> GetTaskByIdAsync(Guid id)
    {
        return await _unitOfWork.TaskRepository.GetByIdAsync(id, includeFunc: x => x.Include(x => x.WorkType)) ?? throw new BadRequestException($"Không tìm thấy thông tin!");
    }

    public async Task<BasePagingResponseModel<Task>> GetAllTasksAsync(TaskFilterQuery query)
    {
        Expression<Func<Task, bool>>? filter = null;

        if (query.WorkTypeId.HasValue)
        {
            Expression<Func<Task, bool>> filter_work_type = x => x.WorkTypeId == query.WorkTypeId.Value;
            filter = filter == null ? filter_work_type : FilterCustoms.CombineFilters(filter, filter_work_type);
        }
        if (!string.IsNullOrEmpty(query.Status))
        {
            Expression<Func<Task, bool>> filter_status = x => x.Status == query.Status;
            filter = filter == null ? filter_status : FilterCustoms.CombineFilters(filter, filter_status);
        }
        if (!string.IsNullOrEmpty(query.Priority))
        {
            Expression<Func<Task, bool>> filter_priority = x => x.Priority == query.Priority;
            filter = filter == null ? filter_priority : FilterCustoms.CombineFilters(filter, filter_priority);
        }


        var (Pagination, Entities) = await _unitOfWork.TaskRepository.ToPagination(
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
                    .Include(x => x.WorkType)
        );


        return BasePagingResponseModel<Task>.CreateInstance(Entities, Pagination);
    }


    public async Task<bool> DeleteTaskAsync(Guid id)
    {
        var task = await _unitOfWork.TaskRepository.GetByIdAsync(id)
            ?? throw new BadRequestException($"Không tìm thấy thông tin!");

        _unitOfWork.TaskRepository.SoftRemove(task);
        return await _unitOfWork.SaveChangesAsync();
    }



}