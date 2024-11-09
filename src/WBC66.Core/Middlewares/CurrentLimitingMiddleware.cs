using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace WBC66.Core
{
    /// <summary>
    /// 限流中间件
    /// </summary>
    public class CurrentLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CurrentLimitingMiddleware> _logger;

        /// <summary>
        /// 定义一个信号量，用于限制并发访问量 允许同时访问的线程数为1，最大允许的线程数为1
        /// </summary>
        /// <returns></returns>
        private static SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        /// <summary>
        /// 构造函数，初始化中间件
        /// </summary>
        /// <param name="next">下一个中间件</param>
        public CurrentLimitingMiddleware(RequestDelegate next, ILogger<CurrentLimitingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// 中间件的核心逻辑
        /// </summary>
        /// <param name="context">HTTP上下文</param>
        /// <returns>异步任务</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            // 检查是否可以进入临界区
            if (!await _semaphore.WaitAsync(0))
            {
                // 如果不能进入，返回429状态码
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                return;
            }

            try
            {
                // 调用下一个中间件
                await _next(context);
            }
            finally
            {
                // 释放信号量
                _semaphore.Release();
            }
        }
    }
}