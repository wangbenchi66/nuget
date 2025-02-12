﻿using System.Linq.Expressions;
using System.Reflection;
using SqlSugar;
using SqlSugar.IOC;
using Easy.SqlSugar.Core.BiewModels;

namespace Easy.SqlSugar.Core
{
    /// <summary>
    /// SqlSugar通用仓储(Ioc模式)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseSqlSugarIocRepository<T> where T : class, new()
    {
        #region 数据库连接对象

        /// <summary>
        /// //多租户事务、GetConnection、IsAnyConnection等功能
        /// </summary>
        public ITenant Tenant => DbBase.AsTenant();

        /// <summary>
        /// 数据库连接对象
        /// </summary>
        private ISqlSugarClient DbBase
        {
            get
            {
                var configId = typeof(T).GetCustomAttribute<TenantAttribute>()?.configId ?? SqlSugarContext.Options.Configs[0].ConfigId;
                return DbScoped.SugarScope.GetConnection(configId);
            }
        }

        /// <summary>
        /// db上下文
        /// </summary>
        public ISqlSugarClient SqlSugarDbContext => DbBase;

        /// <summary>
        /// SqlSugarAdo
        /// </summary>
        public IAdo SqlSugarDbContextAdo => DbBase.Ado;

        #endregion 数据库连接对象

        #region 获取单个实体

        /// <summary>
        /// 获取单个实体
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public virtual T GetSingle(Expression<Func<T, bool>> where)
        {
            return SqlSugarDbContext.Queryable<T>().First(where);
        }

        /// <summary>
        /// 获取单个实体
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public virtual async Task<T> GetSingleAsync(Expression<Func<T, bool>> where)
        {
            return await SqlSugarDbContext.Queryable<T>().FirstAsync(where);
        }

        #endregion 获取单个实体

        #region 获取列表

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public virtual List<T> GetList(Expression<Func<T, bool>> where)
        {
            return SqlSugarDbContext.Queryable<T>().Where(where).ToList();
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public virtual async Task<List<T>> GetListAsync(Expression<Func<T, bool>> where)
        {
            return await SqlSugarDbContext.Queryable<T>().Where(where).ToListAsync();
        }

        /// <summary>
        /// 根据条件查询数据
        /// </summary>
        /// <param name="predicate">条件表达式树</param>
        /// <param name="orderBy">排序字段，如name asc,age desc</param>
        /// <returns>泛型实体集合</returns>
        public virtual async Task<List<T>> GetListAsync(Expression<Func<T, bool>> predicate, string orderBy = "")
        {
            return await SqlSugarDbContext.Queryable<T>().OrderByIF(!string.IsNullOrEmpty(orderBy), orderBy)
                    .WhereIF(predicate != null, predicate).ToListAsync();
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
        /// <returns></returns>
        public virtual int Insert(T entity)
        {
            return SqlSugarDbContext.Insertable(entity).ExecuteReturnIdentity();
        }

        /// <summary>
        /// 写入实体数据
        /// </summary>
        /// <param name="entity">实体数据</param>
        /// <returns></returns>
        public virtual async Task<int> InsertAsync(T entity)
        {
            return await SqlSugarDbContext.Insertable(entity).ExecuteReturnIdentityAsync();
        }

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
                return insert.ExecuteReturnIdentity();
            return insert.InsertColumns(insertColumns).ExecuteReturnIdentity();
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
                return await insert.ExecuteReturnIdentityAsync();
            return await insert.InsertColumns(insertColumns).ExecuteReturnIdentityAsync();
        }

        /// <summary>
        /// 批量写入实体数据
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <returns></returns>
        public virtual int Insert(List<T> entity)
        {
            return SqlSugarDbContext.Insertable(entity.ToArray()).ExecuteReturnIdentity();
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
        /// 更新实体数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual int Update(T entity)
        {
            return SqlSugarDbContext.Updateable(entity).ExecuteCommand();
        }

        /// <summary>
        /// 更新实体数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual async Task<int> UpdateAsync(T entity)
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
        /// <param name="where">更新条件lamdba判断 x=>x.a==1</param>
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
        /// <param name="where">更新条件lamdba判断 x=>x.a==1</param>
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
        /// <param name="where">更新条件lamdba判断 x=>x.a==1</param>
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
        /// <param name="where">更新条件lamdba判断 x=>x.a==1</param>
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
        /// <param name="where">更新条件lamdba判断 x=>x.a==1</param>
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
        /// <param name="where">更新条件lamdba判断 x=>x.a==1</param>
        /// <returns></returns>
        public virtual async Task<int> UpdateAsync(T entity, Expression<Func<T, object>> updateColumns, Expression<Func<T, object>> ignoreColumns, Expression<Func<T, object>> where)
        {
            return await SqlSugarDbContext.Updateable<T>(entity).UpdateColumns(updateColumns).IgnoreColumns(ignoreColumns).WhereColumns(where).ExecuteCommandAsync();
        }

        /// <summary>
        /// 无实体更新
        /// </summary>
        /// <param name="updateColumns">要更新的列x=>new {x.a,x.b}</param>
        /// <param name="where">更新条件lamdba判断 x=>x.a==1</param>
        /// <returns></returns>
        public virtual int Update(Expression<Func<T, object>> updateColumns, Expression<Func<T, object>> where)
        {
            return SqlSugarDbContext.Updateable<T>().UpdateColumns(updateColumns).WhereColumns(where).ExecuteCommand();
        }

        /// <summary>
        /// 无实体更新
        /// </summary>
        /// <param name="updateColumns">要更新的列x=>new {x.a,x.b}</param>
        /// <param name="where">更新条件lamdba判断 x=>x.a==1</param>
        /// <returns></returns>
        public virtual async Task<int> UpdateAsync(Expression<Func<T, object>> updateColumns, Expression<Func<T, object>> where)
        {
            return await SqlSugarDbContext.Updateable<T>().UpdateColumns(updateColumns).WhereColumns(where).ExecuteCommandAsync();
        }

        /// <summary>
        /// 无实体更新
        /// </summary>
        /// <param name="updateColumns">要更新的列</param>
        /// <param name="where">更新条件lamdba判断 x=>x.a==1</param>
        /// <returns></returns>
        public virtual int Update(Dictionary<string, object> updateColumns, Expression<Func<T, object>> where)
        {
            return SqlSugarDbContext.Updateable<T>(updateColumns).WhereColumns(where).ExecuteCommand();
        }

        /// <summary>
        /// 无实体更新
        /// </summary>
        /// <param name="updateColumns">要更新的列</param>
        /// <param name="where">更新条件lamdba判断 x=>x.a==1</param>
        /// <returns></returns>
        public virtual async Task<int> UpdateAsync(Dictionary<string, object> updateColumns, Expression<Func<T, object>> where)
        {
            return await SqlSugarDbContext.Updateable<T>(updateColumns).WhereColumns(where).ExecuteCommandAsync();
        }

        /// <summary>
        /// 无实体更新(批量)
        /// </summary>
        /// <param name="updateColumns">要更新的列</param>
        /// <param name="where">更新条件lamdba判断 x=>x.a==1</param>
        /// <returns></returns>
        public virtual int Update(List<Dictionary<string, object>> updateColumns, Expression<Func<T, object>> where)
        {
            return SqlSugarDbContext.Updateable<T>(updateColumns).WhereColumns(where).ExecuteCommand();
        }

        /// <summary>
        /// 无实体更新(批量)
        /// </summary>
        /// <param name="updateColumns">要更新的列</param>
        /// <param name="where">更新条件lamdba判断 x=>x.a==1</param>
        /// <returns></returns>
        public virtual async Task<int> UpdateAsync(List<Dictionary<string, object>> updateColumns, Expression<Func<T, object>> where)
        {
            return await SqlSugarDbContext.Updateable<T>(updateColumns).WhereColumns(where).ExecuteCommandAsync();
        }

        #endregion 更新实体数据

        #region 添加或更新

        /// <summary>
        /// 添加或更新(根据主键判断插入还是更新，例如id=0插入,否则更新)
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual int InsertOrUpdate(T entity)
        {
            return SqlSugarDbContext.Storageable(entity).DefaultAddElseUpdate().ExecuteCommand();
        }

        /// <summary>
        /// 添加或更新(根据主键判断插入还是更新，例如id=0插入,否则更新)
        /// </summary>
        /// <param name="entitys"></param>
        /// <returns></returns>
        public virtual int InsertOrUpdate(List<T> entitys)
        {
            return SqlSugarDbContext.Storageable(entitys).DefaultAddElseUpdate().ExecuteCommand();
        }

        /// <summary>
        /// 添加或更新(根据主键判断插入还是更新，例如id=0插入,否则更新)
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual async Task<int> InsertOrUpdateAsync(T entity)
        {
            return await SqlSugarDbContext.Storageable(entity).DefaultAddElseUpdate().ExecuteCommandAsync();
        }

        /// <summary>
        /// 添加或更新(根据主键判断插入还是更新，例如id=0插入,否则更新)
        /// </summary>
        /// <param name="entitys"></param>
        /// <returns></returns>
        public virtual async Task<int> InsertOrUpdateAsync(List<T> entitys)
        {
            return await SqlSugarDbContext.Storageable(entitys).DefaultAddElseUpdate().ExecuteCommandAsync();
        }

        /// <summary>
        /// 添加或更新
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="where">条件lamdba判断 x=>new {x.Id}存在则修改 不存在则更新</param>
        /// <returns></returns>
        public virtual int InsertOrUpdate(T entity, Expression<Func<T, object>> where)
        {
            return SqlSugarDbContext.Storageable(entity).WhereColumns(where).ExecuteCommand();
        }

        /// <summary>
        /// 添加或更新
        /// </summary>
        /// <param name="entitys"></param>
        /// <param name="where">条件lamdba判断 x=>new {x.Id}存在则修改 不存在则更新</param>
        /// <returns></returns>
        public virtual int InsertOrUpdate(List<T> entitys, Expression<Func<T, object>> where)
        {
            return SqlSugarDbContext.Storageable(entitys).WhereColumns(where).ExecuteCommand();
        }

        /// <summary>
        /// 添加或更新
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="where">条件lamdba判断 x=>new {x.Id}存在则修改 不存在则更新</param>
        /// <returns></returns>
        public virtual async Task<int> InsertOrUpdateAsync(T entity, Expression<Func<T, object>> where)
        {
            return await SqlSugarDbContext.Storageable(entity).WhereColumns(where).ExecuteCommandAsync();
        }

        /// <summary>
        /// 添加或更新
        /// </summary>
        /// <param name="entitys"></param>
        /// <param name="where">条件lamdba判断 x=>new {x.Id}存在则修改 不存在则更新</param>
        /// <returns></returns>
        public virtual async Task<int> InsertOrUpdateAsync(List<T> entitys, Expression<Func<T, object>> where)
        {
            return await SqlSugarDbContext.Storageable(entitys).WhereColumns(where).ExecuteCommandAsync();
        }


        /// <summary>
        /// 添加或更新
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="columns">要添加或更新的列x=>new {x.a,x.b}</param>
        /// <param name="where">条件lamdba判断 x=>new {x.Id}存在则修改 不存在则更新</param>
        /// <returns></returns>
        public virtual int InsertOrUpdate(T entity, Expression<Func<T, object>> columns, Expression<Func<T, object>> where)
        {
            var x = SqlSugarDbContext.Storageable(entity).WhereColumns(where).ToStorage();
            var tran = Tenant;
            tran.BeginTran();
            try
            {
                var insertList = x.InsertList.Select(z => z.Item).ToList();
                var updateList = x.UpdateList.Select(z => z.Item).ToList();
                int insertCount = SqlSugarDbContext.Insertable(insertList).InsertColumns(columns).ExecuteCommand();
                int updateCount = SqlSugarDbContext.Updateable(updateList).UpdateColumns(columns).ExecuteCommand();
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
        /// <param name="columns">要添加或更新的列x=>new {x.a,x.b}</param>
        /// <param name="where">条件lamdba判断 x=>new {x.Id}存在则修改 不存在则更新</param>
        /// <returns></returns>
        public virtual int InsertOrUpdate(List<T> entitys, Expression<Func<T, object>> columns, Expression<Func<T, object>> where)
        {
            var x = SqlSugarDbContext.Storageable(entitys).WhereColumns(where).ToStorage();
            var tran = Tenant;
            tran.BeginTran();
            try
            {
                var insertList = x.InsertList.Select(z => z.Item).ToList();
                var updateList = x.UpdateList.Select(z => z.Item).ToList();
                int insertCount = SqlSugarDbContext.Insertable(insertList).InsertColumns(columns).ExecuteCommand();
                int updateCount = SqlSugarDbContext.Updateable(updateList).UpdateColumns(columns).ExecuteCommand();
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
        /// <param name="entity"></param>
        /// <param name="columns">要添加或更新的列x=>new {x.a,x.b}</param>
        /// <param name="where">条件lamdba判断 x=>new {x.Id}存在则修改 不存在则更新</param>
        /// <returns></returns>
        public virtual async Task<int> InsertOrUpdateAsync(T entity, Expression<Func<T, object>> columns, Expression<Func<T, object>> where)
        {
            var x = SqlSugarDbContext.Storageable(entity).WhereColumns(where).ToStorage();
            var tran = Tenant;
            await tran.BeginTranAsync();
            try
            {
                var insertList = x.InsertList.Select(z => z.Item).ToList();
                var updateList = x.UpdateList.Select(z => z.Item).ToList();
                int insertCount = await SqlSugarDbContext.Insertable(insertList).InsertColumns(columns).ExecuteCommandAsync();
                int updateCount = await SqlSugarDbContext.Updateable(updateList).UpdateColumns(columns).ExecuteCommandAsync();
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
        /// <param name="columns">要添加或更新的列x=>new {x.a,x.b}</param>
        /// <param name="where">条件lamdba判断 x=>new {x.Id}存在则修改 不存在则更新</param>
        /// <returns></returns>
        public virtual async Task<int> InsertOrUpdateAsync(List<T> entitys, Expression<Func<T, object>> columns, Expression<Func<T, object>> where)
        {
            var x = SqlSugarDbContext.Storageable(entitys).WhereColumns(where).ToStorage();
            var tran = Tenant;
            await tran.BeginTranAsync();
            try
            {
                var insertList = x.InsertList.Select(z => z.Item).ToList();
                var updateList = x.UpdateList.Select(z => z.Item).ToList();
                int insertCount = await SqlSugarDbContext.Insertable(insertList).InsertColumns(columns).ExecuteCommandAsync();
                int updateCount = await SqlSugarDbContext.Updateable(updateList).UpdateColumns(columns).ExecuteCommandAsync();
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

        #endregion 添加或更新

        #region 删除数据

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <returns></returns>
        public virtual bool Delete(T entity)
        {
            return SqlSugarDbContext.Deleteable(entity).ExecuteCommandHasChange();
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <returns></returns>
        public virtual async Task<bool> DeleteAsync(T entity)
        {
            return await SqlSugarDbContext.Deleteable(entity).ExecuteCommandHasChangeAsync();
        }

        /// <summary>
        /// 删除数据(批量)
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <returns></returns>
        public virtual bool Delete(List<T> entity)
        {
            return SqlSugarDbContext.Deleteable<T>(entity).ExecuteCommandHasChange();
        }

        /// <summary>
        /// 删除数据(批量)
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <returns></returns>
        public virtual async Task<bool> DeleteAsync(List<T> entity)
        {
            return await SqlSugarDbContext.Deleteable<T>(entity).ExecuteCommandHasChangeAsync();
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public virtual async Task<bool> DeleteAsync(Expression<Func<T, bool>> where)
        {
            return await SqlSugarDbContext.Deleteable<T>().Where(where).ExecuteCommandHasChangeAsync();
        }

        /// <summary>
        /// 根据主键标识批量删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public virtual bool DeleteByIds(object[] ids)
        {
            return SqlSugarDbContext.Deleteable<T>().In(ids).ExecuteCommandHasChange();
        }

        /// <summary>
        /// 根据主键标识批量删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public virtual async Task<bool> DeleteByIdsAsync(object[] ids)
        {
            return await SqlSugarDbContext.Deleteable<T>().In(ids).ExecuteCommandHasChangeAsync();
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

        #endregion 根据条件查询分页数据

        #region 执行sql语句

        /// <summary>
        /// 执行sql语句并返回List[T]
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual List<T> SqlQuery(string sql, object? parameters)
        {
            return SqlSugarDbContext.Ado.SqlQuery<T>(sql, parameters);
        }

        /// <summary>
        /// 执行sql语句并返回List[T]
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
        /// <typeparam name="T">映射到这个实体</typeparam>
        /// <param name="sql">sql</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public virtual List<T> SqlQuery<T>(string sql, object? parameters)
        {
            return SqlSugarDbContext.Ado.SqlQuery<T>(sql, parameters);
        }

        /// <summary>
        /// 执行sql语句并返回到指定实体中
        /// </summary>
        /// <typeparam name="T">映射到这个实体</typeparam>
        /// <param name="sql">sql</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public virtual async Task<List<T>> SqlQueryAsync<T>(string sql, object? parameters)
        {
            return await SqlSugarDbContext.Ado.SqlQueryAsync<T>(sql, parameters);
        }

        /// <summary>
        /// 执行sql语句并返回到指定实体中
        /// </summary>
        /// <typeparam name="T">映射到这个实体</typeparam>
        /// <param name="sql">sql</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public virtual T SqlQuerySingle<T>(string sql, object? parameters)
        {
            return SqlSugarDbContext.Ado.SqlQuerySingle<T>(sql, parameters);
        }

        /// <summary>
        /// 执行sql语句并返回到指定实体中
        /// </summary>
        /// <typeparam name="T">映射到这个实体</typeparam>
        /// <param name="sql">sql</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public virtual async Task<T> SqlQuerySingleAsync<T>(string sql, object? parameters)
        {
            return await SqlSugarDbContext.Ado.SqlQuerySingleAsync<T>(sql, parameters);
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
        public virtual IPageList<T1> SqlPageQuery<T1>(string sql, object? parameters, int pageIndex, int pageSize)
        {
            //计算分页
            var skip = (pageIndex - 1) * pageSize;
            var take = pageSize;
            var list = SqlSugarDbContext.Ado.SqlQuery<T1>(sql, parameters);
            var total = list.Count;
            if (total == 0)
                return new PageList<T1>(null, pageIndex, pageSize, total);
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
        public virtual async Task<IPageList<T>> SqlPageQueryAsync<T>(string sql, object? parameters, int pageIndex, int pageSize)
        {
            //计算分页
            var skip = (pageIndex - 1) * pageSize;
            var take = pageSize;
            var list = await SqlSugarDbContext.Ado.SqlQueryAsync<T>(sql, parameters);
            var total = list.Count;
            if (total == 0)
                return new PageList<T>(null, pageIndex, pageSize, total);
            return new PageList<T>(list.Skip(skip).Take(take).ToList(), pageIndex, pageSize, total);
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
        /// <returns></returns>
        public virtual bool DbContextBeginTransaction(Func<bool> func)
        {
            var result = new bool();
            var tran = Tenant;
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
            }
            return result;
        }

        /// <summary>
        /// 事务
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public virtual async Task<bool> DbContextBeginTransactionAsync(Func<Task<bool>> func)
        {
            var result = new bool();
            var tran = Tenant;
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
            }
            return result;
        }

        #endregion 事务
    }
}