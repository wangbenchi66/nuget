namespace Easy.Common.Core;

/// <summary>
/// 提供数字相关的扩展方法。
/// </summary>
public static class NumberExtensions
{
    /// <summary>
    /// 保留指定位数的小数（四舍五入）。
    /// </summary>
    /// <param name="value">要处理的数值。</param>
    /// <param name="digits">保留的小数位数。</param>
    /// <returns>四舍五入后的数值。</returns>
    public static decimal RoundTo(this decimal value, int digits)
    {
        return Math.Round(value, digits);
    }

    /// <summary>
    /// 保留指定位数的小数（四舍五入）。
    /// </summary>
    /// <param name="value">要处理的数值。</param>
    /// <param name="digits">保留的小数位数。</param>
    /// <returns>四舍五入后的数值。</returns>
    public static double RoundTo(this double value, int digits)
    {
        return Math.Round(value, digits);
    }

    /// <summary>
    /// 判断数字是否在指定范围内（包含边界）。
    /// </summary>
    /// <typeparam name="T">可比较的类型。</typeparam>
    /// <param name="value">要判断的数值。</param>
    /// <param name="min">最小值。</param>
    /// <param name="max">最大值。</param>
    /// <returns>是否在范围内。</returns>
    public static bool IsBetween<T>(this T value, T min, T max) where T : IComparable<T>
    {
        return value.CompareTo(min) >= 0 && value.CompareTo(max) <= 0;
    }

    /// <summary>
    /// 将数字限制在指定范围内。
    /// </summary>
    /// <typeparam name="T">可比较的类型。</typeparam>
    /// <param name="value">要限制的数值。</param>
    /// <param name="min">最小值。</param>
    /// <param name="max">最大值。</param>
    /// <returns>限制后的数值。</returns>
    public static T Clamp<T>(this T value, T min, T max) where T : IComparable<T>
    {
        if (value.CompareTo(min) < 0) return min;
        if (value.CompareTo(max) > 0) return max;
        return value;
    }

    /// <summary>
    /// 判断 double 是否为有效数字（非 NaN 或 Infinity）。
    /// </summary>
    /// <param name="value">要判断的数值。</param>
    /// <returns>是否有效。</returns>
    public static bool IsValidNumber(this double value)
    {
        return !double.IsNaN(value) && !double.IsInfinity(value);
    }

    /// <summary>
    /// 判断数值是正负然后添加对应的符号（正数加+，负数加-，零不加符号）。
    /// </summary>
    /// <param name="value">要处理的数值。</param>
    /// <returns>带符号的字符串。</returns>
    public static string WithSign(this decimal value)
    {
        if (value > 0) return $"+{value}";
        if (value < 0) return $"-{Math.Abs(value)}";
        return value.ToString();
    }

    /// <summary>
    /// 判断数值是正负然后添加对应的符号（正数加+，负数加-，零不加符号）。
    /// </summary>
    /// <param name="value">要处理的数值。</param>
    /// <returns>带符号的字符串。</returns>
    public static string WithSign(this double value)
    {
        if (value > 0) return $"+{value}";
        if (value < 0) return $"-{Math.Abs(value)}";
        return value.ToString();
    }

    /// <summary>
    /// 添加正负符号并且保留指定位数的小数。
    /// </summary>
    /// <param name="value">要处理的数值。</param>
    /// <param name="digits">保留的小数位数。</param>
    /// <returns>带符号的字符串。</returns>
    public static string WithSignAndDigits(this decimal value, int digits)
    {
        var rounded = Math.Round(Math.Abs(value), digits);
        if (value > 0) return $"+{rounded}";
        if (value < 0) return $"-{rounded}";
        return rounded.ToString();
    }

    /// <summary>
    /// 添加正负符号并且保留指定位数的小数。
    /// </summary>
    /// <param name="value">要处理的数值。</param>
    /// <param name="digits">保留的小数位数。</param>
    /// <returns>带符号的字符串。</returns>
    public static string WithSignAndDigits(this double value, int digits)
    {
        var rounded = Math.Round(Math.Abs(value), digits);
        if (value > 0) return $"+{rounded}";
        if (value < 0) return $"-{rounded}";
        return rounded.ToString();
    }

    /// <summary>
    /// 保留指定小数位数，返回字符串。
    /// </summary>
    /// <param name="value">要处理的数值。</param>
    /// <param name="digits">保留的小数位数。</param>
    /// <returns>格式化后的字符串。</returns>
    public static string ToFixed(this decimal value, int digits)
    {
        return Math.Round(value, digits).ToString($"F{digits}");
    }

    /// <summary>
    /// 保留指定小数位数，返回字符串。
    /// </summary>
    /// <param name="value">要处理的数值。</param>
    /// <param name="digits">保留的小数位数。</param>
    /// <returns>格式化后的字符串。</returns>
    public static string ToFixed(this double value, int digits)
    {
        return Math.Round(value, digits).ToString($"F{digits}");
    }

    /// <summary>
    /// 保留指定小数位数，返回字符串（string类型输入）。
    /// </summary>
    /// <param name="value">要处理的字符串。</param>
    /// <param name="digits">保留的小数位数。</param>
    /// <returns>格式化后的字符串。</returns>
    public static string ToFixed(this string value, int digits)
    {
        if (decimal.TryParse(value, out var dec))
            return dec.ToFixed(digits);
        if (double.TryParse(value, out var dbl))
            return dbl.ToFixed(digits);
        return value;
    }
}