# Easy.Bogus.Core

基于 [Bogus](https://github.com/bchavez/Bogus) 封装的中文语境虚拟数据生成器，通过**属性名语义识别**自动填充符合业务含义的中文测试数据，无需手动编写每个字段的规则。

---

## 安装

```bash
dotnet add package Easy.Bogus.Core
```

---

## 快速开始

```csharp
using Easy.Bogus.Core;

// 生成单个对象
var order = BogusGenerator.GetFake<Order>();

// 批量生成
var orders = BogusGenerator.GetFake<Order>(10);
```

---

## API 说明

### `BogusGenerator.GetFake<T>()`

生成单个虚拟实体对象，可追加自定义规则。

```csharp
// 基础用法
var user = BogusGenerator.GetFake<UserInfo>();

// 追加自定义规则（会覆盖同名字段的默认规则）
var user = BogusGenerator.GetFake<UserInfo>(faker =>
{
    faker.RuleFor(x => x.Status, _ => 1);
    faker.RuleFor(x => x.Remark, _ => "指定备注内容");
});
```

---

### `BogusGenerator.GetFake<T>(int count)`

批量生成指定数量的虚拟实体列表。

```csharp
// 生成 20 条订单数据
var orders = BogusGenerator.GetFake<Order>(20);

// 批量生成并追加自定义规则
var orders = BogusGenerator.GetFake<Order>(20, faker =>
{
    faker.RuleFor(x => x.Status, _ => OrderStatus.Paid);
});
```

---

### `BogusGenerator.CreateEntityFaker<T>()`

返回已预配置好默认规则的 `Faker<T>` 实例，适合需要复用同一个 Faker 多次生成，或需要链式调用 `.Generate()` / `.GenerateForever()` 的场景。

```csharp
var faker = BogusGenerator.CreateEntityFaker<Order>(f =>
{
    f.RuleFor(x => x.Phone, _ => "13800138000");
});

// 多次复用
var batch1 = faker.Generate(5);
var batch2 = faker.Generate(10);

// 无限流
foreach (var item in faker.GenerateForever().Take(100))
{
    // 处理每条数据...
}
```

---

### `BogusGenerator.CreateFaker()`

返回原始 `Faker` 实例（默认 `zh_CN` 语言）。适合需要直接调用 Bogus 底层 API 生成局部随机值的场景。

```csharp
var faker = BogusGenerator.CreateFaker();

string phone    = faker.Random.ReplaceNumbers("1##########");
string city     = faker.PickRandom("北京", "上海", "成都", "杭州");
string trackNo  = faker.Random.Replace("TRK-########");
DateTime recent = faker.Date.Recent(30);
```

---

## 属性名语义识别规则

`BogusGenerator` 会根据属性名（不区分大小写，忽略下划线）自动匹配对应的中文语境数据。

### `string` 类型

| 属性名关键字 | 生成内容 | 示例 |
|---|---|---|
| `name` / `username` / `nickname` / `realname` / `姓名` / `联系人` | 中文姓名 | 张伟 |
| `phone` / `mobile` / `tel` / `手机号` / `电话` | 手机号 | 13812345678 |
| `email` / `mail` / `邮箱` | 国内邮箱（拼音 + 数字） | zhangwei123@163.com |
| `wechat` / `wx` / `weixin` | 微信号 | wx3a8f2c1d |
| `qq` | QQ 号 | 385621047 |
| `idcard` / `identity` / `证件` / `身份证` | 18 位身份证号 | 420106198803124521 |
| `address` / `addr` / `city` / `province` / `street` / `地址` | 完整中国地址 | 广东省广州市天河区天府大道88号3栋2单元1501室 |
| `company` / `corp` / `企业` / `公司` | 中文公司名 | 杭州星河科技有限公司 |
| `url` / `link` / `avatar` / `image` / `img` / `photo` | URL 地址 | https://... |
| `title` / `subject` / `主题` / `标题` | 中文标题 | 关于订单的处理通知 |
| `content` / `desc` / `remark` / `comment` / `备注` / `描述` | 中文段落 | 系统数据处理中，建议稍后复核。 |
| `code` / `no` / `number` / `编号` / `单号` | 大写编码 | ABC-20481563 |
| 其他 | 2~5 个中文词拼接 | 服务配置策略 |

### `int` 类型

| 属性名关键字 | 范围 |
|---|---|
| `age` / `年龄` | 18 ~ 65 |
| `year` / `年份` | 近期年份 |
| `month` / `月份` | 1 ~ 12 |
| `day` / `日期` / `天` | 1 ~ 28 |
| `status` / `state` / `type` / `level` / `flag` / `状态` / `类型` / `级别` | 0 ~ 5 |
| `count` / `num` / `total` / `quantity` / `数量` / `总数` | 1 ~ 5000 |
| `score` / `rate` / `percent` / `评分` / `分数` | 60 ~ 100 |
| `sort` / `order` / `index` / `排序` / `序号` | 1 ~ 200 |
| 其他 | 1 ~ 100 |

### `long` 类型

| 属性名关键字 | 范围 |
|---|---|
| `id` / `snowflake` / `标识` / `主键` | 雪花 ID 级别（10^18 量级） |
| 其他 | 1 ~ int.MaxValue |

### `decimal` / `double` / `float` 类型

| 属性名关键字 | 范围 |
|---|---|
| `amount` / `price` / `money` / `fee` / `cost` / `salary` / `金额` / `价格` / `费用` | 10 ~ 30000（保留 2 位） |
| `rate` / `percent` / `discount` / `tax` / `ratio` / `折扣` / `税率` | 0 ~ 100（保留 2 位） |
| 其他 | 0 ~ 1000（保留 2 位） |

### `DateTime` 类型

| 属性名关键字 | 范围 |
|---|---|
| `birth` / `birthday` / `出生` | 近 60 年内（18~60 岁） |
| `create` / `created` / `add` / `insert` / `创建` / `新增` | 过去 1 年内 |
| `update` / `modified` / `edit` / `修改` / `更新` | 最近 30 天 |
| `start` / `begin` / `开始` | 近 3 个月 ~ 未来 7 天 |
| `end` / `expire` / `deadline` / `结束` / `过期` / `截止` | 未来 1 天 ~ 6 个月 |
| 其他 | 最近 180 天 |

### `bool` 类型

| 属性名关键字 | 概率 |
|---|---|
| `deleted` / `isdeleted` / `删除` | 5% 为 true（表示未删除居多） |
| `enabled` / `active` / `available` / `valid` / `启用` / `有效` | 85% 为 true（表示有效居多） |
| 其他 | 50% |

---

## 主键字段跳过规则

以下情况的属性会被自动跳过，不生成随机值：

- 标记了 `[Key]`（`System.ComponentModel.DataAnnotations`）
- 标记了 `[SugarColumn(IsPrimaryKey = true)]`（SqlSugar，运行时反射识别，**无需引用 SqlSugar 包**）
- 属性没有 `public` 的 `set` 访问器
- 索引器属性

```csharp
public class Order
{
    [Key]
    public long Id { get; set; }          // ← 跳过，保持默认值 0

    public string OrderNo { get; set; }   // ← 自动填充编码格式字符串

    public string UserName { get; set; }  // ← 自动填充中文姓名

    public string ReadOnly => "x";        // ← 跳过，没有 setter
}
```

---

## 枚举类型支持

枚举字段会从枚举值中随机选取一个，无需任何配置。

```csharp
public enum OrderStatus { Init, Paid, Shipping, Completed }

public class Order
{
    public OrderStatus Status { get; set; }  // 自动从枚举值中随机选取
}

var order = BogusGenerator.GetFake<Order>();
// order.Status 可能是 Init / Paid / Shipping / Completed 之一
```

可空枚举同样支持：

```csharp
public OrderStatus? Status { get; set; }  // Nullable 枚举也会自动识别
```

---

## 完整示例

```csharp
using System.ComponentModel.DataAnnotations;
using Easy.Bogus.Core;

public class Order
{
    [Key]
    public long Id { get; set; }

    public string OrderNo { get; set; } = "";
    public string UserName { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Email { get; set; } = "";
    public string Address { get; set; } = "";
    public decimal Amount { get; set; }
    public int Quantity { get; set; }
    public DateTime CreateTime { get; set; }
    public bool IsEnabled { get; set; }
    public OrderStatus Status { get; set; }
    public string Remark { get; set; } = "";
}

public enum OrderStatus { Init, Paid, Shipping, Completed }

// 1. 生成单个
var order = BogusGenerator.GetFake<Order>();

// 2. 批量生成
var orders = BogusGenerator.GetFake<Order>(20);

// 3. 自定义规则覆盖
var vip = BogusGenerator.GetFake<Order>(faker =>
{
    faker.RuleFor(x => x.Status, _ => OrderStatus.Paid);
    faker.RuleFor(x => x.Amount, f => f.Random.Decimal(1000, 9999));
});

// 4. 复用 Faker 多次生成
var entityFaker = BogusGenerator.CreateEntityFaker<Order>(faker =>
{
    faker.RuleFor(x => x.Phone, _ => "13800138000");
});
var batch1 = entityFaker.Generate(5);
var batch2 = entityFaker.Generate(10);

// 5. 直接使用底层 Faker
var faker = BogusGenerator.CreateFaker();
var trackNo = faker.Random.Replace("TRK-########");
```

---

## 支持的目标框架

| 框架 | 版本 |
|---|---|
| .NET Standard | 2.0 |
| .NET | 6.0 |
| .NET | 8.0 |

---

## 依赖

| 包 | 版本 |
|---|---|
| [Bogus](https://www.nuget.org/packages/Bogus) | ≥ 35.6.1 |
| System.ComponentModel.Annotations | ≥ 5.0.0 |
