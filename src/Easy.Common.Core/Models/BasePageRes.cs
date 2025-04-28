namespace Easy.Common.Core;

/// <summary>
/// 返回类
/// </summary>
public class BasePageRes
{
    /// <summary>
    /// 获取或设置数据行
    /// </summary>
    public object? Rows { get; set; } = null;

    /// <summary>
    /// 获取或设置总数
    /// </summary>
    public int Total { get; set; } = 0;

    /// <summary>
    /// 获取或设置页数
    /// </summary>
    public int PageCount { get; set; } = 0;
}

/// <summary>
/// 返回类(泛型)
/// </summary>
/// <typeparam name="T"></typeparam>
public class BasePageRes<T>
{
    /// <summary>
    /// 获取或设置数据行
    /// </summary>
    public List<T> Rows { get; set; } = null;

    /// <summary>
    /// 获取或设置总数
    /// </summary>
    public int Total { get; set; } = 0;

    /// <summary>
    /// 获取或设置页数
    /// </summary>
    public int PageCount { get; set; } = 0;
}