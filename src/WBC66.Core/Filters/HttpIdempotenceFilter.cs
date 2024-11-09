using System.Reflection;
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
        /// 执行过滤器逻辑
        /// </summary>
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
        private async Task<bool> IsRequestIdempotentAsync(HttpContext context)
        {
            var requestId = context.Request.Headers["Idempotency-Key"].FirstOrDefault();
            if (string.IsNullOrEmpty(requestId))
            {
                _logger.LogWarning("缺少 Idempotency-Key 头。");
                return false;
            }

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
        public TimeSpan TimeSpan { get; }

        public IdempotenceTimeAttribute(int hours, int minutes, int seconds)
        {
            TimeSpan = new TimeSpan(hours, minutes, seconds);
        }
    }
}