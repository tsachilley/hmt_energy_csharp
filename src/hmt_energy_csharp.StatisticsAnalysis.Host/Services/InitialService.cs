using hmt_energy_csharp.Energy.Configs;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace hmt_energy_csharp.Services
{
    public class InitialService : IHostedService
    {
        private readonly ILogger<InitialService> _logger;
        private readonly IConfigService _configService;

        public InitialService(ILogger<InitialService> logger, IConfigService configService)
        {
            _logger = logger;
            _configService = configService;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            StaticEntities.StaticEntities.Configs = await _configService.GetList("{\"IsEnabled\":\"1\"}");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
        }
    }
}