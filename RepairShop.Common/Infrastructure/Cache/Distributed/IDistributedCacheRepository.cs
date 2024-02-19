using RepairShop.Infrastructure;

namespace RepairShop.Common.Infrastructure.Cache.DistributedCaching
{
    public interface IDistributedCacheRepository<T> where T : IEntity
    {
        Task<T?> GetAsync(Guid id);
        Task SaveAsync(T dto, TimeSpan? slidingTimeToStore = null);
    }
}