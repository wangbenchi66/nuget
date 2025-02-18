using IGeekFan.AspNetCore.Knife4jUI;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using WBC66.Autofac.Core;
using WBC66.Cache.Core;
using WBC66.Core;
using WBC66.Core.Filters;
using WBC66.Serilog.Core;
using Easy.SqlSugar.Core;
using WebApi.Test;
using WebApi.Test.Filter;
using Microsoft.Extensions.DependencyInjection;
using Autofac;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
builder.Services.AddSwaggerGen();
//Serilog
builder.Host.AddSerilogHost(configuration);

//NLong
//builder.AddNLogSteup(configuration);
// Add services to the container.

//获取配置文件
//var efOptions = configuration.GetSection("DBS").Get<List<EFOptions>>()[0];
//builder.Services.AddEFSetup<TestDBContext>(efOptions);
//builder.Services.AddScoped<IUserEFRepository, UserEFRepository>();
//注入
//builder.Services.AddSingleton<ICategoryRepository, CategoryRepository>();
//builder.Services.AddSingleton<IUserService, UserService>();

//开启内存缓存
builder.Services.AddMemoryCacheSetup();

//使用autofac(内部会自动批量注入Service、Repository、Dao结尾的类  所有继承ITransient、ISingleton、IScoped接口的类注入到容器中)
builder.Host.AddAutofacHostSetup(builder.Services, options =>
{
    //开启内存缓存拦截器(带有IProxyService接口的类将会被拦截),带有CacheResultAttribute特性的方法将会被缓存
    //options.AddMemoryCacheResultAop();
});
//builder.Services.AddRegisterDependencies();

//builder.Services.AddSingleton<CategoryRepository>();

//SqlSugar
var list = configuration.GetSection("DBS").Get<List<ConnectionConfig>>();
foreach (var item in list)
{
    //日志输出
#if DEBUG
    item.AopEvents = new AopEvents()
    {
        OnLogExecuting = (sql, pars) =>
        {
            //Console.WriteLine($"{DateTime.Now},ConfigId:{item.ConfigId}{Environment.NewLine}Sql:{UtilMethods.GetSqlString(DbType.MySql, sql, pars)}");
            Console.WriteLine($"----------------{Environment.NewLine}{DateTime.Now},ConfigId:{item.ConfigId},Sql:{Environment.NewLine}{UtilMethods.GetSqlString((SqlSugar.DbType)item.DbType, sql, pars)}{Environment.NewLine}----------------");
        }
    };
}
#endif
builder.Services.AddSqlSugarScopedSetup(list);

builder.Services.AddControllers(options =>
{
    //添加自定义的模型验证过滤器
    options.Filters.Add<ValidateModelAttribute>();
    //添加自定义的缓存过滤器
    options.Filters.Add<CacheResultFilter>();
    //添加幂等性过滤器
    options.Filters.Add<HttpIdempotenceFilter>();
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
app.UseMiddleware<LogMiddleware>();//添加日志中间件
app.UseMiddleware<ExceptionMiddleware>();//添加异常处理中间件
app.UseMiddleware<CurrentLimitingMiddleware>(1, 1);//添加限流中间件 1个线程 1个并发
//app.UseMiddleware<IdempotenceMiddleware>();//添加幂等性中间件

app.Run();