# nuget包合集
1. Serilog配置
1.1 Serilog配置文件
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
1.2 Serilog配置
``` csharp
builder.Host.AddSerilogHost(configuration);

app.UseSerilogSetup();
```
