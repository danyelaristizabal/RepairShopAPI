using RepairShop.Infrastructure;

namespace Notifications.Contracts.Data
{
    public class DumpedNotificationDto : IEntity
    {
        public Guid Id
        {
            get => NotificationDto.Id;
            init => _ = value;
        }
        public NotificationDto NotificationDto { get; init; }
        public string DumpReason { get; init; }

        public DumpedNotificationDto(NotificationDto notificationDto, string dumpReason)
        {
            NotificationDto = notificationDto;
            DumpReason = dumpReason;
        }
    }
}
