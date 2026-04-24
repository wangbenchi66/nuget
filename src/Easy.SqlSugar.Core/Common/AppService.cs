using Microsoft.Extensions.DependencyInjection;

namespace Easy.SqlSugar.Core;

internal class SqlSugarAppService
{
    public static IServiceCollection Services { get; set; }
    public static IServiceProvider ServicesProvider { get; set; }
}