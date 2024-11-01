using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using WBC66.Cache.Core;

namespace UnitTest
{
    [TestClass]
    public class RedisTest : BaseUnitTest
    {
        private IRedisService _redisService;

        [TestInitialize]
        public void TestInitialize()
        {
            _redisService = ServiceProvider.GetService<IRedisService>();
            _redisService.Initialization("localhost:6379,abortConnect=false,ssl=false,password=123456");
        }

        [TestMethod]
        public void TestMethod1()
        {
            var value = _redisService.Add("key", 123, -1);
            var value2 = _redisService.Get<int>("key", () =>
            {
                return 0;
            }, -1);
            Assert.IsTrue(value);
        }
    }
}