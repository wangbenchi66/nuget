using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using SqlSugar;
using WBC66.Autofac.Core;
using WBC66.SqlSugar.Core;
using WBC66.SqlSugar.Core.BaseProvider;

namespace WebApi.Test
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
    /*public class UserRepository : BaseSqlSugarIocRepository<User>, ISingleton
    {
    }*/

    public class UserRepository : BaseSqlSugarRepository<User>
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

    public class CategoryRepository : BaseSqlSugarRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(ISqlSugarClient db) : base(db)
        {
        }
    }

    public interface ICategoryRepository : IBaseRepository<Category>, ISingleton
    {
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