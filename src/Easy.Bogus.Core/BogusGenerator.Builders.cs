using Bogus;

namespace Easy.Bogus.Core;

public static partial class BogusGenerator
{
    private static string BuildStringValue(Faker faker, string propertyName)
    {
        var key = NormalizeName(propertyName);

        if (ContainsAny(key, "name", "username", "nickname", "realname", "truename", "联系人", "姓名"))
        {
            return faker.Name.FullName();
        }

        if (ContainsAny(key, "phone", "mobile", "telephone", "手机号", "电话"))
        {
            return faker.Random.ReplaceNumbers("1##########");
        }

        if (ContainsAny(key, "email", "mail", "邮箱"))
        {
            return BuildDomesticEmail(faker);
        }

        if (ContainsAny(key, "wechat", "wx", "weixin"))
        {
            return $"wx{faker.Random.AlphaNumeric(8)}";
        }

        if (ContainsAny(key, "qq"))
        {
            return faker.Random.ReplaceNumbers("#########");
        }

        if (ContainsAny(key, "idcard", "identity", "证件", "身份证"))
        {
            return faker.Random.ReplaceNumbers("##################");
        }

        if (ContainsAny(key, "address", "addr", "city", "province", "county", "street", "地址"))
        {
            return BuildDomesticAddress(faker);
        }

        if (ContainsAny(key, "company", "corp", "企业", "公司"))
        {
            return BuildChineseCompanyName(faker);
        }

        if (ContainsAny(key, "url", "link", "avatar", "image", "img", "photo"))
        {
            return faker.Internet.Url();
        }

        if (ContainsAny(key, "title", "subject", "主题", "标题"))
        {
            var topic = faker.PickRandom(ChineseWords);
            var template = faker.PickRandom(ChineseTitleTemplates);
            return string.Format(template, topic);
        }

        if (ContainsAny(key, "content", "desc", "description", "remark", "comment", "memo", "note", "内容", "描述", "备注"))
        {
            return BuildChineseParagraph(faker);
        }

        if (ContainsAny(key, "code", "number", "编号", "单号"))
        {
            return faker.Random.Replace("???-########").ToUpperInvariant();
        }

        return BuildChineseWords(faker, faker.Random.Int(2, 5));
    }

    private static int BuildIntValue(Faker faker, string propertyName)
    {
        var key = NormalizeName(propertyName);

        if (ContainsAny(key, "age", "年龄"))
        {
            return faker.Random.Int(18, 65);
        }

        if (ContainsAny(key, "year", "年份"))
        {
            return faker.Date.Recent().Year;
        }

        if (ContainsAny(key, "month", "月份"))
        {
            return faker.Random.Int(1, 12);
        }

        if (ContainsAny(key, "day", "日期", "天"))
        {
            return faker.Random.Int(1, 28);
        }

        if (ContainsAny(key, "status", "state", "type", "level", "flag", "状态", "类型", "级别"))
        {
            return faker.Random.Int(0, 5);
        }

        if (ContainsAny(key, "count", "num", "total", "quantity", "数量", "总数"))
        {
            return faker.Random.Int(1, 5000);
        }

        if (ContainsAny(key, "score", "rate", "percent", "评分", "分数", "比例"))
        {
            return faker.Random.Int(60, 100);
        }

        if (ContainsAny(key, "sort", "order", "index", "排序", "序号"))
        {
            return faker.Random.Int(1, 200);
        }

        return faker.Random.Int(1, 100);
    }

    private static long BuildLongValue(Faker faker, string propertyName)
    {
        var key = NormalizeName(propertyName);

        if (ContainsAny(key, "id", "snowflake", "标识", "主键"))
        {
            return faker.Random.Long(1000000000000000000, 9000000000000000000);
        }

        return faker.Random.Long(1, int.MaxValue);
    }

    private static double BuildDoubleValue(Faker faker, string propertyName)
    {
        var key = NormalizeName(propertyName);

        if (ContainsAny(key, "amount", "price", "money", "fee", "cost", "salary", "金额", "价格", "费用"))
        {
            return Math.Round(faker.Random.Double(10, 30000), 2);
        }

        if (ContainsAny(key, "rate", "percent", "discount", "tax", "ratio", "比例", "折扣", "税率"))
        {
            return Math.Round(faker.Random.Double(0, 100), 2);
        }

        return Math.Round(faker.Random.Double(0, 1000), 2);
    }

    private static decimal BuildDecimalValue(Faker faker, string propertyName)
    {
        var key = NormalizeName(propertyName);

        if (ContainsAny(key, "amount", "price", "money", "fee", "cost", "salary", "金额", "价格", "费用"))
        {
            return Math.Round((decimal)faker.Random.Double(10, 30000), 2);
        }

        if (ContainsAny(key, "rate", "percent", "discount", "tax", "ratio", "比例", "折扣", "税率"))
        {
            return Math.Round((decimal)faker.Random.Double(0, 100), 2);
        }

        return Math.Round((decimal)faker.Random.Double(0, 1000), 2);
    }

    private static DateTime BuildDateTimeValue(Faker faker, string propertyName)
    {
        var key = NormalizeName(propertyName);
        var now = DateTime.Now;

        if (ContainsAny(key, "birth", "birthday", "出生"))
        {
            return faker.Date.Between(now.AddYears(-60), now.AddYears(-18));
        }

        if (ContainsAny(key, "create", "created", "add", "insert", "创建", "新增"))
        {
            return faker.Date.Between(now.AddYears(-1), now.AddDays(-1));
        }

        if (ContainsAny(key, "update", "modified", "edit", "修改", "更新"))
        {
            return faker.Date.Recent(30);
        }

        if (ContainsAny(key, "start", "begin", "开始"))
        {
            return faker.Date.Between(now.AddMonths(-3), now.AddDays(7));
        }

        if (ContainsAny(key, "end", "expire", "deadline", "结束", "过期", "截止"))
        {
            return faker.Date.Between(now.AddDays(1), now.AddMonths(6));
        }

        return faker.Date.Recent(180);
    }

    private static bool BuildBoolValue(Faker faker, string propertyName)
    {
        var key = NormalizeName(propertyName);

        if (ContainsAny(key, "deleted", "isdeleted", "删除"))
        {
            return faker.Random.Bool(0.05f);
        }

        if (ContainsAny(key, "enabled", "active", "available", "valid", "启用", "有效"))
        {
            return faker.Random.Bool(0.85f);
        }

        return faker.Random.Bool();
    }

    private static string NormalizeName(string propertyName)
    {
        return propertyName.Replace("_", string.Empty).ToLowerInvariant();
    }

    private static bool ContainsAny(string source, params string[] keys)
    {
        return keys.Any(source.Contains);
    }

    private static string BuildChineseWords(Faker faker, int count)
    {
        var words = Enumerable.Range(0, count)
            .Select(_ => faker.PickRandom(ChineseWords));
        return string.Concat(words);
    }

    private static string BuildChineseSentence(Faker faker)
    {
        var head = BuildChineseWords(faker, faker.Random.Int(2, 3));
        var action = faker.PickRandom(new[] { "已完成", "处理中", "待确认", "已同步", "可用", "已归档" });
        var tail = faker.PickRandom(new[] { "请及时关注。", "建议稍后复核。", "可继续下一步操作。", "当前结果仅供参考。" });
        return $"{head}{action}，{tail}";
    }

    private static string BuildChineseParagraph(Faker faker)
    {
        var sentenceCount = faker.Random.Int(2, 4);
        var sentences = Enumerable.Range(0, sentenceCount)
            .Select(_ => BuildChineseSentence(faker))
            .ToList();

        if (faker.Random.Bool(0.35f))
        {
            sentences.Add(faker.PickRandom(ChineseRemarkTemplates));
        }

        return string.Join(string.Empty, sentences);
    }

    private static string BuildDomesticEmail(Faker faker)
    {
        var nameSeed = faker.Name.LastName() + faker.Name.FirstName();
        var local = ToPinyinLocalPart(nameSeed);
        var suffix = faker.Random.Int(10, 9999).ToString();
        var domain = faker.PickRandom(DomesticEmailDomains);
        return $"{local}{suffix}@{domain}";
    }

    private static string BuildChineseCompanyName(Faker faker)
    {
        var city = faker.PickRandom(ChinaCities);
        var prefix = faker.PickRandom(ChineseCompanyPrefixes);
        var suffix = faker.PickRandom(ChineseCompanySuffixes);
        return $"{city}{prefix}{suffix}";
    }

    private static string BuildDomesticAddress(Faker faker)
    {
        var province = faker.PickRandom(ChinaProvinces);
        var city = faker.PickRandom(ChinaCities);
        var district = faker.PickRandom(ChinaDistricts);
        var street = faker.PickRandom(ChineseStreetNames);
        var number = faker.Random.Int(1, 999);
        var building = faker.Random.Int(1, 20);
        var unit = faker.Random.Int(1, 4);
        var room = faker.Random.Int(101, 2802);
        return $"{province}{city}{district}{street}{number}号{building}栋{unit}单元{room}室";
    }

    private static string ToPinyinLocalPart(string source)
    {
        if (string.IsNullOrWhiteSpace(source))
        {
            return "user";
        }

        var buffer = new List<string>();
        foreach (var ch in source)
        {
            if (char.IsLetterOrDigit(ch))
            {
                buffer.Add(char.ToLowerInvariant(ch).ToString());
                continue;
            }

            if (PinyinCharMap.TryGetValue(ch, out var py))
            {
                buffer.Add(py);
            }
        }

        var value = string.Concat(buffer);
        return string.IsNullOrWhiteSpace(value) ? "user" : value;
    }
}
