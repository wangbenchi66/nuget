using CSRedis;

namespace WBC66.Cache.Core
{
    /// <summary>
    /// Redis
    /// </summary>
    public class RedisService : IRedisService
    {

        public RedisService()
        {
        }

        /// <summary>
        /// 初始化(单节点)
        /// </summary>
        /// <param name="conn"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void Initialization(string conn)
        {
            if (string.IsNullOrWhiteSpace(conn)) { throw new ArgumentNullException(nameof(conn)); }
            RedisHelper.Initialization(new CSRedisClient(conn));
        }

        /// <summary>
        /// 初始化(哨兵)
        /// </summary>
        /// <param name="masterName">master节点</param>
        /// <param name="conn">哨兵地址</param>
        /// <exception cref="ArgumentNullException"></exception>
        public void Initialization(string masterName, string[] conn)
        {
            if (conn == null) { throw new ArgumentNullException(nameof(conn)); }
            RedisHelper.Initialization(new CSRedisClient(masterName, conn));
        }

        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiration"></param>
        /// <returns></returns>
        public bool Add(string key, object value, int expiration = -1)
        {
            return RedisHelper.Set(key, value, expiration);
        }

        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiration">分钟数</param>
        public async Task<bool> AddAsync(string key, object value, int expiration = -1)
        {
            return await RedisHelper.SetAsync(key, value, expiration);
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T Get<T>(string key)
        {
            return RedisHelper.Get<T>(key);
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<T> GetAsync<T>(string key)
        {
            return await RedisHelper.GetAsync<T>(key);
        }

        /// <summary>
        /// 获取(委托)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="func"></param>
        /// <param name="expiration"></param>
        /// <returns></returns>
        public async Task<T> GetAsync<T>(string key, Func<T> func, int expiration = -1)
        {
            var resule = await RedisHelper.GetAsync<T>(key);
            if (resule == null || resule.ToString().Trim() == "[]")
            {
                resule = func();
                await RedisHelper.SetAsync(key, resule, expiration);
            }
            return resule;
        }

        /// <summary>
        /// 获取(委托)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="func"></param>
        /// <param name="expiration"></param>
        /// <returns></returns>
        public T Get<T>(string key, Func<T> func, int expiration = -1)
        {
            var resule = RedisHelper.Get<T>(key);
            if (resule == null || resule.ToString().Trim() == "[]")
            {
                resule = func();
                RedisHelper.Set(key, resule, expiration);
            }
            return resule;
        }

        /// <summary>
        /// 判断redis指定key是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<bool> ExistsAsync(string key)
        {
            return await RedisHelper.ExistsAsync(key);
        }

        /// <summary>
        /// 删除redis指定key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<bool> Remove(string key)
        {
            return await RedisHelper.DelAsync(key) > 0;
        }

        /// <summary>
        /// 订阅模式
        /// </summary>
        /// <param name="channels">队列名称</param>
        /// <returns>返回object的body</returns>
        public object Subscribe(string channels)
        {
            string body = string.Empty;
            RedisHelper.Subscribe((channels, msg =>
            {
                Console.WriteLine(msg.Body);
                body = msg.Body;
            }
            ));
            return body;
        }

        /// <summary>
        /// 发布模式
        /// </summary>
        /// <param name="channels">队列名称</param>
        /// <param name="body">队列主体数据</param>
        /// <returns></returns>
        public bool Publish(string channels, string body)
        {
            return RedisHelper.Publish(channels, body) > 0;
        }
    }
}