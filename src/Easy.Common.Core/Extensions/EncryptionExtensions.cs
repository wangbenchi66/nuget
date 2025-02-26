using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Easy.Common.Core;

/// <summary>
/// 加密解密帮助类
/// </summary>
public static class EncryptionExtensions
{
    private static readonly string key = Guid.NewGuid().ToString().Substring(0, 16); // 16 bytes key for AES
    private static readonly string iv = Guid.NewGuid().ToString().Substring(0, 16); // 16 bytes IV for AES

    /// <summary>
    /// 加密并缩短
    /// </summary>
    /// <param name="plainText"></param>
    /// <returns></returns>
    public static string EncryptAndShorten(string plainText)
    {
        byte[] compressedData = Compress(plainText);

        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Encoding.UTF8.GetBytes(key);
            aesAlg.IV = Encoding.UTF8.GetBytes(iv);

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    csEncrypt.Write(compressedData, 0, compressedData.Length);
                }
                byte[] encrypted = msEncrypt.ToArray();
                string base64Encrypted = Convert.ToBase64String(encrypted);
                return Base64UrlEncode(base64Encrypted);
            }
        }
    }
    /// <summary>
    /// Base64Url编码
    /// </summary>
    /// <param name="base64"></param>
    /// <returns></returns>
    private static string Base64UrlEncode(string base64)
    {
        return base64.Replace('+', '-').Replace('/', '_').Replace("=", "");
    }
    /// <summary>
    /// 解密并还原
    /// </summary>
    /// <param name="cipherText"></param>
    /// <returns></returns>
    public static string DecryptAndUnshorten(string cipherText)
    {
        cipherText = cipherText.Replace('-', '+').Replace('_', '/').PadRight(cipherText.Length + (4 - cipherText.Length % 4) % 4, '=');
        byte[] cipherTextBytes = Convert.FromBase64String(cipherText);

        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Encoding.UTF8.GetBytes(key);
            aesAlg.IV = Encoding.UTF8.GetBytes(iv);

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msDecrypt = new MemoryStream(cipherTextBytes))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (MemoryStream msDecompressed = new MemoryStream())
                    {
                        csDecrypt.CopyTo(msDecompressed);
                        byte[] decompressedData = msDecompressed.ToArray();
                        return Decompress(decompressedData);
                    }
                }
            }
        }
    }
    /// <summary>
    /// 压缩
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    private static byte[] Compress(string data)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            using (GZipStream gzip = new GZipStream(ms, CompressionMode.Compress))
            {
                using (StreamWriter writer = new StreamWriter(gzip))
                {
                    writer.Write(data);
                }
            }
            return ms.ToArray();
        }
    }
    /// <summary>
    /// 解压
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    private static string Decompress(byte[] data)
    {
        using (MemoryStream ms = new MemoryStream(data))
        {
            using (GZipStream gzip = new GZipStream(ms, CompressionMode.Decompress))
            {
                using (StreamReader reader = new StreamReader(gzip))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}