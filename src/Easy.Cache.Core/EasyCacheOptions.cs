namespace Easy.Cache.Core;

public class EasyCacheOptions
{
    public const string SectionName = "RedisConfigurations";

    /// <summary>
    /// 多 Redis 实例配置
    /// - 单节点：ConnectionString
    /// - 哨兵：ConnectionString + SentinelNodes（推荐）
    /// - 也支持在 ConnectionString 中直接写 sentinel=xxx:26379（可多个）
    /// </summary>
    public Dictionary<string, RedisConnectionOptions>? RedisClients { get; set; }
}

public class RedisConnectionOptions
{
    public string? ConnectionString { get; set; }

    /// <summary>
    /// 哨兵地址列表，如：192.168.1.10:26379
    /// </summary>
    public List<string>? SentinelNodes { get; set; }
}