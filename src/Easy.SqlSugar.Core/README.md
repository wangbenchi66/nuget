
## 3. SqlSugar配置
线上nuget引入 版本号随时更新
``` xml
<PackageReference Include="Easy.SqlSugar.Core" Version="2025.12.01.1" />
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

### 最简单的使用方法
``` csharp
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
            //Console.WriteLine(UniversalExtensions.GetSqlInfoString(configId, sql, p, dbType, sqlFileInfo));
        };
        aop.OnError = (SqlSugarException exp) =>
        {
            Console.WriteLine(UniversalExtensions.GetSqlErrorString(configId, exp, sqlFileInfo));
        };
    }
});
builder.Services.AddSqlSugarSetup(sqlSugarScope);

//已经内置了很多调用方法，详情使用见下边3.4的使用示例
//在仓储使用，ISingleton需要自己注入(内部已经封装了自动注入，如果不生效就自己注入)
public class UserRepository : BaseSqlSugarRepository<User>, ISingleton
{
}
//或者在service中直接注入BaseSqlSugarRepository<User>进行使用
private readonly BaseSqlSugarRepository<User> _userRepository;
public SqlSugarTestApis(BaseSqlSugarRepository<User> userRepository)
{
    _userRepository = userRepository;
}
```

### 静态类SugarDbManger使用方法
``` csharp
//或者可以不注入，直接使用静态的db(框架内置已经封装好了可以直接用)
//获取db对应的ISqlSugarClient
var db=SugarDbManger.Db;
var db = SugarDbManger.GetConfigDb("journal");//根据ConfigId获取对应的DbContext
var db2 = SugarDbManger.GetTenantDb<User>();//根据实体类获取对应的DbContext(需要实体类有Tenant特性标识)
//获取对应仓储
var userRepository = SugarDbManger.GetConfigDbRepository<UserPg>("Pg");//根据ConfigId获取对应的仓储
var userRepository = SugarDbManger.GetTenantDbRepository<User>();//根据实体类获取对应的仓储(需要实体类有Tenant特性标识)
//获取一个新的DbContext
var db = SugarDbManger.GetNewDb();
```
### 雪花id配置
``` csharp
//默认使用的是sqlsugar自带的雪花id生成器,框架内部每次启动会生成一个随机的0-31之间的机器码 不需要手动配置

//推荐使用下面的Yitter.IdGenerator雪花id算法,内置时钟回拨问题处理,支持71000年唯一不重复，suagr的雪花只支持到69年后
YitIdHelper.SetIdGenerator(UniversalExtensions.YitSnowflakeOptions);//直接这样设置使用 内部已经配置好了，如果不用这种内置的雪花id解析方法可能会出问题
StaticConfig.CustomSnowFlakeFunc = YitIdHelper.NextId;

//雪花id解析方法

//解析sqlsugar自带的雪花id
UniversalExtensions.ParseSugarSnowflakeId(id);
//解析Yitter.IdGenerator雪花id,内部会自动读取当前的配置
UniversalExtensions.ParseYitSnowflakeId(id);

//会返回一个实体类，样式如下
{
  "SnowflakeId": 1945020448118469,//原始雪花id
  "Timestamp": "2025-11-21T10:51:22.498+08:00",//时间戳
  "Date": "2025-11-21 10:51:22",//本地时间
  "DatacenterId": 0,//数据中心id,Yitter.IdGenerator没有数据中心id，默认0
  "WorkerId": 27,//机器id
  "Sequence": 5//序列号
}
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
_userRepository.AsQueryable()
_userRepository.AsInsertable()
_userRepository.AsUpdateable()
_userRepository.AsDeleteable()
_userRepository.SqlSugarDbContext
//使用上下文Ado
_userRepository.SqlSugarDbContextAdo
```
### 3.4.1 框架仓储模式
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
#### 3.4.2 官方仓储方法
``` csharp
//查询
var data1 = base.GetById(1);//根据id查询
var data2 = base.GetList();//查询所有 (FindList)
var data3 = base.GetList(it => it.Id == 1); //TOP1条件
var data4 = base.GetSingle(it => it.Id == 1);//查询单条记录，结果集不能超过1，不然会提示错误
var  data= base.GetFirst(it => it.Id == 1);//查询第一条记录
var p = new PageModel() { PageIndex = 1, PageSize = 2 };
var data5 = base.GetPageList(it => it.Name == "xx", p);
Console.Write(p.PageCount);
var data6 = base.GetPageList(it => it.Name == "xx", p, it => it.Name, OrderByType.Asc);
Console.Write(p.PageCount);
List<IConditionalModel> conModels = new List<IConditionalModel>();
conModels.Add(new ConditionalModel(){FieldName="id",ConditionalType=ConditionalType.Equal,FieldValue="1"});//id=1
var data7 = base.GetPageList(conModels, p, it => it.Name, OrderByType.Asc);
var data8 = base.AsQueryable().Where(x => x.Id == 1).ToList();//使用Queryable
 
//插入
base.Insert(insertObj);//单条插入
base.InsertRange(InsertObjs);//批量插入
var id = base.InsertReturnIdentity(insertObj);//插入返回自增
var SnowflakeId=base.InsertReturnSnowflakeId(insertObj);//插入返回雪花ID
base.AsInsertable(insertObj).ExecuteCommand();//复杂功能使用Insertable
 
 
//删除
base.Delete(T);//实体删除 需要有主键
base.Delete(List<T>);//集合删除 需要有主键
base.DeleteById(1);
base.DeleteByIds(new object [] { 1, 2 }); //数组带是 ids方法 ，封装传 object [] 类型
//技巧 int [] 转换成 object[]  写法：ids.Cast<object>().ToArray()
base.Delete(it => it.Id == 1);
base.AsDeleteable().Where(it => it.Id == 1).ExecuteCommand();//复杂功能用Deleteable
 
//实体方式更新更新
base.Update(insertObj); //单条更新
base.UpdateRange(InsertObjs);//批量更新
base.AsUpdateable(insertObj)//复杂功能用 Updateable 
   .UpdateColumns(it=>new { it.Name })
   .ExecuteCommand();
 
//表达式方式更新
base.Update(it =>new Order(){Name="a" /*可以多列*/ },it =>it.Id==1); //只更新name 并且id=1
base.UpdateSetColumnsTrue(it=>new Order(){ Name ="a"}, it=>it.Id==1);//更新name+过滤事件赋值字段 
base.AsUpdateable()//复杂功能用 Updateable 
   .SetColumns(it=>new  Order{  Name="a" })
   .Where(it=>it.Id==1).ExecuteCommand();
 
 
//高级操作
base.Context //获取db对象
base.AsSugarClient // 获取完整的db对象
base.AsTenant  // 获取多库相关操作
 
//如果Context是子Db
base.Context.Root//可以用Root拿主Db
base.Context.Root.AsTenant()//拿到租户对象
 
 
//切换仓储 （可以注入多个仓储取代，这样就不用切换了）
base.ChangeRepository<Repository<OrderItem>>() //多租户用法有区别：https://www.donet5.com/Home/Doc?typeId=2405
base.Change<OrderItem>()//只支持自带方法和单库
```

#### 4.扩展方法
``` csharp
//获取数据库类型
var dbType= DataBaseTypeExtensions.GetDatabaseType(conn);


//将微软官方特性转换为sqlsugar特性 转换Key、Table、DatabaseGenerated、MaxLength、 Required特性为sqlsugar特性
//用的时候在数据库初始化中直接设置
ConfigureExternalServices = UniversalExtensions.GetInitConfigureExternalServices()
//例如：
var db = new ConnectionConfig()
{
    ConfigId = item.ToString(),
    ConnectionString = conn.CheckTrustServerCertificate(),
    DbType = DataBaseTypeExtensions.GetDatabaseType(conn),
    IsAutoCloseConnection = true,
    MoreSettings = new ConnMoreSettings()
    {
        IsAutoRemoveDataCache = true,
        IsWithNoLockQuery = true,
    },
    ConfigureExternalServices = UniversalExtensions.GetInitConfigureExternalServices(),// 初始化时转换table和key的特性为sqlsugar 不需要可以注释
};


//检测TrustServerCertificate,没有则添加TrustServerCertificate=true
conn=conn.CheckTrustServerCertificate(dbType);
//检测Encrypt,没有则添加Encrypt=true
conn=conn.CheckEncrypt(dbType);

//获取sql执行文件信息(会过滤行号为0的记录，如果文件层级大于3,则只显示最后3级目录)
// 返回的文件信息格式为：
// 位置:文件名,行号:行号
var sqlFileInfo = db.Ado.SqlStackTrace.MyStackTraceList.GetSqlFileInfo();//获取文件执行信息

//获取db.Aop.OnLogExecuting日志打印的sql语句,sqlFileInfo可为空
var infoSql= UniversalExtensions.GetSqlInfoString(configId, sql, p, dbType, sqlFileInfo);

//获取db.Aop.OnError错误日志打印的错误sql语句,sqlFileInfo可为空
var errorSql= UniversalExtensions.GetSqlErrorString(configId, exp, sqlFileInfo);

//示例
var list = configuration.GetSection("DBS").Get<List<ConnectionConfig>>();
foreach (var item in list)
{
    item.ConnectionString = item.ConnectionString.CheckTrustServerCertificate().CheckEncrypt();
}
var sqlsugarSope = new SqlSugarScope(list, db =>
{
//仅开发模式打印sql语句
#if DEBUG
    foreach (var item in list)
    {
        var configId = item.ConfigId;
        var dbType = item.DbType;
        string sqlFileInfo = db.GetConnection(configId).Ado.SqlStackTrace.MyStackTraceList.GetSqlFileInfo();
        db.GetConnection(configId).Aop.OnLogExecuting = (sql, p) => Console.WriteLine(UniversalExtensions.GetSqlInfoString(configId, sql, p, dbType, sqlFileInfo));
        db.GetConnection(configId).Aop.OnError = (exp) => Console.WriteLine(UniversalExtensions.GetSqlErrorString(configId, exp, sqlFileInfo));
    }
#endif
});
```

#### 5.数据库缓存类(使用方法请参考3.2.3 开启内存缓存)
##### 5.1 内存缓存
``` csharp
using System.Collections;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Caching.Memory;
using SqlSugar;

namespace Easy.SqlSugar.Core.Cache
{
    /// <summary>
    /// Net Core自带内存缓存，在Startup.cs里增加services.AddMemoryCache();
    /// </summary>
    public class MemoryCacheService : ICacheService
    {
        private MemoryCacheHelper cache = new MemoryCacheHelper();

        public void Add<V>(string key, V value)
        {
            cache.Set(key, value);
        }

        public void Add<V>(string key, V value, int cacheDurationInSeconds)
        {
            cache.Set(key, value, cacheDurationInSeconds);
        }

        public bool ContainsKey<V>(string key)
        {
            return cache.Exists(key);
        }

        public V Get<V>(string key)
        {
            return cache.Get<V>(key);
        }

        public IEnumerable<string> GetAllKey<V>()
        {
            return cache.GetCacheKeys();
        }

        public V GetOrCreate<V>(string cacheKey, Func<V> create, int cacheDurationInSeconds = int.MaxValue)
        {
            if (cache.Exists(cacheKey))
            {
                return cache.Get<V>(cacheKey);
            }
            else
            {
                var result = create();
                cache.Set(cacheKey, result, cacheDurationInSeconds);
                return result;
            }
        }

        public void Remove<V>(string key)
        {
            cache.Remove(key);
        }
    }

    public class MemoryCacheHelper
    {
        private static readonly MemoryCache Cache = new MemoryCache(new MemoryCacheOptions());

        /// <summary>
        /// 验证缓存项是否存在
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public bool Exists(string key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            return Cache.TryGetValue(key, out _);
        }

        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存Value</param>
        /// <param name="expiresSliding">滑动过期时长（如果在过期时间内有操作，则以当前时间点延长过期时间）</param>
        /// <param name="expiressAbsoulte">绝对过期时长</param>
        /// <returns></returns>
        public bool Set(string key, object value, TimeSpan expiresSliding, TimeSpan expiressAbsoulte)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            Cache.Set(key, value,
                new MemoryCacheEntryOptions().SetSlidingExpiration(expiresSliding)
                    .SetAbsoluteExpiration(expiressAbsoulte));
            return Exists(key);
        }

        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存Value</param>
        /// <param name="expiresIn">缓存时长</param>
        /// <param name="isSliding">是否滑动过期（如果在过期时间内有操作，则以当前时间点延长过期时间）</param>
        /// <returns></returns>
        public bool Set(string key, object value, TimeSpan expiresIn, bool isSliding = false)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            Cache.Set(key, value,
                isSliding
                    ? new MemoryCacheEntryOptions().SetSlidingExpiration(expiresIn)
                    : new MemoryCacheEntryOptions().SetAbsoluteExpiration(expiresIn));

            return Exists(key);
        }

        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存Value</param>
        /// <returns></returns>
        public void Set(string key, object value)
        {
            Set(key, value, TimeSpan.FromDays(1));
        }

        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存Value</param>
        /// <param name="ts"></param>
        /// <returns></returns>
        public void Set(string key, object value, TimeSpan ts)
        {
            Set(key, value, ts, false);
        }

        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存Value</param>
        /// <param name="ts"></param>
        /// <returns></returns>
        public void Set(string key, object value, int seconds)
        {
            var ts = TimeSpan.FromSeconds(seconds);
            Set(key, value, ts, false);
        }

        #region 删除缓存

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public void Remove(string key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            Cache.Remove(key);
        }

        /// <summary>
        /// 批量删除缓存
        /// </summary>
        /// <returns></returns>
        public void RemoveAll(IEnumerable<string> keys)
        {
            if (keys == null)
                throw new ArgumentNullException(nameof(keys));

            keys.ToList().ForEach(item => Cache.Remove(item));
        }

        #endregion 删除缓存

        #region 获取缓存

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public TResult Get<TResult>(string key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            return Cache.Get<TResult>(key);
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public object Get(string key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            return Cache.Get(key);
        }

        /// <summary>
        /// 获取缓存集合
        /// </summary>
        /// <param name="keys">缓存Key集合</param>
        /// <returns></returns>
        public IDictionary<string, object> GetAll(IEnumerable<string> keys)
        {
            if (keys == null)
                throw new ArgumentNullException(nameof(keys));

            var dict = new Dictionary<string, object>();
            keys.ToList().ForEach(item => dict.Add(item, Cache.Get(item)));
            return dict;
        }

        #endregion 获取缓存

        /// <summary>
        /// 删除所有缓存
        /// </summary>
        public void RemoveCacheAll()
        {
            var l = GetCacheKeys();
            foreach (var s in l)
            {
                Remove(s);
            }
        }

        /// <summary>
        /// 删除匹配到的缓存
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public void RemoveCacheRegex(string pattern)
        {
            IList<string> l = SearchCacheRegex(pattern);
            foreach (var s in l)
            {
                Remove(s);
            }
        }

        /// <summary>
        /// 搜索 匹配到的缓存
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public IList<string> SearchCacheRegex(string pattern)
        {
            var cacheKeys = GetCacheKeys();
            var l = cacheKeys.Where(k => Regex.IsMatch(k, pattern)).ToList();
            return l.AsReadOnly();
        }

        /// <summary>
        /// 获取所有缓存键
        /// </summary>
        /// <returns></returns>
        public List<string> GetCacheKeys()
        {
            //Microsoft.Extensions.Caching.Memory版本修改后里边的名称发生了变更
            var netVersion = Assembly.Load("Microsoft.Extensions.Caching.Memory").GetName().Version.Major;
            if (netVersion <= 5)
            {
                const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
                var entries = Cache.GetType().GetField("_entries", flags).GetValue(Cache);
                var cacheItems = entries as IDictionary;
                var keys = new List<string>();
                if (cacheItems == null) return keys;
                foreach (DictionaryEntry cacheItem in cacheItems)
                {
                    keys.Add(cacheItem.Key.ToString());
                }
                return keys;
            }
            else
            {
                const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
                var coherentState = Cache.GetType().GetField("_coherentState", flags).GetValue(Cache);
                var entries = coherentState.GetType().GetField("_stringEntries", flags).GetValue(coherentState);
                var cacheItems = entries as IDictionary;
                var keys = new List<string>();
                if (cacheItems == null) return keys;
                foreach (DictionaryEntry cacheItem in cacheItems)
                {
                    keys.Add(cacheItem.Key.ToString());
                }
                return keys;
            }
        }
    }
}
```
##### 5.2 Redis缓存
``` csharp
using CSRedis;
using SqlSugar;

namespace Easy.SqlSugar.Core.Cache
{
    /// <summary>
    /// csredis 缓存
    /// </summary>
    public class CsRedisCacheService : ICacheService
    {
        public CsRedisCacheService(CSRedisClient client)
        {
            RedisHelper.Initialization(client);
        }

        //注意:SugarRedis 不要扔到构造函数里面， 一定要单例模式  

        public void Add<V>(string key, V value)
        {
            RedisHelper.Set(key, value);
        }

        public void Add<V>(string key, V value, int cacheDurationInSeconds)
        {
            RedisHelper.Set(key, value, cacheDurationInSeconds);
        }

        public bool ContainsKey<V>(string key)
        {
            return RedisHelper.Exists(key);
        }

        public V Get<V>(string key)
        {
            return RedisHelper.Get<V>(key);
        }

        public IEnumerable<string> GetAllKey<V>()
        {
            //获取redisHelper配置的prefix
            string prefix = RedisHelper.Prefix;
            if (string.IsNullOrWhiteSpace(prefix))
                return RedisHelper.Keys("SqlSugarDataCache.*");
            else
                return RedisHelper.Keys($"{prefix}SqlSugarDataCache.*");
        }

        public V GetOrCreate<V>(string cacheKey, Func<V> create, int cacheDurationInSeconds = int.MaxValue)
        {
            if (this.ContainsKey<V>(cacheKey))
            {
                var result = this.Get<V>(cacheKey);
                if (result == null)
                {
                    return create();
                }
                else
                {
                    return result;
                }
            }
            else
            {
                var result = create();
                this.Add(cacheKey, result, cacheDurationInSeconds);
                return result;
            }
        }

        public void Remove<V>(string key)
        {
            string prefix = RedisHelper.Prefix;
            RedisHelper.Del($"{key.Replace(prefix, "")}");
        }
    }
}
```
项目gitee地址：https://gitee.com/wangbenchi66/nuget