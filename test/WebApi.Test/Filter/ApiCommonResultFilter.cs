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
            var result = context.Result;
            //如果是文件流直接返回||或者有 NoApiResultAttribute 特性就直接返回
            if (result is FileStreamResult || context.ActionDescriptor.EndpointMetadata.Any(m => m is NoApiResultAttribute))
            {
                await next();
                return;
            }
            //非 ObjectResult → 直接放行
            if (result is not ObjectResult objectResult)
            {
                await next();
                return;
            }

            var value = objectResult.Value;
            if (value is null)
            {
                context.Result = ApiResult.Ok(null).ToIActionResult();
                await next();
                return;
            }

            var type = value.GetType();

            //ApiResultPlus<TSuccess, TError> → 转 ApiResult（表达式树委托，最高速）
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ApiResultPlus<,>))
            {
                var apiResult = ApiResultPlusExtensions.Convert(value);
                context.Result = apiResult.ToIActionResult();
                await next();
                return;
            }

            //已经是 ApiResult 或继承 ApiResult → 不包装
            if (value is ApiResult)
            {
                await next();
                return;
            }

            //普通对象 → 包装为 ApiResult
            int status = context.HttpContext.Response.StatusCode;
            ApiResult wrapped = status == StatusCodes.Status200OK ? ApiResult.Ok(value) : ApiResult.Fail(value.ToString(), (HttpStatusCode)status);

            context.Result = wrapped.ToIActionResult();
            //统一确保返回 JSON
            context.HttpContext.Response.ContentType ??= "application/json";
            await next();
        }
    }
}