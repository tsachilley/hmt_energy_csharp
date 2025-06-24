using hmt_energy_csharp.EntityFrameworkCore.Oracle;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace hmt_energy_csharp.Engineroom.MainSwitchboards
{
    public class EfCoreMainSwitchboardRepository : EfCoreRepository<hmt_energy_csharpOracleDbContext, MainSwitchboard, long>, IMainSwitchboardRepository
    {
        public EfCoreMainSwitchboardRepository(IDbContextProvider<hmt_energy_csharpOracleDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
    }
}