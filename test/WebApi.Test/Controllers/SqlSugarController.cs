using Microsoft.AspNetCore.Mvc;
using UnitTest.Repository;

namespace WebApi.Test.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SqlSugarController : ControllerBase
    {

        private readonly IUserRepository _userRepository;

        public SqlSugarController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public object Get()
        {
            //所有操作都有异步方法，增加Async即可
            //查询单个
            var obj = _userRepository.GetSingle(p => p.Id == 1);
            return obj;
            /*//查询列表
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
            var isUpdate = _userRepository.Update(new User() { Id = 1 });
            //修改指定列
            var isUpdate2 = _userRepository.Update(p => new User() { Id = 1 }, p => p.Id == 1);
            //根据条件更新 (实体,要修改的列,条件)
            var isUpdate3 = _userRepository.Update(obj, new List<string>() { "name" }, new List<string>() { "Id = 1" });
            //批量修改
            var isUpdate4 = _userRepository.Update(new List<User>() { new User() { Id = 1 }, new User() { Id = 2 } });
            //删除
            var isDelete = _userRepository.Delete(obj);
            //批量删除
            var isDelete2 = _userRepository.Delete(new List<User>() { new User() { Id = 1 }, new User() { Id = 2 } });
            //根据主键删除
            var isDelete3 = _userRepository.DeleteByIds([1, 2]);


            //执行自定义sql
            //查询
            var list2 = _userRepository.SqlQuery("select * from user", null);
            //查询到指定实体
            var list3 = _userRepository.SqlQuery<User>("select * from user", null);
            //查询分页到指定实体
            var page4 = _userRepository.SqlPageQuery<User>("select * from user", null, 1, 10);
            //执行增删改
            var count2 = _userRepository.ExecuteSql("update user set name='a' where id=1", null);


            //执行事务
            var BeginTranRes = _userRepository.DbContextBeginTransaction(() =>
            {
                _userRepository.Insert(new User() { Id = 1 });
                _userRepository.Insert(new User() { Id = 2 });
                return true;
            });*/

            return "ok";
        }
    }
}