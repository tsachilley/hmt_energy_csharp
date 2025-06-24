using hmt_energy_csharp.EntityFrameworkCore.Oracle;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace hmt_energy_csharp.Engineroom.LubOils
{
    public class EfCoreLubOilRepository : EfCoreRepository<hmt_energy_csharpOracleDbContext, LubOil, long>, ILubOilRepository
    {
        public EfCoreLubOilRepository(IDbContextProvider<hmt_energy_csharpOracleDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
    }
}