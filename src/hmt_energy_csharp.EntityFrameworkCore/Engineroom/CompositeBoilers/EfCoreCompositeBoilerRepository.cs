using hmt_energy_csharp.EntityFrameworkCore.Oracle;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace hmt_energy_csharp.Engineroom.CompositeBoilers
{
    public class EfCoreCompositeBoilerRepository : EfCoreRepository<hmt_energy_csharpOracleDbContext, CompositeBoiler, long>, ICompositeBoilerRepository
    {
        public EfCoreCompositeBoilerRepository(IDbContextProvider<hmt_energy_csharpOracleDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
    }
}