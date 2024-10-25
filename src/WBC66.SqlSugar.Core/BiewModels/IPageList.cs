namespace WBC66.SqlSugar.Core.BiewModels
{
    /// <summary>
    /// 定义分页列表接口，继承自 <see cref="IList{T}"/>。
    /// </summary>
    /// <typeparam name="T">列表中元素的类型。</typeparam>
    public interface IPageList<T> : IList<T>
    {
        /// <summary>
        /// 获取当前页的索引。
        /// </summary>
        int PageIndex { get; }

        /// <summary>
        /// 获取每页的大小。
        /// </summary>
        int PageSize { get; }

        /// <summary>
        /// 获取总记录数。
        /// </summary>
        int TotalCount { get; }

        /// <summary>
        /// 获取总页数。
        /// </summary>
        int TotalPages { get; }

        /// <summary>
        /// 获取一个值，该值指示是否有上一页。
        /// </summary>
        bool HasPreviousPage { get; }

        /// <summary>
        /// 获取一个值，该值指示是否有下一页。
        /// </summary>
        bool HasNextPage { get; }
    }
}