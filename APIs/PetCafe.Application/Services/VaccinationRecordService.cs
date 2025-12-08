using Hangfire;
using Microsoft.EntityFrameworkCore;
using PetCafe.Application.GlobalExceptionHandling.Exceptions;
using PetCafe.Application.Models.ShareModels;
using PetCafe.Application.Models.VaccinationRecordModels;
using PetCafe.Application.Services.Commons;
using PetCafe.Domain.Constants;
using PetCafe.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace PetCafe.Application.Services;

public interface IVaccinationRecordService
{
    Task<VaccinationRecord> CreateAsync(VaccinationRecordCreateModel model);
    Task<VaccinationRecord> UpdateAsync(Guid id, VaccinationRecordUpdateModel model);
    Task<bool> DeleteAsync(Guid id);
    Task<VaccinationRecord> GetByIdAsync(Guid id);
    Task<BasePagingResponseModel<VaccinationRecord>> GetAllByPetIdAsync(Guid petId, FilterQuery query);
}


public class VaccinationRecordService(
    IUnitOfWork _unitOfWork,
    ICurrentTime _currentTime
) : IVaccinationRecordService
{
    public async Task<VaccinationRecord> CreateAsync(VaccinationRecordCreateModel model)
    {

        var schedule = await _unitOfWork.VaccinationScheduleRepository.GetByIdAsync(
            model.ScheduleId,
            includeFunc: x => x
                .Include(x => x.Pet)
                .Include(x => x.Record!)
        ) ?? throw new BadRequestException("Không tìm thấy lịch tiêm chủng!");

        var vaccinationRecord = _unitOfWork.Mapper.Map<VaccinationRecord>(model);

        schedule.Status = VaccinationScheduleStatus.COMPLETED;
        schedule.CompletedDate = _currentTime.GetCurrentTime;
        schedule.RecordId = vaccinationRecord.Id;

        // Get TeamId from existing DailyTask of the current schedule, or get first active team
        var existingDailyTask = await _unitOfWork.DailyTaskRepository.FirstOrDefaultAsync(
            x => x.VaccinationScheduleId == schedule.Id && !x.IsDeleted
        );

        Guid teamId;
        if (existingDailyTask != null)
        {
            teamId = existingDailyTask.TeamId;
            // Update existing DailyTask to COMPLETED
            existingDailyTask.Status = DailyTaskStatusConstant.COMPLETED;
            existingDailyTask.CompletionDate = _currentTime.GetCurrentTime;
            _unitOfWork.DailyTaskRepository.Update(existingDailyTask);
        }
        else
        {
            // Get first active team as fallback
            var team = await _unitOfWork.TeamRepository.FirstOrDefaultAsync(
                x => x.IsActive && !x.IsDeleted
            ) ?? throw new BadRequestException("Không tìm thấy team đang hoạt động để gán nhiệm vụ!");
            teamId = team.Id;
        }
        await _unitOfWork.VaccinationRecordRepository.AddAsync(vaccinationRecord);
        _unitOfWork.VaccinationScheduleRepository.Update(schedule);
        await _unitOfWork.SaveChangesAsync();

        return vaccinationRecord;
    }

    public async Task<VaccinationRecord> UpdateAsync(Guid id, VaccinationRecordUpdateModel model)
    {
        var vaccinationRecord = await _unitOfWork.VaccinationRecordRepository.GetByIdAsync(
            id,
            includeFunc: x => x
                .Include(x => x.Schedule!)
                    .ThenInclude(s => s.Pet)
                .Include(x => x.Schedule!)
        ) ?? throw new BadRequestException("Không tìm thấy thông tin!");

        _unitOfWork.Mapper.Map(model, vaccinationRecord);
        _unitOfWork.VaccinationRecordRepository.Update(vaccinationRecord);
        await _unitOfWork.SaveChangesAsync();

        // Update related DailyTask if schedule exists and is completed
        if (vaccinationRecord.Schedule != null && vaccinationRecord.Schedule.Status == VaccinationScheduleStatus.COMPLETED)
        {
            var dailyTask = await _unitOfWork.DailyTaskRepository.FirstOrDefaultAsync(
                x => x.VaccinationScheduleId == vaccinationRecord.Schedule.Id && !x.IsDeleted
            );

            if (dailyTask != null)
            {
                dailyTask.Status = DailyTaskStatusConstant.COMPLETED;
                dailyTask.CompletionDate = vaccinationRecord.Schedule.CompletedDate ?? _currentTime.GetCurrentTime;
                _unitOfWork.DailyTaskRepository.Update(dailyTask);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        return vaccinationRecord;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var vaccinationRecord = await _unitOfWork.VaccinationRecordRepository.GetByIdAsync(id) ?? throw new BadRequestException("Không tìm thấy thông tin!");
        _unitOfWork.VaccinationRecordRepository.SoftRemove(vaccinationRecord);
        return await _unitOfWork.SaveChangesAsync();
    }

    public async Task<VaccinationRecord> GetByIdAsync(Guid id)
    {
        return await _unitOfWork
            .VaccinationRecordRepository
            .GetByIdAsync(
                id,
                includeFunc: x => x
                    .Include(x => x.Pet)
                    .Include(x => x.Schedule!)
                ) ?? throw new BadRequestException("Không tìm thấy thông tin!");
    }

    public async Task<BasePagingResponseModel<VaccinationRecord>> GetAllByPetIdAsync(Guid petId, FilterQuery query)
    {
        var (Pagination, Entities) = await _unitOfWork.VaccinationRecordRepository.ToPagination(
            pageIndex: query.Page ?? 0,
           pageSize: query.Limit ?? 10,
           filter: x => x.PetId == petId,
           searchTerm: query.Q,
           searchFields: ["Veterinarian", "BatchNumber", "ClinicName", "Notes"],
           sortOrders: query.OrderBy?.ToDictionary(
                   k => k.OrderColumn ?? "CreatedAt",
                   v => (v.OrderDir ?? "ASC").Equals("ASC", StringComparison.CurrentCultureIgnoreCase)
               ) ?? new Dictionary<string, bool> { { "CreatedAt", false } },
           includeFunc: x => x.Include(x => x.Schedule!)
       );
        return BasePagingResponseModel<VaccinationRecord>.CreateInstance(Entities, Pagination);
    }
}
