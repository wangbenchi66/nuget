using Easy.Cache.Core;

namespace WebApi.Test.Apis;

/// <summary>
/// EasyCache相关接口测试
/// </summary>
public class EasyCacheApis : BaseApi
{
    private readonly IEasyCacheService _cacheService;

    public EasyCacheApis(IEasyCacheService cacheService)
    {
        _cacheService = cacheService;
    }

    //测试string类型的缓存
    public async Task<ApiResult> TestStringCache()
    {
        var key = "test_string_cache";
        var value = "Hello EasyCache!";
        await _cacheService.AddAsync(key, value, 30); //缓存30秒
        var cacheValue = await _cacheService.GetAsync<string>(key);
        return ApiResult.Ok(cacheValue);
    }

    //测试hash类型的缓存
    public async Task<ApiResult> TestHashCache()
    {
        var key = "test_hash_cache";
        var field = "field1";
        var value = "Hello EasyCache Hash!";
        await _cacheService.HSetAsync(key, field, value); //缓存30秒
        var cacheValue = await _cacheService.HGetAsync<string>(key, field);
        return ApiResult.Ok(cacheValue);
    }

    //测试hash类型删除某一个field
    public async Task<ApiResult> TestHashDeleteField(string field)
    {
        var key = "test_hash_cache";
        //await _cacheService.h(key, field);
        var cacheValue = await _cacheService.HGetAsync<string>(key, field);
        return ApiResult.Ok(cacheValue); //应该返回null
    }
}