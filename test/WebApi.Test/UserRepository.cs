﻿using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using SqlSugar;
using WBC66.Autofac.Core;
using Easy.SqlSugar.Core;
using Easy.SqlSugar.Core.BaseProvider;

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

        public string Name { get; set; }

        [SugarColumn(InsertServerTime = true)]
        public DateTime CreateTime { get; set; }
    }


    /// <summary>
    /// 用户仓储
    /// </summary>
    /*public class UserRepository : BaseSqlSugarIocRepository<User>, ISingleton
    {
    }*/

    public class UserRepository : BaseSqlSugarRepository<User>, IUserRepository
    {
        public UserRepository(ISqlSugarClient db) : base(db)
        {
        }
    }

    /// <summary>
    /// 用户仓储接口层
    /// </summary>
    public interface IUserRepository : IBaseSqlSugarRepository<User>, ISingleton
    {
    }

    public class UserService : BaseSqlSugarService<User, IUserRepository>, IUserService
    {
        public UserService(IUserRepository repository) : base(repository)
        {
        }
    }

    public interface IUserService : IBaseSqlSugarService<User>, ISingleton
    {
    }


    #region 打卡模块

    public class CategoryRepository : BaseSqlSugarRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(ISqlSugarClient db) : base(db)
        {
        }
    }

    public interface ICategoryRepository : IBaseSqlSugarRepository<Category>, ISingleton
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