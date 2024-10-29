using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace WBC66.EF.Core
{
    /// <summary>
    /// EF服务
    /// </summary>
    public static class EFSetup
    {
        /// <summary>
        /// 添加EF配置
        /// </summary>
        /// <param name="services"></param>
        /// <param name="efOptions"></param>
        /// <typeparam name="TDbContext"></typeparam>
        public static void AddEFSetup<TDbContext>(this IServiceCollection services, EFOptions efOptions) where TDbContext : DbContext
        {
            if (services == null) { throw new ArgumentNullException(nameof(services)); }
            if (efOptions == null)
                throw new ArgumentNullException(nameof(efOptions), "EF配置不能为空");
            services.AddDbContext<TDbContext>(options =>
            {
                switch (efOptions.DbType)
                {
                    case 0:
                        options.UseMySql(efOptions.ConnectionString, ServerVersion.AutoDetect(efOptions.ConnectionString));
                        break;

                    case 1:
                        options.UseSqlServer(efOptions.ConnectionString);
                        break;

                    default:
                        throw new Exception("暂不支持的数据库类型");
                }
            });
        }
    }
}