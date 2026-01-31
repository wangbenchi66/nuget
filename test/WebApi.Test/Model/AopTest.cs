using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SqlSugar;

namespace WebApi.Test.Model
{
    /// <summary>
    /// aop事件测试
    ///</summary>
    [Tenant("journal")]
    [SugarTable("aoptest")]
    public class AopTest
    {
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(IsPrimaryKey = true)]
        public long Id { get; set; }
        /// <summary>
        ///  
        /// ///</summary>
        public DateTime? CreateTime { get; set; }
        /// <summary>
        ///  
        ///</summary>
        public DateTime? UpdateTime { get; set; }
    }
}