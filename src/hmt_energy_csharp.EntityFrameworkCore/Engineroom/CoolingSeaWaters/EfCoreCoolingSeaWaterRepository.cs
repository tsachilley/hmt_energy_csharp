using hmt_energy_csharp.EntityFrameworkCore.Oracle;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace hmt_energy_csharp.Engineroom.CoolingSeaWaters
{
    public class EfCoreCoolingSeaWaterRepository : EfCoreRepository<hmt_energy_csharpOracleDbContext, CoolingSeaWater, long>, ICoolingSeaWaterRepository
    {
        public EfCoreCoolingSeaWaterRepository(IDbContextProvider<hmt_energy_csharpOracleDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
    }
}