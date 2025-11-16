using Microsoft.EntityFrameworkCore;
using PetCafe.Application.Repositories;
using PetCafe.Application.Services.Commons;
using PetCafe.Application.Utilities;
using PetCafe.Domain.Constants;
using Task = System.Threading.Tasks.Task;

namespace PetCafe.Application.Services;

public interface IBookingNotificationService
{
    Task CheckAndSendBookingNotificationsAsync();
}

public class BookingNotificationService(
    IUnitOfWork _unitOfWork,
    INotificationService _notificationService,
    ICurrentTime _currentTime
) : IBookingNotificationService
{
    public async Task CheckAndSendBookingNotificationsAsync()
    {
        var now = _currentTime.GetCurrentTime;
        var today = now.Date;
        var tomorrow = today.AddDays(1);

        // Lấy các booking có ngày booking là hôm nay hoặc ngày mai
        // và chưa được gửi notification (hoặc có thể check bằng cách khác)
        var upcomingBookings = await _unitOfWork.BookingRepository
            .WhereAsync(
                x => x.BookingDate.Date >= today &&
                     x.BookingDate.Date <= tomorrow &&
                     x.BookingStatus == BookingStatusConstant.PENDING &&
                     !x.IsDeleted,
                includeFunc: x => x
                    .Include(x => x.Customer!)
                        .ThenInclude(x => x.Account)
                    .Include(x => x.Service)
            );

        foreach (var booking in upcomingBookings)
        {
            // Kiểm tra nếu booking có customer và customer có accountId
            if (booking.CustomerId == null || booking.Customer?.AccountId == null) continue;

            var accountId = booking.Customer.AccountId;
            var bookingDate = booking.BookingDate.Date;
            var isToday = bookingDate == today;
            var isTomorrow = bookingDate == tomorrow;

            // Gửi notification cho booking hôm nay (vào buổi sáng, ví dụ 8:00)
            if (isToday && now.Hour >= 8 && now.Hour < 9)
            {
                var timeString = $"{booking.StartTime:hh\\:mm} - {booking.EndTime:hh\\:mm}";
                await _notificationService.SendNotificationAsync(
                    accountId: accountId,
                    title: "Nhắc nhở lịch hẹn hôm nay",
                    message: $"Bạn có lịch hẹn {booking.Service?.Name ?? "dịch vụ"} vào {timeString} hôm nay. Vui lòng đến đúng giờ!",
                    notificationType: "Booking",
                    priority: "High",
                    referenceId: booking.Id,
                    referenceType: "CustomerBooking"
                );
            }
            // Gửi notification cho booking ngày mai (vào buổi tối, ví dụ 20:00)
            else if (isTomorrow && now.Hour >= 20)
            {
                var timeString = $"{booking.StartTime:hh\\:mm} - {booking.EndTime:hh\\:mm}";
                await _notificationService.SendNotificationAsync(
                    accountId: accountId,
                    title: "Nhắc nhở lịch hẹn ngày mai",
                    message: $"Bạn có lịch hẹn {booking.Service?.Name ?? "dịch vụ"} vào {timeString} ngày mai. Vui lòng chuẩn bị!",
                    notificationType: "Booking",
                    priority: "Normal",
                    referenceId: booking.Id,
                    referenceType: "CustomerBooking"
                );
            }
        }
    }
}

