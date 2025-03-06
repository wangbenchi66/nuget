using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq;
using System.Linq.Expressions;

namespace Easy.EF.Core.Common
{
    public static class EntityEntryExtensions
    {
        //获取主键
        public static IKey? GetPrimaryKeyValue(this EntityEntry entry)
        {
            var key = entry.Metadata.FindPrimaryKey();
            if (key == null)
            {
                throw new InvalidOperationException("No primary key found for the entity.");
            }
            return key;
        }

        // 获取主键值
        public static object GetPrimaryKey(this EntityEntry entry)
        {
            var key = entry.Metadata.FindPrimaryKey();
            if (key == null)
            {
                throw new InvalidOperationException("No primary key found for the entity.");
            }
            return key.Properties.Select(p => entry.Property(p.Name).CurrentValue).FirstOrDefault();
        }
    }
}