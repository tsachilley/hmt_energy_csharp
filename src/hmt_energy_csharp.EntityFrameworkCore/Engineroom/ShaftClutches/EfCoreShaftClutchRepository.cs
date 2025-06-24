using hmt_energy_csharp.EntityFrameworkCore.Oracle;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace hmt_energy_csharp.Engineroom.ShaftClutches
{
    public class EfCoreShaftClutchRepository : EfCoreRepository<hmt_energy_csharpOracleDbContext, ShaftClutch, long>, IShaftClutchRepository
    {
        public EfCoreShaftClutchRepository(IDbContextProvider<hmt_energy_csharpOracleDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
    }
}