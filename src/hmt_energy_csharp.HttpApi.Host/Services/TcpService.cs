using hmt_energy_csharp.Protos;
using hmt_energy_csharp.VDRs;
using hmt_energy_csharp.WhiteLists;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NetCoreServer;
using Serilog;
using System;
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
        private readonly IVDRService _vdrService;

        public EETcpSession(TcpServer server, IConsulService consulService, IWhiteListAppService whiteList, IVDRService vdrService) : base(server)
        {
            _logger = NullLogger<EETcpSession>.Instance;
            _consulService = consulService;
            _whiteList = whiteList;
            _vdrService = vdrService;
        }

        protected override void OnConnected()
        {
            _logger.LogError($"Chat TCP session with Id {Id} connected!");

            // Send invite message
            //string message = "Hello from TCP chat! Please send a message or '!' to disconnect the client!";
            //SendAsync(message);
        }

        protected override void OnDisconnected()
        {
            _logger.LogError($"Chat TCP session with Id {Id} disconnected!");
        }

        protected override async void OnReceived(byte[] buffer, long offset, long size)
        {
            var receiveMsg = Encoding.UTF8.GetString(buffer, (int)offset, (int)size).Replace("\r", "").Replace("\n", "");
            CheckRequest request = new CheckRequest();
            Log.Information("tcpReceive:" + receiveMsg);

            try
            {
                request.Ip = ((IPEndPoint)Socket.RemoteEndPoint).Address.ToString();
                if (receiveMsg.IndexOf("$login") > -1 && receiveMsg.IndexOf(";") == receiveMsg.Length - 1)
                {
                    //获取IP、ID
                    request.Id = Convert.ToInt32(receiveMsg.Split(',')[1].Trim(';'));
                    Log.Information("udp验证参数:" + request.ToJson());
                    OkResp response = null;
                    //是否在白名单
                    using (var _channel = await _consulService.GetGrpcChannelAsync("conn-srv"))
                    {
                        var client = new Conn.ConnClient(_channel);
                        response = await client.CheckConnAsync(request);
                    }
                    Log.Information("tcp验证结果:" + response.ToJson());
                    //在白名单记录一下
                    if (response != null && response.Success)
                    {
                        if (!await _whiteList.IsInWhiteListAsync(request.Id.ToString(), request.Ip))
                        {
                            WhiteListDto dto = new WhiteListDto();
                            dto.TargetId = request.Id.ToString();
                            dto.TargetIp = request.Ip;
                            await _whiteList.CreateAsync(dto);
                        }
                        SendAsync("success");
                    }
                    else
                    {
                        SendAsync($"failure:{hmt_energy_csharpDomainErrorCodes.TcpNotRegister}");
                    }
                }
                else if (receiveMsg[0].Equals('@') && receiveMsg[receiveMsg.Length - 3].Equals('*'))
                {
                    var datas = receiveMsg.Split(",");
                    var deviceSN = datas[0].Trim('@');
                    var shipSentenceId = Convert.ToInt32(datas[1]);

                    if (await _whiteList.IsInWhiteListAsync("0", request.Ip))
                    {
                        DeviceRequest deviceRequest = new DeviceRequest();
                        deviceRequest.Number = deviceSN;
                        /*DeviceInfoResp deviceInfoResp = null;
                        using (var _channel = await _consulService.GetGrpcChannelAsync("base-srv"))
                        {
                            var client = new Base.BaseClient(_channel);
                            deviceInfoResp = await client.GetDeviceByNumberAsync(deviceRequest);
                        }
                        if (deviceInfoResp != null && deviceInfoResp.DeviceInfo != null)
                        {*/
                        //处理数据并保存
                        var result = await _vdrService.DataAnalysisAsync(receiveMsg, "@", deviceSN);
                        if (result > 0)
                        {
                            SendAsync($"@{shipSentenceId};");
                        }
                        else
                        {
                            SendAsync($"failure:{hmt_energy_csharpDomainErrorCodes.TcpInsertFailure}");
                        }
                        /*}
                        else
                        {
                            SendAsync("failure");
                        }*/
                    }
                    else
                    {
                        SendAsync($"failure:{hmt_energy_csharpDomainErrorCodes.TcpOutofWhitelist}");
                    }
                }
                else
                {
                    SendAsync($"failure:{hmt_energy_csharpDomainErrorCodes.TcpDataInvalid}");
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
        private readonly IVDRService _vdrService;

        public EETcpServer(IPAddress address, int port, IConsulService consulService, IWhiteListAppService whiteList, IVDRService vdrService) : base(address, port)
        {
            _logger = NullLogger<EETcpServer>.Instance;
            _consulService = consulService;
            _whiteList = whiteList;
            _vdrService = vdrService;
        }

        protected override TcpSession CreateSession()
        { return new EETcpSession(this, _consulService, _whiteList, _vdrService); }

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
        private readonly IVDRService _vdrService;

        public EETcpServer _tcpServer { get; set; }

        public TcpService(IConfiguration configuration, IConsulService consulService, IWhiteListAppService whiteList, IVDRService vdrService)
        {
            _configuration = configuration;
            _consulService = consulService;
            _whiteList = whiteList;
            _vdrService = vdrService;

            _tcpServer = new EETcpServer(IPAddress.Any, Convert.ToInt32(_configuration.GetSection("TcpServer")["Port"]), _consulService, _whiteList, _vdrService);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _tcpServer?.Start();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _tcpServer?.Stop();
        }
    }
}