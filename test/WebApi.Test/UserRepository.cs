using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using SqlSugar;
using WBC66.SqlSugar.Core;

namespace UnitTest.Repository
{
    /// <summary>
    /// 用户表
    ///</summary>
    [SugarTable("test_user")]//表别名
    [Tenant("journal")]//数据库标识 需要与配置文件中的ConfigId对应
    public class User
    {
        /// <summary>
        ///用户id
        ///</summary>
        [Key]
        [SugarColumn(ColumnName = "ID", IsPrimaryKey = true)]
        public int Id { get; set; }

        [SugarColumn(ColumnName = "Name")]
        public string Name { get; set; }
    }

    /// <summary>
    /// 用户仓储
    /// </summary>
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        //在这里直接用base.  也可以直接调用仓储的方法
        public override User GetSingle(Expression<Func<User, bool>> where)
        {
            return base.SqlSugarDbContext.Queryable<User>().Where(where).Where(p => p.Id == 1).First();
        }
    }

    /// <summary>
    /// 用户仓储接口层
    /// </summary>
    public interface IUserRepository : IBaseRepository<User>
    {
    }
}