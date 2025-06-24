using hmt_energy_csharp.EntityFrameworkCore.Oracle;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace hmt_energy_csharp.Engineroom.FOSupplyUnits
{
    public class EfCoreFOSupplyUnitRepository : EfCoreRepository<hmt_energy_csharpOracleDbContext, FOSupplyUnit, long>, IFOSupplyUnitRepository
    {
        public EfCoreFOSupplyUnitRepository(IDbContextProvider<hmt_energy_csharpOracleDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
    }
}