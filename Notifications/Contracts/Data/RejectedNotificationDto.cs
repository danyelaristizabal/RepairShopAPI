using Notifications.Domain.Responses;
using RepairShop.Infrastructure;

namespace Notifications.Contracts.Data
{
    public class RejectedNotificationDto : IEntity
    {
        public Guid Id { get; init; }
        public NotificationDto NotificationDto { get; init; }
        public Reason Reason { get; init; }
        public RejectedNotificationDto(Guid id, NotificationDto notificationDto, Reason reason)
        {
            Id = id;
            NotificationDto = notificationDto;
            Reason = reason;
        }
    }
}
