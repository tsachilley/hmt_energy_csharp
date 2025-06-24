using Volo.Abp.Domain.Repositories;

namespace hmt_energy_csharp.Energy.Generators
{
    public interface IGeneratorRepository : IRepository<Generator, long>
    {
    }
}