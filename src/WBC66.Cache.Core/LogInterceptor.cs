using Castle.DynamicProxy;
using Microsoft.Extensions.Caching.Memory;

namespace WBC66.Cache.Core
{
    /// <summary>
    /// 类AOP拦截器，用于拦截方法执行前后
    /// </summary>
    public class MemoryCacheInterceptor : IInterceptor
    {
        private readonly IMemoryCache _memoryCache;
        /// <summary>
        /// 构造函数 注入IMemoryCache
        /// </summary>
        /// <param name="memoryCache"></param>
        public MemoryCacheInterceptor(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        /// <summary>
        /// 拦截器
        /// </summary>
        /// <param name="invocation"></param>
        public void Intercept(IInvocation invocation)
        {
            //判断有没有CacheResult特性
            if (invocation.MethodInvocationTarget.GetCustomAttributes(true).Any(a => a.GetType() == typeof(CacheResultAttribute)))
            {
                //开始执行前，先判断缓存是否存在
                var cacheKey = $"{invocation.Method.DeclaringType.FullName}.{invocation.Method.Name}_{ParamHash.GetParametHash(invocation.Arguments)}";
                if (!_memoryCache.TryGetValue(cacheKey, out object cacheValue))
                {
                    //缓存不存在，执行方法
                    invocation.Proceed();
                    //获取方法执行结果
                    var result = invocation.ReturnValue;
                    //获取缓存时间
                    var cacheAttribute = invocation.MethodInvocationTarget.GetCustomAttributes(true).FirstOrDefault(a => a.GetType() == typeof(CacheResultAttribute)) as CacheResultAttribute;
                    var cacheDuration = cacheAttribute.Duration;
                    //设置缓存
                    var cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(cacheDuration)
                    };
                    _memoryCache.Set(cacheKey, result, cacheEntryOptions);
                }
                else
                {
                    //缓存存在，直接返回
                    invocation.ReturnValue = cacheValue;
                }
            }
        }
    }
    /// <summary>
    /// 代理服务接口(会自动注入)，所有实现该接口的类都会被拦截，拦截器会判断是否有CacheResult特性
    /// </summary>
    public interface IProxyService
    {

    }
}