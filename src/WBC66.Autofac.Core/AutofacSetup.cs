using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
            var assemblies = GetAllAssemblies().ToArray();

            // 注册仓储层，使用 InstancePerLifetimeScope 生命周期
            builder.RegisterAssemblyTypes(assemblies)
                .Where(c => c.Name.ToLower().EndsWith("repository") || c.Name.ToLower().EndsWith("dao"))
                .PublicOnly()
                .Where(cc => cc.IsClass && cc.GetInterfaces().Any())
                .InstancePerLifetimeScope()
                .AsImplementedInterfaces()
                .PreserveExistingDefaults(); // 保留现有的默认值，避免重复注入

            // 注册服务层，使用单例生命周期
            builder.RegisterAssemblyTypes(assemblies)
                .Where(c => c.Name.ToLower().EndsWith("service"))
                .PublicOnly()
                .Where(cc => cc.IsClass && cc.GetInterfaces().Any())
                .SingleInstance()
                .AsImplementedInterfaces()
                .PreserveExistingDefaults(); // 保留现有的默认值，避免重复注入

            // 注册没有接口的仓储层，使用 InstancePerLifetimeScope 生命周期
            builder.RegisterAssemblyTypes(assemblies)
                .Where(c => c.Name.ToLower().EndsWith("repository") || c.Name.ToLower().EndsWith("dao"))
                .PublicOnly()
                .Where(cc => cc.IsClass && !cc.GetInterfaces().Any())
                .InstancePerLifetimeScope()
                .PreserveExistingDefaults(); // 保留现有的默认值，避免重复注入

            // 注册没有接口的服务层，使用单例生命周期
            var registeredTypes = builder.RegisterAssemblyTypes(assemblies)
                .Where(c => c.Name.ToLower().EndsWith("service"))
                .PublicOnly()
                .Where(cc => cc.IsClass && !cc.GetInterfaces().Any())
                .SingleInstance()
                .PreserveExistingDefaults();// 保留现有的默认值，避免重复注入
        }

        private static bool HasLifetimeInterface(Type type)
        {
            // 检查类是否实现了 ITransient、IScoped 或 ISingleton 接口
            return typeof(ITransient).IsAssignableFrom(type) ||
                   typeof(IScoped).IsAssignableFrom(type) ||
                   typeof(ISingleton).IsAssignableFrom(type);
        }

        /// <summary>
        /// 不使用autofac的将所有继承ITransient、ISingleton、IScoped接口的类注入到容器中
        /// </summary>
        /// <param name="services"></param>
        public static void AddServiceDynamic(this IServiceCollection services)
        {
            var assemblies = GetAllAssemblies();
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes().Where(t => t.IsClass);
                foreach (var type in types)
                {
                    if (typeof(ISingleton).IsAssignableFrom(type))
                    {
                        services.TryAddSingleton(type, type);
                    }
                    else if (typeof(ITransient).IsAssignableFrom(type))
                    {
                        services.TryAddTransient(type, type);
                    }
                    else if (typeof(IScoped).IsAssignableFrom(type))
                    {
                        services.TryAddScoped(type, type);
                    }
                }
            }
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
                var types = assembly.GetTypes().Where(x => x.IsClass);
                var transientTypes = types
                    .Where(t => typeof(ITransient).IsAssignableFrom(t));

                var scopedTypes = types
                    .Where(t => typeof(IScoped).IsAssignableFrom(t));

                var singletonTypes = types
                    .Where(t => typeof(ISingleton).IsAssignableFrom(t));

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
                // 该类真实的业务接口：排除生命周期标记接口
                var interfaces = type.GetInterfaces()
                    .Where(i => i != typeof(IDisposable))
                    .Where(i => !markerInterfaces.Contains(i))
                    .ToList();

                // 判断是否有符合 “I + 实现类名” 的接口
                var matchedInterface = interfaces
                    .FirstOrDefault(i => i.Name == $"I{type.Name}");

                if (matchedInterface != null)
                {
                    // 有匹配的接口 → 按接口注入
                    if (!services.Any(s => s.ServiceType == matchedInterface))
                    {
                        services.TryAdd(new ServiceDescriptor(matchedInterface, type, lifetime));
                        //Console.WriteLine($"Registered {matchedInterface.Name} → {type.Name} as {lifetime}");
                    }
                }
                else
                {
                    // 没有匹配接口 → 注册自身
                    if (!services.Any(s => s.ServiceType == type))
                    {
                        services.TryAdd(new ServiceDescriptor(type, type, lifetime));
                        //Console.WriteLine($"Registered {type.Name} as {lifetime}");
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