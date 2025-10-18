using Microsoft.EntityFrameworkCore;
using PetCafe.Application.GlobalExceptionHandling.Exceptions;
using PetCafe.Application.Models.ShareModels;
using PetCafe.Application.Models.SlotModels;
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

public class SlotService(IUnitOfWork _unitOfWork) : ISlotService
{
    public async Task<Slot> CreateAsync(SlotCreateModel model)
    {
        await ValidateAreaAndTimeAvailability(model.AreaId, model.StartTime, model.EndTime, model.ApplicableDays, null);

        // Kiểm tra nhóm thú cưng có sẵn sàng tại thời điểm đó không
        await ValidatePetGroupAvailability(model.PetGroupId, model.StartTime, model.EndTime, null);

        var slot = _unitOfWork.Mapper.Map<Slot>(model);
        await _unitOfWork.SlotRepository.AddAsync(slot);
        await _unitOfWork.SaveChangesAsync();
        return slot;
    }

    public async Task<Slot> UpdateAsync(Guid id, SlotUpdateModel model)
    {
        var slot = await _unitOfWork.SlotRepository.GetByIdAsync(id) ?? throw new BadRequestException("Không tìm thấy thông tin!");

        // Kiểm tra xem area và time có trùng với những slot khác không
        await ValidateAreaAndTimeAvailability(model.AreaId, model.StartTime, model.EndTime, model.ApplicableDays, id);

        // Kiểm tra nhóm thú cưng có sẵn sàng tại thời điểm đó không
        await ValidatePetGroupAvailability(model.PetGroupId, model.StartTime, model.EndTime, id);
        _unitOfWork.Mapper.Map(model, slot);
        _unitOfWork.SlotRepository.Update(slot);
        await _unitOfWork.SaveChangesAsync();
        return slot;
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
                .Include(x => x.PetGroup).ThenInclude(x => x.Pets.Where(x => !x.IsDeleted))
                .Include(x => x.PetGroup).ThenInclude(x => x.PetSpecies!)
                .Include(x => x.PetGroup).ThenInclude(x => x.PetBreed!)
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
                .Include(x => x.PetGroup).ThenInclude(x => x.PetSpecies!)
                .Include(x => x.PetGroup).ThenInclude(x => x.PetBreed!)
        );
        return BasePagingResponseModel<Slot>.CreateInstance(Entities, Pagination);
    }

    private async Task ValidateAreaAndTimeAvailability(Guid areaId, TimeSpan startTime, TimeSpan endTime, List<string> applicableDays, Guid? excludeSlotId)
    {
        // Tìm các slot có cùng area và thời gian trùng lặp
        var overlappingSlots = await _unitOfWork.SlotRepository
            .WhereAsync(s =>
                s.AreaId == areaId &&
                !s.IsDeleted &&
                ((s.StartTime <= startTime && s.EndTime > startTime) || // Slot hiện tại bắt đầu trong khoảng thời gian của slot khác
                (s.StartTime < endTime && s.EndTime >= endTime) || // Slot hiện tại kết thúc trong khoảng thời gian của slot khác
                (s.StartTime >= startTime && s.EndTime <= endTime)) && // Slot khác nằm hoàn toàn trong khoảng thời gian của slot hiện tại
                (excludeSlotId == null || s.Id != excludeSlotId) // Loại trừ slot hiện tại khi cập nhật
            );

        if (overlappingSlots.Count > 0)
        {
            // Kiểm tra xem có trùng lặp về ngày áp dụng không
            foreach (var slot in overlappingSlots)
            {
                // Kiểm tra xem có bất kỳ ngày nào trùng nhau không
                bool hasOverlappingDays = slot.ApplicableDays.Intersect(applicableDays).Any();

                if (hasOverlappingDays)
                {
                    string overlappingDays = string.Join(", ", slot.ApplicableDays.Intersect(applicableDays));
                    throw new BadRequestException(
                        $"Khu vực này đã được đặt trong khoảng thời gian {slot.StartTime.ToString(@"hh\:mm")} - {slot.EndTime.ToString(@"hh\:mm")} " +
                        $"vào các ngày: {overlappingDays}. Vui lòng chọn thời gian, ngày hoặc khu vực khác!");
                }
            }
        }
    }

    // Phương thức kiểm tra nhóm thú cưng có sẵn sàng tại thời điểm đó không
    private async Task ValidatePetGroupAvailability(Guid petGroupId, TimeSpan startTime, TimeSpan endTime, Guid? excludeSlotId)
    {
        // Tìm các slot có cùng nhóm thú cưng và thời gian trùng lặp
        var overlappingSlots = await _unitOfWork.SlotRepository
            .WhereAsync(s =>
                s.PetGroupId == petGroupId &&
                !s.IsDeleted &&
                ((s.StartTime <= startTime && s.EndTime > startTime) || // Slot hiện tại bắt đầu trong khoảng thời gian của slot khác
                (s.StartTime < endTime && s.EndTime >= endTime) || // Slot hiện tại kết thúc trong khoảng thời gian của slot khác
                (s.StartTime >= startTime && s.EndTime <= endTime)) && // Slot khác nằm hoàn toàn trong khoảng thời gian của slot hiện tại
                (excludeSlotId == null || s.Id != excludeSlotId) // Loại trừ slot hiện tại khi cập nhật
            );


        if (overlappingSlots.Count != 0)
        {
            throw new BadRequestException("Nhóm thú cưng này đã được đặt trong khoảng thời gian này. Vui lòng chọn thời gian khác hoặc nhóm thú cưng khác!");
        }
    }
}
