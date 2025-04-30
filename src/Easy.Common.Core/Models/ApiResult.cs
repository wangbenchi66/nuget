using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace Easy.Common.Core
{
    /// <summary>
    /// api返回帮助类
    /// </summary>
    public class ApiResult : ApiResult<object>
    {

        /// <summary>
        /// 成功（有数据）
        /// </summary>
        public static ApiResult Ok(object? data = null, string? message = null)
        {
            return new ApiResult
            {
                StateCode = HttpStatusCode.OK,
                Success = true,
                Msg = message ?? "操作成功",
                Data = data
            };
        }

        /// <summary>
        /// 失败（只返回错误信息）
        /// </summary>
        public static ApiResult Fail(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        {
            return new ApiResult
            {
                StateCode = statusCode,
                Success = false,
                Msg = message ?? "操作失败",
                Data = null
            };
        }

        /// <summary>
        /// 失败（带数据返回）
        /// </summary>
        public static ApiResult Fail(string message, object? data, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        {
            return new ApiResult
            {
                StateCode = statusCode,
                Success = false,
                Msg = message ?? "操作失败",
                Data = data
            };
        }
    }


    /// <summary>
    /// 通用结果
    /// </summary>
    public class ApiResult<T>
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public HttpStatusCode StateCode { get; set; } = HttpStatusCode.OK;

        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; } = true;

        /// <summary>
        /// 返回消息
        /// </summary>
        public string Msg { get; set; } = "操作成功";

        /// <summary>
        /// 用于结果集返回
        /// </summary>
        public T? Data { set; get; }

        /// <summary>
        /// 唯一码
        /// </summary>
        public Guid TraceId { get; set; } = Guid.NewGuid();

        /*
                /// <summary>
                /// 执行结果
                /// </summary>
                /// <param name="context"></param>
                /// <returns></returns>
                public Task ExecuteResultAsync(ActionContext context)
                {
                    if (context.ActionDescriptor is ControllerActionDescriptor cad)
                    {
                        var methodInfo = cad.MethodInfo;
                        if (methodInfo.IsDefined(typeof(NoApiResultAttribute), inherit: true))
                        {
                            return Task.CompletedTask;
                        }
                    }
                    var response = context.HttpContext.Response;
                    response.ContentType ??= "application/json;charset=utf-8";
                    return Task.FromResult(response.WriteAsync(JsonHelper.ToJson(this)));
                }*/

        /// <summary>
        /// 成功返回（泛型）
        /// </summary>
        public static ApiResult<T> Ok(T? data, string? message = "操作成功")
        {
            return new ApiResult<T>
            {
                StateCode = HttpStatusCode.OK,
                Success = true,
                Msg = message ?? "操作成功",
                Data = data
            };
        }

        /// <summary>
        /// 失败返回（泛型）
        /// </summary>
        public static ApiResult<T> Fail(string message, T? data = default, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        {
            return new ApiResult<T>
            {
                StateCode = statusCode,
                Success = false,
                Msg = message ?? "操作失败",
                Data = data
            };
        }
    }

    /// <summary>
    /// ApiResult扩展
    /// </summary>
    public static class ApiResultExtensions
    {
        /// <summary>
        /// 将ApiResult转换为IActionResult
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result"></param>
        /// <returns></returns>
        public static IActionResult ToActionResult<T>(this ApiResult<T> result)
        {
            return new ObjectResult(result)
            {
                StatusCode = (int)result.StateCode,
                DeclaredType = typeof(ApiResult<T>)
            };
        }
    }
}