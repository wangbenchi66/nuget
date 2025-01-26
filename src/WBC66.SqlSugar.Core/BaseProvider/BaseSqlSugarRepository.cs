using System.Linq.Expressions;
using SqlSugar;

namespace WBC66.SqlSugar.Core.BaseProvider
{
    public class BaseSqlSugarRepository<T> : SimpleClient<T> where T : class, new()
    {

        /// <summary>
        /// db上下文
        /// </summary>
        public ISqlSugarClient SqlSugarDbContext;

        /// <summary>
        /// SqlSugarAdo
        /// </summary>
        public IAdo SqlSugarDbContextAdo;

        public ITenant itenant = null;//多租户事务、GetConnection、IsAnyConnection等功能

        public BaseSqlSugarRepository(ISqlSugarClient db)
        {
            itenant = db.AsTenant();//用来处理事务
            base.Context = db.AsTenant().GetConnectionWithAttr<T>();//获取子Db
            SqlSugarDbContext = Context;
            SqlSugarDbContextAdo = Context.Ado;
        }
    }
}