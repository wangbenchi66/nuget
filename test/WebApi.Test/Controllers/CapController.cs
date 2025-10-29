using DotNetCore.CAP;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Test.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CapController : ControllerBase
    {
        private readonly ICapPublisher _capPublisher;

        public CapController(ICapPublisher capPublisher)
        {
            _capPublisher = capPublisher;
        }

        /// <summary>
        /// 测试cap发送消息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task CapSendMessage()
        {
            CapMessage message = new CapMessage
            {
                Name = Guid.NewGuid().ToString(),
                Time = DateTime.Now
            };
            await _capPublisher.PublishAsync("test.cap.sendmessage", message);
        }

        /// <summary>
        /// 测试cap接受消息
        /// </summary>
        /// <param name="message"></param>
        [NonAction]
        [CapSubscribe("test.cap.sendmessage")]
        public void CapReceiveMessage(CapMessage message)
        {
            Console.WriteLine($"接收到cap消息：{message.ToJson()}");
        }

        public class CapMessage
        {
            public string Name { get; set; }
            public DateTime Time { get; set; }
        }
    }
}