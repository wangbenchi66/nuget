using System.Linq.Expressions;
using SqlSugar;
using Easy.SqlSugar.Core.BiewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Easy.SqlSugar.Core
{
    /// <summary>
    /// 通用仓储
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseSqlSugarRepository<T> : SimpleClient<T>, IBaseSqlSugarRepository<T> where T : class, new()
    {
        private ISqlSugarClient GetSqlSugarClient()
        {
            //var db = new SqlSugarScope(Config.SqlSugarConfigs);
            //var client = db.GetConnectionScopeWithAttr<T>();
            //ISqlSugarClient sqlSugarDb = null;
            ISqlSugarClient sqlSugarDb = AppService.Services.BuildServiceProvider().GetRequiredService<ISqlSugarClient>();
            //using (var scope = AppService.Services.BuildServiceProvider().CreateScope())
            //{
            //    sqlSugarDb = scope.ServiceProvider.GetRequiredService<ISqlSugarClient>();
            //}
            //如果T上没有Tenant特性标记则使用默认的ConfigId
            SqlSugarScopeProvider db = null;
            if (typeof(T).GetCustomAttributes(typeof(TenantAttribute), true).Length == 0)
            {
                db = sqlSugarDb.AsTenant().GetConnectionScope(sqlSugarDb.CurrentConnectionConfig.ConfigId);
            }
            else
            {
                db = sqlSugarDb.AsTenant().GetConnectionScopeWithAttr<T>();
            }
            //db = sqlSugarDb.AsTenant().GetConnectionScopeWithAttr<T>();
            return db;
        }

        /// <summary>
        /// db上下文
        /// </summary>
        /// <remarks>
        /// 如果遇到线程安全问题 参考 下边三个链接内容，已经尽力解决线程安全问题了，如果还是出现问题可以使用db.CopyNew()方法重新创建一个新的实例 保证线程安全
        /// https://www.donet5.com/Home/Doc?typeId=1231  
        /// https://www.donet5.com/Home/Doc?typeId=1224
        /// https://www.donet5.com/Home/Doc?typeId=2349
        /// </remarks>
        public ISqlSugarClient SqlSugarDbContext
        {
            get
            {
                return GetSqlSugarClient();
            }
        }

        public override ISqlSugarClient Context => SqlSugarDbContext;

        /// <summary>
        /// SqlSugarAdo
        /// </summary>
        public IAdo SqlSugarDbContextAdo => SqlSugarDbContext.Ado;

        /// <summary>
        /// //多租户事务、GetConnection、IsAnyConnection等功能
        /// </summary>
        public ITenant SqlSugarTenant => SqlSugarDbContext.AsTenant();

        #region 获取单个实体

        public override T GetSingle(Expression<Func<T, bool>> whereExpression)
        {
            return SqlSugarDbContext.Queryable<T>().First(whereExpression);
        }

        public override Task<T> GetSingleAsync(Expression<Func<T, bool>> whereExpression)
        {
            return SqlSugarDbContext.Queryable<T>().FirstAsync(whereExpression);
        }

        #endregion 获取单个实体

        #region 获取列表

        /// <summary>
        /// 根据条件查询数据
        /// </summary>
        /// <param name="predicate">条件表达式树</param>
        /// <param name="orderBy">排序字段，如name asc,age desc</param>
        /// <returns>泛型实体集合</returns>
        public virtual async Task<List<T>> GetListAsync(Expression<Func<T, bool>> predicate, string orderBy)
        {
            return await SqlSugarDbContext.Queryable<T>().OrderByIF(!string.IsNullOrWhiteSpace(orderBy), orderBy)
                    .WhereIF(predicate != null, predicate).ToListAsync();
        }

        /// <summary>
        /// 根据条件查询数据
        /// </summary>
        /// <param name="predicate">条件表达式树</param>
        /// <param name="orderByPredicate">排序字段</param>
        /// <param name="orderByType">排序顺序</param>
        /// <returns>泛型实体集合</returns>
        public virtual List<T> GetList(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> orderByPredicate, OrderByType orderByType)
        {
            return SqlSugarDbContext.Queryable<T>().OrderByIF(orderByPredicate != null, orderByPredicate, orderByType)
                    .WhereIF(predicate != null, predicate).ToList();
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
            return await SqlSugarDbContext.Queryable<T>().OrderByIF(orderByPredicate != null, orderByPredicate, orderByType)
                    .WhereIF(predicate != null, predicate).ToListAsync();
        }

        #endregion 获取列表

        #region 写入实体数据

        /// <summary>
        /// 写入实体数据
        /// </summary>
        /// <param name="entity">实体数据</param>
        /// <param name="insertColumns">插入的列</param>
        /// <returns></returns>
        public virtual int Insert(T entity, Expression<Func<T, object>>? insertColumns = null)
        {
            var insert = SqlSugarDbContext.Insertable(entity);
            if (insertColumns == null)
                return insert.ExecuteCommand();
            return insert.InsertColumns(insertColumns).ExecuteCommand();
        }

        /// <summary>
        /// 写入实体数据
        /// </summary>
        /// <param name="entity">实体数据</param>
        /// <param name="insertColumns">插入的列</param>
        /// <returns></returns>
        public virtual async Task<int> InsertAsync(T entity, Expression<Func<T, object>>? insertColumns = null)
        {
            var insert = SqlSugarDbContext.Insertable(entity);
            if (insertColumns == null)
                return await insert.ExecuteCommandAsync();
            return await insert.InsertColumns(insertColumns).ExecuteCommandAsync();
        }

        /// <summary>
        /// 批量写入实体数据
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <returns></returns>
        public virtual int Insert(List<T> entity)
        {
            return SqlSugarDbContext.Insertable(entity.ToArray()).ExecuteCommand();
        }

        /// <summary>
        /// 批量写入实体数据
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <returns></returns>
        public virtual async Task<int> InsertAsync(List<T> entity)
        {
            return await SqlSugarDbContext.Insertable(entity.ToArray()).ExecuteCommandAsync();
        }

        #endregion 写入实体数据

        #region 更新实体数据

        /// <summary>
        /// 批量更新实体数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual int Update(List<T> entity)
        {
            return SqlSugarDbContext.Updateable(entity).ExecuteCommand();
        }

        /// <summary>
        /// 批量更新实体数据    
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual async Task<int> UpdateAsync(List<T> entity)
        {
            return await SqlSugarDbContext.Updateable(entity).ExecuteCommandAsync();
        }

        /// <summary>
        /// 批量更新实体数据指定字段
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="updateColumns">要更新的字段x=>new {x.a,x.b}</param>
        /// <returns></returns>
        public virtual int Update(List<T> entity, Expression<Func<T, object>> updateColumns)
        {
            return SqlSugarDbContext.Updateable(entity).UpdateColumns(updateColumns).ExecuteCommand();
        }

        /// <summary>
        /// 批量更新实体数据指定字段
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="updateColumns">要更新的字段x=>new {x.a,x.b}</param>
        /// <returns></returns>
        public virtual async Task<int> UpdateAsync(List<T> entity, Expression<Func<T, object>> updateColumns)
        {
            return await SqlSugarDbContext.Updateable(entity).UpdateColumns(updateColumns).ExecuteCommandAsync();
        }

        /// <summary>
        /// 更新实体数据指定字段
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="updateColumns">要更新的字段x=>new {x.a,x.b}</param>
        /// <returns></returns>
        public virtual int Update(T entity, Expression<Func<T, object>> updateColumns)
        {
            return SqlSugarDbContext.Updateable(entity).UpdateColumns(updateColumns).ExecuteCommand();
        }

        /// <summary>
        /// 更新实体数据指定字段
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="updateColumns">要更新的字段x=>new {x.a,x.b}</param>
        /// <returns></returns>
        public virtual async Task<int> UpdateAsync(T entity, Expression<Func<T, object>> updateColumns)
        {
            return await SqlSugarDbContext.Updateable(entity).UpdateColumns(updateColumns).ExecuteCommandAsync();
        }

        /// <summary>
        /// 更新某个字段
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="updateColumns">要更新的字段x=>new {x.a,x.b}</param>
        /// <param name="where">更新条件lambda判断 x=>x.a==1</param>
        /// <returns></returns>
        public virtual int Update(T entity, Expression<Func<T, object>> updateColumns, Expression<Func<T, object>> where)
        {
            return SqlSugarDbContext.Updateable<T>(entity).UpdateColumns(updateColumns).WhereColumns(where).ExecuteCommand();
        }

        /// <summary>
        /// 更新某个字段
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="updateColumns">要更新的字段x=>new {x.a,x.b}</param>
        /// <param name="where">更新条件lambda判断 x=>x.a==1</param>
        /// <returns></returns>
        public virtual async Task<int> UpdateAsync(T entity, Expression<Func<T, object>> updateColumns, Expression<Func<T, object>> where)
        {
            return await SqlSugarDbContext.Updateable<T>(entity).UpdateColumns(updateColumns).WhereColumns(where).ExecuteCommandAsync();
        }

        /// <summary>
        /// 更新某个字段
        /// </summary>
        /// <param name="entitys"></param>
        /// <param name="updateColumns">要更新的字段x=>new {x.a,x.b}</param>
        /// <param name="where">更新条件 lambda 判断 x=>x.a==1</param>
        /// <returns></returns>
        public virtual int Update(List<T> entitys, Expression<Func<T, object>> updateColumns, Expression<Func<T, object>> where)
        {
            return SqlSugarDbContext.Updateable<T>(entitys).UpdateColumns(updateColumns).WhereColumns(where).ExecuteCommand();
        }

        /// <summary>
        /// 更新某个字段
        /// </summary>
        /// <param name="entitys"></param>
        /// <param name="updateColumns">要更新的字段x=>new {x.a,x.b}</param>
        /// <param name="where">更新条件lambda判断 x=>x.a==1</param>
        /// <returns></returns>
        public virtual async Task<int> UpdateAsync(List<T> entitys, Expression<Func<T, object>> updateColumns, Expression<Func<T, object>> where)
        {
            return await SqlSugarDbContext.Updateable<T>(entitys).UpdateColumns(updateColumns).WhereColumns(where).ExecuteCommandAsync();
        }

        /// <summary>
        /// 根据条件更新
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="updateColumns">要更新的列x=>new {x.a,x.b}</param>
        /// <param name="ignoreColumns">忽略的列x=>new {x.a,x.b}</param>
        /// <param name="where">更新条件lambda判断 x=>x.a==1</param>
        /// <returns></returns>
        public virtual int Update(T entity, Expression<Func<T, object>> updateColumns, Expression<Func<T, object>> ignoreColumns, Expression<Func<T, object>> where)
        {
            return SqlSugarDbContext.Updateable<T>(entity).UpdateColumns(updateColumns).IgnoreColumns(ignoreColumns).WhereColumns(where).ExecuteCommand();
        }

        /// <summary>
        /// 根据条件更新
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="updateColumns">要更新的列x=>new {x.a,x.b}</param>
        /// <param name="ignoreColumns">忽略的列x=>new {x.a,x.b}</param>
        /// <param name="where">更新条件lambda判断 x=>x.a==1</param>
        /// <returns></returns>
        public virtual async Task<int> UpdateAsync(T entity, Expression<Func<T, object>> updateColumns, Expression<Func<T, object>> ignoreColumns, Expression<Func<T, object>> where)
        {
            return await SqlSugarDbContext.Updateable<T>(entity).UpdateColumns(updateColumns).IgnoreColumns(ignoreColumns).WhereColumns(where).ExecuteCommandAsync();
        }

        /// <summary>
        /// 无实体更新
        /// </summary>
        /// <param name="updateColumns">要更新的列x=>new {x.a,x.b}</param>
        /// <param name="where">更新条件lambda判断 x=>x.a==1</param>
        /// <returns></returns>
        public virtual int Update(Expression<Func<T, object>> updateColumns, Expression<Func<T, object>> where)
        {
            return SqlSugarDbContext.Updateable<T>().UpdateColumns(updateColumns).WhereColumns(where).ExecuteCommand();
        }

        /// <summary>
        /// 无实体更新
        /// </summary>
        /// <param name="updateColumns">要更新的列x=>new {x.a,x.b}</param>
        /// <param name="where">更新条件lambda判断 x=>x.a==1</param>
        /// <returns></returns>
        public virtual async Task<int> UpdateAsync(Expression<Func<T, object>> updateColumns, Expression<Func<T, object>> where)
        {
            return await SqlSugarDbContext.Updateable<T>().UpdateColumns(updateColumns).WhereColumns(where).ExecuteCommandAsync();
        }

        /// <summary>
        /// 无实体更新
        /// </summary>
        /// <param name="updateColumns">要更新的列</param>
        /// <param name="where">更新条件lambda判断 x=>x.a==1</param>
        /// <returns></returns>
        public virtual int Update(Dictionary<string, object> updateColumns, Expression<Func<T, object>> where)
        {
            return SqlSugarDbContext.Updateable<T>(updateColumns).WhereColumns(where).ExecuteCommand();
        }

        /// <summary>
        /// 无实体更新
        /// </summary>
        /// <param name="updateColumns">要更新的列</param>
        /// <param name="where">更新条件lambda判断 x=>x.a==1</param>
        /// <returns></returns>
        public virtual async Task<int> UpdateAsync(Dictionary<string, object> updateColumns, Expression<Func<T, object>> where)
        {
            return await SqlSugarDbContext.Updateable<T>(updateColumns).WhereColumns(where).ExecuteCommandAsync();
        }

        /// <summary>
        /// 无实体更新(批量)
        /// </summary>
        /// <param name="updateColumns">要更新的列</param>
        /// <param name="where">更新条件lambda判断 x=>x.a==1</param>
        /// <returns></returns>
        public virtual int Update(List<Dictionary<string, object>> updateColumns, Expression<Func<T, object>> where)
        {
            return SqlSugarDbContext.Updateable<T>(updateColumns).WhereColumns(where).ExecuteCommand();
        }

        /// <summary>
        /// 无实体更新(批量)
        /// </summary>
        /// <param name="updateColumns">要更新的列</param>
        /// <param name="where">更新条件lambda判断 x=>x.a==1</param>
        /// <returns></returns>
        public virtual async Task<int> UpdateAsync(List<Dictionary<string, object>> updateColumns, Expression<Func<T, object>> where)
        {
            return await SqlSugarDbContext.Updateable<T>(updateColumns).WhereColumns(where).ExecuteCommandAsync();
        }

        #endregion 更新实体数据

        #region 添加或更新

        /// <summary>
        /// 添加或更新
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="where">条件lambda判断 x=>new {x.Id}存在则修改 不存在则更新</param>
        /// <returns></returns>
        public virtual int InsertOrUpdate(T entity, Expression<Func<T, object>> where)
        {
            return SqlSugarDbContext.Storageable(entity).WhereColumns(where).ExecuteCommand();
        }

        /// <summary>
        /// 添加或更新
        /// </summary>
        /// <param name="entitys"></param>
        /// <param name="where">条件lambda判断 x=>new {x.Id}存在则修改 不存在则更新</param>
        /// <returns></returns>
        public virtual int InsertOrUpdate(List<T> entitys, Expression<Func<T, object>> where)
        {
            return SqlSugarDbContext.Storageable(entitys).WhereColumns(where).ExecuteCommand();
        }

        /// <summary>
        /// 添加或更新
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="where">条件lambda判断 x=>new {x.Id}存在则修改 不存在则更新</param>
        /// <returns></returns>
        public virtual async Task<int> InsertOrUpdateAsync(T entity, Expression<Func<T, object>> where)
        {
            return await SqlSugarDbContext.Storageable(entity).WhereColumns(where).ExecuteCommandAsync();
        }

        /// <summary>
        /// 添加或更新
        /// </summary>
        /// <param name="entitys"></param>
        /// <param name="where">条件lambda判断 x=>new {x.Id}存在则修改 不存在则更新</param>
        /// <returns></returns>
        public virtual async Task<int> InsertOrUpdateAsync(List<T> entitys, Expression<Func<T, object>> where)
        {
            return await SqlSugarDbContext.Storageable(entitys).WhereColumns(where).ExecuteCommandAsync();
        }

        /*
                /// <summary>
                /// 添加或更新
                /// </summary>
                /// <param name="entity"></param>
                /// <param name="updateColumns">(添加是全量)更新的列x=>new {x.a,x.b}</param>
                /// <param name="where">条件lambda判断 x=>new {x.Id}存在则修改 不存在则更新</param>
                /// <returns></returns>
                public virtual int InsertOrUpdate(TResult entity, Expression<Func<TResult, object>> updateColumns, Expression<Func<TResult, object>> where)
                {
                    var x = SqlSugarDbContext.Storageable(entity).WhereColumns(where).ToStorage();
                    var tran = SqlSugarTenant;
                    tran.BeginTran();
                    try
                    {
                        var insertList = x.InsertList.Select(z => z.Item).ToList();
                        var updateList = x.UpdateList.Select(z => z.Item).ToList();
                        int insertCount = SqlSugarDbContext.Insertable(insertList).ExecuteCommand();
                        int updateCount = SqlSugarDbContext.Updateable(updateList).UpdateColumns(updateColumns).ExecuteCommand();
                        tran.CommitTran();
                        return insertCount + updateCount;
                    }
                    catch (Exception e)
                    {
                        tran.RollbackTran();
                        Console.WriteLine($"执行添加或删除失败,错误:{e.Message},{e.StackTrace}");
                        throw e;
                    }
                }

                /// <summary>
                /// 添加或更新
                /// </summary>
                /// <param name="entitys"></param>
                /// <param name="updateColumns">要添加或更新的列x=>new {x.a,x.b}</param>
                /// <param name="where">条件lambda判断 x=>new {x.Id}存在则修改 不存在则更新</param>
                /// <returns></returns>
                public virtual int InsertOrUpdate(List<TResult> entitys, Expression<Func<TResult, object>> updateColumns, Expression<Func<TResult, object>> where)
                {
                    var x = SqlSugarDbContext.Storageable(entitys).WhereColumns(where).ToStorage();
                    var insertCount = x.AsInsertable.ExecuteCommand();
                    var updateCount = x.AsUpdateable.UpdateColumns(updateColumns).ExecuteCommand();
                    //var x = SqlSugarDbContext.Storageable(entitys).ExecuteCommand();
                }

                /// <summary>
                /// 添加或更新
                /// </summary>
                /// <param name="entity"></param>
                /// <param name="updateColumns">(添加是全量)更新的列x=>new {x.a,x.b}</param>
                /// <param name="where">条件lambda判断 x=>new {x.Id}存在则修改 不存在则更新</param>
                /// <returns></returns>
                public virtual async Task<int> InsertOrUpdateAsync(TResult entity, Expression<Func<TResult, object>> updateColumns, Expression<Func<TResult, object>> where)
                {
                    var x = SqlSugarDbContext.Storageable(entity).WhereColumns(where).ToStorage();
                    var tran = SqlSugarTenant;
                    await tran.BeginTranAsync();
                    try
                    {
                        var insertList = x.InsertList.Select(z => z.Item).ToList();
                        var updateList = x.UpdateList.Select(z => z.Item).ToList();
                        int insertCount = await SqlSugarDbContext.Insertable(insertList).ExecuteCommandAsync();
                        int updateCount = await SqlSugarDbContext.Updateable(updateList).UpdateColumns(updateColumns).ExecuteCommandAsync();
                        await tran.CommitTranAsync();
                        return insertCount + updateCount;
                    }
                    catch (Exception e)
                    {
                        await tran.RollbackTranAsync();
                        Console.WriteLine($"执行添加或删除失败,错误:{e.Message},{e.StackTrace}");
                        throw e;
                    }
                }

                /// <summary>
                /// 添加或更新
                /// </summary>
                /// <param name="entitys"></param>
                /// <param name="updateColumns">(添加是全量)更新的列x=>new {x.a,x.b}</param>
                /// <param name="where">条件lambda判断 x=>new {x.Id}存在则修改 不存在则更新</param>
                /// <returns></returns>
                public virtual async Task<int> InsertOrUpdateAsync(List<TResult> entitys, Expression<Func<TResult, object>> updateColumns, Expression<Func<TResult, object>> where)
                {
                    var x = SqlSugarDbContext.Storageable(entitys).WhereColumns(where).ToStorage();
                    var tran = SqlSugarTenant;
                    await tran.BeginTranAsync();
                    try
                    {
                        var insertList = x.InsertList.Select(z => z.Item).ToList();
                        var updateList = x.UpdateList.Select(z => z.Item).ToList();
                        int insertCount = await SqlSugarDbContext.Insertable(insertList).ExecuteCommandAsync();
                        int updateCount = await SqlSugarDbContext.Updateable(updateList).UpdateColumns(updateColumns).ExecuteCommandAsync();
                        await tran.CommitTranAsync();
                        return insertCount + updateCount;
                    }
                    catch (Exception e)
                    {
                        await tran.RollbackTranAsync();
                        Console.WriteLine($"执行添加或删除失败,错误:{e.Message},{e.StackTrace}");
                        throw e;
                    }
                }*/

        #endregion 添加或更新

        #region 删除数据

        #endregion 删除数据

        #region 判断数据是否存在

        /// <summary>
        /// 判断数据是否存在
        /// </summary>
        /// <param name="predicate">条件表达式树</param>
        /// <returns></returns>
        public virtual bool Exists(Expression<Func<T, bool>> predicate)
        {
            return SqlSugarDbContext.Queryable<T>().Where(predicate).Any();
        }

        /// <summary>
        /// 判断数据是否存在
        /// </summary>
        /// <param name="predicate">条件表达式树</param>
        /// <returns></returns>
        public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            return await SqlSugarDbContext.Queryable<T>().Where(predicate).AnyAsync();
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
            return SqlSugarDbContext.Queryable<T>().Count(predicate);
        }

        /// <summary>
        /// 获取数据总数
        /// </summary>
        /// <param name="predicate">条件表达式树</param>
        /// <returns></returns>
        public virtual async Task<int> GetCountAsync(Expression<Func<T, bool>> predicate)
        {
            return await SqlSugarDbContext.Queryable<T>().CountAsync(predicate);
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
        public virtual IPageList<T> QueryPage(Expression<Func<T, bool>> predicate, string orderBy = "", int pageIndex = 1,
            int pageSize = 20)
        {
            var totalCount = 0;
            var page = SqlSugarDbContext.Queryable<T>().OrderByIF(!string.IsNullOrEmpty(orderBy), orderBy)
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
        public virtual async Task<IPageList<T>> QueryPageAsync(Expression<Func<T, bool>> predicate, string orderBy = "",
            int pageIndex = 1, int pageSize = 20)
        {
            RefAsync<int> totalCount = 0;
            var page = await SqlSugarDbContext.Queryable<T>().OrderByIF(!string.IsNullOrEmpty(orderBy), orderBy)
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
        public virtual IPageList<T> QueryPage(Expression<Func<T, bool>> predicate,
            Expression<Func<T, object>> orderByExpression, OrderByType orderByType, int pageIndex = 1,
            int pageSize = 20)
        {
            var totalCount = 0;
            var page = SqlSugarDbContext.Queryable<T>().OrderByIF(orderByExpression != null, orderByExpression, orderByType)
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
        public virtual async Task<IPageList<T>> QueryPageAsync(Expression<Func<T, bool>> predicate,
            Expression<Func<T, object>> orderByExpression, OrderByType orderByType, int pageIndex = 1,
            int pageSize = 20)
        {
            RefAsync<int> totalCount = 0;
            var page = await SqlSugarDbContext.Queryable<T>().WhereIF(predicate != null, predicate).OrderByIF(orderByExpression != null, orderByExpression, orderByType)
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

        /// <summary>
        /// 分页
        /// </summary>
        /// <param name="queryable"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pagesize"></param>
        /// <param name="returnRowCount"></param>
        /// <returns></returns>
        public virtual IPageList<T> QueryPage(ISugarQueryable<T> queryable, int pageIndex, int pagesize, bool returnRowCount = true)
        {
            pageIndex = pageIndex <= 0 ? 1 : pageIndex;
            pagesize = pagesize <= 0 ? 10 : pagesize;
            int rowcount = 0;
            List<T>? list = returnRowCount ? queryable.ToPageList(pageIndex, pagesize, ref rowcount) : queryable.ToPageList(pageIndex, pagesize);
            return new PageList<T>(list, pageIndex, pagesize, rowcount);
        }

        /// <summary>
        /// 分页
        /// </summary>
        /// <param name="queryable"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pagesize"></param>
        /// <param name="returnRowCount"></param>
        /// <returns></returns>
        public virtual async Task<IPageList<T>> QueryPageAsync(ISugarQueryable<T> queryable, int pageIndex, int pagesize, bool returnRowCount = true)
        {
            pageIndex = pageIndex <= 0 ? 1 : pageIndex;
            pagesize = pagesize <= 0 ? 10 : pagesize;
            RefAsync<int> rowcount = 0;
            List<T>? list = returnRowCount ? await queryable.ToPageListAsync(pageIndex, pagesize, rowcount) : await queryable.ToPageListAsync(pageIndex, pagesize);
            return new PageList<T>(list, pageIndex, pagesize, rowcount);
        }

        #endregion 根据条件查询分页数据

        #region 执行sql语句

        /// <summary>
        /// 执行sql语句并返回List[TResult]
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual List<T> SqlQuery(string sql, object? parameters)
        {
            return SqlSugarDbContext.Ado.SqlQuery<T>(sql, parameters);
        }

        /// <summary>
        /// 执行sql语句并返回List[TResult]
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual async Task<List<T>> SqlQueryAsync(string sql, object? parameters)
        {
            return await SqlSugarDbContext.Ado.SqlQueryAsync<T>(sql, parameters);
        }

        /// <summary>
        /// 执行sql语句并返回到指定实体中
        /// </summary>
        /// <typeparam name="TResult">映射到这个实体</typeparam>
        /// <param name="sql">sql</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public virtual List<TResult> SqlQuery<TResult>(string sql, object? parameters)
        {
            return SqlSugarDbContext.Ado.SqlQuery<TResult>(sql, parameters);
        }

        /// <summary>
        /// 执行sql语句并返回到指定实体中
        /// </summary>
        /// <typeparam name="TResult">映射到这个实体</typeparam>
        /// <param name="sql">sql</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public virtual async Task<List<TResult>> SqlQueryAsync<TResult>(string sql, object? parameters)
        {
            return await SqlSugarDbContext.Ado.SqlQueryAsync<TResult>(sql, parameters);
        }

        /// <summary>
        /// 执行sql语句并返回到指定实体中
        /// </summary>
        /// <typeparam name="TResult">映射到这个实体</typeparam>
        /// <param name="sql">sql</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public virtual TResult SqlQuerySingle<TResult>(string sql, object? parameters)
        {
            return SqlSugarDbContext.Ado.SqlQuerySingle<TResult>(sql, parameters);
        }

        /// <summary>
        /// 执行sql语句并返回到指定实体中
        /// </summary>
        /// <typeparam name="TResult">映射到这个实体</typeparam>
        /// <param name="sql">sql</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public virtual async Task<TResult> SqlQuerySingleAsync<TResult>(string sql, object? parameters)
        {
            return await SqlSugarDbContext.Ado.SqlQuerySingleAsync<TResult>(sql, parameters);
        }

        /// <summary>
        /// 执行分页sql语句并返回到指定实体中
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public virtual IPageList<TResult> SqlPageQuery<TResult>(string sql, object? parameters, int pageIndex, int pageSize)
        {
            //计算分页
            var skip = (pageIndex - 1) * pageSize;
            var take = pageSize;
            var list = SqlSugarDbContext.Ado.SqlQuery<TResult>(sql, parameters);
            var total = list.Count;
            if (total == 0)
                return new PageList<TResult>(null, pageIndex, pageSize, total);
            return new PageList<TResult>(list.Skip(skip).Take(take).ToList(), pageIndex, pageSize, total);
        }

        /// <summary>
        /// 执行分页sql语句并返回到指定实体中
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public virtual async Task<IPageList<TResult>> SqlPageQueryAsync<TResult>(string sql, object? parameters, int pageIndex, int pageSize)
        {
            //计算分页
            var skip = (pageIndex - 1) * pageSize;
            var take = pageSize;
            var list = await SqlSugarDbContext.Ado.SqlQueryAsync<TResult>(sql, parameters);
            var total = list.Count;
            if (total == 0)
                return new PageList<TResult>(null, pageIndex, pageSize, total);
            return new PageList<TResult>(list.Skip(skip).Take(take).ToList(), pageIndex, pageSize, total);
        }

        /// <summary>
        /// 执行sql语句返回影响行数
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数</param>
        /// <returns>返回影响行数</returns>
        public virtual int ExecuteSql(string sql, object? parameters)
        {
            if (parameters is IEnumerable<object> parameterList)
            {
                int totalAffectedRows = 0;
                using (SqlSugarDbContext.Ado.OpenAlways())
                {
                    foreach (var parameter in GetSugarParameters(parameterList) as List<SugarParameter[]>)
                    {
                        totalAffectedRows += SqlSugarDbContext.Ado.ExecuteCommand(sql, parameter);
                    }
                    return totalAffectedRows;
                }
            }
            else
            {
                return SqlSugarDbContext.Ado.ExecuteCommand(sql, parameters);
            }
        }

        /// <summary>
        /// 执行sql语句返回影响行数
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数</param>
        /// <returns>返回影响行数</returns>
        public virtual async Task<int> ExecuteSqlAsync(string sql, object? parameters)
        {
            if (parameters is IEnumerable<object> parameterList)
            {
                int totalAffectedRows = 0;
                using (SqlSugarDbContext.Ado.OpenAlways())
                {
                    foreach (var parameter in GetSugarParameters(parameterList) as List<SugarParameter[]>)
                    {
                        totalAffectedRows += await SqlSugarDbContext.Ado.ExecuteCommandAsync(sql, parameter);
                    }
                    return totalAffectedRows;
                }
            }
            else
            {
                return await SqlSugarDbContext.Ado.ExecuteCommandAsync(sql, parameters);
            }
        }

        /// <summary>
        /// 将parameter中的参数转换成List[SugarParameter]类型
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        private static object GetSugarParameters(object parameter)
        {
            if (parameter == null)
            {
                return null;
            }
            if (parameter is IEnumerable<object> parameterList)
            {
                List<SugarParameter[]> sugarParameters = new List<SugarParameter[]>();
                foreach (var item in parameterList)
                {
                    var properties = item.GetType().GetProperties();
                    List<SugarParameter> sugarParameter = new List<SugarParameter>();
                    foreach (var property in properties)
                    {
                        var value = property.GetValue(item);
                        if (value != null)
                        {
                            sugarParameter.Add(new SugarParameter(property.Name, value));
                        }
                    }
                    sugarParameters.Add(sugarParameter.ToArray());
                }
                return sugarParameters;
            }
            if (parameter is T)
            {
                var properties = parameter.GetType().GetProperties();
                List<SugarParameter> sugarParameters = new List<SugarParameter>();
                foreach (var property in properties)
                {
                    var value = property.GetValue(parameter);
                    if (value != null)
                    {
                        sugarParameters.Add(new SugarParameter(property.Name, value));
                    }
                }
                return sugarParameters;
            }
            return parameter;
        }

        #endregion 执行sql语句

        #region 事务

        /// <summary>
        /// 事务
        /// </summary>
        /// <param name="func"></param>
        /// <param name="logAction"></param>
        /// <returns></returns>
        public virtual bool DbContextBeginTransaction(Func<bool> func, Action<Exception>? logAction = null)
        {
            var result = new bool();
            var tran = SqlSugarTenant;
            try
            {
                tran.BeginTran();
                result = func();
                if (result)
                {
                    tran.CommitTran();
                }
                else
                {
                    tran.RollbackTran();
                    result = false;
                }
            }
            catch (Exception ex)
            {
                tran.RollbackTran();
                result = false;
                Console.WriteLine("执行事务发生错误，错误信息:{0},详细信息:{1}", ex.Message, ex);
                logAction?.Invoke(ex);
            }
            return result;
        }

        /// <summary>
        /// 事务
        /// </summary>
        /// <param name="func"></param>
        /// <param name="logAction"></param>
        /// <returns></returns>
        public virtual async Task<bool> DbContextBeginTransactionAsync(Func<Task<bool>> func, Action<Exception>? logAction = null)
        {
            var result = new bool();
            var tran = SqlSugarTenant;
            try
            {
                await tran.BeginTranAsync();
                result = await func();
                if (result)
                {
                    await tran.CommitTranAsync();
                }
                else
                {
                    await tran.RollbackTranAsync();
                    result = false;
                }
            }
            catch (Exception ex)
            {
                await tran.RollbackTranAsync();
                result = false;
                Console.WriteLine("执行事务发生错误，错误信息:{0},详细信息:{1}", ex.Message, ex);
                logAction?.Invoke(ex);
            }
            return result;
        }

        #endregion 事务
    }
}