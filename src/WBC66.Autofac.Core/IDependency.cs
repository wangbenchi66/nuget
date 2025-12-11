namespace WBC66.Autofac.Core
{
    /// <summary>
    /// 瞬时注入接口(继承该接口的类型将会被注入)
    /// </summary>
    public interface ITransient
    {
    }

    /// <summary>
    /// 单例注入接口(继承该接口的类型将会被注入)
    /// </summary>
    public interface ISingleton
    {
    }

    /// <summary>
    /// 作用域注入接口(继承该接口的类型将会被注入)
    /// </summary>
    public interface IScoped
    {
    }
}