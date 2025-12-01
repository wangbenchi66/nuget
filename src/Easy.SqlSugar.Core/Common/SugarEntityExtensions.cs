using System.Reflection;
using System.Text.RegularExpressions;
using SqlSugar;

namespace Easy.SqlSugar.Core.Common
{
    public static class SugarEntityExtensions
    {
        /// <summary>
        /// 转换为 SqlSugar 可用的字典参数(内部过滤掉导航属性和忽略的属性)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static Dictionary<string, object>? ToSqlSugarDictionary<T>(T obj)
        {
            if (obj == null) return null;

            var type = obj.GetType();

            // 必须有 SugarTable
            var tableAttr = type.GetCustomAttribute<SugarTable>();
            if (tableAttr == null)
            {
                throw new Exception($"{type.Name} 必须标记 [SugarTable] 才能转换为字典。");
            }

            return type
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p =>
                {
                    if (!p.CanRead) return false;

                    //过滤导航属性
                    if (p.GetCustomAttribute<Navigate>() != null) return false;
                    var colAttr = p.GetCustomAttribute<SugarColumn>(false);

                    //IsIgnore=true属性
                    if (colAttr?.IsIgnore == true) return false;

                    //仅忽略更新/插入的字段也不作为参数
                    //if (colAttr?.IsOnlyIgnoreUpdate == true) return false;
                    //if (colAttr?.IsOnlyIgnoreInsert == true) return false;

                    //if (colAttr?.IsPrimaryKey == true && colAttr?.IsIdentity == true)
                    //{
                    //}

                    //过滤掉复杂类型只允许基础类型(为了避免导航属性相关
                    if (!IsSimpleType(p.PropertyType)) return false;

                    return true;
                })
                .ToDictionary(
                    p => p.Name,
                    p => p.GetValue(obj) ?? DBNull.Value
                );
        }
        private static bool IsSimpleType(Type type)
        {
            return type.IsPrimitive ||
                   type.IsEnum ||
                   type == typeof(string) ||
                   type == typeof(DateTime) ||
                   type == typeof(Guid) ||
                   type == typeof(decimal) ||
                   type == typeof(long) ||
                   type == typeof(int) ||
                   type == typeof(short) ||
                   type == typeof(float) ||
                   type == typeof(double) ||
                   type == typeof(bool) ||
                   type == typeof(byte) ||
                   type == typeof(byte[]);
        }

        /// <summary>
        /// 解析sql然后转换为 SqlSugar 可用的字典参数(内部过滤掉导航属性和忽略的属性)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exception"></exception>
        public static Dictionary<string, object> ToSqlSugarDictionary<T>(T obj, string sql)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (string.IsNullOrEmpty(sql)) throw new ArgumentNullException(nameof(sql));
            var type = obj.GetType();
            //提取 sql参数@开头
            var paramMatches = Regex.Matches(sql, @"@(\w+)");
            var paramNames = paramMatches.Select(m => m.Groups[1].Value).Distinct().ToHashSet(StringComparer.OrdinalIgnoreCase);

            //遍历实体属性
            var dict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!prop.CanRead) continue;
                if (paramNames.Contains(prop.Name))
                {
                    dict[prop.Name] = prop.GetValue(obj) ?? DBNull.Value;
                }
            }
            return dict;
        }

        /// <summary>
        /// 实体转换为SugarParameter
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static SugarParameter[] ToSugarParameters<T>(this T obj)
        {
            //必须是实体对象,不能是集合
            if (obj == null) return null;
            var dict = ToSqlSugarDictionary(obj);
            if (dict == null || dict.Count == 0)
            {
                return Array.Empty<SugarParameter>();
            }
            return dict.Select(kv => new SugarParameter(kv.Key, kv.Value)).ToArray();
        }

        /// <summary>
        /// 检查并修正 SQL 语句
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static string CheckSql(string sql)
        {
            sql = InSqlReplace(sql);
            return sql;
        }

        /// <summary>
        /// 自动为 SQL 中的 IN @param 添加括号，变成 IN (@param)
        /// 示例: "WHERE id IN @ids" → "WHERE id IN (@ids)"
        /// </summary>
        public static string InSqlReplace(string sql)
        {
            if (string.IsNullOrWhiteSpace(sql))
                return sql;
            //先检查一下是否包含 IN 关键字，避免不必要的正则处理
            if (!sql.Contains(" IN ", StringComparison.OrdinalIgnoreCase))
                return sql;

            // 匹配模式：IN 后跟空白，然后是参数占位符（@xxx, :xxx, ? 等）
            // 注意：不匹配 'IN (' 已有括号的情况
            var pattern = @"\bIN\s+(?!\s*\()(@[:\w]+|:\w+|\?\d*)\b";

            // 替换为 IN ($1)
            var fixedSql = Regex.Replace(sql, pattern, "IN ($1)", RegexOptions.IgnoreCase);
            return fixedSql;
        }
    }
}