using SqlSugar;
using SqlSugar.IOC;

namespace Easy.SqlSugar.Core
{
    /// <summary>
    /// SqlSugar 配置选项类
    /// </summary>
    public class SqlSugarOptions
    {
        /// <summary>
        /// IOC 配置列表
        /// /// </summary>
        public List<IocConfig> IocConfigs { get; set; }

        public List<ConnectionConfig> Configs { get; set; }

        /// <summary>
        /// 启用日志
        /// </summary>
        public bool Logger { get; set; }
    }

    /// <summary>
    /// SqlSugar 上下文类
    /// </summary>
    public class SqlSugarContext
    {
        /// <summary>
        /// SqlSugar 配置选项
        /// </summary>
        public static SqlSugarOptions Options { get; set; }
    }
}