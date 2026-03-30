using System.ComponentModel.DataAnnotations;
using Easy.Bogus.Core;

namespace WebApi.Test.Apis
{
    /// <summary>
    /// BogusGenerator 使用示例
    /// </summary>
    public class BogusGeneratorApis : BaseApi
    {
        /// <summary>
        /// 1. 最简单用法：生成单个对象
        /// </summary>
        public object GetSingle()
        {
            var data = BogusGenerator.GetFake<DemoOrder>();
            return data;
        }

        /// <summary>
        /// 2. 批量生成
        /// </summary>
        public object GetList(int count = 5)
        {
            var safeCount = Math.Clamp(count, 1, 50);
            return BogusGenerator.GetFake<DemoOrder>(safeCount);
        }

        /// <summary>
        /// 3. 在 GetFake 中追加自定义规则
        /// </summary>
        public object GetWithCustomRule()
        {
            var data = BogusGenerator.GetFake<DemoOrder>(faker =>
            {
                faker.RuleFor(x => x.Status, _ => OrderStatus.Paid);
                faker.RuleFor(x => x.Remark, _ => "接口演示：已强制设置为已支付");
                faker.RuleFor(x => x.Amount, f => Math.Round(f.Random.Decimal(99, 399), 2));
            });

            return data;
        }

        /// <summary>
        /// 4. 使用 CreateEntityFaker 复用规则并多次生成
        /// </summary>
        public object GetByEntityFaker(int count = 3)
        {
            var safeCount = Math.Clamp(count, 1, 30);

            var entityFaker = BogusGenerator.CreateEntityFaker<DemoOrder>(faker =>
            {
                faker.RuleFor(x => x.CreateTime, f => f.Date.Recent(15));
                faker.RuleFor(x => x.Phone, _ => "13800138000");
            });

            var list = entityFaker.Generate(safeCount);
            return list;
        }

        /// <summary>
        /// 5. 使用 CreateFaker 生成通用随机值
        /// </summary>
        public object GetByCommonFaker()
        {
            var faker = BogusGenerator.CreateFaker();
            return new
            {
                追踪号 = faker.Random.Replace("TRK-########"),
                城市 = faker.PickRandom("北京", "上海", "杭州", "深圳", "成都"),
                备注 = faker.Lorem.Sentence(8),
                时间戳 = DateTimeOffset.Now.ToUnixTimeMilliseconds()
            };
        }

        /// <summary>
        /// 6. 演示主键/Key 字段会被跳过自动赋值
        /// </summary>
        public object GetPrimaryKeyBehavior()
        {
            var data = BogusGenerator.GetFake<DemoOrder>();
            return new
            {
                data.Id,
                Key是否为空 = data.Id == 0,
                data.OrderNo,
                data.UserName
            };
        }

        private class DemoOrder
        {
            [Key]
            public long Id { get; set; }

            public string? OrderNo { get; set; }

            public string UserName { get; set; } = string.Empty;

            public string Phone { get; set; } = string.Empty;

            public string? Email { get; set; } = string.Empty;

            public string Address { get; set; } = string.Empty;

            public decimal? Amount { get; set; }

            public int? Quantity { get; set; }

            public DateTime? CreateTime { get; set; }

            public bool? IsEnabled { get; set; }

            public OrderStatus? Status { get; set; }

            public string Remark { get; set; } = string.Empty;
        }

        private enum OrderStatus
        {
            Init = 0,
            Paid = 1,
            Shipping = 2,
            Completed = 3
        }
    }
}