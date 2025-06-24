using hmt_energy_csharp.EntityFrameworkCore.Oracle;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace hmt_energy_csharp.Engineroom.CoolingFreshWaters
{
    public class EfCoreCoolingFreshWaterRepository : EfCoreRepository<hmt_energy_csharpOracleDbContext, CoolingFreshWater, long>, ICoolingFreshWaterRepository
    {
        public EfCoreCoolingFreshWaterRepository(IDbContextProvider<hmt_energy_csharpOracleDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
    }
}