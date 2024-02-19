using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RepairShop.Common.Contracts;
using RepairShop.Common.Infrastructure.Database.MongoDB;
using RepairShop.Common.Infrastructure.Settings;
using RepairShop.Infrastructure.MassTransit;
using Tickets.Contracts.Data;
using Tickets.Domain.Services;
using Tickets.Infrastructure.MessaegBus.Consumers;
using Tickets.Infrastructure.MessageBus.Consumers;
using Tickets.Services;

namespace Tickets.Infrastructure.Extensions
{
    public static class TicketsBuilderExtensions
    {
        public static void ConfigureTicketsModuleIfNeeded(this WebApplicationBuilder builder, List<Action<ConfigurationManager, IRiderRegistrationConfigurator>> messageBusActors, List<Action<ConfigurationManager, IRiderRegistrationContext, IKafkaFactoryConfigurator>> addTopics)
        {
            if (!builder.Configuration.GetValue<bool>("HostSettings:Tickets")) return;

            ConfigureControllers(builder);
            ConfigureDatabase(builder);

            messageBusActors.Add(AddMessageBusActors);
            addTopics.Add(ConfigureMessageBusActors);
        }
        private static void ConfigureControllers(WebApplicationBuilder builder)
        {
            var profileModuleAssembly = typeof(TicketsBuilderExtensions).Assembly;
            builder.Services.AddScoped<ITicketsService, TicketsService>();
            builder.Services.AddControllers().AddApplicationPart(profileModuleAssembly);
        }
        private static void ConfigureDatabase(WebApplicationBuilder builder)
        {
            const string ticketsName = "tickets";
            builder.Services.AddMongoRepository<TicketDto>(ticketsName, typeof(TicketsBuilderExtensions).Assembly.GetName().Name ?? string.Empty);
            const string usersCollectionName = "users";
            builder.Services.AddMongoRepository<UserDto>(usersCollectionName, typeof(TicketsBuilderExtensions).Assembly.GetName().Name ?? string.Empty);
        }
        private static void AddMessageBusActors(ConfigurationManager config, IRiderRegistrationConfigurator rider)
        {
            const string publisherPostfix = "Publisher";
            string assemblyName = typeof(TicketsBuilderExtensions).Assembly.GetName().Name;

            var messageBusSettings = config.GetSection(nameof(MessagebusSettings)).Get<MessagebusSettings[]>();
            var messageBusSetting = messageBusSettings?.FirstOrDefault(x => x.AssemblyName == assemblyName);

            if (messageBusSetting is not null && messageBusSetting.Consumers?.Count > 0)
            {
                if (messageBusSetting.Consumers.ContainsKey(nameof(UserCreatedConsumer))
                    && messageBusSettings!.CanRegisterKey(assemblyName!, nameof(UserCreatedConsumer)))
                    rider.AddConsumer<UserCreatedConsumer>();

                if (messageBusSetting.Consumers.ContainsKey(nameof(UserDeletedConsumer))
                    && messageBusSettings!.CanRegisterKey(assemblyName!, nameof(UserDeletedConsumer)))
                    rider.AddConsumer<UserDeletedConsumer>();

                if (messageBusSetting.Consumers.ContainsKey(nameof(UserUpdatedConsumer))
                    && messageBusSettings!.CanRegisterKey(assemblyName!, nameof(UserUpdatedConsumer)))
                    rider.AddConsumer<UserUpdatedConsumer>();
            }

            if (messageBusSetting?.Publishers?.Count > 0)
            {
                if (messageBusSetting.Publishers.TryGetValue(nameof(NotificationContract) + publisherPostfix, out string? publishTopic)
                    && !string.IsNullOrEmpty(publishTopic)
                    && messageBusSettings!.CanRegisterKey(assemblyName!, nameof(NotificationContract) + publisherPostfix))
                    rider.AddProducer<NotificationContract>(publishTopic);
            }
        }
        private static void ConfigureMessageBusActors(ConfigurationManager config, IRiderRegistrationContext context, IKafkaFactoryConfigurator rider)
        {
            string assemblyName = typeof(TicketsBuilderExtensions).Assembly.GetName().Name;

            var messageBusSettings = config.GetSection(nameof(MessagebusSettings)).Get<MessagebusSettings[]>();
            var messageBusSetting = messageBusSettings?.FirstOrDefault(x => x.AssemblyName == assemblyName);

            if (messageBusSetting?.Consumers?.Count > 0)
            {
                if (messageBusSetting.Consumers.TryGetValue(nameof(UserCreatedConsumer), out string? updateCreatedTopic) &&
                    !string.IsNullOrEmpty(updateCreatedTopic) &&
                     messageBusSettings!.CanRegisterKey(assemblyName!, nameof(UserCreatedConsumer)))
                    rider.TopicEndpoint<UserCreated>(updateCreatedTopic, messageBusSetting.Group, e =>
                    {
                        e.ConfigureConsumer<UserCreatedConsumer>(context);
                    });

                if (messageBusSetting.Consumers.TryGetValue(nameof(UserUpdatedConsumer), out string? userUpdatedTopic) &&
                    !string.IsNullOrEmpty(userUpdatedTopic) &&
                     messageBusSettings!.CanRegisterKey(assemblyName!, nameof(UserUpdatedConsumer)))
                    rider.TopicEndpoint<UserUpdated>(userUpdatedTopic, messageBusSetting.Group, e =>
                {
                    e.ConfigureConsumer<UserUpdatedConsumer>(context);
                });

                if (messageBusSetting.Consumers.TryGetValue(nameof(UserDeletedConsumer), out string? userDeletedTopic) &&
                    !string.IsNullOrEmpty(userDeletedTopic) &&
                     messageBusSettings!.CanRegisterKey(assemblyName!, nameof(UserDeletedConsumer)))
                    rider.TopicEndpoint<UserDeleted>(userDeletedTopic, messageBusSetting.Group, e =>
                {
                    e.ConfigureConsumer<UserDeletedConsumer>(context);
                });
            }

        }
    }
}
