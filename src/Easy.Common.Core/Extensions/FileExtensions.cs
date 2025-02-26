using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Easy.Common.Core;

public static class FileExtensions
{

    /// <summary>
    /// 获取文件路径下的文件夹名称
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <returns></returns>
    public static string GetDirectoryName(string filePath)
    {
        return Path.GetDirectoryName(filePath);
    }

    /// <summary>
    /// 获取文件夹下的文件名称(包含子目录及模糊匹配)
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <param name="searchPattern">文件模糊匹配规则</param>
    /// <param name="searchOption">遍历所有文件夹</param>
    /// <returns></returns>
    public static string[] GetFiles(string filePath, string searchPattern = "*.*", SearchOption searchOption = SearchOption.AllDirectories)
    {
        return Directory.GetFiles(filePath, searchPattern, searchOption);
    }

    /// <summary>
    /// 获取文件内容
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <returns></returns>
    public static string? GetFileConnent(string filePath)
    {
        //文件是否存在
        if (!File.Exists(filePath))
            return null;
        return File.ReadAllText(filePath);
    }

    /// <summary>
    /// 获取文件内容(异步)
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <returns></returns>
    public static async Task<string?> GetFileConnentAsync(string filePath)
    {
        if (!File.Exists(filePath))
            return null;
        return await File.ReadAllTextAsync(filePath);
    }

    /// <summary>
    /// 获取文件信息
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <returns></returns>
    public static FileInfo? GetFileInfo(string filePath)
    {
        if (!File.Exists(filePath))
            return null;
        return new FileInfo(filePath);
    }

    /// <summary>
    /// 文件备份
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="backupPath"></param>
    public static bool FileBackup(string filePath, string backupPath = null)
    {
        if (!File.Exists(filePath))
            return false;
        if (backupPath.IsNull())
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            string fileExtension = Path.GetExtension(filePath);
            string directoryName = GetDirectoryName(filePath);
            backupPath = Path.Combine(directoryName, $"{fileName}_{DateTime.Now:yyyyMMddHHmmssfff}{fileExtension}");
        }
        File.Copy(filePath, backupPath);
        return true;
    }

    /// <summary>
    /// 根据路径获取文件内容
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <returns></returns>
    public static string GetFile(string filePath)
    {
        string json = string.Empty;
        using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite, FileShare.ReadWrite))
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            using (StreamReader sr = new StreamReader(fs))
            {
                json = sr.ReadToEnd().ToString();
            }
        }
        return json;
    }

    /// <summary>
    /// 创建文件
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="content"></param>
    public static void CreateFile(string filePath, string content)
    {
        if (!File.Exists(filePath))
        {
            CreateDirectory(filePath);
            using (FileStream fs = File.Create(filePath))
            {
                byte[] bytes = Encoding.UTF8.GetBytes(content);
                fs.Write(bytes, 0, bytes.Length);
            }
        }
    }

    /// <summary>
    /// 创建文件夹,如果存在则不创建(可选 默认创建)
    /// </summary>
    /// <param name="path">如果是文件路径则会自动获取到文件夹</param>
    /// <param name="isCreate"></param>
    public static void CreateDirectory(string path, bool isCreate = true)
    {
        //判断是文件路径还是文件夹路径，如果是文件路径则获取文件夹路径
        string directoryPath = Path.GetExtension(path).IsNull() ? path : GetDirectoryName(path);
        if (directoryPath.IsNull())
            throw new Exception("文件路径错误");
        if (!Directory.Exists(directoryPath) && isCreate)
            Directory.CreateDirectory(directoryPath);
    }
}