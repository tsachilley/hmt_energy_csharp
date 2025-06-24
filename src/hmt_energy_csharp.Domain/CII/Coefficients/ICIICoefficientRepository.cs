using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace hmt_energy_csharp.CII.Coefficients
{
    public interface ICIICoefficientRepository : IRepository<CIICoefficient, long>
    {
    }
}