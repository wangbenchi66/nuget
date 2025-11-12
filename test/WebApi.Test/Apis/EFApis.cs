using Microsoft.EntityFrameworkCore;
using WebApi.Test.EF;

namespace WebApi.Test.Apis;

public class EFApis : BaseApi
{

    /* private readonly IUserEFRepository _userRepository;

public EFApis(IUserEFRepository userRepository)
{
_userRepository = userRepository;
}

public object Get()
{
return _userRepository.GetSingle(p => p.Id == 1);
}

public object GetList()
{
return _userRepository.GetList(p => true);
}

public object Add1()
{
var user = _userRepository.EFContext.UserEF.Add(new UserEF { Name = "test" });
_userRepository.EFContext.SaveChanges();
return user.Entity.Id;
return _userRepository.InsertReturnIdentity(new UserEF { Name = "test" });
}

public object Update1()
{
var user = new UserEF { Id = 1, Name = "自定义", CreateTime = DateTime.Now };
//return _userRepository.Update(new UserEF { Id = 1, Name = "自定义" }, true);
//只更新某列
//return _userRepository.Update(user, x => new { x.Name, x.CreateTime }, true);
return _userRepository.Update(user, x => new { x.Name, x.CreateTime }, x => x.Id, false);
}

public object UpdateSetColumn()
{
var user = new UserEF { Id = 1, Name = "自定义", CreateTime = DateTime.Now };
return _userRepository.Update(user, x => new { x.Name, x.CreateTime }, true);
}

public object Update_SetColumn_WhereColumn()
{
var user = new UserEF { Name = "自定义", CreateTime = DateTime.Now };
return _userRepository.Update(user, x => new { x.CreateTime }, x => x.Name, true);
}

public object DeleteById()
{
return _userRepository.EFContext.UserEF.Where(p => p.Id == 100010).Delete();
}

public object QueryPage()
{
return _userRepository.QueryPage(p => true, "", 1, 10);
}*/


    private readonly TestDBContext _context;

    public EFApis(TestDBContext context)
    {
        _context = context;
    }

    public async Task<object> Get()
    {
        return await _context.Set<UserEF>().Where(x => x.Id == 1).ToListAsync();
    }
}