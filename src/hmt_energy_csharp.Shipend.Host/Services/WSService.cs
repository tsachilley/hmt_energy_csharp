using hmt_energy_csharp.HttpRequest;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nacos;
using NetCoreServer;
using Newtonsoft.Json.Linq;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace hmt_energy_csharp.Services
{
    public class WSService : IHostedService
    {
        private readonly ILogger<WSService> _logger;
        private readonly IConfiguration _configuration;
        private readonly ClientWebSocket _clientWS;

        public WSService(ILogger<WSService> logger, IConfiguration configuration, ClientWebSocket clientWS)
        {
            _logger = logger;
            _configuration = configuration;
            _clientWS = clientWS;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _clientWS.ConnectAsync(new Uri(_configuration["websockect:url"]), cancellationToken);
                if (_clientWS != null)
                {
                    _ = Task.Factory.StartNew(async () =>
                    {
                        var buffer = ArrayPool<byte>.Shared.Rent(1024);
                        try
                        {
                            while (_clientWS.State == WebSocketState.Open)
                            {
                                var result = await _clientWS.ReceiveAsync(buffer, CancellationToken.None);
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
            if (_clientWS != null && _clientWS.State == WebSocketState.Open)
            {
                _clientWS.CloseAsync(WebSocketCloseStatus.NormalClosure, null, cancellationToken);
                _clientWS.Abort();
                _clientWS.Dispose();
            }
        }
    }
}