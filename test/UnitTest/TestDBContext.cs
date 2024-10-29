using Microsoft.EntityFrameworkCore;
using UnitTest.Repository;

namespace UnitTest
{
    public class TestDBContext : DbContext
    {
        public TestDBContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<UserEF> Users { get; set; }
    }
}