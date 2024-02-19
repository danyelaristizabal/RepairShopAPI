using Notifications.Domain.Enums;
using RepairShop.Common.Domain.Enums;
using RepairShop.Infrastructure;

namespace Notifications.Contracts.Data
{
    public class RuleDto : IEntity
    {
        public Guid Id { get; init; }
        public Guid? UserId { get; set; }
        public IEnumerable<NotificationType> NotificationTypes { get; init; }
        public int AllowedTimeIntervalInMinutes { get; init; }
        public int Rate { get; init; }
        public RuleType RuleType { get; init; }
        public RuleDto(Guid id, IEnumerable<NotificationType> notificationTypes, int allowedTimeIntervalInMinutes, int rate, RuleType ruleType, Guid? userId = null)
        {
            Id = id;
            UserId = userId;
            NotificationTypes = notificationTypes;
            AllowedTimeIntervalInMinutes = allowedTimeIntervalInMinutes;
            Rate = rate;
            RuleType = ruleType;
        }
    }
}
