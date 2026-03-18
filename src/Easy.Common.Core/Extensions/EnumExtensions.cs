namespace Easy.Common.Core;

/// <summary>
/// 枚举扩展方法
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    /// 获取枚举描述
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="enumValue"></param>
    /// <returns></returns>
    public static string GetDescription<T>(this T enumValue) where T : Enum
    {
        var type = typeof(T);
        var memberInfo = type.GetMember(enumValue.ToString());
        if (memberInfo != null && memberInfo.Length > 0)
        {
            var attributes = memberInfo[0].GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
            if (attributes != null && attributes.Length > 0)
            {
                return ((System.ComponentModel.DescriptionAttribute)attributes[0]).Description;
            }
        }
        return enumValue.ToString();
    }
}