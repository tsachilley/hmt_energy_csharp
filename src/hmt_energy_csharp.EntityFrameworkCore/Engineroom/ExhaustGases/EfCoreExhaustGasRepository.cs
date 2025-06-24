using hmt_energy_csharp.EntityFrameworkCore.Oracle;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace hmt_energy_csharp.Engineroom.ExhaustGases
{
    public class EfCoreExhaustGasRepository : EfCoreRepository<hmt_energy_csharpOracleDbContext, ExhaustGas, long>, IExhaustGasRepository
    {
        public EfCoreExhaustGasRepository(IDbContextProvider<hmt_energy_csharpOracleDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
    }
}