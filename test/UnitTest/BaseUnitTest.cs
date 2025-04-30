using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Easy.SqlSugar.Core;
using SqlSugar;
using WBC66.Autofac.Core;
using Microsoft.Extensions.DependencyInjection;
using Easy.EF.Core;
using static UnitTest.EFTest;
using Autofac.Core;
using Easy.EF.Core.BaseProvider;

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
            //builder.Host.AddSerilogHostJson(configuration);

            //NLog
            //builder.AddNLogSteup();
            //注入
            //builder.Services.AddSingleton<IUserRepository, UserRepository>();
            builder.Host.AddAutofacHostSetup(builder.Services, options =>
            {
                //开启内存缓存拦截器(带有IProxyService接口的类将会被拦截),带有CacheResultAttribute特性的方法将会被缓存
                //options.AddMemoryCacheResultAop();
            });
            //SqlSugar
            //var list = configuration.GetSection("DBS").Get<List<ConnectionConfig>>();
            //builder.Services.AddSqlSugarIocSetup(list);


            EFOptions efOptions = new EFOptions()
            {
                DbType = 0,
                ConnectionString = configuration.GetSection("DBS:0:ConnectionString").Value
            };
            builder.Services.AddEFSetup<TestDBContext>(efOptions);
            //注入IBaseEFRepository,BaseEFRepository
            builder.Services.AddSingleton(typeof(IBaseEFRepository<,>), typeof(BaseEFRepository<,>));
            builder.Services.AddSingleton<IUserEFRepository, UserEFRepository>();

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

            //builder.Services.AddEf

            var app = builder.Build();
            //app.UseSerilogSetup();
            ServiceProvider = app.Services;
        }


        protected static T GetService<T>()
        {
            using var scope = ServiceProvider.CreateScope();
            return scope.ServiceProvider.GetRequiredService<T>();
        }


        protected static readonly IServiceProvider ServiceProvider;
    }
}