using Notifications.Domain.Enums;
using RepairShop.Common.Domain.Enums;

namespace Notifications.Contracts.Responses
{
    public record RuleResponse(Guid Id, IEnumerable<NotificationType> NotificationTypes, int AllowedTimeSpanBetweenNotifications, int Rate, RuleType RuleType, Guid? UserId = null);
}
