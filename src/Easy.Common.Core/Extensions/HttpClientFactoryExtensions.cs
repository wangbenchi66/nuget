using System.Text;
using System.Text.Json;

namespace Easy.Common.Core;

public static class HttpClientFactoryExtensions
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    /// <summary>
    /// GET 返回反序列化对象
    /// </summary>
    /// <typeparam name="T">返回类型</typeparam>
    /// <param name="factory">HTTP客户端工厂</param>
    /// <param name="clientName">请求客户端名称</param>
    /// <param name="requestUri">请求地址</param>
    /// <returns></returns>
    public static async Task<T> GetAsync<T>(this IHttpClientFactory factory, string clientName, string requestUri)
    {
        var client = factory.CreateClient(clientName);
        var response = await client.GetAsync(requestUri);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        if (typeof(T) == typeof(string))
            return (T)(object)content;

        return JsonSerializer.Deserialize<T>(content, _jsonOptions)!;
    }

    /// <summary>
    /// POST 返回反序列化对象(form json)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="factory">HTTP客户端工厂</param>
    /// <param name="clientName">请求客户端名称</param>
    /// <param name="requestUri">请求地址</param>
    /// <param name="data">请求参数</param>
    /// <returns></returns>
    public static async Task<T> PostAsync<T>(this IHttpClientFactory factory, string clientName, string requestUri, object data)
    {
        var client = factory.CreateClient(clientName);
        var json = JsonSerializer.Serialize(data, _jsonOptions);
        var req = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await client.PostAsync(requestUri, req);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        if (typeof(T) == typeof(string))
            return (T)(object)content;

        return JsonSerializer.Deserialize<T>(content, _jsonOptions)!;
    }

    /// <summary>
    /// POST 表单数据
    /// </summary>
    /// <param name="factory">HTTP客户端工厂</param>
    /// <param name="clientName">请求客户端名称</param>
    /// <param name="requestUri">请求地址</param>
    /// <param name="data">请求参数</param>
    /// <typeparam name="T">返回类型</typeparam>
    /// <returns></returns>
    public static async Task<T> PostFormAsync<T>(this IHttpClientFactory factory, string clientName, string requestUri, object data)
    {
        var client = factory.CreateClient(clientName);
        var dict = new Dictionary<string, string>();

        foreach (var prop in data.GetType().GetProperties())
        {
            var val = prop.GetValue(data);
            if (val != null)
                dict.Add(prop.Name, val.ToString()!);
        }

        var content = new FormUrlEncodedContent(dict);
        var response = await client.PostAsync(requestUri, content);
        response.EnsureSuccessStatusCode();

        var resp = await response.Content.ReadAsStringAsync();
        if (typeof(T) == typeof(string))
            return (T)(object)resp;

        return JsonSerializer.Deserialize<T>(resp, _jsonOptions)!;
    }
    /// <summary>
    /// 文件参数
    /// </summary>
    public class FileParameter
    {
        /// <summary>
        /// 文件内容
        /// </summary>
        public byte[] Content { get; set; } = Array.Empty<byte>();
        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileName { get; set; } = string.Empty;
        /// <summary>
        /// 文件类型
        /// </summary>
        public string ContentType { get; set; } = "application/octet-stream";
    }
    /// <summary>
    /// POST 文件上传
    /// </summary>
    /// <param name="factory">HTTP客户端工厂</param>
    /// <param name="clientName">请求客户端名称</param>
    /// <param name="requestUri">请求地址</param>
    /// <param name="data">请求参数 FileParameter是单文件, IEnumerable[FileParameter]是多文件,new { id = 123, files }是表单参数+文件</param>
    /// <typeparam name="T">返回类型</typeparam>
    /// <returns></returns>
    public static async Task<T> PostMultipartAsync<T>(this IHttpClientFactory factory, string clientName, string requestUri, object data)
    {
        var client = factory.CreateClient(clientName);
        var form = new MultipartFormDataContent();

        foreach (var prop in data.GetType().GetProperties())
        {
            var val = prop.GetValue(data);
            if (val == null) continue;

            if (val is FileParameter file)
            {
                form.Add(new ByteArrayContent(file.Content), prop.Name, file.FileName);
            }
            else if (val is IEnumerable<FileParameter> files)
            {
                foreach (var f in files)
                {
                    form.Add(new ByteArrayContent(f.Content), prop.Name, f.FileName);
                }
            }
            else
            {
                form.Add(new StringContent(val.ToString()!), prop.Name);
            }
        }

        var response = await client.PostAsync(requestUri, form);
        response.EnsureSuccessStatusCode();

        var resp = await response.Content.ReadAsStringAsync();
        if (typeof(T) == typeof(string))
            return (T)(object)resp;

        return JsonSerializer.Deserialize<T>(resp, _jsonOptions)!;
    }


}