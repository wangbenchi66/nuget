using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Routing;
using System.Diagnostics;

namespace WBC66.Core
{
    /// <summary>
    /// 日志中间件
    /// </summary>
    public class LogMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LogMiddleware> _logger;

        public LogMiddleware(RequestDelegate next, ILogger<LogMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var routeData = context.GetRouteData();
            var action = routeData.Values["action"];
            var requestId = context.TraceIdentifier;
            var logMessage = $"请求ID: {requestId}\n请求路径: {context.Request.Path}\n操作: {action}";

            // 记录请求参数
            logMessage = await LogRequestParameters(context, logMessage);

            // 记录响应
            logMessage = await LogResponse(context, logMessage);

            stopwatch.Stop();
            var elapsedSeconds = stopwatch.Elapsed.TotalSeconds;
            logMessage += $"\n请求处理时间: {elapsedSeconds} 秒";

            // 根据状态码设置日志级别
            var statusCode = context.Response.StatusCode;
            if (statusCode >= 500)
            {
                _logger.LogError(logMessage);
            }
            else if (statusCode >= 400)
            {
                _logger.LogWarning(logMessage);
            }
            else
            {
                _logger.LogInformation(logMessage);
            }
        }

        /// <summary>
        /// 记录请求参数
        /// </summary>
        /// <param name="context">HTTP上下文</param>
        /// <param name="logMessage">日志消息</param>
        /// <returns>更新后的日志消息</returns>
        private async Task<string> LogRequestParameters(HttpContext context, string logMessage)
        {
            if (context.Request.Method == HttpMethods.Get)
            {
                var queryParams = context.Request.QueryString.Value;
                logMessage += $"\n查询参数: {queryParams}";
            }
            else if (context.Request.Method == HttpMethods.Post)
            {
                context.Request.EnableBuffering();
                var body = await new StreamReader(context.Request.Body).ReadToEndAsync();
                context.Request.Body.Position = 0;
                logMessage += $"\n请求体: {body}";
            }
            return logMessage;
        }

        /// <summary>
        /// 记录响应
        /// </summary>
        /// <param name="context">HTTP上下文</param>
        /// <param name="logMessage">日志消息</param>
        /// <returns>更新后的日志消息</returns>
        private async Task<string> LogResponse(HttpContext context, string logMessage)
        {
            var originalBodyStream = context.Response.Body;
            using (var responseBody = new MemoryStream())
            {
                context.Response.Body = responseBody;

                await _next(context);

                context.Response.Body.Seek(0, SeekOrigin.Begin);
                var responseText = await new StreamReader(context.Response.Body).ReadToEndAsync();
                context.Response.Body.Seek(0, SeekOrigin.Begin);

                if (context.Response.ContentType != null && context.Response.ContentType.Contains("application/octet-stream"))
                {
                    var fileSize = responseBody.Length;
                    logMessage += $"\n响应文件大小: {fileSize} 字节";
                }
                else
                {
                    logMessage += $"\n响应:\n{responseText}";
                    //状态码
                    logMessage += $"\n状态码: {context.Response.StatusCode}";
                }

                await responseBody.CopyToAsync(originalBodyStream);
            }
            return logMessage;
        }
    }
}