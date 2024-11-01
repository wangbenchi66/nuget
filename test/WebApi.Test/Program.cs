using SqlSugar.IOC;
using UnitTest.Repository;
using WBC66.SqlSugar.Core;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
//Serilog
//builder.Host.AddSerilogHost(configuration);

//NLong
//builder.AddNLogSteup(configuration);
// Add services to the container.

//��ȡ�����ļ�
//var efOptions = configuration.GetSection("DBS").Get<List<EFOptions>>()[0];
//builder.Services.AddEFSetup<TestDBContext>(efOptions);
//builder.Services.AddScoped<IUserEFRepository, UserEFRepository>();
//ע��
builder.Services.AddSingleton<IUserRepository, UserRepository>();
//SqlSugar
builder.Services.AddSqlSugarSetup(configuration.GetSection("DBS").Get<List<IocConfig>>(), true, config =>
{
    config.Aop.OnLogExecuting = (sql, pars) =>
    {
        Console.WriteLine("�����Զ����¼�{0}", sql);
    };
});
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
//app.UseAuthorization();

app.MapControllers();

app.Run();