using RepairShop.Infrastructure;

public interface ICacheRepository<T> where T : IEntity
{
    Task ReloadCacheAsync();
    public Task<IReadOnlyCollection<T>> GetAllListAsync();

}
