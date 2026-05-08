using Easy.SqlSugar.Core;
using SqlSugar;
using WBC66.Autofac.Core;

namespace WebApi.Test.Apis;

/// <summary>
/// sqlsugar依赖注入相关接口测试
/// </summary>
public class SqlSugarDiApis : BaseApi
{
    private readonly UserRepository _userRepository;

    private readonly BaseSqlSugarRepository<User> _userReps;

    public SqlSugarDiApis(UserRepository userRepository, ISqlSugarClient sqlSugarClient, BaseSqlSugarRepository<User> userReps)
    {
        _userRepository = userRepository;
        _userReps = userReps;
    }

    public async Task<ApiResult> GetUserList()
    {
        var users = await _userReps.GetListAsync(x => true);
        //var users = await _userRepository.GetSingleAsync(x => true);
        return ApiResult.Ok(users);
    }
}

public class UserRepository : BaseSqlSugarRepository<User>, IScoped
{
}