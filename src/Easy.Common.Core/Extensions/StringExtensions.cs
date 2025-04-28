using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Easy.Common.Core.Extensions
{
    public static class StringExtensions
    {
        public static bool _windows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        /// <summary>
        /// 替换路径中的分隔符
        /// </summary>
        /// <param name="path">路径字符串</param>
        /// <returns>替换后的路径字符串</returns>
        public static string ReplacePath(this string path)
        {
            if (string.IsNullOrEmpty(path))
                return "";
            if (_windows)
                return path.Replace("/", "\\");
            return path.Replace("\\", "/");

        }

        private static DateTime dateStart = new DateTime(1970, 1, 1, 8, 0, 0);

        private static long longTime = 621355968000000000;

        private static int samllTime = 10000000;

        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <param name="dateTime">日期时间</param>
        /// <returns>时间戳</returns>
        public static long GetTimeStamp(this DateTime dateTime)
        {
            return (dateTime.ToUniversalTime().Ticks - longTime) / samllTime;
        }

        /// <summary>
        /// 时间戳转换成日期
        /// </summary>
        /// <param name="timeStamp">时间戳</param>
        /// <returns>日期时间</returns>
        public static DateTime GetTimeSpmpToDate(this object timeStamp)
        {
            if (timeStamp == null) return dateStart;
            DateTime dateTime = new DateTime(longTime + Convert.ToInt64(timeStamp) * samllTime, DateTimeKind.Utc).ToLocalTime();
            return dateTime;
        }

        /*   public static string CreateHtmlParas(this string urlPath, int? userId = null)
           {
               if (string.IsNullOrEmpty(urlPath))
                   return null;
               userId = userId ?? UserContext.Current.UserInfo.User_Id;
               return $"{urlPath}{(urlPath.IndexOf("?token") > 0 ? "&" : "?")}uid={userId}&rt_v={DateTime.Now.ToString("HHmmss")}";
               // return urlPath + ((urlPath.IndexOf("?token") > 0 ? "&" : "?") + "uid=" + userId);
           }*/

        /// <summary>
        /// 判断字符串是否为URL
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>是否为URL</returns>
        public static bool IsUrl(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return false;
            string Url = @"(http://)?([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?";
            return Regex.IsMatch(str, Url);

        }

        /// <summary>
        /// 判断是不是正确的手机号码
        /// </summary>
        /// <param name="input">输入字符串</param>
        /// <returns>是否为正确的手机号码</returns>
        public static bool IsPhoneNo(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return false;
            if (input.Length != 11)
                return false;

            if (new Regex(@"^1[3578][01379]\d{8}$").IsMatch(input)
                || new Regex(@"^1[34578][01256]\d{8}").IsMatch(input)
                || new Regex(@"^(1[012345678]\d{8}|1[345678][0123456789]\d{8})$").IsMatch(input)
                )
                return true;
            return false;
        }

        /// <summary>
        /// 尝试将字符串转换为Guid
        /// </summary>
        /// <param name="guid">字符串</param>
        /// <param name="outId">输出的Guid</param>
        /// <returns>是否转换成功</returns>
        public static bool GetGuid(this string guid, out Guid outId)
        {
            Guid emptyId = Guid.Empty;
            return Guid.TryParse(guid, out outId);
        }

        /// <summary>
        /// 判断字符串是否为Guid
        /// </summary>
        /// <param name="guid">字符串</param>
        /// <returns>是否为Guid</returns>
        public static bool IsGuid(this string guid)
        {
            Guid newId;
            return guid.GetGuid(out newId);
        }

        /// <summary>
        /// 判断对象是否为整数
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns>是否为整数</returns>
        public static bool IsInt(this object obj)
        {
            if (obj == null)
                return false;
            bool reslut = Int32.TryParse(obj.ToString(), out int _number);
            return reslut;

        }

        /// <summary>
        /// 判断对象是否为日期
        /// </summary>
        /// <param name="str">对象</param>
        /// <returns>是否为日期</returns>
        public static bool IsDate(this object str)
        {
            return str.IsDate(out _);
        }

        /// <summary>
        /// 判断对象是否为日期，并输出日期
        /// </summary>
        /// <param name="str">对象</param>
        /// <param name="dateTime">输出的日期</param>
        /// <returns>是否为日期</returns>
        public static bool IsDate(this object str, out DateTime dateTime)
        {
            dateTime = DateTime.Now;
            if (str == null || str.ToString() == "")
            {
                return false;
            }
            return DateTime.TryParse(str.ToString(), out dateTime);
        }

        /// <summary>
        /// 判断字符串是否为合法数字
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="formatString">格式字符串</param>
        /// <returns>是否为合法数字</returns>
        public static bool IsNumber(this string str, string formatString)
        {
            if (string.IsNullOrEmpty(str)) return false;

            return Regex.IsMatch(str, @"^[+-]?\d*[.]?\d*$");
            //int precision = 32;
            //int scale = 5;
            //try
            //{
            //    if (string.IsNullOrEmpty(formatString))
            //    {
            //        precision = 10;
            //        scale = 2;
            //    }
            //    else
            //    {
            //        string[] numbers = formatString.Split(',');
            //        precision = Convert.ToInt32(numbers[0]);
            //        scale = numbers.Length == 0 ? 2 : Convert.ToInt32(numbers[1]);
            //    }
            //}
            //catch { };
            //return IsNumber(str, precision, scale);
        }

        /**/

        /// <summary>
        /// 判断字符串是否为合法数字（指定整数位数和小数位数）
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="precision">整数位数</param>
        /// <param name="scale">小数位数</param>
        /// <returns>是否为合法数字</returns>
        public static bool IsNumber(this string str, int precision, int scale)
        {
            if ((precision == 0) && (scale == 0))
            {
                return false;
            }
            string pattern = @"(^\d{1," + precision + "}";
            if (scale > 0)
            {
                pattern += @"\.\d{0," + scale + "}$)|" + pattern;
            }
            pattern += "$)";
            return Regex.IsMatch(str, pattern);
        }

        /// <summary>
        /// 判断对象是否为空或空字符串
        /// </summary>
        /// <param name="str">对象</param>
        /// <returns>是否为空或空字符串</returns>
        public static bool IsNull(this object str)
        {
            if (str == null)
                return true;
            return str.ToString() == "";
        }

        /// <summary>
        /// 判断泛型集合是否为null或空
        /// </summary>
        /// <typeparam name="T">集合元素类型</typeparam>
        /// <param name="source">泛型集合</param>
        /// <returns>是否为null或空</returns>
        public static bool IsNull<T>(this IEnumerable<T> source)
        {
            if (source == null)
                return true;
            using (var enumerator = source.GetEnumerator())
                return !enumerator.MoveNext();
        }

        /// <summary>
        /// 判断集合是否为null或空
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static bool IsNull<T>(this List<T> list)
        {
            if (list == null || list.Count == 0) return true;
            return false;
        }

        /// <summary>
        /// 获取对象的整数值
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns>整数值</returns>
        public static int GetInt(this object obj)
        {
            if (obj == null)
                return 0;
            int.TryParse(obj.ToString(), out int _number);
            return _number;

        }

        /// <summary>
        /// 获取对象的长整数值
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns>长整数值</returns>
        public static long GetLong(this object obj)
        {
            if (obj == null)
                return 0;

            try
            {
                return Convert.ToInt64(Convert.ToDouble(obj));
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// 获取对象的浮点数值
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns>浮点数值</returns>
        public static float GetFloat(this object obj)
        {
            if (System.DBNull.Value.Equals(obj) || null == obj)
                return 0;

            try
            {
                return float.Parse(obj.ToString());
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// 获取对象的双精度浮点数值
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns>双精度浮点数值</returns>
        public static double GetDouble(this object obj)
        {
            if (System.DBNull.Value.Equals(obj) || null == obj)
                return 0;

            try
            {
                return Convert.ToDouble(obj);
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// 获取对象的十进制数值
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns>十进制数值</returns>
        public static decimal GetDecimal(this object obj)
        {
            if (System.DBNull.Value.Equals(obj) || null == obj)
                return 0;

            try
            {
                return Convert.ToDecimal(obj);
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// 获取对象的动态值
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns>动态值</returns>
        public static dynamic GetDynamic(this object obj)
        {
            if (System.DBNull.Value.Equals(obj) || null == obj)
                return null;

            try
            {
                string str = obj.ToString();
                if (str.IsNumber(25, 15)) return Convert.ToDecimal(obj);
                else return str;
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 获取对象的日期时间值
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns>日期时间值</returns>
        public static DateTime? GetDateTime(this object obj)
        {
            if (System.DBNull.Value.Equals(obj) || null == obj)
                return null;
            bool result = DateTime.TryParse(obj.ToString(), out DateTime dateTime);
            if (!result)
                return null;
            return dateTime;
        }

        /// <summary>
        /// 替换字符串中的空格字符
        /// </summary>
        /// <param name="input">输入字符串</param>
        /// <param name="replacement">替换字符</param>
        /// <returns>替换后的字符串</returns>
        public static string ReplaceWhitespace(this string input, string replacement = "")
        {
            return string.IsNullOrEmpty(input) ? null : Regex.Replace(input, "\\s", replacement, RegexOptions.Compiled);
        }

        private static char[] randomConstant ={
        '0','1','2','3','4','5','6','7','8','9',
        'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z',
        'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z'
      };

        /// <summary>
        /// 生成指定长度的随机数
        /// </summary>
        /// <param name="length">长度</param>
        /// <returns>随机数</returns>
        public static string GenerateRandomNumber(this int length)
        {
            System.Text.StringBuilder newRandom = new System.Text.StringBuilder(62);
            Random rd = new Random();
            for (int i = 0; i < length; i++)
            {
                newRandom.Append(randomConstant[rd.Next(62)]);
            }
            return newRandom.ToString();
        }

    }
}