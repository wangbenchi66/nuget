using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

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

        private static string[] skipPaths =
        {
            "/swagger", "/health", "scalar",".html", ".css", ".js", ".png", ".jpg", ".jpeg", ".gif", ".ico"
        };

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value;

            if (skipPaths.Any(s => path.IndexOf(s, StringComparison.OrdinalIgnoreCase) >= 0))
            {
                await _next(context);
                return;
            }

            // 请求计时器
            var stopwatch = Stopwatch.StartNew();

            context.Request.EnableBuffering();
            var originalBody = context.Response.Body;

            try
            {
                await using var ms = new MemoryStream();
                context.Response.Body = ms;

                // 读取请求内容
                var reqLog = await GetRequestLog(context);

                await _next(context);

                stopwatch.Stop();

                var contentType = context.Response.ContentType ?? string.Empty;
                if (IsBinaryContent(contentType))
                {
                    _logger.LogInformation(
                    "========== 请求日志==========" +
                    "【请求路径】{Path}" +
                    "【请求内容】{ReqData}" +
                    "【耗时】 {Elapsed} ms\n" +
                    "【响应类型】 文件({ContentType})，已跳过内容记录",
                        path, reqLog, contentType, stopwatch.ElapsedMilliseconds);
                }
                else
                {
                    var resLog = await GetResponseLog(ms);
                    var elapsed = stopwatch.ElapsedMilliseconds;

                    // 日志模板统一
                    string logTemplate =
                    "========== 请求响应日志==========" +
                    "【请求路径】 {Path}" +
                    "【请求内容】{ReqData}" +
                    "【耗时】 {Elapsed} ms\n" +
                    "【响应内容】{ResData}\n";
                    // 判断耗时级别
                    var logAction = elapsed switch
                    {
                        > 60000 => _logger.LogError,
                        > 10000 => _logger.LogWarning,
                        _ => ((Action<string, object?[]>)((tpl, args) => _logger.LogInformation(tpl, args)))
                    };
                    // 统一日志输出
                    logAction(
                               logTemplate,
                               new object?[]
                               {
                               path,
                               reqLog,
                               resLog,
                               elapsed
                               });
                }
                ms.Position = 0;
                await ms.CopyToAsync(originalBody);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "请求日志中间件异常");
            }
            finally
            {
                context.Response.Body = originalBody;
            }
        }

        /// <summary>
        /// 读取请求体日志
        /// </summary>
        private static async Task<string> GetRequestLog(HttpContext context)
        {
            var req = context.Request;
            var body = string.Empty;

            if (req.ContentLength > 0 && req.Body.CanRead)
            {
                using var reader = new StreamReader(req.Body, Encoding.UTF8, leaveOpen: true);
                body = await reader.ReadToEndAsync();
                req.Body.Position = 0;
            }

            return $"Query: {req.QueryString} Body: {body}";
        }

        /// <summary>
        /// 读取响应体日志
        /// </summary>
        private static async Task<string> GetResponseLog(MemoryStream ms)
        {
            ms.Position = 0;
            var resText = await new StreamReader(ms, Encoding.UTF8).ReadToEndAsync();
            // 如果是 JSON 转义的 Unicode（例如 \u6D4B\u8BD5），还原成中文
            if (resText.Contains("\\u"))
                resText = Regex.Unescape(resText);

            // 检测是否是 HTML
            if (Regex.IsMatch(resText, "<[^>]+>"))
                resText = "HTML 内容已省略";

            return resText;
        }

        /// <summary>
        /// 判断是否为二进制或文件类型
        /// </summary>
        private static bool IsBinaryContent(string contentType)
        {
            var binaryTypes = new[]
            {
            "application/octet-stream",
            "application/pdf",
            "application/zip",
            "application/x-rar",
            "application/vnd.ms-excel",
            "application/vnd.openxmlformats-officedocument",
            "image/",
            "video/",
            "audio/"
        };

            return binaryTypes.Any(t => contentType.StartsWith(t, StringComparison.OrdinalIgnoreCase));
        }
    }
}