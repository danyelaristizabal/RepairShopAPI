using Microsoft.Extensions.Logging;
using Notifications.Contracts.Data;
using Notifications.Domain;
using Notifications.Domain.Services;
using Notifications.Infrastructure.MessageBus.Consumers;
using Notifications.Mapping;
using RedLockNet;
using RepairShop.Common.Domain.Enums;
using RepairShop.Infrastructure;
using System.Text.Json;

namespace Notifications.Application
{
    public class NotificationService : INotificationService
    {
        private readonly IDistributedLockFactory _distributedLockFactory;
        private readonly IRateLimiterService _rateCheckerService;
        private readonly IRepository<RejectedNotificationDto> _dumpedNotificationsRepository;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(IDistributedLockFactory distributedLockFactory,
            IRateLimiterService rateCheckerService,
            IRepository<RejectedNotificationDto> dumpedNotificationsRepository,
            ILogger<NotificationService> logger)
        {
            _distributedLockFactory = distributedLockFactory;
            _rateCheckerService = rateCheckerService;
            _dumpedNotificationsRepository = dumpedNotificationsRepository;
            _logger = logger;
        }

        public async Task ProcessNotification(Notification notification)
        {
            _logger.LogInformation($"Starting to process Notification. NotificationId = [{notification.Id}], UserId = [{notification.UserId}], NotificationType = [{notification.NotificationType}]");
            var wait = TimeSpan.FromSeconds(120);
            var retry = TimeSpan.FromSeconds(1);

            await using (var redLock = await _distributedLockFactory.CreateLockAsync($"{nameof(NotificationsConsumer)}_{notification.UserId}", TimeSpan.FromMinutes(1), wait, retry))
            {
                if (redLock.IsAcquired)
                {
                    await SendNotification(notification);
                }
            }
        }
        private async Task SendNotification(Notification notification)
        {
            var checkResult = await _rateCheckerService.CheckNotification(notification);
            if (checkResult.Success)
            {
                await PassToTransportLayer(notification);
                _logger.LogInformation($"Notification with Id = [{notification.Id}] was successfully sent by [{notification.NotificationTransportType}] for UserId = [{notification.UserId}]  Notification = [{JsonSerializer.Serialize(notification)}]");
                return;
            }

            await _dumpedNotificationsRepository.CreateAsync(new RejectedNotificationDto(notification.Id, notification.ToNotificationDto(), checkResult.Reason!));
            _logger.LogInformation($"Notification with Id = [{notification.Id}] was successfully dumped.");
        }
        private Task PassToTransportLayer(Notification notification)
        {

            switch (notification.NotificationTransportType)
            {
                case NotificationSystemType.Email:
                    // Implement logic to send email
                    break;
                case NotificationSystemType.SMS:
                    // Implement logic to send SMS
                    break;
                default:
                    break;
            }
            return Task.CompletedTask;
        }
    }
}
