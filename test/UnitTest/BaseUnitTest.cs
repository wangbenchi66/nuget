﻿using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar.IOC;
using UnitTest.Repository;
using Easy.SqlSugar.Core;

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
            builder.Services.AddSingleton<IUserRepository, UserRepository>();
            ////SqlSugar
            builder.Services.AddSqlSugarIocSetup(new List<IocConfig>()
                {
                    new IocConfig()
                    {
                        ConfigId = "journal",
                        ConnectionString = "server=localhost;Database=journal;Uid=root;Pwd=123456;allowPublicKeyRetrieval=true;",
                        DbType = IocDbType.MySql,
                        IsAutoCloseConnection = true
                    }
                }, true);

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