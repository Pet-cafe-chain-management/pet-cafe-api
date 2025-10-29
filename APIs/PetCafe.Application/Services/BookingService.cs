using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PetCafe.Application.GlobalExceptionHandling.Exceptions;
using PetCafe.Application.Models.BookingModels;
using PetCafe.Application.Models.ShareModels;
using PetCafe.Application.Utilities;
using PetCafe.Domain.Constants;
using PetCafe.Domain.Entities;

namespace PetCafe.Application.Services;


public interface IBookingService
{
    Task<CustomerBooking> GetByIdAsync(Guid id);
    Task<BasePagingResponseModel<CustomerBooking>> GetAllPagingAsync(BookingFilterQuery query);
    Task<CustomerBooking> UpdateAsync(Guid id, BookingUpdateModel model);
    Task<BasePagingResponseModel<CustomerBooking>> GetAllPagingByCustomerIdAsync(Guid customerId, BookingFilterQuery query);
}


public class BookingService(
    IUnitOfWork _unitOfWork
) : IBookingService
{
    public async Task<CustomerBooking> GetByIdAsync(Guid id)
    {
        return await _unitOfWork
            .BookingRepository
            .GetByIdAsync(id,
                includeFunc: x => x
                    .Include(x => x.Customer)
                    .Include(x => x.Service)
                    .Include(x => x.Slot)
                    .Include(x => x.Team)
        ) ?? throw new BadRequestException("Không tìm thấy thông tin!");
    }

    public async Task<BasePagingResponseModel<CustomerBooking>> GetAllPagingAsync(BookingFilterQuery query)
    {

        Expression<Func<CustomerBooking, bool>> filter = x => true;
        if (query.CustomerId.HasValue)
        {
            filter = filter != null ? FilterCustoms.CombineFilters(filter, x => x.CustomerId == query.CustomerId) : x => x.CustomerId == query.CustomerId;
        }
        if (query.ServiceId.HasValue)
        {
            filter = filter != null ? FilterCustoms.CombineFilters(filter, x => x.ServiceId == query.ServiceId) : x => x.ServiceId == query.ServiceId;
        }
        if (query.TeamId.HasValue)
        {
            filter = filter != null ? FilterCustoms.CombineFilters(filter, x => x.TeamId == query.TeamId) : x => x.TeamId == query.TeamId;
        }

        if (!string.IsNullOrEmpty(query.BookingStatus))
        {
            filter = filter != null ? FilterCustoms.CombineFilters(filter, x => x.BookingStatus == query.BookingStatus) : x => x.BookingStatus == query.BookingStatus;
        }
        if (query.FromDate.HasValue)
        {
            filter = filter != null ? FilterCustoms.CombineFilters(filter, x => x.BookingDate >= query.FromDate) : x => x.BookingDate >= query.FromDate;
        }
        if (query.ToDate.HasValue)
        {
            filter = filter != null ? FilterCustoms.CombineFilters(filter, x => x.BookingDate <= query.ToDate) : x => x.BookingDate <= query.ToDate;
        }
        if (query.FeedbackRating.HasValue)
        {
            filter = filter != null ? FilterCustoms.CombineFilters(filter, x => x.FeedbackRating == query.FeedbackRating) : x => x.FeedbackRating == query.FeedbackRating;
        }

        var (Pagination, Entities) = await _unitOfWork.BookingRepository.ToPagination(
            pageIndex: query.Page ?? 0,
            pageSize: query.Limit ?? 10,
            filter: filter,
            includeFunc: x => x.Include(x => x.Customer).Include(x => x.Service).Include(x => x.Slot).Include(x => x.Team)
        );

        return BasePagingResponseModel<CustomerBooking>.CreateInstance(Entities, Pagination);
    }

    public async Task<BasePagingResponseModel<CustomerBooking>> GetAllPagingByCustomerIdAsync(Guid customerId, BookingFilterQuery query)
    {
        Expression<Func<CustomerBooking, bool>> filter = x => x.CustomerId == customerId;

        if (!string.IsNullOrEmpty(query.BookingStatus))
        {
            filter = filter != null ? FilterCustoms.CombineFilters(filter, x => x.BookingStatus == query.BookingStatus) : x => x.BookingStatus == query.BookingStatus;
        }
        if (query.FromDate.HasValue)
        {
            filter = filter != null ? FilterCustoms.CombineFilters(filter, x => x.BookingDate >= query.FromDate) : x => x.BookingDate >= query.FromDate;
        }
        if (query.ToDate.HasValue)
        {
            filter = filter != null ? FilterCustoms.CombineFilters(filter, x => x.BookingDate <= query.ToDate) : x => x.BookingDate <= query.ToDate;
        }
        if (query.FeedbackRating.HasValue)
        {
            filter = filter != null ? FilterCustoms.CombineFilters(filter, x => x.FeedbackRating == query.FeedbackRating) : x => x.FeedbackRating == query.FeedbackRating;
        }

        var (Pagination, Entities) = await _unitOfWork.BookingRepository.ToPagination(
            pageIndex: query.Page ?? 0,
            pageSize: query.Limit ?? 10,
            filter: filter,
            includeFunc: x => x.Include(x => x.Customer).Include(x => x.Service).Include(x => x.Slot).Include(x => x.Team)
        );

        return BasePagingResponseModel<CustomerBooking>.CreateInstance(Entities, Pagination);
    }

    public async Task<CustomerBooking> UpdateAsync(Guid id, BookingUpdateModel model)
    {
        var booking = await _unitOfWork.BookingRepository.GetByIdAsync(id) ?? throw new BadRequestException("Không tìm thấy thông tin!");

        booking.Notes = model.Notes;

        if (model.BookingStatus == BookingStatusConstant.CONFIRMED && booking.BookingStatus == BookingStatusConstant.PENDING)
        {
            booking.BookingStatus = BookingStatusConstant.CONFIRMED;
        }

        if (model.BookingStatus == BookingStatusConstant.IN_PROGRESS && booking.BookingStatus == BookingStatusConstant.CONFIRMED)
        {
            booking.BookingStatus = BookingStatusConstant.IN_PROGRESS;
        }

        if (model.BookingStatus == BookingStatusConstant.CANCELLED && booking.BookingStatus != BookingStatusConstant.CANCELLED)
        {
            booking.CancelDate = DateTime.UtcNow;
            booking.CancelReason = model.CancelReason;
        }

        if (model.BookingStatus == BookingStatusConstant.COMPLETED && booking.BookingStatus == BookingStatusConstant.IN_PROGRESS)
        {
            booking.BookingStatus = BookingStatusConstant.COMPLETED;
        }

        if (booking.BookingStatus == BookingStatusConstant.COMPLETED && (!string.IsNullOrEmpty(booking.FeedbackComment) || model.FeedbackRating.HasValue))
        {
            booking.FeedbackRating = model.FeedbackRating;
            booking.FeedbackComment = model.FeedbackComment;
            booking.FeedbackDate = DateTime.UtcNow;
        }

        _unitOfWork.BookingRepository.Update(booking);
        await _unitOfWork.SaveChangesAsync();
        return booking;
    }
}