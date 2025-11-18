using Easy.DynamicApi;
using Easy.EF.Core;
using Easy.SqlSugar.Core;
using Easy.SqlSugar.Core.Common;
using IGeekFan.AspNetCore.Knife4jUI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Caching.Memory;
using Scalar.AspNetCore;
using Serilog;
using SqlSugar;
using WBC66.Autofac.Core;
using WBC66.Core;
using WBC66.Serilog.Core;
using WebApi.Test.EF;
using WebApi.Test.Filter;
using WebApi.Test.Service;
using Yitter.IdGenerator;

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
//builder.Host.AddSerilogHost(configuration);
//builder.Host.AddSerilogHostJson(configuration);
builder.Host.AddSerilogHost("/home/logger/nuget", Serilog.Events.LogEventLevel.Information);

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
    item.ConnectionString = item.ConnectionString.CheckTrustServerCertificate(item.DbType).CheckEncrypt(item.DbType);
}
var sqlSugarScope = new SqlSugarScope(list, db =>
{
    foreach (var item in list)
    {
        var configId = item.ConfigId;
        var dbType = item.DbType;
        var conn = db.GetConnection(configId);
        var sqlFileInfo = conn.Ado.SqlStackTrace.MyStackTraceList.GetSqlFileInfo();
        var aop = conn.Aop;
        aop.OnLogExecuting = (sql, p) =>
        {
            Log.Information(UniversalExtensions.GetSqlInfoString(configId, sql, p, dbType, sqlFileInfo));
        };
        aop.OnError = (SqlSugarException exp) =>
        {
            Console.WriteLine(UniversalExtensions.GetSqlErrorString(configId, exp, sqlFileInfo));
        };
    }
});
builder.Services.AddHttpContextAccessor();
//builder.Services.AddSqlSugarScopedSetup(sqlSugarScope);
builder.Services.AddSqlSugarScopedSetup(sqlSugarScope);
/*var listIoc = configuration.GetSection("DBS").Get<List<IocConfig>>();
builder.Services.AddSqlSugarIocSetup(listIoc);*/
SnowFlakeSingle.WorkId = 1;
var options = new IdGeneratorOptions
{
    WorkerIdBitLength = 4,  // 最大 16 台机器
    SeqBitLength = 6,       // 每毫秒最多 64 个 ID
    BaseTime = new DateTime(2020, 1, 1)
};

YitIdHelper.SetIdGenerator(options);
StaticConfig.CustomSnowFlakeFunc = () =>
{
    return YitIdHelper.NextId();
};


//EF
EFOptions efOptions = new EFOptions()
{
    DbType = 0,
    ConnectionString = configuration.GetSection("DBS:0:ConnectionString").Value
};
builder.Services.AddEFSetup<TestDBContext>(efOptions);
/*builder.Services.AddSingleton(typeof(IBaseEFRepository<,>), typeof(BaseEFRepository<,>));
builder.Services.AddSingleton<IUserEFRepository, UserEFRepository>();*/
//builder.Services.AddScoped<IUserRepository, UserRepository>();

//Ip2region.Net
builder.Services.AddSingleton<IpService>(_ => new IpService(@".\ip2region.xdb"));


builder.Services.AddControllers(options =>
{
    //添加自定义的模型验证过滤器
    options.Filters.Add<ValidateModelAttribute>();
    options.Filters.Add<ApiCommonResultFilter>();//全局统一返回格式包装
    //添加自定义的缓存过滤器
    //options.Filters.Add<CacheResultFilter>();
    //添加幂等性过滤器
    //options.Filters.Add<HttpIdempotenceFilter>();
}).AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null; // 保留原始属性名
    options.JsonSerializerOptions.WriteIndented = true; // 格式化输出
    //时间格式化
    options.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
}
    );
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    //禁用默认的模型验证过滤器,否则无法返回自定义的错误信息
    options.SuppressModelStateInvalidFilter = true;
});
//动态api
builder.Services.AddDynamicApi();
/*builder.Services.AddReZeroServices(api =>
{
    //有重载可换json文件
    var apiObj = new SuperAPIOptions();
    apiObj!.DependencyInjectionOptions = new DependencyInjectionOptions(Assembly.GetExecutingAssembly());
    //启用超级API
    api.EnableSuperApi(apiObj);//默认载体为sqlite ，有重载可以配置数据库
});
*/

//cap
/*builder.Services.AddCap(x =>
{
    x.UseMySql("server=192.168.21.232;Port=31665;Database=journal;Uid=root;Pwd=123456;allowPublicKeyRetrieval=true;");
    x.UseRabbitMQ(opt =>
    {
        opt.HostName = "192.168.3.244";
        opt.UserName = "fundu";
        opt.Password = "FNS^7Q1MsBh";
        opt.VirtualHost = "/";
        opt.ExchangeName = "cap.default.router";
    });
    x.TopicNamePrefix = "fund";
    x.GroupNamePrefix = "fund";
    x.SucceedMessageExpiredAfter = 24 * 3600;
    x.FailedMessageExpiredAfter = 15 * 24 * 3600;
    x.FailedRetryCount = 5;
    x.FailedRetryInterval = 10;
    x.Version = "v1";
    x.UseDashboard(options =>
    {
        options.PathMatch = "/cap";
    });
});*/
builder.Services.AddHealthChecks();
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
//app.MapScalarApiReference(options =>
//{
//    //options.WithOpenApiRoutePattern("/swagger/{documentName}.json");
//    // or
//    options.OpenApiRoutePattern = "api/swagger/{documentName}/swagger.json";
//});
var documents = new[]
{
    new ScalarDocument("v1", "nuget包", "api/swagger/v1/swagger.json"),
    //new ScalarDocument("v2", "API", "http://k8s.api.com/api/swagger/v1/swagger.json"),
};
app.MapScalarApiReference("/scalar", options =>
{
    options.AddDocuments(documents);
    options.DefaultHttpClient = new(ScalarTarget.Node, ScalarClient.Axios);
});
app.MapControllers();
app.MapHealthChecks("/health");

app.UseMiddleware<LogMiddleware>();//添加日志中间件
//app.UseMiddleware<ExceptionMiddleware>();//添加异常处理中间件
//app.UseMiddleware<CurrentLimitingMiddleware>(1, 1);//添加限流中间件 1个线程 1个并发
//app.UseMiddleware<IdempotenceMiddleware>();//添加幂等性中间件

app.Run();