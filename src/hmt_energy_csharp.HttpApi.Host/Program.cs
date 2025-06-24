using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using System;
using System.Text;
using System.Threading.Tasks;

namespace hmt_energy_csharp;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
#if DEBUG
            .MinimumLevel.Debug()
#else
            .MinimumLevel.Information()
#endif
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Async(c => c.File(new CompactJsonFormatter(), $"Logs/logs.log", rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true, fileSizeLimitBytes: 100 * 1024 * 1024, encoding: Encoding.UTF8, retainedFileCountLimit: 100))
            .WriteTo.Async(c => c.Console())
            .WriteTo.Async(c => c.Debug())
            .CreateLogger();

        try
        {
            Log.Information("Starting hmt_energy_csharp.HttpApi.Host.");
            var builder = WebApplication.CreateBuilder(args);
            builder.Host.UseNacosConfig("NacosConfig");
            builder.Host.AddAppSettingsSecretsJson()
                .UseAutofac()
                .UseSerilog();
            await builder.AddApplicationAsync<hmt_energy_csharpHttpApiHostModule>();
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
    }
}