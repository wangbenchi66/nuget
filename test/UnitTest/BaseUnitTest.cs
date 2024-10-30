using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using WBC66.Cache.Core;

namespace UnitTest
{
    public class BaseUnitTest
    {
        static BaseUnitTest()
        {
            var builder = WebApplication.CreateBuilder();
            builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            var configuration = builder.Configuration;
            //Serilog
            //builder.Host.AddSerilogHost(configuration);

            //NLog
            //builder.AddNLogSteup();
            //注入
            //builder.Services.AddSingleton<IUserRepository, UserRepository>();
            ////SqlSugar
            //builder.Services.AddSqlSugarSetup(configuration);

            //缓存
            builder.Services.AddMemoryCacheSetup();
            builder.Services.AddRedisCacheSetup();

            var app = builder.Build();
            //app.UseSerilogSetup();
            ServiceProvider = app.Services;
        }

        protected static readonly IServiceProvider ServiceProvider;
    }
}