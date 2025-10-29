/*using System.ComponentModel.DataAnnotations.Schema;
using Easy.EF.Core.BaseProvider;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Z.EntityFramework.Plus;

namespace WebApi.Test.Apis
{
    public class EFApis : BaseApi
    {
        private readonly IUserEFRepository _userRepository;

        public EFApis(IUserEFRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public object Get()
        {
            return _userRepository.GetSingle(p => p.Id == 1);
        }

        public object GetList()
        {
            return _userRepository.GetList(p => true);
        }

        public object Add1()
        {
            *//*var user = _userRepository.EFContext.UserEF.Add(new UserEF { Name = "test" });
            _userRepository.EFContext.SaveChanges();
            return user.Entity.Id;*//*
            return _userRepository.InsertReturnIdentity(new UserEF { Name = "test" });
        }

        public object Update1()
        {
            var user = new UserEF { Id = 1, Name = "自定义", CreateTime = DateTime.Now };
            //return _userRepository.Update(new UserEF { Id = 1, Name = "自定义" }, true);
            //只更新某列
            //return _userRepository.Update(user, x => new { x.Name, x.CreateTime }, true);
            return _userRepository.Update(user, x => new { x.Name, x.CreateTime }, x => x.Id, false);
        }

        public object UpdateSetColumn()
        {
            var user = new UserEF { Id = 1, Name = "自定义", CreateTime = DateTime.Now };
            return _userRepository.Update(user, x => new { x.Name, x.CreateTime }, true);
        }

        public object Update_SetColumn_WhereColumn()
        {
            var user = new UserEF { Name = "自定义", CreateTime = DateTime.Now };
            return _userRepository.Update(user, x => new { x.CreateTime }, x => x.Name, true);
        }

        public object DeleteById()
        {
            return _userRepository.EFContext.UserEF.Where(p => p.Id == 100010).Delete();
        }

        public object QueryPage()
        {
            return _userRepository.QueryPage(p => true, "", 1, 10);
        }
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
            optionsBuilder.ConfigureWarnings(warnings => warnings.Throw(RelationalEventId.QueryPossibleUnintendedUseOfEqualsWarning));
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
        public DateTime CreateTime { get; set; }
    }
}*/