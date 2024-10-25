using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar.IOC;

namespace WBC66.SqlSugar.Core
{
    /// <summary>
    /// sqlsugar服务
    /// </summary>
    public static class SqlSugarSetup
    {
        /// <summary>
        /// 添加SqlSugar服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration">配置文件</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void AddSqlSugarSetup(this IServiceCollection services, ConfigurationManager configuration)
        {
            if (services == null) { throw new ArgumentNullException(nameof(services)); }
            var config = configuration.GetSection("DBS").Get<List<IocConfig>>();
            if (config == null)
                throw new ArgumentNullException("请检查是否配置数据库连接字符串");
            SugarIocServices.AddSqlSugar(config);
        }
    }
}