using Easy.SqlSugar.Core;
using SqlSugar;
using Yitter.IdGenerator;

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
                Id = YitIdHelper.NextId()
            };
            return await SugarDbManger.Db.Insertable(obj).ExecuteCommandAsync();
            //return await SugarDbManger.Db.Insertable(obj).ExecuteReturnSnowflakeIdAsync();
        }

        /// <summary>
        /// 根据雪花id解析
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public object GetBySnowFlakeId(long id)
        {
            SnowflakeIdParser.ParseSnowflakeId(id);
            return new
            {
                Message = "解析完成，请查看控制台输出"
            };
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

    /// <summary>
    /// 雪花id解析
    /// </summary>
    public static class SnowflakeIdParser
    {
        //这是 Twitter 的雪花算法的时间基准（epoch），即从 1970 年 1 月 1 日 UTC 到 2010 年 11 月 4 日 UTC 的毫秒数
        private const long Twepoch = 1288834974657L;

        /// <summary>
        /// 解析雪花ID
        /// </summary>
        /// <param name="id"></param>
        public static void ParseSnowflakeId(long id)
        {
            long timestamp = (id >> 22) + Twepoch;
            long datacenterId = (id >> 17) & 0x1F; // 5 位
            long workerId = (id >> 12) & 0x1F;     // 5 位
            long sequence = id & 0xFFF;            // 12 位

            // 将 UTC 时间转为北京时间
            DateTimeOffset utcTime = DateTimeOffset.FromUnixTimeMilliseconds(timestamp);
            DateTime beijingTime = utcTime.ToOffset(TimeSpan.FromHours(8)).DateTime;
            Console.WriteLine($"{new string('-', 30)}");
            Console.WriteLine($"雪花ID: {id}");
            Console.WriteLine($"生成时间（北京时间）: {beijingTime:yyyy-MM-dd HH:mm:ss.fff}");
            Console.WriteLine($"数据中心ID: {datacenterId}");
            Console.WriteLine($"工作机器ID: {workerId}");
            Console.WriteLine($"序列号: {sequence}");
        }

        private const long datacenterId = 1; // 数据中心ID
        private const long workerId = 1; // 工作机器ID
        private const long sequence = 0; // 序列号

        /// <summary>
        /// 生成雪花ID
        /// </summary>
        /// <param name="datacenterId"></param>
        /// <param name="workerId"></param>
        /// <param name="sequence"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static long GenerateSnowflakeId(long datacenterId, long workerId, long sequence)
        {
            if (datacenterId < 0 || datacenterId > 31)
                throw new ArgumentOutOfRangeException(nameof(datacenterId), "数据中心ID必须在0到31之间");
            if (workerId < 0 || workerId > 31)
                throw new ArgumentOutOfRangeException(nameof(workerId), "工作机器ID必须在0到31之间");
            if (sequence < 0 || sequence > 4095)
                throw new ArgumentOutOfRangeException(nameof(sequence), "序列号必须在0到4095之间");
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - Twepoch;
            return (timestamp << 22) | (datacenterId << 17) | (workerId << 12) | sequence;
        }
    }
}