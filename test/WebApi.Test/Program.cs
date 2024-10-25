using Serilog;
using Serilog.Core;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
//Serilog
builder.Host.AddSerilogHost(configuration);
// Add services to the container.

var app = builder.Build();

// Configure the HTTP request pipeline.

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};
Log.Information("��־���ԣ�{time}", DateTime.Now);
Log.Warning("��־���ԣ�{time}", DateTime.Now);
Log.Error("��־���ԣ�{time}", DateTime.Now);
app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    Log.Information("����{@f}", forecast);
    return forecast;
});

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}