## 6. 通过Aop使用内存缓存对接口、方法进行缓存
### 1. nuget包引入
必须引入两个包 至少在2024.11.7以上
``` xml
<PackageReference Include="WBC66.Cache.Core" Version="2024.11.7" />
<PackageReference Include="WBC66.Autofac.Core" Version="2025.02.05.1" />
```
### 必须开启内存缓存 否则后续步骤无法正常进行
``` csharp
//开启内存缓存
builder.Services.AddMemoryCacheSetup();
```
### 2. 对方法使用内存缓存
``` csharp
//使用autofac(内部会自动进行程序集扫描注入,不需要手动注入)
builder.Host.AddAutofacHostSetup(builder.Services, options =>
{
    //开启内存缓存拦截器(带有IProxyService接口的类将会被拦截),带有CacheResultAttribute特性的方法将会被缓存
    options.AddMemoryCacheResultAop();
});
```
### 3. 对控制器接口使用内存缓存
``` csharp
builder.Services.AddControllers(options =>
{
    //添加自定义的缓存过滤器
    options.Filters.Add<CacheResultFilter>();
});
```
### 方法缓存使用示例
``` csharp
//同一个方法不同参数会进行多次缓存
//接口注入直接继承IProxyService接口(继承接口后就不需要再手动注入了),方法上使用CacheResultAttribute特性即可
public class CacheResultService : ICacheResultService
{
    //缓存5秒
    [CacheResult(5)]
    public Student GetStudentAsync(string name)
    {
        return new Student { Name = name };
    }
}
public interface ICacheResultService : IProxyService
{
    Student GetStudentAsync(string name);
}
```
### 控制器缓存使用示例
``` csharp
[HttpGet]
[CacheResult(5)]//缓存5秒
public object Get()
{
    _cacheResultService.GetStudentAsync("这是参数");
    return "ok";
}
```