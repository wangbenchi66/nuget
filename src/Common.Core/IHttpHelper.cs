
namespace Common.Core
{
    /// <summary>
    /// http帮助类
    /// </summary>
    public interface IHttpHelper
    {
        /// <summary>
        /// get请求
        /// </summary>
        /// <param name="sUrl"></param>
        /// <param name="sParam"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T Get<T>(string sUrl, string? sParam = null);

        /// <summary>
        /// get请求
        /// </summary>
        /// <param name="sUrl"></param>
        /// <param name="sParam"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Task<T> GetAsync<T>(string sUrl, string? sParam = null);

        /// <summary>
        /// post请求
        /// </summary>
        /// <param name="sUrl"></param>
        /// <param name="sParam"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T Post<T>(string sUrl, string? sParam = null);

        /// <summary>
        /// post请求
        /// </summary>
        /// <param name="sUrl"></param>
        /// <param name="sParam"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Task<T> PostAsync<T>(string sUrl, string? sParam = null);
    }
}