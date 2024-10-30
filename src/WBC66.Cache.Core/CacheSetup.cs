using Microsoft.Extensions.DependencyInjection;

namespace WBC66.Cache.Core
{
    /// <summary>
    /// 缓存服务
    /// </summary>
    public static class CacheSetup
    {
        /// <summary>
        /// 添加内存缓存服务
        /// </summary>
        /// <param name="services"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void AddMemoryCacheSetup(this IServiceCollection services)
        {
            if (services == null) { throw new ArgumentNullException(nameof(services)); }
            services.AddMemoryCache();
        }

        /// <summary>
        /// 添加redis缓存服务
        /// </summary>
        /// <param name="services"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void AddRedisCacheSetup(this IServiceCollection services)
        {
            if (services == null) { throw new ArgumentNullException(nameof(services)); }
            services.AddSingleton<IRedisService, RedisService>();
        }
    }
}