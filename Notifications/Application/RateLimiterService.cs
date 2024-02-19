using Microsoft.Extensions.Logging;
using Notifications.Contracts.Data;
using Notifications.Domain;
using Notifications.Domain.Enums;
using Notifications.Domain.Responses;
using Notifications.Domain.Services;
using RepairShop.Infrastructure;

namespace Notifications.Application
{
    public class RateLimiterService : IRateLimiterService
    {
        private readonly ICacheRepository<RuleDto> _rulesRepository;
        private readonly IRepository<UserNotificationsHistoryDto> _userNotificationsHistoryRepo;
        private readonly ILogger<RateLimiterService> _logger;

        public RateLimiterService(ICacheRepository<RuleDto> rulesRepository,
            IRepository<UserNotificationsHistoryDto> userNotificationsHistoryRepo,
            ILogger<RateLimiterService> logger)
        {
            _rulesRepository = rulesRepository;
            _userNotificationsHistoryRepo = userNotificationsHistoryRepo;
            _logger = logger;
        }

        public async Task<CheckRateResponse> CheckNotification(Notification notification)
        {

            UserNotificationsHistoryDto? userNHistory = await _userNotificationsHistoryRepo.GetAsync(notification.UserId);
            IEnumerable<RuleDto> rules = await _rulesRepository.GetAllListAsync();

            var result = CheckRates(notification, userNHistory, rules);

            if (result.Success)
            {
                await UpdateUserHistory(notification, userNHistory);
            }

            _logger.LogInformation($"Check rate result Success = [{result.Success}], Reason = [{result.Reason}], NotificationId = [{notification.Id}], UserId = [{notification.UserId}], NotificationType = [{notification.NotificationType}]");
            return result;
        }

        private CheckRateResponse CheckRates(Notification notification, UserNotificationsHistoryDto? userNHistory, IEnumerable<RuleDto> rules)
        {
            if (userNHistory is null || !userNHistory.HistoryOfNotificationsSent.ContainsKey(notification.NotificationType.ToString()))
            {
                _logger.LogInformation($"No notifications history found by UserId. NotificationId = [{notification.Id}], UserId = [{notification.UserId}], NotificationType = [{notification.NotificationType}]");
                return new(true);
            }

            var relevantRules = rules.Where(rule => RuleIsRelevant(rule, notification));
            if (!relevantRules.Any())
            {
                _logger.LogInformation($"No relevant rules found by UserId. NotificationId = [{notification.Id}], UserId = [{notification.UserId}], NotificationType = [{notification.NotificationType}]");
                return new(true);
            }

            List<DateTimeOffset> relevantDates = userNHistory.HistoryOfNotificationsSent[notification.NotificationType.ToString()];

            if (!relevantDates.Any())
            {
                _logger.LogInformation($"No notifications history found by NotificationType. NotificationId = [{notification.Id}], UserId = [{notification.UserId}], NotificationType = [{notification.NotificationType}]");
                return new(true);
            }

            _logger.LogInformation($"Starting to perfom check on RulesIds = [{string.Join(',', relevantRules.Select(r => r.Id))}]. NotificationId = [{notification.Id}], UserId = [{notification.UserId}], NotificationType = [{notification.NotificationType}]");
            foreach (var rule in relevantRules)
            {
                DateTimeOffset dateStartOffset = DateTimeOffset.Now - TimeSpan.FromMinutes(rule.AllowedTimeIntervalInMinutes);
                int amountOfTransactionsOnInterval = relevantDates.Where(d => dateStartOffset < d).Count();
                if (amountOfTransactionsOnInterval >= rule.Rate)
                {
                    return new(false, new Reason(rule.Id, rule.Rate, rule.AllowedTimeIntervalInMinutes, amountOfTransactionsOnInterval));
                }
            }

            return new(true);
        }
        private async Task UpdateUserHistory(Notification notification, UserNotificationsHistoryDto? userNHistory)
        {
            if (userNHistory is null)
                await CreateUserNotificationHistory(notification);
            else
                await UpdateUserNotificationHistory(notification, userNHistory);
        }
        private async Task CreateUserNotificationHistory(Notification notification)
        {
            var userNHistory = new UserNotificationsHistoryDto(notification.UserId);
            var history = new List<DateTimeOffset>() { notification.Date };
            userNHistory.HistoryOfNotificationsSent[notification.NotificationType.ToString()] = history;
            await _userNotificationsHistoryRepo.CreateAsync(userNHistory);
        }
        private async Task UpdateUserNotificationHistory(Notification notification, UserNotificationsHistoryDto userNHistory)
        {
            string notificationTypeKey = notification.NotificationType.ToString();

            if (userNHistory.HistoryOfNotificationsSent.ContainsKey(notificationTypeKey))
            {
                userNHistory.HistoryOfNotificationsSent[notificationTypeKey].Add(notification.Date);
            }
            else
            {
                userNHistory.HistoryOfNotificationsSent[notificationTypeKey] = new() { notification.Date };
            }

            await _userNotificationsHistoryRepo.UpdateAsync(userNHistory);
        }
        public bool RuleIsRelevant(RuleDto rule, Notification notification) =>
            rule.NotificationTypes.Contains(notification.NotificationType) &&
            CheckIfRuleIsRequiredForCurrentNotification(rule, notification);
        private bool CheckIfRuleIsRequiredForCurrentNotification(RuleDto rule, Notification notification)
            => rule.RuleType is RuleType.Global || (rule.RuleType is RuleType.Individual && rule.UserId == notification.UserId);
    }
}
