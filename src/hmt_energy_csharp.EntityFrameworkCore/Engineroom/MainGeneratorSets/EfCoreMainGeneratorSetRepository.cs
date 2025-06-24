using hmt_energy_csharp.EntityFrameworkCore.Oracle;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace hmt_energy_csharp.Engineroom.MainGeneratorSets
{
    public class EfCoreMainGeneratorSetRepository : EfCoreRepository<hmt_energy_csharpOracleDbContext, MainGeneratorSet, long>, IMainGeneratorSetRepository
    {
        public EfCoreMainGeneratorSetRepository(IDbContextProvider<hmt_energy_csharpOracleDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
    }
}