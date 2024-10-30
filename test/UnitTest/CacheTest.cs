using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using WBC66.Cache.Core;

namespace UnitTest
{
    [TestClass]
    public class CacheTest : BaseUnitTest
    {
        private IMemoryCache _memoryCache;

        [TestInitialize]
        public void TestInitialize()
        {
            _memoryCache = ServiceProvider.GetService<IMemoryCache>();
        }

        [TestMethod]
        public void TestMethod1()
        {
            var value = _memoryCache.Get<int>("key");
            _memoryCache.Set("key", 123);
            value = _memoryCache.Get<int>("key");
            Assert.AreEqual(123, value);
        }
    }
}