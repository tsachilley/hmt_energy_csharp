using JetBrains.Annotations;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace hmt_energy_csharp.Connections
{
    public class ConnectionService : hmt_energy_csharpAppService, IConnectionService
    {
        private readonly IConnectionRepository _repository;

        public ConnectionService(IConnectionRepository repository)
        {
            _repository = repository;
        }

        public async Task<ConnectionDto> GetByHostAsync([NotNull] string host)
        {
            var entity = await _repository.FirstOrDefaultAsync(predicate: t => t.delete_time.Equals(null) && t.host.Equals(host));
            return ObjectMapper.Map<Connection, ConnectionDto>(entity);
        }
    }
}