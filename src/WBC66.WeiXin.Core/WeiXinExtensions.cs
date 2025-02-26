using WBC66.WeiXin.Core.Models;

namespace WBC66.WeiXin.Core
{
    /// <summary>
    /// 微信扩展
    /// </summary>
    public class WeiXinExtensions : IWeiXinExtensions
    {
        private readonly IHttpHelper _httpClient;
        private static string _appId = WeiXinInfo.Options.AppId;
        private static string _appSecret = WeiXinInfo.Options.AppSecret;

        public WeiXinExtensions(IHttpHelper httpClient)
        {
            _httpClient = httpClient;
            if (_appId.IsNull() || _appSecret.IsNull())
                throw new ArgumentNullException("请检查是否配置微信AppId和AppSecret");
        }

        /// <summary>
        /// 获取AccessToken(不带任何缓存,考虑到分布式环境，所以不适合使用内存缓存)
        /// </summary>
        /// <returns>返回包含AccessToken的对象</returns>
        public AccessToken GetAccessToken()
        {
            var url = $"https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={_appId}&secret={_appSecret}";
            return _httpClient.Get<AccessToken>(url);
        }

        #region 微信网页开发
        //获取微信授权地址 
        /*
        scope为snsapi_base：
https://open.weixin.qq.com/connect/oauth2/authorize?appid=wx520c15f417810387&redirect_uri=https%3A%2F%2Fchong.qq.com%2Fphp%2Findex.php%3Fd%3D%26c%3DwxAdapter%26m%3DmobileDeal%26showwxpaytitle%3D1%26vb2ctag%3D4_2030_5_1194_60&response_type=code&scope=snsapi_base&state=123#wechat_redirect
scope为snsapi_userinfo：
https://open.weixin.qq.com/connect/oauth2/authorize?appid=wx807d86fb6b3d4fd2&redirect_uri=http%3A%2F%2Fdevelopers.weixin.qq.com&response_type=code&scope=snsapi_userinfo&state=STATE#wechat_redirect
        */

        public string GetAuthorizeUrl(string redirect_uri, string state = "STATE", string scope = "snsapi_base")
        {
            return $"https://open.weixin.qq.com/connect/oauth2/authorize?appid={_appId}&redirect_uri={redirect_uri}&response_type=code&scope={scope}&state={state}#wechat_redirect";
        }

        #endregion 微信网页开发

        /// <summary>
        /// 获取微信信息
        /// </summary>
        /// <param name="access_token">访问令牌</param>
        /// <returns>返回包含微信信息的对象</returns>
        public WxUserInfo GetWeiXinInfo(string? access_token)
        {
            if (access_token.IsNull())
                access_token = GetAccessToken().access_token;
            var url = $"https://api.weixin.qq.com/sns/oauth2/access_token?appid={_appId}&secret={_appSecret}&code={access_token}&grant_type=authorization_code";
            var res = _httpClient.Get<dynamic>(url);
            url = $"https://api.weixin.qq.com/sns/userinfo?access_token={res.access_token}&openid={res.openid}&lang=zh_CN";
            return _httpClient.Get<WxUserInfo>(url);
        }

        /// <summary>
        /// 扫码获取微信信息
        /// </summary>
        /// <param name="openId">用户的OpenID</param>
        /// <param name="access_token">访问令牌</param>
        /// <returns>返回包含微信信息的对象</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public WeiXinInfo GetWeiXinInfoByCode(string openId, string? access_token)
        {
            if (openId.IsNull())
                throw new ArgumentNullException("openId不能为空");
            if (access_token.IsNull())
                access_token = GetAccessToken().access_token;
            var url = $" https://api.weixin.qq.com/cgi-bin/user/info?access_token={access_token}&openid={openId}&lang=zh_CN";
            return _httpClient.Get<WeiXinInfo>(url);
        }

        #region 自定义菜单

        /// <summary>
        /// 创建菜单
        /// </summary>
        /// <param name="menu">菜单内容，格式参考:https://developers.weixin.qq.com/doc/offiaccount/Custom_Menus/Creating_Custom-Defined_Menu.html</param>
        /// <param name="access_token">访问令牌</param>
        /// <returns>返回包含创建结果的对象</returns>
        public WeiXinRes CreateMenu(string menu, string? access_token)
        {
            if (access_token.IsNull())
                access_token = GetAccessToken().access_token;
            var url = $"https://api.weixin.qq.com/cgi-bin/menu/create?access_token={access_token}";
            return _httpClient.Post<WeiXinRes>(url, menu);
        }

        /// <summary>
        /// 查询菜单
        /// </summary>
        /// <param name="access_token"></param>
        /// <returns>菜单内容：https://developers.weixin.qq.com/doc/offiaccount/Custom_Menus/Querying_Custom_Menus.html</returns>
        public string GetMenu(string? access_token)
        {
            if (access_token.IsNull())
                access_token = GetAccessToken().access_token;
            var url = $"https://api.weixin.qq.com/cgi-bin/get_current_selfmenu_info?access_token=={access_token}";
            return _httpClient.Get<string>(url);
        }

        /// <summary>
        /// 删除菜单
        /// </summary>
        /// <param name="access_token"></param>
        /// <returns></returns>
        public WeiXinRes DeleteMenu(string? access_token)
        {
            if (access_token.IsNull())
                access_token = GetAccessToken().access_token;
            var url = $"https://api.weixin.qq.com/cgi-bin/menu/delete?access_token={access_token}";
            return _httpClient.Get<WeiXinRes>(url);
        }

        //事件推送 https://developers.weixin.qq.com/doc/offiaccount/Custom_Menus/Custom_Menu_Push_Events.html

        //个性化菜单接口 https://developers.weixin.qq.com/doc/offiaccount/Custom_Menus/Personalized_menu_interface.html

        /// <summary>
        /// 获取自定义菜单配置
        /// </summary>
        /// <param name="access_token"></param>
        /// <returns></returns>
        public string GetCurrentSelfMenuInfo(string? access_token)
        {
            if (access_token.IsNull())
                access_token = GetAccessToken().access_token;
            var url = $"https://api.weixin.qq.com/cgi-bin/menu/get?access_token={access_token}";
            return _httpClient.Get<string>(url);
        }

        #endregion 自定义菜单
        # region 基础消息能力
        # region 接收普通消息

        //解析文本消息
        public dynamic GetTextMessage(string xml)
        {
            /*
            <xml>
  <ToUserName><![CDATA[toUser]]></ToUserName>
  <FromUserName><![CDATA[fromUser]]></FromUserName>
  <CreateTime>1348831860</CreateTime>
  <MsgType><![CDATA[text]]></MsgType>
  <Content><![CDATA[this is a test]]></Content>
  <MsgId>1234567890123456</MsgId>
  <MsgDataId>xxxx</MsgDataId>
  <Idx>xxxx</Idx>
</xml>
            */
            //将xml转换为对象
            var res = JsonHelper.ToObject<dynamic>(xml);
            return res;
        }

        #endregion

        #endregion
    }
}