using WBC66.WeiXin.Core.Models;

namespace WBC66.WeiXin.Core
{
    /// <summary>
    /// 微信扩展
    /// </summary>
    public interface IWeiXinExtensions
    {
        /// <summary>
        /// 获取AccessToken(不带任何缓存,考虑到分布式环境，所以不适合使用内存缓存)
        /// </summary>
        /// <returns>返回包含AccessToken的对象</returns>
        AccessToken GetAccessToken();

        /// <summary>
        /// 获取微信信息
        /// </summary>
        /// <param name="access_token">访问令牌</param>
        /// <returns>返回包含微信信息的对象</returns>
        WxUserInfo GetWeiXinInfo(string? access_token);

        /// <summary>
        /// 扫码获取微信信息
        /// </summary>
        /// <param name="openId">用户的OpenID</param>
        /// <param name="access_token">访问令牌</param>
        /// <returns>返回包含微信信息的对象</returns>
        WeiXinInfo GetWeiXinInfoByCode(string openId, string? access_token);

        /// <summary>
        /// 获取授权地址
        /// </summary>
        /// <param name="redirect_uri">重定向URI</param>
        /// <param name="state">状态参数</param>
        /// <param name="scope">授权作用域</param>
        /// <returns>返回授权地址</returns>
        string GetAuthorizeUrl(string redirect_uri, string state = "STATE", string scope = "wechat_redirect");

        /// <summary>
        /// 创建菜单
        /// </summary>
        /// <param name="menu">菜单内容，格式参考:https://developers.weixin.qq.com/doc/offiaccount/Custom_Menus/Creating_Custom-Defined_Menu.html</param>
        /// <param name="access_token">访问令牌</param>
        /// <returns>返回包含创建结果的对象</returns>
        WeiXinRes CreateMenu(string menu, string? access_token);

        /// <summary>
        /// 查询菜单
        /// </summary>
        /// <param name="access_token">访问令牌</param>
        /// <returns>菜单内容：https://developers.weixin.qq.com/doc/offiaccount/Custom_Menus/Querying_Custom_Menus.html</returns>
        string GetMenu(string? access_token);

        /// <summary>
        /// 获取自定义菜单配置
        /// </summary>
        /// <param name="access_token">访问令牌</param>
        /// <returns>返回自定义菜单配置</returns>
        string GetCurrentSelfMenuInfo(string? access_token);

        /// <summary>
        /// 解析文本消息
        /// </summary>
        /// <param name="xml">XML格式的文本消息</param>
        /// <returns>返回解析后的动态对象</returns>
        dynamic GetTextMessage(string xml);
    }
}