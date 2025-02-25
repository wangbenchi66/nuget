namespace Easy.EF.Core
{
    /// <summary>
    /// EF配置文件
    /// </summary>
    public class EFOptions
    {
        /// <summary>
        /// 数据库字符串
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// 数据库类型 MySql = 0,SqlServer = 1,
        /// </summary>
        public int DbType { get; set; }
    }
}