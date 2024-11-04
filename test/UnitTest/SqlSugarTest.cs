using Microsoft.Extensions.DependencyInjection;
using UnitTest.Repository;

namespace UnitTest
{
    [TestClass]
    public class SqlSugarTest : BaseUnitTest
    {
        private readonly IUserRepository _userRepository;

        public SqlSugarTest()
        {
            _userRepository = ServiceProvider.GetRequiredService<IUserRepository>();
        }

        [TestMethod]
        public async Task TestMethod1()
        {
            var list = await _userRepository.GetListAsync(p => p.Id > 0);
            Assert.IsNotNull(list);
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