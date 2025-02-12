﻿using Microsoft.Extensions.DependencyInjection;
using SqlSugar;
using SqlSugar.IOC;
using Easy.SqlSugar.Core;

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

        /// <summary>
        /// SqlSugar服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configs"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void AddSqlSugarSetup(this IServiceCollection services, List<ConnectionConfig> configs)
        {
            if (services == null) { throw new ArgumentNullException(nameof(services)); }
            if (configs == null)
                throw new ArgumentNullException("请检查是否配置数据库连接字符串");
            //services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            var db = new SqlSugarClient(configs);
            services.AddSingleton<ISqlSugarClient>(s =>
            {
                SqlSugarScope Db = new SqlSugarScope(configs);
                return Db;
            });
            services.AddScoped(typeof(BaseSqlSugarRepository<>));
        }
    }
}