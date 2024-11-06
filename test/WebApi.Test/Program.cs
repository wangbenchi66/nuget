using IGeekFan.AspNetCore.Knife4jUI;
using Microsoft.AspNetCore.Mvc;
using SqlSugar.IOC;
using UnitTest.Repository;
using WBC66.SqlSugar.Core;
using WebApi.Test.Filter;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
builder.Services.AddSwaggerGen();
//Serilog
//builder.Host.AddSerilogHost(configuration);

//NLong
//builder.AddNLogSteup(configuration);
// Add services to the container.

//获取配置文件
//var efOptions = configuration.GetSection("DBS").Get<List<EFOptions>>()[0];
//builder.Services.AddEFSetup<TestDBContext>(efOptions);
//builder.Services.AddScoped<IUserEFRepository, UserEFRepository>();
//注入
builder.Services.AddSingleton<IUserRepository, UserRepository>();
//SqlSugar
builder.Services.AddSqlSugarSetup(configuration.GetSection("DBS").Get<List<IocConfig>>(), true, config =>
{
    config.Aop.OnLogExecuting = (sql, pars) =>
    {
        Console.WriteLine("这是自定义事件{0}", sql);
    };
});
builder.Services.AddControllers(options =>
{
    //添加自定义的模型验证过滤器
    options.Filters.Add<ValidateModelAttribute>();
});
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    //禁用默认的模型验证过滤器,否则无法返回自定义的错误信息
    options.SuppressModelStateInvalidFilter = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
//app.UseAuthorization();
app.UseSwagger(c =>
{
    c.RouteTemplate = "api/swagger/{documentName}/swagger.json";
});
app.UseKnife4UI(c =>
{
    c.SwaggerEndpoint("../api/swagger/v1/swagger.json", "api");
    c.RoutePrefix = "k4j";
});
app.MapControllers();

app.Run();