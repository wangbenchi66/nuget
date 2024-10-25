using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Serilog.Core;

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

            var app = builder.Build();
            ServiceProvider = app.Services;
        }

        protected static readonly IServiceProvider ServiceProvider;
    }
}