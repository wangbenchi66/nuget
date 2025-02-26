## 5. Autofac配置
### 1. Autofac配置
线上nuget引入 版本号随时更新
``` xml
<PackageReference Include="WBC66.Autofac.Core" Version="2025.02.05.1" />
```
### 2.使用Autofac工厂
``` csharp
//使用Autofac(会直接扫描当前项目的所有Service、Repository、Dao结尾的类进行注入)
builder.Host.AddAutofacHostSetup(builder.Services);

//不使用autofac的将所有实现ITransient、ISingleton、IScoped接口的类注入到容器中
//如果使用了autofac则内部已经实现这里不需要注入
builder.Services.AddRegisterDependencies();
```
### 3.使用示例
``` csharp
//接口注入直接继承ITransient接口即可(一般情况第二步已经注入了就不需要再注入了)
//继承ITransient、ISingleton、IScoped的都注入了
public interface ITestService : ISingleton
{
    string Get();
}
//继承ITransient、ISingleton、IScoped的都注入了
public class TestService : ISingleton
{
}

``` 