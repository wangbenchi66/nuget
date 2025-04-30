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
    public static ApiResult ToApiResult<TSuccess, TError>(this ApiResultPlus<TSuccess, TError> result)
        where TError : ErrorInfo
    {
        return result.Match(
            success =>
            {
                if (success is ApiResult apiResult)
                {
                    return apiResult;
                }
                return ApiResult.Ok(success);
            },
            error => ApiResult.Fail(error.Msg, error.Data)
        );
    }
}