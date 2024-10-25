using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace UnitTest
{
    [TestClass]
    public class SerilogTest : BaseUnitTest
    {
        private ILogger<SerilogTest> _logger;

        [TestInitialize]
        public void TestInitialize()
        {
            _logger = ServiceProvider.GetRequiredService<ILogger<SerilogTest>>();
        }

        [TestMethod]
        public void TestMethod1()
        {
            _logger.LogInformation("日志测试：{time}", DateTime.Now);
            _logger.LogWarning("日志测试：{time}", DateTime.Now);
            _logger.LogError("日志测试：{time}", DateTime.Now);
        }
    }
}