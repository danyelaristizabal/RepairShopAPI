using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RedLockNet;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using RepairShop.Common.Infrastructure.Settings;
using StackExchange.Redis;

namespace RepairShop.Common.Infrastructure.Cache.DistributedCaching
{
    public static class Extensions
    {
        public static void AddRedis(this WebApplicationBuilder builder)
        {
            var redisSettings = builder.Configuration.GetSection(nameof(RedisSettings)).Get<RedisSettings>();
            if (redisSettings is null || string.IsNullOrWhiteSpace(redisSettings.ConnectionString)) return;

            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisSettings.ConnectionString;
                options.InstanceName = redisSettings.InstanceName;
            });

            AddDistributedLocking(builder, redisSettings);
        }

        private static void AddDistributedLocking(WebApplicationBuilder builder, RedisSettings redisSettings)
        {
            var redisConnectionMultiplexer = ConnectionMultiplexer.Connect(redisSettings.ConnectionString);

            builder.Services.AddSingleton<IDistributedLockFactory>(
                provider =>
                RedLockFactory.Create(new List<RedLockMultiplexer>
                {
                    new RedLockMultiplexer(redisConnectionMultiplexer) }
                )
            );
        }
    }
}
