using Easy.Common.Core;

namespace UnitTest
{
    [TestClass]
    public class CommonTest : BaseUnitTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var json = @"{""name"":""Test"", ""number"": ""1""}";
            var obj = json.ToObject<a>();
            Assert.AreEqual("Test", obj.Name);
            Assert.AreEqual(1, obj.Number);
        }
        public class a
        {
            public int Number { get; set; }
            public string Name { get; set; }
        }
    }
}