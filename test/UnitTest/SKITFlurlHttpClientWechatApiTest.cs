using SKIT.FlurlHttpClient.Wechat.Api;
using SKIT.FlurlHttpClient.Wechat.Api.Models;

namespace UnitTest
{
    /// <summary>
    /// SKIT.FlurlHttpClient.Wechat.Api nuget包测试
    /// </summary>
    [TestClass]
    public class SKITFlurlHttpClientWechatApiTest : BaseUnitTest
    {
        private readonly WechatApiClient _client;
        public SKITFlurlHttpClientWechatApiTest()
        {
            var options = new WechatApiClientOptions()
            {
                AppId = "",
                AppSecret = ""
            };
            _client = WechatApiClientBuilder.Create(options).Build();
        }
        [TestMethod]
        public async Task TestGetAccessTokenAsync()
        {
            var response = await _client.ExecuteCgibinStableTokenAsync(new CgibinStableTokenRequest()
            {
                //GrantType = "client_credential"
                ForceRefresh = false,
            });

            var userInfoRes = await _client.ExecuteCgibinUserInfoAsync(new CgibinUserInfoRequest()
            {
                AccessToken = response.AccessToken,
                OpenId = ""
            });
            Assert.IsNotNull(userInfoRes);
        }
    }
}