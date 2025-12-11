using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace Easy.Common.Core;

/// <summary>
/// 错误信息
/// </summary>
public class ErrorInfo
{

    /// <summary>
    /// 错误消息
    /// </summary>
    public string Msg { get; set; }

    /// <summary>
    /// 错误值
    /// </summary>
    public object Data { get; set; }

    public static ErrorInfo Error(string msg, object data = null)
    {
        return new ErrorInfo
        {
            Msg = msg,
            Data = data
        };
    }
}

/// <summary>
/// 自定义返回结果
/// </summary>
public readonly struct ApiResultPlus<TSuccess, TError>
{
    /// <summary>
    /// 是否成功
    /// </summary>
    public bool IsSuccess { get; }

    private readonly TSuccess? _success;
    private readonly TError? _error;

    /// <summary>
    /// 成功数据
    /// </summary>
    public TSuccess? Data => _success;

    /// <summary>
    /// 错误数据
    /// </summary>
    public TError? Error => _error;

    /// <summary>
    /// 成功结果的构造函数
    /// </summary>
    /// <param name="success"></param>
    private ApiResultPlus(TSuccess success)
    {
        this.IsSuccess = true;
        _success = success;
        _error = default;
    }

    /// <summary>
    /// 错误结果的构造函数
    /// </summary>
    /// <param name="error"></param>
    private ApiResultPlus(TError error)
    {
        this.IsSuccess = false;
        _error = error;
        _success = default;
    }

    /// <summary>
    /// 隐式转换成功结果
    /// </summary>
    /// <param name="success"></param>
    public static implicit operator ApiResultPlus<TSuccess, TError>(TSuccess success) => new(success);

    /// <summary>
    /// 隐式转换错误结果
    /// </summary>
    /// <param name="error"></param>
    public static implicit operator ApiResultPlus<TSuccess, TError>(TError error) => new(error);

    /// <summary>
    /// 匹配结果并返回对应的值
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="success"></param>
    /// <param name="error"></param>
    /// <returns></returns>
    public TResult Match<TResult>(
        Func<TSuccess, TResult> success,
        Func<TError, TResult> error) =>
        IsSuccess ? success(_success!) : error(_error!);
}


/// <summary>
/// ApiResultPlus 扩展方法
/// </summary>
public static class ApiResultPlusExtensions
{
    /// <summary>
    /// 将 ApiResultPlus 匹配结果并转换为 ApiResult
    /// </summary>
    /// <typeparam name="TSuccess">成功结果类型</typeparam>
    /// <typeparam name="TError">错误结果类型</typeparam>
    /// <param name="result">ApiResultPlus 实例</param>
    /// <returns>转换后的 ApiResult</returns>
    //public static ApiResult ToApiResult<TSuccess, TError>(this ApiResultPlus<TSuccess, TError> result)
    //    where TError : ErrorInfo
    //{
    //    return result.Match(
    //        success =>
    //        {
    //            if (success is ApiResult apiResult)
    //            {
    //                return apiResult;
    //            }
    //            return ApiResult.Ok(success);
    //        },
    //        error => ApiResult.Fail(error.Msg, error.Data)
    //    );
    //}
    public static ApiResult ToApiResult<TSuccess, TError>(this ApiResultPlus<TSuccess, TError> result)
    {
        return result.Match(
            success =>
            {
                // 如果成功数据本身就是 ApiResult，直接返回
                if (success is ApiResult apiResult)
                {
                    return apiResult;
                }

                // 否则包装成 ApiResult.Ok
                return ApiResult.Ok(success);
            },
            error =>
            {
                if (error == null)
                    return ApiResult.Fail("操作失败");

                if (error is string s)
                    return ApiResult.Fail(s);

                if (typeof(TError).IsEnum)
                    return ApiResult.Fail(error?.ToString());

                if (error is ErrorInfo ei)
                    return ApiResult.Fail(ei.Msg, ei.Data);

                if (error is Exception ex)
                    return ApiResult.Fail(ex.Message);

                return ApiResult.Fail(null, error);
            }
        );
    }


    private static readonly ConcurrentDictionary<Type, Func<object, ApiResult>> Cache =
        new ConcurrentDictionary<Type, Func<object, ApiResult>>();
    /// <summary>
    /// object 类型的 ApiResultPlus 转 ApiResult
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static ApiResult Convert(object value)
    {
        var type = value.GetType();

        // —— 已缓存，直接 O(1) 返回 ——
        if (Cache.TryGetValue(type, out var func))
            return func(value);

        // —— 构建委托（只构建一次） ——
        var converter = BuildConverter(type);

        Cache[type] = converter; // 缓存

        return converter(value);
    }

    private static Func<object, ApiResult> BuildConverter(Type apiResultPlusType)
    {
        // 获取扩展方法 ToApiResult<TSuccess, TError>
        var method = typeof(ApiResultPlusExtensions)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .First(m => m.Name == "ToApiResult" && m.IsGenericMethod);

        // 构建泛型方法
        var genericMethod = method.MakeGenericMethod(apiResultPlusType.GenericTypeArguments);

        // 参数：object input
        var param = Expression.Parameter(typeof(object), "input");

        // 强制转换 input → ApiResultPlus<TSuccess,TError>
        var cast = Expression.Convert(param, apiResultPlusType);

        // 调用 ToApiResult
        var call = Expression.Call(null, genericMethod, cast);

        // 构建 Func<object, ApiResult>
        return Expression.Lambda<Func<object, ApiResult>>(call, param).Compile();
    }
}