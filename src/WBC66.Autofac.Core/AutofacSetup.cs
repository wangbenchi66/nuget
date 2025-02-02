using System.Reflection;
using System.Runtime.Loader;
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
            services.AddAutofac();
        }

        /// <summary>
        /// 批量注入实现IDependency接口的类型
        /// </summary>
        /// <param name="builder">容器构建器</param>
        public static void AddAutofacModule(this ContainerBuilder builder)
        {
            //IDependency接口自动注入
            Type baseType = typeof(IDependency);
            //Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies().ToArray();

            var compilationLibrary = DependencyContext.Default.RuntimeLibraries.Where(x => !x.Serviceable && x.Type == "project").ToList();
            List<Assembly> assemblies = new List<Assembly>();
            foreach (var _compilation in compilationLibrary)
            {
                try
                {
                    assemblies.Add(AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(_compilation.Name)));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(_compilation.Name + ex.Message);
                }
            }
            builder.RegisterAssemblyTypes(assemblies.ToArray())
              .Where(type => baseType.IsAssignableFrom(type) && !type.IsAbstract)
              .AsImplementedInterfaces()
              .InstancePerLifetimeScope()
              .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);
            builder.RegisterAssemblyTypes(assemblies.ToArray())
            .AsImplementedInterfaces()
            .InstancePerDependency()
                .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);
        }
    }
}