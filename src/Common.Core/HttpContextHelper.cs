using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Core
{
    /// <summary>
    /// 请求上下文帮助类
    /// </summary>
    public static class HttpContextHelper
    {
        /// <summary>
        /// 获取参数值的字符串表示
        /// </summary>
        /// <param name="value">参数值</param>
        /// <returns>字符串表示</returns>
        public static string GetParameterValue(object value)
        {
            if (value == null)
            {
                return "null";
            }

            // 如果是简单类型，直接返回字符串表示
            if (value.GetType().IsPrimitive || value is string)
            {
                return value.ToString();
            }

            // 如果是复杂类型，序列化为JSON字符串
            return System.Text.Json.JsonSerializer.Serialize(value);
        }

        /// <summary>
        /// 生成字符串的哈希值
        /// </summary>
        /// <param name="input">输入字符串</param>
        /// <returns>哈希值</returns>
        public static string GetHash(string input)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }

        /// <summary>
        /// 获取参数并加密
        /// </summary>
        /// <param name="parameters">参数</param>
        /// <returns>加密后的字符串</returns>
        public static string GetParametHash(params object[] parameters)
        {
            // 加密前
            var paramKey = string.Join("_", parameters.Select(GetParameterValue));
            // 加密后
            var res = GetHash(paramKey);
            return res;
        }
    }
}