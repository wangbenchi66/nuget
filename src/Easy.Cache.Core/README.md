## 6. 通过Aop使用内存缓存对接口、方法进行缓存
### 1. nuget包引入
必须引入两个包 至少在2024.11.7以上
``` xml
<PackageReference Include="Easy.Cache.Core" Version="2026.3.2" />
```
### 2. 配置
``` json
//如果使用内存缓存则不需要配置，如果使用redis缓存则需要配置redis连接字符串
"RedisConfigurations": {
    "RedisClients": {
      "TestCache": {//单机模式配置(与集群模式配置是一样的)
        "ConnectionString": "localhost:6379,abortConnect=false,ssl=false,password=123456"
      },
      "SentinelCache": {//哨兵模式配置
        "ConnectionString": "mymaster,abortConnect=false,ssl=false,password=123456",
        "SentinelNodes":[
            "localhost:26379",
            "localhost:26380",
        ]
      },
      "ClusterCache": {//集群模式配置
        "ConnectionString": "localhost:6379,abortConnect=false,ssl=false,password=123456"
      }
    }
  }
```


### 使用缓存
``` csharp
//开启内存缓存，如果有redis会使用redis，如果没有则会使用内存缓存
builder.Services.AddEasyCache(configuration,"RedisConfigurations");//默认不传就是RedisConfigurations,如果是其他名称需要在这里修改，
```
### 2. 使用缓存\缓存切换
``` csharp
public class CacheResultService : ICacheResultService
{
    private readonly IEasyCacheService _cacheService;
    public CacheResultService(IEasyCacheService cacheService)
    {
        _cacheService = cacheService;
    }
    public Student GetStudentAsync(string name)
    {
        //直接使用缓存对象进行缓存操作
        return _cacheService.Get($"student:{name}", "1", 30);
    }
}

//缓存客户端切换
public class CacheResultService : ICacheResultService
{
    private readonly IEasyCacheService _cacheService;
    public CacheResultService(IEasyCacheServiceFactory cacheServiceFactory)
    {
        _cacheService = cacheServiceFactory.Create("TestCache");//切换到redis缓存客户端进行缓存操作
    }
    public Student GetStudentAsync(string name)
    {
        //切换到redis缓存客户端进行缓存操作
        return _cacheService.Get($"student:{name}", "1", 30);
    }
}

```
### 3. 对控制器接口使用内存缓存
``` csharp
builder.Services.AddControllers(options =>
{
    //添加自定义的缓存过滤器
    options.Filters.Add<EasyCacheResultFilter>();
});
```
### 方法缓存使用示例
``` csharp
//同一个方法不同参数会进行多次缓存
//接口注入直接继承IProxyService接口(继承接口后就不需要再手动注入了),方法上使用EasyCacheResultAttribute特性即可
public class CacheResultService : ICacheResultService
{
    //缓存5秒
    [EasyCacheResult(5)]
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
[EasyCacheResult(5)]//缓存5秒
public object Get()
{
    _cacheResultService.GetStudentAsync("这是参数");
    return "ok";
}
```