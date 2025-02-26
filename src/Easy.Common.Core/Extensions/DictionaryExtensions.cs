using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Easy.Common.Core;

/// <summary>
/// 字典工具类
/// </summary>
public class DictionaryExtensions
{
    /// <summary>
    /// 对象转换成字典
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public IDictionary<string, object> ToDictonary(object value)
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
}