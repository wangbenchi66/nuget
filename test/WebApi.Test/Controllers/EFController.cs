using Microsoft.AspNetCore.Mvc;
using UnitTest.Repository;
using WBC66.EF.Core;

namespace WebApi.Test.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EFController : ControllerBase
    {
        private readonly IUserEFRepository _userRepository;

        public EFController(IUserEFRepository userEFRepository)
        {
            _userRepository = userEFRepository;
        }

        [HttpGet]
        public object Get()
        {
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
            var userId = _userRepository.Insert(new UserEF() { Id = 1 });


            //添加指定列 (有问题)
            var userId2 = _userRepository.Insert(new UserEF() { Id = 1 }, p => new { p.Id });

            //批量添加
            var userIds = _userRepository.Insert(new List<UserEF>() { new UserEF() { Id = 1 }, new UserEF() { Id = 2 } });
            //修改
            var isUpdate = _userRepository.Update(obj);

            //修改指定列 (有问题)
            var isUpdate2 = _userRepository.Update(obj, new List<string>() { "Name" });
            //根据条件更新 (实体,要修改的列,条件) 有问题
            var isUpdate3 = _userRepository.Update(obj, new List<string>() { "name" }, new List<string>() { "Id = 1" });

            //批量修改
            var isUpdate4 = _userRepository.Update(new List<UserEF>() { new UserEF() { Id = 1 }, new UserEF() { Id = 2, Name = "test1" } });

            //删除
            var isDelete = _userRepository.Delete(obj);
            //批量删除  有问题
            var isDelete2 = _userRepository.Delete(new List<UserEF>() { new UserEF() { Id = 2 }, new UserEF() { Id = 3 } });

            //根据主键删除 有问题
            var isDelete3 = _userRepository.DeleteByIds<UserEF>([3, 2]);

            //执行自定义sql
            //查询
            var list2 = _userRepository.SqlQuery("select * from test_user", null);
            //查询到指定实体
            var list3 = _userRepository.SqlQuery<UserEF>("select * from test_user", null);
            //执行增删改
            var count2 = _userRepository.ExecuteSql("update test_user set name='a' where id=1", null);
            //查询分页到指定实体
            var page4 = _userRepository.SqlPageQuery<UserEF>("select * from test_user", null, 1, 1);
            var page4Count = page4.TotalCount;

            //执行事务
            var BeginTranRes = _userRepository.DbContextBeginTransaction(() =>
            {
                _userRepository.Insert(new UserEF() { Id = 1 });
                _userRepository.Insert(new UserEF() { Id = 2 });
                return true;
            });

            return "ok";
        }
    }
}