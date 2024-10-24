﻿using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UnitTest.Repository;
using WBC66.Serilog.Core;
using WBC66.SqlSugar.Core;

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
            builder.Host.AddSerilogHost(configuration);

            //NLog
            //builder.AddNLogSteup();
            //注入
            builder.Services.AddSingleton<IUserRepository, UserRepository>();
            //SqlSugar
            builder.Services.AddSqlSugarSetup(configuration);

            var app = builder.Build();
            app.UseSerilogSetup();
            ServiceProvider = app.Services;
        }

        protected static readonly IServiceProvider ServiceProvider;
    }
}