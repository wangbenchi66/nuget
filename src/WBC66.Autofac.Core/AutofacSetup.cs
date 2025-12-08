using System.Reflection;
using System.Runtime.Loader;
using System.Security.AccessControl;
using System.Xml.Linq;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Hosting;

namespace WBC66.Autofac.Core
{
    /// <summary>
    /// 添加Autofac服务
    /// </summary>
    public static class AutofacSetup
    {
        /// <summary>
        /// 配置Autofac服务提供工厂和容器
        /// </summary>
        /// <param name="host">主机构建器</param>
        /// <param name="services">服务集合</param>
        /// <param name="customizeModule">自定义的模块</param>
        public static void AddAutofacHostSetup(this IHostBuilder host, IServiceCollection services, Action<ContainerBuilder> customizeModule = null)
        {
            if (host == null) { throw new ArgumentNullException(nameof(host)); }
            host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
            host.ConfigureContainer<ContainerBuilder>((context, builder) =>
            {
                //builder.Populate(services); // 将 IServiceCollection 中的服务填充到 Autofac 容器中
                builder.AddAutofacModule();
                // 自定义模块必须在批量注入之后
                customizeModule?.Invoke(builder);
            });
            services.AddRegisterDependencies();
            services.AddAutofac();
        }


        /// <summary>
        /// 批量注入Service、Repository、Dao结尾的类
        /// </summary>
        /// <param name="builder">容器构建器</param>
        /// <returns></returns>
        private static void AddAutofacModule(this ContainerBuilder builder)
        {
            var assemblies = GetAllAssemblies();

            // 注册仓储层，使用 InstancePerLifetimeScope 生命周期
            builder.RegisterAssemblyTypes(assemblies.ToArray())
                .Where(c => c.Name.ToLower().EndsWith("repository") || c.Name.ToLower().EndsWith("dao"))
                .PublicOnly()
                .Where(cc => cc.IsClass)
                .InstancePerLifetimeScope()
                .AsImplementedInterfaces()
                .PreserveExistingDefaults(); // 保留现有的默认值，避免重复注入

            // 注册服务层，使用单例生命周期
            builder.RegisterAssemblyTypes(assemblies.ToArray())
                .Where(c => c.Name.ToLower().EndsWith("service"))
                .PublicOnly()
                .Where(cc => cc.IsClass)
                .SingleInstance()
                .AsImplementedInterfaces()
                .PreserveExistingDefaults(); // 保留现有的默认值，避免重复注入

            // 注册没有接口的仓储层，使用 InstancePerLifetimeScope 生命周期
            builder.RegisterAssemblyTypes(assemblies.ToArray())
                .Where(c => c.Name.ToLower().EndsWith("repository") || c.Name.ToLower().EndsWith("dao"))
                .PublicOnly()
                .Where(cc => cc.IsClass && !cc.GetInterfaces().Any())
                .InstancePerLifetimeScope()
                .PreserveExistingDefaults(); // 保留现有的默认值，避免重复注入

            // 注册没有接口的服务层，使用单例生命周期
            builder.RegisterAssemblyTypes(assemblies.ToArray())
                .Where(c => c.Name.ToLower().EndsWith("service"))
                .PublicOnly()
                .Where(cc => cc.IsClass && !cc.GetInterfaces().Any())
                .SingleInstance()
                .PreserveExistingDefaults(); // 保留现有的默认值，避免重复注入
        }


        /// <summary>
        /// 不使用autofac的将所有继承ITransient、ISingleton、IScoped接口的类注入到容器中
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static void AddRegisterDependencies(this IServiceCollection services)
        {
            var assemblies = GetAllAssemblies();
            // 扫描三种生命周期的实现类
            var markerInterfaces = new[] { typeof(IScoped), typeof(ITransient), typeof(ISingleton) };
            foreach (var assembly in assemblies)
            {
                var transientTypes = assembly.GetTypes()
                    .Where(t => typeof(ITransient).IsAssignableFrom(t)
                                && !t.IsInterface && !t.IsAbstract);

                var scopedTypes = assembly.GetTypes()
                    .Where(t => typeof(IScoped).IsAssignableFrom(t)
                                && !t.IsInterface && !t.IsAbstract);

                var singletonTypes = assembly.GetTypes()
                    .Where(t => typeof(ISingleton).IsAssignableFrom(t)
                                && !t.IsInterface && !t.IsAbstract);

                Register(services, transientTypes, markerInterfaces, ServiceLifetime.Transient);
                Register(services, scopedTypes, markerInterfaces, ServiceLifetime.Scoped);
                Register(services, singletonTypes, markerInterfaces, ServiceLifetime.Singleton);
            }
        }
        private static void Register(
            IServiceCollection services,
            IEnumerable<Type> types,
            Type[] markerInterfaces,
            ServiceLifetime lifetime)
        {
            foreach (var type in types)
            {
                // 获取非标记接口（业务接口）
                var interfaces = type.GetInterfaces()
                                     .Where(i => !markerInterfaces.Contains(i))
                                     .ToList();

                if (interfaces.Any())
                {
                    // 注册接口 → 实现类
                    foreach (var it in interfaces)
                    {
                        if (!services.Any(s => s.ServiceType == it))
                        {
                            services.Add(new ServiceDescriptor(it, type, lifetime));
                        }
                    }
                }
                else
                {
                    // 没有业务接口 → 注册自身类型
                    if (!services.Any(s => s.ServiceType == type))
                    {
                        services.Add(new ServiceDescriptor(type, type, lifetime));
                    }
                }
            }
        }

        /// <summary>
        /// 获取所有引用的程序集
        /// </summary>
        /// <returns></returns>
        private static List<Assembly> GetAllAssemblies()
        {
            var assemblies = new List<Assembly>();
            var dependencies = DependencyContext.Default.RuntimeLibraries;
            foreach (var library in dependencies)
            {
                try
                {
                    var assembly = Assembly.Load(new AssemblyName(library.Name));
                    assemblies.Add(assembly);
                }
                catch
                {
                    // 忽略加载失败的程序集
                }
            }
            return assemblies;
        }
    }
}