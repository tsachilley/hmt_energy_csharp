using hmt_energy_csharp.EntityFrameworkCore.Oracle;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace hmt_energy_csharp.Engineroom.CompressedAirSupplies
{
    public class EfCoreCompressedAirSupplyRepository : EfCoreRepository<hmt_energy_csharpOracleDbContext, CompressedAirSupply, long>, ICompressedAirSupplyRepository
    {
        public EfCoreCompressedAirSupplyRepository(IDbContextProvider<hmt_energy_csharpOracleDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
    }
}