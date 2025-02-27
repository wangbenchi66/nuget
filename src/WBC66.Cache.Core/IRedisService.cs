namespace WBC66.Cache.Core
{
    /// <summary>
    /// 缓存
    /// </summary>
    public interface IRedisService
    {
        /// <summary>
        /// 初始化(单节点)
        /// </summary>
        /// <param name="conn"></param>
        /// <exception cref="ArgumentNullException"></exception>
        void Initialization(string conn);

        /// <summary>
        /// 初始化(哨兵)
        /// </summary>
        /// <param name="masterName">master节点</param>
        /// <param name="conn">哨兵地址</param>
        /// <exception cref="ArgumentNullException"></exception>
        void Initialization(string masterName, string[] conn);

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
        /// <param name="db"></param>
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
        /// 删除
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<bool> Remove(string key);
    }
}