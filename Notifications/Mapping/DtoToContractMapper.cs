using Notifications.Contracts.Data;
using RepairShop.Common.Contracts;

namespace Notifications.Mapping;

public static class DtoToContractNotificationMapper
{
    public static NotificationDto ToNotificationDto(this NotificationContract notificationSended)
    {
        return new NotificationDto(notificationSended.Id, notificationSended.UserId, notificationSended.Date, notificationSended.NotificationType, notificationSended.Content, notificationSended.NotificationSystemType, notificationSended.RoutingData);
    }
}
