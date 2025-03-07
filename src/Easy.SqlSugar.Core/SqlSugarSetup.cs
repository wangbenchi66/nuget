using Microsoft.Extensions.DependencyInjection;
using SqlSugar;
using SqlSugar.IOC;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Http;

namespace Easy.SqlSugar.Core
{
    /// <summary>
    /// sqlsugar服务
    /// </summary>
    public static class SqlSugarSetup
    {
        /// <summary>
        /// 添加SqlSugar服务(Ioc)
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configs">配置文件</param>
        /// <param name="enableAopLogging">开启日志输出</param>
        /// <param name="aopConfigAction">AOP配置</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void AddSqlSugarIocSetup(this IServiceCollection services, List<IocConfig> configs, bool enableAopLogging = false, Action<SqlSugarClient> aopConfigAction = null)
        {
            if (services == null) { throw new ArgumentNullException(nameof(services)); }
            if (configs == null)
                throw new ArgumentNullException("请检查是否配置数据库连接字符串");
            SqlSugarContext.Options = new SqlSugarOptions() { IocConfigs = configs, Logger = enableAopLogging };
            SugarIocServices.AddSqlSugar(configs);
            if (aopConfigAction != null)
            {
                SugarIocServices.ConfigurationSugar(aopConfigAction);
            }
            else
            {
                if (enableAopLogging)
                {
                    SugarIocServices.ConfigurationSugar(db =>
                    {
                        // 设置AOP事件
                        db.Aop.OnLogExecuting = (sql, pars) =>
                        {
                            // 调用您自定义的日志处理方法
                            SqlSugarAop.OnLogExecuting(db, sql, pars);
                        };
                        db.Aop.OnLogExecuted = (sql, pars) =>
                        {
                            SqlSugarAop.OnLogExecuted(db, sql, pars);
                        };
                        db.Aop.OnError = ex =>
                        {
                            SqlSugarAop.OnError(ex);
                        };
                    });
                }
            }
        }

        public static void AddSqlSugarIocSetup(this IServiceCollection services, List<ConnectionConfig> configs)
        {
            if (services == null) { throw new ArgumentNullException(nameof(services)); }
            if (configs == null)
                throw new ArgumentNullException("请检查是否配置数据库连接字符串");
            services.AddHttpContextAccessor();
            AppService.Services = services;
            services.AddSingleton<ISqlSugarClient>(s =>
            {
                return new SqlSugarClient(configs);
            });
            var bseSqlSugarRepositorytypes = GetAssemblyList("BaseSqlSugarIocRepository");
            foreach (var type in bseSqlSugarRepositorytypes)
            {
                services.TryAddSingleton(type);
            }
        }

        /// <summary>
        /// SqlSugar服务 Singleton单例
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configs"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void AddSqlSugarSingletonSetup(this IServiceCollection services, List<ConnectionConfig> configs)
        {
            if (services == null) { throw new ArgumentNullException(nameof(services)); }
            if (configs == null)
                throw new ArgumentNullException("请检查是否配置数据库连接字符串");
            services.AddHttpContextAccessor();
            AppService.Services = services;
            services.AddSingleton<ISqlSugarClient>(s =>
            {
                return new SqlSugarClient(configs);
            });
            /*services.AddSingleton(typeof(BaseSqlSugarRepository<>));
            services.AddSingleton(typeof(SimpleClient<>));*/
            var bseSqlSugarRepositorytypes = GetAssemblyList();
            foreach (var type in bseSqlSugarRepositorytypes)
            {
                services.TryAddSingleton(type);
            }
            //注入IBaseSqlSugarRepository和BaseSqlSugarRepository 可以直接使用
            services.AddSingleton(typeof(IBaseSqlSugarRepository<>), typeof(BaseSqlSugarRepository<>));
            services.AddSingleton(typeof(BaseSqlSugarRepository<>));
        }

        /// <summary>
        /// SqlSugar服务 Scoped作用域
        /// 所有依赖BaseSqlSugarRepository的类会自动注入
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configs"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void AddSqlSugarScopedSetup(this IServiceCollection services, List<ConnectionConfig> configs)
        {
            if (services == null) { throw new ArgumentNullException(nameof(services)); }
            if (configs == null)
                throw new ArgumentNullException("请检查是否配置数据库连接字符串");
            services.AddHttpContextAccessor();
            AppService.Services = services;
            services.AddScoped<ISqlSugarClient>(s =>
            {
                return new SqlSugarScope(configs);
            });
            /*services.AddScoped(typeof(BaseSqlSugarRepository<>));
            services.AddScoped(typeof(SimpleClient<>));*/
            var bseSqlSugarRepositorytypes = GetAssemblyList();
            foreach (var type in bseSqlSugarRepositorytypes)
            {
                services.TryAddScoped(type);
            }
            //注入IBaseSqlSugarRepository和BaseSqlSugarRepository 可以直接使用
            services.AddScoped(typeof(IBaseSqlSugarRepository<>), typeof(BaseSqlSugarRepository<>));
            services.AddScoped(typeof(BaseSqlSugarRepository<>));
        }

        private static IEnumerable<Type> GetAssemblyList(string name = "BaseSqlSugarRepository")
        {
            var assembly = Assembly.GetEntryAssembly();
            return assembly.GetTypes().Where(t => t.BaseType != null && t.BaseType.Name == name);
        }
    }
}