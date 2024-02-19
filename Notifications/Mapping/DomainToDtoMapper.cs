using Notifications.Contracts.Data;
using Notifications.Domain;

namespace Notifications.Mapping;

public static class DomainToDtoMapper
{
    public static NotificationDto ToNotificationDto(this Notification notification)
    {
        return new NotificationDto(notification.Id, notification.UserId, notification.Date, notification.NotificationType, notification.Content, notification.NotificationTransportType, notification.RoutingData);
    }
    public static RuleDto ToRuleDto(this Rule rule)
    {
        return new RuleDto(rule.Id, rule.NotificationTypes, rule.AllowedTimeIntervalInMinutes, rule.Rate, rule.RuleType, rule.UserId);
    }
    public static Rule ToRule(this RuleDto ruleDto)
    {
        return new Rule(ruleDto.Id, ruleDto.NotificationTypes, ruleDto.AllowedTimeIntervalInMinutes, ruleDto.Rate, ruleDto.RuleType, ruleDto.UserId);
    }
}
