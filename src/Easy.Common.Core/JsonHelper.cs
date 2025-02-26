using System.Text.Json;

namespace Easy.Common.Core
{
    /// <summary>
    /// json序列化、反序列化帮助类
    /// </summary>
    public static class JsonHelper
    {
        /// <summary>
        /// json序列化
        /// </summary>
        /// <param name="obj">要序列化的对象</param>
        /// <returns>序列化后的json字符串</returns>
        public static string ToJson(this object obj)
        {
            if (obj == null)
                return string.Empty;
            return JsonSerializer.Serialize(obj);
        }

        /// <summary>
        /// json反序列化
        /// </summary>
        /// <typeparam name="T">目标对象类型</typeparam>
        /// <param name="json">要反序列化的json字符串</param>
        /// <returns>反序列化后的目标对象</returns>
        public static T ToObject<T>(this string json)
        {
            if (json.IsNull())
                return default(T);
            return JsonSerializer.Deserialize<T>(json);
        }

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
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles,
                //日期格式化
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            };
        }
    }
}