using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using SqlSugar;

namespace Easy.SqlSugar.Core.Cache
{
    /// <summary>
    /// 使用分布式缓存存储器  Microsoft.Extensions.Caching.Distributed.IDistributedCache
    /// </summary>
    public class DistributedCache : ICacheService
    {
        /// <summary>
        /// 分布式缓存存储器
        /// </summary>
        public IDistributedCache Cache { get; set; }

        /// <summary>
        /// 序列化方法
        /// </summary>
        public JsonHelper Serialize { get; set; }

        /// <summary>
        /// 提供分布式缓存存储器
        /// </summary>
        /// <param name="cache">分布式缓存存储器</param>
        /// <param name="serialize">实现序列化的接口s</param>
        public DistributedCache(IDistributedCache cache, JsonHelper serialize = null)
        {
            this.Cache = cache;
            if (serialize == null)
                this.Serialize = new JsonHelper();
            else
                this.Serialize = serialize;
        }

        /// <inheritdoc/>
        public void Add<V>(string key, V value)
        {
            var valStr = this.Serialize.Serialize(value);
            this.Cache.SetString(key, valStr);
        }

        /// <inheritdoc/>
        public void Add<V>(string key, V value, int cacheDurationInSeconds)
        {
            var valStr = this.Serialize.Serialize(value);
            DistributedCacheEntryOptions op = new()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(cacheDurationInSeconds),
            };
            this.Cache.SetString(key, valStr, op);
        }

        /// <inheritdoc/>
        public bool ContainsKey<V>(string key)
        {
            return this.Cache.Get(key) != null;
        }

        /// <inheritdoc/>
        public V Get<V>(string key)
        {
            var val = this.Cache.GetString(key);
            if (val == null) return default(V);
            return (V)this.Serialize.Deserialize(val, typeof(V));
        }

        /// <inheritdoc/>
        public IEnumerable<string> GetAllKey<V>()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public V GetOrCreate<V>(string cacheKey, Func<V> create, int cacheDurationInSeconds = int.MaxValue)
        {
            if (ContainsKey<V>(cacheKey))
            {
                return Get<V>(cacheKey);
            }
            V val = create();
            Add(cacheKey, val, cacheDurationInSeconds);
            return val;
        }

        /// <inheritdoc/>
        public void Remove<V>(string key)
        {
            this.Cache.Remove(key);
        }
    }


    public class JsonHelper : IJsonSerialize
    {
        public T? Deserialize<T>(string str) where T : class
        {
            return JsonSerializer.Deserialize<T>(str);
        }

        public object? Deserialize(string str, Type returnType)
        {
            return JsonSerializer.Deserialize(str, returnType);
        }

        public string Serialize<T>(T obj)
        {
            return JsonSerializer.Serialize(obj);
        }

        public string Serialize(object obj, Type inputType)
        {
            return JsonSerializer.Serialize(obj, inputType);
        }
    }

    /// <summary>
    /// 序列化对象到字符串
    /// </summary>
    public interface IJsonSerialize
    {
        /// <summary>
        /// 序列化对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="obj">对象</param>
        /// <returns>序列化字符串</returns>
        public string Serialize<T>(T obj);

        /// <summary>
        /// 反序列化对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="str">序列化字符串</param>
        /// <returns>对象实例</returns>
        public T? Deserialize<T>(string str) where T : class;

        /// <summary>
        /// 反序列化json字符串
        /// </summary>
        /// <param name="str">json字符串</param>
        /// <param name="returnType">返回类型</param>
        /// <returns>json对象</returns>
        public object? Deserialize(string str, Type returnType);

        /// <summary>
        /// 序列化json对象
        /// </summary>
        /// <param name="obj">json对象</param>
        /// <param name="inputType">输入类型</param>
        /// <returns>json字符串</returns>
        public string Serialize(object obj, Type inputType);
    }
}