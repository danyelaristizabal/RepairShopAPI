using MassTransit;
using Microsoft.Extensions.Logging;
using Notifications.Contracts.Data;
using Notifications.Domain.Services;
using Notifications.Mapping;
using RepairShop.Common.Contracts;
using RepairShop.Infrastructure;
using System.Text.Json;

namespace Notifications.Infrastructure.MessageBus.Consumers
{
    public class NotificationsConsumer : IConsumer<NotificationContract>
    {
        private readonly INotificationService _notificationService;
        private readonly IRepository<DumpedNotificationDto> _failedToProcessNotificationsDumpRepository;
        private readonly ILogger<NotificationsConsumer> _logger;

        public NotificationsConsumer(INotificationService notificationService,

            IRepository<DumpedNotificationDto> dumpedNotificationsRepository,
            ILogger<NotificationsConsumer> logger)
        {
            _notificationService = notificationService;

            _failedToProcessNotificationsDumpRepository = dumpedNotificationsRepository;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<NotificationContract> context)
        {
            try
            {
                await _notificationService.ProcessNotification(context.Message.ToNotification());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An unexpected exception was thrown while consuming notification: {JsonSerializer.Serialize(context)}, sending notification to Dump.");
                await _failedToProcessNotificationsDumpRepository.CreateAsync(new DumpedNotificationDto(context.Message.ToNotificationDto(), ex.GetType().Name));
            }

        }
    }
}
