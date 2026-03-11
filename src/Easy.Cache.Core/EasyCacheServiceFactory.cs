using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Easy.Cache.Core;

public class EasyCacheServiceFactory : IEasyCacheServiceFactory
{
    private readonly EasyRedisClientFactory? _redisClientFactory;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<EasyCacheServiceFactory> _logger;

    public EasyCacheServiceFactory(
        IMemoryCache memoryCache,
        IServiceProvider serviceProvider,
        ILogger<EasyCacheServiceFactory> logger)
    {
        _memoryCache = memoryCache;
        _logger = logger;
        _redisClientFactory = serviceProvider.GetService<EasyRedisClientFactory>();

        if (_redisClientFactory == null)
            _logger.LogInformation("缓存模式：Memory（未检测到 Redis 客户端配置）。");
        else
            _logger.LogInformation("缓存模式：Redis（默认客户端：{DefaultClient}）。", _redisClientFactory.GetDefaultClientName());
    }

    public IEasyCacheService Create(string? redisClientName = null)
    {
        if (_redisClientFactory == null)
        {
            //_logger.LogInformation("缓存模式：Memory（未检测到 Redis 客户端配置）。");
            return new EasyMemoryCacheService(_memoryCache);
        }

        var defaultName = _redisClientFactory.GetDefaultClientName();
        var targetName = string.IsNullOrWhiteSpace(redisClientName)
            ? defaultName
            : redisClientName;

        //_logger.LogInformation("缓存模式：Redis（当前客户端：{DefaultClient}）。", targetName);
        CSRedis.CSRedisClient client;
        try
        {
            client = _redisClientFactory.GetClient(targetName);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Redis 客户端：{ClientName} 出现异常，已切换到默认客户端：{DefaultClient}。异常信息：{ErrorMessage}", targetName, defaultName, ex.Message);
            client = _redisClientFactory.GetClient(defaultName);
        }
        return new EasyRedisCacheService(client);
    }
}