using Easy.DynamicApi.Attributes;
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

        public ApiResult GetRegion(string? ip)
        {
            if (!ip.IsNull())
            {
                return _ipService.GetRegion(ip) switch
                {
                    null => ApiResult.Fail("IP地址不合法"),
                    var region => ApiResult.Ok(FormatRegion(region))
                };
            }
            //获取请求的ip
            ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString();

            // 如果使用了反向代理（如 nginx），可尝试取 X-Forwarded-For 头部
            if (_httpContextAccessor.HttpContext.Request.Headers.TryGetValue("X-Forwarded-For", out var headerIp))
            {
                ip = headerIp.ToString().Split(',')[0]; // 多个 IP 时取第一个
            }
            return _ipService.GetRegion(ip) switch
            {
                null => ApiResult.Fail("IP地址不合法"),
                var region => ApiResult.Ok(FormatRegion(region))
            };
        }

        /// <summary>
        /// 格式化ip地址
        /// </summary>
        /// <param name="region"></param>
        /// <returns>将中国|0|湖北省|孝感市|电信转换为中国湖北省孝感市 不要|0|的数据和 网络类型</returns>
        [NonDynamicApi]
        private static string FormatRegion(string region)
        {
            if (region.IsNull())
                return string.Empty;
            var parts = region.Split('|');
            parts = parts.Where(p => p != "0" && !p.Contains("电信") && !p.Contains("联通") && !p.Contains("移动")).Distinct().ToArray();
            return string.Join("", parts);
        }
    }
}