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
            return "ok";
        }
    }
}