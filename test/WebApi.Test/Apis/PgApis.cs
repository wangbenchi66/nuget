using Easy.SqlSugar.Core;
using SqlSugar;

namespace WebApi.Test.Apis;

public class PgApis : BaseApi
{
    private readonly IBaseSqlSugarRepository<UserPg> _repository;

    public PgApis(IBaseSqlSugarRepository<UserPg> repository)
    {
        _repository = repository;
    }

    public object Get()
    {
        //return _repository.QueryPage(x => true, null, 2, 2);
        var a = SugarDbManger.GetConfigDbRepository<UserPg>("Pg");
        return a.AsQueryable().ToList();
        //var a = SugarDbManger.GetTenantDbRepository<UserPg>();
        //return a.AsQueryable().ToList();
    }

    public object Add()
    {
        var list = new List<UserPg>();
        for (int i = 0; i < 10; i++)
        {
            list.Add(new UserPg
            {
                Name = "Name" + i,
                Exj = new TestExj
                {
                    Age = 20 + i,
                    IsE = i % 2 == 0,
                    Name = "ExjName" + i
                }
            });
        }
        return _repository.InsertRange(list);
    }
}

//[Tenant("Pg")]
[SugarTable("j_user")]
public class UserPg
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true, ColumnName = "id")]
    public int Id { get; set; }

    [SugarColumn(ColumnName = "name")]
    public string Name { get; set; }

    [SugarColumn(ColumnName = "exj", IsJson = true)]
    public TestExj Exj { get; set; }
}

public class TestExj
{
    public string Name { get; set; }
    public int Age { get; set; }
    public bool IsE { get; set; }
}