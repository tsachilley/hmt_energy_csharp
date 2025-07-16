using hmt_energy_csharp.Energy.Configs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Buffers;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace hmt_energy_csharp.Services
{
    public class InitialService : IHostedService
    {
        private readonly ILogger<InitialService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IConfigService _configService;

        public ClientWebSocket CloudendReceive { get; set; }

        public InitialService(ILogger<InitialService> logger, IConfiguration configuration, IConfigService configService)
        {
            _logger = logger;
            _configuration = configuration;
            _configService = configService;

            CloudendReceive = new ClientWebSocket();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            StaticEntities.StaticEntities.Configs = await _configService.GetList("{\"IsEnabled\":\"1\"}");

            await OpenWSCloudendReceive(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await CloseWSCloudendReceive(cancellationToken);
        }

        private async Task OpenWSCloudendReceive(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("wscloud地址:" + _configuration["wscloud:url"]);
                await CloudendReceive.ConnectAsync(new Uri(_configuration["wscloud:url"]), cancellationToken);
                if (CloudendReceive != null)
                {
                    _ = Task.Factory.StartNew(async () =>
                    {
                        var buffer = ArrayPool<byte>.Shared.Rent(1024);
                        try
                        {
                            while (CloudendReceive.State == WebSocketState.Open)
                            {
                                try
                                {
                                    var result = await CloudendReceive.ReceiveAsync(buffer, cancellationToken);
                                    if (result.MessageType == WebSocketMessageType.Close)
                                    {
                                        throw new WebSocketException(WebSocketError.ConnectionClosedPrematurely, result.CloseStatusDescription);
                                    }
                                    var text = Encoding.UTF8.GetString(buffer.AsSpan(0, result.Count));
                                    var tempJO = JObject.Parse(text);
                                    if (!tempJO.ContainsKey("code") || tempJO["code"].ToString() != "314008")
                                        await Console.Out.WriteLineAsync(text);
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError(ex, "云端通过ws接收实时数据失败。");
                                }
                            }
                        }
                        finally
                        {
                            ArrayPool<byte>.Shared.Return(buffer);
                        }
                    });
                }
                else
                {
                    _logger.LogInformation("WebSocket连接失败。");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task CloseWSCloudendReceive(CancellationToken cancellationToken)
        {
            if (CloudendReceive != null && CloudendReceive.State == WebSocketState.Open)
            {
                CloudendReceive.CloseAsync(WebSocketCloseStatus.NormalClosure, null, cancellationToken);
                CloudendReceive.Abort();
                CloudendReceive.Dispose();
            }
        }
    }
}