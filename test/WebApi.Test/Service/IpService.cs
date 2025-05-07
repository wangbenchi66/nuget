using IP2Region.Net.Abstractions;
using IP2Region.Net.XDB;
using Serilog;
using WebApi.Test.Apis;

namespace WebApi.Test.Service;

public class IpService
{
    private readonly ISearcher _searcher;

    public IpService(string dbPath)
    {
        _searcher = new Searcher(CachePolicy.Content, dbPath); // 全内存方式
    }

    public string GetRegion(string ip)
    {
        try
        {
            return _searcher.Search(ip);
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            return null;
        }
    }
}