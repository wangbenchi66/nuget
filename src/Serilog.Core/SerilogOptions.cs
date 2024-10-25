/*----------------------------------------------------------------
 * 命名空间：NetCore.Core.Setups
 * 文件名：SerilogOptions
 * 创建者：WangBenChi
 * 电子邮箱：69945864@qq.com
 * 创建时间：2023/11/13 21:37:19
 *----------------------------------------------------------------*/

namespace Serilog.Core
{
    /// <summary>
    /// Serilog 配置选项。
    /// </summary>
    public class SerilogOptions
    {
        /// <summary>
        /// 配置文件中 Serilog 选项的位置。
        /// </summary>
        public const string Position = "SerilogOptions";

        /// <summary>
        /// 最小日志记录级别。
        /// </summary>
        public string MinimumLevel { get; set; }

        /// <summary>
        /// 特定命名空间的日志记录级别覆盖。
        /// </summary>
        public Dictionary<string, string> Override { get; set; }

        /// <summary>
        /// 文件日志记录选项。
        /// </summary>
        public FileOptions File { get; set; }

        /// <summary>
        /// 控制台日志记录选项。
        /// </summary>
        public ConsoleOptions Console { get; set; }

        /// <summary>
        /// Elasticsearch 日志记录选项。
        /// </summary>
        public ElasticsearchOptions Elasticsearch { get; set; }
    }

    /// <summary>
    /// 文件日志记录选项。
    /// </summary>
    public class FileOptions
    {
        /// <summary>
        /// 日志文件的路径。
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 日志文件的滚动间隔。
        /// </summary>
        public string RollingInterval { get; set; }
    }

    /// <summary>
    /// 控制台日志记录选项。
    /// </summary>
    public class ConsoleOptions
    {
        /// <summary>
        /// 该值指示是否启用控制台日志记录。
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// 最小日志记录级别。
        /// </summary>
        public string Minlevel { get; set; }

        /// <summary>
        /// 日志输出模板。
        /// </summary>
        public string Template { get; set; }
    }

    /// <summary>
    /// Elasticsearch 日志记录选项。
    /// </summary>
    public class ElasticsearchOptions
    {
        /// <summary>
        /// Elasticsearch 服务器的 URI。
        /// </summary>
        public string Uri { get; set; }

        /// <summary>
        /// 索引格式。
        /// </summary>
        public string IndexFormat { get; set; }

        /// <summary>
        /// 分片数量。
        /// </summary>
        public int NumberOfShards { get; set; }

        /// <summary>
        /// 副本数量。
        /// </summary>
        public int NumberOfReplicas { get; set; }

        /// <summary>
        /// 用户名。
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 密码。
        /// </summary>
        public string Password { get; set; }
    }
}