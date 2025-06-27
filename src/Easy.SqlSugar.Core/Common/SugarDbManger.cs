using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar;

namespace Easy.SqlSugar.Core;

/// <summary>
/// SqlSugar数据库管理类
/// </summary>
public class SugarDbManger
{
    /// <summary>
    /// 获取实例（从注入中获取）
    /// </summary>
    public static ISqlSugarClient Db
    {
        get
        {
            var db = AppService.Services.BuildServiceProvider().GetRequiredService<ISqlSugarClient>();
            if (db == null)
            {
                throw new Exception("SqlSugar未注册");
            }
            return db;
        }
    }

    /*
        /// <summary>
        /// 获取实例(从注入中获取)
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static ISqlSugarClient GetInstance() => Db;*/

    /// <summary>
    /// 获取指定ConfigId实例
    /// </summary>
    /// <param name="configId"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static ISqlSugarClient GetConfigDb(string configId)
    {
        if (string.IsNullOrEmpty(configId))
        {
            new Exception("configId不能为空");
        }
        //尝试转换为SqlSugarScope或SqlSugarClient
        ISqlSugarClient db = Db;
        if (db is SqlSugarScope)
        {
            var scope = (SqlSugarScope)db;
            return scope.GetConnectionScope(configId);
        }
        else if (db is SqlSugarClient)
        {
            var client = (SqlSugarClient)db;
            return client.GetConnectionScope(configId);
        }
        throw new Exception("SqlSugar未注册或配置错误");
    }

    /// <summary>
    /// 根据实体上的TenantId特性获取对应的数据库实例
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static ISqlSugarClient GetTenantDb<T>() => Db.AsTenant().GetConnectionScopeWithAttr<T>();

    /// <summary>
    /// 获取新的数据库实例(会是一个SqlSugarClient新的连接)
    /// </summary>
    /// <returns></returns>
    public static SqlSugarClient GetNewDb() => new SqlSugarClient(GetConnectionConfigs(), GetSqlSugarClientAction());

    /// <summary>
    /// 获取所有连接配置
    /// </summary>
    /// <returns></returns>
    public static List<ConnectionConfig> GetConnectionConfigs()
    {
        var client = Db;
        if (client is SqlSugarScope scope)
        {
            var field = typeof(SqlSugarScope).GetField("_configs", BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null)
            {
                var list = field.GetValue(scope) as List<ConnectionConfig>;
                if (list != null)
                    return list;
            }
        }
        else if (client is SqlSugarClient singleClient)
        {
            var field = typeof(SqlSugarClient).GetField("_AllClients", BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null)
            {
                var list = field.GetValue(singleClient) as List<SugarTenant>;
                if (list != null)
                {
                    return list.Select(s => s.ConnectionConfig).ToList();
                }
            }
        }
        return null;
    }

    /// <summary>
    /// 获取所有连接配置的Action
    /// </summary>
    /// <returns></returns>
    public static Action<SqlSugarClient> GetSqlSugarClientAction()
    {
        var client = Db;
        if (client is SqlSugarScope scope)
        {
            var field = typeof(SqlSugarScope).GetField("_configAction", BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null)
            {
                return field.GetValue(scope) as Action<SqlSugarClient>;
            }
        }
        else if (client is SqlSugarClient singleClient)
        {
            var field = typeof(SqlSugarClient).GetField("_configAction", BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null)
            {
                return field.GetValue(singleClient) as Action<SqlSugarClient>;
            }
        }
        return null;
    }
}