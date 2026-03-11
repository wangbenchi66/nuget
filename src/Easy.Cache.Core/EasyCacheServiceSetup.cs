using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Easy.Cache.Core;

public static class EasyCacheServiceSetup
{
    /// <summary>
    /// 自定义缓存服务注册
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="configName">配置文件,默认key RedisConfigurations</param>
    /// <param name="defaultClientName">默认客户端名称</param>
    /// <returns></returns>
    public static IServiceCollection AddEasyCacheServiceSetup(this IServiceCollection services, IConfiguration configuration, string configName = EasyCacheOptions.SectionName)
    {
        var section = configuration.GetSection(configName);

        // 内存缓存总是需要
        services.AddMemoryCache();

        services.AddSingleton<IEasyCacheServiceFactory, EasyCacheServiceFactory>();

        var cacheOptions = section.Get<EasyCacheOptions>() ?? new EasyCacheOptions();
        services.Configure<EasyCacheOptions>(section);

        if (cacheOptions.RedisClients != null && cacheOptions.RedisClients.Count > 0)
        {
            // 注册 Redis 工厂（支持多实例）
            services.AddSingleton<EasyRedisClientFactory>();

            // 默认使用配置中的第一个 Redis 客户端
            services.AddSingleton<IEasyCacheService>(sp =>
            {
                var factory = sp.GetRequiredService<IEasyCacheServiceFactory>();
                return factory.Create();
            });
        }
        else
        {
            // 使用内存缓存
            services.AddSingleton<IEasyCacheService, EasyMemoryCacheService>();
        }

        return services;
    }
}