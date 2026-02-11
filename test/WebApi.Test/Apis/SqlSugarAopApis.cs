using Easy.SqlSugar.Core;
using SqlSugar;

namespace WebApi.Test.Apis
{
    /// <summary>
    /// sqlsugar aop测试
    /// </summary>
    public class SqlSugarAopApis : BaseApi
    {
        private readonly ISqlSugarClient _sqlSugarClient;
        private readonly BaseSqlSugarRepository<TestUser> _repository;

        public SqlSugarAopApis(ISqlSugarClient sqlSugarClient, BaseSqlSugarRepository<TestUser> repository)
        {
            _sqlSugarClient = sqlSugarClient;
            _repository = repository;
        }

        /// <summary>
        /// 测试sqlsugar aop日志
        /// </summary>
        /// <returns></returns>
        public async Task<object> TestAopLogging()
        {
            //var users = await _repository.AsQueryable().ToListAsync();
            var users = await _sqlSugarClient.AsTenant().GetConnectionScopeWithAttr<TestUser>().Queryable<TestUser>().ToListAsync();
            return users;
        }
        /// <summary>
        /// 测试aop添加时间
        /// </summary>
        /// <returns></returns>
        public async Task TestAopAddTime()
        {
            //插入100万条测试数据
            List<TestUser> users = new List<TestUser>();
            for (int i = 0; i < 1000000; i++)
            {
                users.Add(new TestUser
                {
                    name = $"用户{i + 1}"
                });
            }
            await _repository.SqlSugarDbContext.Fastest<TestUser>().PageSize(300000).BulkCopyAsync(users);
        }
        //测试aop修改时间
        public async Task TestAopUpdateTime()
        {
            var users = await _repository.AsQueryable().Where(x => x.id > 7003215).OrderByDescending(x => x.id).Take(1000).ToListAsync();
            System.Console.WriteLine($"查询到{users.Count}条数据");
            users.ForEach(u =>
            {
                u.name = u.id.ToString();
                //u.updatetime = DateTime.Now.AddDays(-15);
            });
            System.Console.WriteLine($"开始更新数据");
            await _repository.SqlSugarDbContext.Updateable(users).PageSize(300).ExecuteCommandAsync();
            //await _repository.SqlSugarDbContext.Fastest<TestUser>().EnableDataAop().PageSize(300).BulkUpdateAsync(users);
        }

        //修改最后一条数据
        public async Task TestAopUpdateLast()
        {
            var user = await _repository.AsQueryable().OrderBy(u => u.id, OrderByType.Desc).FirstAsync();
            user.name += user.id;
            await _repository.UpdateAsync(user);
        }
    }

    /// <summary>
    /// 用户测试表
    ///</summary>
    [Tenant("Pg")]
    [SugarTable("test_user")]
    public class TestUser
    {
        /// <summary>
        /// 主键 
        ///</summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public long id { get; set; }
        /// <summary>
        ///  
        ///</summary>
        public string name { get; set; }
        /// <summary>
        /// 添加时间 
        ///</summary>
        public DateTime? createtime { get; set; }
        /// <summary>
        /// 修改时间 
        ///</summary>
        //[SugarColumn(UpdateServerTime = true)]
        public DateTime? updatetime { get; set; }
    }
}