using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SqlSugar;

namespace Easy.SqlSugar.Core
{
    /// <summary>
    /// sqlsugar服务
    /// </summary>
    public static class SqlSugarSetup
    {

        /*/// <summary>
        /// SqlSugar服务 Singleton单例
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configs"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void AddSqlSugarSingletonSetup(this IServiceCollection services, SqlSugarScope configs)
        {
            if (services == null) { throw new ArgumentNullException(nameof(services)); }
            if (configs == null)
                throw new ArgumentNullException("请检查是否配置数据库连接字符串");
            services.AddSingleton<ISqlSugarClient>(s =>
            {
                return configs;
            });
            AddService(services, 1);
        }*/

        /*/// <summary>
        /// SqlSugar服务 Singleton单例
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configs"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void AddSqlSugarSingletonSetup(this IServiceCollection services, SqlSugarClient configs)
        {
            if (services == null) { throw new ArgumentNullException(nameof(services)); }
            if (configs == null)
                throw new ArgumentNullException("请检查是否配置数据库连接字符串");
            services.AddSingleton<ISqlSugarClient>(s =>
            {
                return configs;
            });
            AddService(services, 1);
        }*/

        /// <summary>
        /// SqlSugar服务 Scoped作用域(首选)
        /// 所有依赖BaseSqlSugarRepository的类会自动注入
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configs"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void AddSqlSugarScopedSetup(this IServiceCollection services, SqlSugarScope configs)
        {
            if (services == null) { throw new ArgumentNullException(nameof(services)); }
            if (configs == null)
                throw new ArgumentNullException("请检查是否配置数据库连接字符串");
            //配置文件加工
            configs = ConfigProcessing(configs);
            services.AddScoped<ISqlSugarClient>(s =>
            {
                return configs;
            });
            AddService(services, 2);
        }

        /// <summary>
        /// SqlSugar服务 Scoped作用域
        /// 所有依赖BaseSqlSugarRepository的类会自动注入
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configs"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static void AddSqlSugarScopedSetup(this IServiceCollection services, SqlSugarClient configs)
        {
            if (services == null) { throw new ArgumentNullException(nameof(services)); }
            if (configs == null)
                throw new ArgumentNullException("请检查是否配置数据库连接字符串");
            //配置文件加工
            configs = ConfigProcessing(configs);
            services.AddScoped<ISqlSugarClient>(s =>
            {
                return configs;
            });
            AddService(services, 2);
        }

        private static SqlSugarScope ConfigProcessing(SqlSugarScope configs)
        {
            //用反射获取_configs，单个的获取补到多库
            var field = typeof(SqlSugarScope).GetField("_configs", BindingFlags.NonPublic | BindingFlags.Instance);
            var _configs = field.GetValue(configs) as List<ConnectionConfig>;
            ConfigProcessing(_configs);
            return configs;
        }
        private static SqlSugarClient ConfigProcessing(SqlSugarClient configs)
        {
            //用反射获取_configs，单个的获取补到多库
            var field = typeof(SqlSugarClient).GetField("_configs", BindingFlags.NonPublic | BindingFlags.Instance);
            var _configs = field.GetValue(configs) as List<ConnectionConfig>;
            ConfigProcessing(_configs);
            return configs;
        }

        /// <summary>
        /// 配置加工
        /// </summary>
        /// <param name="configs"></param>
        private static void ConfigProcessing(List<ConnectionConfig> configs)
        {
            foreach (var item in configs)
            {
                item.ConfigureExternalServices ??= new ConfigureExternalServices();
                item.ConfigureExternalServices.SqlFuncServices ??= UniversalExtensions.GetSqlFuncExternals();
            }
        }

        /// <summary>
        /// 服务注入
        /// </summary>
        /// <param name="services"></param>
        /// <param name="lifecycleType">1:Singleton 2:Scoped</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <remarks>默认Singleton</remarks>
        /// <returns></returns>
        private static void AddService(IServiceCollection services, int lifecycleType = 1)
        {
            //services.AddHttpContextAccessor();
            AppService.Services = services;
            var bseSqlSugarRepositorytypes = GetAssemblyList();
            if (lifecycleType == 1)
            {
                foreach (var type in bseSqlSugarRepositorytypes)
                {
                    services.TryAddSingleton(type);
                }
                services.AddSingleton(typeof(IBaseSqlSugarRepository<>), typeof(BaseSqlSugarRepository<>));
                services.AddSingleton(typeof(BaseSqlSugarRepository<>));
                services.AddSingleton(typeof(IBaseSqlSugarService<>), typeof(BaseSqlSugarService<>));
                services.AddSingleton(typeof(BaseSqlSugarService<>));
            }
            else if (lifecycleType == 2)
            {
                foreach (var type in bseSqlSugarRepositorytypes)
                {
                    services.TryAddScoped(type);
                }
                services.AddScoped(typeof(IBaseSqlSugarRepository<>), typeof(BaseSqlSugarRepository<>));
                services.AddScoped(typeof(BaseSqlSugarRepository<>));
                services.AddScoped(typeof(IBaseSqlSugarService<>), typeof(BaseSqlSugarService<>));
                services.AddScoped(typeof(BaseSqlSugarService<>));
            }
            //雪花id设置,雪花id必须是0-31之内
            SnowFlakeSingle.WorkId = UniversalExtensions.GetRandomWorkId();

        }

        private static IEnumerable<Type> GetAssemblyList(string name = "BaseSqlSugarRepository")
        {
            var assembly = Assembly.GetEntryAssembly();
            return assembly.GetTypes().Where(t => t.BaseType != null && t.BaseType.Name == name);
        }
    }
}