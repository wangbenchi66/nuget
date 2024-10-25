using System.ComponentModel.DataAnnotations;
using SqlSugar;
using WBC66.SqlSugar.Core;

namespace UnitTest.Repository
{
    /// <summary>
    /// 用户表
    ///</summary>
    [SugarTable("J_User")]
    [Tenant("journal")]
    public class User
    {
        /// <summary>
        ///用户id
        ///</summary>
        [Key]
        [SugarColumn(ColumnName = "ID", IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        /// <summary>
        ///用户名称
        ///</summary>
        [SugarColumn(ColumnName = "Name")]
        public string Name { get; set; }
    }

    /// <summary>
    /// 用户仓储
    /// </summary>
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        //在这里直接用base.  也可以直接调用仓储的方法
    }

    /// <summary>
    /// 用户仓储接口层
    /// </summary>
    public interface IUserRepository : IBaseRepository<User>
    {
    }
}