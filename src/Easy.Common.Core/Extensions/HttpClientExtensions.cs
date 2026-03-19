using System.Text;
using System.Text.Json;
using FileParameter = Easy.Common.Core.HttpClientFactoryExtensions.FileParameter;

namespace Easy.Common.Core;

/// <summary>
/// HttpClient 扩展方法
/// </summary>
public static class HttpClientExtensions
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    /// <summary>
    /// GET 请求并返回反序列化对象
    /// </summary>
    /// <typeparam name="T">返回类型</typeparam>
    /// <param name="client">HttpClient</param>
    /// <param name="requestUri">请求地址</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns></returns>
    public static async Task<T> GetAsync<T>(this HttpClient client, string requestUri, CancellationToken cancellationToken = default)
    {
        var response = await client.GetAsync(requestUri, cancellationToken);
        return await DeserializeResponseAsync<T>(response, cancellationToken);
    }

    /// <summary>
    /// POST JSON 请求并返回反序列化对象
    /// </summary>
    /// <typeparam name="T">返回类型</typeparam>
    /// <param name="client">HttpClient</param>
    /// <param name="requestUri">请求地址</param>
    /// <param name="data">请求参数</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns></returns>
    public static async Task<T> PostAsync<T>(this HttpClient client, string requestUri, object data, CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(data, _jsonOptions);
        using var req = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await client.PostAsync(requestUri, req, cancellationToken);
        return await DeserializeResponseAsync<T>(response, cancellationToken);
    }

    /// <summary>
    /// POST 表单请求并返回反序列化对象
    /// </summary>
    /// <typeparam name="T">返回类型</typeparam>
    /// <param name="client">HttpClient</param>
    /// <param name="requestUri">请求地址</param>
    /// <param name="data">请求参数</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns></returns>
    public static async Task<T> PostFormAsync<T>(this HttpClient client, string requestUri, object data, CancellationToken cancellationToken = default)
    {
        var dict = new Dictionary<string, string>();
        foreach (var prop in data.GetType().GetProperties())
        {
            var val = prop.GetValue(data);
            if (val != null)
            {
                dict.Add(prop.Name, val.ToString()!);
            }
        }

        using var content = new FormUrlEncodedContent(dict);
        var response = await client.PostAsync(requestUri, content, cancellationToken);
        return await DeserializeResponseAsync<T>(response, cancellationToken);
    }

    /// <summary>
    /// POST multipart 请求并返回反序列化对象
    /// </summary>
    /// <typeparam name="T">返回类型</typeparam>
    /// <param name="client">HttpClient</param>
    /// <param name="requestUri">请求地址</param>
    /// <param name="data">请求参数，支持 FileParameter、IEnumerable&lt;FileParameter&gt; 或普通表单字段</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns></returns>
    public static async Task<T> PostMultipartAsync<T>(this HttpClient client, string requestUri, object data, CancellationToken cancellationToken = default)
    {
        using var form = new MultipartFormDataContent();

        foreach (var prop in data.GetType().GetProperties())
        {
            var val = prop.GetValue(data);
            if (val == null)
            {
                continue;
            }

            if (val is FileParameter file)
            {
                var fileContent = new ByteArrayContent(file.Content);
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
                form.Add(fileContent, prop.Name, file.FileName);
            }
            else if (val is IEnumerable<FileParameter> files)
            {
                foreach (var f in files)
                {
                    var fileContent = new ByteArrayContent(f.Content);
                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(f.ContentType);
                    form.Add(fileContent, prop.Name, f.FileName);
                }
            }
            else
            {
                form.Add(new StringContent(val.ToString()!), prop.Name);
            }
        }

        var response = await client.PostAsync(requestUri, form, cancellationToken);
        return await DeserializeResponseAsync<T>(response, cancellationToken);
    }

    private static async Task<T> DeserializeResponseAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        if (typeof(T) == typeof(string))
        {
            return (T)(object)content;
        }

        return JsonSerializer.Deserialize<T>(content, _jsonOptions)!;
    }
}
