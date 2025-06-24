using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace hmt_energy_csharp.Energy.Shafts
{
    public interface IShaftRepository : IRepository<Shaft, long>
    {
    }
}