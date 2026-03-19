using System.Net;
using System.Security;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace WBC66.Core;


/// <summary>
/// 异常中间件
/// </summary>
public class ExceptionMiddleware
{
    private const string JsonContentType = "application/json";
    private const int MaxRequestBodyLogLength = 4096;
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            // 处理异常
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        try
        {
            if (exception is UnauthorizedAccessException)
            {
                await WriteFailAsync(context, "用户未登录", HttpStatusCode.Unauthorized, StatusCodes.Status401Unauthorized);
                return;
            }

            if (exception is SecurityException)
            {
                await WriteFailAsync(context, "没有权限访问", HttpStatusCode.Forbidden, StatusCodes.Status403Forbidden);
                return;
            }

            //根据状态码判断 401 403等异常不记录日志，直接返回
            if (context.Response.StatusCode == StatusCodes.Status401Unauthorized || context.Response.StatusCode == StatusCodes.Status403Forbidden)
            {
                return;
            }

            //获取异常参数写入日志
            var request = context.Request;
            string path = request.Path;
            string method = request.Method;
            string req = await GetRequestLog(context);

            string logTemplate =
                    "========== 请求异常日志 ==========\n" +
                    "【IP地址】 {IP}\n" +
                    "【请求路径】 {Path}\n" +
                    "【请求内容】 {Req}\n" +
                    "【异常信息】 {Exception}\n";

            _logger.LogError(
                logTemplate,
                new object?[]
                {
                        context.GetClientIp(),
                        context.GetRequestUrl(),
                        req,
                        exception.ToString()
                });

            if (!context.Response.HasStarted)
            {
                await WriteFailAsync(context, "系统异常，请稍后再试", HttpStatusCode.InternalServerError, StatusCodes.Status500InternalServerError);
            }
        }
        catch (Exception writeEx)
        {
            _logger.LogError(writeEx, "写入异常响应时出错");
        }
    }

    private static async Task WriteFailAsync(HttpContext context, string message, HttpStatusCode code, int statusCode)
    {
        if (context.Response.HasStarted)
        {
            return;
        }

        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = JsonContentType;
        var result = ApiResult.Fail(message, null, code);
        await context.Response.WriteAsJsonAsync(result);
    }

    private static async Task<string> GetRequestLog(HttpContext context)
    {
        var req = context.Request;
        var body = string.Empty;

        if (req.ContentLength.GetValueOrDefault() > 0 && req.Body.CanRead)
        {
            try
            {
                req.EnableBuffering();
                req.Body.Position = 0;
                using var reader = new StreamReader(req.Body, Encoding.UTF8, leaveOpen: true);
                body = await reader.ReadToEndAsync();
            }
            catch
            {
                body = "<读取请求体失败>";
            }
            finally
            {
                if (req.Body.CanSeek)
                {
                    req.Body.Position = 0;
                }
            }
        }

        if (body.Length > MaxRequestBodyLogLength)
        {
            body = body[..MaxRequestBodyLogLength] + "...(内容过长已省略)";
        }

        return $"Query: {req.QueryString} Body: {body}";
    }
}