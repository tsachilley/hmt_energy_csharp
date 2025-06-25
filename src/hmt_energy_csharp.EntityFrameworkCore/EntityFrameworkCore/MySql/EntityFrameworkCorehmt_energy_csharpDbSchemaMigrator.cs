using hmt_energy_csharp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace hmt_energy_csharp.EntityFrameworkCore.MySql;

public class EntityFrameworkCorehmt_energy_csharpDbSchemaMigrator
    : Ihmt_energy_csharpDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCorehmt_energy_csharpDbSchemaMigrator(
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolve the hmt_energy_csharpDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<hmt_energy_csharpDbContext>()
            .Database
            .MigrateAsync();
    }
}