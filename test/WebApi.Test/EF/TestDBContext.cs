using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace WebApi.Test.EF
{
    /// <summary>
    /// 测试db
    /// </summary>
    public class TestDBContext : DbContext
    {
        /// <summary>
        /// 日志工厂
        /// </summary>
        public static readonly ILoggerFactory _loggerFactory = LoggerFactory.Create(builder =>
        {
            builder
            .AddSerilog()
                .AddFilter((category, level) => category == DbLoggerCategory.Database.Command.Name && level == LogLevel.Information);
        });


        public TestDBContext(DbContextOptions<TestDBContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseLoggerFactory(_loggerFactory)
                .EnableSensitiveDataLogging();
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<UserEF> UserEF { get; set; }
    }

    [Table("j_user")]
    public class UserEF
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}