namespace WBC66.WeiXin.Core
{
    /// <summary>
    /// 微信配置
    /// </summary>
    public class WeiXinOptions
    {
        /// <summary>
        /// appID
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// AppSecret
        /// </summary>
        public string AppSecret { get; set; }
    }

    public class WeiXinInfo
    {
        public static WeiXinOptions Options { get; set; }
    }
}