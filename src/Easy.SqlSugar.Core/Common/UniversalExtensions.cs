using System.Text;
using SqlSugar;

namespace Easy.SqlSugar.Core
{
    public static class UniversalExtensions
    {
        /// <summary>
        /// 获取文件信息
        /// </summary>
        /// <remarks>
        /// 返回的文件信息格式为：
        /// 位置:文件名,行号:行号
        /// 以过滤 掉行号为0的记录,如果文件层级大于3,则只显示最后3级目录
        /// </remarks>
        /// <param name="stackTraceList"></param>
        /// <returns></returns>
        public static string GetSqlFileInfo(this List<StackTraceInfoItem> stackTraceList)
        {
            StringBuilder stringBuilder = new StringBuilder();
            try
            {
                stackTraceList = stackTraceList.FindAll(x => x.Line > 0);
                if (stackTraceList == null || stackTraceList.Count == 0)
                    return string.Empty;
                foreach (var item in stackTraceList)
                {
                    var fileNameParts = item.FileName.Split('\\');
                    var fileName = fileNameParts.Length >= 3
                        ? fileNameParts[^3..].Aggregate((x, y) => x + "\\" + y)
                        : item.FileName;
                    stringBuilder.Append($"{Environment.NewLine}位置:{fileName},行号:{item.Line}");
                }
            }
            catch
            {
                return string.Empty;
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// db.Aop.OnLogExecuting日志
        /// </summary>
        /// <param name="configId"></param>
        /// <param name="sql"></param>
        /// <param name="p"></param>
        /// <param name="dbType"></param>
        /// <param name="sqlFileInfo">db.Ado.SqlStackTrace.MyStackTraceList.GetSqlFileInfo()</param>
        /// <returns></returns>
        public static string GetSqlInfoString(object configId, string sql, SugarParameter[] p, DbType dbType, string? sqlFileInfo = null)
        {
            return $"{new string('-', 30)}{Environment.NewLine}{DateTime.Now},ConfigId:{configId},{sqlFileInfo},Sql:{Environment.NewLine}{UtilMethods.GetSqlString(dbType, sql, p)}{Environment.NewLine}{new string('-', 30)}";
        }

        /// <summary>
        /// db.Aop.OnError错误日志
        /// </summary>
        /// <param name="configId"></param>
        /// <param name="exp"></param>
        /// <param name="sqlFileInfo">db.Ado.SqlStackTrace.MyStackTraceList()</param>
        /// <returns></returns>
        public static string GetSqlErrorString(object configId, SqlSugarException exp, string? sqlFileInfo = null)
        {
            return $"{new string('-', 30)}错误{Environment.NewLine}{DateTime.Now},ConfigId:{configId},{sqlFileInfo},Sql:{Environment.NewLine}{exp.Sql}{Environment.NewLine}Error:{exp.Message}{Environment.NewLine}{new string('-', 30)}";
        }
    }
}