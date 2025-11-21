using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
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

        /// <summary>
        /// 将官方特性转换为sqlsugar特性
        /// </summary>
        /// <returns></returns>
        public static ConfigureExternalServices GetInitConfigureExternalServices()
        {
            return new ConfigureExternalServices()
            {
                EntityService = (property, column) =>
                {
                    var attributes = property.GetCustomAttributes(true);
                    if (attributes == null || attributes.Length == 0)
                        return;
                    //主键
                    if (attributes.Any(it => it is KeyAttribute))
                    {
                        column.IsPrimarykey = true;
                    }
                    //忽略
                    if (attributes.Any(it => it is NotMappedAttribute))
                    {
                        column.IsIgnore = true;
                    }
                    //自增
                    if (attributes.Any(it => it is DatabaseGeneratedAttribute))
                    {
                        var attr = (DatabaseGeneratedAttribute)attributes.First(it => it is DatabaseGeneratedAttribute);
                        if (attr.DatabaseGeneratedOption == DatabaseGeneratedOption.Identity)
                        {
                            column.IsIdentity = true;
                        }
                    }
                    // 长度限制
                    if (attributes.Any(it => it is MaxLengthAttribute))
                    {
                        var attr = (MaxLengthAttribute)attributes.First(it => it is MaxLengthAttribute);
                        column.Length = attr.Length;
                    }
                    // 非空约束
                    if (attributes.Any(it => it is RequiredAttribute))
                    {
                        column.IsNullable = false;
                    }
                },
                EntityNameService = (type, entity) =>
                {
                    var attributes = type.GetCustomAttributes(true);
                    if (attributes == null || attributes.Length == 0)
                        return;
                    if (attributes.Any(it => it is TableAttribute))
                    {
                        var attr = (attributes.First(it => it is TableAttribute) as TableAttribute);
                        entity.DbTableName = attr.Name;
                    }
                }
            };
        }

        /// <summary>
        /// 随机获取雪花id的工作单元(根据localIP+mac地址+机器名生成0-31的workId)
        /// </summary>
        /// <returns></returns>
        public static int GetRandomWorkId()
        {
            var localIp = Dns.GetHostAddresses(Dns.GetHostName())
                      .FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork)?.ToString() ?? "";
            var mac = NetworkInterface.GetAllNetworkInterfaces()
                    .FirstOrDefault(x => x.NetworkInterfaceType != NetworkInterfaceType.Loopback && x.OperationalStatus == OperationalStatus.Up)?
                    .GetPhysicalAddress()
                    .ToString() ?? "";
            string machineTag = Environment.MachineName + "_" + localIp + "_" + mac;
            int workId = Math.Abs(machineTag.GetHashCode()) % 32;
            return workId;
        }
        /// <summary>
        /// 获取Yitter.IdGenerator雪花id配置
        /// </summary>
        /// <returns></returns>
        private static Yitter.IdGenerator.IdGeneratorOptions GetYitSnowflakeOptions()
        {
            //定义推特时间戳(2010-11-04 01:42:54.657 UTC)
            DateTime twepochUtc = new DateTime(2010, 11, 4, 1, 42, 54, 657, DateTimeKind.Utc);
            var options = new Yitter.IdGenerator.IdGeneratorOptions((ushort)GetRandomWorkId())
            {
                BaseTime = twepochUtc,
                WorkerIdBitLength = 6,
                SeqBitLength = 6
            };
            return options;
        }
        /// <summary>
        /// 存储生成的Yitter.IdGenerator雪花id配置 一旦生成不能修改
        /// </summary>
        public static readonly Yitter.IdGenerator.IdGeneratorOptions YitSnowflakeOptions = GetYitSnowflakeOptions();
        /// <summary>
        /// 解析sugar雪花id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static SnowflakeIdParts ParseSugarSnowflakeId(long id)
        {
            return SnowflakeIdParser.ParseSqlSugarSnowflakeId(id);
        }
        /// <summary>
        /// 解析Yitter.IdGenerator雪花id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static SnowflakeIdParts ParseYitSnowflakeId(long id)
        {
            var options = YitSnowflakeOptions;
            var res = SnowflakeIdParser.ParseYitBySnowFlakeId(id, options.BaseTime, options.WorkerIdBitLength, options.SeqBitLength);
            return res;
        }

    }
}