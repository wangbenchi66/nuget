using System.ComponentModel.DataAnnotations;
using SqlSugar;

namespace Easy.SqlSugar.Core.BaseProvider;

public class BaseSugarModel
{
}


/// <summary>
/// sugar实体基类
/// </summary>
/// <typeparam name="TKey"></typeparam>
public class BaseSugarModel<TKey> : BaseSugarModel
{
    /// <summary>
    /// 实体主键 自动推断自增
    /// </summary>
    //[SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    [Key]
    public virtual TKey Id { get; set; }
}