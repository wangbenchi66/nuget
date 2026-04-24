using Easy.SqlSugar.Core;

namespace WebApi.Test.Apis;

/// <summary>
/// sqlsugar依赖注入相关接口测试
/// </summary>
public class SqlSugarDiApis : BaseApi
{
    private readonly IUserRepository _userRepository;

    public SqlSugarDiApis(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ApiResult> GetUserList()
    {
        var users = await _userRepository.GetSingleAsync(x => true);
        return ApiResult.Ok(users);
    }
}

public class UserRepository : BaseSqlSugarRepository<User>, IUserRepository
{
}

public interface IUserRepository : IBaseSqlSugarRepository<User>
{
}