using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using WBC66.EF.Core.BiewModels;

namespace WBC66.EF.Core.BaseProvider
{
    /// <summary>
    /// EF通用仓储
    /// </summary>
    public class BaseRepository<TDBContext, T> : IBaseRepository<TDBContext, T> where TDBContext : DbContext where T : class
    {
        private readonly TDBContext _context;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="context"></param>
        public BaseRepository(TDBContext context)
        {
            _context = context;
        }

        #region 获取单个实体

        /// <summary>
        /// 获取单个实体
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public T GetSingle(Expression<Func<T, bool>> where)
        {
            return _context.Set<T>().AsNoTracking().FirstOrDefault(where);
        }

        /// <summary>
        /// 获取单个实体
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public async Task<T> GetSingleAsync(Expression<Func<T, bool>> where)
        {
            return await _context.Set<T>().AsNoTracking().FirstOrDefaultAsync(where);
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
            return _context.Set<T>().AsNoTracking().Where(where).ToList();
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public async Task<List<T>> GetListAsync(Expression<Func<T, bool>> where)
        {
            return await _context.Set<T>().AsNoTracking().Where(where).ToListAsync();
        }

        /// <summary>
        /// 根据条件查询数据
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        public async Task<List<T>> GetListAsync(Expression<Func<T, bool>> predicate, string orderBy = "")
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
        public async Task<List<T>> GetListAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> orderByPredicate, OrderByType orderByType)
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
        /// <returns></returns>
        public int Insert(T entity)
        {
            _context.Set<T>().Add(entity);
            return _context.SaveChanges();
        }

        /// <summary>
        /// 写入实体数据
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <returns></returns>
        public async Task<int> InsertAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            return await _context.SaveChangesAsync();
        }

        /// <summary>
        /// 写入实体数据
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <param name="insertColumns">添加列</param>
        /// <returns></returns>
        public int Insert(T entity, Expression<Func<T, object>>? insertColumns = default)
        {
            if (insertColumns == null)
            {
                _context.Set<T>().Add(entity);
            }
            else
            {
                _context.Set<T>().Add(entity);
                var memberExpression = insertColumns.Body as MemberExpression;
                if (memberExpression == null)
                {
                    throw new ArgumentException("The expression should represent a simple property or field access: 't => t.MyProperty'.", nameof(insertColumns));
                }
                _context.Entry(entity).Property(memberExpression.Member.Name).IsModified = true;
            }
            return _context.SaveChanges();
        }

        /// <summary>
        /// 写入实体数据
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <param name="insertColumns">添加列</param>
        /// <returns></returns>
        public async Task<int> InsertAsync(T entity, Expression<Func<T, object>>? insertColumns = default)
        {
            if (insertColumns == null)
            {
                await _context.Set<T>().AddAsync(entity);
            }
            else
            {
                await _context.Set<T>().AddAsync(entity);
                _context.Entry(entity).Property(insertColumns).IsModified = true;
            }
            return await _context.SaveChangesAsync();
        }

        /// <summary>
        /// 批量写入实体数据
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <returns></returns>
        public int Insert(List<T> entity)
        {
            _context.Set<T>().AddRange(entity);
            return _context.SaveChanges();
        }

        /// <summary>
        /// 批量写入实体数据
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <returns></returns>
        public async Task<int> InsertAsync(List<T> entity)
        {
            await _context.Set<T>().AddRangeAsync(entity);
            return await _context.SaveChangesAsync();
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
            _context.Set<T>().UpdateRange(entity);
            return _context.SaveChanges() > 0;
        }

        /// <summary>
        /// 批量更新实体数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> UpdateAsync(List<T> entity)
        {
            _context.Set<T>().UpdateRange(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 更新实体数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Update(T entity)
        {
            _context.Set<T>().Update(entity);
            return _context.SaveChanges() > 0;
        }

        /// <summary>
        /// 更新实体数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> UpdateAsync(T entity)
        {
            _context.Set<T>().Update(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 更新某个字段
        /// </summary>
        /// <param name="entities">实体</param>
        /// <param name="properties">更新字段</param>
        /// <returns></returns>
        public bool Update<TSource>(IEnumerable<TSource> entities, string[] properties) where TSource : class
        {
            if (properties == null || properties.Length == 0)
                return false;
            foreach (var entity in entities)
            {
                _context.Set<TSource>().Attach(entity);
                var entry = _context.Entry(entity);
                entry.State = EntityState.Unchanged;
                foreach (var property in entry.Properties)
                {
                    if (properties.Contains(property.Metadata.Name))
                    {
                        property.IsModified = true;
                    }
                }
            }
            return _context.SaveChanges() > 0;
        }

        /// <summary>
        /// 更新某个字段
        /// </summary>
        /// <param name="entities">实体</param>
        /// <param name="properties">更新字段</param>
        /// <returns></returns>
        public async Task<bool> UpdateAsync<TSource>(IEnumerable<TSource> entities, string[] properties) where TSource : class
        {
            if (properties == null || properties.Length == 0)
                return false;
            foreach (var entity in entities)
            {
                _context.Set<TSource>().Attach(entity);
                var entry = _context.Entry(entity);
                entry.State = EntityState.Unchanged;
                foreach (var property in entry.Properties)
                {
                    if (properties.Contains(property.Metadata.Name))
                    {
                        property.IsModified = true;
                    }
                }
            }
            return await _context.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 根据条件更新
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="lstColumns">更新字段</param>
        /// <param name="lstIgnoreColumns">忽略字段</param>
        /// <param name="strWhere">条件</param>
        /// <returns></returns>
        public bool Update(T entity, List<string>? lstColumns = default, List<string>? lstIgnoreColumns = default, string strWhere = "")
        {
            var entry = _context.Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                _context.Set<T>().Attach(entity);
            }
            if (lstIgnoreColumns != null && lstIgnoreColumns.Count > 0)
            {
                foreach (var item in lstIgnoreColumns)
                {
                    entry.Property(item).IsModified = false;
                }
            }
            if (lstColumns != null && lstColumns.Count > 0)
            {
                foreach (var item in lstColumns)
                {
                    entry.Property(item).IsModified = true;
                }
            }
            return _context.SaveChanges() > 0;
        }

        /// <summary>
        /// 根据条件更新
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="lstColumns"></param>
        /// <param name="lstIgnoreColumns"></param>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        public async Task<bool> UpdateAsync(T entity, List<string>? lstColumns = default, List<string>? lstIgnoreColumns = default, string strWhere = "")
        {
            var entry = _context.Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                _context.Set<T>().Attach(entity);
            }
            if (lstIgnoreColumns != null && lstIgnoreColumns.Count > 0)
            {
                foreach (var item in lstIgnoreColumns)
                {
                    entry.Property(item).IsModified = false;
                }
            }
            if (lstColumns != null && lstColumns.Count > 0)
            {
                foreach (var item in lstColumns)
                {
                    entry.Property(item).IsModified = true;
                }
            }
            return await _context.SaveChangesAsync() > 0;
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
            _context.Set<T>().Remove(entity);
            return _context.SaveChanges() > 0;
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <returns></returns>
        public async Task<bool> DeleteAsync(T entity)
        {
            _context.Set<T>().Remove(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 删除数据(批量)
        /// </summary>
        /// <param name="entity">实体类集合</param>
        /// <returns></returns>
        public bool Delete(List<T> entity)
        {
            _context.Set<T>().RemoveRange(entity);
            return _context.SaveChanges() > 0;
        }

        /// <summary>
        /// 删除数据(批量)
        /// </summary>
        /// <param name="entity">实体类集合</param>
        /// <returns></returns>
        public async Task<bool> DeleteAsync(List<T> entity)
        {
            _context.Set<T>().RemoveRange(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<bool> DeleteAsync(Expression<Func<T, bool>> predicate)
        {
            var entity = _context.Set<T>().Where(predicate).ToList();
            _context.Set<T>().RemoveRange(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 根据主键标识批量删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public bool DeleteByIds<T>(object[] ids) where T : class, new()
        {
            var list = new List<T>();
            foreach (var id in ids)
            {
                var entity = new T();
                entity.GetType().GetProperty("Id")?.SetValue(entity, id);
                list.Add(entity);
            }
            _context.Set<T>().RemoveRange(list);
            return _context.SaveChanges() > 0;
        }

        /// <summary>
        /// 根据主键标识批量删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task<bool> DeleteByIdsAsync<T>(object[] ids) where T : class, new()
        {
            var list = new List<T>();
            foreach (var id in ids)
            {
                var entity = new T();
                entity.GetType().GetProperty("Id")?.SetValue(entity, id);
                list.Add(entity);
            }
            _context.Set<T>().RemoveRange(list);
            return await _context.SaveChangesAsync() > 0;
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
            return _context.Set<T>().Any(predicate);
        }

        /// <summary>
        /// 判断数据是否存在
        /// </summary>
        /// <param name="predicate">条件表达式树</param>
        /// <returns></returns>
        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
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
        public int GetCount(Expression<Func<T, bool>> predicate)
        {
            return _context.Set<T>().Count(predicate);
        }

        /// <summary>
        /// 获取数据总数
        /// </summary>
        /// <param name="predicate">条件表达式树</param>
        /// <returns></returns>
        public async Task<int> GetCountAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().CountAsync(predicate);
        }

        #endregion 获取数据总数

        #region 查询分页数据

        //计算页码
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
        public IPageList<T> QueryPage(Expression<Func<T, bool>> predicate, string orderBy = "", int pageIndex = 1, int pageSize = 20)
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
        public async Task<IPageList<T>> QueryPageAsync(Expression<Func<T, bool>> predicate, string orderBy = "", int pageIndex = 1, int pageSize = 20)
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
        public IPageList<T> QueryPage(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> orderByExpression, OrderByType orderByType, int pageIndex = 1, int pageSize = 20)
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
        public async Task<IPageList<T>> QueryPageAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> orderByExpression, OrderByType orderByType, int pageIndex = 1, int pageSize = 20)
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
        public IQueryable<T> IQueryablePage(IQueryable<T> queryable, int pageIndex, int pagesize, out int rowcount, Dictionary<string, QueryOrderBy> orderBy, bool returnRowCount = true)
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
        public List<T> SqlQuery(string sql, object? parameters = null)
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
        public async Task<List<T>> SqlQueryAsync(string sql, object? parameters = null)
        {
            if (parameters == null)
                return await _context.Set<T>().FromSqlRaw(sql).ToListAsync();
            return await _context.Set<T>().FromSqlRaw(sql, parameters).ToListAsync();
        }

        /// <summary>
        /// 执行sql语句并返回到指定实体中
        /// </summary>
        /// <typeparam name="TEntity">映射到这个实体</typeparam>
        /// <param name="sql">sql</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public List<TEntity> SqlQuery<TEntity>(string sql, DbParameter[]? parameters = null)
        {
            if (parameters == null)
                return _context.Database.SqlQueryRaw<TEntity>(sql).ToList();
            return _context.Database.SqlQueryRaw<TEntity>(sql, parameters).ToList();
        }

        /// <summary>
        /// 执行sql语句并返回到指定实体中
        /// </summary>
        /// <typeparam name="TEntity">映射到这个实体</typeparam>
        /// <param name="sql">sql</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public async Task<List<TEntity>> SqlQueryAsync<TEntity>(string sql, DbParameter[]? parameters)
        {
            if (parameters == null)
                return await _context.Database.SqlQueryRaw<TEntity>(sql).ToListAsync();
            return await _context.Database.SqlQueryRaw<TEntity>(sql, parameters).ToListAsync();
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
        public IPageList<TEntity> SqlPageQuery<TEntity>(string sql, object? parameters, int pageIndex, int pageSize) where TEntity : class, new()
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
        public async Task<IPageList<TEntity>> SqlPageQueryAsync<TEntity>(string sql, object? parameters, int pageIndex, int pageSize) where TEntity : class, new()
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
        public int ExecuteSql(string sql, object? parameters)
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
        public async Task<int> ExecuteSqlAsync(string sql, object? parameters)
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
        public bool DbContextBeginTransaction(Func<bool> func)
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
                    return false;
                }
            }
        }

        #endregion 开启事务
    }
}