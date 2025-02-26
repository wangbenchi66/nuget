using System.Reflection;

namespace Easy.Common.Core
{
    /// <summary>
    /// 扩展数据转换
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// 数据转换为int类型
        /// </summary>
        /// <param name="thisValue">要转换的对象</param>
        /// <returns>转换后的int值</returns>
        public static int ObjectToInt(this object thisValue)
        {
            int result = 0;
            if (thisValue == null)
                return 0;
            return thisValue != null && thisValue != DBNull.Value && int.TryParse(thisValue.ToString(), out result) ? result : result;
        }

        /// <summary>
        /// 数据转换为int类型
        /// </summary>
        /// <param name="thisValue">要转换的对象</param>
        /// <param name="errorValue">转换失败时返回的默认值</param>
        /// <returns>转换后的int值</returns>
        public static int ObjectToInt(this object thisValue, int errorValue)
        {
            int result = 0;
            return thisValue != null && thisValue != DBNull.Value && int.TryParse(thisValue.ToString(), out result) ? result : errorValue;
        }

        /// <summary>
        /// 数据转换为Double类型
        /// </summary>
        /// <param name="thisValue">要转换的对象</param>
        /// <returns>转换后的Double值</returns>
        public static double ObjectToDouble(this object thisValue)
        {
            double result = 0.0;
            return thisValue != null && thisValue != DBNull.Value && double.TryParse(thisValue.ToString(), out result) ? result : 0.0;
        }

        /// <summary>
        /// 数据转换为Double类型
        /// </summary>
        /// <param name="thisValue">要转换的对象</param>
        /// <param name="errorValue">转换失败时返回的默认值</param>
        /// <returns>转换后的Double值</returns>
        public static double ObjectToDouble(this object thisValue, double errorValue)
        {
            double result = 0.0;
            return thisValue != null && thisValue != DBNull.Value && double.TryParse(thisValue.ToString(), out result) ? result : errorValue;
        }

        /// <summary>
        /// 数据转换为Float类型
        /// </summary>
        /// <param name="thisValue">要转换的对象</param>
        /// <returns>转换后的Float值</returns>
        public static float ObjectToFloat(this object thisValue)
        {
            float result = 0;
            return thisValue != null && thisValue != DBNull.Value && float.TryParse(thisValue.ToString(), out result) ? result : 0;
        }

        /// <summary>
        /// 数据转换为Float类型
        /// </summary>
        /// <param name="thisValue">要转换的对象</param>
        /// <param name="errorValue">转换失败时返回的默认值</param>
        /// <returns>转换后的Float值</returns>
        public static float ObjectToFloat(this object thisValue, float errorValue)
        {
            float result = 0;
            return thisValue != null && thisValue != DBNull.Value && float.TryParse(thisValue.ToString(), out result) ? result : errorValue;
        }

        /// <summary>
        /// 数据转换为String类型
        /// </summary>
        /// <param name="thisValue">要转换的对象</param>
        /// <returns>转换后的String值</returns>
        public static string ObjectToString(this object thisValue)
        {
            return thisValue != null ? thisValue.ToString().Trim() : "";
        }

        /// <summary>
        /// 数据转换为String类型
        /// </summary>
        /// <param name="thisValue">要转换的对象</param>
        /// <param name="errorValue">转换失败时返回的默认值</param>
        /// <returns>转换后的String值</returns>
        public static string ObjectToString(this object thisValue, string errorValue)
        {
            return thisValue != null ? thisValue.ToString().Trim() : errorValue;
        }

        /// <summary>
        /// 数据转换为Decimal类型
        /// </summary>
        /// <param name="thisValue">要转换的对象</param>
        /// <returns>转换后的Decimal值</returns>
        public static Decimal ObjectToDecimal(this object thisValue)
        {
            Decimal result = new Decimal();
            return thisValue != null && thisValue != DBNull.Value && Decimal.TryParse(thisValue.ToString(), out result) ? result : Decimal.Zero;
        }

        /// <summary>
        /// 数据转换为Decimal类型
        /// </summary>
        /// <param name="thisValue">要转换的对象</param>
        /// <param name="errorValue">转换失败时返回的默认值</param>
        /// <returns>转换后的Decimal值</returns>
        public static Decimal ObjectToDecimal(this object thisValue, Decimal errorValue)
        {
            Decimal result = new Decimal();
            return thisValue != null && thisValue != DBNull.Value && Decimal.TryParse(thisValue.ToString(), out result) ? result : errorValue;
        }

        /// <summary>
        /// 数据转换为DateTime类型
        /// </summary>
        /// <param name="thisValue">要转换的对象</param>
        /// <returns>转换后的DateTime值</returns>
        public static DateTime ObjectToDate(this object thisValue)
        {
            DateTime result = DateTime.MinValue;
            if (thisValue != null && thisValue != DBNull.Value && DateTime.TryParse(thisValue.ToString(), out result))
                result = Convert.ToDateTime(thisValue);
            return result;
        }

        /// <summary>
        /// 数据转换为DateTime类型
        /// </summary>
        /// <param name="thisValue">要转换的对象</param>
        /// <param name="errorValue">转换失败时返回的默认值</param>
        /// <returns>转换后的DateTime值</returns>
        public static DateTime ObjectToDate(this object thisValue, DateTime errorValue)
        {
            DateTime result = DateTime.MinValue;
            return thisValue != null && thisValue != DBNull.Value && DateTime.TryParse(thisValue.ToString(), out result) ? result : errorValue;
        }

        /// <summary>
        /// 数据转换为bool类型
        /// </summary>
        /// <param name="thisValue">要转换的对象</param>
        /// <returns>转换后的bool值</returns>
        public static bool ObjectToBool(this object thisValue)
        {
            bool result = false;
            return thisValue != null && thisValue != DBNull.Value && bool.TryParse(thisValue.ToString(), out result) ? result : result;
        }

        /// <summary>
        /// datetime转换为unixtime
        /// </summary>
        /// <param name="time">要转换的DateTime对象</param>
        /// <returns>转换后的unixtime值</returns>
        public static long ObjectToDateTimeInt(this DateTime time)
        {
            DateTime startTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            DateTime timeUTC = DateTime.SpecifyKind(time, DateTimeKind.Utc);
            TimeSpan ts = (timeUTC - startTime);
            return (Int64)ts.TotalMilliseconds;
        }

        /// <summary>
        /// 字典转换为实体对象
        /// </summary>
        /// <typeparam name="T">目标实体类型</typeparam>
        /// <param name="dic">包含数据的字典</param>
        /// <returns>转换后的实体对象</returns>
        public static T DicToEntity<T>(this Dictionary<string, object> dic)
        {
            return new List<Dictionary<string, object>>() { dic }.DicToList<T>().ToList()[0];
        }

        /// <summary>
        /// 字典列表转换为实体对象列表
        /// </summary>
        /// <typeparam name="T">目标实体类型</typeparam>
        /// <param name="dicList">包含数据的字典列表</param>
        /// <returns>转换后的实体对象列表</returns>
        public static List<T> DicToList<T>(this List<Dictionary<string, object>> dicList)
        {
            return dicList.DicToIEnumerable<T>().ToList();
        }

        /// <summary>
        /// 字典列表转换为实体对象列表
        /// </summary>
        /// <param name="dicList">包含数据的字典列表</param>
        /// <param name="type">目标实体类型</param>
        /// <returns>转换后的实体对象列表</returns>
        public static object DicToList(this List<Dictionary<string, object>> dicList, Type type)
        {
            return typeof(ObjectExtensions).GetMethod("DicToList")
               .MakeGenericMethod(new Type[] { type })
               .Invoke(typeof(ObjectExtensions), new object[] { dicList });
        }

        /// <summary>
        /// 字典列表转换为实体对象枚举
        /// </summary>
        /// <typeparam name="T">目标实体类型</typeparam>
        /// <param name="dicList">包含数据的字典列表</param>
        /// <returns>转换后的实体对象枚举</returns>
        public static IEnumerable<T> DicToIEnumerable<T>(this List<Dictionary<string, object>> dicList)
        {
            foreach (Dictionary<string, object> dic in dicList)
            {
                T model = Activator.CreateInstance<T>();
                foreach (PropertyInfo property in model.GetType()
                    .GetProperties(BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance))
                {
                    if (!dic.TryGetValue(property.Name, out object value)) continue;
                    property.SetValue(model, value?.ToString().ChangeType(property.PropertyType), null);
                }
                yield return model;
            }
        }
        /// <summary>
        /// 类型转换
        /// </summary>
        /// <param name="value"></param>
        /// <param name="conversionType"></param>
        /// <returns></returns>
        public static object ChangeType(this string value, Type conversionType)
        {
            if (conversionType.IsGenericType && conversionType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (string.IsNullOrEmpty(value))
                    return null;
                conversionType = Nullable.GetUnderlyingType(conversionType);
            }
            return Convert.ChangeType(value, conversionType);
        }
    }
}