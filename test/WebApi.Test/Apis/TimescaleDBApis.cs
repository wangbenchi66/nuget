using Easy.SqlSugar.Core;
using SqlSugar;

namespace WebApi.Test.Apis
{
    /// <summary>
    /// TimescaleDB测试
    /// </summary>
    public class TimescaleDBApis : BaseApi
    {
        private SimpleClient<StockPrice> _repository => SugarDbManger.GetConfigDbRepository<StockPrice>("TimescaleDB");
        //创建超表最好是在添加数据前处理SELECT create_hypertable('stock_prices', 'time', chunk_time_interval => INTERVAL '1 day');
        //如果表已经存在了，可以直接执行SELECT create_hypertable('stock_prices', 'time', chunk_time_interval => INTERVAL '1 day', if_not_exists => true);

        //插入测试数据 按照时间分区,每分钟一条 模拟一百条股票代码的每分钟一条 添加三十天的数据
        public async Task InsertTestData()
        {
            var random = new Random();
            var stockCodes = Enumerable.Range(1, 100).Select(i => $"STOCK{i:D3}").ToList();
            var dataList = new List<StockPrice>();
            var startTime = DateTime.Now.AddDays(-30);
            for (var time = startTime; time <= DateTime.Now; time = time.AddMinutes(1))
            {
                foreach (var code in stockCodes)
                {
                    dataList.Add(new StockPrice
                    {
                        Time = time,
                        Symbol = code,
                        Price = (decimal)(random.NextDouble() * 100),
                        Volume = random.Next(1000, 10000),
                        Market = "SH"
                    });
                }
            }
            //批量插入数据
            await _repository.Context.Fastest<StockPrice>().PageSize(30000).BulkCopyAsync(dataList);
        }

        //查询最近几小时的数据
        public async Task<object> GetRecentData(int hours)
        {
            string symbol = "STOCK001";
            var sinceTime = DateTime.Now.AddHours(-hours);
            var data = await _repository.AsQueryable()
                .Where(sp => sp.Time >= sinceTime)
                .WhereIF(!string.IsNullOrEmpty(symbol), sp => sp.Symbol == symbol)
                .OrderByDescending(sp => sp.Time)
                .ToListAsync();
            return new
            {
                count = data.Count,
                symbol = symbol,
                time = $"{sinceTime}-{DateTime.Now}",
                data
            };
        }

        //时间跨度 跨chrunk 查询
        public async Task<object> GetDataByTimeRange(DateTime start, DateTime end)
        {
            string symbol = "STOCK001";
            var data = await _repository.AsQueryable()
                .Where(sp => sp.Time >= start && sp.Time <= end)
                .WhereIF(!string.IsNullOrEmpty(symbol), sp => sp.Symbol == symbol)
                .OrderByDescending(sp => sp.Time)
                .ToListAsync();
            return new
            {
                count = data.Count,
                symbol = symbol,
                time = $"{start}-{end}",
                data
            };
        }
    }

    [SugarTable("stock_prices")]
    public class StockPrice
    {
        public DateTime Time { get; set; }
        public string Symbol { get; set; }
        public decimal Price { get; set; }
        public long Volume { get; set; }
        public string Market { get; set; }
    }
}