using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System.Buffers;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace hmt_energy_csharp.Services
{
    public class WSShipendService : IHostedService
    {
        public ClientWebSocket _clientWebSocket { get; set; }

        private readonly ILogger<WSShipendService> _logger;
        private readonly IConfiguration _configuration;

        public WSShipendService(ILogger<WSShipendService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _clientWebSocket = new ClientWebSocket();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _clientWebSocket.ConnectAsync(new Uri(_configuration["wsshipend:url"]), cancellationToken);
                if (_clientWebSocket != null)
                {
                    _ = Task.Factory.StartNew(async () =>
                    {
                        var buffer = ArrayPool<byte>.Shared.Rent(1024);
                        try
                        {
                            while (_clientWebSocket.State == WebSocketState.Open)
                            {
                                var result = await _clientWebSocket.ReceiveAsync(buffer, CancellationToken.None);
                                if (result.MessageType == WebSocketMessageType.Close)
                                {
                                    throw new WebSocketException(WebSocketError.ConnectionClosedPrematurely, result.CloseStatusDescription);
                                }
                                var text = Encoding.UTF8.GetString(buffer.AsSpan(0, result.Count));
                                var tempJO = JObject.Parse(text);
                                if (!tempJO.ContainsKey("code") || tempJO["code"].ToString() != "314008")
                                    await Console.Out.WriteLineAsync(text);
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

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_clientWebSocket != null && _clientWebSocket.State == WebSocketState.Open)
            {
                _clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, null, cancellationToken);
                _clientWebSocket.Abort();
                _clientWebSocket.Dispose();
            }
        }
    }
}