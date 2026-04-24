using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Infrastructure;

/// <summary>
/// 配置文件帮助类
/// </summary>
public class AppService
{
    private static IConfiguration Configuration;

    public static void Init(IConfiguration configuration)
    {
        Configuration = configuration;
    }
    public static void Init(IServiceProvider services)
    {
        _serviceProvider = services;
    }

    public static IServiceProvider _serviceProvider { get; set; }

    /// <summary>
    /// 安全获取服务，自动按注册的生命周期获取
    /// </summary>
    public static T GetService<T>() where T : class
    {
        if (_serviceProvider == null)
            throw new InvalidOperationException("AppService 未初始化，请在 Program.cs 调用 AppService.Init(app.Services)");

        var service = _serviceProvider.GetService<T>();
        if (service != null) return service;

        // Scoped 或 Transient 服务未在请求 Scope 中
        // 尝试使用 IServiceScopeFactory 创建新的 Scope
        var scopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
        using var scope = scopeFactory.CreateScope();
        return scope.ServiceProvider.GetRequiredService<T>();
    }

    /// <summary>
    /// 尝试获取服务，未注册返回 null
    /// </summary>
    public static T? TryGetService<T>() where T : class
    {
        if (_serviceProvider == null) return null;

        var service = _serviceProvider.GetService<T>();
        if (service != null) return service;

        // Scoped 或 Transient 服务未在请求 Scope 中
        var scopeFactory = _serviceProvider.GetService<IServiceScopeFactory>();
        if (scopeFactory == null) return null;

        using var scope = scopeFactory.CreateScope();
        return scope.ServiceProvider.GetService<T>();
    }

    /// <summary>
    /// 获取配置文件中的内容
    /// </summary>
    /// <param name="strings">配置项的路径</param>
    /// <returns>配置项的值</returns>
    public static string GetContent(params string[] strings)
    {
        try
        {
            if (strings.Any())
            {
                return Configuration[string.Join(":", strings)];
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        return "";
    }

    /// <summary>
    /// 获取配置文件中的内容并绑定到指定类型
    /// </summary>
    /// <typeparam name="T">要绑定的类型</typeparam>
    /// <param name="strings">配置项的路径</param>
    /// <returns>绑定到指定类型的配置项的值</returns>
    public static T GetContent<T>(params string[] strings)
    {
        try
        {
            if (strings.Any())
            {
                T list = Activator.CreateInstance<T>();
                Configuration.Bind(string.Join(":", strings), list);
                return list;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        return default(T);
    }

    /// <summary>
    /// 获取当前环境名称
    /// </summary>
    public static string GetEnvironment()
    {
        // 优先读取 ASPNETCORE_ENVIRONMENT
        var env = Configuration?["ASPNETCORE_ENVIRONMENT"];
        if (string.IsNullOrWhiteSpace(env))
        {
            // 再读取自定义环境变量
            env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        }

        return string.IsNullOrWhiteSpace(env) ? "Production" : env;
    }

    /// <summary>
    /// 是否为生产环境
    /// </summary>
    public static bool IsProduction() => string.Equals(GetEnvironment(), "Production", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// 是否为开发环境
    /// </summary>
    public static bool IsDevelopment() => string.Equals(GetEnvironment(), "Development", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// 是否为测试环境
    /// </summary>
    public static bool IsStaging() => string.Equals(GetEnvironment(), "Staging", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// 获取项目名称
    /// </summary>
    /// <returns></returns>
    public static string GetProjectName()
    {
        var projectName = Configuration?["ApplicationName"];
        if (string.IsNullOrWhiteSpace(projectName))
        {
            projectName = Environment.GetEnvironmentVariable("ApplicationName");
        }
        return string.IsNullOrWhiteSpace(projectName) ? "UnknownProject" : projectName;
    }
    /// <summary>
    /// 获取项目名称转换为小写并且移除符号
    /// </summary>
    /// <returns></returns>
    public static string GetProjectNameToLower()
    {
        var projectName = GetProjectName();
        //移除所有特殊符号 不仅是.和_，还包括其他常见的特殊符号
        var regex = new System.Text.RegularExpressions.Regex("[^a-zA-Z0-9]");
        return regex.Replace(projectName, "").ToLower();
    }

    /// <summary>
    /// 获取/home/logger/小写不带特殊字符的项目名称路径
    /// </summary>
    /// <returns></returns>
    public static string HomeLoggerProjectNamePath => Path.Combine("/", "home", "logger", GetProjectNameToLower());
}