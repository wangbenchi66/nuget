using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Mvc.Filters;
using Easy.Common.Core;

namespace WBC66.Cache.Core
{
    /// <summary>
    ///  自定义特性，用于标记需要缓存结果的方法
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class CacheResultAttribute : Attribute
    {
        /// <summary>
        /// 缓存持续时间（秒）
        /// /// </summary>
        public int Duration { get; }

        public CacheResultAttribute(int duration)
        {
            Duration = duration;
        }
    }

    /// <summary>
    /// AOP过滤器，用于在调用接口时根据特性进行缓存
    /// </summary>
    public class CacheResultFilter : IAsyncActionFilter
    {
        private readonly IMemoryCache _memoryCache;

        public CacheResultFilter(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        /// <summary>
        /// 获取缓存或添加新缓存项
        /// </summary>
        /// <typeparam name="T">缓存项的类型</typeparam>
        /// <param name="cacheKey">缓存键</param>
        /// <param name="factory">生成缓存项的工厂方法</param>
        /// <param name="cacheDuration">缓存持续时间</param>
        /// <returns>缓存项</returns>
        public async Task<T> GetOrAddAsync<T>(string cacheKey, Func<Task<T>> factory, TimeSpan cacheDuration)
        {
            // 尝试从缓存中获取值
            if (!_memoryCache.TryGetValue(cacheKey, out T cacheEntry))
            {
                // 如果缓存中没有值，则调用工厂方法生成值
                cacheEntry = await factory();
                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = cacheDuration
                };
                // 将生成的值添加到缓存中
                _memoryCache.Set(cacheKey, cacheEntry, cacheEntryOptions);
            }
            return cacheEntry;
        }

        /// <summary>
        /// 在执行操作之前或之后执行的异步方法
        /// </summary>
        /// <param name="context">操作执行上下文</param>
        /// <param name="next">操作执行委托</param>
        /// <returns>任务</returns>
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // 获取方法上的缓存特性
            var cacheAttribute = context.ActionDescriptor.EndpointMetadata.OfType<CacheResultAttribute>().FirstOrDefault();
            if (cacheAttribute == null)
            {
                // 如果没有缓存特性，直接执行下一个操作
                await next();
                return;
            }

            // 使用方法名和参数作为缓存键，确保唯一性
            var cacheKey = GenerateCacheKey(context.ActionDescriptor.DisplayName, context.ActionArguments);
            var cacheDuration = TimeSpan.FromSeconds(cacheAttribute.Duration);
            var cacheEntry = await GetOrAddAsync(cacheKey, async () =>
            {
                // 执行操作并获取结果
                var resultContext = await next();
                return resultContext.Result;
            }, cacheDuration);

            // 将缓存结果设置为操作结果
            context.Result = cacheEntry;
        }

        /// <summary>
        /// 生成缓存键
        /// </summary>
        /// <param name="methodName">方法名</param>
        /// <param name="parameters">参数</param>
        /// <returns>缓存键</returns>
        private string GenerateCacheKey(string methodName, IDictionary<string, object> parameters)
        {
            return $"{methodName}_{HttpContextHelper.GetParametHash(parameters)}";
        }
    }

}