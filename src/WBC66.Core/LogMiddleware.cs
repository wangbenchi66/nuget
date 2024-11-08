using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Routing;
using System.IO;
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
            var controller = routeData.Values["controller"];
            var action = routeData.Values["action"];
            var logMessage = $"请求路径: {context.Request.Path}\n操作: {action}";

            // 记录请求参数
            logMessage = await LogRequestParameters(context, logMessage);

            _logger.LogInformation(logMessage);

            // 记录响应
            await LogResponse(context);

            stopwatch.Stop();
            var elapsedSeconds = stopwatch.Elapsed.TotalSeconds;
            _logger.LogInformation($"请求处理时间: {elapsedSeconds} 秒");
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
        /// <returns>异步任务</returns>
        private async Task LogResponse(HttpContext context)
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
                    _logger.LogInformation($"响应文件大小: {fileSize} 字节");
                }
                else
                {
                    _logger.LogInformation($"响应:\n{responseText}");
                }

                await responseBody.CopyToAsync(originalBodyStream);
            }
        }
    }
}