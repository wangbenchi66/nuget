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
                //开始执行前如果缓存中有数据则直接返回
                if (BeforeExecution(invocation))
                    return;
                invocation.Proceed();
                //方法执行后
                AfterExecution(invocation);
            }
            else
            {
                invocation.Proceed();
            }
        }

        private bool BeforeExecution(IInvocation invocation)
        {
            var cacheKey = GetCacheKey(invocation);
            if (_memoryCache.TryGetValue(cacheKey, out object cacheValue))
            {
                System.Console.WriteLine("缓存命中");
                invocation.ReturnValue = cacheValue;
                return true;
            }
            return false;
        }

        private void AfterExecution(IInvocation invocation)
        {
            if (invocation.Method.ReturnType != typeof(void) || invocation.Method.ReturnType != typeof(Task))
            {
                System.Console.WriteLine("缓存未命中");
                var result = invocation.ReturnValue;
                var cacheAttribute = invocation.MethodInvocationTarget.GetCustomAttributes(true).FirstOrDefault(a => a.GetType() == typeof(CacheResultAttribute)) as CacheResultAttribute;
                var cacheDuration = cacheAttribute.Duration;
                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(cacheDuration)
                };
                var cacheKey = GetCacheKey(invocation);
                _memoryCache.Set(cacheKey, result, cacheEntryOptions);
            }
        }
        /// <summary>
        /// 获取缓存Key
        /// </summary>
        /// <param name="invocation"></param>
        /// <returns></returns>
        string GetCacheKey(IInvocation invocation)
        {
            return $"{invocation.Method.DeclaringType.FullName}.{invocation.Method.Name}_{ParamHash.GetParametHash(invocation.Arguments)}";
        }
    }

    /// <summary>
    /// 动态代理服务接口(会自动注入)，所有实现该接口的类都会被拦截，拦截器会判断是否存在CacheResult特性
    /// </summary>
    public interface IProxyService
    {

    }
}