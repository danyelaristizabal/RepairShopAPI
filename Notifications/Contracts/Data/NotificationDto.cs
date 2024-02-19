using RepairShop.Common.Domain.Enums;
using RepairShop.Infrastructure;

namespace Notifications.Contracts.Data
{
    public class NotificationDto : IEntity
    {
        public Guid Id { get; init; }
        public Guid UserId { get; init; }
        public DateTimeOffset Date { get; init; }
        public NotificationType NotificationType { get; init; }
        public string Content { get; set; }
        public NotificationSystemType NotificationSystemType { get; init; }
        public string RoutingData { get; init; }

        public NotificationDto(Guid id, Guid userId, DateTimeOffset date, NotificationType notificationType, string content, NotificationSystemType notificationSystemType, string routingData)
        {
            Id = id;
            UserId = userId;
            Date = date;
            NotificationType = notificationType;
            Content = content;
            NotificationSystemType = notificationSystemType;
            RoutingData = routingData;
        }
    }
}
