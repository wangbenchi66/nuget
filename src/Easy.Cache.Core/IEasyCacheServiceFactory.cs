namespace Easy.Cache.Core;

public interface IEasyCacheServiceFactory
{
    IEasyCacheService Create(string? redisClientName = null);
}