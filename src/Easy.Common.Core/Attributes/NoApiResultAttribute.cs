namespace Easy.Common.Core.Attributes
{
    /// <summary>
    /// NoApiResultAttribute特性,用于标记不需要包装的方法,直接返回
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class NoApiResultAttribute : Attribute
    {
    }
}