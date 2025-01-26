using System.ComponentModel.DataAnnotations;
using SqlSugar;
using WBC66.Autofac.Core;
using WBC66.SqlSugar.Core;
using WBC66.SqlSugar.Core.BaseProvider;

namespace UnitTest.Repository
{
    /// <summary>
    /// 用户表
    ///</summary>
    [SugarTable("j_user")]//表别名
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
    /*public class UserRepository : BaseSqlSugarRepository<User>, IDependency
    {

        //在这里直接用base.  也可以直接调用仓储的方法
        public UserRepository(ISqlSugarClient db) : base(db)
        {
        }
    }*/

    public class UserRepository : BaseSqlSugarRepository<User>, IDependency
    {
        public UserRepository(ISqlSugarClient db) : base(db)
        {
        }
    }

    /// <summary>
    /// 用户仓储接口层
    /// </summary>
    public interface IUserRepository : IBaseRepository<User>
    {
    }

    #region 打卡模块

    public class CategoryRepository : BaseSqlSugarRepository<Category>, ISingleton
    {
        public CategoryRepository(ISqlSugarClient db) : base(db)
        {
        }
    }

    /// <summary>
    /// 打卡类别
    ///</summary>
    [SugarTable("checkin.category")]
    [Tenant("CheckIn")]
    public class Category
    {
        /// <summary>
        /// 主键 
        ///</summary>
        [SugarColumn(ColumnName = "ID", IsPrimaryKey = true, IsIdentity = true)]
        public long ID { get; set; }

        /// <summary>
        /// 标题 
        ///</summary>
        [SugarColumn(ColumnName = "Title")]
        public string Title { get; set; }
    }


    #endregion 打卡模块
}