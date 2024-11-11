# nuget包合集
## 1. Serilog配置
线上nuget引入 版本号随时更新
``` xml
<PackageReference Include="WBC66.Serilog.Core" Version="2024.10.28" />
```
### 1.1 Serilog配置文件
``` json
  "SerilogOptions": {
    "MinimumLevel": "Information",
    "Override": {
      "Microsoft": "Warning",
      "System": "Warning"
    },
    "File": {
      "Path": "Serilog/log-.txt",
      "RollingInterval": "Day"
    },
    "Console": {
      "Enabled": true,
      "Minlevel": "Information",
      "Template": "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}"
    },
    "Elasticsearch": {
      "Uri": "http://k8s.els.com",
      "IndexFormat": "test-dev-{0:yyyy.MM.dd}",
      "NumberOfShards": 2,
      "NumberOfReplicas": 1,
      "UserName": "elastic",
      "Password": "changeme"
    }
  }
```
### 1.2 Serilog配置
``` csharp
builder.Host.AddSerilogHost(configuration);

app.UseSerilogSetup();
```
## 2. NLog配置(两种配置模式，一种nlog.config配置文件，一种json配置) 感觉还是serilog更好用
线上nuget引入 版本号随时更新
``` xml
<PackageReference Include="WBC66.NLog.Core" Version="	2024.10.25" />
```
### 2.1 NLog配置文件(xml方式)
``` xml
<?xml version="1.0" encoding="utf-8" ?>
<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd"
      xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
      autoReload="true"
      throwConfigExceptions="true">
	<extensions>
		<add assembly="NLog.Web.AspNetCore"/>
		<add assembly="NLog.Targets.ElasticSearch"/>
	</extensions>

	<targets>
		<target name="logfile" xsi:type="File" fileName="./log/${shortdate}_${level}.log"
				layout="${longdate}|${level:uppercase=true}|${logger}|${message}|${exception:format=tostring}" />
		<target name="logconsole" xsi:type="Console"
				layout="${longdate}|${level:uppercase=true}|${logger}|${message}|${exception:format=tostring}" />
		<!-- ElasticSearch发送-->
		<target xsi:type="ElasticSearch" name="els" index="test-dev-${date:format=yyyy.MM}" uri="http://k8s.els.com"
				layout="${longdate}|${level:uppercase=true}|${logger}|${message}|${exception:format=tostring}"/>
	</targets>

	<rules>
		<!-- 忽略Microsoft的日志 -->
		<logger name="Microsoft.*" minlevel="Trace" final="true" />
		<!-- 所有日志输出到控制台和文件 -->
		<logger name="*" minlevel="Info" writeTo="logfile" />
		<logger name="*" minlevel="Info" writeTo="els" />
	</rules>
</nlog>
```
### 2.2 NLog配置
``` csharp
builder.Host.AddNLogHost();
```
### 2.3 NLog配置文件(json方式)
``` json
"NLog": {
    "autoReload": true,
    "throwConfigExceptions": true,
    "extensions": [
      {
        "assembly": "NLog.Web.AspNetCore"
      },
      {
        "assembly": "NLog.Targets.ElasticSearch"
      }
    ],
    "targets": {
      "logfile": {
        "type": "File",
        "fileName": "./log/${shortdate}_${level}.log",
        "layout": "${longdate}|${level:uppercase=true}|${logger}|${message}|${exception:format=tostring}"
      },
      "logconsole": {
        "type": "Console",
        "layout": "${longdate}|${level:uppercase=true}|${logger}|${message}|${exception:format=tostring}"
      },
      "els": {
        "type": "ElasticSearch",
        "index": "test-dev-${date:format=yyyy.MM}",
        "uri": "http://k8s.els.com",
        "layout": "${longdate}|${level:uppercase=true}|${logger}|${message}|${exception:format=tostring}"
      }
    },
    "rules": [
      {
        "logger": "Microsoft.*",
        "minlevel": "Trace",
        "final": true
      },
      {
        "logger": "*",
        "minlevel": "Info",
        "writeTo": "logfile"
      },
      {
        "logger": "*",
        "minlevel": "Info",
        "writeTo": "els"
      }
    ]
  }
```
### 2.4 NLog配置
``` csharp
builder.AddNLogSteup(configuration);
```

## 3. SqlSugar配置
线上nuget引入 版本号随时更新
``` xml
<PackageReference Include="WBC66.SqlSugar.Core" Version="2024.10.28" />
```
### 3.1 SqlSugar配置文件
``` json

    /*
      对应下边的 DBType
      MySql = 0,
      SqlServer = 1,
      Sqlite = 2,
      Oracle = 3,
      PostgreSQL = 4,
      Dm = 5,//达梦
      Kdbndp = 6,//人大金仓
    */
  "DBS": [
    {
      "ConfigId": "journal",
      "DBType": 0,
      "IsAutoCloseConnection": true,
      "ConnectionString": "server=localhost;Database=journal;Uid=root;Pwd=123456;allowPublicKeyRetrieval=true;"
    }
  ]
```
### 3.2 SqlSugar配置
``` csharp
//使用SqlSugar
//参数含义
//1.配置文件
//2.是否启用AOP日志
//3.ConfigurationSugar自定义配置
builder.Services.AddSqlSugarSetup(configuration.GetSection("DBS").Get<List<IocConfig>>(), true, config =>
{
    config.Aop.OnLogExecuting = (sql, pars) =>
    {
        Console.WriteLine("这是自定义事件{0}", sql);
    };
});

//注入3.3中的仓储(如果使用其他方式注入，可以忽略这里)
builder.Services.AddSingleton<IUserRepository, UserRepository>();
```
### 3.3 实体类、仓储
``` csharp
    /// <summary>
    /// 用户表
    ///</summary>
    [SugarTable("J_User")]//表别名
    [Tenant("journal")]//数据库标识 需要与配置文件中的ConfigId对应
    public class User
    {
        /// <summary>
        ///用户id
        ///</summary>
        [Key]
        [SugarColumn(ColumnName = "ID", IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
    }

    /// <summary>
    /// 用户仓储
    /// </summary>
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        //在这里直接用base.  也可以直接调用仓储的方法
    }

    /// <summary>
    /// 用户仓储接口层
    /// </summary>
    public interface IUserRepository : IBaseRepository<User>
    {
    }
    
    //注入
    private readonly IUserRepository _userRepository;

    public SqlSugarController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    //方法中使用
    public object Get()
    {
        var user = _userRepository.GetSingle(p => p.Id == 1);
        return user;
    }
```
### 3.4 使用示例
``` csharp
//所有操作都有异步方法，增加Async即可
//查询单个
var obj = _userRepository.GetSingle(p => p.Id == 1);
//查询列表
var list = _userRepository.GetList(p => p.Id > 0);
//分页查询 (条件,排序,页码,每页条数)
var page = _userRepository.QueryPage(p => p.Id > 0, "", 1, 10);
//分页查询 (条件,排序,排序方式,页码,每页条数)
var page2 = _userRepository.QueryPage(p => p.Id > 0, o => o.Id, OrderByType.Desc, 1, 10);
//分页排序
// 设置排序参数
// Dictionary<string, QueryOrderBy> orderBy = new Dictionary<string, QueryOrderBy>
// {
//     { "CreateTime", QueryOrderBy.Desc }, // 按 CreateTime 降序排序
//     { "Name", QueryOrderBy.Asc } // 按 Name 升序排序
// };
// var page3 = _userRepository.IQueryablePage(p => p.Id > 0, 1, out int rowcount,  orderBy, true);
//判断数据是否存在
var isAny = _userRepository.Exists(p => p.Id == 1);
//获取数据总数
var count = _userRepository.GetCount(p => p.Id > 0);
//添加
var userId = _userRepository.Insert(new User() { Id = 1 });
//添加指定列
var userId2 = _userRepository.Insert(new User() { Id = 1 }, p => new { p.Id });
//批量添加
var userIds = _userRepository.Insert(new List<User>() { new User() { Id = 1 }, new User() { Id = 2 } });
//修改
var isUpdate = _userRepository.Update(obj);
//修改指定列
var isUpdate2 = _userRepository.Update(p => new User() { Name = "2" }, p => p.Name == "test");
//根据条件更新 (实体,要修改的列,条件)
var isUpdate3 = _userRepository.Update(obj, new List<string>() { "name" }, new List<string>() { "Id = 1" });
//批量修改
var isUpdate4 = _userRepository.Update(new List<User>() { new User() { Id = 1 }, new User() { Id = 2 } });
//删除
var isDelete = _userRepository.Delete(obj);
//批量删除  有问题
var isDelete2 = _userRepository.Delete(new List<User>() { new User() { Id = 1 }, new User() { Id = 2 } });
//根据主键删除
var isDelete3 = _userRepository.DeleteByIds([1, 2]);

//执行自定义sql
//查询
var list2 = _userRepository.SqlQuery("select * from test_user", null);
//查询到指定实体
var list3 = _userRepository.SqlQuery<User>("select * from test_user", null);
//执行增删改
var count2 = _userRepository.ExecuteSql("update test_user set name='a' where id=1", null);
//查询分页到指定实体
var page4 = _userRepository.SqlPageQuery<User>("select * from test_user", null, 1, 1);
var page4Count = page4.TotalCount;

//执行事务
var BeginTranRes = _userRepository.DbContextBeginTransaction(() =>
{
    _userRepository.Insert(new User() { Id = 1 });
    _userRepository.Insert(new User() { Id = 2 });
    return true;
});
```
## 4. EF配置(推荐使用上边的SqlSugar兼容性更强一些功能更完善)
线上nuget引入 版本号随时更新
``` xml
<PackageReference Include="WBC66.EF.Core" Version="2024.10.29" />
```
### 4.1 EF配置文件
``` json
  "DBS": [
    /*
      对应下边的 DBType
      MySql = 0,
      SqlServer = 1
    */
    {
      "DBType": 0,
      "ConnectionString": "server=localhost;Database=journal;Uid=root;Pwd=123456;allowPublicKeyRetrieval=true;"
    }
  ]
```
### 4.2 EF上下文
``` csharp
    public class TestDBContext : DbContext
    {
        /// <summary>
        /// 日志工厂
        /// </summary>
        public static readonly ILoggerFactory MyLoggerFactory = LoggerFactory.Create(builder =>
        {
            builder
                .AddFilter((category, level) => category == DbLoggerCategory.Database.Command.Name && level == LogLevel.Information)
                .AddConsole();
        });
        public TestDBContext(DbContextOptions options) : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //日志
            optionsBuilder.UseLoggerFactory(MyLoggerFactory);
            optionsBuilder.EnableSensitiveDataLogging();
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<UserEF> Users { get; set; }
    }
```
### 4.3 EF配置
``` csharp
//获取配置文件
var efOptions = configuration.GetSection("DBS").Get<List<EFOptions>>()[0];
builder.Services.AddEFSetup<TestDBContext>(efOptions);
builder.Services.AddScoped<IUserEFRepository, UserEFRepository>();
//注入
builder.Services.AddSingleton<IUserRepository, UserRepository>();
```
### 4.4 实体类、仓储
``` csharp
    /// <summary>
    /// 用户表
    ///</summary>
    [Table("test_user")]
    public class UserEF
    {
        /// <summary>
        ///用户id
        ///</summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        ///用户名称
        ///</summary>
        public string? Name { get; set; }
    }

    /// <summary>
    /// 用户仓储
    /// </summary>
    public class UserEFRepository : BaseRepository<TestDBContext, UserEF>, IUserEFRepository
    {
        public UserEFRepository(TestDBContext context) : base(context)
        {
        }
        //需要重写方法直接在这里重写
        public override int Insert(List<UserEF> entity, bool isSave = true)
        {
            return base.Insert(entity, isSave);
        }
    }

    /// <summary>
    /// 用户仓储接口层
    /// </summary>
    public interface IUserEFRepository : IBaseRepository<TestDBContext, UserEF>
    {
    }
```
### 4.5 使用示例
``` csharp
//所有操作都有异步方法，增加Async即可
//添加、修改、删除可以设置是否立即保存，然后调用SaveChanges()方法说动保存
//查询单个
var obj = _userRepository.GetSingle(p => p.Id == 1);
//查询列表
var list = _userRepository.GetList(p => p.Id > 0);
//分页查询 (条件,排序,页码,每页条数)
var page = _userRepository.QueryPage(p => p.Id > 0, "", 1, 10);
//分页查询 (条件,排序,排序方式,页码,每页条数)
var page2 = _userRepository.QueryPage(p => p.Id > 0, o => o.Id, OrderByTypeDesc, 1, 10);
//分页排序
// 设置排序参数
// Dictionary<string, QueryOrderBy> orderBy = new Dictionary<string,QueryOrderBy>
// {
//     { "CreateTime", QueryOrderBy.Desc }, // 按 CreateTime 降序排序
//     { "Name", QueryOrderBy.Asc } // 按 Name 升序排序
// };
// var page3 = _userRepository.IQueryablePage(p => p.Id > 0, 1, out introwcount,  orderBy, true);
//判断数据是否存在
var isAny = _userRepository.Exists(p => p.Id == 1);
//获取数据总数
var count = _userRepository.GetCount(p => p.Id > 0);
//添加
var userId = _userRepository.Insert(new UserEF() { Id = 1 });
//批量添加
var userIds = _userRepository.Insert(new List<UserEF>() { new UserEF() { Id= 1 }, new UserEF() { Id = 2 } });
//修改
var isUpdate = _userRepository.Update(obj);
//批量修改
var isUpdate4 = _userRepository.Update(new List<UserEF>() { new UserEF() {Id = 1 }, new UserEF() { Id = 2, Name = "test1" } });
//删除
var isDelete = _userRepository.Delete(obj);
//执行自定义sql
//查询
var list2 = _userRepository.SqlQuery("select * from test_user", null);
//查询到指定实体
var list3 = _userRepository.SqlQuery<UserEF>("select * from test_user",null);
//执行增删改
var count2 = _userRepository.ExecuteSql("update test_user set name='a'where id=1", null);
//查询分页到指定实体
var page4 = _userRepository.SqlPageQuery<UserEF>("select * from test_user",null, 1, 1);
var page4Count = page4.TotalCount;
//执行事务
var BeginTranRes = _userRepository.DbContextBeginTransaction(() =>
{
    _userRepository.Insert(new UserEF() { Id = 1 });
    _userRepository.Insert(new UserEF() { Id = 2 });
    return true;
});
```
## 5. Autofac配置
### 1. Autofac配置
线上nuget引入 版本号随时更新
``` xml
<PackageReference Include="WBC66.Autofac.Core" Version="2024.11.4" />
```
### 2.使用Autofac工厂
``` csharp
//使用Autofac(会直接扫描当前项目的所有程序集进行注入)
builder.Host.AddAutofacHostSetup(builder.Services);
```
### 3.使用示例
``` csharp
//接口注入直接继承IDependency接口即可(一般情况第二步已经注入了就不需要再注入了)
public interface ITestService : IDependency
{
    string Get();
}
``` 
## 6. 通过Aop使用内存缓存对接口、方法进行缓存
### 1. nuget包引入
必须引入两个包 至少在2024.11.7以上
``` xml
<PackageReference Include="WBC66.Cache.Core" Version="2024.11.7" />
<PackageReference Include="WBC66.Autofac.Core" Version="2024.11.7" />
```
### 必须开启内存缓存 否则后续步骤无法正常进行
``` csharp
//开启内存缓存
builder.Services.AddMemoryCacheSetup();
```
### 2. 对方法使用内存缓存
``` csharp
//使用autofac(内部会自动进行程序集扫描注入,不需要手动注入)
builder.Host.AddAutofacHostSetup(builder.Services, options =>
{
    //开启内存缓存拦截器(带有IProxyService接口的类将会被拦截),带有CacheResultAttribute特性的方法将会被缓存
    options.AddMemoryCacheResultAop();
});
```
### 3. 对控制器接口使用内存缓存
``` csharp
builder.Services.AddControllers(options =>
{
    //添加自定义的缓存过滤器
    options.Filters.Add<CacheResultFilter>();
});
```
### 方法缓存使用示例
``` csharp
//同一个方法不同参数会进行多次缓存
//接口注入直接继承IProxyService接口(继承接口后就不需要再手动注入了),方法上使用CacheResultAttribute特性即可
public class CacheResultService : ICacheResultService
{
    //缓存5秒
    [CacheResult(5)]
    public Student GetStudentAsync(string name)
    {
        return new Student { Name = name };
    }
}
public interface ICacheResultService : IProxyService
{
    Student GetStudentAsync(string name);
}
```
### 控制器缓存使用示例
``` csharp
[HttpGet]
[CacheResult(5)]//缓存5秒
public object Get()
{
    _cacheResultService.GetStudentAsync("这是参数");
    return "ok";
}
```
## 7. 过滤器,中间件
### 1. 过滤器
``` csharp
//幂等性过滤器
builder.Services.AddControllers(options =>
{
    //添加自定义的缓存过滤器 需要配合第六段的Aop使用
    options.Filters.Add<CacheResultFilter>();
    //添加自定义的幂等性过滤器
    options.Filters.Add<IdempotentFilter>();
});
```
### 2. 中间件
``` csharp
//添加自定义的中间件
app.UseMiddleware<LogMiddleware>();//添加日志中间件
app.UseMiddleware<ExceptionMiddleware>();//添加异常处理中间件
app.UseMiddleware<CurrentLimitingMiddleware>(1, 1);//添加限流中间件 1个线程 1个并发
```

