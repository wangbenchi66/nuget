using Bogus;

namespace Easy.Bogus.Core;


/// <summary>
/// Bogus 数据生成器 
/// Bogus地址https://github.com/bchavez/Bogus?tab=readme-ov-file
/// </summary>
public static partial class BogusGenerator
{

    /// <summary>
    /// 创建一个新的通用 Faker。
    /// 适合补充一些局部随机字段或生成通用随机值。
    /// </summary>
    public static Faker CreateFaker(string locale = "zh_CN")
    {
        return new Faker(locale);
    }

    /// <summary>
    /// 创建一个已配置好默认规则的 Faker&lt;T&gt;。
    /// 适合需要继续链式调用 Generate、GenerateForever、RuleFor 的场景。
    /// </summary>
    public static Faker<T> CreateEntityFaker<T>(Action<Faker<T>>? customRules = null) where T : class, new()
    {
        var faker = new Faker<T>("zh_CN");
        foreach (var rule in GetPropertyRules(typeof(T)))
        {
            switch (rule.Kind)
            {
                case FakeValueKind.String:
                    faker.RuleFor(rule.PropertyName, f => rule.IsNullable && f.Random.Bool(0.2f) ? (object?)null : BuildStringValue(f, rule.PropertyName));
                    break;
                case FakeValueKind.Int:
                    faker.RuleFor(rule.PropertyName, f => rule.IsNullable && f.Random.Bool(0.2f) ? (object?)null : BuildIntValue(f, rule.PropertyName));
                    break;
                case FakeValueKind.DateTime:
                    faker.RuleFor(rule.PropertyName, f => rule.IsNullable && f.Random.Bool(0.2f) ? (object?)null : BuildDateTimeValue(f, rule.PropertyName));
                    break;
                case FakeValueKind.DateTimeOffset:
                    faker.RuleFor(rule.PropertyName, f => rule.IsNullable && f.Random.Bool(0.2f) ? (object?)null : (object)new DateTimeOffset(BuildDateTimeValue(f, rule.PropertyName)));
                    break;
                case FakeValueKind.Decimal:
                    faker.RuleFor(rule.PropertyName, f => rule.IsNullable && f.Random.Bool(0.2f) ? (object?)null : BuildDecimalValue(f, rule.PropertyName));
                    break;
                case FakeValueKind.Bool:
                    faker.RuleFor(rule.PropertyName, f => rule.IsNullable && f.Random.Bool(0.2f) ? (object?)null : BuildBoolValue(f, rule.PropertyName));
                    break;
                case FakeValueKind.Byte:
                    faker.RuleFor(rule.PropertyName, f => rule.IsNullable && f.Random.Bool(0.2f) ? (object?)null : (object)(byte)f.Random.Int(byte.MinValue, byte.MaxValue));
                    break;
                case FakeValueKind.SByte:
                    faker.RuleFor(rule.PropertyName, f => rule.IsNullable && f.Random.Bool(0.2f) ? (object?)null : (object)(sbyte)f.Random.Int(sbyte.MinValue, sbyte.MaxValue));
                    break;
                case FakeValueKind.Short:
                    faker.RuleFor(rule.PropertyName, f => rule.IsNullable && f.Random.Bool(0.2f) ? (object?)null : (object)(short)f.Random.Int(short.MinValue, short.MaxValue));
                    break;
                case FakeValueKind.UShort:
                    faker.RuleFor(rule.PropertyName, f => rule.IsNullable && f.Random.Bool(0.2f) ? (object?)null : (object)(ushort)f.Random.Int(ushort.MinValue, ushort.MaxValue));
                    break;
                case FakeValueKind.UInt:
                    faker.RuleFor(rule.PropertyName, f => rule.IsNullable && f.Random.Bool(0.2f) ? (object?)null : (object)(uint)f.Random.Int(1, int.MaxValue));
                    break;
                case FakeValueKind.Long:
                    faker.RuleFor(rule.PropertyName, f => rule.IsNullable && f.Random.Bool(0.2f) ? (object?)null : BuildLongValue(f, rule.PropertyName));
                    break;
                case FakeValueKind.ULong:
                    faker.RuleFor(rule.PropertyName, f => rule.IsNullable && f.Random.Bool(0.2f) ? (object?)null : (object)(ulong)Math.Abs(BuildLongValue(f, rule.PropertyName)));
                    break;
                case FakeValueKind.Float:
                    faker.RuleFor(rule.PropertyName, f => rule.IsNullable && f.Random.Bool(0.2f) ? (object?)null : (object)(float)BuildDoubleValue(f, rule.PropertyName));
                    break;
                case FakeValueKind.Double:
                    faker.RuleFor(rule.PropertyName, f => rule.IsNullable && f.Random.Bool(0.2f) ? (object?)null : BuildDoubleValue(f, rule.PropertyName));
                    break;
                case FakeValueKind.Char:
                    faker.RuleFor(rule.PropertyName, f => rule.IsNullable && f.Random.Bool(0.2f) ? (object?)null : (object)f.Random.Char('A', 'Z'));
                    break;
                case FakeValueKind.Guid:
                    faker.RuleFor(rule.PropertyName, f => rule.IsNullable && f.Random.Bool(0.2f) ? (object?)null : (object)f.Random.Guid());
                    break;
                case FakeValueKind.Enum:
                    if (rule.EnumValues != null && rule.EnumValues.Length > 0)
                    {
                        faker.RuleFor(rule.PropertyName, f => rule.IsNullable && f.Random.Bool(0.2f) ? (object?)null : f.PickRandom(rule.EnumValues));
                    }
                    break;
            }
        }

        customRules?.Invoke(faker);
        return faker;
    }

    /// <summary>
    /// 生成一个虚拟实体对象
    /// </summary>
    public static T GetFake<T>(Action<Faker<T>>? customRules = null) where T : class, new()
    {
        return CreateEntityFaker(customRules).Generate();
    }

    /// <summary>
    /// 生成指定数量的虚拟实体对象列表
    /// </summary>
    public static List<T> GetFake<T>(int count, Action<Faker<T>>? customRules = null) where T : class, new()
    {
        return CreateEntityFaker(customRules).Generate(count);
    }
}