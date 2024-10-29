using System.Linq.Expressions;
using System.Reflection;
using Serilog;
using SqlSugar;
using SqlSugar.IOC;
using WBC66.SqlSugar.Core.BiewModels;

namespace WBC66.SqlSugar.Core
{
    /// <summary>
    /// SqlSugar通用仓储
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseRepository<T> where T : class, new()
    {
        #region 数据库连接对象

        /// <summary>
        /// 数据库连接对象
        /// </summary>
        public ISqlSugarClient DbBase
        {
            get
            {
                var configId = typeof(T).GetCustomAttribute<TenantAttribute>()?.configId;
                return DbScoped.SugarScope.GetConnection(configId);
            }
        }

        /// <summary>
        /// db上下文
        /// </summary>
        public ISqlSugarClient DbBaseClient => DbBase;

        #endregion 数据库连接对象

        #region 构造函数

        /// <summary>
        /// 初始化构造方法
        /// </summary>
        protected BaseRepository()
        {
            //sql执行前
            DbBaseClient.Aop.OnLogExecuting = (sql, pars) =>
            {
                //打印sql
                SqlSugarAop.OnLogExecuting(DbBaseClient, sql, pars);
            };
            //sql执行完成
            DbBaseClient.Aop.OnLogExecuted = (sql, pars) =>
            {
                //打印sql
                SqlSugarAop.OnLogExecuted(DbBaseClient, sql, pars);
            };
            //sql执行错误
            DbBaseClient.Aop.OnError = SqlSugarAop.OnError;
        }

        #endregion 构造函数

        #region 获取单个实体

        /// <summary>
        /// 获取单个实体
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public T GetSingle(Expression<Func<T, bool>> where)
        {
            return DbBaseClient.Queryable<T>().First(where);
        }

        /// <summary>
        /// 获取单个实体
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public async Task<T> GetSingleAsync(Expression<Func<T, bool>> where)
        {
            return await DbBaseClient.Queryable<T>().FirstAsync(where);
        }

        #endregion 获取单个实体

        #region 获取列表

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public List<T> GetList(Expression<Func<T, bool>> where)
        {
            return DbBaseClient.Queryable<T>().Where(where).ToList();
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public async Task<List<T>> GetListAsync(Expression<Func<T, bool>> where)
        {
            return await DbBaseClient.Queryable<T>().Where(where).ToListAsync();
        }

        /// <summary>
        /// 根据条件查询数据
        /// </summary>
        /// <param name="predicate">条件表达式树</param>
        /// <param name="orderBy">排序字段，如name asc,age desc</param>
        /// <returns>泛型实体集合</returns>
        public async Task<List<T>> GetListAsync(Expression<Func<T, bool>> predicate, string orderBy = "")
        {
            return await DbBaseClient.Queryable<T>().OrderByIF(!string.IsNullOrEmpty(orderBy), orderBy)
                    .WhereIF(predicate != null, predicate).ToListAsync();
        }

        /// <summary>
        /// 根据条件查询数据
        /// </summary>
        /// <param name="predicate">条件表达式树</param>
        /// <param name="orderByPredicate">排序字段</param>
        /// <param name="orderByType">排序顺序</param>
        /// <returns>泛型实体集合</returns>
        public async Task<List<T>> GetListAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> orderByPredicate, OrderByType orderByType)
        {
            return await DbBaseClient.Queryable<T>().OrderByIF(orderByPredicate != null, orderByPredicate, orderByType)
                    .WhereIF(predicate != null, predicate).ToListAsync();
        }

        #endregion 获取列表

        #region 写入实体数据

        /// <summary>
        /// 写入实体数据
        /// </summary>
        /// <param name="entity">实体数据</param>
        /// <returns></returns>
        public int Insert(T entity)
        {
            return DbBaseClient.Insertable(entity).ExecuteReturnIdentity();
        }

        /// <summary>
        /// 写入实体数据
        /// </summary>
        /// <param name="entity">实体数据</param>
        /// <returns></returns>
        public async Task<int> InsertAsync(T entity)
        {
            return await DbBaseClient.Insertable(entity).ExecuteReturnIdentityAsync();
        }

        /// <summary>
        /// 写入实体数据
        /// </summary>
        /// <param name="entity">实体数据</param>
        /// <param name="insertColumns">插入的列</param>
        /// <returns></returns>
        public int Insert(T entity, Expression<Func<T, object>>? insertColumns = null)
        {
            var insert = DbBaseClient.Insertable(entity);
            if (insertColumns == null)
                return insert.ExecuteReturnIdentity();
            return insert.InsertColumns(insertColumns).ExecuteReturnIdentity();
        }

        /// <summary>
        /// 写入实体数据
        /// </summary>
        /// <param name="entity">实体数据</param>
        /// <param name="insertColumns">插入的列</param>
        /// <returns></returns>
        public async Task<int> InsertAsync(T entity, Expression<Func<T, object>>? insertColumns = null)
        {
            var insert = DbBaseClient.Insertable(entity);
            if (insertColumns == null)
                return await insert.ExecuteReturnIdentityAsync();
            return await insert.InsertColumns(insertColumns).ExecuteReturnIdentityAsync();
        }

        /// <summary>
        /// 批量写入实体数据
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <returns></returns>
        public int Insert(List<T> entity)
        {
            return DbBaseClient.Insertable(entity.ToArray()).ExecuteReturnIdentity();
        }

        /// <summary>
        /// 批量写入实体数据
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <returns></returns>
        public async Task<int> InsertAsync(List<T> entity)
        {
            return await DbBaseClient.Insertable(entity.ToArray()).ExecuteCommandAsync();
        }

        #endregion 写入实体数据

        #region 更新实体数据

        /// <summary>
        /// 批量更新实体数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Update(List<T> entity)
        {
            return DbBaseClient.Updateable(entity).ExecuteCommandHasChange();
        }

        /// <summary>
        /// 批量更新实体数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> UpdateAsync(List<T> entity)
        {
            return await DbBaseClient.Updateable(entity).ExecuteCommandHasChangeAsync();
        }

        /// <summary>
        /// 更新实体数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Update(T entity)
        {
            return DbBaseClient.Updateable(entity).ExecuteCommandHasChange();
        }

        /// <summary>
        /// 更新实体数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> UpdateAsync(T entity)
        {
            return await DbBaseClient.Updateable(entity).ExecuteCommandHasChangeAsync();
        }

        /// <summary>
        /// 更新某个字段
        /// </summary>
        /// <param name="columns">lamdba表达式,如it =&gt; new Student() { Name = "a", CreateTime = DateTime.Now }</param>
        /// <param name="where">lamdba判断</param>
        /// <returns></returns>
        public bool Update(Expression<Func<T, T>> columns, Expression<Func<T, bool>> where)
        {
            var i = DbBaseClient.Updateable<T>().SetColumns(columns).Where(where).ExecuteCommand();
            return i > 0;
        }

        /// <summary>
        /// 更新某个字段
        /// </summary>
        /// <param name="columns">lamdba表达式,如it =&gt; new Student() { Name = "a", CreateTime = DateTime.Now }</param>
        /// <param name="where">lamdba判断</param>
        /// <returns></returns>
        public async Task<bool> UpdateAsync(Expression<Func<T, T>> columns, Expression<Func<T, bool>> where)
        {
            return await DbBaseClient.Updateable<T>().SetColumns(columns).Where(where).ExecuteCommandHasChangeAsync();
        }

        /// <summary>
        /// 根据条件更新
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="lstColumns"></param>
        /// <param name="lstIgnoreColumns"></param>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        public async Task<bool> UpdateAsync(T entity, List<string>? lstColumns = null,
            List<string>? lstIgnoreColumns = null, string strWhere = "")
        {
            var up = DbBaseClient.Updateable(entity);
            if (lstIgnoreColumns?.Count > 0)
                up = up.IgnoreColumns(lstIgnoreColumns.ToArray());
            if (lstColumns?.Count > 0) up = up.UpdateColumns(lstColumns.ToArray());
            if (!string.IsNullOrEmpty(strWhere)) up = up.Where(strWhere);
            return await up.ExecuteCommandHasChangeAsync();
        }

        /// <summary>
        /// 根据条件更新
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="lstColumns"></param>
        /// <param name="lstIgnoreColumns"></param>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        public bool Update(T entity, List<string>? lstColumns = null, List<string>? lstIgnoreColumns = null,
            string strWhere = "")
        {
            var up = DbBaseClient.Updateable(entity);
            if (lstIgnoreColumns?.Count > 0)
                up = up.IgnoreColumns(lstIgnoreColumns.ToArray());
            if (lstColumns?.Count > 0) up = up.UpdateColumns(lstColumns.ToArray());
            if (!string.IsNullOrEmpty(strWhere)) up = up.Where(strWhere);
            return up.ExecuteCommandHasChange();
        }

        #endregion 更新实体数据

        #region 删除数据

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <returns></returns>
        public bool Delete(T entity)
        {
            return DbBaseClient.Deleteable(entity).ExecuteCommandHasChange();
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <returns></returns>
        public async Task<bool> DeleteAsync(T entity)
        {
            return await DbBaseClient.Deleteable(entity).ExecuteCommandHasChangeAsync();
        }

        /// <summary>
        /// 删除数据(批量)
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <returns></returns>
        public bool Delete(List<T> entity)
        {
            return DbBaseClient.Deleteable<T>(entity).ExecuteCommandHasChange();
        }

        /// <summary>
        /// 删除数据(批量)
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <returns></returns>
        public async Task<bool> DeleteAsync(List<T> entity)
        {
            return await DbBaseClient.Deleteable<T>(entity).ExecuteCommandHasChangeAsync();
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public async Task<bool> DeleteAsync(Expression<Func<T, bool>> where)
        {
            return await DbBaseClient.Deleteable<T>().Where(where).ExecuteCommandHasChangeAsync();
        }

        /// <summary>
        /// 根据主键标识批量删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public bool DeleteByIds(object[] ids)
        {
            return DbBaseClient.Deleteable<T>().In(ids).ExecuteCommandHasChange();
        }

        /// <summary>
        /// 根据主键标识批量删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task<bool> DeleteByIdsAsync(object[] ids)
        {
            return await DbBaseClient.Deleteable<T>().In(ids).ExecuteCommandHasChangeAsync();
        }

        #endregion 删除数据

        #region 判断数据是否存在

        /// <summary>
        /// 判断数据是否存在
        /// </summary>
        /// <param name="predicate">条件表达式树</param>
        /// <returns></returns>
        public bool Exists(Expression<Func<T, bool>> predicate)
        {
            return DbBaseClient.Queryable<T>().Where(predicate).Any();
        }

        /// <summary>
        /// 判断数据是否存在
        /// </summary>
        /// <param name="predicate">条件表达式树</param>
        /// <returns></returns>
        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            return await DbBaseClient.Queryable<T>().Where(predicate).AnyAsync();
        }

        #endregion 判断数据是否存在

        #region 获取数据总数

        /// <summary>
        /// 获取数据总数
        /// </summary>
        /// <param name="predicate">条件表达式树</param>
        /// <returns></returns>
        public int GetCount(Expression<Func<T, bool>> predicate)
        {
            return DbBaseClient.Queryable<T>().Count(predicate);
        }

        /// <summary>
        /// 获取数据总数
        /// </summary>
        /// <param name="predicate">条件表达式树</param>
        /// <returns></returns>
        public async Task<int> GetCountAsync(Expression<Func<T, bool>> predicate)
        {
            return await DbBaseClient.Queryable<T>().CountAsync(predicate);
        }

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
        public IPageList<T> QueryPage(Expression<Func<T, bool>> predicate, string orderBy = "", int pageIndex = 1,
            int pageSize = 20)
        {
            var totalCount = 0;
            var page = DbBaseClient.Queryable<T>().OrderByIF(!string.IsNullOrEmpty(orderBy), orderBy)
                    .WhereIF(predicate != null, predicate).ToPageList(pageIndex, pageSize, ref totalCount);
            var list = new PageList<T>(page, pageIndex, pageSize, totalCount);
            return list;
        }

        /// <summary>
        /// 根据条件查询分页数据
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="orderBy"></param>
        /// <param name="pageIndex">当前页面索引</param>
        /// <param name="pageSize">分布大小</param>
        /// <returns></returns>
        public async Task<IPageList<T>> QueryPageAsync(Expression<Func<T, bool>> predicate, string orderBy = "",
            int pageIndex = 1, int pageSize = 20)
        {
            RefAsync<int> totalCount = 0;
            var page = await DbBaseClient.Queryable<T>().OrderByIF(!string.IsNullOrEmpty(orderBy), orderBy)
                    .WhereIF(predicate != null, predicate).ToPageListAsync(pageIndex, pageSize, totalCount);
            var list = new PageList<T>(page, pageIndex, pageSize, totalCount);
            return list;
        }

        /// <summary>
        /// 根据条件查询分页数据
        /// </summary>
        /// <param name="predicate">判断集合</param>
        /// <param name="orderByType">排序方式</param>
        /// <param name="pageIndex">当前页面索引</param>
        /// <param name="pageSize">分布大小</param>
        /// <param name="orderByExpression"></param>
        /// <returns></returns>
        public IPageList<T> QueryPage(Expression<Func<T, bool>> predicate,
            Expression<Func<T, object>> orderByExpression, OrderByType orderByType, int pageIndex = 1,
            int pageSize = 20)
        {
            var totalCount = 0;
            var page = DbBaseClient.Queryable<T>().OrderByIF(orderByExpression != null, orderByExpression, orderByType)
                    .WhereIF(predicate != null, predicate).ToPageList(pageIndex, pageSize, ref totalCount);
            var list = new PageList<T>(page, pageIndex, pageSize, totalCount);
            return list;
        }

        /// <summary>
        /// 根据条件查询分页数据
        /// </summary>
        /// <param name="predicate">判断集合</param>
        /// <param name="orderByType">排序方式</param>
        /// <param name="pageIndex">当前页面索引</param>
        /// <param name="pageSize">分布大小</param>
        /// <param name="orderByExpression"></param>
        /// <returns></returns>
        public async Task<IPageList<T>> QueryPageAsync(Expression<Func<T, bool>> predicate,
            Expression<Func<T, object>> orderByExpression, OrderByType orderByType, int pageIndex = 1,
            int pageSize = 20)
        {
            RefAsync<int> totalCount = 0;
            var page = await DbBaseClient.Queryable<T>().WhereIF(predicate != null, predicate).OrderByIF(orderByExpression != null, orderByExpression, orderByType)
                    .ToPageListAsync(pageIndex, pageSize, totalCount);
            var list = new PageList<T>(page, pageIndex, pageSize, totalCount);
            return list;
        }

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
        public virtual ISugarQueryable<T> IQueryablePage(ISugarQueryable<T> queryable, int pageIndex, int pagesize, out int rowcount, Dictionary<string, QueryOrderBy> orderBy, bool returnRowCount = true)
        {
            pageIndex = pageIndex <= 0 ? 1 : pageIndex;
            pagesize = pagesize <= 0 ? 10 : pagesize;
            rowcount = returnRowCount ? queryable.Count() : 0;
            return queryable.GetISugarQueryableOrderBy<T>(orderBy)
                .Skip((pageIndex - 1) * pagesize)
                .Take(pagesize);
        }

        #endregion 根据条件查询分页数据

        #region 执行sql语句

        /// <summary>
        /// 执行sql语句并返回List[T]
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public List<T> SqlQuery(string sql, object? parameters)
        {
            return DbBaseClient.Ado.SqlQuery<T>(sql, parameters);
        }

        /// <summary>
        /// 执行sql语句并返回List[T]
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<List<T>> SqlQueryAsync(string sql, object? parameters)
        {
            return await DbBaseClient.Ado.SqlQueryAsync<T>(sql, parameters);
        }

        /// <summary>
        /// 执行sql语句并返回到指定实体中
        /// </summary>
        /// <typeparam name="T">映射到这个实体</typeparam>
        /// <param name="sql">sql</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public List<T> SqlQuery<T>(string sql, object? parameters)
        {
            return DbBaseClient.Ado.SqlQuery<T>(sql, parameters);
        }

        /// <summary>
        /// 执行sql语句并返回到指定实体中
        /// </summary>
        /// <typeparam name="T">映射到这个实体</typeparam>
        /// <param name="sql">sql</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public async Task<List<T>> SqlQueryAsync<T>(string sql, object? parameters)
        {
            return await DbBaseClient.Ado.SqlQueryAsync<T>(sql, parameters);
        }

        /// <summary>
        /// 执行分页sql语句并返回到指定实体中
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IPageList<T1> SqlPageQuery<T1>(string sql, object? parameters, int pageIndex, int pageSize)
        {
            //计算分页
            var skip = (pageIndex - 1) * pageSize;
            var take = pageSize;
            var list = DbBaseClient.Ado.SqlQuery<T1>(sql, parameters);
            var total = list.Count;
            return new PageList<T1>(list.Skip(skip).Take(take).ToList(), pageIndex, pageSize, total);
        }

        /// <summary>
        /// 执行分页sql语句并返回到指定实体中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<IPageList<T>> SqlPageQueryAsync<T>(string sql, object? parameters, int pageIndex, int pageSize)
        {
            //计算分页
            var skip = (pageIndex - 1) * pageSize;
            var take = pageSize;
            var list = await DbBaseClient.Ado.SqlQueryAsync<T>(sql, parameters);
            var total = list.Count;
            return new PageList<T>(list.Skip(skip).Take(take).ToList(), pageIndex, pageSize, total);
        }

        /// <summary>
        /// 执行sql语句返回影响行数
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数</param>
        /// <returns>返回影响行数</returns>
        public int ExecuteSql(string sql, object? parameters)
        {
            return DbBaseClient.Ado.ExecuteCommand(sql, parameters);
        }

        /// <summary>
        /// 执行sql语句返回影响行数
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数</param>
        /// <returns>返回影响行数</returns>
        public async Task<int> ExecuteSqlAsync(string sql, object? parameters)
        {
            return await DbBaseClient.Ado.ExecuteCommandAsync(sql, parameters);
        }

        #endregion 执行sql语句

        /// <summary>
        /// 事务
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public virtual bool DbContextBeginTransaction(Func<bool> func)
        {
            var result = new bool();
            try
            {
                DbBaseClient.Ado.BeginTran();
                result = func();
                if (result)
                {
                    DbBaseClient.Ado.CommitTran();
                }
                else
                {
                    DbBaseClient.Ado.RollbackTran();
                    result = false;
                }
            }
            catch (Exception ex)
            {
                DbBaseClient.Ado.RollbackTran();
                result = false;
                Log.Error(ex.Message, ex);
            }
            return result;
        }
    }
}