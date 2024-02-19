using Notifications.Domain.Enums;
using RepairShop.Common.Domain.Enums;

namespace Notifications.Domain
{
    public record Rule(Guid Id, IEnumerable<NotificationType> NotificationTypes, int AllowedTimeIntervalInMinutes, int Rate, RuleType RuleType, Guid? UserId = null);
}
