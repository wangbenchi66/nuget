using System.Net.Http.Headers;

namespace Easy.Common.Core
{
    /// <summary>
    /// http帮助类
    /// </summary>
    public class HttpHelper : IHttpHelper
    {
        private string Content_Type = "application/json";

        public HttpHelper()
        {
        }

        /// <summary>
        /// HttpClient异步发送Post请求方法
        /// </summary>
        /// <param name="sUrl"></param>
        /// <param name="sParam"></param>
        /// <returns></returns>
        public async Task<T> PostAsync<T>(string sUrl, string? sParam = null)
        {
            HttpClient http = new HttpClient();
# if NET8_0_OR_GREATER
            HttpContent content = new StringContent(sParam, mediaType: new MediaTypeHeaderValue(Content_Type));
#else
            HttpContent content = new StringContent(sParam);
            content.Headers.ContentType = new MediaTypeHeaderValue(Content_Type);
#endif
            HttpResponseMessage req = await http.PostAsync(sUrl, content);
            req.EnsureSuccessStatusCode();
            var res = await req.Content.ReadAsStringAsync();
            return JsonHelper.ToObject<T>(res);
        }

        /// <summary>
        /// HttpClient同步发送Post请求方法
        /// </summary>
        /// <param name="sUrl"></param>
        /// <param name="sParam"></param>
        /// <returns></returns>
        public T Post<T>(string sUrl, string? sParam = null) => PostAsync<T>(sUrl, sParam).Result;

        /// <summary>
        /// 发送带参数的httpclient get请求
        /// </summary>
        /// <param name="sUrl"></param>
        /// <param name="sParam"></param>
        /// <returns></returns>
        public async Task<T> GetAsync<T>(string sUrl, string? sParam = null)
        {
            HttpClient http = new HttpClient();
            var url = sUrl;
            if (sParam != null)
            {
                if (sUrl.IndexOf("?") != -1)
                    url = sUrl + "?" + sParam;
                else
                    url = sUrl + "&" + sParam;
            }
            //设置Content_Type请求头
            http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Content_Type));
            HttpResponseMessage req = await http.GetAsync(url);
            req.EnsureSuccessStatusCode();
            var res = req.Content.ReadAsStringAsync().Result;
            return JsonHelper.ToObject<T>(res);
        }

        /// <summary>
        /// 发送带参数的httpclient get请求
        /// </summary>
        /// <param name="sUrl"></param>
        /// <param name="sParam"></param>
        /// <returns></returns>
        public T Get<T>(string sUrl, string? sParam = null) => GetAsync<T>(sUrl, sParam).Result;
    }
}