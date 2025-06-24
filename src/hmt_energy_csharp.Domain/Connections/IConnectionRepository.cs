using Volo.Abp.Domain.Repositories;

namespace hmt_energy_csharp.Connections
{
    public interface IConnectionRepository : IRepository<Connection, int>
    {
    }
}