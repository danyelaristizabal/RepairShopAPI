using Notifications.Contracts.Requests;
using Notifications.Contracts.Responses;
using Notifications.Domain;
using RepairShop.Common.Contracts;

namespace Notifications.Mapping;

public static class DomainToContractMapper
{
    public static Notification ToNotification(this NotificationContract notificationSended)
    {
        return new Notification(notificationSended.Id, notificationSended.UserId, notificationSended.Date, notificationSended.NotificationType, notificationSended.Content, notificationSended.NotificationSystemType, notificationSended.RoutingData);
    }
    public static RuleResponse ToRuleResponse(this Rule rule)
    {
        return new RuleResponse(rule.Id, rule.NotificationTypes, rule.AllowedTimeIntervalInMinutes, rule.Rate, rule.RuleType, rule.UserId);
    }
    public static Rule ToRule(this RuleRequest request)
    {
        return new Rule(Guid.NewGuid(), request.NotificationTypes, request.AllowedTimeIntervalInMinutes, request.Rate, request.RuleType, request.UserId);
    }
    public static Rule ToRule(this UpdateRuleRequest request)
    {
        return new Rule(request.Id, request.Rule.NotificationTypes, request.Rule.AllowedTimeIntervalInMinutes, request.Rule.Rate, request.Rule.RuleType, request.Rule.UserId);
    }
}
