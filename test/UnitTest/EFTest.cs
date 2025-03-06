using System.ComponentModel.DataAnnotations.Schema;
using Easy.EF.Core.BaseProvider;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace UnitTest
{
    [TestClass]
    public class EFTest : BaseUnitTest
    {
        private IUserEFRepository _userRepository;

        [TestInitialize]
        public void Initialize()
        {
            _userRepository = ServiceProvider.GetRequiredService<IUserEFRepository>();
        }


        [TestMethod]
        public async Task Get()
        {

            //所有操作都有异步方法，增加Async即可
            //添加、修改、删除可以设置是否立即保存，然后调用SaveChanges()方法说动保存
            //查询单个
            var obj = _userRepository.GetSingle(p => p.Id == 1);
        }

        public class TestDBContext : DbContext
        {
            /// <summary>
            /// 日志工厂
            /// </summary>
            public static readonly ILoggerFactory _loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddFilter((category, level) => category == DbLoggerCategory.Database.Command.Name && level == LogLevel.Information)
                    .AddConsole();
            });

            public TestDBContext(DbContextOptions<TestDBContext> options) : base(options)
            {
            }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseLoggerFactory(_loggerFactory);
                optionsBuilder.EnableSensitiveDataLogging();
                base.OnConfiguring(optionsBuilder);
            }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);
            }

            public DbSet<UserEF> UserEF { get; set; }
        }

        public class UserEFRepository : BaseEFRepository<TestDBContext, UserEF>, IUserEFRepository
        {
            public UserEFRepository(TestDBContext context) : base(context)
            {
            }
        }

        /// <summary>
        /// 用户仓储接口层
        /// </summary>
        public interface IUserEFRepository : IBaseEFRepository<TestDBContext, UserEF>
        {
        }

        [Table("j_user")]
        public class UserEF
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}