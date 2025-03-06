using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Easy.EF.Core.BiewModels;
using Easy.EF.Core.Common;
using Easy.Common.Core;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System.Linq;

namespace Easy.EF.Core.BaseProvider
{
    /// <summary>
    /// EF通用仓储
    /// </summary>
    public class BaseEFRepository<TDBContext, T> : IBaseEFRepository<TDBContext, T> where TDBContext : DbContext where T : class
    {
        public TDBContext EFContext { get; private set; }

        private readonly TDBContext _context;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="context"></param>
        public BaseEFRepository(TDBContext context)
        {
            _context = context;
            EFContext = _context;
        }

        #region 获取单个实体

        /// <summary>
        /// 获取单个实体
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public virtual T GetSingle(Expression<Func<T, bool>> where)
        {
            return _context.Set<T>().AsNoTracking().First(where);
        }

        /// <summary>
        /// 获取单个实体
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public virtual async Task<T> GetSingleAsync(Expression<Func<T, bool>> where)
        {
            return await _context.Set<T>().AsNoTracking().FirstAsync(where);
        }

        #endregion 获取单个实体

        #region 获取列表

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="whereExpression"></param>
        /// <returns></returns>
        public virtual List<T> GetList(Expression<Func<T, bool>> whereExpression)
        {
            return _context.Set<T>().AsNoTracking().Where(whereExpression).ToList();
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="whereExpression"></param>
        /// <returns></returns>
        public virtual async Task<List<T>> GetListAsync(Expression<Func<T, bool>> whereExpression)
        {
            return await _context.Set<T>().AsNoTracking().Where(whereExpression).ToListAsync();
        }

        /// <summary>
        /// 根据条件查询数据
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        public virtual async Task<List<T>> GetListAsync(Expression<Func<T, bool>> predicate, string orderBy = "")
        {
            var query = _context.Set<T>().AsNoTracking().Where(predicate);
            // if (!string.IsNullOrEmpty(orderBy))
            // {
            //     query = query.OrderBy(orderBy);
            // }
            return await query.ToListAsync();
        }

        /// <summary>
        /// 根据条件查询数据
        /// </summary>
        /// <param name="predicate">条件表达式树</param>
        /// <param name="orderByPredicate">排序字段</param>
        /// <param name="orderByType">排序顺序</param>
        /// <returns>泛型实体集合</returns>
        public virtual async Task<List<T>> GetListAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> orderByPredicate, OrderByType orderByType)
        {
            var query = _context.Set<T>().AsNoTracking().Where(predicate);
            if (orderByType == OrderByType.Asc)
            {
                query = query.OrderBy(orderByPredicate);
            }
            else
            {
                query = query.OrderByDescending(orderByPredicate);
            }
            return await query.ToListAsync();
        }

        #endregion 获取列表

        #region 写入实体数据

        /// <summary>
        /// 写入实体数据
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <param name="isSave">保存更改</param>
        /// <returns></returns>
        public virtual int Insert(T entity, bool isSave = false)
        {
            _context.Set<T>().Add(entity);
            if (isSave)
                return _context.SaveChanges();
            return 0;
        }

        /// <summary>
        /// 写入实体数据
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <param name="isSave">保存更改</param>
        /// <returns></returns>
        public virtual async Task<int> InsertAsync(T entity, bool isSave = false)
        {
            await _context.Set<T>().AddAsync(entity);
            if (isSave)
                return await _context.SaveChangesAsync();
            return 0;
        }

        /// <summary>
        /// 批量写入实体数据
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <param name="isSave">保存更改</param>
        /// <returns></returns>
        public virtual int Insert(List<T> entity, bool isSave = false)
        {
            _context.Set<T>().AddRange(entity);
            if (isSave)
                return _context.SaveChanges();
            return 0;
        }

        /// <summary>
        /// 批量写入实体数据
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <param name="isSave">保存更改</param>
        /// <returns></returns>
        public virtual async Task<int> InsertAsync(List<T> entity, bool isSave = false)
        {
            await _context.Set<T>().AddRangeAsync(entity);
            if (isSave)
                return await _context.SaveChangesAsync();
            return 0;
        }

        private object InsertReturn(T obj)
        {
            var entity = _context.Set<T>().Add(obj);
            _context.SaveChanges();
            return entity.GetPrimaryKey();
        }

        private async Task<object> InsertReturnAsync(T obj)
        {
            var entity = await _context.Set<T>().AddAsync(obj);
            await _context.SaveChangesAsync();
            return entity.GetPrimaryKey();
        }

        public virtual int InsertReturnIdentity(T insertObj)
        {
            return InsertReturn(insertObj).ObjToInt();
        }

        public virtual async Task<int> InsertReturnIdentityAsync(T insertObj)
        {
            return (await InsertReturnAsync(insertObj)).ObjToInt();
        }

        public virtual long InsertReturnBigIdentity(T insertObj)
        {
            return InsertReturn(insertObj).ObjToLong();
        }

        public virtual async Task<long> InsertReturnBigIdentityAsync(T insertObj)
        {
            return (await InsertReturnAsync(insertObj)).ObjToLong();
        }

        #endregion 写入实体数据

        #region 更新实体数据

        /// <summary>
        /// 批量更新实体数据
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="isSave">保存更改</param>
        /// <returns></returns>
        public virtual bool Update(List<T> entity, bool isSave = false)
        {
            _context.Set<T>().UpdateRange(entity);
            if (isSave)
                return _context.SaveChanges() > 0;
            return false;
        }

        /// <summary>
        /// 批量更新实体数据
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="isSave">保存更改</param>
        /// <returns></returns>
        public virtual async Task<bool> UpdateAsync(List<T> entity, bool isSave = false)
        {
            _context.Set<T>().UpdateRange(entity);
            if (isSave)
                return await _context.SaveChangesAsync() > 0;
            return false;
        }

        /// <summary>
        /// 更新实体数据
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="isSave">保存更改</param>
        /// <returns></returns>
        public virtual bool Update(T entity, bool isSave = false)
        {
            _context.Set<T>().Update(entity);
            if (isSave)
                return _context.SaveChanges() > 0;
            return false;
        }

        /// <summary>
        /// 更新实体数据
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="isSave">保存更改</param>
        /// <returns></returns>
        public virtual async Task<bool> UpdateAsync(T entity, bool isSave = false)
        {
            _context.Set<T>().Update(entity);
            if (isSave)
                return await _context.SaveChangesAsync() > 0;
            return false;
        }

        /// <summary>
        /// 更新实体指定列
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="property">更新值，用法：x => new { x.Name, x.CreateTime }</param>
        /// <param name="isSave">是否保存更改</param>
        /// <returns></returns>
        public virtual bool Update(T entity, Expression<Func<T, object>> property, bool isSave = false)
        {
            _context.Set<T>().Attach(entity);
            var properties = GetMemberNames(property);
            foreach (var memberInfo in properties)
            {
                _context.Entry(entity).Property(memberInfo).IsModified = true;
            }
            if (isSave)
                return _context.SaveChanges() > 0;
            return false;
        }

        /// <summary>
        /// 更新实体指定列
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="property">更新值，用法：x => new { x.Name, x.CreateTime }</param>
        /// <param name="isSave">是否保存更改</param>
        /// <returns></returns>
        public virtual async Task<bool> UpdateAsync(T entity, Expression<Func<T, object>> property, bool isSave = false)
        {
            _context.Set<T>().Attach(entity);
            var properties = GetMemberNames(property);
            foreach (var memberInfo in properties)
            {
                _context.Entry(entity).Property(memberInfo).IsModified = true;
            }
            if (isSave)
                return await _context.SaveChangesAsync() > 0;
            return false;
        }

        /// <summary>
        /// 更新实体指定列 TODO:可以用但是很麻烦 而且易错
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="property">更新值，用法：x => new { x.Name, x.CreateTime }</param>
        /// <param name="whereColumns">根据列值条件更新，用法：x => new { x.Name }</param>
        /// <param name="isSave">是否保存更改</param>
        /// <returns></returns>
        public virtual bool Update(T entity, Expression<Func<T, object>> property, Expression<Func<T, object>> whereColumns, bool isSave = false)
        {
            var properties = GetMemberNames(property);
            var whereProperties = GetMemberNames(whereColumns);
            /*    ctx.Users.Where(x => x.LastLoginDate < DateTime.Now.AddYears(-2))
             .Update(x => new User() { IsSoftDeleted = 1 });*/
            if (!properties.Any() || !whereProperties.Any())
            {
                throw new ArgumentException("必须指定更新字段和条件字段");
            }
            return true;
        }

        /*
                /// <summary>
                /// 更新实体指定列 TODO:可以用但是很麻烦 而且易错
                /// </summary>
                /// <param name="entity">实体</param>
                /// <param name="property">更新值，用法：x => new { x.Name, x.CreateTime }</param>
                /// <param name="whereColumns">根据列值条件更新，用法：x => new { x.Name }</param>
                /// <param name="isSave">是否保存更改</param>
                /// <returns></returns>
                public virtual bool Update(T entity, Expression<Func<T, object>> property, Expression<Func<T, object>> whereColumns, bool isSave = false)
                {
                    var properties = GetMemberNames(property);
                    var whereProperties = GetMemberNames(whereColumns);

                    if (!properties.Any() || !whereProperties.Any())
                    {
                        throw new ArgumentException("必须指定更新字段和条件字段");
                    }

                    // 获取表名
                    string tableName = _context.Model.FindEntityType(typeof(T)).GetTableName();

                    // 构造 SQL 语句，使用 `?` 作为参数占位符（EF Core 自动转换为 @p0, @p1, ...）
                    string setClause = string.Join(", ", properties.Select(_ => "?"));
                    string whereClause = string.Join(" AND ", whereProperties.Select(_ => "?"));

                    string sql = $"UPDATE {tableName} SET {string.Join(", ", properties.Select(p => $"{p} = ?"))} WHERE {string.Join(" AND ", whereProperties.Select(p => $"{p} = ?"))}";

                    // 构造参数数组（顺序必须匹配 SQL 语句）
                    var sqlParams = properties
                        .Select(p => entity.GetType().GetProperty(p).GetValue(entity) ?? DBNull.Value)
                        .Concat(whereProperties.Select(p => entity.GetType().GetProperty(p).GetValue(entity) ?? DBNull.Value))
                        .ToArray();

                    // 执行 SQL
                    int affectedRows = _context.Database.ExecuteSqlRaw(sql, sqlParams);
                    return affectedRows > 0;
                }
        */


        private List<string> GetMemberNames(Expression<Func<T, object>> expression)
        {
            var memberNames = new List<string>();
            if (expression.Body is NewExpression newExpression)
            {
                memberNames.AddRange(newExpression.Members.Select(m => m.Name));
            }
            else if (expression.Body is MemberInitExpression memberInitExpression)
            {
                memberNames.AddRange(memberInitExpression.Bindings.Select(b => b.Member.Name));
            }
            else if (expression.Body is UnaryExpression unaryExpression && unaryExpression.Operand is MemberExpression memberExpression)
            {
                memberNames.Add(memberExpression.Member.Name);
            }
            else if (expression.Body is MemberExpression member)
            {
                memberNames.Add(member.Member.Name);
            }
            return memberNames;
        }



        #endregion 更新实体数据

        #region 删除数据


        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <param name="isSave">保存更改</param>
        /// <returns></returns>
        public virtual bool Delete(T entity, bool isSave = false)
        {
            _context.Set<T>().Remove(entity);
            if (isSave)
                return _context.SaveChanges() > 0;
            return false;
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <param name="isSave">保存更改</param>
        /// <returns></returns>
        public virtual async Task<bool> DeleteAsync(T entity, bool isSave = false)
        {
            _context.Set<T>().Remove(entity);
            if (isSave)
                return await _context.SaveChangesAsync() > 0;
            return false;
        }

        #endregion 删除数据

        #region 判断数据是否存在

        /// <summary>
        /// 判断数据是否存在
        /// </summary>
        /// <param name="predicate">条件表达式树</param>
        /// <returns></returns>
        public virtual bool Exists(Expression<Func<T, bool>> predicate)
        {
            return _context.Set<T>().Any(predicate);
        }

        /// <summary>
        /// 判断数据是否存在
        /// </summary>
        /// <param name="predicate">条件表达式树</param>
        /// <returns></returns>
        public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().AnyAsync(predicate);
        }

        #endregion 判断数据是否存在

        #region 获取数据总数

        /// <summary>
        /// 获取数据总数
        /// </summary>
        /// <param name="predicate">条件表达式树</param>
        /// <returns></returns>
        public virtual int GetCount(Expression<Func<T, bool>> predicate)
        {
            return _context.Set<T>().Count(predicate);
        }

        /// <summary>
        /// 获取数据总数
        /// </summary>
        /// <param name="predicate">条件表达式树</param>
        /// <returns></returns>
        public virtual async Task<int> GetCountAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().CountAsync(predicate);
        }

        #endregion 获取数据总数

        #region 查询分页数据

        /// <summary>
        /// 计算页码
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        private int GetPageIndex(int pageIndex)
        {
            return pageIndex < 1 ? 1 : pageIndex;
        }

        /// <summary>
        /// 根据条件查询分页数据
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="orderBy"></param>
        /// <param name="pageIndex">当前页面索引</param>
        /// <param name="pageSize">分布大小</param>
        /// <returns></returns>
        public virtual IPageList<T> QueryPage(Expression<Func<T, bool>> predicate, string orderBy = "", int pageIndex = 1, int pageSize = 20)
        {
            var query = _context.Set<T>().AsNoTracking().Where(predicate);
            // if (!string.IsNullOrEmpty(orderBy))
            // {
            //     query = query.OrderBy(orderBy);
            // }
            var total = query.Count();
            var data = query.Skip((GetPageIndex(pageIndex) - 1) * pageSize).Take(pageSize).ToList();
            return new PageList<T>(data, pageIndex, pageSize, total);
        }

        /// <summary>
        /// 根据条件查询分页数据
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="orderBy"></param>
        /// <param name="pageIndex">当前页面索引</param>
        /// <param name="pageSize">分布大小</param>
        /// <returns></returns>
        public virtual async Task<IPageList<T>> QueryPageAsync(Expression<Func<T, bool>> predicate, string orderBy = "", int pageIndex = 1, int pageSize = 20)
        {
            var query = _context.Set<T>().AsNoTracking().Where(predicate);
            // if (!string.IsNullOrEmpty(orderBy))
            // {
            //     query = query.OrderBy(orderBy);
            // }
            var total = await query.CountAsync();
            var data = await query.Skip((GetPageIndex(pageIndex) - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PageList<T>(data, pageIndex, pageSize, total);
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
        public virtual IPageList<T> QueryPage(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> orderByExpression, OrderByType orderByType, int pageIndex = 1, int pageSize = 20)
        {
            var query = _context.Set<T>().AsNoTracking().Where(predicate);
            if (orderByType == OrderByType.Asc)
            {
                query = query.OrderBy(orderByExpression);
            }
            else
            {
                query = query.OrderByDescending(orderByExpression);
            }
            var total = query.Count();
            var data = query.Skip((GetPageIndex(pageIndex) - 1) * pageSize).Take(pageSize).ToList();
            return new PageList<T>(data, pageIndex, pageSize, total);
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
        public virtual async Task<IPageList<T>> QueryPageAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> orderByExpression, OrderByType orderByType, int pageIndex = 1, int pageSize = 20)
        {
            var query = _context.Set<T>().AsNoTracking().Where(predicate);
            if (orderByType == OrderByType.Asc)
            {
                query = query.OrderBy(orderByExpression);
            }
            else
            {
                query = query.OrderByDescending(orderByExpression);
            }
            var total = await query.CountAsync();
            var data = await query.Skip((GetPageIndex(pageIndex) - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PageList<T>(data, pageIndex, pageSize, total);
        }

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
        public virtual IQueryable<T> IQueryablePage(IQueryable<T> queryable, int pageIndex, int pagesize, out int rowcount, Dictionary<string, QueryOrderBy> orderBy, bool returnRowCount = true)
        {
            rowcount = 0;
            if (returnRowCount)
            {
                rowcount = queryable.Count();
            }
            // if (orderBy != null && orderBy.Count > 0)
            // {
            //     queryable = queryable.OrderBy(orderBy);
            // }
            return queryable.AsNoTracking().Skip((pageIndex - 1) * pagesize).Take(pagesize);
        }

        #endregion 查询分页数据

        #region 执行SQL语句

        /// <summary>
        /// 执行sql语句并返回List[T]
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual List<T> SqlQuery(string sql, object? parameters = null)
        {
            if (parameters == null)
                return _context.Set<T>().FromSqlRaw(sql).ToList();
            return _context.Set<T>().FromSqlRaw(sql, parameters).ToList();
        }

        /// <summary>
        /// 执行sql语句并返回List
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual async Task<List<T>> SqlQueryAsync(string sql, object? parameters = null)
        {
            if (parameters == null)
                return await _context.Set<T>().FromSqlRaw(sql).ToListAsync();
            return await _context.Set<T>().FromSqlRaw(sql, parameters).ToListAsync();
        }

        /*
                /// <summary>
                /// 执行sql语句并返回到指定实体中
                /// </summary>
                /// <typeparam name="TEntity">映射到这个实体</typeparam>
                /// <param name="sql">sql</param>
                /// <param name="parameters">参数</param>
                /// <returns></returns>
                List<TResult> IBaseRepository<TDBContext, T>.SqlQuery<TResult>(string sql, object[]? parameters) whereExpression TResult : class
                {
        #if NET6_0
                    if (parameters == null)
                        return _context.Set<TResult>().FromSqlRaw(sql).ToList();
                    return _context.Set<TResult>().FromSqlRaw(sql, parameters).ToList();
        #else
                    if (parameters == null)
                        return _context.Database.SqlQueryRaw<TResult>(sql).ToList();
                    return _context.Database.SqlQueryRaw<TResult>(sql, parameters).ToList();
        #endif
                }

                /// <summary>
                /// 执行sql语句并返回到指定实体中
                /// </summary>
                /// <typeparam name="TEntity">映射到这个实体</typeparam>
                /// <param name="sql">sql</param>
                /// <param name="parameters">参数</param>
                /// <returns></returns>
                async Task<List<TResult>> IBaseRepository<TDBContext, T>.SqlQueryAsync<TResult>(string sql, object[]? parameters) whereExpression TResult : class
                {
        #if NET6_0
                    if (parameters == null)
                        return await _context.Set<TResult>().FromSqlRaw(sql).ToListAsync();
                    return await _context.Set<TResult>().FromSqlRaw(sql, parameters).ToListAsync();
        #else
                    if (parameters == null)
                        return await _context.Database.SqlQueryRaw<TResult>(sql).ToListAsync();
                        return await _context.Database.SqlQueryRaw<TResult>(sql, parameters).ToListAsync();
        #endif
                }*/

        /// <summary>
        /// 执行分页sql语句并返回到指定实体中
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public virtual IPageList<TEntity> SqlPageQuery<TEntity>(string sql, object? parameters, int pageIndex, int pageSize) where TEntity : class, new()
        {
            IQueryable<TEntity>? query = null;
            if (parameters == null)
                query = _context.Set<TEntity>().FromSqlRaw(sql);
            else
                query = _context.Set<TEntity>().FromSqlRaw(sql, parameters);
            var total = query.Count();
            var data = query.Skip((GetPageIndex(pageIndex) - 1) * pageSize).Take(pageSize).ToList();
            return new PageList<TEntity>(data, pageIndex, pageSize, total);
        }

        /// <summary>
        /// 执行分页sql语句并返回到指定实体中
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public virtual async Task<IPageList<TEntity>> SqlPageQueryAsync<TEntity>(string sql, object? parameters, int pageIndex, int pageSize) where TEntity : class, new()
        {
            IQueryable<TEntity>? query = null;
            if (parameters == null)
                query = _context.Set<TEntity>().FromSqlRaw(sql);
            else
                query = _context.Set<TEntity>().FromSqlRaw(sql, parameters);
            var total = await query.CountAsync();
            var data = await query.Skip((GetPageIndex(pageIndex) - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PageList<TEntity>(data, pageIndex, pageSize, total);
        }

        /// <summary>
        /// 执行sql语句返回影响行数
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数</param>
        /// <returns>返回影响行数</returns>
        public virtual int ExecuteSql(string sql, object? parameters)
        {
            if (parameters == null)
                return _context.Database.ExecuteSqlRaw(sql);
            return _context.Database.ExecuteSqlRaw(sql, parameters);
        }

        /// <summary>
        /// 执行sql语句返回影响行数
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数</param>
        /// <returns>返回影响行数</returns>
        public virtual async Task<int> ExecuteSqlAsync(string sql, object? parameters)
        {
            if (parameters == null)
                return await _context.Database.ExecuteSqlRawAsync(sql);
            return await _context.Database.ExecuteSqlRawAsync(sql, parameters);
        }

        #endregion 执行SQL语句

        #region 开启事务

        /// <summary>
        /// 开启事务
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public virtual bool DbContextBeginTransaction(Func<bool> func)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var result = func();
                    if (result)
                    {
                        transaction.Commit();
                        return true;
                    }
                    transaction.Rollback();
                    return false;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine($"执行事务发生错误：{ex}");
                    return false;
                }
            }
        }

        #endregion 开启事务

        /// <summary>
        /// 保存更改
        /// </summary>
        /// <returns></returns>
        public virtual int SaveChanges() => _context.SaveChanges();

        /// <summary>
        /// 保存更改
        /// </summary>
        /// <returns></returns>
        public virtual async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}