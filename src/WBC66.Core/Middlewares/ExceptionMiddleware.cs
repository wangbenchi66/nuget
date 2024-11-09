using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

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
                _logger.LogError(ex, ex.Message);
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
            context.Response.ContentType = "application/json";
            //状态码如果是200，修改为500，如果不是200，不修改
            if (context.Response.StatusCode == (int)HttpStatusCode.OK)
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            var requestId = context.TraceIdentifier;

            await context.Response.WriteAsync(JsonConvert.SerializeObject(new
            {
                code = context.Response.StatusCode,
                message = "Internal Server Error.",
                requestId = requestId
            }));
        }
    }
}