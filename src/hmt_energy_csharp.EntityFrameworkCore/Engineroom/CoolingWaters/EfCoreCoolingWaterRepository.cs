using hmt_energy_csharp.EntityFrameworkCore.Oracle;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace hmt_energy_csharp.Engineroom.CoolingWaters
{
    public class EfCoreCoolingWaterRepository : EfCoreRepository<hmt_energy_csharpOracleDbContext, CoolingWater, long>, ICoolingWaterRepository
    {
        public EfCoreCoolingWaterRepository(IDbContextProvider<hmt_energy_csharpOracleDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
    }
}