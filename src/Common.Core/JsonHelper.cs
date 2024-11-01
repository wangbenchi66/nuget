using Masuit.Tools;
using Newtonsoft.Json;

namespace Common.Core
{
    public static class JsonHelper
    {
        /// <summary>
        /// json序列化
        /// </summary>
        /// <param name="obj">要序列化的对象</param>
        /// <returns>序列化后的json字符串</returns>
        public static string ToJson(this object obj)
        {
            if (obj.IsNullOrEmpty())
                return string.Empty;
            return JsonConvert.SerializeObject(obj);
        }

        /// <summary>
        /// json反序列化
        /// </summary>
        /// <typeparam name="T">目标对象类型</typeparam>
        /// <param name="json">要反序列化的json字符串</param>
        /// <returns>反序列化后的目标对象</returns>
        public static T ToObject<T>(this string json)
        {
            if (json.IsNullOrEmpty())
                return default(T);
            return JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// json序列化(自定义序列化格式)
        /// </summary>
        /// <param name="obj">要序列化的对象</param>
        /// <param name="settings">JsonSerializerSettings对象，用于自定义序列化格式</param>
        /// <returns>序列化后的json字符串</returns>
        public static string ToJson(this object obj, JsonSerializerSettings settings = null)
        {
            if (settings == null)
            {
                settings = GetJsonSerializerSettings();
            }
            return JsonConvert.SerializeObject(obj, settings);
        }

        /// <summary>
        /// json反序列化(自定义序列化格式)
        /// </summary>
        /// <param name="obj">要反序列化的对象</param>
        /// <param name="settings">JsonSerializerSettings对象，用于自定义反序列化格式</param>
        /// <returns>反序列化后的json字符串</returns>
        public static string ToObject(this object obj, JsonSerializerSettings settings = null)
        {
            if (settings == null)
            {
                settings = GetJsonSerializerSettings();
            }
            return JsonConvert.SerializeObject(obj, settings);
        }

        /// <summary>
        /// 获取JsonSerializerSettings对象
        /// </summary>
        /// <returns>JsonSerializerSettings对象</returns>
        private static JsonSerializerSettings GetJsonSerializerSettings()
        {
            return new JsonSerializerSettings()
            {
                //忽略循环引用
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                //日期格式化
                DateFormatString = "yyyy-MM-dd HH:mm:ss",
                /*//空值处理
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(),
                //空值处理
                NullValueHandling = NullValueHandling.Ignore*/
            };
        }
    }
}