namespace WebApi.Test.Apis
{
    /// <summary>
    /// 服务注册相关
    /// </summary>
    public class DiApis : BaseApi
    {
        private readonly AService _service;
        private readonly ILogger<DiApis> _logger;

        public DiApis(AService service, ILogger<DiApis> logger)
        {
            _service = service;
            _logger = logger;
        }
        public string GetData()
        {
            return _service.GetData();
        }
    }
    public class AService
    {
        public string GetData() => "Hello from AService";
    }
}