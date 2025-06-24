using Consul;
using hmt_energy_csharp.Entites;
using hmt_energy_csharp.EntityFrameworkCore;
using hmt_energy_csharp.MultiTenancy;
using hmt_energy_csharp.Services;
using Medallion.Threading;
using Medallion.Threading.Redis;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.MultiTenancy;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Autofac;
using Volo.Abp.Caching;
using Volo.Abp.Caching.StackExchangeRedis;
using Volo.Abp.DistributedLocking;
using Volo.Abp.Modularity;
using Volo.Abp.Swashbuckle;
using Volo.Abp.VirtualFileSystem;

namespace hmt_energy_csharp;

[DependsOn(
    typeof(hmt_energy_csharpHttpApiModule),
    typeof(AbpAutofacModule),
    typeof(AbpCachingStackExchangeRedisModule),
    typeof(AbpDistributedLockingModule),
    typeof(AbpAspNetCoreMvcUiMultiTenancyModule),
    typeof(hmt_energy_csharpApplicationModule),
    typeof(hmt_energy_csharpEntityFrameworkCoreModule),
    typeof(AbpAspNetCoreSerilogModule),
    typeof(AbpSwashbuckleModule)
)]
public class hmt_energy_csharpHttpApiHostModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        var hostingEnvironment = context.Services.GetHostingEnvironment();

        ConfigureConventionalControllers();
        ConfigureAuthentication(context, configuration);
        ConfigureCache(configuration);
        ConfigureVirtualFileSystem(context);
        ConfigureDataProtection(context, configuration, hostingEnvironment);
        ConfigureDistributedLocking(context, configuration);
        ConfigureCors(context, configuration);
        ConfigureSwaggerServices(context, configuration);
        ConfigureGrpc(context, configuration);
        ConfigureConsul(context, configuration);

        ConfigureUdp(context, configuration);
        ConfigureTcp(context, configuration);

        ConfigureOpenTelemetry(context, configuration);

        ConfigureStaticEntities(context, configuration);
    }

    private void ConfigureStaticEntities(ServiceConfigurationContext context, IConfiguration configuration)
    {
        context.Services
            //初始化activitysource
            .AddSingleton(new DiagnosticsConfig(configuration["jaeger:name"], new ActivitySource(configuration["jaeger:name"])));
    }

    private void ConfigureOpenTelemetry(ServiceConfigurationContext context, IConfiguration configuration)
    {
        context.Services
            .AddOpenTelemetry()
            .WithTracing(tracerProviderBuilder =>
                tracerProviderBuilder
                    .AddSource(configuration["jaeger:name"])
                    .ConfigureResource(resourceBuilder =>
                        resourceBuilder.AddService(configuration["jaeger:name"])
                    )
                    .AddAspNetCoreInstrumentation()
                    .AddGrpcClientInstrumentation()
                    .AddOtlpExporter(o =>
                    {
                        o.Endpoint = new Uri($"http://{configuration["jaeger:host"]}:{configuration["jaeger:port"]}");
                    })
                    .AddConsoleExporter()
            )
            .WithMetrics(metricsProviderBuilder =>
                metricsProviderBuilder
                    .ConfigureResource(resource =>
                        resource.AddService(configuration["jaeger:name"])
                    )
                    .AddAspNetCoreInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddConsoleExporter()
                    .AddOtlpExporter()
            );
    }

    private void ConfigureTcp(ServiceConfigurationContext context, IConfiguration configuration)
    {
        context.Services.AddHostedService<TcpService>();
    }

    private void ConfigureUdp(ServiceConfigurationContext context, IConfiguration configuration)
    {
        //context.Services.AddSingleton<EEUdpServer>();
        context.Services.AddHostedService<UdpService>();
    }

    private void ConfigureCache(IConfiguration configuration)
    {
        Configure<AbpDistributedCacheOptions>(options => { options.KeyPrefix = "hmt_energy_csharp:"; });
    }

    private void ConfigureVirtualFileSystem(ServiceConfigurationContext context)
    {
        var hostingEnvironment = context.Services.GetHostingEnvironment();

        if (hostingEnvironment.IsDevelopment())
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                //options.FileSets.ReplaceEmbeddedByPhysical<hmt_energy_csharpDomainSharedModule>(
                //    Path.Combine(hostingEnvironment.ContentRootPath,
                //        $"..{Path.DirectorySeparatorChar}hmt_energy_csharp.Domain.Shared"));
                options.FileSets.ReplaceEmbeddedByPhysical<hmt_energy_csharpDomainModule>(
                    Path.Combine(hostingEnvironment.ContentRootPath,
                        $"..{Path.DirectorySeparatorChar}hmt_energy_csharp.Domain"));
                options.FileSets.ReplaceEmbeddedByPhysical<hmt_energy_csharpApplicationContractsModule>(
                    Path.Combine(hostingEnvironment.ContentRootPath,
                        $"..{Path.DirectorySeparatorChar}hmt_energy_csharp.Application.Contracts"));
                options.FileSets.ReplaceEmbeddedByPhysical<hmt_energy_csharpApplicationModule>(
                    Path.Combine(hostingEnvironment.ContentRootPath,
                        $"..{Path.DirectorySeparatorChar}hmt_energy_csharp.Application"));
            });
        }
    }

    private void ConfigureConventionalControllers()
    {
        Configure<AbpAspNetCoreMvcOptions>(options =>
        {
            options.ConventionalControllers.Create(typeof(hmt_energy_csharpApplicationModule).Assembly);
        });
    }

    private void ConfigureAuthentication(ServiceConfigurationContext context, IConfiguration configuration)
    {
        context.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = configuration["AuthServer:Authority"];
                options.RequireHttpsMetadata = Convert.ToBoolean(configuration["AuthServer:RequireHttpsMetadata"]);
                options.Audience = "hmt_energy_csharp";
            });
    }

    private static void ConfigureSwaggerServices(ServiceConfigurationContext context, IConfiguration configuration)
    {
        context.Services.AddAbpSwaggerGenWithOAuth(
            configuration["AuthServer:Authority"],
            new Dictionary<string, string>
            {
                    {"hmt_energy_csharp", "hmt_energy_csharp API"}
            },
            options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "hmt_energy_csharp API", Version = "v1" });
                options.DocInclusionPredicate((docName, description) => true);
                options.CustomSchemaIds(type => type.FullName);
            });
    }

    private void ConfigureDataProtection(
        ServiceConfigurationContext context,
        IConfiguration configuration,
        IWebHostEnvironment hostingEnvironment)
    {
        var dataProtectionBuilder = context.Services.AddDataProtection().SetApplicationName("hmt_energy_csharp");
        if (!hostingEnvironment.IsDevelopment())
        {
            var redis = ConnectionMultiplexer.Connect(configuration["Redis:Configuration"]);
            dataProtectionBuilder.PersistKeysToStackExchangeRedis(redis, "hmt_energy_csharp-Protection-Keys");
        }
    }

    private void ConfigureDistributedLocking(
        ServiceConfigurationContext context,
        IConfiguration configuration)
    {
        context.Services.AddSingleton<IDistributedLockProvider>(sp =>
        {
            var connection = ConnectionMultiplexer
                .Connect(configuration["Redis:Configuration"]);
            return new RedisDistributedSynchronizationProvider(connection.GetDatabase());
        });
    }

    private void ConfigureCors(ServiceConfigurationContext context, IConfiguration configuration)
    {
        context.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder
                    .WithOrigins(
                        configuration["App:CorsOrigins"]
                            .Split(",", StringSplitOptions.RemoveEmptyEntries)
                            .Select(o => o.RemovePostFix("/"))
                            .ToArray()
                    )
                    .WithAbpExposedHeaders()
                    .SetIsOriginAllowedToAllowWildcardSubdomains()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
    }

    private void ConfigureGrpc(ServiceConfigurationContext context, IConfiguration configuration)
    {
        context.Services.AddGrpc(options =>
        {
            options.EnableDetailedErrors = true;
        });
    }

    private void ConfigureConsul(ServiceConfigurationContext context, IConfiguration configuration)
    {
        context.Services.AddSingleton<IConsulClient>(new ConsulClient(c =>
        {
            c.Address = new Uri(configuration["Consul:Address"]);
        }));
        context.Services.AddHostedService<ConsulRegisterService>();
        context.Services.AddSingleton<IConsulService, ConsulService>();
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        var env = context.GetEnvironment();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseAbpRequestLocalization();
        app.UseCorrelationId();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseCors();
        app.UseAuthentication();

        if (MultiTenancyConsts.IsEnabled)
        {
            app.UseMultiTenancy();
        }

        app.UseAuthorization();

        app.UseSwagger();
        app.UseAbpSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "hmt_energy_csharp API");

            var configuration = context.GetConfiguration();
            options.OAuthClientId(configuration["AuthServer:SwaggerClientId"]);
            options.OAuthScopes("hmt_energy_csharp");
        });

        app.UseAuditing();
        app.UseAbpSerilogEnrichers();
        app.UseUnitOfWork();
        app.UseConfiguredEndpoints(action =>
        {
            action.MapGrpcService<HealthService>();
            action.MapGrpcService<VoyageDataService>();
            action.MapGrpcService<NavigationDataService>();
        });
    }
}