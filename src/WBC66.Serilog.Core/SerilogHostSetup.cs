/*----------------------------------------------------------------
 * 命名空间：NetCore.Core
 * 创建者：WangBenChi
 * 创建时间：2023/04/12 14:35:23
 *----------------------------------------------------------------*/

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Elasticsearch;
using Serilog.Sinks.Elasticsearch;

namespace WBC66.Serilog.Core
{
    /// <summary>
    /// Serilog封装
    /// </summary>
    public static class SerilogHostSetup
    {
        #region 方式一 直接引入，支持设置日志路径，忽略日志源，输出模板

        /// <summary>
        /// 默认忽略的日志源
        /// </summary>
        private static readonly string[] DefaultIgnoredSources =
        {
            "Microsoft.AspNetCore.Hosting.Diagnostics",
            "Microsoft.AspNetCore.Routing.EndpointMiddleware",
            "Microsoft.AspNetCore.StaticFiles.StaticFileMiddleware",
            "Serilog.AspNetCore.RequestLoggingMiddleware",
            "Microsoft.AspNetCore.DataProtection",
            "Microsoft.AspNetCore.Cors",
            "Microsoft.AspNetCore.Mvc",
            "Microsoft.AspNetCore.ResponseCaching"
        };

        /// <summary>
        /// 默认日志级别
        /// </summary>
        private static readonly LogEventLevel[] Levels =
        {
            LogEventLevel.Information,
            LogEventLevel.Warning,
            LogEventLevel.Error
        };

        /// <summary>
        /// 默认输出模板
        /// </summary>
        private const string DefaultOutputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3} {SourceContext:l}] {Message:lj}{NewLine}{Exception}";

        public static void AddSerilogHost(this IHostBuilder builder,
                                          string? filePath = null,
                                          IEnumerable<string>? ignoredSources = null,
                                          string? outputTemplate = null)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                //获取系统路径和项目名称设置为filePath
                var basePath = AppDomain.CurrentDomain.BaseDirectory;
                var projectName = AppDomain.CurrentDomain.FriendlyName;
                filePath = Path.Combine(basePath, "logs", projectName);
            }
            var sources = ignoredSources?.ToArray() ?? DefaultIgnoredSources;
            var template = string.IsNullOrWhiteSpace(outputTemplate) ? DefaultOutputTemplate : outputTemplate;

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext() // 加入上下文信息，如请求Id
                                         //.MinimumLevel.Information()    // 全局最小级别
                .ConfigureConsole(template)
                .ConfigureFile(filePath, template)
                .ApplyIgnoredSources(sources)
                .CreateLogger();

            builder.UseSerilog();
        }

        private static LoggerConfiguration ConfigureConsole(this LoggerConfiguration logger, string outputTemplate)
        {
            return logger.WriteTo.Console(outputTemplate: outputTemplate);
        }

        /// <summary>
        /// 按不同日志级别分别写入文件
        /// </summary>
        private static LoggerConfiguration ConfigureFile(this LoggerConfiguration logger, string basePath, string outputTemplate)
        {
            foreach (var level in Levels)
            {
                var levelName = level.ToString();
                logger = logger.WriteTo.Logger(lc =>
                    lc.Filter.ByIncludingOnly(e => e.Level == level)
                      //两种滚动日志方式任选其一，只是生成的文件名格式不一样而已

                      //Information20251029.log
                      .WriteTo.File(
                          path: Path.Combine(basePath, $"{levelName}.log"),
                          rollingInterval: RollingInterval.Day,
                          retainedFileCountLimit: 10,
                          encoding: System.Text.Encoding.UTF8,
                          shared: true,
                          outputTemplate: outputTemplate)
                //20251029Information.log
                //.WriteTo.RollingFile(Path.Combine(basePath, "{Date}" + $"{levelName}.log"), retainedFileCountLimit: 7, shared: true, outputTemplate: DefaultOutputTemplate)
                );
            }
            return logger;
        }

        /// <summary>
        /// 忽略或降低指定命名空间的日志级别,直接设置为Warning级别减少输出
        /// </summary>
        private static LoggerConfiguration ApplyIgnoredSources(this LoggerConfiguration config, IEnumerable<string> sources)
        {
            foreach (var source in sources)
                config = config.MinimumLevel.Override(source, LogEventLevel.Warning);
            return config;
        }


        #endregion 方式一 直接引入，支持设置日志路径，忽略日志源，输出模板

        #region 方式二 官方Json配置方式

        /// <summary>
        /// 添加Serilog(json的方式)
        /// </summary>
        /// <remarks>
        /// 完整json示例 https://github.com/serilog/serilog-settings-configuration/blob/dev/sample/Sample/appsettings.json
        /// </remarks>
        /// <param name="builder"></param>
        /// <param name="configuration"></param>
        public static void AddSerilogHostJson(this IHostBuilder builder, IConfiguration configuration)
        {
            builder.UseSerilog((ctx, lc) =>
            {
                lc.ReadFrom.Configuration(ctx.Configuration);
            });
        }

        #endregion 方式二 官方Json配置方式

        #region 方式三 配置类的方式

        /// <summary>
        /// 添加Serilog（配置类的方式）
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configuration"></param>
        public static void AddSerilogHost(this IHostBuilder builder, ConfigurationManager configuration)
        {
            var options = configuration.GetSection(SerilogOptions.Position).Get<SerilogOptions>();
            if (options == null)
                return;

            Log.Logger = new LoggerConfiguration()
                    .ConfigureMinimumLevel(options)
                    .ConfigureFile(options.File)
                    .ConfigureConsole(options.Console)
                    .ConfigureElasticsearch(options.Elasticsearch)
                    .CreateLogger();

            builder.UseSerilog();
        }

        /// <summary>
        /// 添加Serilog中间件
        /// </summary>
        /// <param name="app"></param>
        public static void UseSerilogSetup(this IApplicationBuilder app)
        {
            app.UseSerilogRequestLogging(p => { p.MessageTemplate = "HTTP {RequestMethod} {RequestPath} 状态码 {StatusCode} 耗时 {Elapsed:0.0000} ms"; });
        }

        /// <summary>
        /// 最小日志级别配置
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        private static LoggerConfiguration ConfigureMinimumLevel(this LoggerConfiguration logger, SerilogOptions options)
        {
            logger.MinimumLevel.Is(Enum.Parse<LogEventLevel>(options.MinimumLevel));
            if (options.Override != null && options.Override.Any())
            {
                foreach (var item in options.Override)
                {
                    logger.MinimumLevel.Override(item.Key, Enum.Parse<LogEventLevel>(item.Value));
                }
            }
            return logger;
        }

        /// <summary>
        /// 文件日志配置
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        private static LoggerConfiguration ConfigureFile(this LoggerConfiguration logger, FileOptions options)
        {
            if (options == null)
                return logger;

            var basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, options.Path);
            foreach (var logLevel in Enum.GetValues<LogEventLevel>())
            {
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, basePath, $"{logLevel}.txt");
                logger
                // 日志根据级别区分，并应用通用过滤器
                .WriteTo.Logger(lc => lc.Filter.With(new HealthCheckFilter())
                .Filter.ByIncludingOnly(le => le.Level == logLevel)
                //滚动日志,格式固定为日期在最后面，例如 log-20250526.txt 不能变更
                .WriteTo.File(
                        path: path,
                        //outputTemplate: options.DefaultOutputTemplate,
                        rollingInterval: options.RollingInterval,
                        shared: true,
                        fileSizeLimitBytes: 10 * 1024 * 1024,
                        rollOnFileSizeLimit: true,
                        restrictedToMinimumLevel: logLevel
                        ));
                /*滚动日志，指定文件名格式，其中{Date}为占位符,不需要替换，Serilog会自动替换(需要引入Serilog.Sinks.RollingFile)
                 string levelPath = Path.Combine(path, "{Date}" + $"{level}.log");
                 */
                // .WriteTo.RollingFile(levelPath, retainedFileCountLimit: 7, shared: true)
            }
            return logger;
        }

        /// <summary>
        /// 控制台日志配置
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        private static LoggerConfiguration ConfigureConsole(this LoggerConfiguration logger, ConsoleOptions options)
        {
            if (options == null || !options.Enabled)
                return logger;

            logger.WriteTo.Console(
                restrictedToMinimumLevel: Enum.Parse<LogEventLevel>(options.Minlevel),
                outputTemplate: options.Template
            );

            return logger;
        }

        /// <summary>
        /// Elasticsearch 日志配置
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        private static LoggerConfiguration ConfigureElasticsearch(this LoggerConfiguration logger, ElasticsearchOptions options)
        {
            if (options == null || string.IsNullOrEmpty(options.Uri))
                return logger;

            var uris = new List<Uri> { new Uri(options.Uri) };
            uris = PingAndFilterUrisAsync(uris).Result;
            if (uris.Count == 0)
                return logger;

            logger.WriteTo.Elasticsearch(new ElasticsearchSinkOptions(uris)
            {
                IndexFormat = options.IndexFormat,
                NumberOfShards = options.NumberOfShards,
                NumberOfReplicas = options.NumberOfReplicas,
                CustomFormatter = new ExceptionAsObjectJsonFormatter(renderMessage: true),
                EmitEventFailure = EmitEventFailureHandling.RaiseCallback,
                FailureCallback = FailureCallback,
                AutoRegisterTemplate = true,
                AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
                ModifyConnectionSettings = conn =>
                {
                    if (!string.IsNullOrEmpty(options.UserName) && !string.IsNullOrEmpty(options.Password))
                    {
                        conn.BasicAuthentication(options.UserName, options.Password);
                    }
                    return conn;
                }
            });

            return logger;
        }

        /// <summary>
        /// 日志记录失败回调
        /// </summary>
        /// <param name="logEvent"></param>
        /// <param name="exception"></param>
        private static void FailureCallback(LogEvent logEvent, Exception exception)
        {
            Console.WriteLine($"日志记录失败，信息：{exception.Message}");
        }

        /// <summary>
        /// 测试节点是否可用
        /// </summary>
        /// <param name="uris"></param>
        /// <returns></returns>
        private static async Task<List<Uri>> PingAndFilterUrisAsync(List<Uri> uris)
        {
            using var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(3) };
            var successfulUris = new List<Uri>();

            foreach (var uri in uris)
            {
                try
                {
                    HttpResponseMessage response = await httpClient.GetAsync(uri);
                    if (response.IsSuccessStatusCode)
                    {
                        successfulUris.Add(uri);
                    }
                }
                catch (TaskCanceledException)
                {
                    Console.WriteLine($"请求至 '{uri}' 超时。");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"请求至 '{uri}' 时发生错误：{ex.Message}");
                }
            }

            return successfulUris;
        }

        #endregion 方式三 配置类的方式
    }

    /// <summary>
    /// 健康检查信息过滤
    /// </summary>
    public class HealthCheckFilter : ILogEventFilter
    {
        /// <summary>
        /// 是否记录日志
        /// </summary>
        /// <param name="logEvent"></param>
        /// <returns></returns>
        public bool IsEnabled(LogEvent logEvent)
        {
            if (logEvent.Properties.ContainsKey("RequestPath") && logEvent.Properties["RequestPath"].ToString().Contains("health"))
                return false;
            return true;
        }
    }
}