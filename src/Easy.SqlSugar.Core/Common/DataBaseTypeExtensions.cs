using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

            throw new ArgumentException("未知的数据库类型");
        }
    }
}