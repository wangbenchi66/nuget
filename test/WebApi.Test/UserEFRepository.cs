using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WBC66.EF.Core.BaseProvider;

namespace UnitTest.Repository
{
    /// <summary>
    /// 用户表
    ///</summary>
    [Table("test_user")]
    public class UserEF
    {
        /// <summary>
        ///用户id
        ///</summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        ///用户名称
        ///</summary>
        public string? Name { get; set; }
    }

    /// <summary>
    /// 用户仓储
    /// </summary>
    public class UserEFRepository : BaseRepository<TestDBContext, UserEF>, IUserEFRepository
    {
        public UserEFRepository(TestDBContext context) : base(context)
        {
        }
    }

    /// <summary>
    /// 用户仓储接口层
    /// </summary>
    public interface IUserEFRepository : IBaseRepository<TestDBContext, UserEF>
    {
    }
}