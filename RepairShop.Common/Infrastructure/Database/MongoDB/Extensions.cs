using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using RepairShop.Infrastructure;
using RepairShop.Infrastructure.Settings;

namespace RepairShop.Common.Infrastructure.Database.MongoDB
{
    public static class Extensions
    {
        public static void AddMongo(this WebApplicationBuilder builder)
        {
            BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
            BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));

            var mongoDbSettings = builder.Configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings[]>();

            var clients = new Dictionary<string, IMongoClient>();

            if (mongoDbSettings is not null)
                foreach (var mongoDbSetting in mongoDbSettings)
                    if (!string.IsNullOrEmpty(mongoDbSetting.AssemblyName))
                        clients.Add(mongoDbSetting.AssemblyName, new MongoClient(mongoDbSetting.ConnectionString));

            builder.Services.AddSingleton(clients);
            builder.Services.AddSingleton<MongoClientFactory>();
        }

        public static IServiceCollection AddMongoRepository<T>(this IServiceCollection services, string collectionName, string databaseName) where T : IEntity
        {
            services.AddSingleton<IRepository<T>>(serviceProvider =>
            {
                var database = serviceProvider.GetService<MongoClientFactory>();
                if (database == null)
                {
                    throw new InvalidOperationException("MongoClientFactory is not registered in the service provider.");
                }

                var logger = serviceProvider.GetService<ILogger<MongoRepository<T>>>();
                if (logger == null)
                {
                    throw new InvalidOperationException("Logger<MongoRepository<T>> is not registered in the service provider.");
                }

                return new MongoRepository<T>(database.GetClient(databaseName).GetDatabase(databaseName), collectionName, logger);
            });
            return services;
        }

    }
}
