using RepairShop.Common.Domain.Enums;

namespace Notifications.Domain
{
    public class Notification
    {
        public Guid Id { get; init; }
        public Guid UserId { get; init; }
        public DateTimeOffset Date { get; init; }
        public NotificationType NotificationType { get; init; }
        public string Content { get; set; }
        public NotificationSystemType NotificationTransportType { get; init; }
        public string RoutingData { get; init; }

        public Notification(Guid id, Guid userId, DateTimeOffset date, NotificationType notificationType, string content, NotificationSystemType notificationSystemType, string routingData)
        {
            Id = id;
            UserId = userId;
            Date = date;
            NotificationType = notificationType;
            Content = content;
            NotificationTransportType = notificationSystemType;
            RoutingData = routingData;
        }
    }

}
