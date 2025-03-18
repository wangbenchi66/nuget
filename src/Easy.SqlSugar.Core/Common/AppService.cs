using Microsoft.Extensions.DependencyInjection;
using SqlSugar;

namespace Easy.SqlSugar.Core;

internal class AppService
{
    public static IServiceCollection Services { get; set; }
}

internal static class Config
{
    public static List<ConnectionConfig> SqlSugarConfigs { get; set; }
}