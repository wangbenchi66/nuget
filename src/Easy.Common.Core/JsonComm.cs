using System.Text.Json;
using System.Text.Json.Serialization;

namespace Easy.Common.Core
{
    /// <summary>
    /// json序列化、反序列化帮助类
    /// </summary>
    public static class JsonComm
    {

        /// <summary>
        /// json序列化(自定义序列化格式)
        /// </summary>
        /// <param name="obj">要序列化的对象</param>
        /// <param name="options">JsonSerializerOptions对象，用于自定义序列化格式</param>
        /// <returns>序列化后的json字符串</returns>
        public static string ToJson(this object obj, JsonSerializerOptions options = null)
        {
            if (obj == null)
                return string.Empty;
            return JsonSerializer.Serialize(obj, options ?? GetJsonSerializerOptions());
        }

        /// <summary>
        /// json反序列化(自定义序列化格式)
        /// </summary>
        /// <typeparam name="T">目标对象类型</typeparam>
        /// <param name="json">要反序列化的json字符串</param>
        /// <param name="options">JsonSerializerOptions对象，用于自定义反序列化格式</param>
        /// <returns>反序列化后的目标对象</returns>
        public static T ToObject<T>(this string json, JsonSerializerOptions options = null)
        {
            if (json.IsNull())
                return default(T);
            return JsonSerializer.Deserialize<T>(json, options ?? GetJsonSerializerOptions());
        }

        /// <summary>
        /// 获取JsonSerializerOptions对象
        /// </summary>
        /// <returns>JsonSerializerOptions对象</returns>
        private static JsonSerializerOptions GetJsonSerializerOptions()
        {
            return new JsonSerializerOptions
            {
                //忽略循环引用
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                //日期格式化
                WriteIndented = true,
                //设置属性命名策略
                PropertyNamingPolicy = null,
                // 忽略大小写
                PropertyNameCaseInsensitive = true,
                //总是包含null
                DefaultIgnoreCondition = JsonIgnoreCondition.Never,
                //设置编码器
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                //保持原格式
                AllowTrailingCommas = true,
                //时间格式化
                Converters = { new DateTimeConverter("yyyy-MM-dd HH:mm:ss") },
                // 允许从字符串读取数字
                NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString,
            };
        }

        /// <summary>
        /// 时间格式化转换器
        /// </summary>
        private class DateTimeConverter : JsonConverter<DateTime>
        {
            private readonly string _format;

            public DateTimeConverter(string format = "yyyy-MM-dd HH:mm:ss")
            {
                _format = format;
            }

            public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return DateTime.Parse(reader.GetString()!);
            }

            public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(value.ToString(_format));
            }
        }

    }
}