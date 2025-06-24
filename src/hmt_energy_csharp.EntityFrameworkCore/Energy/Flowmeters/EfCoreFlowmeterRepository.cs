using hmt_energy_csharp.EntityFrameworkCore.Oracle;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace hmt_energy_csharp.Energy.Flowmeters
{
    public class EfCoreFlowmeterRepository : EfCoreRepository<hmt_energy_csharpOracleDbContext, Flowmeter, long>, IFlowmeterRepository
    {
        public EfCoreFlowmeterRepository(IDbContextProvider<hmt_energy_csharpOracleDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<IList<Flowmeter>> GetRecentlyFmAsync(string sn, DateTime receviceDatetime, string deviceNo, string fuelType)
        {
            StringBuilder sbSql = new StringBuilder();

            sbSql.Append($"SELECT * FROM \"energy_flowmeter\" WHERE \"Id\" = ( SELECT MAX( \"Id\" ) FROM \"energy_flowmeter\" WHERE \"ReceiveDatetime\" < TO_DATE( '{receviceDatetime.ToString("yyyy-MM-dd HH:mm:ss")}', 'YYYY-MM-DD HH24:MI:SS' ) AND \"Number\" = '{sn}' AND \"DeviceNo\" = '{deviceNo}' AND \"FuelType\" = '{fuelType}' )");

            var dbset = await GetDbSetAsync();
            return await dbset.FromSqlRaw(sbSql.ToString()).ToListAsync();
        }
    }
}