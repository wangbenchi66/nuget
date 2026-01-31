using Easy.SqlSugar.Core;
using Easy.SqlSugar.Core.Common;
using Microsoft.AspNetCore.Mvc;
using WebApi.Test.Model;

namespace WebApi.Test.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SqlSugarController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<SqlSugarController> _logger;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUserService _userService;

        public SqlSugarController(IUserRepository userRepository, ILogger<SqlSugarController> logger, ICategoryRepository categoryRepository, IUserService userService)
        {
            _userRepository = userRepository;
            _logger = logger;
            _categoryRepository = categoryRepository;
            _userService = userService;
        }


        [HttpGet]
        public async Task<ApiResult> Get()
        {
            //所有操作都有异步方法，增加Async即可
            //查询单个
            var obj = _userService.GetSingle(p => p.Id == 1);
            _logger.LogInformation("查询单个结果：{@obj}", obj);
            //var e = await _userService.ExistsAsync(x => x.Id == 1);

            //var obj1 = _categoryRepository.GetSingle(p => p.ID == 1);
            //_logger.LogInformation("查询单个结果：{@obj1}", obj1);
            return ApiResult.Ok(obj);
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
            var inserOrUpdate = _userRepository.InsertOrUpdate(new User() { Id = 1, Name = "admin" });
            //根据条件添加或更新
            var inserOrUpdate2 = _userRepository.InsertOrUpdate(new User() { Id = 1, Name = "admin" }, x => new { x.Id, x.Name });

            //删除
            var isDelete = _userRepository.Delete(obj);
            //批量删除 
            var isDelete2 = _userRepository.Delete(new List<User>() { new User() { Id = 1 }, new User() { Id = 2 } });
            //根据主键删除
            //var isDelete3 = _userRepository.DeleteByIds(new int[] { 1, 2 });

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

            //普通仓储模式
            /*
                        var list = _userRepository.GetList(p => p.Id > 0);
                        //分页查询 (条件,排序,页码,每页条数)
                        int pgae = 1;
                        int pageSize = 10;
                        int totalCount = 0;
                        var page = _userRepository.AsQueryable().Where(p => p.Id > 0).OrderBy(o => o.Id).ToPageList(pgae, pageSize, ref totalCount);
                        //分页查询 (条件,排序,排序方式,页码,每页条数)
                        var page2 = _userRepository.AsQueryable().Where(p => p.Id > 0).OrderBy(o => o.Id, SqlSugar.OrderByType.Desc).ToPageList(pgae, pageSize, ref totalCount);
                        //分页排序
                        // 设置排序参数
                        List<OrderByModel> orderByModels = new List<OrderByModel>
                                            {
                                                new OrderByModel() { FieldName = "CreateTime", OrderByType = OrderByType.Desc }, // 按 CreateTime 降序排序
                                                new OrderByModel() { FieldName = "Name", OrderByType = OrderByType.Asc } // 按 Name 升序排序
                                            };
                        var page3 = _userRepository.AsQueryable().Where(p => p.Id > 0).OrderBy(orderByModels).ToPageList(pgae, pageSize, ref totalCount);


                        //判断数据是否存在
                        var isAny = _userRepository.AsQueryable().Any(p => p.Id == 1);
                        //获取数据总数
                        var count = _userRepository.AsQueryable().Count(p => p.Id > 0);

                        //添加
                        var user = new User() { Id = 1 };
                        var userId = _userRepository.InsertReturnIdentity(user);
                        //添加指定列
                        var userId2 = _userRepository.AsInsertable(user).InsertColumns(p => new { p.Id }).ExecuteReturnIdentity();
                        //批量添加
                        _userRepository.AsInsertable(new List<User>() { new() { Id = 1 }, new() { Id = 2 } });

                        //修改
                        var isUpdate = _userRepository.Update(user);
                        //修改指定列
                        var isUpdate2 = _userRepository.AsUpdateable(user).UpdateColumns(p => new User() { Name = "2" }).Where(p => p.Name == "test").ExecuteCommand();
                        //根据条件更新 (实体,要修改的列,条件)
                        var isUpdate3 = _userRepository.AsUpdateable(user).UpdateColumns(p => new User() { Name = "2" }).Where(p => p.Id == 1).ExecuteCommand();
                        //批量修改
                        _userRepository.AsUpdateable(new List<User>() { new() { Id = 1 }, new() { Id = 2 } });

                        //删除
                        var isDelete = _userRepository.Delete(user);
                        //批量删除
                        _userRepository.Delete(new List<User>() { new() { Id = 1 }, new() { Id = 2 } });
                        //根据主键删除
                        _userRepository.DeleteByIds(new dynamic[] { 1, 2 });

                        //执行自定义sql
                        //查询
                        var list2 = _userRepository.SqlSugarDbContext.SqlQueryable<User>("select * from test_user");
                        //查询到指定实体
                        var list3 = _userRepository.SqlSugarDbContext.SqlQueryable<User>("select * from test_user").ToList();
                        //执行增删改
                        var count2 = _userRepository.SqlSugarDbContextAdo.ExecuteCommand("update test_user set name='a' where id=1");

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
                        }*/





            return ApiResult.Ok();
        }

        /// <summary>
        /// 测试事务
        /// </summary>
        /// <returns></returns>
        [HttpGet("Tran")]
        public object Tran()
        {
            //事务
            _userRepository.DbContextBeginTransaction(() =>
            {
                _userRepository.Insert(new User() { Name = "99999" });
                _categoryRepository.Insert(new Category() { Title = "99999" });
                //查询结果
                var obj = _userRepository.GetSingle(p => p.Name == "99999");
                var obj1 = _categoryRepository.GetSingle(p => p.Title == "99999");
                return true;
            });

            return "ok";
        }

        //测试添加或更新
        [HttpGet("InsertOrUpdate")]
        public object InsertOrUpdate()
        {
            //添加或更新
            var user = new User() { Id = 1, Name = "admin12" };
            var user2 = new User() { Id = 99999, Name = "admin91", CreateTime = DateTime.Now };
            var list = new List<User> { user, user2 };
            var isInsertOrUpdate = _userRepository.InsertOrUpdate(list, x => new { x.Id, x.Name });
            return "ok";
        }

        //测试sql语句执行
        [HttpGet("SqlQuery")]
        public object SqlQuery()
        {
            //执行自定义sql
            //查询
            var list2 = _userRepository.SqlQuery("select * from j_user", null);
            //查询到指定实体
            var list3 = _userRepository.SqlQuery<User>("select * from j_user", null);
            //查询单条
            var list4 = _userRepository.SqlQuerySingle<User>("select * from j_user", null);
            return "ok";
        }

        /*
                //测试Service
                [HttpGet("Service")]
                public object Service()
                {
                    //查询单个
                    var obj = _userServices.GetSingle(p => p.Id == 1);

                    RefAsync<int> t = 0, p = 0;

                    var list = _userRepository.QueryPage(x => x.Id == 1, " CreateTime desc", 1, 10);

                    _logger.LogInformation("查询单个结果：{@obj}", obj);
                    return "ok";
                }*/

        [HttpGet("Repository")]
        public IActionResult Repository()
        {
            //查询单个
            var obj = _userRepository.GetSingle(p => p.Id == 1);
            _logger.LogInformation("查询单个结果：{@obj}", obj);
            return Ok(obj);
        }

        [HttpGet("Cache")]
        public object Cache()
        {
            //var obj = _userRepository.SqlSugarDbContext.Queryable<User>().Where(p => p.Id == 1).WithCache().First();
            var obj = _userRepository.GetSingle(p => p.Id == 99999);
            _logger.LogInformation("查询单个结果：{@obj}", obj);
            return obj;
        }

        [HttpGet("Update")]
        public object Update()
        {
            var obj = _userRepository.GetSingle(p => p.Id == 99999);
            var user = new User() { Id = 99999, Name = "admin2" };
            //return _userRepository.SqlSugarDbContext.Insertable<User>(user).RemoveDataCache().ExecuteCommand();
            return _userRepository.AsUpdateable(user).RemoveDataCache().ExecuteCommand();
        }


        //并发测试
        [HttpGet("Concurrent")]
        public async Task<object> Concurrent()
        {
            DateTime time = DateTime.Now;
            var tasks = new List<Task>();
            for (int i = 99999; i < 100010; i++)
            {
                tasks.Add(Task.Run(async () =>
                {
                    //var db = _userRepository.CopyNew();
                    var user = new User() { Id = i, Name = $"admin{i}", CreateTime = time };
                    //await _userRepository.UpdateAsync(user, x => new { x.Name });
                    await _userRepository.InsertOrUpdateAsync(user);
                }));
            }

            await Task.WhenAll(tasks);

            return "并发测试完成";
        }

        [HttpGet("ConcurrentParallel")]
        public string ConcurrentParallel()
        {
            DateTime time = DateTime.Now;
            Parallel.For(99999, 100010, new ParallelOptions { MaxDegreeOfParallelism = 10 }, i =>
            {
                var user = new User { Id = i, Name = $"admin{i}", CreateTime = time };
                _userRepository.InsertOrUpdate(user); // 同步方法
            });

            return "Parallel 并发测试完成";
        }


        [HttpGet("ConcurrentUpdate")]
        public async Task<object> ConcurrentUpdate()
        {
            var user = new User() { Id = 99999, Name = $"admin1" };
            await _userRepository.UpdateAsync(user);
            return "并发测试完成";
        }

        /// <summary>
        /// 测试静态的上下文
        /// </summary>
        /// <returns></returns>
        [HttpGet("StaticContext")]
        public object StaticContext()
        {
            //使用静态上下文
            Console.WriteLine(SugarDbManger.Db.ContextID);
            var db = SugarDbManger.GetConfigDb("journal");
            Console.WriteLine(db.ContextID);
            var db2 = SugarDbManger.GetTenantDb<User>();
            Console.WriteLine(db2.ContextID);
            var list = SugarDbManger.GetNewDb();
            Console.WriteLine(list.ContextID);
            //var obj = SugarDbManger.GetInstance().Queryable<User>().Where(p => p.Id == 1).First();
            //return obj;
            return null;
        }

        /// <summary>
        /// 参数化修改测试
        /// </summary>
        /// <returns></returns>
        [HttpGet("ParamUpdate")]
        public async Task<object> ParamUpdate()
        {
            var user = new User() { Id = 99999, Name = "admin_param31" };
            var list = new List<User>
            {
                new User() { Id = 99999, Name = "admin_param31" },
                new User() { Id = 100000, Name = "admin_param1" },
            };
            var parms = SugarEntityExtensions.ToSqlSugarDictionary(list);
            var isUpdate = await _userRepository.ExecuteSqlAsync("update j_user set name=@Name where id=@Id", parms) > 0;
            return isUpdate;
        }
        /// <summary>
        /// 参数化in查询
        /// </summary>
        /// <returns></returns>
        [HttpGet("ParamInQuery")]
        public async Task<object> ParamInQuery()
        {
            var ids = new List<int> { 99999, 100000, 100001 };
            var name = "自定义";
            string sql = "select * from j_user where id IN @ids or name in @name";
            var list = await _userRepository.SqlQueryAsync(SugarEntityExtensions.InSqlReplace(sql), new { ids, name });
            return list;
        }

        //sql执行前aop事件处理添加时间&修改时间
        [HttpGet("AddAopTest")]
        public async Task<object> AddAopTest()
        {
            var db = SugarDbManger.GetConfigDb("journal");
            var aopTest = new AopTest() { };
            var snowId = await db.Insertable(aopTest).ExecuteReturnSnowflakeIdAsync();
            return snowId;
        }
        [HttpGet("EditAopTest")]
        public async Task<object> EditAopTest(long id)
        {
            var db = SugarDbManger.GetConfigDb("journal");
            var aopTest = db.Queryable<AopTest>().InSingle(id);
            aopTest.UpdateTime = DateTime.Now.AddDays(1000);
            var res = await db.Updateable(aopTest).ExecuteCommandHasChangeAsync();
            return res;
        }
    }
}