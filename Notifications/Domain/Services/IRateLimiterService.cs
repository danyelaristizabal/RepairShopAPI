using Notifications.Domain.Responses;

namespace Notifications.Domain.Services
{
    public interface IRateLimiterService
    {
        Task<CheckRateResponse> CheckNotification(Notification notification);
    }
}
