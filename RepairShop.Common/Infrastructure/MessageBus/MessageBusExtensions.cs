using Confluent.Kafka;
using Confluent.Kafka.Admin;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RepairShop.Common.Infrastructure.Settings;
using RepairShop.Infrastructure.Settings;

namespace RepairShop.Infrastructure.MassTransit
{
    public static class MessageBusExtensions
    {
        public static async Task<IServiceCollection> AddMassTransitWithKafkaAsync(this WebApplicationBuilder builder,
            IEnumerable<Action<ConfigurationManager, IRiderRegistrationConfigurator>> registerMessageBusActors,
            IEnumerable<Action<ConfigurationManager, IRiderRegistrationContext, IKafkaFactoryConfigurator>> addTopics)
        {

            var kafkaSettings = builder.Configuration?.GetSection(nameof(KafkaSettings)).Get<KafkaSettings>();

            if (kafkaSettings?.TopicsToCreate?.Count() > 0)
                foreach (var topic in kafkaSettings.TopicsToCreate)
                {
                    await CreateTopicAsync(kafkaSettings.Host, topic);
                }

            builder.Services.AddMassTransit(x =>
            {
                x.UsingInMemory();
                x.AddRider(rider =>
                {
                    if (builder.Configuration is not null)
                        foreach (var register in registerMessageBusActors)
                            register(builder.Configuration, rider);

                    rider.UsingKafka((context, configurator) =>
                    {
                        configurator.Host(kafkaSettings?.Host);
                        configurator.SecurityProtocol = SecurityProtocol.Plaintext;

                        if (builder.Configuration is not null)
                            foreach (var addTopic in addTopics)
                                addTopic(builder.Configuration, context, configurator);

                    });
                });
            });
            return builder.Services;
        }

        public static async Task CreateTopicAsync(string bootstrapServers, string topicName)
        {
            using var adminClient = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = bootstrapServers }).Build();

            try
            {
                await adminClient.CreateTopicsAsync(new TopicSpecification[]
                {
                    new TopicSpecification { Name = topicName, ReplicationFactor = 1, NumPartitions = 1 }
                });
            }
            catch (CreateTopicsException e)
            {
                if (e.Results[0].Error.Code != ErrorCode.TopicAlreadyExists)
                {
                    throw;
                }
            }
        }
        public static bool CanRegisterKey(this MessagebusSettings[] messageBusSettings, string currentAssemblyName, string key)
        {
            var orderedSettings = messageBusSettings!.OrderBy(x => x.AssemblyName).ToList();

            var settingsWithKey = orderedSettings.Where(x => (x?.Consumers?.ContainsKey(key) ?? false) || (x?.Publishers?.ContainsKey(key) ?? false)).ToList();

            if (settingsWithKey.Count == 1)
                return true;
            else if (settingsWithKey.Count > 1)
            {
                var orderedAssemblyNames = settingsWithKey.Select(x => x.AssemblyName).OrderBy(x => x).ToList();
                if (orderedAssemblyNames.First() == currentAssemblyName)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
