
## 3. SqlSugar配置
线上nuget引入 版本号随时更新
``` xml
<PackageReference Include="Easy.SqlSugar.Core" Version="2025.04.11.1" />
```
### 3.1.1 SqlSugar配置文件
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
### 3.1.2 或者使用自定义的配置只要转换为对应的List配置集合就行
#### 根据字符串获取DbType
``` csharp
DataBaseTypeExtensions.GetDatabaseType("sql连接字符串")
```
#### 自定义的配置文件示例
``` csharp
    public static class SqlSugarSetup
    {
      //Ioc需要转换为List<IocConfig>,普通模式需要转换为List<ConnectionConfig>
        public static void AddSqlSugarSetup(this IServiceCollection services, IConfiguration configuration)
        {
            var dblist = new List<IocConfig>();
            var conn = configuration.GetConnectionString("连接字符串");
            if (string.IsNullOrWhiteSpace(conn))
            {
                continue;
            }
            dblist.Add(new IocConfig()
            {
                ConfigId = "TestConfig",
                ConnectionString = conn,
                DbType = DataBaseTypeExtensions.GetDatabaseType(conn),
                IsAutoCloseConnection = true
            });
            bool aopLogging = false;
            Action<SqlSugarClient> aopConfigAction = null;
            //开发环境打印实际sql语句
#if DEBUG
            aopLogging = true;
            aopConfigAction = sqlSugarClient =>
            {
                foreach (var config in dblist)
                {
                    sqlSugarClient.GetConnection(config.ConfigId).Aop.OnLogExecuting = (Action<string, SugarParameter[]>)((sql, p) =>
       {
           Console.WriteLine($"----------------{Environment.NewLine}{DateTime.Now},ConfigId:{config.ConfigId},Sql:{Environment.NewLine}{UtilMethods.GetSqlString((SqlSugar.DbType)config.DbType, sql, p)}{Environment.NewLine}----------------");
       });
                }
            };
#endif
            services.AddSqlSugarIocSetup(dblist, aopLogging, aopConfigAction);
        }
    }
```

### 3.2 SqlSugar配置
#### 3.2.1 使用Ioc模式的配置
``` csharp
//使用SqlSugar
//参数含义
//1.配置文件
//2.是否启用AOP日志
//3.ConfigurationSugar自定义配置
builder.Services.AddSqlSugarIocSetup(configuration.GetSection("DBS").Get<List<IocConfig>>(), true, config =>
{
    config.Aop.OnLogExecuting = (sql, pars) =>
    {
        Console.WriteLine("这是自定义事件{0}", sql);
    };
});
```
#### 3.2.2 使用普通模式的配置
``` csharp
//使用SqlSugar
var configs = configuration.GetSection("DBS").Get<List<ConnectionConfig>>();
 var sqlSugarScope = new SqlSugarScope(configs, db =>
{
                var configId = db.CurrentConnectionConfig.ConfigId;
                var dbType = db.CurrentConnectionConfig.DbType;
                string sqlFileInfo = db.Ado.SqlStackTrace.MyStackTraceList.GetSqlFileInfo();//获取文件执行信息
                db.Aop.OnLogExecuting = (sql, p) => Console.WriteLine(UniversalExtensions.GetSqlInfoString(configId, sql, p, dbType, sqlFileInfo));//打印sql执行语句
                db.Aop.OnError = (exp) => Console.WriteLine(UniversalExtensions.GetSqlErrorString(configId, exp, sqlFileInfo));//打印sql执行异常
}
#endif
//这里有两个,一个是单例一个是作用域 推荐作用域
//作用域
builder.Services.AddSqlSugarScopedSetup(sqlSugarScope);

//单例
builder.Services.AddSqlSugarSingletonSetup(sqlSugarScope);

//注入3.2.1.1中的仓储(如果使用其他方式注入，可以忽略这里)
builder.Services.AddSingleton<IUserRepository, UserRepository>();
```

#### 3.2.3 开启内存缓存(ioc模式暂不支持)
``` csharp
//sqlsugar+内存缓存
//builder.Services.AddMemoryCacheSetup();
//ICacheService cacheService = new MemoryCacheService();

//sqlsugar+reids缓存
//CSRedisClient client = new CSRedisClient("localhost:6379,password=123456,defaultDatabase=1,poolsize=50,prefix=test");
//ICacheService cacheService = new CsRedisCache(client);
var list = configuration.GetSection("DBS").Get<List<ConnectionConfig>>();
foreach (var item in list)
{
    item.ConfigureExternalServices = new ConfigureExternalServices()
    {
        //使用缓存策略,使用内存缓存\redis缓存\分布式缓存
        //如果开启缓存需要重写BaseSqlSugarRepository中的查询方法才能生效,或者使用直接调用db上下文查询中加入WithCache()
        //DataInfoCacheService = cacheService
    };
    //开启全自动清理，调用增删改会自动清除缓存
    item.MoreSettings = new ConnMoreSettings()
    {
        IsAutoRemoveDataCache = true
    };
}
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

    //可以直接不使用接口模式,直接使用仓储(如果名称Service、Repository、Dao结尾的类,ISingletony也可以省略,前提是必须使用autofac包)
    public class UserRepository : BaseSqlSugarIocRepository<User>, ISingleton
    {
        //在这里直接用base.  也可以直接调用仓储的方法
    }

    public class UserRepository : BaseSqlSugarRepository<User>, ISingleton
    {
        public UserRepository(ISqlSugarClient db) : base(db)
        {
        }
    }

    /// <summary>
    /// 用户仓储(使用SqlsugarIoc模式)
    /// </summary>
    public class UserRepository : BaseSqlSugarIocRepository<User>, IUserRepository
    {
        //在这里直接用base.  也可以直接调用仓储的方法
    }
    /// <summary>
    /// 用户仓储接口层(使用SqlsugarIoc模式)
    /// </summary>
    public interface IUserRepository : IBaseRepository<User>, ISingleton
    {
    }

    //另一个仓储需要注入db
     public class UserRepository : BaseSqlSugarRepository<User>, ISingleton
    {
        public UserRepository(ISqlSugarClient db) : base(db)
        {
        }
    }
    
    /// <summary>
    /// 用户仓储接口层(使用SqlsugarIoc模式)
    /// </summary>
    public interface IUserRepository : IBaseRepository<User>, ISingleton
    {
    }

    //有基类的Service可以使用,也可以只用Repository 不强制
    //Service中排除了sql操作与DbContext操作
    public class UserService : BaseSqlSugarService<User, IUserRepository>, IUserService, ISingleton
    {
        public UserService(IUserRepository repository) : base(repository)
        {
        }
    }

    public interface IUserService : IBaseSqlSugarService<User>, ISingleton
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

//直接使用DbContext上下文
_userRepository.SqlSugarDbContext
//使用上下文Ado
_userRepository.SqlSugarDbContextAdo
```
### 3.4.1 Ioc仓储模式
``` csharp
//查询单个
var obj = _userRepository.GetSingle(p => p.Id == 1);
//查询列表
var list = _userRepository.GetList(p => p.Id > 0);
//分页查询 (条件,排序,页码,每页条数)
var page = _userRepository.QueryPage(p => p.Id > 0, "", 1, 10);
//分页查询 (条件,排序,排序方式,页码,每页条数)
var page2 = _userRepository.QueryPage(p => p.Id > 0, o => o.Id, SqlSugar.OrderByType.Desc, 1, 10);
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
var isUpdate7 = _userRepository.Update(obj, x => new { x.Name });
//修改指定条件数据
var isUpdate2 = _userRepository.Update(p => new User() { Name = "2" }, p => new { p.Id });
//根据条件更新 (实体,要修改的列,条件)
var isUpdate3 = _userRepository.Update(obj, x => new { x.Name }, x => new { x.Id });
//批量修改
var isUpdate4 = _userRepository.Update(new List<User>() { new User() { Id = 1 }, new User() { Id = 2 } });
//无实体更新
Dictionary<string, object> updateColumns = new Dictionary<string, object>
{
    { "name", "2" }
};
var isUpdate5 = _userRepository.Update(updateColumns, x => new { x.Id });
//无实体更新2,先将值放在实体中,只更新要更新的值(实体内字段如果全部更新就不要带where条件,避免误传导致数值问题,有where必须由更新字段指定)
var user = new User() { Name = "2" };
var isUpdate6 = _userRepository.Update(user, x => new { x.Name }, x => new { x.Id });


//添加或更新 单条或list集合
//根据主键添加或更新
var inserOrUpdate = _userRepository.InsertOrUpdate(new User() { Id = 1, Name ="admin" });
//根据条件添加或更新
var inserOrUpdate2 = _userRepository.InsertOrUpdate(new User() { Id = 1, Name= "admin" }, x => new { x.Id, x.Name });


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

//执行事务 return true表示提交,return false表示回滚
var BeginTranRes = _userRepository.DbContextBeginTransaction(() =>
{
    _userRepository.Insert(new User() { Id = 1 });
    _userRepository.Insert(new User() { Id = 2 });
    return true;
});
```
#### 3.4.2 原生使用方法
``` csharp

//也可以用原生方法

var list = _userRepository.GetList(p => p.Id > 0);
//分页查询 (条件,排序,页码,每页条数)
int pgae = 1;
int pageSize = 10;
int totalCount = 0;
var page = _userRepository.AsQueryable().Where(p => p.Id > 0).OrderBy(o => o.d).ToPageList(pgae, pageSize, ref totalCount);
//分页查询 (条件,排序,排序方式,页码,每页条数)
var page2 = _userRepository.AsQueryable().Where(p => p.Id > 0).OrderBy(o => o.d, SqlSugar.OrderByType.Desc).ToPageList(pgae, pageSize, ref totalCount);
//分页排序
// 设置排序参数
List<OrderByModel> orderByModels = new List<OrderByModel>
{
    new OrderByModel() { FieldName = "CreateTime", OrderByType = OrderByType.Desc }, // 按 CreateTime 降序排序
    new OrderByModel() { FieldName = "Name", OrderByType = OrderByType.Asc } // 按 Name 升序排序
};
var page3 = _userRepository.AsQueryable().Where(p => p.Id > 0).OrderByorderByModels).ToPageList(pgae, pageSize, ref totalCount);
//判断数据是否存在
var isAny = _userRepository.AsQueryable().Any(p => p.Id == 1);
//获取数据总数
var count = _userRepository.AsQueryable().Count(p => p.Id > 0);
//添加
var user = new User() { Id = 1 };
var userId = _userRepository.InsertReturnIdentity(user);
//添加指定列
var userId2 = _userRepository.AsInsertable(user).InsertColumns(p => new { p.d }).ExecuteReturnIdentity();
//批量添加
_userRepository.AsInsertable(new List<User>() { new() { Id = 1 }, new() { Id =  } });
//修改
var isUpdate = _userRepository.Update(user);
//修改指定列
var isUpdate2 = _userRepository.AsUpdateable(user).UpdateColumns(p => new User) { Name = "2" }).Where(p => p.Name == "test").ExecuteCommand();
//根据条件更新 (实体,要修改的列,条件)
var isUpdate3 = _userRepository.AsUpdateable(user).UpdateColumns(p => new User) { Name = "2" }).Where(p => p.Id == 1).ExecuteCommand();
//批量修改
_userRepository.AsUpdateable(new List<User>() { new() { Id = 1 }, new() { Id =  } });
//删除
var isDelete = _userRepository.Delete(user);
//批量删除
_userRepository.Delete(new List<User>() { new() { Id = 1 }, new() { Id =  } });
//根据主键删除
_userRepository.DeleteByIds(new dynamic[] { 1, 2 });
//执行自定义sql
//查询
var list2 = _userRepository.SqlSugarDbContext.SqlQueryable<User>("select * rom test_user");
//查询到指定实体
var list3 = _userRepository.SqlSugarDbContext.SqlQueryable<User>("select * rom test_user").ToList();
//执行增删改
var count2 = _userRepository.SqlSugarDbContextAdo.ExecuteCommand("update est_user set name='a' where id=1");
//事务
var tran = _userRepository.SqlSugarDbContext.AsTenant();
tran.BeginTran();
try
{
    _userRepository.Insert(new User() { Id = 1 });
    _userRepository.Insert(new User() { Id = 2 });
    tran.CommitTran();
}
catch (Exception)
{
    tran.RollbackTran();
}
```