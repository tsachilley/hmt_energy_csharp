using hmt_energy_csharp.Energy.Configs;
using hmt_energy_csharp.ProtocolDatas;
using hmt_energy_csharp.Sockets;
using hmt_energy_csharp.WhiteLists;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NetCoreServer;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace hmt_energy_csharp.Services
{
    public class EETcpSession : TcpSession
    {
        private ILogger<EETcpSession> _logger;
        private readonly IConsulService _consulService;
        private readonly IWhiteListAppService _whiteList;
        private readonly IProtocolDataBTService _protocolDataBTService;
        private readonly IConfigService _configService;

        public EETcpSession(TcpServer server, IConsulService consulService, IWhiteListAppService whiteList, IProtocolDataBTService protocolDataBTService, IConfigService configService) : base(server)
        {
            _logger = NullLogger<EETcpSession>.Instance;
            _consulService = consulService;
            _whiteList = whiteList;
            _protocolDataBTService = protocolDataBTService;
            _configService = configService;
        }

        protected override void OnConnected()
        {
        }

        protected override void OnDisconnected()
        {
        }

        protected override async void OnReceived(byte[] buffer, long offset, long size)
        {
            var receiveMsg = Encoding.UTF8.GetString(buffer, (int)offset, (int)size).Replace("\n", "").Replace("\r", "");
            if (receiveMsg.IsNullOrWhiteSpace())
                return;

            try
            {
                if (receiveMsg.Trim() == "$shipinfo start;")
                {
                    StaticEntities.StaticEntities.tcpConfigParam.IsReady = true;
                    SendAsync(receiveMsg);
                }
                else if (receiveMsg.Trim() == "$shipinfo end;")
                {
                    var configs = StaticEntities.StaticEntities.tcpConfigParam.Content.Split(';');
                    for (var i = 0; i < configs.Length; i++)
                    {
                        var deviceInfo = configs[i].Split(',');
                        var dto = StaticEntities.StaticEntities.Configs.FirstOrDefault(t => t.Code == deviceInfo[0]);
                        dto.Interval = Convert.ToInt32(deviceInfo[1]);
                        await _configService.Update((int)dto.Id, dto);
                    }
                    StaticEntities.StaticEntities.Configs.Clear();
                    StaticEntities.StaticEntities.Configs = await _configService.GetList("{\"IsEnabled\":\"1\"}");

                    StaticEntities.StaticEntities.tcpConfigParam.Content = string.Empty;
                    StaticEntities.StaticEntities.tcpConfigParam.IsReady = false;
                    SendAsync(receiveMsg);
                }
                if (StaticEntities.StaticEntities.tcpConfigParam.IsReady)
                {
                    StaticEntities.StaticEntities.tcpConfigParam.Content += receiveMsg;
                    SendAsync(receiveMsg);
                }
                else
                {
                    var clientIp = ((IPEndPoint)Socket.RemoteEndPoint).Address.ToString();
                    if (receiveMsg[0].Equals("|") && receiveMsg[receiveMsg.Length - 3].Equals("*"))
                    {
                        var datas = receiveMsg.Split(",");
                        var deviceSN = datas[0].Trim('|');
                        var shipSentenceId = Convert.ToInt32(datas[1]);

                        if (await _whiteList.IsInWhiteListAsync("0", clientIp))
                        {
                            var result = await _protocolDataBTService.DecodeAsync(deviceSN, receiveMsg, "|", "");
                            if (result.IsSuccess)
                            {
                                SendAsync($"@{shipSentenceId};");
                            }
                            else
                            {
                                SendAsync($"failure:{hmt_energy_csharpDomainErrorCodes.TcpInsertFailure}");
                            }
                        }
                        else
                        {
                            SendAsync($"failure:{hmt_energy_csharpDomainErrorCodes.TcpOutofWhitelist}");
                        }
                    }
                    else if (receiveMsg[0].Equals('!') && receiveMsg.Substring(receiveMsg.Length - 7).Equals("1234567"))
                    {
                        SendAsync($"!1234567;");
                    }
                    else
                    {
                        SendAsync($"failure:{hmt_energy_csharpDomainErrorCodes.TcpDataInvalid}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                SendAsync($"failure:{hmt_energy_csharpDomainErrorCodes.TcpProcessError}");
            }
        }

        protected override void OnError(SocketError error)
        {
            _logger.LogError($"Chat TCP session caught an error with code {error}");
        }
    }

    public class EETcpServer : TcpServer
    {
        private readonly ILogger<EETcpServer> _logger;
        private readonly IConsulService _consulService;
        private readonly IWhiteListAppService _whiteList;
        private readonly IProtocolDataBTService _protocolDataBTService;
        private readonly IConfigService _configService;

        public EETcpServer(IPAddress address, int port, IConsulService consulService, IWhiteListAppService whiteList, IProtocolDataBTService protocolDataBTService, IConfigService configService) : base(address, port)
        {
            _logger = NullLogger<EETcpServer>.Instance;
            _consulService = consulService;
            _whiteList = whiteList;
            _protocolDataBTService = protocolDataBTService;
            _configService = configService;
        }

        protected override TcpSession CreateSession()
        { return new EETcpSession(this, _consulService, _whiteList, _protocolDataBTService, _configService); }

        protected override void OnError(SocketError error)
        {
            _logger.LogError($"Chat TCP server caught an error with code {error}");
        }
    }

    public class TcpService : IHostedService
    {
        private readonly IConfiguration _configuration;
        private readonly IConsulService _consulService;
        private readonly IWhiteListAppService _whiteList;
        private readonly IProtocolDataBTService _protocolDataBTService;
        private readonly TcpClientService _eeTcpClient;
        private readonly IConfigService _configService;

        public EETcpServer _tcpServer { get; set; }

        public TcpService(IConfiguration configuration, IConsulService consulService, IWhiteListAppService whiteList, IProtocolDataBTService protocolDataBTService, TcpClientService eeTcpClient, IConfigService configService)
        {
            _configuration = configuration;
            _consulService = consulService;
            _whiteList = whiteList;
            _protocolDataBTService = protocolDataBTService;
            _eeTcpClient = eeTcpClient;
            _configService = configService;
            _tcpServer = new EETcpServer(IPAddress.Any, Convert.ToInt32(_configuration.GetSection("TcpServer")["Port"]), _consulService, _whiteList, _protocolDataBTService, _configService);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _tcpServer?.Start();

            Task.Factory.StartNew(() =>
           {
               while (true)
               {
                   if (_eeTcpClient?.ConnectAsync() == true)
                       break;
                   Task.Delay(1000 * 30);
               }
           });
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _tcpServer?.Stop();

            _eeTcpClient?.ConnectAsync();
            _eeTcpClient?.DisconnectAndStop();
        }
    }
}