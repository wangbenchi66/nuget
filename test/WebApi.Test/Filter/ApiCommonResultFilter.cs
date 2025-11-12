using System.Net;
using Easy.Common.Core.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApi.Test.Filter
{
    public class ApiCommonResultFilter : IAsyncResultFilter
    {
        /// <summary>
        /// 处理结果执行异步方法
        /// </summary>
        /// <param name="context">结果执行上下文</param>
        /// <param name="next">结果执行委托</param>
        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            //如果是文件流直接返回
            if (context.Result is FileStreamResult)
            {
                await next();
                return;
            }

            //如果方法有NoApiResultAttribute特性将直接返回
            if (context.ActionDescriptor.EndpointMetadata.Any(m => m.GetType() == typeof(NoApiResultAttribute)))
            {
                await next();
                return;
            }

            // 如果是 ObjectResult 类型
            if (context.Result is ObjectResult objectResult)
            {
                var value = objectResult.Value;

                // 已经是 ApiResult 类型就不再包装
                if (value is ApiResult || value?.GetType().Name.StartsWith("ApiResult") == true)
                {
                    await next();
                    return;
                }

                // 包装为 ApiResult<object>
                ApiResult result = null;
                if (context.HttpContext.Response.StatusCode == (int)HttpStatusCode.OK)
                {
                    result = ApiResult.Ok(value);
                }
                else
                {
                    result = ApiResult.Fail(value?.ToString(), (HttpStatusCode)context.HttpContext.Response.StatusCode);
                }

                context.Result = result.ToIActionResult();
            }

            if (context.HttpContext.Response.ContentType.IsNull())
                context.HttpContext.Response.ContentType = "application/json";

            await next();
        }
    }
}