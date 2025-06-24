using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace hmt_energy_csharp.Connections
{
    public interface IConnectionService : IApplicationService
    {
        Task<ConnectionDto> GetByHostAsync(string host);
    }
}