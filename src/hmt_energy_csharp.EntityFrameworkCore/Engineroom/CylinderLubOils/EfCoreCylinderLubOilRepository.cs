using hmt_energy_csharp.EntityFrameworkCore.Oracle;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace hmt_energy_csharp.Engineroom.CylinderLubOils
{
    public class EfCoreCylinderLubOilRepository : EfCoreRepository<hmt_energy_csharpOracleDbContext, CylinderLubOil, long>, ICylinderLubOilRepository
    {
        public EfCoreCylinderLubOilRepository(IDbContextProvider<hmt_energy_csharpOracleDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
    }
}