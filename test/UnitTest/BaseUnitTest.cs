using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Easy.SqlSugar.Core;
using SqlSugar;
using WBC66.Autofac.Core;

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
            builder.Services.AddRegisterDependencies();
            //SqlSugar
            var list = configuration.GetSection("DBS").Get<List<ConnectionConfig>>();
            builder.Services.AddSqlSugarIocSetup(list);

            //缓存
            // builder.Services.AddMemoryCacheSetup();
            // builder.Services.AddRedisCacheSetup();

            //微信
            /*var weiXinOptions = new WeiXinOptions()
            {
                AppId = "",
                AppSecret = ""
            };
            builder.Services.AddWeiXinService(weiXinOptions);*/

            var app = builder.Build();
            //app.UseSerilogSetup();
            ServiceProvider = app.Services;
        }

        protected static readonly IServiceProvider ServiceProvider;
    }
}