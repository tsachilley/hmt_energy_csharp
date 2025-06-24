using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace hmt_energy_csharp.Data;

/* This is used if database provider does't define
 * Ihmt_energy_csharpDbSchemaMigrator implementation.
 */

public class Nullhmt_energy_csharpDbSchemaMigrator : Ihmt_energy_csharpDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}