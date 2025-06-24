using hmt_energy_csharp;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using System;
using System.Text;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Information)
    .MinimumLevel.Override("Volo.Abp", LogEventLevel.Information)
    .MinimumLevel.Override("hmt_energy_csharp", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Async(c => c.File(new CompactJsonFormatter(), $"Logs/logs.txt", rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true, fileSizeLimitBytes: 100 * 1024 * 1024, encoding: Encoding.UTF8, retainedFileCountLimit: 100))
    .WriteTo.Async(c => c.Console())
    .WriteTo.Async(c => c.Debug())
    .CreateLogger();

try
{
    Log.Information("Starting hmt_energy_csharp.Shipend.Host.");
    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseNacosConfig("NacosConfig");
    builder.Host.AddAppSettingsSecretsJson()
        .UseAutofac()
        .UseSerilog();
    await builder.AddApplicationAsync<hmt_energy_csharpShipendHostModule>();
    var app = builder.Build();
    await app.InitializeApplicationAsync();
    await app.RunAsync();
    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly!");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}