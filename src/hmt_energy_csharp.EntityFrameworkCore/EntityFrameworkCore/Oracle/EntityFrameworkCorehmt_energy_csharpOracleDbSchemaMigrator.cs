using hmt_energy_csharp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace hmt_energy_csharp.EntityFrameworkCore.Oracle
{
    public class EntityFrameworkCorehmt_energy_csharpOracleDbSchemaMigrator : Ihmt_energy_csharpDbSchemaMigrator, ITransientDependency
    {
        private readonly IServiceProvider _serviceProvider;

        public EntityFrameworkCorehmt_energy_csharpOracleDbSchemaMigrator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task MigrateAsync()
        {
            await _serviceProvider
                .GetRequiredService<hmt_energy_csharpOracleDbContext>()
                .Database
                .MigrateAsync();
        }
    }
}