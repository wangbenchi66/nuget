using System.Net;

namespace Easy.Common.Core
{
    /// <summary>
    /// api返回帮助类
    /// </summary>
    public class ApiResult : ApiResult<object>
    {
        /// <summary>
        /// 成功（无数据）
        /// </summary>
        public static ApiResult Ok(string? message = null)
        {
            return new ApiResult
            {
                StateCode = HttpStatusCode.OK,
                Success = true,
                Msg = message ?? "操作成功",
                Data = null
            };
        }

        /// <summary>
        /// 成功（有数据）
        /// </summary>
        public static ApiResult Ok(string? message = null, object? data = null)
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
        public static ApiResult Fail(string errorInfo, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        {
            return new ApiResult
            {
                StateCode = statusCode,
                Success = false,
                Msg = "操作失败",
                ErrorInfo = errorInfo,
                Data = null
            };
        }

        /// <summary>
        /// 失败（带数据返回）
        /// </summary>
        public static ApiResult Fail(string errorInfo, object? data, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        {
            return new ApiResult
            {
                StateCode = statusCode,
                Success = false,
                Msg = "操作失败",
                ErrorInfo = errorInfo,
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

        /// <summary>
        /// 失败信息
        /// </summary>
        public string? ErrorInfo { set; get; }

        /*
                /// <summary>
                /// 执行结果
                /// </summary>
                /// <param name="context"></param>
                /// <returns></returns>
                public Task ExecuteResultAsync(ActionContext context)
                {
                    //如果有NoApiResultAttribute特性，则直接返回
                    if (context.ActionDescriptor.EndpointMetadata.Any(m => m.GetType() == typeof(NoApiResultAttribute)))
                    {
                        return Task.CompletedTask;
                    }
                    HttpResponse response = context.HttpContext.Response;
                    if (context.HttpContext.Response.ContentType != null)
                        response.ContentType = context.HttpContext.Response.ContentType;
                    else
                        response.ContentType = "application/json;charset=utf-8";
                    return Task.FromResult(response.WriteAsync(JsonHelper.ToJson(this)));
                }*/
    }
}