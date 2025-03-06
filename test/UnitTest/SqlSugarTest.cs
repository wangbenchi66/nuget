using Easy.SqlSugar.Core;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar;
using UnitTest.Repository;
using WBC66.Autofac.Core;

namespace UnitTest
{
    [TestClass]
    public class SqlSugarTest : BaseUnitTest
    {
        private readonly IUserRepository _userRepository = ServiceProvider.GetRequiredService<IUserRepository>();
        private readonly ICategoryRepository _categoryRepository = ServiceProvider.GetRequiredService<ICategoryRepository>();

        [TestMethod]
        public async Task TestMethod1()
        {
            var list = await _userRepository.GetListAsync(p => p.Id > 0);
            var list2 = await _categoryRepository.GetListAsync(p => true);
            //Assert.IsNotNull(list);
            Assert.IsTrue(list.Count == 7);
        }

        [TestMethod]
        public async Task Update()
        {
            string sql = "UPDATE j_user SET HeadImgUrl=@HeadImgUrl WHERE ID=@Id;";
            List<User> list = await _userRepository.GetListAsync(p => p.Id == 1);
            list.ForEach(x =>
            {
                x.HeadImgUrl = "7";
            }
            );
            var res = await _userRepository.ExecuteSqlAsync(sql, list);
            Assert.IsTrue(res > 0);
        }
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
}