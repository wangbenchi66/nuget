using System.Collections;
using System.Collections.Concurrent;
using System.Reflection;
using CSRedis;
using Microsoft.Extensions.Caching.Memory;

namespace Easy.Cache.Core;

public class EasyMemoryCacheService : IEasyCacheService
{
    private readonly IMemoryCache _memoryCache;

    public EasyMemoryCacheService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache), "内存缓存实例不能为空。");
    }

    /// <summary>
    /// 添加缓存。
    /// </summary>
    /// <param name="key">缓存键。</param>
    /// <param name="value">缓存值。</param>
    /// <param name="expiration">过期时间（秒），小于 0 表示不过期。</param>
    /// <returns>是否写入成功。</returns>
    public bool Add(string key, object value, int expiration = -1)
    {
        ValidateKey(key);
        var options = BuildOptions(expiration);
        _memoryCache.Set(key, value, options);
        return true;
    }

    /// <summary>
    /// 异步添加缓存。
    /// </summary>
    /// <param name="key">缓存键。</param>
    /// <param name="value">缓存值。</param>
    /// <param name="expiration">过期时间（秒），小于 0 表示不过期。</param>
    /// <returns>是否写入成功。</returns>
    public Task<bool> AddAsync(string key, object value, int expiration = -1)
    {
        return Task.FromResult(Add(key, value, expiration));
    }

    /// <summary>
    /// 批量添加缓存。
    /// </summary>
    /// <param name="keyValues">要写入的键值集合。</param>
    /// <param name="expiration">过期时间（秒），小于 0 表示不过期。</param>
    /// <returns>是否写入成功。</returns>
    public bool Batch(Dictionary<string, object> keyValues, int expiration = -1)
    {
        if (keyValues == null)
        {
            throw new ArgumentNullException(nameof(keyValues), "批量添加集合不能为空。");
        }

        foreach (var item in keyValues)
        {
            Add(item.Key, item.Value, expiration);
        }
        return true;
    }

    /// <summary>
    /// 批量添加缓存到hash中
    /// </summary>
    /// <param name="keyValues">要写入的键值集合。</param>
    /// <returns>是否写入成功。</returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public bool BatchHSet(string key, Dictionary<string, object> keyValues)
    {
        throw new NotImplementedException("请切换到 Redis 缓存以获得更高效的批量操作支持。");
    }

    /// <summary>
    /// 判断指定键是否存在。
    /// </summary>
    /// <param name="key">缓存键。</param>
    /// <returns>是否存在。</returns>
    public bool Exists(string key)
    {
        ValidateKey(key);
        return _memoryCache.TryGetValue(key, out _);
    }

    /// <summary>
    /// 异步判断指定键是否存在。
    /// </summary>
    /// <param name="key">缓存键。</param>
    /// <returns>是否存在。</returns>
    public Task<bool> ExistsAsync(string key)
    {
        ValidateKey(key);
        return Task.FromResult(_memoryCache.TryGetValue(key, out _));
    }

    /// <summary>
    /// 查询key 
    /// </summary>
    /// <param name="pattern"></param>
    /// <returns></returns>
    public List<string> SearchKeys(string pattern)
    {
        ValidateKey(pattern);
        //返回所有缓存键中包含 pattern 的键列表
        return GetCacheKeys(_memoryCache);
    }


    /// <summary>
    /// 获取所有缓存键
    /// </summary>
    /// <returns></returns>
    private List<string> GetCacheKeys(IMemoryCache cache)
    {
        //Microsoft.Extensions.Caching.Memory版本修改后里边的名称发生了变更
        var netVersion = Assembly.Load("Microsoft.Extensions.Caching.Memory").GetName().Version.Major;
        if (netVersion <= 5)
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            var entries = cache.GetType().GetField("_entries", flags).GetValue(cache);
            var cacheItems = entries as IDictionary;
            var keys = new List<string>();
            if (cacheItems == null) return keys;
            foreach (DictionaryEntry cacheItem in cacheItems)
            {
                keys.Add(cacheItem.Key.ToString());
            }
            return keys;
        }
        else
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            var coherentState = cache.GetType().GetField("_coherentState", flags).GetValue(cache);
            var entries = coherentState.GetType().GetField("_stringEntries", flags).GetValue(coherentState);
            var cacheItems = entries as IDictionary;
            var keys = new List<string>();
            if (cacheItems == null) return keys;
            foreach (DictionaryEntry cacheItem in cacheItems)
            {
                keys.Add(cacheItem.Key.ToString());
            }
            return keys;
        }
    }

    /// <summary>
    /// 获取缓存；未命中时通过委托回源并写入缓存。
    /// </summary>
    /// <typeparam name="T">返回值类型。</typeparam>
    /// <param name="key">缓存键。</param>
    /// <param name="func">回源委托。</param>
    /// <param name="expiration">过期时间（秒），小于 0 表示不过期。</param>
    /// <returns>缓存或回源结果。</returns>
    public T Get<T>(string key, Func<T> func, int expiration = -1)
    {
        ValidateKey(key);
        if (func is null)
        {
            throw new ArgumentNullException(nameof(func), "回源委托不能为空。");
        }

        if (_memoryCache.TryGetValue(key, out var cached) && cached is T typed)
        {
            return typed;
        }

        var value = func();
        var options = BuildOptions(expiration);
        _memoryCache.Set(key, value, options);
        return value;
    }

    /// <summary>
    /// 根据键获取缓存值。
    /// </summary>
    /// <typeparam name="T">返回值类型。</typeparam>
    /// <param name="key">缓存键。</param>
    /// <returns>缓存值；未命中时返回默认值。</returns>
    public T Get<T>(string key)
    {
        ValidateKey(key);
        if (_memoryCache.TryGetValue(key, out var cached) && cached is T typed)
        {
            return typed;
        }

        return default!;
    }

    /// <summary>
    /// 异步获取缓存；未命中时通过委托回源并写入缓存。
    /// </summary>
    /// <typeparam name="T">返回值类型。</typeparam>
    /// <param name="key">缓存键。</param>
    /// <param name="func">回源委托。</param>
    /// <param name="expiration">过期时间（秒），小于 0 表示不过期。</param>
    /// <returns>缓存或回源结果。</returns>
    public async Task<T> GetAsync<T>(string key, Func<Task<T>> func, int expiration = -1)
    {
        ValidateKey(key);
        if (_memoryCache.TryGetValue(key, out T cached))
        {
            return cached;
        }

        T value = await func();
        _memoryCache.Set(key, value, TimeSpan.FromSeconds(expiration));
        return value;
    }

    /// <summary>
    /// 异步根据键获取缓存值。
    /// </summary>
    /// <typeparam name="T">返回值类型。</typeparam>
    /// <param name="key">缓存键。</param>
    /// <returns>缓存值；未命中时返回默认值。</returns>
    public Task<T> GetAsync<T>(string key)
    {
        return Task.FromResult(Get<T>(key));
    }

    /// <summary>
    /// 从 Hash 结构中读取指定字段。
    /// </summary>
    /// <typeparam name="T">返回值类型。</typeparam>
    /// <param name="key">Hash 键。</param>
    /// <param name="field">Hash 字段名。</param>
    /// <returns>字段值；未命中时返回默认值。</returns>
    public T HGet<T>(string key, string field)
    {
        ValidateKey(key);
        ValidateKey(field);

        if (_memoryCache.TryGetValue(key, out var hashObj) && hashObj is ConcurrentDictionary<string, object> hash)
        {
            if (hash.TryGetValue(field, out var value) && value is T typed)
            {
                return typed;
            }
        }

        return default!;
    }

    /// <summary>
    /// 异步从 Hash 结构中读取指定字段。
    /// </summary>
    /// <typeparam name="T">返回值类型。</typeparam>
    /// <param name="key">Hash 键。</param>
    /// <param name="field">Hash 字段名。</param>
    /// <returns>字段值；未命中时返回默认值。</returns>
    public Task<T> HGetAsync<T>(string key, string field)
    {
        return Task.FromResult(HGet<T>(key, field));
    }

    /// <summary>
    /// 判断 Hash 中是否存在指定字段。
    /// </summary>
    /// <param name="key">Hash 键。</param>
    /// <param name="field">Hash 字段名。</param>
    /// <returns>字段是否存在。</returns>
    public bool HExists(string key, string field)
    {
        ValidateKey(key);
        ValidateKey(field);

        if (_memoryCache.TryGetValue(key, out var hashObj) && hashObj is ConcurrentDictionary<string, object> hash)
        {
            return hash.ContainsKey(field);
        }

        return false;
    }

    /// <summary>
    /// 异步判断 Hash 中是否存在指定字段。
    /// </summary>
    /// <param name="key">Hash 键。</param>
    /// <param name="field">Hash 字段名。</param>
    /// <returns>字段是否存在。</returns>
    public Task<bool> HExistsAsync(string key, string field)
    {
        return Task.FromResult(HExists(key, field));
    }

    /// <summary>
    /// 向 Hash 结构写入字段值。
    /// </summary>
    /// <param name="key">Hash 键。</param>
    /// <param name="field">Hash 字段名。</param>
    /// <param name="value">字段值。</param>
    /// <param name="expiration">过期时间（秒），小于 0 表示不过期。</param>
    /// <returns>是否写入成功。</returns>
    public bool HSet(string key, string field, object value, int expiration = -1)
    {
        ValidateKey(key);
        ValidateKey(field);

        var options = BuildOptions(expiration);
        var hash = _memoryCache.GetOrCreate(key, entry =>
        {
            if (options != null)
            {
                entry.SetOptions(options);
            }
            return new ConcurrentDictionary<string, object>();
        })!;

        hash[field] = value;
        if (options != null)
        {
            _memoryCache.Set(key, hash, options);
        }
        else
        {
            _memoryCache.Set(key, hash);
        }

        return true;
    }

    /// <summary>
    /// 异步向 Hash 结构写入字段值。
    /// </summary>
    /// <param name="key">Hash 键。</param>
    /// <param name="field">Hash 字段名。</param>
    /// <param name="value">字段值。</param>
    /// <param name="expiration">过期时间（秒），小于 0 表示不过期。</param>
    /// <returns>是否写入成功。</returns>
    public Task<bool> HSetAsync(string key, string field, object value, int expiration = -1)
    {
        return Task.FromResult(HSet(key, field, value, expiration));
    }

    /// <summary>
    /// 递增操作。
    /// 在多 Pod/多实例部署下，内存缓存不具备全局原子性与一致性，因此不支持。
    /// </summary>
    /// <param name="key">缓存键。</param>
    /// <param name="step">递增步长。</param>
    /// <returns>不返回结果，直接抛出异常。</returns>
    public long Increment(string key, long step = 1)
    {
        throw new NotSupportedException("内存缓存不支持递增功能：在多 Pod/多实例部署下无法保证全局原子性，请切换到 Redis 缓存。");
    }

    /// <summary>
    /// 异步递增操作。
    /// 在多 Pod/多实例部署下，内存缓存不具备全局原子性与一致性，因此不支持。
    /// </summary>
    /// <param name="key">缓存键。</param>
    /// <param name="step">递增步长。</param>
    /// <returns>不返回结果，直接抛出异常。</returns>
    public Task<long> IncrementAsync(string key, long step = 1)
    {
        throw new NotSupportedException("内存缓存不支持递增功能：在多 Pod/多实例部署下无法保证全局原子性，请切换到 Redis 缓存。");
    }

    /// <summary>
    /// 获取分布式锁对象。
    /// 在多 Pod/多实例部署下，内存缓存无法实现跨实例互斥，因此不支持。
    /// </summary>
    /// <param name="name">锁名称。</param>
    /// <param name="timeoutSeconds">锁超时时间（秒）。</param>
    /// <param name="autoDelay">是否启用自动续期。</param>
    /// <returns>不返回结果，直接抛出异常。</returns>
    public CSRedisClientLock Lock(string name, int timeoutSeconds, bool autoDelay = true)
    {
        throw new NotSupportedException("内存缓存不支持分布式锁功能：在多 Pod/多实例部署下无法实现跨实例互斥，请切换到 Redis 缓存。");
    }

    /// <summary>
    /// 删除指定键。
    /// </summary>
    /// <param name="key">缓存键。</param>
    /// <returns>是否删除成功。</returns>
    public bool Remove(string key)
    {
        ValidateKey(key);
        var exists = _memoryCache.TryGetValue(key, out _);
        _memoryCache.Remove(key);
        return exists;
    }

    /// <summary>
    /// 异步删除指定键。
    /// </summary>
    /// <param name="key">缓存键。</param>
    /// <returns>是否删除成功。</returns>
    public Task<bool> RemoveAsync(string key)
    {
        ValidateKey(key);
        var exists = _memoryCache.TryGetValue(key, out _);
        _memoryCache.Remove(key);
        return Task.FromResult(exists);
    }

    /// <summary>
    /// 订阅消息。
    /// 在多 Pod/多实例部署下，内存缓存不具备跨实例消息总线能力，因此不支持。
    /// </summary>
    /// <param name="channel">频道名称。</param>
    /// <param name="onMessage">消息回调。</param>
    public void Subscribe(string channel, Action<string> onMessage)
    {
        throw new NotSupportedException("内存缓存不支持订阅功能：在多 Pod/多实例部署下无法实现跨实例消息分发，请切换到 Redis 缓存。");
    }

    /// <summary>
    /// 发布消息。
    /// 在多 Pod/多实例部署下，内存缓存不具备跨实例消息总线能力，因此不支持。
    /// </summary>
    /// <param name="channel">频道名称。</param>
    /// <param name="body">消息内容。</param>
    /// <returns>不返回结果，直接抛出异常。</returns>
    public bool Publish(string channel, string body)
    {
        throw new NotSupportedException("内存缓存不支持发布功能：在多 Pod/多实例部署下无法实现跨实例消息分发，请切换到 Redis 缓存。");
    }

    private static MemoryCacheEntryOptions? BuildOptions(int expiration)
    {
        if (expiration < 0)
        {
            return null;
        }

        return new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(expiration)
        };
    }

    private static void ValidateKey(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("缓存 key 不能为空。", nameof(key));
        }
    }
}