﻿using System.Linq.Expressions;
using SqlSugar;
using Easy.SqlSugar.Core.BiewModels;

namespace Easy.SqlSugar.Core
{
    /// <summary>
    /// 通用仓储接口
    /// </summary>
    /// <typeparam name="T">泛型实体类型</typeparam>
    public interface IBaseSqlSugarRepository<T> : ISimpleClient<T> where T : class, new()
    {
        /// <summary>
        /// //多租户事务、GetConnection、IsAnyConnection等功能
        /// </summary>
        ITenant SqlSugarTenant { get; }

        /// <summary>
        /// 数据库上下文
        /// </summary>
        ISqlSugarClient SqlSugarDbContext { get; }

        /// <summary>
        /// SqlSugarAdo
        /// </summary>
        IAdo SqlSugarDbContextAdo { get; }

        #region 获取单个实体

        #endregion 获取单个实体

        #region 获取列表

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
        Task<List<T>> GetListAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> orderByPredicate,
            OrderByType orderByType);

        #endregion 获取列表

        #region 写入实体数据

        /// <summary>
        /// 写入实体数据
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <param name="insertColumns">添加列</param>
        /// <returns>影响行数</returns>
        int Insert(T entity, Expression<Func<T, object>>? insertColumns = default);

        /// <summary>
        /// 异步写入实体数据
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <param name="insertColumns">添加列</param>
        /// <returns>影响行数</returns>
        Task<int> InsertAsync(T entity, Expression<Func<T, object>>? insertColumns = default);

        /// <summary>
        /// 批量写入实体数据
        /// </summary>
        /// <param name="entity">实体类集合</param>
        /// <returns>影响行数</returns>
        int Insert(List<T> entity);

        /// <summary>
        /// 异步批量写入实体数据
        /// </summary>
        /// <param name="entity">实体类集合</param>
        /// <returns>影响行数</returns>
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
        /// <param name="where">更新条件lambda判断 x=>x.a==1</param>
        /// <returns></returns>
        int Update(T entity, Expression<Func<T, object>> updateColumns, Expression<Func<T, object>> where);

        /// <summary>
        /// 更新某个字段
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="updateColumns">要更新的字段x=>new {x.a,x.b}</param>
        /// <param name="where">更新条件lambda判断 x=>x.a==1</param>
        /// <returns></returns>
        Task<int> UpdateAsync(T entity, Expression<Func<T, object>> updateColumns, Expression<Func<T, object>> where);

        /// <summary>
        /// 更新某个字段
        /// </summary>
        /// <param name="entitys"></param>
        /// <param name="updateColumns">要更新的字段x=>new {x.a,x.b}</param>
        /// <param name="where">更新条件lambda判断 x=>x.a==1</param>
        /// <returns></returns>
        int Update(List<T> entitys, Expression<Func<T, object>> updateColumns, Expression<Func<T, object>> where);

        /// <summary>
        /// 更新某个字段
        /// </summary>
        /// <param name="entitys"></param>
        /// <param name="updateColumns">要更新的字段x=>new {x.a,x.b}</param>
        /// <param name="where">更新条件lambda判断 x=>x.a==1</param>
        /// <returns></returns>
        Task<int> UpdateAsync(List<T> entitys, Expression<Func<T, object>> updateColumns,
            Expression<Func<T, object>> where);

        /// <summary>
        /// 根据条件更新
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="updateColumns">要更新的列x=>new {x.a,x.b}</param>
        /// <param name="ignoreColumns">忽略的列x=>new {x.a,x.b}</param>
        /// <param name="where">更新条件lambda判断 x=>x.a==1</param>
        /// <returns></returns>
        int Update(T entity, Expression<Func<T, object>> updateColumns, Expression<Func<T, object>> ignoreColumns,
            Expression<Func<T, object>> where);

        /// <summary>
        /// 根据条件更新
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="updateColumns">要更新的列x=>new {x.a,x.b}</param>
        /// <param name="ignoreColumns">忽略的列x=>new {x.a,x.b}</param>
        /// <param name="where">更新条件lambda判断 x=>x.a==1</param>
        /// <returns></returns>
        Task<int> UpdateAsync(T entity, Expression<Func<T, object>> updateColumns,
            Expression<Func<T, object>> ignoreColumns, Expression<Func<T, object>> where);

        /// <summary>
        /// 无实体更新
        /// </summary>
        /// <param name="updateColumns">要更新的列x=>new {x.a,x.b}</param>
        /// <param name="where">更新条件lambda判断 x=>x.a==1</param>
        /// <returns></returns>
        int Update(Expression<Func<T, object>> updateColumns, Expression<Func<T, object>> where);

        /// <summary>
        /// 无实体更新
        /// </summary>
        /// <param name="updateColumns">要更新的列x=>new {x.a,x.b}</param>
        /// <param name="where">更新条件lambda判断 x=>x.a==1</param>
        /// <returns></returns>
        Task<int> UpdateAsync(Expression<Func<T, object>> updateColumns, Expression<Func<T, object>> where);

        /// <summary>
        /// 无实体更新
        /// </summary>
        /// <param name="updateColumns">要更新的列</param>
        /// <param name="where">更新条件lambda判断 x=>x.a==1</param>
        /// <returns></returns>
        int Update(Dictionary<string, object> updateColumns, Expression<Func<T, object>> where);

        /// <summary>
        /// 无实体更新
        /// </summary>
        /// <param name="updateColumns">要更新的列</param>
        /// <param name="where">更新条件lambda判断 x=>x.a==1</param>
        /// <returns></returns>
        Task<int> UpdateAsync(Dictionary<string, object> updateColumns, Expression<Func<T, object>> where);

        /// <summary>
        /// 无实体更新(批量)
        /// </summary>
        /// <param name="updateColumns">要更新的列</param>
        /// <param name="where">更新条件lambda判断 x=>x.a==1</param>
        /// <returns></returns>
        int Update(List<Dictionary<string, object>> updateColumns, Expression<Func<T, object>> where);

        /// <summary>
        /// 无实体更新(批量)
        /// </summary>
        /// <param name="updateColumns">要更新的列</param>
        /// <param name="where">更新条件lambda判断 x=>x.a==1</param>
        /// <returns></returns>
        Task<int> UpdateAsync(List<Dictionary<string, object>> updateColumns, Expression<Func<T, object>> where);

        #endregion 更新实体数据

        #region 添加或更新

        /// <summary>
        /// 添加或更新
        /// </summary>
        /// <param name="entitys"></param>
        /// <param name="where">条件lambda判断 x=>new {x.Id}存在则修改 不存在则更新</param>
        /// <returns></returns>
        int InsertOrUpdate(T entitys, Expression<Func<T, object>> where);

        /// <summary>
        /// 添加或更新
        /// </summary>
        /// <param name="entitys"></param>
        /// <param name="where">条件lambda判断 x=>new {x.Id}存在则修改 不存在则更新</param>
        /// <returns></returns>
        int InsertOrUpdate(List<T> entitys, Expression<Func<T, object>> where);

        /// <summary>
        /// 添加或更新
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="where">条件lambda判断 x=>new {x.Id}存在则修改 不存在则更新</param>
        /// <returns></returns>
        Task<int> InsertOrUpdateAsync(T entity, Expression<Func<T, object>> where);

        /// <summary>
        /// 添加或更新
        /// </summary>
        /// <param name="entitys"></param>
        /// <param name="where">条件lambda判断 x=>new {x.Id}存在则修改 不存在则更新</param>
        /// <returns></returns>
        Task<int> InsertOrUpdateAsync(List<T> entitys, Expression<Func<T, object>> where);

        /*
                /// <summary>
                /// 添加或更新(带事务)
                /// </summary>
                /// <param name="entity"></param>
                /// <param name="updateColumns">(添加是全量)更新的列x=>new {x.a,x.b}</param>
                /// <param name="where">条件lambda判断 x=>new {x.Id}存在则修改 不存在则更新</param>
                /// <returns></returns>
                int InsertOrUpdate(TResult entity, Expression<Func<TResult, object>> updateColumns, Expression<Func<TResult, object>> where);

                /// <summary>
                /// 添加或更新(带事务)
                /// </summary>
                /// <param name="entitys"></param>
                /// <param name="updateColumns">(添加是全量)更新的列x=>new {x.a,x.b}</param>
                /// <param name="where">条件lambda判断 x=>new {x.Id}存在则修改 不存在则更新</param>
                /// <returns></returns>
                int InsertOrUpdate(List<TResult> entitys, Expression<Func<TResult, object>> updateColumns, Expression<Func<TResult, object>> where);

                /// <summary>
                /// 添加或更新(带事务)
                /// </summary>
                /// <param name="entity"></param>
                /// <param name="updateColumns">(添加是全量)更新的列x=>new {x.a,x.b}</param>
                /// <param name="where">条件lambda判断 x=>new {x.Id}存在则修改 不存在则更新</param>
                /// <returns></returns>
                Task<int> InsertOrUpdateAsync(TResult entity, Expression<Func<TResult, object>> updateColumns, Expression<Func<TResult, object>> where);

                /// <summary>
                /// 添加或更新(带事务)
                /// </summary>
                /// <param name="entitys"></param>
                /// <param name="updateColumns">(添加是全量)更新的列x=>new {x.a,x.b}</param>
                /// <param name="where">条件lambda判断 x=>new {x.Id}存在则修改 不存在则更新</param>
                /// <returns></returns>
                Task<int> InsertOrUpdateAsync(List<TResult> entitys, Expression<Func<TResult, object>> updateColumns, Expression<Func<TResult, object>> where);*/

        #endregion 添加或更新

        #region 删除数据

        #endregion 删除数据

        #region 判断数据是否存在

        /// <summary>
        /// 判断数据是否存在
        /// </summary>
        /// <param name="predicate">条件表达式树</param>
        /// <returns>是否存在</returns>
        bool Exists(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// 异步判断数据是否存在
        /// </summary>
        /// <param name="predicate">条件表达式树</param>
        /// <returns>是否存在</returns>
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);

        #endregion 判断数据是否存在

        #region 获取数据总数

        /// <summary>
        /// 获取数据总数
        /// </summary>
        /// <param name="predicate">条件表达式树</param>
        /// <returns>数据总数</returns>
        int GetCount(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// 异步获取数据总数
        /// </summary>
        /// <param name="predicate">条件表达式树</param>
        /// <returns>数据总数</returns>
        Task<int> GetCountAsync(Expression<Func<T, bool>> predicate);

        #endregion 获取数据总数

        #region 查询分页数据

        /// <summary>
        /// 根据条件查询分页数据
        /// </summary>
        /// <param name="predicate">条件表达式树</param>
        /// <param name="orderBy">排序字段</param>
        /// <param name="pageIndex">当前页面索引</param>
        /// <param name="pageSize">分页大小</param>
        /// <returns>分页数据</returns>
        IPageList<T> QueryPage(Expression<Func<T, bool>> predicate, string orderBy = "", int pageIndex = 1,
            int pageSize = 20);

        /// <summary>
        /// 异步根据条件查询分页数据
        /// </summary>
        /// <param name="predicate">条件表达式树</param>
        /// <param name="orderBy">排序字段</param>
        /// <param name="pageIndex">当前页面索引</param>
        /// <param name="pageSize">分页大小</param>
        /// <returns>分页数据</returns>
        Task<IPageList<T>> QueryPageAsync(Expression<Func<T, bool>> predicate, string orderBy = "", int pageIndex = 1,
            int pageSize = 20);

        /// <summary>
        /// 根据条件查询分页数据
        /// </summary>
        /// <param name="predicate">条件表达式树</param>
        /// <param name="orderByExpression">排序字段</param>
        /// <param name="orderByType">排序方式</param>
        /// <param name="pageIndex">当前页面索引</param>
        /// <param name="pageSize">分页大小</param>
        /// <returns>分页数据</returns>
        IPageList<T> QueryPage(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> orderByExpression,
            OrderByType orderByType, int pageIndex = 1, int pageSize = 20);

        /// <summary>
        /// 异步根据条件查询分页数据
        /// </summary>
        /// <param name="predicate">条件表达式树</param>
        /// <param name="orderByExpression">排序字段</param>
        /// <param name="orderByType">排序方式</param>
        /// <param name="pageIndex">当前页面索引</param>
        /// <param name="pageSize">分页大小</param>
        /// <returns>分页数据</returns>
        Task<IPageList<T>> QueryPageAsync(Expression<Func<T, bool>> predicate,
            Expression<Func<T, object>> orderByExpression, OrderByType orderByType, int pageIndex = 1,
            int pageSize = 20);

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="queryable">查询对象</param>
        /// <param name="pageIndex">当前页面索引</param>
        /// <param name="pagesize">分页大小</param>
        /// <param name="rowcount">总行数</param>
        /// <param name="orderBy">排序字段</param>
        /// <param name="returnRowCount">是否返回总行数</param>
        /// <returns>分页查询对象</returns>
        ISugarQueryable<T> IQueryablePage(ISugarQueryable<T> queryable, int pageIndex, int pagesize, out int rowcount,
            Dictionary<string, QueryOrderBy> orderBy, bool returnRowCount = true);
        /// <summary>
        /// 分页
        /// </summary>
        /// <param name="queryable"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pagesize"></param>
        /// <param name="returnRowCount"></param>
        /// <returns></returns>
        IPageList<T> QueryPage(ISugarQueryable<T> queryable, int pageIndex, int pagesize, bool returnRowCount = true);

        /// <summary>
        /// 分页
        /// </summary>
        /// <param name="queryable"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pagesize"></param>
        /// <param name="returnRowCount"></param>
        /// <returns></returns>
        Task<IPageList<T>> QueryPageAsync(ISugarQueryable<T> queryable, int pageIndex, int pagesize, bool returnRowCount = true);

        #endregion 查询分页数据

        #region 执行SQL语句

        /// <summary>
        /// 执行sql语句并返回List[TResult]
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数</param>
        /// <returns>实体对象列表</returns>
        List<T> SqlQuery(string sql, object? parameters = default);

        /// <summary>
        /// 异步执行sql语句并返回List[TResult]
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数</param>
        /// <returns>实体对象列表</returns>
        Task<List<T>> SqlQueryAsync(string sql, object? parameters = default);

        /// <summary>
        /// 执行sql语句并返回到指定实体中
        /// </summary>
        /// <typeparam name="TResult">映射到这个实体</typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数</param>
        /// <returns>实体对象列表</returns>
        List<TResult> SqlQuery<TResult>(string sql, object? parameters);

        /// <summary>
        /// 异步执行sql语句并返回到指定实体中
        /// </summary>
        /// <typeparam name="TResult">映射到这个实体</typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数</param>
        /// <returns>实体对象列表</returns>
        Task<List<TResult>> SqlQueryAsync<TResult>(string sql, object? parameters);

        /// <summary>
        /// 执行sql语句并返回到指定实体中
        /// </summary>
        /// <typeparam name="TResult">映射到这个实体</typeparam>
        /// <param name="sql">sql</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        TResult SqlQuerySingle<TResult>(string sql, object? parameters);

        /// <summary>
        /// 执行sql语句并返回到指定实体中
        /// </summary>
        /// <typeparam name="TResult">映射到这个实体</typeparam>
        /// <param name="sql">sql</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        Task<TResult> SqlQuerySingleAsync<TResult>(string sql, object? parameters);

        /// <summary>
        /// 执行分页sql语句并返回到指定实体中
        /// </summary>
        /// <typeparam name="TResult">映射到这个实体</typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数</param>
        /// <param name="pageIndex">当前页面索引</param>
        /// <param name="pageSize">分页大小</param>
        /// <returns>分页数据</returns>
        IPageList<TResult> SqlPageQuery<TResult>(string sql, object? parameters, int pageIndex, int pageSize);

        /// <summary>
        /// 异步执行分页sql语句并返回到指定实体中
        /// </summary>
        /// <typeparam name="TResult">映射到这个实体</typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数</param>
        /// <param name="pageIndex">当前页面索引</param>
        /// <param name="pageSize">分页大小</param>
        /// <returns>分页数据</returns>
        Task<IPageList<TResult>>
            SqlPageQueryAsync<TResult>(string sql, object? parameters, int pageIndex, int pageSize);

        /// <summary>
        /// 执行sql语句返回影响行数
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数</param>
        /// <returns>返回影响行数</returns>
        int ExecuteSql(string sql, object? parameters);

        /// <summary>
        /// 异步执行sql语句返回影响行数
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
        /// <param name="func">事务操作</param>
        /// <returns>事务是否成功</returns>
        bool DbContextBeginTransaction(Func<bool> func);

        /// <summary>
        /// 开启事务
        /// </summary>
        /// <param name="func">事务操作</param>
        /// <returns>事务是否成功</returns>
        Task<bool> DbContextBeginTransactionAsync(Func<Task<bool>> func);

        #endregion 开启事务
    }
}