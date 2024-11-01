using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WBC66.WeiXin.Core.Models
{
    /// <summary>
    /// 微信AccessToken返回
    /// </summary>
    public class AccessToken
    {
        /// <summary>
        /// 获取到的凭证
        /// </summary>
        public string access_token { get; set; }

        /// <summary>
        /// 凭证有效时间，单位：秒
        /// </summary>
        public int expires_in { get; set; }
    }
}