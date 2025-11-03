using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace WBC66.Core
{
    /// <summary>
    /// 异常中间件
    /// </summary>
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        /// <summary>
        /// 异常中间件
        /// </summary>
        /// <param name="next"></param>
        /// <param name="logger"></param>
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// 异常中间件
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        /// <summary>
        /// 异常处理
        /// </summary>
        /// <param name="context"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            try
            {
                //获取异常参数写入日志
                var request = context.Request;
                string path = request.Path;
                string method = request.Method;
                string req = await GetRequestLog(context);

                string logTemplate =
                    "========== 请求异常日志 ==========" +
                    "【时间】 {Time}" +
                    "【请求路径】 {Path}" +
                    "【请求方法】 {Method}" +
                    "【请求内容】 {Req}\n" +
                    "【异常信息】 {Exception}\n";

                _logger.LogError(
                    logTemplate,
                    new object?[]
                    {
                        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        path,
                        method,
                        req,
                        exception.ToString()
                    });

                if (!context.Response.HasStarted)
                {
                    context.Response.Clear();
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    context.Response.ContentType = "application/json";
                    var result = ApiResult.Fail(exception.Message, HttpStatusCode.InternalServerError);
                    //保持原格式
                    var options = new JsonSerializerOptions { PropertyNamingPolicy = null };
                    await context.Response.WriteAsJsonAsync(result, options);
                }
            }
            catch (Exception writeEx)
            {
                _logger.LogError(writeEx, "写入异常响应时出错");
            }
        }

        private static async Task<string> GetRequestLog(HttpContext context)
        {
            var req = context.Request;
            var body = string.Empty;
            req.EnableBuffering();

            if (req.ContentLength > 0 && req.Body.CanRead)
            {
                req.Body.Position = 0;
                using var reader = new StreamReader(req.Body, Encoding.UTF8, leaveOpen: true);
                body = await reader.ReadToEndAsync();
                req.Body.Position = 0;
            }
            return $"Query: {req.QueryString} Body: {body}";
        }
    }
}