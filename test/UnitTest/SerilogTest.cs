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
            _logger.LogInformation("��־���ԣ�{time}", DateTime.Now);
            _logger.LogWarning("��־���ԣ�{time}", DateTime.Now);
            _logger.LogError("��־���ԣ�{time}", DateTime.Now);
        }
    }
}