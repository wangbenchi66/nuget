using Microsoft.Extensions.DependencyInjection;

namespace WBC66.WeiXin.Core
{
    /// <summary>
    /// 微信服务
    /// </summary>
    public static class WeiXinSetup
    {
        /// <summary>
        /// 添加微信服务
        /// </summary>
        public static void AddWeiXinService(this IServiceCollection services, WeiXinOptions options)
        {
            if (services == null) { throw new ArgumentNullException(nameof(services)); }
            if (options == null) { throw new ArgumentNullException(nameof(options)); }
            WeiXinInfo.Options = options;
            services.AddSingleton<IHttpHelper, HttpHelper>();
            services.AddSingleton<IWeiXinExtensions, WeiXinExtensions>();
        }
    }
}