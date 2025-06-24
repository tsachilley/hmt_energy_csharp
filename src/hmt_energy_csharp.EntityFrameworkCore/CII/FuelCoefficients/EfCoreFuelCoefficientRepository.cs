using hmt_energy_csharp.EntityFrameworkCore.Oracle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace hmt_energy_csharp.CII.FuelCoefficients
{
    public class EfCoreFuelCoefficientRepository : EfCoreRepository<hmt_energy_csharpOracleDbContext, FuelCoefficient, long>, IFuelCoefficientRepository
    {
        public EfCoreFuelCoefficientRepository(IDbContextProvider<hmt_energy_csharpOracleDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
    }
}