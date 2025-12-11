using System.Reflection;

namespace Easy.Common.Core;

/// <summary>
/// 字典工具类
/// </summary>
public static class DictionaryExtensions
{
    /// <summary>
    /// 对象转换成字典
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static IDictionary<string, object> ToDictonary(object value)
    {
        IDictionary<string, object> valuePairs = new Dictionary<string, object>();
        // 1、获取反射类型
        Type type = value.GetType();

        // 2、获取所有反射属性
        PropertyInfo[] propertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        // 3、遍历PropertyInfo
        foreach (PropertyInfo info in propertyInfos)
        {
            valuePairs.Add(info.Name, Convert.ToString(info.GetValue(value)));
        }

        return valuePairs;
    }

    /// <summary>
    /// 字典转换为实体
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dict"></param>
    /// <returns></returns>
    public static T DicToEntity<T>(this IDictionary<string, object> dict) where T : new()
    {
        T obj = new T();
        Type type = typeof(T);
        foreach (var item in dict)
        {
            PropertyInfo prop = type.GetProperty(item.Key);
            if (prop != null && prop.CanWrite)
            {
                prop.SetValue(obj, Convert.ChangeType(item.Value, prop.PropertyType), null);
            }
        }
        return obj;
    }
}