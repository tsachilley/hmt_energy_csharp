using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace hmt_energy_csharp.Energy.Flowmeters
{
    public interface IFlowmeterRepository : IRepository<Flowmeter, long>
    {
        Task<IList<Flowmeter>> GetRecentlyFmAsync(string sn, DateTime receviceDatetime, string deviceNo, string fuelType);
    }
}