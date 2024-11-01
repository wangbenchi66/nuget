using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WBC66.WeiXin.Core.Models
{
    /// <summary>
    /// 微信错误信息
    /// </summary>
    public class WeiXinRes
    {
        /// <summary>
        /// 错误码
        /// </summary>
        public int errcode { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string errmsg { get; set; }

        /// <summary>
        /// 是否发生错误
        /// </summary>
        public bool IsError { get { return errcode != 0; } }
    }
}