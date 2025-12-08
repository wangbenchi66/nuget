using System.Collections;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Easy.Common.Core;

/// <summary>
/// 一些基础的扩展方法
/// </summary>
public static class FoundationExtensions
{
    #region 判断字符串是否为手机号码

    /// <summary>
    /// 判断字符串是否为手机号码
    /// </summary>
    /// <param name="mobilePhoneNumber"> </param>
    /// <returns> </returns>
    public static bool IsMobile(string mobilePhoneNumber)
    {
        if (mobilePhoneNumber.Length < 11)
        {
            return false;
        }

        //电信手机号码正则
        string dianxin = @"^1[345789][01379]\d{8}$";
        Regex regexDx = new Regex(dianxin);
        //联通手机号码正则
        string liantong = @"^1[345678][01256]\d{8}$";
        Regex regexLt = new Regex(liantong);
        //移动手机号码正则
        string yidong = @"^1[345789][0123456789]\d{8}$";
        Regex regexYd = new Regex(yidong);
        if (regexDx.IsMatch(mobilePhoneNumber) || regexLt.IsMatch(mobilePhoneNumber) || regexYd.IsMatch(mobilePhoneNumber))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    #endregion 判断字符串是否为手机号码

    #region 检测是否符合email格式

    /// <summary>
    /// 检测是否符合email格式
    /// </summary>
    /// <param name="strEmail"> 要判断的email字符串 </param>
    /// <returns> 判断结果 </returns>
    public static bool IsValidEmail(string strEmail)
    {
        return Regex.IsMatch(strEmail, @"^[\w\.]+([-]\w+)*@[A-Za-z0-9-_]+[\.][A-Za-z0-9-_]");
    }

    public static bool IsValidDoEmail(string strEmail)
    {
        return Regex.IsMatch(strEmail,
            @"^@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
    }

    #endregion 检测是否符合email格式

    #region 检测是否是正确的Url

    /// <summary>
    /// 检测是否是正确的Url
    /// </summary>
    /// <param name="strUrl"> 要验证的Url </param>
    /// <returns> 判断结果 </returns>
    public static bool IsUrl(string strUrl)
    {
        return Regex.IsMatch(strUrl,
            @"^(http|https)\://([a-zA-Z0-9\.\-]+(\:[a-zA-Z0-9\.&%\$\-]+)*@)*((25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])|localhost|([a-zA-Z0-9\-]+\.)*[a-zA-Z0-9\-]+\.(com|edu|gov|int|mil|net|org|biz|arpa|info|name|pro|aero|coop|museum|[a-zA-Z]{1,10}))(\:[0-9]+)*(/($|[a-zA-Z0-9\.\,\?\'\\\+&%\$#\=~_\-]+))*$");
    }

    #endregion 检测是否是正确的Url

    #region string 转int数组

    public static int[] StringToIntArray(string str)
    {
        try
        {
            if (string.IsNullOrEmpty(str)) return new int[0];
            if (str.EndsWith(","))
            {
                str = str.Remove(str.Length - 1, 1);
            }
            var idstrarr = str.Split(',');
            var idintarr = new int[idstrarr.Length];

            for (int i = 0; i < idstrarr.Length; i++)
            {
                idintarr[i] = Convert.ToInt32(idstrarr[i]);
            }
            return idintarr;
        }
        catch
        {
            return new int[0];
        }
    }

    #endregion string 转int数组

    #region String转数组

    public static string[] StringToStringArray(string str)
    {
        try
        {
            if (string.IsNullOrEmpty(str)) return new string[0];
            if (str.EndsWith(",")) str = str.Remove(str.Length - 1, 1);
            return str.Split(',');
        }
        catch
        {
            return new string[0];
        }
    }

    #endregion String转数组

    #region String数组转Int数组

    public static int[] StringArrAyToIntArray(string[] str)
    {
        try
        {
            int[] iNums = Array.ConvertAll<string, int>(str, s => int.Parse(s));
            return iNums;
        }
        catch
        {
            return new int[0];
        }
    }

    #endregion String数组转Int数组

    #region string转Guid数组

    public static System.Guid[] StringToGuidArray(string str)
    {
        try
        {
            if (string.IsNullOrEmpty(str)) return new System.Guid[0];
            if (str.EndsWith(",")) str = str.Remove(str.Length - 1, 1);
            var strarr = str.Split(',');
            System.Guid[] guids = new System.Guid[strarr.Length];
            for (int index = 0; index < strarr.Length; index++)
            {
                guids[index] = System.Guid.Parse(strarr[index]);
            }
            return guids;
        }
        catch
        {
            return new System.Guid[0];
        }
    }

    #endregion string转Guid数组

    #region 获取32位md5加密

    /// <summary>
    /// 通过创建哈希字符串适用于任何 MD5 哈希函数 （在任何平台） 上创建 32 个字符的十六进制格式哈希字符串
    /// </summary>
    /// <param name="source"> </param>
    /// <returns> 32位md5加密字符串 </returns>
    public static string Md5For32(string source)
    {
        using (MD5 md5Hash = MD5.Create())
        {
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(source));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            string hash = sBuilder.ToString();
            return hash.ToUpper();
        }
    }

    #endregion 获取32位md5加密

    #region 获取16位md5加密

    /// <summary>
    /// 获取16位md5加密
    /// </summary>
    /// <param name="source"> </param>
    /// <returns> 16位md5加密字符串 </returns>
    public static string Md5For16(string source)
    {
        using (MD5 md5Hash = MD5.Create())
        {
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(source));
            //转换成字符串，并取9到25位
            string sBuilder = BitConverter.ToString(data, 4, 8);
            //BitConverter转换出来的字符串会在每个字符中间产生一个分隔符，需要去除掉
            sBuilder = sBuilder.Replace("-", "");
            return sBuilder.ToUpper();
        }
    }

    #endregion 获取16位md5加密

    #region 清除HTML中指定样式

    /// <summary>
    /// 清除HTML中指定样式
    /// </summary>
    /// <param name="content"> </param>
    /// <param name="rule"> </param>
    /// <returns> </returns>
    public static string ClearHtml(string content, string[] rule)
    {
        if (!rule.Any())
        {
            return content;
        }

        foreach (var item in rule)
        {
            content = Regex.Replace(content, "/" + item + @"\s*=\s*\d+\s*/i", "");
            content = Regex.Replace(content, "/" + item + @"\s*=\s*.+?[""]/i", "");
            content = Regex.Replace(content, "/" + item + @"\s*:\s*\d+\s*px\s*;?/i", "");
        }
        return content;
    }

    #endregion 清除HTML中指定样式

    #region list随机排序方法

    /// <summary>
    /// list随机排序方法
    /// </summary>
    /// <typeparam name="T"> </typeparam>
    /// <param name="ListT"> </param>
    /// <returns> </returns>
    public static List<T> RandomSortList<T>(List<T> ListT)
    {
        Random random = new Random();
        List<T> newList = new List<T>();
        foreach (T item in ListT)
        {
            newList.Insert(random.Next(newList.Count + 1), item);
        }
        return newList;
    }

    #endregion list随机排序方法

    #region 截前后字符(串)

    ///<summary>
    /// 截前后字符(串)
    ///</summary>
    ///<param name="val">原字符串</param>
    ///<param name="str">要截掉的字符串</param>
    ///<param name="all">是否贪婪</param>
    ///<returns></returns>
    public static string GetCaptureInterceptedText(string val, string str, bool all = false)
    {
        return Regex.Replace(val, @"(^(" + str + ")" + (all ? "*" : "") + "|(" + str + ")" + (all ? "*" : "") + "$)", "");
    }

    #endregion 截前后字符(串)

    #region 密码加密方法

    /// <summary>
    /// 密码加密方法
    /// </summary>
    /// <param name="password"> 要加密的字符串 </param>
    /// <param name="createTime"> 时间组合 </param>
    /// <returns> </returns>
    public static string EnPassword(string password, DateTime createTime)
    {
        var dtStr = createTime.ToString("yyyyMMddHHmmss");
        var md5 = Md5For32(password);
        var enPwd = Md5For32(md5 + dtStr);
        return enPwd;
    }

    #endregion 密码加密方法

    #region UrlEncode (URL编码)

    /// <summary>
    /// UrlEncode (URL编码)
    /// </summary>
    /// <param name="str"> </param>
    /// <returns> </returns>
    public static string UrlEncode(string str)
    {
        StringBuilder sb = new StringBuilder();
        byte[] byStr = System.Text.Encoding.UTF8.GetBytes(str); //默认是System.Text.Encoding.Default.GetBytes(str)
        for (int i = 0; i < byStr.Length; i++)
        {
            sb.Append(@"%" + Convert.ToString(byStr[i], 16));
        }

        return (sb.ToString());
    }

    #endregion UrlEncode (URL编码)

    #region HmacSHA256加密

    /// <summary>
    /// 钉钉HmacSHA256加密
    /// </summary>
    /// <param name="timestamp"> </param>
    /// <param name="secret"> </param>
    /// <returns> </returns>
    public static string HmacSHA256(long timestamp, string secret)
    {
        string signString = $"{timestamp}\n{secret}";
        using (HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret)))
        {
            byte[] signData = hmac.ComputeHash(Encoding.UTF8.GetBytes(signString));
            string sign = HttpUtility.UrlEncode(Convert.ToBase64String(signData));
            return sign;
        }
    }

    #endregion HmacSHA256加密

    #region 空值判断
    /// <summary>
    /// 判断字符串是否为非null或非空
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static bool IsNotNull(this string str) => !IsNull(str);
    /// <summary>
    /// 判断泛型集合是否为非null或非空
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static bool IsNotNull<T>(this IEnumerable<T> source) => !IsNull(source);
    /// <summary>
    /// 判断集合是否为非null或非空
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    public static bool IsNotNull<T>(this List<T> list) => !IsNull(list);
    /// <summary>
    /// 检查对象是否为非null或非空
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static bool IsNotNull(this object obj) => !IsNull(obj);
    /// <summary>
    /// 判断多个对象是否全部为非null或非空
    /// </summary>
    /// <param name="objs"></param>
    /// <returns></returns>
    public static bool AreAllNotNull(params object[] objs) => !AreAllNull(objs);

    /// <summary>
    /// 深度检查对象属性是否全不为空（仅当需要极致判空时用）
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static bool DeepIsNotNull(this object obj) => !DeepIsNull(obj);
    /// <summary>
    /// 判断字符串是否为null或空
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static bool IsNull(this string str)
    {
        return str == null || str == "" || string.IsNullOrWhiteSpace(str);
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
    /// 检查对象是否为null或空
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static bool IsNull(this object obj)
    {
        if (obj == null)
            return true;

        Type type = obj.GetType();

        // 判断字符串
        if (obj is string str)
            return string.IsNullOrWhiteSpace(str);

        // 判断集合/数组
        if (obj is IEnumerable enumerable)
        {
            return !enumerable.Cast<object>().Any();
        }

        // 判断可空值类型 (Nullable<T>)
        if (Nullable.GetUnderlyingType(type) != null)
        {
            return obj.Equals(Activator.CreateInstance(type));
        }

        // 判断 Guid.Empty
        if (obj is Guid guid)
            return guid == Guid.Empty;

        // 判断 DateTime.MinValue
        if (obj is DateTime dateTime)
            return dateTime == DateTime.MinValue;

        // 基础值类型（int, double, float, bool 等），默认非空
        if (type.IsValueType)
            return false;

        // 对于普通对象，检查所有可读属性是否全部为null
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                              .Where(p => p.CanRead)
                              .ToList();

        if (properties.Count == 0)
            return false; // 没有属性的对象，认为非空

        foreach (var property in properties)
        {
            var value = property.GetValue(obj);
            if (value != null)
                return false; // 只要有一个属性非空，就认为对象非空
        }

        return true; // 所有属性都为null，判定为空
    }

    /// <summary>
    /// 判断多个对象是否全部为null或空
    /// </summary>
    /// <param name="objs"></param>
    /// <returns></returns>
    public static bool AreAllNull(params object[] objs)
    {
        foreach (var obj in objs)
        {
            if (!IsNull(obj))
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// 深度检查对象属性是否全为空（仅当需要极致判空时用）
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static bool DeepIsNull(this object obj)
    {
        if (obj == null) return true;

        var properties = obj.GetType().GetProperties();
        return properties.Length == 0 || properties.All(p => p.GetValue(obj) == null);
    }

    #endregion 空值判断
}