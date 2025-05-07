# nuget包合集
## 1. Serilog配置
线上nuget引入 版本号随时更新
``` xml
<PackageReference Include="WBC66.Serilog.Core" Version="2024.10.28" />
```
### 1.1 Serilog配置文件
``` json
  "SerilogOptions": {
    "MinimumLevel": "Information",
    "Override": {
      "Microsoft": "Warning",
      "System": "Warning"
    },
    "File": {
      "Path": "Serilog/log-.txt",
      "RollingInterval": "Day"
    },
    "Console": {
      "Enabled": true,
      "Minlevel": "Information",
      "Template": "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}"
    },
    "Elasticsearch": {
      "Uri": "http://k8s.els.com",
      "IndexFormat": "test-dev-{0:yyyy.MM.dd}",
      "NumberOfShards": 2,
      "NumberOfReplicas": 1,
      "UserName": "elastic",
      "Password": "changeme"
    }
  }
```
### 1.2 Serilog配置
``` csharp
builder.Host.AddSerilogHost(configuration);

app.UseSerilogSetup();
```
## 2. NLog配置(两种配置模式，一种nlog.config配置文件，一种json配置) 感觉还是serilog更好用
线上nuget引入 版本号随时更新
``` xml
<PackageReference Include="WBC66.NLog.Core" Version="	2024.10.25" />
```
### 2.1 NLog配置文件(xml方式)
``` xml
<?xml version="1.0" encoding="utf-8" ?>
<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd"
      xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
      autoReload="true"
      throwConfigExceptions="true">
	<extensions>
		<add assembly="NLog.Web.AspNetCore"/>
		<add assembly="NLog.Targets.ElasticSearch"/>
	</extensions>

	<targets>
		<target name="logfile" xsi:type="File" fileName="./log/${shortdate}_${level}.log"
				layout="${longdate}|${level:uppercase=true}|${logger}|${message}|${exception:format=tostring}" />
		<target name="logconsole" xsi:type="Console"
				layout="${longdate}|${level:uppercase=true}|${logger}|${message}|${exception:format=tostring}" />
		<!-- ElasticSearch发送-->
		<target xsi:type="ElasticSearch" name="els" index="test-dev-${date:format=yyyy.MM}" uri="http://k8s.els.com"
				layout="${longdate}|${level:uppercase=true}|${logger}|${message}|${exception:format=tostring}"/>
	</targets>

	<rules>
		<!-- 忽略Microsoft的日志 -->
		<logger name="Microsoft.*" minlevel="Trace" final="true" />
		<!-- 所有日志输出到控制台和文件 -->
		<logger name="*" minlevel="Info" writeTo="logfile" />
		<logger name="*" minlevel="Info" writeTo="els" />
	</rules>
</nlog>
```
### 2.2 NLog配置
``` csharp
builder.Host.AddNLogHost();
```
### 2.3 NLog配置文件(json方式)
``` json
"NLog": {
    "autoReload": true,
    "throwConfigExceptions": true,
    "extensions": [
      {
        "assembly": "NLog.Web.AspNetCore"
      },
      {
        "assembly": "NLog.Targets.ElasticSearch"
      }
    ],
    "targets": {
      "logfile": {
        "type": "File",
        "fileName": "./log/${shortdate}_${level}.log",
        "layout": "${longdate}|${level:uppercase=true}|${logger}|${message}|${exception:format=tostring}"
      },
      "logconsole": {
        "type": "Console",
        "layout": "${longdate}|${level:uppercase=true}|${logger}|${message}|${exception:format=tostring}"
      },
      "els": {
        "type": "ElasticSearch",
        "index": "test-dev-${date:format=yyyy.MM}",
        "uri": "http://k8s.els.com",
        "layout": "${longdate}|${level:uppercase=true}|${logger}|${message}|${exception:format=tostring}"
      }
    },
    "rules": [
      {
        "logger": "Microsoft.*",
        "minlevel": "Trace",
        "final": true
      },
      {
        "logger": "*",
        "minlevel": "Info",
        "writeTo": "logfile"
      },
      {
        "logger": "*",
        "minlevel": "Info",
        "writeTo": "els"
      }
    ]
  }
```
### 2.4 NLog配置
``` csharp
builder.AddNLogSteup(configuration);
```

## 3. [SqlSugar配置](./src/Easy.SqlSugar.Core/README.md)
## 4. [EF配置](./src/Easy.EF.Core/README.md)
## 5. [Autofac配置](./src/WBC66.Autofac.Core/README.md)
## 6. [Aop缓存配置](./src/WBC66.Cache.Core/README.md)

## 7. 过滤器,中间件
### 7.1. 过滤器
``` csharp
//幂等性过滤器
builder.Services.AddControllers(options =>
{
    //添加自定义的缓存过滤器 需要配合第六段的Aop使用
    options.Filters.Add<CacheResultFilter>();
    //添加自定义的幂等性过滤器
    options.Filters.Add<IdempotentFilter>();
});
```
### 7.2. 中间件
``` csharp
//添加自定义的中间件
app.UseMiddleware<LogMiddleware>();//添加日志中间件
app.UseMiddleware<ExceptionMiddleware>();//添加异常处理中间件
app.UseMiddleware<CurrentLimitingMiddleware>(1, 1);//添加限流中间件 1个线程 1个并发
```

## 8. [通用扩展类](./src/Easy.Common.Core/README.md)