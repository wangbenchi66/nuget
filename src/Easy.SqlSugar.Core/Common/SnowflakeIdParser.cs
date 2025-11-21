using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easy.SqlSugar.Core
{
    public static class SnowflakeIdParser
    {
        //这是 Twitter 的雪花算法的时间基准（epoch），即从 1970 年 1 月 1 日 UTC 到 2010 年 11 月 4 日 UTC 的毫秒数
        private const long _twepoch = 1288834974657L;

        /// <summary>
        /// 解析雪花ID
        /// </summary>
        /// <param name="id"></param>
        public static SnowflakeIdParts ParseSqlSugarSnowflakeId(long id)
        {
            long timestamp = (id >> 22) + _twepoch;
            long datacenterId = (id >> 17) & 0x1F; // 5 位
            long workerId = (id >> 12) & 0x1F;     // 5 位
            long sequence = id & 0xFFF;            // 12 位
            // 将 UTC 时间转为本地时间
            DateTimeOffset utcTime = DateTimeOffset.FromUnixTimeMilliseconds(timestamp);
            DateTime beijingTime = utcTime.ToLocalTime().DateTime;
            return new SnowflakeIdParts
            {
                Timestamp = utcTime,
                Date = beijingTime,
                DatacenterId = datacenterId,
                WorkerId = workerId,
                Sequence = sequence,
                SnowflakeId = id
            };
        }

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
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - _twepoch;
            return (timestamp << 22) | (datacenterId << 17) | (workerId << 12) | sequence;
        }

        /// <summary>
        /// 解析Yitter雪花ID
        /// </summary>
        /// <param name="id"></param>
        /// <param name="baseTime"></param>
        /// <param name="workerBits"></param>
        /// <param name="seqBits"></param>
        public static SnowflakeIdParts ParseYitBySnowFlakeId(long id, DateTime baseTime, int workerBits, int seqBits)
        {
            long seqMask = (1L << seqBits) - 1;
            long workerMask = (1L << workerBits) - 1;

            // 1. 取序列号（低 seqBits 位）
            long seq = id & seqMask;

            // 2. 取 WorkerId
            long workerId = (id >> seqBits) & workerMask;

            // 3. 取时间戳（剩下的高位）
            long timestamp = id >> (workerBits + seqBits);
            DateTime time = baseTime.AddMilliseconds(timestamp);
            //转换为本地时间
            time = time.ToLocalTime();
            return new SnowflakeIdParts
            {
                Timestamp = new DateTimeOffset(time),
                Date = time,
                DatacenterId = 0, // Yitter雪花ID没有数据中心ID
                WorkerId = workerId,
                Sequence = seq,
                SnowflakeId = id
            };
        }
    }
    /// <summary>
    /// 雪花ID解析结果
    /// </summary>
    public class SnowflakeIdParts
    {
        /// <summary>
        /// 雪花id
        /// </summary>
        public long SnowflakeId { get; set; }
        /// <summary>
        /// 时间戳
        /// </summary>
        public DateTimeOffset Timestamp { get; set; }
        /// <summary>
        /// 时间日期
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// 数据中心ID
        /// </summary>
        public long DatacenterId { get; set; }
        /// <summary>
        /// 工作机器ID
        /// </summary>
        public long WorkerId { get; set; }
        /// <summary>
        /// 序列号
        /// </summary>
        public long Sequence { get; set; }
    }
}