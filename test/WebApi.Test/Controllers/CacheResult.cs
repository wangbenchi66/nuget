using Microsoft.AspNetCore.Mvc;
using WBC66.Cache.Core;
using WBC66.Core.Filters;

namespace WebApi.Test.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CacheResult : ControllerBase
    {
        private readonly ICacheResultService _cacheResultService;

        public CacheResult(ICacheResultService cacheResultService)
        {
            _cacheResultService = cacheResultService;
        }

        [HttpGet]
        [CacheResult(5)]
        public object Get()
        {
            _cacheResultService.GetStudentAsync("这是参数");
            return "ok";
        }

        [HttpPost("Post")]
        [IdempotenceTimeAttribute(0, 10, 1)]
        public async Task<object> Post(Student test)
        {
            return _cacheResultService.GetStudentAsync(test.Name);
        }
    }

    public class CacheResultService : ICacheResultService
    {
        [CacheResult(15)]
        public Student GetStudentAsync(string name)
        {
            return new Student { Name = name };
        }
    }

    public interface ICacheResultService /*: IProxyService*/
    {
        Student GetStudentAsync(string name);
    }

    public class Student
    {
        public string Name { get; set; }
    }
}