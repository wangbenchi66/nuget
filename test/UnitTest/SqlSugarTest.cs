using Microsoft.Extensions.DependencyInjection;
using UnitTest.Repository;

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
    }
}