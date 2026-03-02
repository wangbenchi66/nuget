using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSRedis;

namespace Easy.Cache.Core
{
    public interface IEasyCacheService
    {
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiration"></param>
        /// <returns></returns>
        bool Add(string key, object value, int expiration = -1);

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiration"></param>
        /// <returns></returns>
        Task<bool> AddAsync(string key, object value, int expiration = -1);

        /// <summary>
        /// 批量添加
        /// </summary>
        /// <param name="items">key/value 集合</param>
        /// <param name="expiration">过期时间（秒），小于 0 表示不过期</param>
        /// <returns>成功写入数量</returns>
        int AddBatch(IEnumerable<KeyValuePair<string, object>> items, int expiration = -1);

        /// <summary>
        /// 批量添加
        /// </summary>
        /// <param name="items">key/value 集合</param>
        /// <param name="expiration">过期时间（秒），小于 0 表示不过期</param>
        /// <returns>成功写入数量</returns>
        Task<int> AddBatchAsync(IEnumerable<KeyValuePair<string, object>> items, int expiration = -1);

        /// <summary>
        /// 获取(委托)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="func"></param>
        /// <param name="expiration"></param>
        /// <returns></returns>
        Task<T> GetAsync<T>(string key, Func<T> func, int expiration = -1);

        /// <summary>
        /// 获取(委托)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="func"></param>
        /// <param name="expiration"></param>
        /// <returns></returns>
        T Get<T>(string key, Func<T> func, int expiration = -1);


        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<bool> ExistsAsync(string key);

        /// <summary>
        /// 获取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        T Get<T>(string key);

        /// <summary>
        /// 获取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<T> GetAsync<T>(string key);

        /// <summary>
        /// Hash 获取
        /// </summary>
        T HGet<T>(string key, string field);

        /// <summary>
        /// Hash 获取
        /// </summary>
        Task<T> HGetAsync<T>(string key, string field);

        /// <summary>
        /// Hash 字段是否存在
        /// </summary>
        bool HExists(string key, string field);

        /// <summary>
        /// Hash 字段是否存在
        /// </summary>
        Task<bool> HExistsAsync(string key, string field);

        /// <summary>
        /// Hash 设置
        /// </summary>
        bool HSet(string key, string field, object value, int expiration = -1);

        /// <summary>
        /// Hash 设置
        /// </summary>
        Task<bool> HSetAsync(string key, string field, object value, int expiration = -1);

        /// <summary>
        /// 递增。
        /// 在分布式场景（多 Pod/多实例）下，建议使用 Redis 实现以保证全局原子性。
        /// </summary>
        long Increment(string key, long step = 1);

        /// <summary>
        /// 异步递增。
        /// 在分布式场景（多 Pod/多实例）下，建议使用 Redis 实现以保证全局原子性。
        /// </summary>
        Task<long> IncrementAsync(string key, long step = 1);

        /// <summary>
        /// 获取分布式锁对象。
        /// 仅 Redis 实现可保证跨实例安全，内存实现在多 Pod 场景会抛出异常。
        /// </summary>
        /// <param name="name">锁名称</param>
        /// <param name="timeoutSeconds">超时（秒）</param>
        /// <param name="autoDelay">自动延长锁超时时间，看门狗线程的超时时间为timeoutSeconds/2 ， 在看门狗线程超时时间时自动延长锁的时间为timeoutSeconds。除非程序意外退出，否则永不超时。</param>
        /// <returns></returns>
        CSRedisClientLock Lock(string name, int timeoutSeconds, bool autoDelay = true);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool Remove(string key);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<bool> RemoveAsync(string key);

        /// <summary>
        /// 订阅消息。
        /// 仅 Redis 实现支持跨实例发布订阅，内存实现在多 Pod 场景会抛出异常。
        /// </summary>
        /// <param name="channel">频道名称</param>
        /// <param name="onMessage">消息回调</param>
        void Subscribe(string channel, Action<string> onMessage);

        /// <summary>
        /// 发布消息。
        /// 仅 Redis 实现支持跨实例发布订阅，内存实现在多 Pod 场景会抛出异常。
        /// </summary>
        /// <param name="channel">频道名称</param>
        /// <param name="body">消息内容</param>
        /// <returns></returns>
        bool Publish(string channel, string body);
    }
}