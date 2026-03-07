using CSRedis;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Easy.Cache.Core;

public class EasyRedisClientFactory : IDisposable
{
    private readonly Dictionary<string, CSRedisClient> _clients = new();
    private readonly string _defaultClientName;
    private readonly ILogger<EasyRedisClientFactory> _logger;

    public EasyRedisClientFactory(IOptions<EasyCacheOptions> cacheOptions, ILogger<EasyRedisClientFactory> logger)
    {
        _logger = logger;
        var options = cacheOptions.Value;

        // 支持多实例
        if (options.RedisClients == null || options.RedisClients.Count == 0)
        {
            throw new InvalidOperationException("没有redis客户端配置，请检查配置文件中的 'RedisConfigurations:RedisClients' 节点");
        }
        _defaultClientName = options.RedisClients.Keys.First();
        foreach (var (name, config) in options.RedisClients)
        {
            InitializeClient(name, config);
        }
    }

    private void InitializeClient(string name, RedisConnectionOptions config)
    {
        if (config == null)
        {
            _logger.LogWarning("Redis 客户端 '{Name}' 配置为空，跳过初始化。", name);
            return;
        }

        if (string.IsNullOrWhiteSpace(config.ConnectionString))
        {
            _logger.LogWarning("Redis 客户端 '{Name}' 的连接字符串为空，跳过初始化。", name);
            return;
        }

        try
        {
            if (_clients.ContainsKey(name))
            {
                _logger.LogWarning("Redis 客户端 '{Name}' 已存在，跳过重复创建。", name);
                return;
            }
            var (csredis, connectionString) = CreateClient(config);
            _clients[name] = csredis;
            _logger.LogInformation("初始化 Redis 客户端 '{Name} 成功'，连接字符串: {ConnStr}", name, MaskConnectionString(connectionString));
        }
        catch (Exception ex)
        {
            var conn = config.ConnectionString ?? string.Empty;
            _logger.LogError(ex, "初始化 Redis 客户端 '{Name}' 失败，连接字符串: {ConnStr}", name, MaskConnectionString(conn));
            throw;
        }
    }

    private static (CSRedisClient client, string usedConnectionString) CreateClient(RedisConnectionOptions config)
    {
        if (string.IsNullOrWhiteSpace(config.ConnectionString))
        {
            throw new InvalidOperationException("Redis 配置无效：ConnectionString 不能为空。");
        }

        var rawConn = config.ConnectionString.Trim();
        var sentinels = ResolveSentinelNodes(config, rawConn);
        if (sentinels.Count == 0)
        {
            return (new CSRedisClient(rawConn), rawConn);
        }

        var connWithoutSentinel = RemoveSentinelSegments(rawConn);
        if (string.IsNullOrWhiteSpace(connWithoutSentinel))
        {
            throw new InvalidOperationException("Redis 哨兵模式配置无效：移除 sentinel 参数后连接字符串为空，请检查配置。");
        }

        return (new CSRedisClient(connWithoutSentinel, sentinels.ToArray()), connWithoutSentinel);
    }

    private static List<string> ResolveSentinelNodes(RedisConnectionOptions config, string connectionString)
    {
        var result = new List<string>();

        if (config.SentinelNodes is { Count: > 0 })
        {
            result.AddRange(config.SentinelNodes.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()));
        }

        var segments = connectionString.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        foreach (var segment in segments)
        {
            var pair = segment.Split('=', 2, StringSplitOptions.TrimEntries);
            if (pair.Length == 2 && pair[0].Equals("sentinel", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(pair[1]))
            {
                result.Add(pair[1].Trim());
            }
        }

        return result.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
    }

    private static string RemoveSentinelSegments(string connectionString)
    {
        var segments = connectionString
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(segment =>
            {
                var pair = segment.Split('=', 2, StringSplitOptions.TrimEntries);
                return !(pair.Length == 2 && pair[0].Equals("sentinel", StringComparison.OrdinalIgnoreCase));
            });

        return string.Join(',', segments);
    }

    /// <summary>
    /// 获取默认 Redis 实例名称（配置中的第一个）
    /// </summary>
    public string GetDefaultClientName() => _defaultClientName;

    public CSRedisClient GetClient(string name)
    {
        return _clients.TryGetValue(name, out var client)
            ? client
            : throw new ArgumentException($"未找到名为“{name}”的 Redis 客户端配置。", nameof(name));
    }

    public IEnumerable<string> GetClientNames() => _clients.Keys;

    public void Dispose()
    {
        foreach (var client in _clients.Values)
        {
            client?.Dispose();
        }
        _clients.Clear();
    }

    private static string MaskConnectionString(string connStr)
    {
        return System.Text.RegularExpressions.Regex.Replace(
            connStr, @"password=([^,\s]+)", "password=***");
    }
}