using WebApi.Test.Service;

namespace WebApi.Test.Apis
{
    public class IpApis : BaseApi
    {
        private readonly IpService _ipService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public IpApis(IpService ipService, IHttpContextAccessor httpContextAccessor)
        {
            _ipService = ipService;
            _httpContextAccessor = httpContextAccessor;
        }

        public ApiResult GetRegion()
        {
            //获取请求的ip
            string ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString();

            // 如果使用了反向代理（如 nginx），可尝试取 X-Forwarded-For 头部
            if (_httpContextAccessor.HttpContext.Request.Headers.TryGetValue("X-Forwarded-For", out var headerIp))
            {
                ip = headerIp.ToString().Split(',')[0]; // 多个 IP 时取第一个
            }
            return _ipService.GetRegion(ip) switch
            {
                null => ApiResult.Fail("IP地址不合法"),
                var region => ApiResult.Ok(region)
            };
        }
    }
}