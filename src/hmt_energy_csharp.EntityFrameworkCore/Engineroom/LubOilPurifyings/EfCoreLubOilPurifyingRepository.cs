using hmt_energy_csharp.EntityFrameworkCore.Oracle;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace hmt_energy_csharp.Engineroom.LubOilPurifyings
{
    public class EfCoreLubOilPurifyingRepository : EfCoreRepository<hmt_energy_csharpOracleDbContext, LubOilPurifying, long>, ILubOilPurifyingRepository
    {
        public EfCoreLubOilPurifyingRepository(IDbContextProvider<hmt_energy_csharpOracleDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
    }
}