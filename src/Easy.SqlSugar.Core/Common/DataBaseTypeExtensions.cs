using SqlSugar;
using SqlSugar.IOC;

namespace Easy.SqlSugar.Core.Common
{
    /// <summary>
    /// 数据库类型扩展
    /// </summary>
    public static class DataBaseTypeExtensions
    {
        /// <summary>
        /// 根据连接字符串获取数据库类型
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <returns></returns>
        public static IocDbType GetDatabaseIocType(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException(nameof(connectionString), "数据库连接字符串不能为空");

            // SQL Server (MSSQL) 区分条件
            if (connectionString.Contains("Data Source", StringComparison.OrdinalIgnoreCase) &&
                connectionString.Contains("Initial Catalog", StringComparison.OrdinalIgnoreCase))
            {
                return IocDbType.SqlServer;
            }

            // MySQL 区分条件
            if (connectionString.Contains("Server", StringComparison.OrdinalIgnoreCase) &&
                connectionString.Contains("Database", StringComparison.OrdinalIgnoreCase))
            {
                if (connectionString.Contains("Uid", StringComparison.OrdinalIgnoreCase) ||
                    connectionString.Contains("User Id", StringComparison.OrdinalIgnoreCase))
                {
                    return IocDbType.MySql;
                }
            }

            // PostgreSQL
            if (connectionString.Contains("Host", StringComparison.OrdinalIgnoreCase) &&
                connectionString.Contains("Database", StringComparison.OrdinalIgnoreCase))
            {
                return IocDbType.PostgreSQL;
            }

            // Oracle
            if (connectionString.Contains("Data Source", StringComparison.OrdinalIgnoreCase) &&
                connectionString.Contains("User Id", StringComparison.OrdinalIgnoreCase))
            {
                return IocDbType.Oracle;
            }

            // SQLite
            if (connectionString.Contains("Data Source", StringComparison.OrdinalIgnoreCase) &&
                connectionString.Contains("Version", StringComparison.OrdinalIgnoreCase))
            {
                return IocDbType.Sqlite;
            }

            throw new ArgumentException("未知的数据库类型");
        }

        /// <summary>
        /// 根据连接字符串获取数据库类型
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <returns></returns>
        public static DbType GetDatabaseType(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException(nameof(connectionString), "数据库连接字符串不能为空");

            // SQL Server (MSSQL) 区分条件
            if (connectionString.Contains("Data Source", StringComparison.OrdinalIgnoreCase) &&
                connectionString.Contains("Initial Catalog", StringComparison.OrdinalIgnoreCase))
            {
                return DbType.SqlServer;
            }

            // MySQL 区分条件
            if (connectionString.Contains("Server", StringComparison.OrdinalIgnoreCase) &&
                connectionString.Contains("Database", StringComparison.OrdinalIgnoreCase))
            {
                if (connectionString.Contains("Uid", StringComparison.OrdinalIgnoreCase) ||
                    connectionString.Contains("User Id", StringComparison.OrdinalIgnoreCase))
                {
                    return DbType.MySql;
                }
            }

            // PostgreSQL
            if (connectionString.Contains("Host", StringComparison.OrdinalIgnoreCase) &&
                connectionString.Contains("Database", StringComparison.OrdinalIgnoreCase))
            {
                return DbType.PostgreSQL;
            }

            // Oracle
            if (connectionString.Contains("Data Source", StringComparison.OrdinalIgnoreCase) &&
                connectionString.Contains("User Id", StringComparison.OrdinalIgnoreCase))
            {
                return DbType.Oracle;
            }

            // SQLite
            if (connectionString.Contains("Data Source", StringComparison.OrdinalIgnoreCase) &&
                connectionString.Contains("Version", StringComparison.OrdinalIgnoreCase))
            {
                return DbType.Sqlite;
            }
            //mongodb
            if (connectionString.Contains("mongodb", StringComparison.OrdinalIgnoreCase))
            {
                return DbType.MongoDb;
            }
            throw new ArgumentException("未知的数据库类型");
        }

        /// <summary>
        /// 检测TrustServerCertificate
        /// </summary>
        /// <remarks>
        /// 如果连接字符串中已经包含 TrustServerCertificate则不进行修改。
        /// 如果连接字符串中不包含这两个参数，则在连接字符串末尾添加 TrustServerCertificate=true 参数。
        /// </remarks>
        /// <param name="connectionString"></param>
        /// <param name="dbType"></param>
        /// <returns></returns>
        public static string CheckTrustServerCertificate(this string connectionString, DbType dbType)
        {
            // 仅对 SqlServer 数据库进行检测
            if (dbType == DbType.SqlServer && !connectionString.Contains("TrustServerCertificate", StringComparison.OrdinalIgnoreCase))
            {
                // 根据连接字符串是否以分号结尾决定拼接方式
                connectionString += connectionString.EndsWith(";")
                    ? "TrustServerCertificate=true;"
                    : ";TrustServerCertificate=true;";
            }
            return connectionString;
        }

        /// <summary>
        /// 检测Encrypt
        /// </summary>
        /// <remarks>
        /// 如果连接字符串中已经包含 Encrypt=true 或 Encrypt=False，则不进行修改。
        /// 如果连接字符串中不包含这两个参数，则在连接字符串末尾添加 Encrypt=true 参数。
        /// <param name="connectionString"></param>
        /// <param name="dbType"></param>
        /// <returns></returns>
        public static string CheckEncrypt(this string connectionString, DbType dbType)
        {
            // 仅对 SqlServer 数据库进行检测
            if (dbType == DbType.SqlServer && !connectionString.Contains("Encrypt", StringComparison.OrdinalIgnoreCase))
            {
                // 检查是否已包含 Encrypt 参数
                // 根据连接字符串是否以分号结尾决定拼接方式
                connectionString += connectionString.EndsWith(";")
                    ? "Encrypt=true;"
                    : ";Encrypt=true;";
            }
            return connectionString;
        }
    }
}