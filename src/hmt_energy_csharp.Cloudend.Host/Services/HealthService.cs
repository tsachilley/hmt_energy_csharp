using Grpc.Core;
using hmt_energy_csharp.Protos;
using System.Threading.Tasks;

namespace hmt_energy_csharp.Services
{
    public class HealthService : Health.HealthBase
    {
        public override Task<HealthResponse> Check(HealthRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HealthResponse() { Status = HealthResponse.Types.ServingStatus.Serving });
        }

        public override async Task Watch(HealthRequest request, IServerStreamWriter<HealthResponse> responseStream, ServerCallContext context)
        {
            await responseStream.WriteAsync(new HealthResponse() { Status = HealthResponse.Types.ServingStatus.Serving });
        }
    }
}