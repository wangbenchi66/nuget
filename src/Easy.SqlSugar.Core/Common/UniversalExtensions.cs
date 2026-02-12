using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using Easy.Common.Core;
using SqlSugar;

namespace Easy.SqlSugar.Core;
/// <summary>
/// sqlsugar通用扩展方法
/// </summary>
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



    #region aop数据据库操作事件处理
    private static readonly HashSet<string> _createTimeFieldNames = new(StringComparer.OrdinalIgnoreCase) { "createtime", "createdtime", "addtime", "create_time", "created_at" };
    private static readonly HashSet<string> _updateTimeFieldNames = new(StringComparer.OrdinalIgnoreCase) { "updatetime", "updatedtime", "lasttime", "modifydate", "update_time", "updated_at" };

    /// <summary>
    /// 只读集合，外部只能通过AddCreateTimeField和AddUpdateTimeField方法添加字段名，确保字段名的有效性和唯一性
    /// </summary>
    public static IReadOnlyCollection<string> CreateTimeFieldNames => _createTimeFieldNames;
    /// <summary>
    /// 只读集合，外部只能通过AddCreateTimeField和AddUpdateTimeField方法添加字段名，确保字段名的有效性和唯一性
    /// </summary>
    public static IReadOnlyCollection<string> UpdateTimeFieldNames => _updateTimeFieldNames;

    /// <summary>
    /// 添加创建时间字段名，添加后会在aop数据操作事件中自动赋值当前时间
    /// </summary>
    /// <remarks>
    /// (系统默认识别了一些字段，可以使用UniversalExtensions.CreateTimeFieldNames查看默认识别的字段名,避免冗余代码)
    /// 注意：如果数据库中已经存在创建时间字段但不在默认识别的字段中，则需要调用此方法添加该字段名，否则aop事件将无法自动赋值创建时间
    /// 如果需要添加多个字段名，可以使用逗号分隔的字符串或字符串数组传入，例如：
    /// AddCreateTimeField("createtime,create_time,createdtime");
    /// 已进行过输入校验，确保添加的字段名不包含特殊字符且不重复，避免无效字段名导致的性能问题
    /// </remarks>
    /// <param name="fieldNames"></param>
    public static void AddCreateTimeField(params string[] fieldNames) => AddTimeField(_createTimeFieldNames, fieldNames);

    /// <summary>
    /// 添加更新时间字段名，添加后会在aop数据操作事件中自动赋值当前时间
    /// </summary>
    /// <remarks>
    /// (系统默认识别了一些字段，可以使用UniversalExtensions.UpdateTimeFieldNames查看默认识别的字段名,避免冗余代码)
    /// 注意：如果数据库中已经存在更新时间字段但不在默认识别的字段中，则需要调用此方法添加该字段名，否则aop事件将无法自动赋值更新时间
    /// 如果需要添加多个字段名，可以使用逗号分隔的字符串或字符串数组传入，例如：
    /// AddUpdateTimeField("updatetime,update_time,updatedtime");
    /// 已进行过输入校验，确保添加的字段名不包含特殊字符且不重复，避免无效字段名导致的性能问题
    /// </remarks>
    /// <param name="fieldNames"></param>
    public static void AddUpdateTimeField(params string[] fieldNames) => AddTimeField(_updateTimeFieldNames, fieldNames);

    /// <summary>
    /// 私有方法，添加时间字段名到指定集合，确保唯一性和有效性
    /// </summary>
    /// <param name="fieldNames"></param>
    /// <param name="newFieldNames"></param>
    private static void AddTimeField(HashSet<string> fieldNames, params string[] newFieldNames)
    {
        //TODO:要不要加限制后边看实际使用情况  限定添加的字段名称或数量，例如限制只能添加以time结尾的字段名，或限制总字段名数量不超过20个等，以避免无效字段名导致的性能问题
        if (newFieldNames.IsNull()) return;
        foreach (var fieldName in newFieldNames.Where(name => !string.IsNullOrWhiteSpace(name) && !System.Text.RegularExpressions.Regex.IsMatch(name.Trim(), @"^[A-Za-z0-9_]+$")).Select(name => name.Trim().ToLower()).ToArray())
            fieldNames.Add(fieldName.Trim());
    }

    /// <summary>
    /// 时间默认赋值处理方法
    /// </summary>
    /// <remarks>
    /// 在aop数据操作事件中调用此方法，根据字段名自动识别并赋值创建时间和更新时间，避免重复代码和遗漏赋值的情况
    /// 注意：此方法需要配合AddCreateTimeField和AddUpdateTimeField方法使用，确保正确识别时间字段名，否则将无法自动赋值时间
    /// </remarks>
    /// <param name="oldValue"></param>
    /// <param name="entityInfo"></param>
    public static void HandleTimeField(object oldValue, DataFilterModel entityInfo)
    {
        var normalized = entityInfo.PropertyName.ToLower();
        bool isCreateTime = CreateTimeFieldNames.Contains(normalized);
        bool isUpdateTime = UpdateTimeFieldNames.Contains(normalized);
        if (!isCreateTime && !isUpdateTime)
            return;
        var now = DateTime.Now;
        switch (entityInfo.OperationType)
        {
            case DataFilterType.InsertByObject:
                entityInfo.SetValue(now);
                break;

            case DataFilterType.UpdateByObject:
                if (!isUpdateTime) break;
                entityInfo.SetValue(now);
                break;
        }
    }

    #endregion aop数据据库操作事件处理

    #region sqlsugar扩展函数
    /// <summary>
    /// sqlsugar扩展函数配置，添加自定义的sql函数，IsNull和IsNotNull
    /// </summary>
    /// <returns></returns>
    public static List<SqlFuncExternal> GetSqlFuncExternals()
    {
        return new List<SqlFuncExternal>()
        {
            new SqlFuncExternal()
            {
                UniqueMethodName = "IsNotNull",
                MethodValue = (expInfo, dbType, expContext) =>
                {
                    return string.Format("({0} IS NOT NULL)", expInfo.Args[0].MemberName);
                }
            },
            new SqlFuncExternal()
            {
                UniqueMethodName = "IsNull",
                MethodValue = (expInfo, dbType, expContext) =>
                {
                    return string.Format("({0} IS NULL)", expInfo.Args[0].MemberName);
                }
            }
        };
    }
    #endregion
}