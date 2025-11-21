using Easy.SqlSugar.Core;
using SqlSugar;

namespace WebApi.Test.Apis
{
    public class SnowFlakeApis : BaseApi
    {
        private readonly ISqlSugarClient _client;

        public SnowFlakeApis(ISqlSugarClient client)
        {
            _client = client;
        }

        public object Get()
        {
            return _client.Queryable<SnowFlake>().OrderBy(it => it.Id).ToList();
        }

        public object GetById(long id)
        {
            return SugarDbManger.Db.Queryable<SnowFlake>().InSingle(id);
        }

        public async Task<object> Post()
        {
            var obj = new SnowFlake()
            {
                Name = new Random().Next(1000, 9999).ToString(),
                AddTime = DateTime.Now,
                //Id = YitIdHelper.NextId()
            };
            return await SugarDbManger.Db.Insertable(obj).ExecuteReturnSnowflakeIdAsync();
            //return await SugarDbManger.Db.Insertable(obj).ExecuteReturnSnowflakeIdAsync();
        }

        /// <summary>
        /// 根据雪花id解析
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public object GetBySnowFlakeId(long id)
        {
            return UniversalExtensions.ParseSugarSnowflakeId(id);
        }
        public object GetYitBySnowFlakeId(long id)
        {
            return UniversalExtensions.ParseYitSnowflakeId(id);
        }
    }

    [SugarTable("SnowFlake")]
    public class SnowFlake
    {
        [SugarColumn(IsPrimaryKey = true)]
        public long Id { get; set; }
        public string Name { get; set; }
        public DateTime AddTime { get; set; }
    }
}