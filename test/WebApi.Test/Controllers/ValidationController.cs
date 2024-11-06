using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Test.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ValidationController : ControllerBase
    {
        [HttpPost("Post")]
        public IActionResult Post(Test test)
        {
            return Ok();
        }
        public class Test : IValidatableObject
        {
            [Required(ErrorMessage = "Name不能为空")]
            public string Name { get; set; }
            [Required]
            public string Age { get; set; }
            //自定义验证,实现IValidatableObject接口,如果不需要可以不继承
            public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
            {
                if (Name == "admin")
                {
                    yield return new ValidationResult("Name不能为admin", new[] { nameof(Name) });
                }
                if (Name == Age)
                {
                    yield return new ValidationResult("Name不能等于Age");
                }
            }
        }
    }
}