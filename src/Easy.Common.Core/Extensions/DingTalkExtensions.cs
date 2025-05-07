using System.Text;

namespace Easy.Common.Core.Extensions
{
    public class DingTalkExtensions
    {

        private string _sec = string.Empty;

        private string _accessToken = string.Empty;

        public DingTalkExtensions(string sec, string accessToken)
        {
            _sec = sec;
            _accessToken = accessToken;
            if (_sec.IsNull() || _accessToken.IsNull())
            {
                throw new ArgumentNullException("sec 或 accessToken 是空的");
            }
        }


        /// <summary>
        /// 获取url
        /// </summary>
        /// <returns></returns>
        private string GetUrl()
        {
            if (_sec.IsNull()) return string.Empty;
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            string sign = FoundationExtensions.HmacSHA256(timestamp, _sec);
            return $"https://oapi.dingtalk.com/robot/send?access_token={_accessToken}&timestamp={timestamp}&sign={sign}";
        }

        /// <summary>
        /// 发送文本消息
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public async Task<bool> SendTextMsgAsync(string msg)
        {
            var param = new
            {
                msgtype = "text",
                text = new
                {
                    content = msg
                }
            };
            return await PostAsync(GetUrl(), param.ToJson());
        }

        /// <summary>
        /// 发送链接消息
        /// </summary>
        /// <param name="title"></param>
        /// <param name="text"></param>
        /// <param name="picUrl"></param>
        /// <param name="messageUrl"></param>
        /// <returns></returns>
        public async Task<bool> SendLinkMsgAsync(string title, string text, string picUrl, string messageUrl)
        {
            var param = new
            {
                msgtype = "link",
                link = new
                {
                    text,
                    title,
                    picUrl,
                    messageUrl
                }
            };
            return await PostAsync(GetUrl(), param.ToJson());
        }

        /// <summary>
        /// 发送markdown消息
        /// </summary>
        /// <param name="title"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public async Task<bool> SendMarkdownMsgAsync(string title, string text)
        {
            var param = new
            {
                msgtype = "markdown",
                markdown = new
                {
                    title,
                    text
                }
            };
            return await PostAsync(GetUrl(), param.ToJson());
        }

        private async Task<bool> PostAsync(string url, string param)
        {
            if (url.IsNull()) return false;
            try
            {
                HttpClient httpClient = new HttpClient();
                HttpContent httpContent = new StringContent(param, Encoding.UTF8, "application/json");
                var res = await httpClient.PostAsync(url, httpContent);
                return res.IsSuccessStatusCode;
            }
            catch (HttpRequestException ex)
            {
                // 处理请求异常
                Console.WriteLine($"钉钉推送请求失败: {ex.Message}");
            }
            catch (TaskCanceledException ex)
            {
                // 处理请求超时异常
                Console.WriteLine($"钉钉推送请求超时: {ex.Message}");
            }
            catch (Exception ex)
            {
                // 处理其他异常
                Console.WriteLine($"钉钉推送请求异常: {ex.Message}");
            }
            return false;
        }
    }
}