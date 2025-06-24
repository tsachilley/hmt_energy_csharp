using hmt_energy_csharp.EntityFrameworkCore.Oracle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace hmt_energy_csharp.Energy.PowerUnits
{
    public class EfCorePowerUnitRepository : EfCoreRepository<hmt_energy_csharpOracleDbContext, PowerUnit, long>, IPowerUnitRepository
    {
        public EfCorePowerUnitRepository(IDbContextProvider<hmt_energy_csharpOracleDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
    }
}