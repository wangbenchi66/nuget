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
                builder.AddAutofacModule();
                //自定义模块必须在批量注入之后
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

            //var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            var assemblies = GetAllAssemblies();

            builder.RegisterAssemblyTypes(assemblies.ToArray())//程序集内所有具象类 
            .Where(c => c.Name.ToLower().EndsWith("repository") || c.Name.ToLower().EndsWith("service") || c.Name.ToLower().EndsWith("dao"))
            .PublicOnly()//只要public访问权限的
            .Where(cc => cc.IsClass)//只要class型（主要为了排除值和interface类型） 
            .InstancePerLifetimeScope();


            builder.RegisterAssemblyTypes(assemblies.ToArray())//程序集内所有具象类 
           .Where(c => c.Name.ToLower().EndsWith("repository") || c.Name.ToLower().EndsWith("service") || c.Name.ToLower().EndsWith("dao"))
           .PublicOnly()//只要public访问权限的
           .InstancePerLifetimeScope()
           .AsImplementedInterfaces();
        }


        /// <summary>
        /// 不使用autofac的将所有继承ITransient、ISingleton、IScoped接口的类注入到容器中
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static void AddRegisterDependencies(this IServiceCollection services)
        {
            //var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            var assemblies = GetAllAssemblies();
            foreach (var assembly in assemblies)
            {
                var dependencyTypes = assembly.GetTypes()
                    .Where(t => typeof(ITransient).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);
                var singletonTypes = assembly.GetTypes()
                    .Where(t => typeof(ISingleton).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);
                var scopedTypes = assembly.GetTypes()
                    .Where(t => typeof(IScoped).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

                foreach (var type in dependencyTypes)
                {
                    var interfaces = type.GetInterfaces();
                    if (interfaces.Any())
                    {
                        foreach (var @interface in interfaces)
                        {
                            services.AddTransient(@interface, type);
                        }
                    }
                    else
                    {
                        services.AddTransient(type);
                    }
                }
                foreach (var type in singletonTypes)
                {
                    var interfaces = type.GetInterfaces();
                    if (interfaces.Any())
                    {
                        foreach (var @interface in interfaces)
                        {
                            services.AddSingleton(@interface, type);
                        }
                    }
                    else
                    {
                        services.AddSingleton(type);
                    }
                }
                foreach (var type in scopedTypes)
                {
                    var interfaces = type.GetInterfaces();
                    if (interfaces.Any())
                    {
                        foreach (var @interface in interfaces)
                        {
                            services.AddScoped(@interface, type);
                        }
                    }
                    else
                    {
                        services.AddScoped(type);
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