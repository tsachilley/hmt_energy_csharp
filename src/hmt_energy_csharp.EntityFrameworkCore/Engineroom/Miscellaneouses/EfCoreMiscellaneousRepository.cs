using hmt_energy_csharp.EntityFrameworkCore.Oracle;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace hmt_energy_csharp.Engineroom.Miscellaneouses
{
    public class EfCoreMiscellaneousRepository : EfCoreRepository<hmt_energy_csharpOracleDbContext, Miscellaneous, long>, IMiscellaneousRepository
    {
        public EfCoreMiscellaneousRepository(IDbContextProvider<hmt_energy_csharpOracleDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
    }
}