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
    }
}