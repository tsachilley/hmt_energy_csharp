using System;
using System.Threading.Tasks;
using Volo.Abp.Data;

namespace hmt_energy_csharp
{
    public class hmt_energy_csharpConnectionStringResolver : IConnectionStringResolver
    {
        public string Resolve(string connectionStringName = null)
        {
            throw new NotImplementedException();
        }

        public Task<string> ResolveAsync(string connectionStringName = null)
        {
            throw new NotImplementedException();
        }
    }
}