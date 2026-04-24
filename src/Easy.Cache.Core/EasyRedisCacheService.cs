using CSRedis;

namespace Easy.Cache.Core;

public class EasyRedisCacheService : IEasyCacheService
{
    private readonly CSRedisClient _redis;

    public EasyRedisCacheService(CSRedisClient redis)
    {
        _redis = redis ?? throw new ArgumentNullException(nameof(redis), "Redis 客户端不能为空。");
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
        return _redis.Set(key, value, expiration);
    }

    /// <summary>
    /// 异步添加缓存。
    /// </summary>
    /// <param name="key">缓存键。</param>
    /// <param name="value">缓存值。</param>
    /// <param name="expiration">过期时间（秒），小于 0 表示不过期。</param>
    /// <returns>是否写入成功。</returns>
    public async Task<bool> AddAsync(string key, object value, int expiration = -1)
    {
        return await _redis.SetAsync(key, value, expiration);
    }

    /// <summary>
    /// 批量添加缓存（使用 Pipeline）。
    /// </summary>
    /// <param name="keyValues">要写入的键值集合。</param>
    /// <param name="expiration">过期时间（秒），小于 0 表示不过期。</param>
    /// <returns>是否写入成功。</returns>
    public bool Batch(Dictionary<string, object> keyValues, int expiration = -1)
    {
        var pipeline = _redis.StartPipe();
        int count = 0;
        foreach (var item in keyValues)
        {
            pipeline.Set(item.Key, item.Value, expiration);
            count++;
        }
        pipeline.EndPipe();
        return true;
    }

    /// <summary>
    /// 批量添加缓存到hash中
    /// </summary>
    /// <param name="key">Hash 键。</param>
    /// <param name="keyValues">要写入的键值集合。</param>
    /// <returns>是否写入成功。</returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public bool BatchHSet(string key, Dictionary<string, object> keyValues)
    {
        var pipeline = _redis.StartPipe();
        foreach (var item in keyValues)
        {
            pipeline.HSet(key, item.Key, item.Value);
        }
        pipeline.EndPipe();
        return true;
    }

    /// <summary>
    /// 查询key 
    /// </summary>
    /// <param name="pattern"></param>
    /// <returns></returns>
    public List<string> SearchKeys(string pattern)
    {
        if (string.IsNullOrWhiteSpace(pattern))
        {
            throw new ArgumentException("查询模式不能为空。", nameof(pattern));
        }
        return _redis.Keys(pattern).ToList();
    }

    /// <summary>
    /// 根据键获取缓存值。
    /// </summary>
    /// <typeparam name="T">返回值类型。</typeparam>
    /// <param name="key">缓存键。</param>
    /// <returns>缓存值；未命中时返回默认值。</returns>
    public T Get<T>(string key)
    {
        return _redis.Get<T>(key);
    }

    /// <summary>
    /// 异步根据键获取缓存值。
    /// </summary>
    /// <typeparam name="T">返回值类型。</typeparam>
    /// <param name="key">缓存键。</param>
    /// <returns>缓存值；未命中时返回默认值。</returns>
    public async Task<T> GetAsync<T>(string key)
    {
        return await _redis.GetAsync<T>(key);
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
        return _redis.HGet<T>(key, field);
    }

    /// <summary>
    /// 异步从 Hash 结构中读取指定字段。
    /// </summary>
    /// <typeparam name="T">返回值类型。</typeparam>
    /// <param name="key">Hash 键。</param>
    /// <param name="field">Hash 字段名。</param>
    /// <returns>字段值；未命中时返回默认值。</returns>
    public async Task<T> HGetAsync<T>(string key, string field)
    {
        return await _redis.HGetAsync<T>(key, field);
    }

    /// <summary>
    /// 判断 Hash 中是否存在指定字段。
    /// </summary>
    /// <param name="key">Hash 键。</param>
    /// <param name="field">Hash 字段名。</param>
    /// <returns>字段是否存在。</returns>
    public bool HExists(string key, string field)
    {
        return _redis.HExists(key, field);
    }

    /// <summary>
    /// 异步判断 Hash 中是否存在指定字段。
    /// </summary>
    /// <param name="key">Hash 键。</param>
    /// <param name="field">Hash 字段名。</param>
    /// <returns>字段是否存在。</returns>
    public async Task<bool> HExistsAsync(string key, string field)
    {
        return await _redis.HExistsAsync(key, field);
    }

    /// <summary>
    /// 向 Hash 结构写入字段值。
    /// </summary>
    /// <param name="key">Hash 键。</param>
    /// <param name="field">Hash 字段名。</param>
    /// <param name="value">字段值。</param>
    /// <param name="expiration">过期时间（秒），小于 0 表示不修改过期时间。</param>
    /// <returns>是否写入成功。</returns>
    public bool HSet(string key, string field, object value, int expiration = -1)
    {
        var result = _redis.HSet(key, field, value);
        if (expiration >= 0)
        {
            _redis.Expire(key, expiration);
        }
        return result;
    }

    /// <summary>
    /// 异步向 Hash 结构写入字段值。
    /// </summary>
    /// <param name="key">Hash 键。</param>
    /// <param name="field">Hash 字段名。</param>
    /// <param name="value">字段值。</param>
    /// <param name="expiration">过期时间（秒），小于 0 表示不修改过期时间。</param>
    /// <returns>是否写入成功。</returns>
    public async Task<bool> HSetAsync(string key, string field, object value, int expiration = -1)
    {
        var result = await _redis.HSetAsync(key, field, value);
        if (expiration >= 0)
        {
            await _redis.ExpireAsync(key, expiration);
        }
        return result;
    }

    /// <summary>
    /// 删除hash中的某一个field
    /// </summary>
    /// <param name="key">Hash 键。</param>
    /// <param name="field">Hash 字段名。</param>
    /// <returns>是否删除成功。</returns>
    public bool HSetRemove(string key, string field)
    {
        return _redis.HDel(key, field) > 0;
    }

    /// <summary>
    /// 异步删除hash中的某一个field
    /// </summary>
    /// <param name="key">Hash 键。</param>
    /// <param name="field">Hash 字段名。</param>
    /// <returns>是否删除成功。</returns>
    public async Task<bool> HSetRemoveAsync(string key, string field)
    {
        return await _redis.HDelAsync(key, field) > 0;
    }

    /// <summary>
    /// 对指定键进行整型递增。
    /// </summary>
    /// <param name="key">缓存键。</param>
    /// <param name="step">递增步长。</param>
    /// <returns>递增后的值。</returns>
    public long Increment(string key, long step = 1)
    {
        return _redis.IncrBy(key, step);
    }

    /// <summary>
    /// 异步对指定键进行整型递增。
    /// </summary>
    /// <param name="key">缓存键。</param>
    /// <param name="step">递增步长。</param>
    /// <returns>递增后的值。</returns>
    public async Task<long> IncrementAsync(string key, long step = 1)
    {
        return await _redis.IncrByAsync(key, step);
    }

    /// <summary>
    /// 获取分布式锁对象。
    /// </summary>
    /// <param name="name">锁名称。</param>
    /// <param name="timeoutSeconds">锁超时时间（秒）。</param>
    /// <param name="autoDelay">是否启用自动续期（看门狗）。</param>
    /// <returns>分布式锁对象。</returns>
    public CSRedisClientLock Lock(string name, int timeoutSeconds, bool autoDelay = true)
    {
        return _redis.Lock(name, timeoutSeconds, autoDelay);
    }

    /// <summary>
    /// 获取缓存；未命中时通过委托回源并写入缓存。
    /// </summary>
    /// <typeparam name="T">返回值类型。</typeparam>
    /// <param name="key">缓存键。</param>
    /// <param name="func">回源委托。</param>
    /// <param name="expiration">过期时间（秒），小于 0 表示不过期。</param>
    /// <returns>缓存或回源结果。</returns>
    public async Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> func, int expiration = -1)
    {
        var resule = await _redis.GetAsync<T>(key);
        if (resule != null)
        {
            resule = await func();
            await _redis.SetAsync(key, resule, expiration);
        }
        return resule;
    }

    /// <summary>
    /// 获取缓存；未命中时通过委托回源并写入缓存。
    /// </summary>
    /// <typeparam name="T">返回值类型。</typeparam>
    /// <param name="key">缓存键。</param>
    /// <param name="func">回源委托。</param>
    /// <param name="expiration">过期时间（秒），小于 0 表示不过期。</param>
    /// <returns>缓存或回源结果。</returns>
    public T GetOrAdd<T>(string key, Func<T> func, int expiration = -1)
    {
        var resule = _redis.Get<T>(key);
        if (resule != null)
        {
            resule = func();
            _redis.Set(key, resule, expiration);
        }
        return resule;
    }

    /// <summary>
    /// 判断指定键是否存在。
    /// </summary>
    /// <param name="key">缓存键。</param>
    /// <returns>是否存在。</returns>
    public bool Exists(string key)
    {
        return _redis.Exists(key);
    }

    /// <summary>
    /// 异步判断指定键是否存在。
    /// </summary>
    /// <param name="key">缓存键。</param>
    /// <returns>是否存在。</returns>
    public async Task<bool> ExistsAsync(string key)
    {
        return await _redis.ExistsAsync(key);
    }

    /// <summary>
    /// 删除指定键。
    /// </summary>
    /// <param name="key">缓存键。</param>
    /// <returns>是否删除成功。</returns>
    public bool Remove(string key)
    {
        return _redis.Del(key) > 0;
    }

    /// <summary>
    /// 异步删除指定键。
    /// </summary>
    /// <param name="key">缓存键。</param>
    /// <returns>是否删除成功。</returns>
    public async Task<bool> RemoveAsync(string key)
    {
        return await _redis.DelAsync(key) > 0;
    }

    /// <summary>
    /// 订阅指定频道消息。
    /// </summary>
    /// <param name="channel">频道名称。</param>
    /// <param name="onMessage">消息回调。</param>
    public void Subscribe(string channel, Action<string> onMessage)
    {
        if (string.IsNullOrWhiteSpace(channel))
        {
            throw new ArgumentException("频道名称不能为空。", nameof(channel));
        }

        if (onMessage == null)
        {
            throw new ArgumentNullException(nameof(onMessage), "消息回调不能为空。");
        }

        _redis.Subscribe((channel, msg => onMessage(msg.Body)));
    }

    /// <summary>
    /// 向指定频道发布消息。
    /// </summary>
    /// <param name="channel">频道名称。</param>
    /// <param name="body">消息内容。</param>
    /// <returns>收到消息的订阅者数量是否大于 0。</returns>
    public bool Publish(string channel, string body)
    {
        if (string.IsNullOrWhiteSpace(channel))
        {
            throw new ArgumentException("频道名称不能为空。", nameof(channel));
        }

        return _redis.Publish(channel, body) > 0;
    }
}