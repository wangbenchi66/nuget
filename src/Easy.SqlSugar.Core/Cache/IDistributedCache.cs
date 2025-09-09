/*using SqlSugar;

namespace Easy.SqlSugar.Core.Cache;

public class SqlSugarCacheService : ICacheService
{
    private readonly IDistributedCache _cache;

    public SqlSugarCacheService(IDistributedCacheFactory cacheFactory)
    {
        _cache = cacheFactory.Create(DistributedCacheName.Redis_FundTrade, DistributedCacheType.RedisSingle);
    }

    public void Add<V>(string key, V value)
    {
        _cache.Set(key, value);
    }

    public void Add<V>(string key, V value, int cacheDurationInSeconds)
    {
        var time = DateTime.Now.AddSeconds(cacheDurationInSeconds);
        _cache.Set(key, value, time);
    }

    public bool ContainsKey<V>(string key)
    {
        return _cache.ExistsAsync(key).Result;
    }

    public V Get<V>(string key)
    {
        return _cache.Get<V>(key);
    }

    public IEnumerable<string> GetAllKey<V>()
    {
        string prefix = _cache.Prefix;
        if (string.IsNullOrWhiteSpace(prefix))
            return _cache.SearchKeysAsync("SqlSugarDataCache.*").Result;
        else
            return _cache.SearchKeysAsync($"{prefix}SqlSugarDataCache.*").Result;
    }

    public void Remove<V>(string key)
    {
        string prefix = _cache.Prefix;
        if (string.IsNullOrWhiteSpace(prefix))
            _cache.Remove(key);
        else
            _cache.Remove(key.Replace(prefix, string.Empty));
    }

    public V GetOrCreate<V>(string cacheKey, Func<V> create, int cacheDurationInSeconds = 2147483647)
    {
        if (ContainsKey<V>(cacheKey))
        {
            var result = Get<V>(cacheKey);
            if (result == null)
            {
                return create();
            }
            else
            {
                return result;
            }
        }
        else
        {
            var result = create();
            if (cacheDurationInSeconds == int.MaxValue)
                cacheDurationInSeconds = 86400;
            Add(cacheKey, result, cacheDurationInSeconds);
            return result;
        }
    }
}*/