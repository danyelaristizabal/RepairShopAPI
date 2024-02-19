using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Notifications.Application;
using Notifications.Application.Services;
using Notifications.Contracts.Data;
using Notifications.Domain.Services;
using Notifications.Infrastructure.MessageBus.Consumers;
using RedLockNet;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using RepairShop.Common.Contracts;
using RepairShop.Common.Infrastructure.Database.MongoDB;
using RepairShop.Common.Infrastructure.Settings;
using RepairShop.Infrastructure.MassTransit;
using System.Net;

namespace Notifications.Infrastructure.Extensions
{
    public static class NotificationsBuilderExtensions
    {
        public static void ConfigureNotificationsModuleIfNeeded(this WebApplicationBuilder builder,
            List<Action<ConfigurationManager, IRiderRegistrationConfigurator>> messageBusActors,
            List<Action<ConfigurationManager, IRiderRegistrationContext, IKafkaFactoryConfigurator>> addTopics)
        {
            if (!builder.Configuration.GetValue<bool>($"HostSettings:{typeof(NotificationsBuilderExtensions).Assembly.GetName().Name}")) return;

            ConfigureControllers(builder);
            ConfigureDistributedLocking(builder);
            ConfigureDatabase(builder);
            messageBusActors.Add(AddMessageBusActors);
            addTopics.Add(ConfigureMessageBusActors);
        }
        private static void ConfigureControllers(WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IRateLimiterService, RateLimiterService>();
            builder.Services.AddScoped<INotificationService, NotificationService>();
            builder.Services.AddScoped<IRuleService, RulesService>();
            builder.Services.AddControllers().AddApplicationPart(typeof(NotificationsBuilderExtensions).Assembly);
        }
        private static void ConfigureDistributedLocking(WebApplicationBuilder builder)
        {
            var redisSettings = builder.Configuration.GetSection(nameof(RedisSettings)).Get<RedisSettings>();

            if (redisSettings is null ||
                string.IsNullOrWhiteSpace(redisSettings.ConnectionString) ||
               !redisSettings.Port.HasValue ||
                string.IsNullOrEmpty(redisSettings.Host)) return;

            string host = redisSettings.Host;
            int port = redisSettings.Port.Value;

            var endPoints = new List<RedLockEndPoint>
                {
                    new DnsEndPoint(host, port),
                };

            var redLockFactory = RedLockFactory.Create(endPoints);
            builder.Services.AddSingleton<IDistributedLockFactory>(redLockFactory);
        }
        private static void ConfigureDatabase(WebApplicationBuilder builder)
        {
            string assemblyName = typeof(NotificationsBuilderExtensions).Assembly.GetName().Name ?? string.Empty;
            const string RulesCollectionName = "rules";
            builder.Services.AddMongoRepository<RuleDto>(RulesCollectionName, assemblyName);
            const string RejectedNotifications = "rejectednotitifications";
            builder.Services.AddMongoRepository<RejectedNotificationDto>(RejectedNotifications, assemblyName);
            const string FailedToProcessDumpCollectionName = "dumpednotifications";
            builder.Services.AddMongoRepository<DumpedNotificationDto>(FailedToProcessDumpCollectionName, assemblyName);
            const string UsersHistoryCollectionName = "usershistory";
            builder.Services.AddMongoRepository<UserNotificationsHistoryDto>(UsersHistoryCollectionName, assemblyName);
            builder.Services.AddSingleton<ICacheRepository<RuleDto>, CacheRepository<RuleDto>>();
        }
        private static void AddMessageBusActors(ConfigurationManager config, IRiderRegistrationConfigurator rider)
        {
            string assemblyName = typeof(NotificationsBuilderExtensions).Assembly.GetName().Name;

            var messageBusSettings = config.GetSection(nameof(MessagebusSettings)).Get<MessagebusSettings[]>();

            var messageBusSetting = messageBusSettings?.FirstOrDefault(x => x.AssemblyName == typeof(NotificationsBuilderExtensions).Assembly.GetName().Name);

            if (messageBusSetting?.Consumers?.Count > 0)
            {
                if (messageBusSetting.Consumers.ContainsKey(nameof(NotificationsConsumer))
                    && messageBusSettings!.CanRegisterKey(assemblyName!, nameof(NotificationsConsumer)))
                    rider.AddConsumer<NotificationsConsumer>();
            }
        }
        private static void ConfigureMessageBusActors(ConfigurationManager config, IRiderRegistrationContext context, IKafkaFactoryConfigurator rider)
        {
            string assemblyName = typeof(NotificationsBuilderExtensions).Assembly.GetName().Name;
            var messageBusSettings = config.GetSection(nameof(MessagebusSettings)).Get<MessagebusSettings[]>();
            var messageBusSetting = messageBusSettings?.FirstOrDefault(x => x.AssemblyName == assemblyName);

            if (messageBusSetting?.Consumers?.Count > 0)
            {
                if (messageBusSetting.Consumers.TryGetValue(nameof(NotificationsConsumer), out string? Topic) &&
                    !string.IsNullOrEmpty(Topic) && messageBusSettings!.CanRegisterKey(assemblyName!, nameof(NotificationsConsumer)))
                    rider.TopicEndpoint<NotificationContract>(Topic, messageBusSetting.Group, e =>
                    {
                        e.ConfigureConsumer<NotificationsConsumer>(context);
                    });
            }
        }
    }
}
