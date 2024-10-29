using UnitTest;
using UnitTest.Repository;
using WBC66.EF.Core;
using WBC66.Serilog.Core;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
//Serilog
builder.Host.AddSerilogHost(configuration);

//NLong
//builder.AddNLogSteup(configuration);
// Add services to the container.

var efOptions = configuration.GetSection("DBS").Get<List<EFOptions>>()[0];
builder.Services.AddEFSetup<TestDBContext>(efOptions);
builder.Services.AddScoped<IUserEFRepository, UserEFRepository>();
//×¢Èë
builder.Services.AddSingleton<IUserRepository, UserRepository>();
//SqlSugar
//builder.Services.AddSqlSugarSetup(configuration);
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
//app.UseAuthorization();

app.MapControllers();

app.Run();