using Easy.Cache.Core;
using Microsoft.Extensions.DependencyInjection;

namespace UnitTest
{
    [TestClass]
    public class RedisTest : BaseUnitTest
    {
        private IEasyCacheService _cacheService;

        [TestInitialize]
        public void TestInitialize()
        {
            _cacheService = ServiceProvider.GetService<IEasyCacheServiceFactory>().Create();
        }

        [TestMethod]
        public void TestMethod1()
        {
            var value = _cacheService.Add("key", 123, -1);
            var value2 = _cacheService.Get<int>("key", () =>
            {
                return 0;
            }, -1);
            Assert.IsTrue(value);
        }
    }
}