using Easy.Common.Core;
using IGeekFan.AspNetCore.Knife4jUI;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using WBC66.Autofac.Core;
using WBC66.Cache.Core;
using WBC66.Core;
using WBC66.Core.Filters;
using WBC66.Serilog.Core;
using Easy.SqlSugar.Core;
using WebApi.Test.Filter;
using Microsoft.Extensions.Caching.Memory;
using Easy.DynamicApi;
using Microsoft.AspNetCore.Mvc.Controllers;
using Easy.EF.Core.BaseProvider;
using Easy.EF.Core;
using WebApi.Test.Apis;
using Microsoft.EntityFrameworkCore;
using SqlSugar.IOC;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
builder.Services.AddSwaggerGen(s =>
{
    var xmlsFiles = Directory.GetFiles(AppContext.BaseDirectory, "*.xml");
    foreach (var xmlFile in xmlsFiles)
    {
        s.IncludeXmlComments(xmlFile);
    }
    //设置swagger文档信息，如果没有注释则显示默认信息
    s.CustomOperationIds(apiDesc =>
    {
        var controllerAction = apiDesc.ActionDescriptor as ControllerActionDescriptor;
        if (controllerAction == null)
            return apiDesc.RelativePath;
        return controllerAction.ControllerName + "-" + (controllerAction.ActionName.IsNull() ? controllerAction.MethodInfo.Name : controllerAction.ActionName);
    });
});
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
//builder.Services.AddMemoryCacheSetup();

//使用autofac(内部会自动批量注入Service、Repository、Dao结尾的类  所有继承ITransient、ISingleton、IScoped接口的类注入到容器中)
builder.Host.AddAutofacHostSetup(builder.Services, options =>
{
    //开启内存缓存拦截器(带有IProxyService接口的类将会被拦截),带有CacheResultAttribute特性的方法将会被缓存
    //options.AddMemoryCacheResultAop();
});
builder.Services.AddRegisterDependencies();
//注入IBaseSqlSugarRepository,泛型

//sqlsugar+内存缓存
//builder.Services.AddMemoryCacheSetup();
//MemoryCacheService cache = new MemoryCacheService();
//ICacheService cacheService = cache;

//sqlsugar+reids缓存
//CSRedisClient client = new CSRedisClient("192.168.21.235:6379,password=123456,defaultDatabase=1,poolsize=50,prefix=t1111");
//CsRedisCacheService cacheService = new CsRedisCacheService(client);
//ICacheService cache = cacheService;
var list = configuration.GetSection("DBS").Get<List<ConnectionConfig>>();
foreach (var item in list)
{
    //开启全自动清理，调用增删改会自动清除缓存
    //item.MoreSettings = new ConnMoreSettings()
    //{
    //    IsAutoRemoveDataCache = true
    //};
    //item.ConfigureExternalServices = new ConfigureExternalServices()
    //{
    //    //使用缓存策略,使用内存缓存\redis缓存\分布式缓存
    //    //如果开启缓存需要重写BaseSqlSugarRepository中的查询方法才能生效,或者使用db上下文查询中加入WithCache()
    //    DataInfoCacheService = cacheService
    //};
    //日志输出
    //#if DEBUG
    //    item.AopEvents = new AopEvents()
    //    {
    //        OnLogExecuting = (sql, pars) =>
    //        {
    //            Console.WriteLine($"{new string('-', 30)}{Environment.NewLine}{DateTime.Now},ConfigId:{item.ConfigId},DBType:{item.DbType},Sql:{Environment.NewLine}{UtilMethods.GetSqlString((SqlSugar.DbType)item.DbType, sql, pars)}{Environment.NewLine}{new string('-', 30)}");
    //        },
    //        OnError = (exp) =>
    //        {
    //            Console.WriteLine($"{new string('-', 30)}{Environment.NewLine}{DateTime.Now},ConfigId:{item.ConfigId},DBType:{item.DbType},Error:{Environment.NewLine}{exp.Message}{Environment.NewLine}{new string('-', 30)}");
    //        }
    //    };
    //#endif
}
var sqlSugarScope = new SqlSugarScope(list, db =>
{
    var configId = db.CurrentConnectionConfig.ConfigId;
    var dbType = db.CurrentConnectionConfig.DbType;
    //输出数据执行位置来定位和排查数据
    var stackTraceList = db.Ado.SqlStackTrace.MyStackTraceList.FindAll(x => x.Line > 0);
    StringBuilder stringBuilder = new StringBuilder();
    foreach (var item in stackTraceList)
    {
        var fileName = item.FileName.Split('\\')[^3..].Aggregate((x, y) => x + "\\" + y);
        stringBuilder.Append($"{Environment.NewLine}位置:{fileName},行号:{item.Line}");
    }

    db.Aop.OnLogExecuting = (sql, p) =>
    {
        Console.WriteLine($"{new string('-', 30)}{Environment.NewLine}{DateTime.Now},ConfigId:{configId},{stringBuilder.ToString()},Sql:{Environment.NewLine}{UtilMethods.GetSqlString(dbType, sql, p)}{Environment.NewLine}{new string('-', 30)}");

    };
    db.Aop.OnError = (exp) =>
    {
        Console.WriteLine($"{new string('-', 30)}错误{Environment.NewLine}{DateTime.Now},ConfigId:{configId},{stringBuilder.ToString()},Sql:{Environment.NewLine}{exp.Sql}{Environment.NewLine}Error:{exp.Message}{Environment.NewLine}{new string('-', 30)}");
    };
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddSqlSugarScopedSetup(sqlSugarScope);
/*var listIoc = configuration.GetSection("DBS").Get<List<IocConfig>>();
builder.Services.AddSqlSugarIocSetup(listIoc);*/

/*
//EF
EFOptions efOptions = new EFOptions()
{
    DbType = 0,
    ConnectionString = configuration.GetSection("DBS:0:ConnectionString").Value
};
builder.Services.AddEFSetup<TestDBContext>(efOptions);
builder.Services.AddSingleton(typeof(IBaseEFRepository<,>), typeof(BaseEFRepository<,>));
builder.Services.AddSingleton<IUserEFRepository, UserEFRepository>();*/


builder.Services.AddControllers(options =>
{
    //添加自定义的模型验证过滤器
    options.Filters.Add<ValidateModelAttribute>();
    //添加自定义的缓存过滤器
    //options.Filters.Add<CacheResultFilter>();
    //添加幂等性过滤器
    //options.Filters.Add<HttpIdempotenceFilter>();
});
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    //禁用默认的模型验证过滤器,否则无法返回自定义的错误信息
    options.SuppressModelStateInvalidFilter = true;
});
//动态api
builder.Services.AddDynamicApi();
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
//app.UseMiddleware<LogMiddleware>();//添加日志中间件
//app.UseMiddleware<ExceptionMiddleware>();//添加异常处理中间件
//app.UseMiddleware<CurrentLimitingMiddleware>(1, 1);//添加限流中间件 1个线程 1个并发
//app.UseMiddleware<IdempotenceMiddleware>();//添加幂等性中间件

app.Run();