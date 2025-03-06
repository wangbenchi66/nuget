using System.Data.Common;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Easy.EF.Core.BiewModels;

namespace Easy.EF.Core.BaseProvider
{
    public interface IBaseEFRepository<TDBContext, T> where TDBContext : DbContext where T : class
    {
        TDBContext EFContext { get; }

        #region 获取单个实体

        /// <summary>
        /// 获取单个实体
        /// </summary>
        /// <param name="whereExpression"></param>
        /// <returns></returns>
        T GetSingle(Expression<Func<T, bool>> whereExpression);

        /// <summary>
        /// 获取单个实体
        /// </summary>
        /// <param name="whereExpression"></param>
        /// <returns></returns>
        Task<T> GetSingleAsync(Expression<Func<T, bool>> whereExpression);

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
        Task<List<T>> GetListAsync(Expression<Func<T, bool>> predicate, string orderBy = "");

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
        /// <param name="entity">实体类</param>
        /// <param name="isSave">保存更改</param>
        /// <returns></returns>
        int Insert(T entity, bool isSave = false);

        /// <summary>
        /// 写入实体数据
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <param name="isSave">保存更改</param>
        /// <returns></returns>
        Task<int> InsertAsync(T entity, bool isSave = false);

        /// <summary>
        /// 批量写入实体数据
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <param name="isSave">保存更改</param>
        /// <returns></returns>
        int Insert(List<T> entity, bool isSave = false);

        /// <summary>
        /// 批量写入实体数据
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <param name="isSave">保存更改</param>
        /// <returns></returns>
        Task<int> InsertAsync(List<T> entity, bool isSave = false);

        int InsertReturnIdentity(T insertObj);

        Task<int> InsertReturnIdentityAsync(T insertObj);

        long InsertReturnBigIdentity(T insertObj);

        Task<long> InsertReturnBigIdentityAsync(T insertObj);


        #endregion 写入实体数据

        #region 更新实体数据

        /// <summary>
        /// 批量更新实体数据
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="isSave">保存更改</param>
        /// <returns></returns>
        bool Update(List<T> entity, bool isSave = false);

        /// <summary>
        /// 批量更新实体数据
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="isSave">保存更改</param>
        /// <returns></returns>
        Task<bool> UpdateAsync(List<T> entity, bool isSave = false);

        /// <summary>
        /// 更新实体数据
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="isSave">保存更改</param>
        /// <returns></returns>
        bool Update(T entity, bool isSave = false);

        /// <summary>
        /// 更新实体数据
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="isSave">保存更改</param>
        /// <returns></returns>
        Task<bool> UpdateAsync(T entity, bool isSave = false);

        /// <summary>
        /// 更新实体指定列
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="property">更新值，用法：x => new { x.Name, x.CreateTime }</param>
        /// <param name="isSave">是否保存更改</param>
        /// <returns></returns>
        bool Update(T entity, Expression<Func<T, object>> property, bool isSave = false);

        /// <summary>
        /// 更新实体指定列
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="property">更新值，用法：x => new { x.Name, x.CreateTime }</param>
        /// <param name="isSave">是否保存更改</param>
        /// <returns></returns>
        Task<bool> UpdateAsync(T entity, Expression<Func<T, object>> property, bool isSave = false);

        /// <summary>
        /// 更新实体指定列
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="property">更新值，用法：x => new { x.Name, x.CreateTime }</param>
        /// <param name="whereColumns">根据列值条件更新，用法：x => new { x.Name }</param>
        /// <param name="isSave">是否保存更改</param>
        /// <returns></returns>
        bool Update(T entity, Expression<Func<T, object>> property, Expression<Func<T, object>> whereColumns, bool isSave = false);

        #endregion 更新实体数据

        #region 删除数据

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <param name="isSave">保存更改</param>
        /// <returns></returns>
        bool Delete(T entity, bool isSave = false);

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <param name="isSave">保存更改</param>
        /// <returns></returns>
        Task<bool> DeleteAsync(T entity, bool isSave = false);

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

        #region 查询分页数据

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
        /// 分页查询
        /// </summary>
        /// <param name="queryable"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pagesize"></param>
        /// <param name="rowcount"></param>
        /// <param name="orderBy"></param>
        /// <param name="returnRowCount"></param>
        /// <returns></returns>
        IQueryable<T> IQueryablePage(IQueryable<T> queryable, int pageIndex, int pagesize, out int rowcount, Dictionary<string, QueryOrderBy> orderBy, bool returnRowCount = true);

        #endregion 查询分页数据

        #region 执行SQL语句

        /// <summary>
        /// 执行sql语句并返回List[T]
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        List<T> SqlQuery(string sql, object? parameters = null);

        /// <summary>
        /// 执行sql语句并返回List
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        Task<List<T>> SqlQueryAsync(string sql, object? parameters = null);

        /*/// <summary>
        /// 执行sql语句并返回到指定实体中
        /// </summary>
        /// <typeparam name="T">映射到这个实体</typeparam>
        /// <param name="sql">sql</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        List<TResult> SqlQuery<TResult>(string sql, DbParameter[]? parameters) whereExpression TResult : class;

        /// <summary>
        /// 执行sql语句并返回到指定实体中
        /// </summary>
        /// <typeparam name="TResult">映射到这个实体</typeparam>
        /// <param name="sql">sql</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        Task<List<TResult>> SqlQueryAsync<TResult>(string sql, DbParameter[]? parameters = null);*/

        /// <summary>
        /// 执行分页sql语句并返回到指定实体中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        IPageList<T> SqlPageQuery<T>(string sql, object? parameters, int pageIndex, int pageSize) where T : class, new();

        /// <summary>
        /// 执行分页sql语句并返回到指定实体中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        Task<IPageList<T>> SqlPageQueryAsync<T>(string sql, object? parameters, int pageIndex, int pageSize) where T : class, new();

        /// <summary>
        /// 执行sql语句返回影响行数
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数</param>
        /// <returns>返回影响行数</returns>
        int ExecuteSql(string sql, object? parameters);

        /// <summary>
        /// 执行sql语句返回影响行数
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数</param>
        /// <returns>返回影响行数</returns>
        Task<int> ExecuteSqlAsync(string sql, object? parameters);

        #endregion 执行SQL语句

        #region 开启事务

        /// <summary>
        /// 开启事务
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        bool DbContextBeginTransaction(Func<bool> func);

        #endregion 开启事务

        /// <summary>
        /// 保存更改
        /// </summary>
        /// <returns></returns>
        int SaveChanges();

        /// <summary>
        /// 保存更改
        /// </summary>
        /// <returns></returns>
        Task<int> SaveChangesAsync();
    }
}