using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PetCafe.Application.GlobalExceptionHandling.Exceptions;
using PetCafe.Application.Models.ShareModels;
using PetCafe.Application.Models.VaccinationScheduleModels;
using PetCafe.Application.Utilities;
using PetCafe.Domain.Entities;

namespace PetCafe.Application.Services;


public interface IVaccinationScheduleService
{
    Task<VaccinationSchedule> CreateAsync(VaccinationScheduleCreateModel model);
    Task<VaccinationSchedule> UpdateAsync(Guid id, VaccinationScheduleUpdateModel model);
    Task<bool> DeleteAsync(Guid id);
    Task<VaccinationSchedule> GetByIdAsync(Guid id);

    Task<BasePagingResponseModel<VaccinationSchedule>> GetAllAsync(VaccinationScheduleScheduleFilterQuery query);
}


public class VaccinationScheduleService(IUnitOfWork _unitOfWork) : IVaccinationScheduleService
{
    public async Task<VaccinationSchedule> CreateAsync(VaccinationScheduleCreateModel model)
    {
        var schedule = _unitOfWork.Mapper.Map<VaccinationSchedule>(model);
        await _unitOfWork.VaccinationScheduleRepository.AddAsync(schedule);
        await _unitOfWork.SaveChangesAsync();
        return schedule;
    }

    public async Task<VaccinationSchedule> UpdateAsync(Guid id, VaccinationScheduleUpdateModel model)
    {
        var schedule = await _unitOfWork.VaccinationScheduleRepository.GetByIdAsync(id) ?? throw new BadRequestException("Không tìm thấy thông tin!");
        _unitOfWork.Mapper.Map(model, schedule);
        _unitOfWork.VaccinationScheduleRepository.Update(schedule);
        await _unitOfWork.SaveChangesAsync();
        return schedule;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var schedule = await _unitOfWork.VaccinationScheduleRepository.GetByIdAsync(id) ?? throw new BadRequestException("Không tìm thấy thông tin!");
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
                    .Include(x => x.VaccineType)
                    .Include(x => x.Record!)
                ) ?? throw new BadRequestException("Không tìm thấy thông tin!");
    }

    public async Task<BasePagingResponseModel<VaccinationSchedule>> GetAllAsync(VaccinationScheduleScheduleFilterQuery query)
    {
        Expression<Func<VaccinationSchedule, bool>>? filter = x => x.VaccineType.Name.Contains(query.VaccineType);

        if (query.PetId != null || query.PetId != Guid.Empty)
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
                        .Include(x => x.VaccineType)
                        .Include(x => x.Record!)
                );

        return BasePagingResponseModel<VaccinationSchedule>.CreateInstance(Entities, Pagination); ;
    }
}