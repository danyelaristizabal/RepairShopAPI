using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RepairShop.Common.Contracts;
using RepairShop.Common.Infrastructure.Database.MongoDB;
using RepairShop.Common.Infrastructure.Settings;
using RepairShop.Infrastructure.MassTransit;
using Users.Application.Services;
using Users.Contracts.Data;
using Users.Domain.Services;

namespace Users.Infrastructure.Extensions
{
    public static class UsersBuilderExtensions
    {
        public static void ConfigureUsersModuleIfNeeded(this WebApplicationBuilder builder, List<Action<ConfigurationManager, IRiderRegistrationConfigurator>> messageBusActors, List<Action<ConfigurationManager, IRiderRegistrationContext, IKafkaFactoryConfigurator>> addTopics)
        {
            if (!builder.Configuration.GetValue<bool>("HostSettings:Users")) return;

            ConfigureControllers(builder);
            ConfigureDatabase(builder);

            messageBusActors.Add(ConfigureMessageBusActors);
        }
        private static void ConfigureControllers(WebApplicationBuilder builder)
        {
            var profileModuleAssembly = typeof(UsersBuilderExtensions).Assembly;
            builder.Services.AddScoped<IUserService, UsersService>();
            builder.Services.AddControllers().AddApplicationPart(profileModuleAssembly);
        }
        private static void ConfigureDatabase(WebApplicationBuilder builder)
        {
            const string collectionName = "users";
            builder.Services.AddMongoRepository<UserDto>(collectionName, typeof(UsersBuilderExtensions).Assembly.GetName().Name ?? string.Empty);
        }
        private static void ConfigureMessageBusActors(ConfigurationManager config, IRiderRegistrationConfigurator rider)
        {
            const string publisherPostfix = "Publisher";

            string assemblyName = typeof(UsersBuilderExtensions).Assembly.GetName().Name;

            var messageBusSettings = config.GetSection(nameof(MessagebusSettings)).Get<MessagebusSettings[]>();
            var messageBusSetting = messageBusSettings?.FirstOrDefault(x => x.AssemblyName == assemblyName);

            if (messageBusSetting?.Publishers?.Count > 0)
            {
                if (messageBusSetting.Publishers.TryGetValue(nameof(UserCreated) + publisherPostfix, out string? userCreatedTopic) &&
                    !string.IsNullOrEmpty(userCreatedTopic)
                    && messageBusSettings!.CanRegisterKey(assemblyName!, nameof(UserCreated) + publisherPostfix))
                    rider.AddProducer<UserCreated>(userCreatedTopic);

                if (messageBusSetting.Publishers.TryGetValue(nameof(UserUpdated) + publisherPostfix, out string? userUpdatedTopic) &&
                    !string.IsNullOrEmpty(userUpdatedTopic) &&
                    messageBusSettings!.CanRegisterKey(assemblyName!, nameof(UserUpdated) + publisherPostfix))
                    rider.AddProducer<UserUpdated>(userUpdatedTopic);

                if (messageBusSetting.Publishers.TryGetValue(nameof(UserDeleted) + publisherPostfix, out string? userDeletedTopic) &&
                    !string.IsNullOrEmpty(userDeletedTopic) &&
                    messageBusSettings!.CanRegisterKey(assemblyName!, nameof(UserDeleted) + publisherPostfix))
                    rider.AddProducer<UserDeleted>(userDeletedTopic);

                if (messageBusSetting.Publishers.TryGetValue(nameof(NotificationContract) + publisherPostfix, out string? publishTopic) &&
                    !string.IsNullOrEmpty(publishTopic) &&
                    messageBusSettings!.CanRegisterKey(assemblyName!, nameof(NotificationContract) + publisherPostfix))
                    rider.AddProducer<NotificationContract>(publishTopic);
            }
        }
    }
}
