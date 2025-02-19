using System.ComponentModel.DataAnnotations;
using SqlSugar;
using Easy.SqlSugar.Core;
using WBC66.Autofac.Core;

namespace UnitTest.Repository
{
    internal enum DbData
    {
        journal,
        CheckIn
    }

    /// <summary>
    /// 用户表
    ///</summary>
    [SugarTable("J_User")]
    [Tenant(nameof(DbData.journal))]
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

        /// <summary>
        ///用户名称
        ///</summary>
        [SugarColumn(ColumnName = "Coding")]
        public string Coding { get; set; }

        /// <summary>
        ///用户名称
        ///</summary>
        [SugarColumn(ColumnName = "HeadImgUrl")]
        public string HeadImgUrl { get; set; }

    }

    /// <summary>
    /// 用户仓储
    /// </summary>
    public class UserRepository : BaseSqlSugarIocRepository<User>, IUserRepository
    {
    }

    /// <summary>
    /// 用户仓储接口层
    /// </summary>
    public interface IUserRepository : IBaseSqlSugarRepository<User>, ISingleton
    {
    }

    #region 打卡模块

    public class CategoryRepository : BaseSqlSugarIocRepository<Category>, ICategoryRepository
    {
    }

    public interface ICategoryRepository : IBaseSqlSugarRepository<Category>, ISingleton
    {
    }

    /// <summary>
    /// 打卡类别
    ///</summary>
    [SugarTable("checkin.category")]
    [Tenant(DbData.CheckIn)]
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
        public string Title { get; set; }
    }


    #endregion 打卡模块
}