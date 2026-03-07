using Serilog;

namespace WebApi.Test.Apis
{
    public class SerilogApis : BaseApi
    {
        private readonly ILogger<SerilogApis> _logger;

        public SerilogApis(ILogger<SerilogApis> logger)
        {
            _logger = logger;
        }

        public object Get()
        {
            _logger.LogInformation("当前时间{date}", DateTime.Now.ToString("G"));
            _logger.LogWarning("当前时间{date}", DateTime.Now);
            _logger.LogError("当前时间{date}", DateTime.Now);
            _logger.LogCritical("当前时间{date}", DateTime.Now);
            _logger.LogDebug("当前时间{date}", DateTime.Now);
            _logger.LogTrace("当前时间{date}", DateTime.Now);
            return "ok";
        }
        public object SerilogTest()
        {
            Log.Verbose("这是一个 Verbose 级别的日志");
            Log.Debug("这是一个 Debug 级别的日志");
            Log.Information("这是一个 Information 级别的日志");
            Log.Warning("这是一个 Warning 级别的日志");
            Log.Error("这是一个 Error 级别的日志");
            Log.Fatal("这是一个 Fatal 级别的日志");
            return "Serilog test completed";
        }
    }
}