using System.Net.Http.Headers;

namespace Easy.Common.Core
{
    /// <summary>
    /// http帮助类,需要注册AddHttpClient功能
    /// </summary>
    public class HttpHelper : IHttpHelper
    {
        private string Content_Type = "application/json";
        private readonly IHttpClientFactory _httpClient;

        public HttpHelper(IHttpClientFactory httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// HttpClient异步发送Post请求方法
        /// </summary>
        /// <param name="sUrl"></param>
        /// <param name="sParam"></param>
        /// <returns></returns>
        public async Task<T> PostAsync<T>(string sUrl, string? sParam = null)
        {
# if NET8_0_OR_GREATER
            HttpContent content = new StringContent(sParam, mediaType: new MediaTypeHeaderValue(Content_Type));
#else
            HttpContent content = new StringContent(sParam);
            content.Headers.ContentType = new MediaTypeHeaderValue(Content_Type);
#endif
            var client = _httpClient.CreateClient();
            HttpResponseMessage req = await client.PostAsync(sUrl, content);
            if (req.IsSuccessStatusCode)
            {
                var res = await req.Content.ReadAsStringAsync();
                return JsonComm.ToObject<T>(res);
            }
            else
            {
                return default;
            }
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
            var url = sUrl;
            if (sParam != null)
            {
                if (sUrl.IndexOf("?") != -1)
                    url = sUrl + "?" + sParam;
                else
                    url = sUrl + "&" + sParam;
            }
            //设置Content_Type请求头
            var client = _httpClient.CreateClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Content_Type));
            HttpResponseMessage req = await client.GetAsync(url);
            if (req.IsSuccessStatusCode)
            {
                var res = await req.Content.ReadAsStringAsync();
                return JsonComm.ToObject<T>(res);
            }
            else
            {
                return default;
            }
        }

        /// <summary>
        /// 发送带参数的httpclient get请求
        /// </summary>
        /// <param name="sUrl"></param>
        /// <param name="sParam"></param>
        /// <returns></returns>
        public T Get<T>(string sUrl, string? sParam = null) => GetAsync<T>(sUrl, sParam).Result;

        public async Task<string> GetAsync(string sUrl, string? sParam = null) => await GetAsync<string>(sUrl, sParam);

        public string Get(string sUrl, string? sParam = null) => GetAsync<string>(sUrl, sParam).Result;

        public async Task<string> PostAsync(string sUrl, string? sParam = null) => await PostAsync<string>(sUrl, sParam);

        public string Post(string sUrl, string? sParam = null) => PostAsync<string>(sUrl, sParam).Result;
    }
}