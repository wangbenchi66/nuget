
## 4. EF配置(推荐使用上边的SqlSugar兼容性更强一些功能更完善)
线上nuget引入 版本号随时更新
``` xml
<PackageReference Include="Easy.EF.Core" Version="2024.10.29" />
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