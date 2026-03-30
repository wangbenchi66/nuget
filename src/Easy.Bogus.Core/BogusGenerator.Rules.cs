using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Easy.Bogus.Core;

public static partial class BogusGenerator
{
    private enum FakeValueKind
    {
        None,
        String,
        Int,
        DateTime,
        DateTimeOffset,
        Decimal,
        Bool,
        Byte,
        SByte,
        Short,
        UShort,
        UInt,
        Long,
        ULong,
        Float,
        Double,
        Char,
        Guid,
        Enum
    }

    private sealed class PropertyRule
    {
        public string PropertyName { get; set; } = string.Empty;

        public FakeValueKind Kind { get; set; }

        public object[]? EnumValues { get; set; }

        /// <summary>是否可空类型（Nullable&lt;T&gt;），为 true 时生成时有 20% 概率返回 null。</summary>
        public bool IsNullable { get; set; }
    }

    private static readonly ConcurrentDictionary<Type, List<PropertyRule>> PropertyRuleCache = new();

    private static List<PropertyRule> GetPropertyRules(Type entityType)
    {
        return PropertyRuleCache.GetOrAdd(entityType, BuildPropertyRules);
    }

    private static List<PropertyRule> BuildPropertyRules(Type entityType)
    {
        var result = new List<PropertyRule>();

        foreach (var property in entityType.GetProperties())
        {
            if (!property.CanWrite || property.SetMethod == null || !property.SetMethod.IsPublic)
            {
                continue;
            }

            if (property.GetIndexParameters().Length > 0)
            {
                continue;
            }

            var type = property.PropertyType;
            var underlyingType = Nullable.GetUnderlyingType(type);
            var realType = underlyingType ?? type;
            bool isNullable = underlyingType != null;

            if (IsSqlSugarPrimaryKey(property))
            {
                continue;
            }

            if (property.GetCustomAttribute<KeyAttribute>() != null)
            {
                continue;
            }

            if (realType.IsEnum)
            {
                result.Add(new PropertyRule
                {
                    PropertyName = property.Name,
                    Kind = FakeValueKind.Enum,
                    EnumValues = Enum.GetValues(realType).Cast<object>().ToArray()
                    ,
                    IsNullable = isNullable
                });
                continue;
            }

            var kind = GetFakeValueKind(realType);
            if (kind == FakeValueKind.None)
            {
                continue;
            }

            result.Add(new PropertyRule
            {
                PropertyName = property.Name,
                Kind = kind
                ,
                IsNullable = isNullable
            });
        }

        return result;
    }

    private static bool IsSqlSugarPrimaryKey(PropertyInfo property)
    {
        var attribute = property.GetCustomAttributes(inherit: true)
            .FirstOrDefault(attr =>
            {
                var typeName = attr.GetType().Name;
                return string.Equals(typeName, "SugarColumn", StringComparison.Ordinal)
                       || string.Equals(typeName, "SugarColumnAttribute", StringComparison.Ordinal);
            });

        if (attribute == null)
        {
            return false;
        }

        var isPrimaryKeyProperty = attribute.GetType()
            .GetProperty("IsPrimaryKey", BindingFlags.Public | BindingFlags.Instance);

        if (isPrimaryKeyProperty == null || isPrimaryKeyProperty.PropertyType != typeof(bool))
        {
            return false;
        }

        return (bool)(isPrimaryKeyProperty.GetValue(attribute) ?? false);
    }

    private static FakeValueKind GetFakeValueKind(Type type)
    {
        if (type == typeof(string)) return FakeValueKind.String;
        if (type == typeof(int)) return FakeValueKind.Int;
        if (type == typeof(DateTime)) return FakeValueKind.DateTime;
        if (type == typeof(DateTimeOffset)) return FakeValueKind.DateTimeOffset;
        if (type == typeof(decimal)) return FakeValueKind.Decimal;
        if (type == typeof(bool)) return FakeValueKind.Bool;
        if (type == typeof(byte)) return FakeValueKind.Byte;
        if (type == typeof(sbyte)) return FakeValueKind.SByte;
        if (type == typeof(short)) return FakeValueKind.Short;
        if (type == typeof(ushort)) return FakeValueKind.UShort;
        if (type == typeof(uint)) return FakeValueKind.UInt;
        if (type == typeof(long)) return FakeValueKind.Long;
        if (type == typeof(ulong)) return FakeValueKind.ULong;
        if (type == typeof(float)) return FakeValueKind.Float;
        if (type == typeof(double)) return FakeValueKind.Double;
        if (type == typeof(char)) return FakeValueKind.Char;
        if (type == typeof(Guid)) return FakeValueKind.Guid;
        return FakeValueKind.None;
    }
}
