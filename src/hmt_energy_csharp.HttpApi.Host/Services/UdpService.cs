using hmt_energy_csharp.Entites;
using hmt_energy_csharp.Protos;
using hmt_energy_csharp.VDRs;
using hmt_energy_csharp.VesselInfos;
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
    public class EEUdpServer : UdpServer
    {
        private readonly IConsulService _consulService;
        private readonly IWhiteListAppService _whiteList;
        private readonly IVDRService _vdrService;
        private readonly DiagnosticsConfig _diagnosticsConfig;
        private readonly ILogger<EEUdpServer> _logger;

        public EEUdpServer(IPAddress address, int port, IConsulService consulService, IWhiteListAppService whiteList, IVDRService vdrService, DiagnosticsConfig diagnosticsConfig) : base(address, port)
        {
            _consulService = consulService;
            _whiteList = whiteList;
            _vdrService = vdrService;
            _diagnosticsConfig = diagnosticsConfig;
            _logger = NullLogger<EEUdpServer>.Instance;
        }

        protected override void OnStarted()
        {
            // Start receive datagrams
            ReceiveAsync();
        }

        protected override async void OnReceived(EndPoint endpoint, byte[] buffer, long offset, long size)
        {
            var receiveMsg = Encoding.UTF8.GetString(buffer, (int)offset, (int)size).Replace("\r", "").Replace("\n", "");
            await Console.Out.WriteLineAsync(receiveMsg);
            if (receiveMsg.IsNullOrWhiteSpace())
                return;
            CheckRequest request = new CheckRequest();
            Log.Information("udpReceive:" + receiveMsg);

            try
            {
                request.Ip = ((IPEndPoint)endpoint).Address.ToString();
                if (receiveMsg.IndexOf("$login") > -1 && receiveMsg.IndexOf(";") == receiveMsg.Length - 1)
                {
                    //获取IP、ID
                    request.Id = Convert.ToInt32(receiveMsg.Split(',')[1].Trim(';'));
                    request.Ip = ((System.Net.IPEndPoint)endpoint).Address.ToString();
                    Log.Information("udp验证参数:" + request.ToJson());
                    OkResp response = null;
                    //是否在白名单
                    using (var _channel = await _consulService.GetGrpcChannelAsync("conn-srv"))
                    {
                        var client = new Conn.ConnClient(_channel);
                        response = await client.CheckConnAsync(request);
                    }
                    Log.Information("udp验证结果:" + response.ToJson());
                    //在白名单记录一下
                    //if (response != null && response.Success)
                    //{
                    if (!await _whiteList.IsInWhiteListAsync(request.Id.ToString(), request.Ip))
                    {
                        WhiteListDto dto = new WhiteListDto();
                        dto.TargetId = request.Id.ToString();
                        dto.TargetIp = request.Ip;
                        await _whiteList.CreateAsync(dto);
                    }
                    SendAsync(endpoint, "success");
                    //}
                    //else
                    //{
                    //    SendAsync(endpoint, $"failure:{hmt_energy_csharpDomainErrorCodes.UdpNotRegister}");
                    //}
                }
                else if (receiveMsg[0].ToString().Equals("@") && receiveMsg[receiveMsg.Length - 3].ToString().Equals("*"))
                {
                    var datas = receiveMsg.Split(",");
                    var deviceSN = datas[0].Trim('@');
                    var shipSentenceId = Convert.ToInt32(datas[1]);

                    Log.Information("requestip:" + request.Ip);

                    if (await _whiteList.IsInWhiteListAsync("0", request.Ip))
                    {
                        /*DeviceRequest deviceRequest = new DeviceRequest();
                        deviceRequest.Number = deviceSN;
                        DeviceInfoResp deviceInfoResp = null;
                        using (var _channel = await _consulService.GetGrpcChannelAsync("base-srv"))
                        {
                            var client = new Base.BaseClient(_channel);
                            deviceInfoResp = await client.GetDeviceByNumberAsync(deviceRequest);
                        }
                        if (deviceInfoResp != null && deviceInfoResp.DeviceInfo != null)
                        {*/
                        //处理数据并保存
                        using var activity = _diagnosticsConfig.ActivitySource.StartActivity("ReceiveVesselinfo");
                        activity.SetTag("rdata", receiveMsg);
                        if (!StaticEntity.RealtimeVesselinfos.ContainsKey(deviceSN))
                            StaticEntity.RealtimeVesselinfos.Add(deviceSN, new VesselInfo());
                        var result = await _vdrService.DataAnalysisAsync(receiveMsg, "@", deviceSN);
                        if (result > 0)
                        {
                            SendAsync(endpoint, $"@{shipSentenceId};");
                        }
                        else
                        {
                            SendAsync(endpoint, $"failure:{hmt_energy_csharpDomainErrorCodes.UdpInsertFailure}");
                        }
                        activity.Dispose();
                        /*}
                        else
                        {
                            SendAsync(endpoint, "failure");
                        }*/
                    }
                    else
                    {
                        SendAsync(endpoint, $"failure:{hmt_energy_csharpDomainErrorCodes.UdpOutofWhitelist}");
                    }
                }
                else
                {
                    // Echo the message back to the sender
                    SendAsync(endpoint, $"failure:{hmt_energy_csharpDomainErrorCodes.UdpDataInvalid}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                SendAsync(endpoint, $"failure::{hmt_energy_csharpDomainErrorCodes.UdpProcessError}");
            }
        }

        protected override void OnSent(EndPoint endpoint, long sent)
        {
            // Continue receive datagrams
            ReceiveAsync();
        }

        protected override void OnError(SocketError error)
        {
            _logger.LogError($"Echo UDP server caught an error with code {error}");
        }
    }

    public class UdpService : IHostedService
    {
        public EEUdpServer udpServer { get; set; }
        private readonly IConsulService _consulService;
        private readonly IWhiteListAppService _whiteList;
        private readonly IVDRService _vdrService;
        private readonly DiagnosticsConfig _diagnosticsConfig;

        public UdpService(IConfiguration configuration, IConsulService consulService, IWhiteListAppService whiteList, IVDRService vdrService, DiagnosticsConfig diagnosticsConfig)
        {
            _consulService = consulService;
            _whiteList = whiteList;
            _vdrService = vdrService;
            _diagnosticsConfig = diagnosticsConfig;
            udpServer = new EEUdpServer(IPAddress.Any, Convert.ToInt32(configuration.GetSection("UdpServer")["Port"]), _consulService, _whiteList, _vdrService, _diagnosticsConfig);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            udpServer?.Start();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            udpServer?.Stop();
        }
    }
}