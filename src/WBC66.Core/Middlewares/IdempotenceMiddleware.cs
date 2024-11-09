using Microsoft.AspNetCore.Http;

namespace WBC66.Core
{
    /// <summary>
    /// 幂等性过滤器
    /// </summary>
    public class IdempotenceMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Dictionary<string, bool> _processedRequests = new Dictionary<string, bool>();

        /// <summary>
        /// 构造函数，初始化中间件
        /// </summary>
        /// <param name="next"></param>
        public IdempotenceMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// 中间件的核心方法，处理传入的HTTP请求
        /// </summary>
        /// <param name="context">HTTP上下文</param>
        /// <returns>异步任务</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            // 检查请求是否包含幂等性键
            if (context.Request.Headers.TryGetValue("Idempotency-Key", out var idempotencyKey))
            {
                // 处理幂等性逻辑，例如检查请求是否已经处理过
                if (await IsRequestProcessedAsync(idempotencyKey))
                {
                    // 如果请求已经处理过，返回409冲突状态码
                    context.Response.StatusCode = StatusCodes.Status409Conflict;
                    return;
                }

                // 标记请求为已处理
                await MarkRequestAsProcessedAsync(idempotencyKey);
            }

            // 调用下一个中间件
            await _next(context);
        }

        /// <summary>
        /// 检查请求是否已经处理过的具体实现
        /// </summary>
        /// <param name="idempotencyKey">幂等性键</param>
        /// <returns>请求是否已处理的布尔值</returns>
        private Task<bool> IsRequestProcessedAsync(string idempotencyKey)
        {
            return Task.FromResult(_processedRequests.ContainsKey(idempotencyKey));
        }

        /// <summary>
        /// 标记请求为已处理的具体实现
        /// </summary>
        /// <param name="idempotencyKey">幂等性键</param>
        /// <returns>异步任务</returns>
        private Task MarkRequestAsProcessedAsync(string idempotencyKey)
        {
            _processedRequests[idempotencyKey] = true;
            return Task.CompletedTask;
        }
    }
}