using System.Threading.Tasks;

namespace hmt_energy_csharp.Data;

public interface Ihmt_energy_csharpDbSchemaMigrator
{
    Task MigrateAsync();
}