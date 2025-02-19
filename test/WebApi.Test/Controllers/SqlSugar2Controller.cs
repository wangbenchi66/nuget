using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Test.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SqlSugar2Controller : ControllerBase
    {
        /*private readonly UserRepository _userRepository;

        public SqlSugar2Controller(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet("Repository")]
        public object Repository()
        {
            //查询单个
            var obj = _userRepository.GetSingle(p => p.Id == 1);
            return "ok";
        }*/
    }
}