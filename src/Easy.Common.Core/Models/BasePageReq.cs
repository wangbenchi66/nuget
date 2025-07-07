using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easy.Common.Core.Models
{
    /// <summary>
    /// 分页请求基类
    /// </summary>
    public class BasePageReq
    {
        [DefaultValue(1)]
        public int Page { get; set; } = 1;

        [DefaultValue(10)]
        public int PageSize { get; set; } = 10;
    }
}
