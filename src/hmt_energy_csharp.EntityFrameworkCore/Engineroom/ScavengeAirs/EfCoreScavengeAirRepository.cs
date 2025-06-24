using hmt_energy_csharp.EntityFrameworkCore.Oracle;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace hmt_energy_csharp.Engineroom.ScavengeAirs
{
    public class EfCoreScavengeAirRepository : EfCoreRepository<hmt_energy_csharpOracleDbContext, ScavengeAir, long>, IScavengeAirRepository
    {
        public EfCoreScavengeAirRepository(IDbContextProvider<hmt_energy_csharpOracleDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
    }
}