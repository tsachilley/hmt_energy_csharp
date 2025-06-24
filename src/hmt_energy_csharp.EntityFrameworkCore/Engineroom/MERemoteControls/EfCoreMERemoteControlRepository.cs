using hmt_energy_csharp.EntityFrameworkCore.Oracle;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace hmt_energy_csharp.Engineroom.MERemoteControls
{
    public class EfCoreMERemoteControlRepository : EfCoreRepository<hmt_energy_csharpOracleDbContext, MERemoteControl, long>, IMERemoteControlRepository
    {
        public EfCoreMERemoteControlRepository(IDbContextProvider<hmt_energy_csharpOracleDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
    }
}