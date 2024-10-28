using UnitTest.Repository;
using WBC66.Serilog.Core;
using WBC66.SqlSugar.Core;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
//Serilog
builder.Host.AddSerilogHost(configuration);

//NLong
//builder.AddNLogSteup(configuration);
// Add services to the container.
//×¢Èë
builder.Services.AddSingleton<IUserRepository, UserRepository>();
//SqlSugar
builder.Services.AddSqlSugarSetup(configuration);
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
//app.UseAuthorization();

app.MapControllers();

app.Run();