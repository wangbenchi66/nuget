using Microsoft.AspNetCore.Mvc;
using SqlSugar;

namespace WebApi.Test.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SqlSugarController : ControllerBase
    {
        private readonly UserRepository _userRepository;
        private readonly ILogger<SqlSugarController> _logger;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUserService _userServices;

        public SqlSugarController(UserRepository userRepository, ILogger<SqlSugarController> logger, ICategoryRepository categoryRepository, IUserService userServices)
        {
            _userRepository = userRepository;
            _logger = logger;
            _categoryRepository = categoryRepository;
            _userServices = userServices;
        }

        [HttpGet]
        public object Get()
        {
            //所有操作都有异步方法，增加Async即可
            //查询单个
            var obj = _userRepository.GetSingle(p => p.Id == 1);
            _logger.LogInformation("查询单个结果：{@obj}", obj);

            var obj1 = _categoryRepository.GetSingle(p => p.ID == 1);
            _logger.LogInformation("查询单个结果：{@obj1}", obj1);
            //return obj;
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
            //根据条件添加并更新指定列
            var isInsertOrUpdate = _userRepository.InsertOrUpdate(user, x => new { x.Name }, x => new { x.Id, x.Name });

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





            return "ok";
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
            var user = new User() { Id = 1, Name = "admin1" };
            var user2 = new User() { Id = 99999, Name = "admin99" };
            var list = new List<User> { user, user2 };
            var isInsertOrUpdate = _userRepository.InsertOrUpdate(list, x => new { x.Name, x.Id }, x => new { x.Id, x.Name });
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

        //测试Service
        [HttpGet("Service")]
        public object Service()
        {
            //查询单个
            var obj = _userServices.GetSingle(p => p.Id == 1);
            _logger.LogInformation("查询单个结果：{@obj}", obj);
            return "ok";
        }

    }
}