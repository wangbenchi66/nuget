namespace Easy.Common.Core
{
    /// <summary>
    /// 分页列表接口，扩展了 IList[T>，用于表示分页结果。
    /// </summary>
    /// <typeparam name="T">分页数据的类型</typeparam>
    public interface IPageList<T> : IList<T>
    {
        /// <summary>
        /// 当前页的页码（从1开始）。
        /// </summary>
        int PageIndex { get; }

        /// <summary>
        /// 每页显示的记录数。
        /// </summary>
        int PageSize { get; }

        /// <summary>
        /// 当前分页结果中的总记录数。
        /// </summary>
        int TotalCount { get; }

        /// <summary>
        /// 总页数，根据总记录数和每页记录数计算得出。
        /// </summary>
        int TotalPages { get; }

        /// <summary>
        /// 是否存在上一页。
        /// </summary>
        bool HasPreviousPage { get; }

        /// <summary>
        /// 是否存在下一页。
        /// </summary>
        bool HasNextPage { get; }
    }
}