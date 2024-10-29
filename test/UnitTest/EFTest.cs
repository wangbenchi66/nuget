using Microsoft.Extensions.DependencyInjection;
using UnitTest.Repository;

namespace UnitTest
{
    [TestClass]
    public class EFTest : BaseUnitTest
    {
        private IUserEFRepository _repository;

        public EFTest()
        {
            _repository = ServiceProvider.GetRequiredService<IUserEFRepository>();
        }

        [TestMethod]
        public async Task TestMethod1()
        {
            var entity = await _repository.GetSingleAsync(e => e.Id == 1);
            Assert.IsNotNull(entity);
        }
    }
}