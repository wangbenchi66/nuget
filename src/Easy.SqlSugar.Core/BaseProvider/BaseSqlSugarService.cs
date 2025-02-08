using System.Linq.Expressions;
using Easy.SqlSugar.Core.BiewModels;
using SqlSugar;

namespace Easy.SqlSugar.Core.BaseProvider
{
    /// <summary>
    /// SqlSugar服务基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TRepository"></typeparam>
    public class BaseSqlSugarService<T, TRepository>
        where T : class, new()
        where TRepository : IBaseSqlSugarRepository<T>
    {

        protected IBaseSqlSugarRepository<T> _repository;

        public BaseSqlSugarService(TRepository repository)
        {
            _repository = repository;
        }


        #region 获取单个实体

        /// <summary>
        /// 获取单个实体
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public virtual T GetSingle(Expression<Func<T, bool>> where)
        {
            return _repository.GetSingle(where);
        }

        /// <summary>
        /// 获取单个实体
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public virtual async Task<T> GetSingleAsync(Expression<Func<T, bool>> where)
        {
            return await _repository.GetSingleAsync(where);
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
            return _repository.GetList(where);
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public virtual async Task<List<T>> GetListAsync(Expression<Func<T, bool>> where)
        {
            return await _repository.GetListAsync(where);
        }

        /// <summary>
        /// 根据条件查询数据
        /// </summary>
        /// <param name="predicate">条件表达式树</param>
        /// <param name="orderBy">排序字段，如name asc,age desc</param>
        /// <returns>泛型实体集合</returns>
        public virtual async Task<List<T>> GetListAsync(Expression<Func<T, bool>> predicate, string orderBy = "")
        {
            return await _repository.GetListAsync(predicate, orderBy);
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
            return await _repository.GetListAsync(predicate, orderByPredicate, orderByType);
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
            return _repository.Insert(entity);
        }

        /// <summary>
        /// 写入实体数据
        /// </summary>
        /// <param name="entity">实体数据</param>
        /// <returns></returns>
        public virtual async Task<int> InsertAsync(T entity)
        {
            return await _repository.InsertAsync(entity);
        }

        /// <summary>
        /// 写入实体数据
        /// </summary>
        /// <param name="entity">实体数据</param>
        /// <param name="insertColumns">插入的列</param>
        /// <returns></returns>
        public virtual int Insert(T entity, Expression<Func<T, object>>? insertColumns = null)
        {
            return _repository.Insert(entity, insertColumns);
        }

        /// <summary>
        /// 写入实体数据
        /// </summary>
        /// <param name="entity">实体数据</param>
        /// <param name="insertColumns">插入的列</param>
        /// <returns></returns>
        public virtual async Task<int> InsertAsync(T entity, Expression<Func<T, object>>? insertColumns = null)
        {
            return await _repository.InsertAsync(entity, insertColumns);
        }

        /// <summary>
        /// 批量写入实体数据
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <returns></returns>
        public virtual int Insert(List<T> entity)
        {
            return _repository.Insert(entity);
        }

        /// <summary>
        /// 批量写入实体数据
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <returns></returns>
        public virtual async Task<int> InsertAsync(List<T> entity)
        {
            return await _repository.InsertAsync(entity);
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
            return _repository.Update(entity);
        }

        /// <summary>
        /// 批量更新实体数据    
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual async Task<int> UpdateAsync(List<T> entity)
        {
            return await _repository.UpdateAsync(entity);
        }

        /// <summary>
        /// 更新实体数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual int Update(T entity)
        {
            return _repository.Update(entity);
        }

        /// <summary>
        /// 更新实体数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual async Task<int> UpdateAsync(T entity)
        {
            return await _repository.UpdateAsync(entity);
        }

        /// <summary>
        /// 批量更新实体数据指定字段
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="updateColumns">要更新的字段x=>new {x.a,x.b}</param>
        /// <returns></returns>
        public virtual int Update(List<T> entity, Expression<Func<T, object>> updateColumns)
        {
            return _repository.Update(entity, updateColumns);
        }

        /// <summary>
        /// 批量更新实体数据指定字段
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="updateColumns">要更新的字段x=>new {x.a,x.b}</param>
        /// <returns></returns>
        public virtual async Task<int> UpdateAsync(List<T> entity, Expression<Func<T, object>> updateColumns)
        {
            return await _repository.UpdateAsync(entity, updateColumns);
        }

        /// <summary>
        /// 更新实体数据指定字段
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="updateColumns">要更新的字段x=>new {x.a,x.b}</param>
        /// <returns></returns>
        public virtual int Update(T entity, Expression<Func<T, object>> updateColumns)
        {
            return _repository.Update(entity, updateColumns);
        }

        /// <summary>
        /// 更新实体数据指定字段
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="updateColumns">要更新的字段x=>new {x.a,x.b}</param>
        /// <returns></returns>
        public virtual async Task<int> UpdateAsync(T entity, Expression<Func<T, object>> updateColumns)
        {
            return await _repository.UpdateAsync(entity, updateColumns);
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
            return _repository.Update(entity, updateColumns, where);
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
            return await _repository.UpdateAsync(entity, updateColumns, where);
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
            return _repository.Update(entitys, updateColumns, where);
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
            return await _repository.UpdateAsync(entitys, updateColumns, where);
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
            return _repository.Update(entity, updateColumns, ignoreColumns, where);
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
            return await _repository.UpdateAsync(entity, updateColumns, ignoreColumns, where);
        }

        /// <summary>
        /// 无实体更新
        /// </summary>
        /// <param name="updateColumns">要更新的列x=>new {x.a,x.b}</param>
        /// <param name="where">更新条件lamdba判断 x=>x.a==1</param>
        /// <returns></returns>
        public virtual int Update(Expression<Func<T, object>> updateColumns, Expression<Func<T, object>> where)
        {
            return _repository.Update(updateColumns, where);
        }

        /// <summary>
        /// 无实体更新
        /// </summary>
        /// <param name="updateColumns">要更新的列x=>new {x.a,x.b}</param>
        /// <param name="where">更新条件lamdba判断 x=>x.a==1</param>
        /// <returns></returns>
        public virtual async Task<int> UpdateAsync(Expression<Func<T, object>> updateColumns, Expression<Func<T, object>> where)
        {
            return await _repository.UpdateAsync(updateColumns, where);
        }

        /// <summary>
        /// 无实体更新
        /// </summary>
        /// <param name="updateColumns">要更新的列</param>
        /// <param name="where">更新条件lamdba判断 x=>x.a==1</param>
        /// <returns></returns>
        public virtual int Update(Dictionary<string, object> updateColumns, Expression<Func<T, object>> where)
        {
            return _repository.Update(updateColumns, where);
        }

        /// <summary>
        /// 无实体更新
        /// </summary>
        /// <param name="updateColumns">要更新的列</param>
        /// <param name="where">更新条件lamdba判断 x=>x.a==1</param>
        /// <returns></returns>
        public virtual async Task<int> UpdateAsync(Dictionary<string, object> updateColumns, Expression<Func<T, object>> where)
        {
            return await _repository.UpdateAsync(updateColumns, where);
        }

        /// <summary>
        /// 无实体更新(批量)
        /// </summary>
        /// <param name="updateColumns">要更新的列</param>
        /// <param name="where">更新条件lamdba判断 x=>x.a==1</param>
        /// <returns></returns>
        public virtual int Update(List<Dictionary<string, object>> updateColumns, Expression<Func<T, object>> where)
        {
            return _repository.Update(updateColumns, where);
        }

        /// <summary>
        /// 无实体更新(批量)
        /// </summary>
        /// <param name="updateColumns">要更新的列</param>
        /// <param name="where">更新条件lamdba判断 x=>x.a==1</param>
        /// <returns></returns>
        public virtual async Task<int> UpdateAsync(List<Dictionary<string, object>> updateColumns, Expression<Func<T, object>> where)
        {
            return await _repository.UpdateAsync(updateColumns, where);
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
            return _repository.InsertOrUpdate(entity);
        }

        /// <summary>
        /// 添加或更新(根据主键判断插入还是更新，例如id=0插入,否则更新)
        /// </summary>
        /// <param name="entitys"></param>
        /// <returns></returns>
        public virtual int InsertOrUpdate(List<T> entitys)
        {
            return _repository.InsertOrUpdate(entitys);
        }

        /// <summary>
        /// 添加或更新(根据主键判断插入还是更新，例如id=0插入,否则更新)
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual async Task<int> InsertOrUpdateAsync(T entity)
        {
            return await _repository.InsertOrUpdateAsync(entity);
        }

        /// <summary>
        /// 添加或更新(根据主键判断插入还是更新，例如id=0插入,否则更新)
        /// </summary>
        /// <param name="entitys"></param>
        /// <returns></returns>
        public virtual async Task<int> InsertOrUpdateAsync(List<T> entitys)
        {
            return await _repository.InsertOrUpdateAsync(entitys);
        }

        /// <summary>
        /// 添加或更新
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="where">条件lamdba判断 x=>new {x.Id}存在则修改 不存在则更新</param>
        /// <returns></returns>
        public virtual int InsertOrUpdate(T entity, Expression<Func<T, object>> where)
        {
            return _repository.InsertOrUpdate(entity, where);
        }

        /// <summary>
        /// 添加或更新
        /// </summary>
        /// <param name="entitys"></param>
        /// <param name="where">条件lamdba判断 x=>new {x.Id}存在则修改 不存在则更新</param>
        /// <returns></returns>
        public virtual int InsertOrUpdate(List<T> entitys, Expression<Func<T, object>> where)
        {
            return _repository.InsertOrUpdate(entitys, where);
        }

        /// <summary>
        /// 添加或更新
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="where">条件lamdba判断 x=>new {x.Id}存在则修改 不存在则更新</param>
        /// <returns></returns>
        public virtual async Task<int> InsertOrUpdateAsync(T entity, Expression<Func<T, object>> where)
        {
            return await _repository.InsertOrUpdateAsync(entity, where);
        }

        /// <summary>
        /// 添加或更新
        /// </summary>
        /// <param name="entitys"></param>
        /// <param name="where">条件lamdba判断 x=>new {x.Id}存在则修改 不存在则更新</param>
        /// <returns></returns>
        public virtual async Task<int> InsertOrUpdateAsync(List<T> entitys, Expression<Func<T, object>> where)
        {
            return await _repository.InsertOrUpdateAsync(entitys, where);
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
            return _repository.InsertOrUpdate(entity, columns, where);
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
            return _repository.InsertOrUpdate(entitys, columns, where);
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
            return await _repository.InsertOrUpdateAsync(entity, columns, where);
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
            return await _repository.InsertOrUpdateAsync(entitys, columns, where);
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
            return _repository.Delete(entity);
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <returns></returns>
        public virtual async Task<bool> DeleteAsync(T entity)
        {
            return await _repository.DeleteAsync(entity);
        }

        /// <summary>
        /// 删除数据(批量)
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <returns></returns>
        public virtual bool Delete(List<T> entity)
        {
            return _repository.Delete(entity);
        }

        /// <summary>
        /// 删除数据(批量)
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <returns></returns>
        public virtual async Task<bool> DeleteAsync(List<T> entity)
        {
            return await _repository.DeleteAsync(entity);
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public virtual async Task<bool> DeleteAsync(Expression<Func<T, bool>> where)
        {
            return await _repository.DeleteAsync(where);
        }

        /// <summary>
        /// 根据主键标识批量删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public virtual bool DeleteByIds(object[] ids)
        {
            return _repository.DeleteByIds(ids);
        }

        /// <summary>
        /// 根据主键标识批量删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public virtual async Task<bool> DeleteByIdsAsync(object[] ids)
        {
            return await _repository.DeleteByIdsAsync(ids);
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
            return _repository.Exists(predicate);
        }

        /// <summary>
        /// 判断数据是否存在
        /// </summary>
        /// <param name="predicate">条件表达式树</param>
        /// <returns></returns>
        public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            return await _repository.ExistsAsync(predicate);
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
            return _repository.GetCount(predicate);
        }

        /// <summary>
        /// 获取数据总数
        /// </summary>
        /// <param name="predicate">条件表达式树</param>
        /// <returns></returns>
        public virtual async Task<int> GetCountAsync(Expression<Func<T, bool>> predicate)
        {
            return await _repository.GetCountAsync(predicate);
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
            return _repository.QueryPage(predicate, orderBy, pageIndex, pageSize);
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
            return await _repository.QueryPageAsync(predicate, orderBy, pageIndex, pageSize);
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
            return _repository.QueryPage(predicate, orderByExpression, orderByType, pageIndex, pageSize);
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
            return await _repository.QueryPageAsync(predicate, orderByExpression, orderByType, pageIndex, pageSize);
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
            return _repository.IQueryablePage(queryable, pageIndex, pagesize, out rowcount, orderBy, returnRowCount);
        }

        #endregion 根据条件查询分页数据

        #region 事务

        /// <summary>
        /// 事务
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public virtual bool DbContextBeginTransaction(Func<bool> func)
        {
            return _repository.DbContextBeginTransaction(func);
        }

        /// <summary>
        /// 事务
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public virtual async Task<bool> DbContextBeginTransactionAsync(Func<Task<bool>> func)
        {
            return await _repository.DbContextBeginTransactionAsync(func);
        }

        #endregion 事务
    }
}