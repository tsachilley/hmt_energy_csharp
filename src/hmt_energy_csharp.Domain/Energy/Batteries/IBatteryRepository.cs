using Volo.Abp.Domain.Repositories;

namespace hmt_energy_csharp.Energy.Batteries
{
    public interface IBatteryRepository : IRepository<Battery, long>
    {
    }
}