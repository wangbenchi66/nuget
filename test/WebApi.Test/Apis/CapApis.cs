/*using DotNetCore.CAP;

namespace WebApi.Test.Apis
{
    /// <summary>
    /// cap测试
    /// </summary>
    public class CapApis : BaseApi, ICapSubscribe
    {
        private readonly ICapPublisher _capPublisher;

        public CapApis(ICapPublisher capPublisher)
        {
            _capPublisher = capPublisher;
        }

        /// <summary>
        /// 测试cap发送消息
        /// </summary>
        /// <returns></returns>
        public async Task CapSendMessage()
        {
            await _capPublisher.PublishAsync("test.cap.sendmessage", new { Name = $"cap test message {DateTime.Now}" });
        }

        /// <summary>
        /// 测试cap接受消息
        /// </summary>
        /// <param name="message"></param>
        [CapSubscribe("test.cap.sendmessage")]
        public void CapReceiveMessage(dynamic message)
        {
            Console.WriteLine($"接收到cap消息：{message.Name}");
        }
    }
}*/