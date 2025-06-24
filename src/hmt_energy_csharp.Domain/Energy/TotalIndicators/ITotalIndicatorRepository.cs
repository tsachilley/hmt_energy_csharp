using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace hmt_energy_csharp.Energy.TotalIndicators
{
    public interface ITotalIndicatorRepository : IRepository<TotalIndicator, long>
    {
    }
}