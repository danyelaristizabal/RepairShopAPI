using RepairShop.Common.Domain.Enums;

namespace RepairShop.Common.Contracts
{
    public record NotificationContract(Guid Id, Guid UserId, DateTimeOffset Date, NotificationType NotificationType, string Content, NotificationSystemType NotificationSystemType, string RoutingData);
}
