using hmt_energy_csharp.EntityFrameworkCore.Oracle;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace hmt_energy_csharp.Engineroom.FOs
{
    public class EfCoreFORepository : EfCoreRepository<hmt_energy_csharpOracleDbContext, FO, long>, IFORepository
    {
        public EfCoreFORepository(IDbContextProvider<hmt_energy_csharpOracleDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
    }
}