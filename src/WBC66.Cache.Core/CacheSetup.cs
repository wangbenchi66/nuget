using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Extras.DynamicProxy;
using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;

namespace WBC66.Cache.Core
{
    /// <summary>
    /// 缓存服务
    /// </summary>
    public static class CacheSetup
    {
        /// <summary>
        /// 添加内存缓存服务
        /// </summary>
        /// <param name="services"></param>
        public static void AddMemoryCacheSetup(this IServiceCollection services)
        {
            services.AddMemoryCache();
        }
        /// <summary>
        /// 开启内存缓存拦截器(带有IProxyService接口的类将会被拦截),带有CacheResultAttribute特性的方法将会被缓存
        /// </summary>
        /// <param name="options"></param>
        public static void AddMemoryCacheResultAop(this ContainerBuilder options)
        {
            //添加拦截器LogInterceptor
            options.RegisterType<MemoryCacheInterceptor>().As<IInterceptor>();
            //注册所有实现 IProxyService 的类，并启用拦截器
            options.RegisterType<MemoryCacheInterceptor>().AsSelf();
            options.RegisterAssemblyTypes(AppDomain.CurrentDomain.GetAssemblies())
                .Where(t => typeof(IProxyService).IsAssignableFrom(t) && t.IsClass)
                .AsImplementedInterfaces()
                .EnableInterfaceInterceptors()
                .InterceptedBy(typeof(MemoryCacheInterceptor));
        }

        /// <summary>
        /// 添加redis缓存服务
        /// </summary>
        /// <param name="services"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void AddRedisCacheSetup(this IServiceCollection services)
        {
            if (services == null) { throw new ArgumentNullException(nameof(services)); }
            services.AddSingleton<IRedisService, RedisService>();
        }
    }
}