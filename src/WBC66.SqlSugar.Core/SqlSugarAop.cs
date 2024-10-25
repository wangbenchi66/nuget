using Serilog;
using SqlSugar;
using SqlSugar.Extensions;

namespace WBC66.SqlSugar.Core;

public static class SqlSugarAop
{
    /// <summary>
    /// sql打印
    /// </summary>
    /// <param name="sqlSugarScopeProvider"></param>
    /// <param name="sql"></param>
    /// <param name="p"></param>
    public static void OnLogExecuting(ISqlSugarClient sqlSugarScopeProvider, string sql, SugarParameter[] p)
    {
#if NET8_0_OR_GREATER
        string msg = """
            ConnId:[{@ConnId}]
            完整sql:
            {@sql}
            """;
#else
        string msg = @"
            ConnId:[{ConnId}]
            完整sql:
            {sql}
            ";
#endif
        Log.Information(msg, sqlSugarScopeProvider.CurrentConnectionConfig.ConfigId, GetWholeSql(sql, p));
    }

    /// <summary>
    /// sql参数解码
    /// </summary>
    /// <param name="sql"></param>
    /// <param name="paramArr"></param>
    /// <returns></returns>
    private static string GetWholeSql(string sql, SugarParameter[] paramArr)
    {
        foreach (var param in paramArr)
        {
            sql = sql.Replace(param.ParameterName, $@"'{param.Value.ObjToString()}'");
        }
        return sql;
    }

    /// <summary>
    /// sql执行完成
    /// </summary>
    /// <param name="sqlSugarScopeProvider"></param>
    /// <param name="sql"></param>
    /// <param name="pars"></param>
    public static void OnLogExecuted(ISqlSugarClient sqlSugarScopeProvider, string sql, SugarParameter[] pars)
    {
        //执行时间超过1秒
        if (sqlSugarScopeProvider.Ado.SqlExecutionTime.TotalSeconds > 1)
        {
#if NET8_0_OR_GREATER
            Log.Warning("""
            sql执行耗时：{time}
            sql:{sql}
            """, sqlSugarScopeProvider.Ado.SqlExecutionTime, GetWholeSql(sql, pars));
#else
            Log.Warning(@"
            sql执行耗时：{time}
            sql:{sql}
            ", sqlSugarScopeProvider.Ado.SqlExecutionTime, GetWholeSql(sql, pars));
#endif
        }
    }

    /// <summary>
    /// sql执行错误
    /// </summary>
    /// <param name="exp"></param>
    public static void OnError(SqlSugarException exp)
    {
#if NET8_0_OR_GREATER
        string msg = """
        sql执行错误：{@expMsg}
        sql:{@sql}
        """;
#else
        string msg = @"
        sql执行错误：{expMsg}
        sql:{sql}
        ";
#endif
        Log.Error(msg, exp.Message, UtilMethods.GetNativeSql(exp.Sql, (SugarParameter[])exp.Parametres));
    }
}