using Easy.SqlSugar.Core;
using SqlSugar;

namespace WebApi.Test.Apis
{
    public class SqlSuagrTestApis:BaseApi
    {
        private readonly BaseSqlSugarRepository<BulkNum> _bulkNumRepository;
        private readonly ISqlSugarClient _sqlSugarClient;

        public SqlSuagrTestApis(BaseSqlSugarRepository<BulkNum> bulkNumRepository,ISqlSugarClient sqlSugarClient)
        {
            _bulkNumRepository = bulkNumRepository;
            _sqlSugarClient = sqlSugarClient;
        }
        public async Task Init()
        {
            _sqlSugarClient.CodeFirst.InitTables(typeof(BulkNum));
        }

        public async Task<object> Get()
        {
            return await _bulkNumRepository.GetSingleAsync(x=>x.Id==1);
        }
        public async Task<object> Add1()
        {
            //使用遍历的模式插入三条
            var objs= Enumerable.Range(1, 3)
                .Select(_ => new BulkNum { Name = "test", CreateTime = DateTime.Now, Num = 1.01m })
                .ToList();
            return await _bulkNumRepository.InsertAsync(objs);
        }

        public async Task<object> Update1()
        {
            var objs = await _bulkNumRepository.GetListAsync(x => true);
            //给所有的Num加1.1
            return await _bulkNumRepository.SqlSugarDbContext.Updateable<BulkNum>(objs)
                .UpdateColumns(x => new BulkNum { Num = x.Num + 1.1m})
                //.Where(x=>x.Id>0)
                //.Where(x=>x.Id==1)
                .WhereColumns(x=>x.Id)
                .ExecuteCommandAsync();
        }
    }

    [Tenant("journal")]
    [SugarTable("bulk_num")]
    public class BulkNum
    {
        [SugarColumn(IsPrimaryKey = true,IsIdentity = true)]
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreateTime { get; set; }
        public decimal Num { get; set; }
    }
}