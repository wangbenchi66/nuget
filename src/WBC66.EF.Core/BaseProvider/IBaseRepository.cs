using System.Data.Common;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using WBC66.EF.Core.BiewModels;

namespace WBC66.EF.Core.BaseProvider
{
    public interface IBaseRepository<TDBContext, T> where TDBContext : DbContext where T : class
    {

        #region 获取单个实体

        /// <summary>
        /// 获取单个实体
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        T GetSingle(Expression<Func<T, bool>> where);

        /// <summary>
        /// 获取单个实体
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        Task<T> GetSingleAsync(Expression<Func<T, bool>> where);

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
        /// <returns></returns>
        int Insert(T entity);

        /// <summary>
        /// 写入实体数据
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <returns></returns>
        Task<int> InsertAsync(T entity);

        /// <summary>
        /// 写入实体数据
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <param name="insertColumns">添加列</param>
        /// <returns></returns>
        int Insert(T entity, Expression<Func<T, object>>? insertColumns = default);

        /// <summary>
        /// 写入实体数据
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <param name="insertColumns">添加列</param>
        /// <returns></returns>
        Task<int> InsertAsync(T entity, Expression<Func<T, object>>? insertColumns = default);

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
        bool Update(List<T> entity);

        /// <summary>
        /// 批量更新实体数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> UpdateAsync(List<T> entity);

        /// <summary>
        /// 更新实体数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        bool Update(T entity);

        /// <summary>
        /// 更新实体数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> UpdateAsync(T entity);

        /// <summary>
        /// 更新某个字段
        /// </summary>
        /// <param name="entities">实体</param>
        /// <param name="properties">更新字段</param>
        /// <returns></returns>
        bool Update<TSource>(IEnumerable<TSource> entities, string[] properties) where TSource : class;

        /// <summary>
        /// 更新某个字段
        /// </summary>
        /// <param name="entities">实体</param>
        /// <param name="properties">更新字段</param>
        /// <returns></returns>
        Task<bool> UpdateAsync<TSource>(IEnumerable<TSource> entities, string[] properties) where TSource : class;

        /// <summary>
        /// 根据条件更新
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="lstColumns"></param>
        /// <param name="lstIgnoreColumns"></param>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        bool Update(T entity, List<string>? lstColumns = default, List<string>? lstIgnoreColumns = default, string strWhere = "");

        /// <summary>
        /// 根据条件更新
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="lstColumns"></param>
        /// <param name="lstIgnoreColumns"></param>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        Task<bool> UpdateAsync(T entity, List<string>? lstColumns = default, List<string>? lstIgnoreColumns = default, string strWhere = "");

        #endregion 更新实体数据

        #region 删除数据

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <returns></returns>
        bool Delete(T entity);

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <returns></returns>
        Task<bool> DeleteAsync(T entity);

        /// <summary>
        /// 删除数据(批量)
        /// </summary>
        /// <param name="entity">实体类集合</param>
        /// <returns></returns>
        bool Delete(List<T> entity);

        /// <summary>
        /// 删除数据(批量)
        /// </summary>
        /// <param name="entity">实体类集合</param>
        /// <returns></returns>
        Task<bool> DeleteAsync(List<T> entity);

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<bool> DeleteAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// 根据主键标识批量删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        bool DeleteByIds<T>(object[] ids) where T : class, new();

        /// <summary>
        /// 根据主键标识批量删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task<bool> DeleteByIdsAsync<T>(object[] ids) where T : class, new();

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

        /// <summary>
        /// 执行sql语句并返回到指定实体中
        /// </summary>
        /// <typeparam name="TEntity">映射到这个实体</typeparam>
        /// <param name="sql">sql</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        List<TEntity> SqlQuery<TEntity>(string sql, DbParameter[]? parameters = null);

        /// <summary>
        /// 执行sql语句并返回到指定实体中
        /// </summary>
        /// <typeparam name="TEntity">映射到这个实体</typeparam>
        /// <param name="sql">sql</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        Task<List<TEntity>> SqlQueryAsync<TEntity>(string sql, DbParameter[]? parameters = null);

        /// <summary>
        /// 执行分页sql语句并返回到指定实体中
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        IPageList<TEntity> SqlPageQuery<TEntity>(string sql, object? parameters, int pageIndex, int pageSize) where TEntity : class, new();

        /// <summary>
        /// 执行分页sql语句并返回到指定实体中
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        Task<IPageList<TEntity>> SqlPageQueryAsync<TEntity>(string sql, object? parameters, int pageIndex, int pageSize) where TEntity : class, new();

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

    }
}