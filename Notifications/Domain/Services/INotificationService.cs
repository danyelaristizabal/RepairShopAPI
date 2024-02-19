namespace Notifications.Domain.Services
{
    public interface INotificationService
    {
        Task ProcessNotification(Notification notification);
    }
}
