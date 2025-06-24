using hmt_energy_csharp.EntityFrameworkCore.MySql;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace hmt_energy_csharp.Connections
{
    public class EfCoreConnectionRepository : EfCoreRepository<hmt_energy_csharpDbContext, Connection, int>, IConnectionRepository
    {
        public EfCoreConnectionRepository(IDbContextProvider<hmt_energy_csharpDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
    }
}