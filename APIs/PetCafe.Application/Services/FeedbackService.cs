using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PetCafe.Application.GlobalExceptionHandling.Exceptions;
using PetCafe.Application.Models.FeedbackModels;
using PetCafe.Application.Models.ShareModels;
using PetCafe.Application.Services.Commons;
using PetCafe.Application.Utilities;
using PetCafe.Domain.Constants;
using PetCafe.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace PetCafe.Application.Services;

public interface IFeedbackService
{
    Task<ServiceFeedback> GetByIdAsync(Guid id);
    Task<BasePagingResponseModel<ServiceFeedback>> GetAllPagingAsync(FeedbackFilterQuery query);
    Task<ServiceFeedback> CreateAsync(FeedbackCreateModel model);
    Task<ServiceFeedback> UpdateAsync(Guid id, FeedbackUpdateModel model);
    Task<bool> DeleteAsync(Guid id);
    Task<BasePagingResponseModel<ServiceFeedback>> GetByServiceIdAsync(Guid serviceId, FeedbackFilterQuery query);
}

public class FeedbackService(IUnitOfWork _unitOfWork, IClaimsService _claimsService) : IFeedbackService
{
    public async Task<ServiceFeedback> CreateAsync(FeedbackCreateModel model)
    {
        // Validate booking exists and belongs to customer
        var booking = await _unitOfWork.BookingRepository.FirstOrDefaultAsync(
            x => x.Id == model.CustomerBookingId && x.CustomerId == _claimsService.GetCurrentUser
        ) ?? throw new BadRequestException("Không tìm thấy thông tin đặt lịch!");

        if (booking.BookingStatus != BookingStatusConstant.COMPLETED)
        {
            throw new BadRequestException("Đánh giá chỉ được thực hiện sau khi dịch vụ đã hoàn thành!");
        }
        // Check if feedback already exists for this booking
        var existingFeedback = await _unitOfWork.FeedbackRepository.FirstOrDefaultAsync(
            x => x.CustomerBookingId == model.CustomerBookingId
        );
        if (existingFeedback != null)
        {
            throw new BadRequestException("Đã có đánh giá cho lịch đặt này!");
        }

        // Validate service exists
        var service = await _unitOfWork.ServiceRepository.GetByIdAsync(model.ServiceId)
            ?? throw new BadRequestException("Không tìm thấy dịch vụ!");

        // Validate customer exists
        var customer = await _unitOfWork.CustomerRepository.GetByIdAsync(_claimsService.GetCurrentUser)
            ?? throw new BadRequestException("Không tìm thấy khách hàng!");

        var feedback = _unitOfWork.Mapper.Map<ServiceFeedback>(model);
        feedback.FeedbackDate = DateTime.UtcNow;
        feedback.CustomerId = _claimsService.GetCurrentUser;
        await _unitOfWork.FeedbackRepository.AddAsync(feedback);
        await _unitOfWork.SaveChangesAsync();
        return feedback;
    }

    public async Task<ServiceFeedback> UpdateAsync(Guid id, FeedbackUpdateModel model)
    {
        var feedback = await _unitOfWork.FeedbackRepository.GetByIdAsync(id)
            ?? throw new BadRequestException("Không tìm thấy feedback!");

        // Validate booking belongs to customer if changed
        if (feedback.CustomerBookingId != model.CustomerBookingId || feedback.CustomerId != _claimsService.GetCurrentUser)
        {
            var booking = await _unitOfWork.BookingRepository.FirstOrDefaultAsync(
                x => x.Id == model.CustomerBookingId && x.CustomerId == _claimsService.GetCurrentUser
            ) ?? throw new BadRequestException("Không tìm thấy thông tin đặt lịch!");
        }

        _unitOfWork.Mapper.Map(model, feedback);
        feedback.FeedbackDate = DateTime.UtcNow;
        _unitOfWork.FeedbackRepository.Update(feedback);
        await _unitOfWork.SaveChangesAsync();
        return feedback;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var feedback = await _unitOfWork.FeedbackRepository.GetByIdAsync(id)
            ?? throw new BadRequestException("Không tìm thấy thông tin đánh giá!");
        _unitOfWork.FeedbackRepository.SoftRemove(feedback);
        return await _unitOfWork.SaveChangesAsync();
    }

    public async Task<ServiceFeedback> GetByIdAsync(Guid id)
    {
        return await _unitOfWork.FeedbackRepository.GetByIdAsync(id,
            includeFunc: x => x
                .Include(f => f.Customer)
                .Include(f => f.Service)
                .Include(f => f.CustomerBooking)
        ) ?? throw new BadRequestException("Không tìm thấy thông tin đánh giá!");
    }

    public async Task<BasePagingResponseModel<ServiceFeedback>> GetAllPagingAsync(FeedbackFilterQuery query)
    {
        #region Filter

        Expression<Func<ServiceFeedback, bool>>? filter = null;

        if (query.ServiceId.HasValue)
        {
            Expression<Func<ServiceFeedback, bool>> tmp_filter = x => x.ServiceId == query.ServiceId.Value;
            filter = filter != null ? FilterCustoms.CombineFilters(filter, tmp_filter) : tmp_filter;
        }

        if (query.CustomerId.HasValue)
        {
            Expression<Func<ServiceFeedback, bool>> tmp_filter = x => x.CustomerId == query.CustomerId.Value;
            filter = filter != null ? FilterCustoms.CombineFilters(filter, tmp_filter) : tmp_filter;
        }

        if (query.MinRating.HasValue)
        {
            Expression<Func<ServiceFeedback, bool>> tmp_filter = x => x.Rating >= query.MinRating.Value;
            filter = filter != null ? FilterCustoms.CombineFilters(filter, tmp_filter) : tmp_filter;
        }

        if (query.MaxRating.HasValue)
        {
            Expression<Func<ServiceFeedback, bool>> tmp_filter = x => x.Rating <= query.MaxRating.Value;
            filter = filter != null ? FilterCustoms.CombineFilters(filter, tmp_filter) : tmp_filter;
        }

        #endregion

        var (Pagination, Entities) = await _unitOfWork.FeedbackRepository.ToPagination(
            pageIndex: query.Page ?? 0,
            pageSize: query.Limit ?? 10,
            filter: filter,
            searchTerm: query.Q,
            searchFields: ["Comment"],
            sortOrders: query.OrderBy?.ToDictionary(
                    k => k.OrderColumn ?? "FeedbackDate",
                    v => (v.OrderDir ?? "DESC").Equals("ASC", StringComparison.CurrentCultureIgnoreCase)
                ) ?? new Dictionary<string, bool> { { "FeedbackDate", false } },
            includeFunc: x => x
                .Include(f => f.Customer)
                .Include(f => f.Service)
                .Include(f => f.CustomerBooking)
        );
        return BasePagingResponseModel<ServiceFeedback>.CreateInstance(Entities, Pagination);
    }

    public async Task<BasePagingResponseModel<ServiceFeedback>> GetByServiceIdAsync(Guid serviceId, FeedbackFilterQuery query)
    {
        #region Filter

        Expression<Func<ServiceFeedback, bool>> filter = x => x.ServiceId == serviceId;

        if (query.MinRating.HasValue)
        {
            Expression<Func<ServiceFeedback, bool>> tmp_filter = x => x.Rating >= query.MinRating.Value;
            filter = FilterCustoms.CombineFilters(filter, tmp_filter);
        }

        if (query.MaxRating.HasValue)
        {
            Expression<Func<ServiceFeedback, bool>> tmp_filter = x => x.Rating <= query.MaxRating.Value;
            filter = FilterCustoms.CombineFilters(filter, tmp_filter);
        }

        #endregion

        var (Pagination, Entities) = await _unitOfWork.FeedbackRepository.ToPagination(
            pageIndex: query.Page ?? 0,
            pageSize: query.Limit ?? 10,
            filter: filter,
            searchTerm: query.Q,
            searchFields: ["Comment"],
            sortOrders: query.OrderBy?.ToDictionary(
                    k => k.OrderColumn ?? "FeedbackDate",
                    v => (v.OrderDir ?? "DESC").Equals("ASC", StringComparison.CurrentCultureIgnoreCase)
                ) ?? new Dictionary<string, bool> { { "FeedbackDate", false } },
            includeFunc: x => x
                .Include(f => f.Customer)
                .Include(f => f.Service)
                .Include(f => f.CustomerBooking)
        );
        return BasePagingResponseModel<ServiceFeedback>.CreateInstance(Entities, Pagination);
    }
}

