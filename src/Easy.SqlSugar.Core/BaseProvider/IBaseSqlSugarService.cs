using System.Linq.Expressions;
using Easy.SqlSugar.Core.BiewModels;
using SqlSugar;

namespace Easy.SqlSugar.Core.BaseProvider
{
    public interface IBaseSqlSugarService<T> : ISimpleClient<T> where T : class, new()
    {
        #region 获取单个实体


        #endregion 获取单个实体

        #region 获取列表

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        List<T> GetList(Expression<Func<T, bool>> where);

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        Task<List<T>> GetListAsync(Expression<Func<T, bool>> where);

        /// <summary>
        /// 根据条件查询数据
        /// </summary>
        /// <param name="predicate">条件表达式树</param>
        /// <param name="orderBy">排序字段，如name asc,age desc</param>
        /// <returns>泛型实体集合</returns>
        Task<List<T>> GetListAsync(Expression<Func<T, bool>> predicate, string orderBy);

        /// <summary>
        /// 根据条件查询数据
        /// </summary>
        /// <param name="predicate">条件表达式树</param>
        /// <param name="orderByPredicate">排序字段</param>
        /// <param name="orderByType">排序顺序</param>
        /// <returns>泛型实体集合</returns>
        Task<List<T>> GetListAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> orderByPredicate, OrderByType orderByType);

        #endregion 获取列表

        #region 写入实体数据

        /// <summary>
        /// 写入实体数据
        /// </summary>
        /// <param name="entity">实体数据</param>
        /// <param name="insertColumns">插入的列</param>
        /// <returns></returns>
        int Insert(T entity, Expression<Func<T, object>>? insertColumns = null);

        /// <summary>
        /// 写入实体数据
        /// </summary>
        /// <param name="entity">实体数据</param>
        /// <param name="insertColumns">插入的列</param>
        /// <returns></returns>
        Task<int> InsertAsync(T entity, Expression<Func<T, object>>? insertColumns = null);

        /// <summary>
        /// 批量写入实体数据
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <returns></returns>
        int Insert(List<T> entity);

        /// <summary>
        /// 批量写入实体数据
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <returns></returns>
        Task<int> InsertAsync(List<T> entity);

        #endregion 写入实体数据

        #region 更新实体数据

        /// <summary>
        /// 批量更新实体数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        int Update(List<T> entity);

        /// <summary>
        /// 批量更新实体数据    
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> UpdateAsync(List<T> entity);

        /// <summary>
        /// 批量更新实体数据指定字段
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="updateColumns">要更新的字段x=>new {x.a,x.b}</param>
        /// <returns></returns>
        int Update(List<T> entity, Expression<Func<T, object>> updateColumns);

        /// <summary>
        /// 批量更新实体数据指定字段
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="updateColumns">要更新的字段x=>new {x.a,x.b}</param>
        /// <returns></returns>
        Task<int> UpdateAsync(List<T> entity, Expression<Func<T, object>> updateColumns);

        /// <summary>
        /// 更新实体数据指定字段
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="updateColumns">要更新的字段x=>new {x.a,x.b}</param>
        /// <returns></returns>
        int Update(T entity, Expression<Func<T, object>> updateColumns);

        /// <summary>
        /// 更新实体数据指定字段
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="updateColumns">要更新的字段x=>new {x.a,x.b}</param>
        /// <returns></returns>
        Task<int> UpdateAsync(T entity, Expression<Func<T, object>> updateColumns);

        /// <summary>
        /// 更新某个字段
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="updateColumns">要更新的字段x=>new {x.a,x.b}</param>
        /// <param name="where">更新条件lamdba判断 x=>x.a==1</param>
        /// <returns></returns>
        int Update(T entity, Expression<Func<T, object>> updateColumns, Expression<Func<T, object>> where);

        /// <summary>
        /// 更新某个字段
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="updateColumns">要更新的字段x=>new {x.a,x.b}</param>
        /// <param name="where">更新条件lamdba判断 x=>x.a==1</param>
        /// <returns></returns>
        Task<int> UpdateAsync(T entity, Expression<Func<T, object>> updateColumns, Expression<Func<T, object>> where);

        /// <summary>
        /// 更新某个字段
        /// </summary>
        /// <param name="entitys"></param>
        /// <param name="updateColumns">要更新的字段x=>new {x.a,x.b}</param>
        /// <param name="where">更新条件lamdba判断 x=>x.a==1</param>
        /// <returns></returns>
        int Update(List<T> entitys, Expression<Func<T, object>> updateColumns, Expression<Func<T, object>> where);

        /// <summary>
        /// 更新某个字段
        /// </summary>
        /// <param name="entitys"></param>
        /// <param name="updateColumns">要更新的字段x=>new {x.a,x.b}</param>
        /// <param name="where">更新条件lamdba判断 x=>x.a==1</param>
        /// <returns></returns>
        Task<int> UpdateAsync(List<T> entitys, Expression<Func<T, object>> updateColumns, Expression<Func<T, object>> where);

        /// <summary>
        /// 根据条件更新
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="updateColumns">要更新的列x=>new {x.a,x.b}</param>
        /// <param name="ignoreColumns">忽略的列x=>new {x.a,x.b}</param>
        /// <param name="where">更新条件lamdba判断 x=>x.a==1</param>
        /// <returns></returns>
        int Update(T entity, Expression<Func<T, object>> updateColumns, Expression<Func<T, object>> ignoreColumns, Expression<Func<T, object>> where);

        /// <summary>
        /// 根据条件更新
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="updateColumns">要更新的列x=>new {x.a,x.b}</param>
        /// <param name="ignoreColumns">忽略的列x=>new {x.a,x.b}</param>
        /// <param name="where">更新条件lamdba判断 x=>x.a==1</param>
        /// <returns></returns>
        Task<int> UpdateAsync(T entity, Expression<Func<T, object>> updateColumns, Expression<Func<T, object>> ignoreColumns, Expression<Func<T, object>> where);

        /// <summary>
        /// 无实体更新
        /// </summary>
        /// <param name="updateColumns">要更新的列x=>new {x.a,x.b}</param>
        /// <param name="where">更新条件lamdba判断 x=>x.a==1</param>
        /// <returns></returns>
        int Update(Expression<Func<T, object>> updateColumns, Expression<Func<T, object>> where);

        /// <summary>
        /// 无实体更新
        /// </summary>
        /// <param name="updateColumns">要更新的列x=>new {x.a,x.b}</param>
        /// <param name="where">更新条件lamdba判断 x=>x.a==1</param>
        /// <returns></returns>
        Task<int> UpdateAsync(Expression<Func<T, object>> updateColumns, Expression<Func<T, object>> where);

        /// <summary>
        /// 无实体更新
        /// </summary>
        /// <param name="updateColumns">要更新的列</param>
        /// <param name="where">更新条件lamdba判断 x=>x.a==1</param>
        /// <returns></returns>
        int Update(Dictionary<string, object> updateColumns, Expression<Func<T, object>> where);

        /// <summary>
        /// 无实体更新
        /// </summary>
        /// <param name="updateColumns">要更新的列</param>
        /// <param name="where">更新条件lamdba判断 x=>x.a==1</param>
        /// <returns></returns>
        Task<int> UpdateAsync(Dictionary<string, object> updateColumns, Expression<Func<T, object>> where);

        /// <summary>
        /// 无实体更新(批量)
        /// </summary>
        /// <param name="updateColumns">要更新的列</param>
        /// <param name="where">更新条件lamdba判断 x=>x.a==1</param>
        /// <returns></returns>
        int Update(List<Dictionary<string, object>> updateColumns, Expression<Func<T, object>> where);

        /// <summary>
        /// 无实体更新(批量)
        /// </summary>
        /// <param name="updateColumns">要更新的列</param>
        /// <param name="where">更新条件lamdba判断 x=>x.a==1</param>
        /// <returns></returns>
        Task<int> UpdateAsync(List<Dictionary<string, object>> updateColumns, Expression<Func<T, object>> where);

        #endregion 更新实体数据

        #region 添加或更新

        /// <summary>
        /// 添加或更新
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="where">条件lamdba判断 x=>new {x.Id}存在则修改 不存在则更新</param>
        /// <returns></returns>
        int InsertOrUpdate(T entity, Expression<Func<T, object>> where);

        /// <summary>
        /// 添加或更新
        /// </summary>
        /// <param name="entitys"></param>
        /// <param name="where">条件lamdba判断 x=>new {x.Id}存在则修改 不存在则更新</param>
        /// <returns></returns>
        int InsertOrUpdate(List<T> entitys, Expression<Func<T, object>> where);

        /// <summary>
        /// 添加或更新
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="where">条件lamdba判断 x=>new {x.Id}存在则修改 不存在则更新</param>
        /// <returns></returns>
        Task<int> InsertOrUpdateAsync(T entity, Expression<Func<T, object>> where);

        /// <summary>
        /// 添加或更新
        /// </summary>
        /// <param name="entitys"></param>
        /// <param name="where">条件lamdba判断 x=>new {x.Id}存在则修改 不存在则更新</param>
        /// <returns></returns>
        Task<int> InsertOrUpdateAsync(List<T> entitys, Expression<Func<T, object>> where);

        /*
                /// <summary>
                /// 添加或更新
                /// </summary>
                /// <param name="entity"></param>
                /// <param name="columns">要添加或更新的列x=>new {x.a,x.b}</param>
                /// <param name="where">条件lamdba判断 x=>new {x.Id}存在则修改 不存在则更新</param>
                /// <returns></returns>
                int InsertOrUpdate(TResult entity, Expression<Func<TResult, object>> columns, Expression<Func<TResult, object>> where);

                /// <summary>
                /// 添加或更新
                /// </summary>
                /// <param name="entitys"></param>
                /// <param name="columns">要添加或更新的列x=>new {x.a,x.b}</param>
                /// <param name="where">条件lamdba判断 x=>new {x.Id}存在则修改 不存在则更新</param>
                /// <returns></returns>
                int InsertOrUpdate(List<TResult> entitys, Expression<Func<TResult, object>> columns, Expression<Func<TResult, object>> where);

                /// <summary>
                /// 添加或更新
                /// </summary>
                /// <param name="entity"></param>
                /// <param name="columns">要添加或更新的列x=>new {x.a,x.b}</param>
                /// <param name="where">条件lamdba判断 x=>new {x.Id}存在则修改 不存在则更新</param>
                /// <returns></returns>
                Task<int> InsertOrUpdateAsync(TResult entity, Expression<Func<TResult, object>> columns, Expression<Func<TResult, object>> where);

                /// <summary>
                /// 添加或更新
                /// </summary>
                /// <param name="entitys"></param>
                /// <param name="columns">要添加或更新的列x=>new {x.a,x.b}</param>
                /// <param name="where">条件lamdba判断 x=>new {x.Id}存在则修改 不存在则更新</param>
                /// <returns></returns>
                Task<int> InsertOrUpdateAsync(List<TResult> entitys, Expression<Func<TResult, object>> columns, Expression<Func<TResult, object>> where);*/

        #endregion 添加或更新

        #region 删除数据

        #endregion 删除数据

        #region 判断数据是否存在

        /// <summary>
        /// 判断数据是否存在
        /// </summary>
        /// <param name="predicate">条件表达式树</param>
        /// <returns></returns>
        bool Exists(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// 判断数据是否存在
        /// </summary>
        /// <param name="predicate">条件表达式树</param>
        /// <returns></returns>
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);

        #endregion 判断数据是否存在

        #region 获取数据总数

        /// <summary>
        /// 获取数据总数
        /// </summary>
        /// <param name="predicate">条件表达式树</param>
        /// <returns></returns>
        int GetCount(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// 获取数据总数
        /// </summary>
        /// <param name="predicate">条件表达式树</param>
        /// <returns></returns>
        Task<int> GetCountAsync(Expression<Func<T, bool>> predicate);

        #endregion 获取数据总数

        #region 根据条件查询分页数据

        /// <summary>
        /// 根据条件查询分页数据
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="orderBy"></param>
        /// <param name="pageIndex">当前页面索引</param>
        /// <param name="pageSize">分布大小</param>
        /// <returns></returns>
        IPageList<T> QueryPage(Expression<Func<T, bool>> predicate, string orderBy = "", int pageIndex = 1, int pageSize = 20);

        /// <summary>
        /// 根据条件查询分页数据
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="orderBy"></param>
        /// <param name="pageIndex">当前页面索引</param>
        /// <param name="pageSize">分布大小</param>
        /// <returns></returns>
        Task<IPageList<T>> QueryPageAsync(Expression<Func<T, bool>> predicate, string orderBy = "", int pageIndex = 1, int pageSize = 20);

        /// <summary>
        /// 根据条件查询分页数据
        /// </summary>
        /// <param name="predicate">判断集合</param>
        /// <param name="orderByType">排序方式</param>
        /// <param name="pageIndex">当前页面索引</param>
        /// <param name="pageSize">分布大小</param>
        /// <param name="orderByExpression"></param>
        /// <returns></returns>
        IPageList<T> QueryPage(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> orderByExpression, OrderByType orderByType, int pageIndex = 1, int pageSize = 20);

        /// <summary>
        /// 根据条件查询分页数据
        /// </summary>
        /// <param name="predicate">判断集合</param>
        /// <param name="orderByType">排序方式</param>
        /// <param name="pageIndex">当前页面索引</param>
        /// <param name="pageSize">分布大小</param>
        /// <param name="orderByExpression"></param>
        /// <returns></returns>
        Task<IPageList<T>> QueryPageAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> orderByExpression, OrderByType orderByType, int pageIndex = 1, int pageSize = 20);

        /// <summary>
        /// 分页排序
        /// </summary>
        /// <param name="queryable">查询对象</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pagesize">每页记录数</param>
        /// <param name="rowcount">总记录数</param>
        /// <param name="orderBy">排序条件</param>
        /// <param name="returnRowCount">是否返回总记录数</param>
        /// <returns>分页后的查询对象</returns>
        ISugarQueryable<T> IQueryablePage(ISugarQueryable<T> queryable, int pageIndex, int pagesize, out int rowcount, Dictionary<string, QueryOrderBy> orderBy, bool returnRowCount = true);

        #endregion 根据条件查询分页数据

        #region 事务

        /// <summary>
        /// 事务
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        bool DbContextBeginTransaction(Func<bool> func);

        /// <summary>
        /// 事务
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        Task<bool> DbContextBeginTransactionAsync(Func<Task<bool>> func);

        #endregion 事务
    }
}