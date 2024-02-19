using RepairShop.Infrastructure;
using System.Collections.Concurrent;

public class CacheRepository<T> : ICacheRepository<T> where T : IEntity
{
    private readonly IRepository<T> _underlyingRepository;
    private ConcurrentDictionary<string, T> _cache;
    private SemaphoreSlim _cacheLock = new SemaphoreSlim(1, 1);
    public IEnumerable<T> Cache => _cache.Values;
    private bool isCacheLoaded = false;
    public CacheRepository(IRepository<T> underlyingRepository)
    {
        this._underlyingRepository = underlyingRepository;
        this._cache = new ConcurrentDictionary<string, T>();
    }
    private async Task LoadCacheAsync()
    {
        if (!isCacheLoaded)
        {
            await _cacheLock.WaitAsync();
            try
            {
                if (!isCacheLoaded)
                {
                    var allItems = await _underlyingRepository.GetAllListAsync();
                    _cache = new ConcurrentDictionary<string, T>(allItems.ToDictionary(item => item.Id.ToString()));
                    isCacheLoaded = true;
                }
            }
            finally
            {
                _cacheLock.Release();
            }
        }
    }
    public async Task ReloadCacheAsync()
    {
        isCacheLoaded = false;
        await LoadCacheAsync();
    }

    public async Task<IReadOnlyCollection<T>> GetAllListAsync()
    {
        if (!isCacheLoaded)
        {
            return await _underlyingRepository.GetAllListAsync();
        }

        return _cache.Values.ToList().AsReadOnly();
    }
}
