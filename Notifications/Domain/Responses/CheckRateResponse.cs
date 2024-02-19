namespace Notifications.Domain.Responses
{
    public record CheckRateResponse(bool Success, Reason? Reason = null);
    public record Reason(Guid RuleId, int Rate, int AllowedTimeIntervalInMinutes, int CurrentCount);
}
