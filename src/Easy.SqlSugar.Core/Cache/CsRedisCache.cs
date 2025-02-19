/*using CSRedis;
using SqlSugar;

namespace Easy.SqlSugar.Core.Cache
{
    /// <summary>
    /// csredis 缓存
    /// </summary>
    public class CsRedisCacheService : ICacheService
    {
        public CsRedisCacheService(CSRedisClient client)
        {
            RedisHelper.Initialization(client);
        }

        //注意:SugarRedis 不要扔到构造函数里面， 一定要单例模式  

        public void Add<V>(string key, V value)
        {
            RedisHelper.Set(key, value);
        }

        public void Add<V>(string key, V value, int cacheDurationInSeconds)
        {
            RedisHelper.Set(key, value, cacheDurationInSeconds);
        }

        public bool ContainsKey<V>(string key)
        {
            return RedisHelper.Exists(key);
        }

        public V Get<V>(string key)
        {
            return RedisHelper.Get<V>(key);
        }

        public IEnumerable<string> GetAllKey<V>()
        {
            //获取redisHelper配置的prefix
            string prefix = RedisHelper.Prefix;
            if (string.IsNullOrWhiteSpace(prefix))
                return RedisHelper.Keys("SqlSugarDataCache.*");
            else
                return RedisHelper.Keys($"{prefix}SqlSugarDataCache.*");
        }

        public V GetOrCreate<V>(string cacheKey, Func<V> create, int cacheDurationInSeconds = int.MaxValue)
        {
            if (this.ContainsKey<V>(cacheKey))
            {
                var result = this.Get<V>(cacheKey);
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
                this.Add(cacheKey, result, cacheDurationInSeconds);
                return result;
            }
        }

        public void Remove<V>(string key)
        {
            string prefix = RedisHelper.Prefix;
            RedisHelper.Del($"{key.Replace(prefix, "")}");
        }
    }
}*/