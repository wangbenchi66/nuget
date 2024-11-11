using System.Reflection;
using Common.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace WBC66.Core.Filters
{
    /// <summary>
    /// http幂等性过滤器(需要已注入内存缓存)
    /// </summary>
    public class HttpIdempotenceFilter : IAsyncActionFilter
    {
        private readonly ILogger<HttpIdempotenceFilter> _logger;
        private readonly IMemoryCache _cache;

        /// <summary>
        /// http幂等性过滤器
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="cache"></param>
        public HttpIdempotenceFilter(ILogger<HttpIdempotenceFilter> logger, IMemoryCache cache)
        {
            _logger = logger;
            _cache = cache;
        }

        /// <summary>
        /// 方法前执行过滤器逻辑
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (await ShouldCheckIdempotenceAsync(context.HttpContext))
            {
                if (!await IsRequestIdempotentAsync(context.HttpContext))
                {
                    context.HttpContext.Response.StatusCode = StatusCodes.Status200OK;
                    await context.HttpContext.Response.WriteAsync("触发幂等性，重复请求");
                    return;
                }
            }

            await next();
        }

        /// <summary>
        /// 检查请求是否在幂等性时间内
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private async Task<bool> IsRequestIdempotentAsync(HttpContext context)
        {
            //获取请求参数的哈希值
            var requestId = HttpContextHelper.GetParametHash(context.Request.Path, context.Request.QueryString, context.Request.Body);
            if (_cache.TryGetValue(requestId, out _))
            {
                _logger.LogWarning("检测到重复请求。");
                return false;
            }

            _cache.Set(requestId, true, _idempotenceTime);
            return true;
        }

        /// <summary>
        /// 判断方法是否包含幂等性特性
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private async Task<bool> ShouldCheckIdempotenceAsync(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            if (endpoint == null)
            {
                return false;
            }

            var controllerActionDescriptor = endpoint.Metadata.GetMetadata<ControllerActionDescriptor>();
            if (controllerActionDescriptor == null)
            {
                return false;
            }

            var methodInfo = controllerActionDescriptor.MethodInfo;
            var attribute = methodInfo.GetCustomAttribute<IdempotenceTimeAttribute>();
            if (attribute != null)
            {
                _idempotenceTime = attribute.TimeSpan;
                return true;
            }

            return false;
        }

        private TimeSpan _idempotenceTime;
    }

    /// <summary>
    /// 幂等性时间特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class IdempotenceTimeAttribute : Attribute
    {
        /// <summary>
        /// 幂等性时间
        /// </summary>
        public TimeSpan TimeSpan { get; }

        /// <summary>
        /// 幂等性特性
        /// </summary>
        /// <param name="hours">小时</param>
        /// <param name="minutes">分钟</param>
        /// <param name="seconds">秒数</param>
        public IdempotenceTimeAttribute(int hours, int minutes, int seconds)
        {
            TimeSpan = new TimeSpan(hours, minutes, seconds);
        }
    }
}