using System.Reflection;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Easy.Common.Core
{
    /// <summary>
    /// 通用扩展方法类，用于数据类型转换
    /// </summary>
    public static class DataConversionExtensions
    {
        #region 对象类型转换为基本数据类型

        /// <summary>
        /// 将对象转换为整数类型（默认返回0）
        /// </summary>
        /// <param name="thisValue">要转换的对象</param>
        /// <returns>转换后的整数值</returns>
        public static int ToInt(this object thisValue, int defaultValue = 0)
        {
            if (thisValue == null || thisValue == DBNull.Value)
                return defaultValue;
            if (thisValue is Enum)
                return Convert.ToInt32(thisValue);

            return int.TryParse(thisValue.ToString(), out var result) ? result : defaultValue;
        }

        /// <summary>
        /// 将对象转换为长整型（默认返回0）
        /// </summary>
        /// <param name="thisValue">要转换的对象</param>
        /// <returns>转换后的长整型值</returns>
        public static long ToLong(this object thisValue, long defaultValue = 0)
        {
            if (thisValue == null || thisValue == DBNull.Value)
                return defaultValue;
            if (thisValue is Enum)
                return Convert.ToInt64(thisValue);

            return long.TryParse(thisValue.ToString(), out var result) ? result : defaultValue;
        }

        /// <summary>
        /// 将对象转换为浮动类型（默认返回0.0）
        /// </summary>
        /// <param name="thisValue">要转换的对象</param>
        /// <returns>转换后的浮动类型值</returns>
        public static double ToDouble(this object thisValue, double defaultValue = 0.0)
        {
            if (thisValue == null || thisValue == DBNull.Value)
                return defaultValue;

            return double.TryParse(thisValue.ToString(), out var result) ? result : defaultValue;
        }

        /// <summary>
        /// 将对象转换为字符串类型（默认返回空字符串）
        /// </summary>
        /// <param name="thisValue">要转换的对象</param>
        /// <returns>转换后的字符串</returns>
        public static string ToStringValue(this object thisValue, string defaultValue = "")
        {
            return thisValue?.ToString().Trim() ?? defaultValue;
        }

        /// <summary>
        /// 将对象转换为Decimal类型（默认返回0）
        /// </summary>
        /// <param name="thisValue">要转换的对象</param>
        /// <returns>转换后的Decimal值</returns>
        public static decimal ToDecimal(this object thisValue, decimal defaultValue = 0)
        {
            if (thisValue == null || thisValue == DBNull.Value)
                return defaultValue;

            return decimal.TryParse(thisValue.ToString(), out var result) ? result : defaultValue;
        }

        /// <summary>
        /// 将对象转换为日期类型（默认返回DateTime.MinValue）
        /// </summary>
        /// <param name="thisValue">要转换的对象</param>
        /// <returns>转换后的日期值</returns>
        public static DateTime ToDate(this object thisValue, DateTime defaultValue = default)
        {
            if (thisValue == null || thisValue == DBNull.Value)
                return defaultValue;

            return DateTime.TryParse(thisValue.ToString(), out var result) ? result : defaultValue;
        }

        /// <summary>
        /// 将对象转换为布尔类型（默认返回false）
        /// </summary>
        /// <param name="thisValue">要转换的对象</param>
        /// <returns>转换后的布尔值</returns>
        public static bool ToBool(this object thisValue, bool defaultValue = false)
        {
            if (thisValue == null || thisValue == DBNull.Value)
                return defaultValue;

            return bool.TryParse(thisValue.ToString(), out var result) ? result : defaultValue;
        }

        #endregion 对象类型转换为基本数据类型

        #region 字典与实体对象转换

        /// <summary>
        /// 将字典转换为指定类型的实体对象
        /// </summary>
        /// <typeparam name="T">目标实体类型</typeparam>
        /// <param name="dic">包含数据的字典</param>
        /// <returns>转换后的实体对象</returns>
        public static T ToEntity<T>(this Dictionary<string, object> dic)
        {
            return new List<Dictionary<string, object>>() { dic }.ToList<T>().First();
        }

        /// <summary>
        /// 将字典列表转换为指定类型的实体对象列表
        /// </summary>
        /// <typeparam name="T">目标实体类型</typeparam>
        /// <param name="dicList">包含数据的字典列表</param>
        /// <returns>转换后的实体对象列表</returns>
        public static List<T> ToList<T>(this List<Dictionary<string, object>> dicList)
        {
            return dicList.ToIEnumerable<T>().ToList();
        }

        /// <summary>
        /// 将字典列表转换为指定类型的实体对象枚举
        /// </summary>
        /// <typeparam name="T">目标实体类型</typeparam>
        /// <param name="dicList">包含数据的字典列表</param>
        /// <returns>转换后的实体对象枚举</returns>
        public static IEnumerable<T> ToIEnumerable<T>(this List<Dictionary<string, object>> dicList)
        {
            foreach (var dic in dicList)
            {
                T model = Activator.CreateInstance<T>();
                foreach (var property in model.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (dic.TryGetValue(property.Name, out var value))
                    {
                        property.SetValue(model, value?.ToString().ToType(property.PropertyType));
                    }
                }
                yield return model;
            }
        }

        #endregion 字典与实体对象转换

        #region 类型转换

        /// <summary>
        /// 将字符串转换为指定类型
        /// </summary>
        /// <param name="value">要转换的字符串</param>
        /// <param name="conversionType">目标类型</param>
        /// <returns>转换后的对象</returns>
        public static object ToType(this string value, Type conversionType)
        {
            if (conversionType.IsGenericType && conversionType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (value.IsNull())
                    return null;
                conversionType = Nullable.GetUnderlyingType(conversionType);
            }
            return Convert.ChangeType(value, conversionType);
        }

        #endregion 类型转换

        #region 字符串转换为基本数据类型

        /// <summary>
        /// 将字符串转换为指定的数值类型（如：SByte, Byte, Int16, Int32等）
        /// </summary>
        /// <param name="value">字符串</param>
        /// <returns>转换后的数值</returns>
        public static T? ToValue<T>(this string value) where T : struct
        {
            if (value.IsNull()) return null;
            var type = typeof(T);

            if (type == typeof(sbyte))
                return (T?)(object)SByte.Parse(value);
            if (type == typeof(byte))
                return (T?)(object)Byte.Parse(value);
            if (type == typeof(short))
                return (T?)(object)short.Parse(value);
            if (type == typeof(int))
                return (T?)(object)int.Parse(value);
            if (type == typeof(long))
                return (T?)(object)long.Parse(value);
            if (type == typeof(float))
                return (T?)(object)float.Parse(value);
            if (type == typeof(double))
                return (T?)(object)double.Parse(value);
            if (type == typeof(decimal))
                return (T?)(object)decimal.Parse(value);
            if (type == typeof(bool))
                return (T?)(object)bool.Parse(value);
            if (type == typeof(Guid))
                return (T?)(object)Guid.Parse(value);
            if (type == typeof(DateTime))
                return (T?)(object)DateTime.Parse(value);

            return null;
        }

        #endregion 字符串转换为基本数据类型
    }

    /// <summary>
    /// json时间格式化转换器
    /// </summary>
    /// <remarks>
    /// 控制器中使用 例如
    /// AddJsonOptions(options =>{
    /// options.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
    /// });
    /// </remarks>
    public class DateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => DateTime.Parse(reader.GetString()!);

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
            => writer.WriteStringValue(value.ToString("yyyy-MM-dd HH:mm:ss"));
    }

}