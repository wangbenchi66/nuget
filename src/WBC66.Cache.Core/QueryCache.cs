using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Mvc.Filters;

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
            return $"{methodName}_{ParamHash.GetParametHash(parameters)}";
        }
    }

    public static class ParamHash
    {
        /// <summary>
        /// 获取参数值的字符串表示
        /// </summary>
        /// <param name="value">参数值</param>
        /// <returns>字符串表示</returns>
        internal static string GetParameterValue(object value)
        {
            if (value == null)
            {
                return "null";
            }

            // 如果是简单类型，直接返回字符串表示
            if (value.GetType().IsPrimitive || value is string)
            {
                return value.ToString();
            }

            // 如果是复杂类型，序列化为JSON字符串
            return System.Text.Json.JsonSerializer.Serialize(value);
        }

        /// <summary>
        /// 生成字符串的哈希值
        /// </summary>
        /// <param name="input">输入字符串</param>
        /// <returns>哈希值</returns>
        internal static string GetHash(string input)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }

        /// <summary>
        /// 获取参数并加密
        /// </summary>
        /// <param name="parameters">参数</param>
        /// <returns>加密后的字符串</returns>
        internal static string GetParametHash(params object[] parameters)
        {
            // 加密前
            var paramKey = string.Join("_", parameters.Select(GetParameterValue));
            // 加密后
            var res = GetHash(paramKey);
            return res;
        }
    }
}