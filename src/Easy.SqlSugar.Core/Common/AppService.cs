using Microsoft.Extensions.DependencyInjection;

namespace Easy.SqlSugar.Core;

internal class SqlSugarAppService
{
    public static IServiceCollection Services { get; set; }

    private static IServiceProvider _servicesProvider;
    private static readonly object _lock = new object();

    /// <summary>
    /// 获取 ServiceProvider（懒加载，确保所有服务注册完成后再 Build）
    /// </summary>
    public static IServiceProvider ServicesProvider
    {
        get
        {
            if (_servicesProvider == null)
            {
                lock (_lock)
                {
                    if (_servicesProvider == null)
                    {
                        _servicesProvider = Services.BuildServiceProvider();
                    }
                }
            }
            return _servicesProvider;
        }
        set
        {
            _servicesProvider = value;
        }
    }
}