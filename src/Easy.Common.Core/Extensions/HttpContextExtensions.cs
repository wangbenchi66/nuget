using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Easy.Common.Core;

/// <summary>
/// HttpContext 扩展方法
/// </summary>
public static class HttpContextExtensions
{
    /// <summary>
    /// 从httpcontext中获取ip地址
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public static string GetClientIp(this HttpContext context)
    {
        // 优先读取 X-Forwarded-For
        var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault<string>();
        if (!string.IsNullOrWhiteSpace(forwardedFor))
        {
            // X-Forwarded-For 可能包含多个 IP，取第一个
            return forwardedFor.Split(',').FirstOrDefault()?.Trim();
        }
        // 其次读取 X-Real-IP
        var realIp = context.Request.Headers["X-Real-IP"].FirstOrDefault<string>();
        if (!string.IsNullOrWhiteSpace(realIp))
        {
            return realIp;
        }
        // 最后使用 RemoteIpAddress
        return context.Connection.RemoteIpAddress?.ToString() ?? "未知IP";
    }

    /// <summary>
    /// 获取当前用户的UserId（假设ClaimType.NameIdentifier存储了UserId）
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public static string GetUserId(this HttpContext context)
    {
        return context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }

    /// <summary>
    /// 获取当前用户的用户名（假设ClaimType.Name存储了用户名）
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public static string GetUserName(this HttpContext context)
    {
        return context.User?.Identity?.Name;
    }

    /// <summary>
    /// 判断当前用户是否已认证
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public static bool IsAuthenticated(this HttpContext context)
    {
        return context.User?.Identity?.IsAuthenticated ?? false;
    }

    /// <summary>
    /// 获取指定的Claim值
    /// </summary>
    /// <param name="context"></param>
    /// <param name="claimType"></param>
    /// <returns></returns>
    public static string GetClaimValue(this HttpContext context, string claimType)
    {
        return context.User?.FindFirst(claimType)?.Value;
    }

    /// <summary>
    /// 获取请求的完整Url
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public static string GetRequestUrl(this HttpContext context)
    {
        var request = context.Request;
        return $"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}";
    }

    /// <summary>
    /// 获取请求的User-Agent
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public static string GetUserAgent(this HttpContext context)
    {
        return context.Request.Headers["User-Agent"].FirstOrDefault();
    }
}