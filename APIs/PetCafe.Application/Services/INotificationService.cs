using Microsoft.AspNetCore.SignalR;
using PetCafe.Application.Hubs;
using PetCafe.Application.Services.Commons;
using PetCafe.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace PetCafe.Application.Services;

public interface INotificationService
{
    Task<Notification> SendNotificationAsync(
        Guid accountId,
        string title,
        string message,
        string notificationType,
        string priority = "Normal",
        Guid? referenceId = null,
        string? referenceType = null,
        DateTime? scheduledSendDate = null
    );

    Task SendNotificationToUserAsync(Guid accountId, Notification notification);
    Task SendNotificationToMultipleUsersAsync(List<Guid> accountIds, Notification notification);
}


public class NotificationService(
    IUnitOfWork _unitOfWork,
    IHubContext<NotificationHub> _hubContext,
    ICurrentTime _currentTime
) : INotificationService
{
    public async Task<Notification> SendNotificationAsync(
        Guid accountId,
        string title,
        string message,
        string notificationType,
        string priority = "Normal",
        Guid? referenceId = null,
        string? referenceType = null,
        DateTime? scheduledSendDate = null
    )
    {
        var notification = new Notification
        {
            AccountId = accountId,
            Title = title,
            Message = message,
            NotificationType = notificationType,
            Priority = priority,
            ReferenceId = referenceId,
            ReferenceType = referenceType,
            ScheduledSendDate = scheduledSendDate,
            IsRead = false
        };

        // Nếu có scheduledSendDate và nó trong tương lai, chỉ lưu vào DB
        // Nếu không có hoặc đã đến thời điểm, gửi ngay
        if (scheduledSendDate == null || scheduledSendDate <= _currentTime.GetCurrentTime)
        {
            notification.SentDate = _currentTime.GetCurrentTime;
            await _unitOfWork.NotificationRepository.AddAsync(notification);
            await _unitOfWork.SaveChangesAsync();

            // Gửi realtime notification
            await SendNotificationToUserAsync(accountId, notification);
        }
        else
        {
            // Chỉ lưu vào DB, sẽ được gửi bởi background job
            await _unitOfWork.NotificationRepository.AddAsync(notification);
            await _unitOfWork.SaveChangesAsync();
        }

        return notification;
    }

    public async Task SendNotificationToUserAsync(Guid accountId, Notification notification)
    {
        try
        {
            await _hubContext.Clients
                .Group($"user_{accountId}")
                .SendAsync("ReceiveNotification", new
                {
                    id = notification.Id,
                    accountId = notification.AccountId,
                    title = notification.Title,
                    message = notification.Message,
                    notificationType = notification.NotificationType,
                    priority = notification.Priority,
                    isRead = notification.IsRead,
                    readDate = notification.ReadDate,
                    referenceId = notification.ReferenceId,
                    referenceType = notification.ReferenceType,
                    sentDate = notification.SentDate,
                    createdAt = notification.CreatedAt
                });
        }
        catch (Exception ex)
        {
            // Log error nhưng không throw để không ảnh hưởng đến business logic
            Console.WriteLine($"Error sending notification to user {accountId}: {ex.Message}");
        }
    }

    public async Task SendNotificationToMultipleUsersAsync(List<Guid> accountIds, Notification notification)
    {
        var tasks = accountIds.Select(accountId => SendNotificationToUserAsync(accountId, notification));
        await Task.WhenAll(tasks);
    }
}


