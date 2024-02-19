using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using RepairShop.Infrastructure;
using System.Text.Json;

namespace RepairShop.Common.Infrastructure.Cache.DistributedCaching
{
    public abstract class BaseDistributedCacheRepository<T> : IDistributedCacheRepository<T> where T : IEntity
    {
        protected readonly IDistributedCache _cache;
        private static readonly TimeSpan _defaultStoreTime = TimeSpan.FromMinutes(30);
        protected readonly ILogger<BaseDistributedCacheRepository<T>> _logger;

        protected BaseDistributedCacheRepository(IDistributedCache cache, ILogger<BaseDistributedCacheRepository<T>> logger)
        {
            _cache = cache;
            _logger = logger;
        }
        public async Task<T?> GetAsync(Guid id)
        {
            string suffix = id.ToString();
            var obj = await _cache.GetRecordAsync<T>(GenerateCacheKey(suffix));
            _logger.LogInformation($"get key [{GenerateCacheKey(suffix)}] [{JsonSerializer.Serialize(obj)}]");
            return obj;
        }
        public async Task SaveAsync(T dto, TimeSpan? slidingTimeToStore = null) => await SaveAsync(dto, GetAbsoluteTimeToStore(), slidingTimeToStore);
        public async Task SaveAsync(T dto, TimeSpan absoluteTimeToStore, TimeSpan? slidingTimeToStore = null)
        {
            _logger.LogInformation($"save key [{GenerateCacheKey(dto.Id.ToString())}] [{JsonSerializer.Serialize(dto)}]");
            await _cache.SetRecordAsync(GenerateCacheKey(dto.Id.ToString()), dto, absoluteTimeToStore, slidingTimeToStore);
        }
        private string GenerateCacheKey(string suffix)
        {
            return $"{GetKeyPrefix()}_{suffix}";
        }
        protected abstract string GetKeyPrefix();
        private TimeSpan GetAbsoluteTimeToStore() => _defaultStoreTime;
    }
}