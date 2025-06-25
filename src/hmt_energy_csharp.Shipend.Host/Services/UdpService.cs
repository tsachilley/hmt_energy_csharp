using hmt_energy_csharp.Dtos;
using hmt_energy_csharp.Energy.Configs;
using hmt_energy_csharp.Engineroom.AssistantDecisions;
using hmt_energy_csharp.Engineroom.CompositeBoilers;
using hmt_energy_csharp.Engineroom.CompressedAirSupplies;
using hmt_energy_csharp.Engineroom.CoolingFreshWaters;
using hmt_energy_csharp.Engineroom.CoolingSeaWaters;
using hmt_energy_csharp.Engineroom.CoolingWaters;
using hmt_energy_csharp.Engineroom.CylinderLubOils;
using hmt_energy_csharp.Engineroom.ExhaustGases;
using hmt_energy_csharp.Engineroom.FOs;
using hmt_energy_csharp.Engineroom.FOSupplyUnits;
using hmt_energy_csharp.Engineroom.LubOilPurifyings;
using hmt_energy_csharp.Engineroom.LubOils;
using hmt_energy_csharp.Engineroom.MainGeneratorSets;
using hmt_energy_csharp.Engineroom.MainSwitchboards;
using hmt_energy_csharp.Engineroom.MERemoteControls;
using hmt_energy_csharp.Engineroom.Miscellaneouses;
using hmt_energy_csharp.Engineroom.ScavengeAirs;
using hmt_energy_csharp.Engineroom.ShaftClutches;
using hmt_energy_csharp.Entites;
using hmt_energy_csharp.ProtocolDatas;
using hmt_energy_csharp.Protos;
using hmt_energy_csharp.ResponseResults;
using hmt_energy_csharp.VDRs;
using hmt_energy_csharp.WhiteLists;
using Jint;
using Jint.Native;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NetCoreServer;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UdpClient = NetCoreServer.UdpClient;

namespace hmt_energy_csharp.Services
{
    public class UdpService : IHostedService
    {
        public EEUdpServer udpServer { get; set; }
        public TabletUdpClient tabletUdpClient { get; set; }

        private readonly IConfiguration _configuration;
        private readonly IConsulService _consulService;
        private readonly IWhiteListAppService _whiteList;
        private readonly IVDRService _vdrService;
        private readonly DiagnosticsConfig _diagnosticsConfig;
        private readonly IProtocolDataService _protocolDataService;
        private readonly IConfigService _configService;

        public ClientWebSocket ClientWebSocket { get; set; }

        public UdpService(IConfiguration configuration, IConsulService consulService, IWhiteListAppService whiteList, IVDRService vdrService, DiagnosticsConfig diagnosticsConfig, IProtocolDataService protocolDataService, IConfigService configService)
        {
            _configuration = configuration;
            _consulService = consulService;
            _whiteList = whiteList;
            _vdrService = vdrService;
            _diagnosticsConfig = diagnosticsConfig;
            _protocolDataService = protocolDataService;
            _configService = configService;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            udpServer = new EEUdpServer(IPAddress.Any, Convert.ToInt32(_configuration.GetSection("UdpServer")["Port"]), _consulService, _whiteList, _vdrService, _diagnosticsConfig, _protocolDataService, _configService, _configuration);
            udpServer?.Start();
            StaticEntities.StaticEntities.Configs = await _configService.GetList("{\"IsEnabled\":\"1\"}");

            #region 启动平板UDP客户端

            tabletUdpClient = new TabletUdpClient(_configuration.GetSection("UdpTabletClient")["Address"], Convert.ToInt32(_configuration.GetSection("UdpTabletClient")["Port"]));
            Task.Run(async () =>
            {
                while (!tabletUdpClient._stop)
                {
                    try
                    {
                        if (!tabletUdpClient.IsConnected)
                        {
                            tabletUdpClient.Connect();
                            Log.Information("udptablet重新连接");
                        }
                        else
                            tabletUdpClient.Send("$$$hellotablet$$$");
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, MethodBase.GetCurrentMethod().DeclaringType.Namespace + "_" + MethodBase.GetCurrentMethod().Name);
                    }
                    await Task.Delay(30 * 1000);
                }
            });

            //开始发送机舱数据
            Task.Run(async () =>
            {
                var SN = "SAD1";
                while (!tabletUdpClient._stop)
                {
                    try
                    {
                        if (StaticEntities.ShowEntities.Vessels.Any(t => t.SN == SN))
                        {
                            tabletUdpClient.Send((new { CompositeBoiler = StaticEntities.ShowEntities.CompositeBoilers.FirstOrDefault(t => t.Number == SN).CompositeBoilerDtos, }).ToJson());
                            await Task.Delay(100);
                            tabletUdpClient.Send((new { CompressedAirSupply = StaticEntities.ShowEntities.CompressedAirSupplies.FirstOrDefault(t => t.Number == SN).CompressedAirSupplyDtos, }).ToJson());
                            await Task.Delay(100);
                            tabletUdpClient.Send((new { CoolingFreshWater = StaticEntities.ShowEntities.CoolingFreshWaters.FirstOrDefault(t => t.Number == SN).CoolingFreshWaterDtos, }).ToJson());
                            await Task.Delay(100);
                            tabletUdpClient.Send((new { CoolingSeaWater = StaticEntities.ShowEntities.CoolingSeaWaters.FirstOrDefault(t => t.Number == SN).CoolingSeaWaterDtos, }).ToJson());
                            await Task.Delay(100);
                            tabletUdpClient.Send((new { CoolingWater = StaticEntities.ShowEntities.CoolingWaters.FirstOrDefault(t => t.Number == SN).CoolingWaterDtos, }).ToJson());
                            await Task.Delay(100);
                            tabletUdpClient.Send((new { CylinderLubOil = StaticEntities.ShowEntities.CylinderLubOils.FirstOrDefault(t => t.Number == SN).CylinderLubOilDtos, }).ToJson());
                            await Task.Delay(100);
                            tabletUdpClient.Send((new { ExhaustGas = StaticEntities.ShowEntities.ExhaustGases.FirstOrDefault(t => t.Number == SN).ExhaustGasDtos, }).ToJson());
                            await Task.Delay(100);
                            tabletUdpClient.Send((new { FO = StaticEntities.ShowEntities.FOs.FirstOrDefault(t => t.Number == SN).FODtos, }).ToJson());
                            await Task.Delay(100);
                            tabletUdpClient.Send((new { FOSupplyUnit = StaticEntities.ShowEntities.FOSupplyUnits.FirstOrDefault(t => t.Number == SN).FOSupplyUnitDtos, }).ToJson());
                            await Task.Delay(100);
                            tabletUdpClient.Send((new { LubOilPurifying = StaticEntities.ShowEntities.LubOilPurifyings.FirstOrDefault(t => t.Number == SN).LubOilPurifyingDtos, }).ToJson());
                            await Task.Delay(100);
                            tabletUdpClient.Send((new { LubOil = StaticEntities.ShowEntities.LubOils.FirstOrDefault(t => t.Number == SN).LubOilDtos, }).ToJson());
                            await Task.Delay(100);
                            tabletUdpClient.Send((new { MainGeneratorSet = StaticEntities.ShowEntities.MainGeneratorSets.FirstOrDefault(t => t.Number == SN).MainGeneratorSetDtos, }).ToJson());
                            await Task.Delay(100);
                            tabletUdpClient.Send((new { MainSwitchboard = StaticEntities.ShowEntities.MainSwitchboards.FirstOrDefault(t => t.Number == SN).MainSwitchboardDtos, }).ToJson());
                            await Task.Delay(100);
                            tabletUdpClient.Send((new { MERemoteControl = StaticEntities.ShowEntities.MERemoteControls.FirstOrDefault(t => t.Number == SN).MERemoteControlDtos, }).ToJson());
                            await Task.Delay(100);
                            tabletUdpClient.Send((new { Miscellaneous = StaticEntities.ShowEntities.Miscellaneouses.FirstOrDefault(t => t.Number == SN).MiscellaneousDtos, }).ToJson());
                            await Task.Delay(100);
                            tabletUdpClient.Send((new { ScavengeAir = StaticEntities.ShowEntities.ScavengeAirs.FirstOrDefault(t => t.Number == SN).ScavengeAirDtos, }).ToJson());
                            await Task.Delay(100);
                            tabletUdpClient.Send((new { ShaftClutch = StaticEntities.ShowEntities.ShaftClutchs.FirstOrDefault(t => t.Number == SN).ShaftClutchDtos, }).ToJson());
                        }
                        else
                        {
                            tabletUdpClient.Send((new { CompositeBoiler = new List<CompositeBoilerDto>() { new CompositeBoilerDto() }, }).ToJson());
                            await Task.Delay(100);
                            tabletUdpClient.Send((new { CompressedAirSupply = new List<CompressedAirSupplyDto>() { new CompressedAirSupplyDto() }, }).ToJson());
                            await Task.Delay(100);
                            tabletUdpClient.Send((new { CoolingFreshWater = new List<CoolingFreshWaterDto>() { new CoolingFreshWaterDto() }, }).ToJson());
                            await Task.Delay(100);
                            tabletUdpClient.Send((new { CoolingSeaWater = new List<CoolingSeaWaterDto>() { new CoolingSeaWaterDto() }, }).ToJson());
                            await Task.Delay(100);
                            tabletUdpClient.Send((new { CoolingWater = new List<CoolingWaterDto>() { new CoolingWaterDto() }, }).ToJson());
                            await Task.Delay(100);
                            tabletUdpClient.Send((new { CylinderLubOil = new List<CylinderLubOilDto>() { new CylinderLubOilDto() }, }).ToJson());
                            await Task.Delay(100);
                            tabletUdpClient.Send((new { ExhaustGas = new List<ExhaustGasDto>() { new ExhaustGasDto() }, }).ToJson());
                            await Task.Delay(100);
                            tabletUdpClient.Send((new { FO = new List<FODto>() { new FODto() }, }).ToJson());
                            await Task.Delay(100);
                            tabletUdpClient.Send((new { FOSupplyUnit = new List<FOSupplyUnitDto>() { new FOSupplyUnitDto() }, }).ToJson());
                            await Task.Delay(100);
                            tabletUdpClient.Send((new { LubOilPurifying = new List<LubOilPurifyingDto>() { new LubOilPurifyingDto() }, }).ToJson());
                            await Task.Delay(100);
                            tabletUdpClient.Send((new { LubOil = new List<LubOilDto>() { new LubOilDto() }, }).ToJson());
                            await Task.Delay(100);
                            tabletUdpClient.Send((new { MainGeneratorSet = new List<MainGeneratorSetDto>() { new MainGeneratorSetDto(), new MainGeneratorSetDto(), new MainGeneratorSetDto() }, }).ToJson());
                            await Task.Delay(100);
                            tabletUdpClient.Send((new { MainSwitchboard = new List<MainSwitchboardDto>() { new MainSwitchboardDto(), new MainSwitchboardDto(), new MainSwitchboardDto() }, }).ToJson());
                            await Task.Delay(100);
                            tabletUdpClient.Send((new { MERemoteControl = new List<MERemoteControlDto>() { new MERemoteControlDto() }, }).ToJson());
                            await Task.Delay(100);
                            tabletUdpClient.Send((new { Miscellaneous = new List<MiscellaneousDto>() { new MiscellaneousDto() }, }).ToJson());
                            await Task.Delay(100);
                            tabletUdpClient.Send((new { ScavengeAir = new List<ScavengeAirDto>() { new ScavengeAirDto() }, }).ToJson());
                            await Task.Delay(100);
                            tabletUdpClient.Send((new { ShaftClutch = new List<ShaftClutchDto>() { new ShaftClutchDto() }, }).ToJson());
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, MethodBase.GetCurrentMethod().DeclaringType.Namespace + "_" + MethodBase.GetCurrentMethod().Name);
                    }
                    await Task.Delay(10 * 1000);
                }
            });

            #endregion 启动平板UDP客户端

            Log.Information("HostStart启动");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            udpServer?.Stop();
            tabletUdpClient?.DisconnectAndStop();
        }
    }

    public class EEUdpServer : UdpServer
    {
        private readonly IConsulService _consulService;
        private readonly IWhiteListAppService _whiteList;
        private readonly IVDRService _vdrService;
        private readonly DiagnosticsConfig _diagnosticsConfig;
        private readonly IProtocolDataService _protocolDataService;
        private readonly IConfigService _configService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<EEUdpServer> _logger;

        public ClientWebSocket _clientWS { get; set; }
        public ClientWebSocket ClientWebSocket { get; set; }

        public JObject JECA { get; set; }
        public string TurfJs { get; set; }
        public Engine JintEngine { get; set; }

        public int shipId { get; set; } = -1;

        public EEUdpServer(IPAddress address, int port, IConsulService consulService, IWhiteListAppService whiteList, IVDRService vdrService, DiagnosticsConfig diagnosticsConfig, IProtocolDataService protocolDataService, IConfigService configService, IConfiguration configuration) : base(address, port)
        {
            _consulService = consulService;
            _whiteList = whiteList;
            _vdrService = vdrService;
            _diagnosticsConfig = diagnosticsConfig;
            _protocolDataService = protocolDataService;
            _configService = configService;
            _configuration = configuration;
            _logger = NullLogger<EEUdpServer>.Instance;
        }

        protected override void OnStarted()
        {
            Log.Information($"{DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")} UDP服务器信息=>{Endpoint}");

            // Start receive datagrams
            ReceiveAsync();

            // 打开报警用ws
            try
            {
                _clientWS = new ClientWebSocket();
                _clientWS.ConnectAsync(new Uri(_configuration["websockect:url"]), CancellationToken.None).Wait();
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
                                /* 判断是否接通websockect
                                 * var tempJO = JObject.Parse(text);
                                if (!tempJO.ContainsKey("code") || tempJO["code"].ToString() != "314008")
                                    await Console.Out.WriteLineAsync(text);*/
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
                    Log.Information("WebSocket连接失败。");
                }
            }
            catch (Exception)
            {
                throw;
            }

            // 打开实时数据上传用ws
            try
            {
                ClientWebSocket = new ClientWebSocket();
                ClientWebSocket.ConnectAsync(new Uri(_configuration["wsshipend:url"]), CancellationToken.None).Wait();
                if (ClientWebSocket != null)
                {
                    _ = Task.Factory.StartNew(async () =>
                    {
                        var buffer = ArrayPool<byte>.Shared.Rent(1024);
                        try
                        {
                            while (ClientWebSocket.State == WebSocketState.Open)
                            {
                                var result = await ClientWebSocket.ReceiveAsync(buffer, CancellationToken.None);
                                if (result.MessageType == WebSocketMessageType.Close)
                                {
                                    throw new WebSocketException(WebSocketError.ConnectionClosedPrematurely, result.CloseStatusDescription);
                                }
                                var text = Encoding.UTF8.GetString(buffer.AsSpan(0, result.Count));
                                /*//判断是否接通websockect
                                var tempJO = JObject.Parse(text);
                                if (!tempJO.ContainsKey("code") || tempJO["code"].ToString() != "314008")
                                {
                                    await Console.Out.WriteLineAsync(text);
                                }*/
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
                    Log.Information("WebSocket连接失败。");
                }
            }
            catch (Exception)
            {
                throw;
            }

            //初始化eca及相关功能
            InitECA();

            // 打开中央处理单元掉线监测
            Task.Factory.StartNew(async () =>
            {
                var number = "SAD1";
                while (true)
                {
                    if (StaticEntities.StaticEntities.MonitoredDevices.Any(t => t.Number == number))
                    {
                        Devices.MonitoredDevice mds = StaticEntities.StaticEntities.MonitoredDevices.FirstOrDefault(t => t.Number == number);
                        if (mds.Devices.Count > 0)
                        {
                            try
                            {
                                var dtUtc = mds.Devices.MaxBy(t => t.Value).Value;

                                if (dtUtc.AddSeconds(30) < DateTime.UtcNow)
                                {
                                    DateTime CurDt = DateTime.Now;

                                    var logContents = new List<LogBook>();
                                    var logBook = new LogBook();
                                    LogInfo logInfo = new LogInfo();
                                    logInfo.SerialNumber = number;
                                    logBook.time = logInfo.RecordTime = CurDt.ToString("yyyy-MM-dd HH:mm:ss");
                                    logBook.content = logInfo.Content = $"中央处理单元信息丢失。";
                                    logBook.type = logInfo.LogType = "error";
                                    logInfo.Creater = "auto";

                                    var logContentsEn = new List<LogBook>();
                                    var logBookEn = new LogBook();
                                    LogInfo logInfoEn = new LogInfo();
                                    logInfoEn.SerialNumber = number;
                                    logBookEn.time = logInfoEn.RecordTime = CurDt.ToString("yyyy-MM-dd HH:mm:ss");
                                    logBookEn.content = logInfoEn.Content = $"Lost MCU Signal.";
                                    logBookEn.type = logInfoEn.LogType = "error";
                                    logInfoEn.Creater = "auto";

                                    using var _channel = await _consulService.GetGrpcChannelAsync("base-srv");
                                    var client = new Base.BaseClient(_channel);
                                    var deviceResponse = await client.GetDeviceByNumberAsync(new DeviceRequest { Number = number });
                                    shipId = deviceResponse.DeviceInfo.ShipId;
                                    //await client.CreateLogAsync(logInfo);
                                    await client.CreateLogAsync(logInfoEn);
                                    logContents.Add(logBook);
                                    logContentsEn.Add(logBookEn);
                                    await _clientWS.SendAsync(Encoding.UTF8.GetBytes((new { category = "eems", content = logContents, shipId = shipId, level = "曹" }).ToJson() + "$warningInfoEn$" + (new { category = "eems", content = logContentsEn, shipId = shipId, level = "曹" }).ToJson()), WebSocketMessageType.Text, true, CancellationToken.None);
                                }
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                    await Task.Delay(1000 * 30);
                }
            });
            Log.Information("UDP启动");
        }

        //初始化eca、js 启动eca预警任务
        private void InitECA()
        {
            try
            {
                JECA = File.ReadAllText("Geo/ECA.json").ToJObject();
                TurfJs = File.ReadAllText("Geo/turfminjs");
                JintEngine = new Engine().SetValue("log", new Action<object>(Console.WriteLine)).Execute(TurfJs);
                JintEngine.Execute(@"
                    function getECAInfo(point, geometry, geometryType, featureName) {
                        var turfGeometry;
                        var turfLine;
                        var currentPoint = turf.point(point);
                        var closestPoint;
                        var closestDistance;
                        var result;
                        if (geometryType == 'Polygon') {
                            turfGeometry = turf.polygon(geometry);
                        } else if (geometryType == 'MultiPolygon') {
                            turfGeometry = turf.multiPolygon(geometry);
                        }
                        var inPolygon = turf.booleanPointInPolygon(currentPoint, turfGeometry, { ignoreBoundary: false });
                        if (!inPolygon) {
                            turfLine = turf.polygonToLine(turfGeometry);
                            turfLine = JSON.parse(JSON.stringify(turfLine));
                            closestPoint = turf.nearestPointOnLine(turfLine, currentPoint).geometry.coordinates;
                            closestDistance = turf.distance(currentPoint, closestPoint) / 1.852;
                            result = { name: featureName, distance: closestDistance };
                        } else {
                            result = { name: featureName, distance: 0 };
                        }
                        return result;
                    }
                ");

                Task.Run(async () =>
                {
                    while (true)
                    {
                        await Task.Delay(1000 * 60);

                        var vessel = StaticEntities.ShowEntities.Vessels.FirstOrDefault(t => t.SN == "SAD1");
                        if (vessel == null)
                            continue;

                        if (shipId > 0)
                            try
                            {
                                JsValue fResult = null;
                                if (JECA.ContainsKey("features"))
                                {
                                    foreach (var feature in JECA["features"].Children())
                                    {
                                        if (((JObject)feature).ContainsKey("geometry") && ((JObject)feature).ContainsKey("properties"))
                                        {
                                            if (((JObject)feature["geometry"]).ContainsKey("coordinates") && ((JObject)feature["geometry"]).ContainsKey("type") && ((JObject)feature["properties"]).ContainsKey("name"))
                                            {
                                                var featurename = feature["properties"]["name"].ToString();
                                                var geometrytype = feature["geometry"]["type"].ToString();
                                                var coordinates = feature["geometry"]["coordinates"];

                                                double[] position = new double[2] { Convert.ToDouble(vessel.Longitude), Convert.ToDouble(vessel.Latitude) };
                                                //测试用
                                                //double[] position = new double[2] { Convert.ToDouble(122.101395130671), Convert.ToDouble(32.1146169209686) };
                                                var result = JintEngine.Invoke("getECAInfo", position, coordinates, geometrytype, featurename);
                                                if (result.Get("distance").ToString() != "undefined")
                                                {
                                                    if (Convert.ToDouble(result.Get("distance")) == 0)
                                                    {
                                                        fResult = result;
                                                        break;
                                                    }
                                                    else
                                                    {
                                                        if (fResult == null)
                                                        {
                                                            fResult = result;
                                                        }
                                                        else
                                                        {
                                                            if (Convert.ToDouble(fResult.Get("distance")) > Convert.ToDouble(result.Get("distance")))
                                                                fResult = result;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    //通过ws发送eca信息
                                    if (fResult != null && Convert.ToDouble(fResult.Get("distance")) < 20)
                                        try
                                        {
                                            string strDt = DateTimeOffset.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                                            ResponseResult responseResult = new ResponseResult
                                            {
                                                LogContents = new List<LogBook> {new LogBook
                                            {
                                                time = strDt,
                                                content = "距离" + fResult.Get("name") + "区域还剩" + fResult.Get("distance") + "海里"
                                            } },
                                                LogContentsEn = new List<LogBook> {new LogBook
                                            {
                                                time = strDt,
                                                content = "There are still " + fResult.Get("distance") + " nautical miles left from Area " + fResult.Get("name")
                                            } }
                                            };
                                            await _clientWS.SendAsync(Encoding.UTF8.GetBytes((new { category = "eca", content = responseResult.LogContents, shipId = shipId, level = "info" }).ToJson() + "$warningInfoEn$" + (new { category = "eca", content = responseResult.LogContentsEn, shipId = shipId, level = "info" }).ToJson()), WebSocketMessageType.Text, true, CancellationToken.None);
                                            //测试用
                                            //await _clientWS.SendAsync(Encoding.UTF8.GetBytes((new { category = "eca", content = responseResult.LogContents, shipId = 24, level = "info" }).ToJson() + "$warningInfoEn$" + (new { category = "eca", content = responseResult.LogContentsEn, shipId = 24, level = "info" }).ToJson()), WebSocketMessageType.Text, true, CancellationToken.None);
                                        }
                                        catch (Exception ex)
                                        {
                                            _logger.LogError(ex, "{Namespace}_{MethodName}", MethodBase.GetCurrentMethod()?.DeclaringType?.Namespace, MethodBase.GetCurrentMethod()?.Name);
                                        }
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "{Namespace}_{MethodName}", MethodBase.GetCurrentMethod()?.DeclaringType?.Namespace, MethodBase.GetCurrentMethod()?.Name);
                            }
                    }
                }, CancellationToken.None);
            }
            catch (Exception ex)
            {
                Console.WriteLine("初始化eca及相关js失败，系统关闭，请检查相关文件···");
                _logger.LogError(ex, "初始化eca及相关js失败，系统关闭，请检查相关文件···");
                throw;
            }
        }

        protected override async void OnReceived(EndPoint endpoint, byte[] buffer, long offset, long size)
        {
            Log.Information($"接收信息参数=>{endpoint.ToString()} {offset} {size}");
            var receiveMsg = Encoding.UTF8.GetString(buffer, (int)offset, (int)size).Replace("\r", "").Replace("\n", "");
            if (receiveMsg.IsNullOrWhiteSpace())
                return;

            try
            {
                Log.Information($"{DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")} 接收实时数据=》{receiveMsg}");
                var clientIp = ((IPEndPoint)endpoint).Address.ToString();
                if (receiveMsg[0].Equals('|') && receiveMsg[receiveMsg.Length - 3].Equals('*'))
                {
                    Log.Information($"航行信息.");
                    var datas = receiveMsg.Split(",");
                    var number = datas[0].Trim('|');    //采集系统设备序列号
                    var collectId = Convert.ToInt32(datas[1]);  //语句id
                    var shipinfo = new BaseShipInfo();

                    using (var _channel = await _consulService.GetGrpcChannelAsync("base-srv"))
                    {
                        try
                        {
                            var client = new Base.BaseClient(_channel);
                            var deviceResponse = await client.GetDeviceByNumberAsync(new DeviceRequest { Number = number });
                            shipId = deviceResponse.DeviceInfo.ShipId;
                            var shipResponse = await client.GetShipByIdAsync(new IdRequest { Id = deviceResponse.DeviceInfo.ShipId });
                            shipinfo.ShipType = shipResponse.ShipInfo.TypeName;
                            shipinfo.DWT = shipResponse.ShipInfo.Dwt;
                            shipinfo.GT = shipResponse.ShipInfo.Gt;
                            shipinfo.Pitch = shipResponse.ShipInfo.Pitch;

                            var responseCurrentFuelType = await client.GetShipFuelSwitchRecordListAsync(new ShipFuelSwitchRecordListReq { ShipId = shipId, PageInfo = new PageInfo { Pn = 1, Ps = 1 } });
                            var cftList = responseCurrentFuelType.List;
                            if (cftList.Count > 0)
                                shipinfo.CurrentFuelType = cftList[0].PresentFuelType;
                            else
                                shipinfo.CurrentFuelType = "HFO";
                        }
                        catch (Exception ex)
                        {
                            shipinfo.CurrentFuelType = "HFO";
                            Log.Error(ex, MethodBase.GetCurrentMethod().DeclaringType.Namespace + "_" + MethodBase.GetCurrentMethod().Name);
                        }
                        //shipinfo.CurrentFuelType = "HFO";
                    }

                    //测试用
                    /*shipId = 24;
                    shipinfo.ShipType = "bulk carrier/散货船";
                    shipinfo.DWT = 115599.6f;
                    shipinfo.GT = 10000;
                    shipinfo.Pitch = 9321.321;
                    shipinfo.CurrentFuelType = "HFO";*/

                    var result = await _protocolDataService.DecodeAsync(number, receiveMsg, "|", "", shipinfo);
                    if (result.IsSuccess)
                    {
                        SendAsync(endpoint, $"@{collectId};");
                        Log.Information($"{collectId} 接收成功。");
                    }
                    else
                    {
                        SendAsync(endpoint, $"failure:{hmt_energy_csharpDomainErrorCodes.UdpInsertFailure}");
                        Log.Error($"{collectId} 接收失败。");
                    }

                    if (result.LogContents.Count > 0)
                    {
                        using (var _channel = await _consulService.GetGrpcChannelAsync("base-srv"))
                        {
                            var client = new Base.BaseClient(_channel);
                            var level = "曹";
                            foreach (var logEntity in result.LogContentsEn)
                            {
                                LogInfo logInfo = new LogInfo();
                                logInfo.SerialNumber = number;
                                logInfo.RecordTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                logInfo.Content = logEntity.content;
                                logInfo.LogType = logEntity.type;
                                logInfo.Creater = "auto";
                                await client.CreateLogAsync(logInfo);

                                if (logEntity.type == "alert")
                                    level = "呸";
                            }

                            await _clientWS.SendAsync(Encoding.UTF8.GetBytes((new { category = "eems", content = result.LogContents, shipId = shipId, level = level }).ToJson() + "$warningInfoEn$" + (new { category = "eems", content = result.LogContentsEn, shipId = shipId, level = level }).ToJson()), WebSocketMessageType.Text, true, CancellationToken.None);
                        }
                    }

                    #region 实时数据通过ws上传云端

                    try
                    {
                        if (ClientWebSocket.State == WebSocketState.Open)
                        {
                            var rtInfos = new
                            {
                                vessel = StaticEntities.ShowEntities.Vessels.FirstOrDefault(t => t.SN == number),
                                flowmeter = StaticEntities.ShowEntities.Flowmeters.FirstOrDefault(t => t.Number == number),
                                battery = StaticEntities.ShowEntities.Batteries.FirstOrDefault(t => t.Number == number),
                                generator = StaticEntities.ShowEntities.Generators.FirstOrDefault(t => t.Number == number),
                                liquidLevel = StaticEntities.ShowEntities.LiquidLevels.FirstOrDefault(t => t.Number == number),
                                supplyUnit = StaticEntities.ShowEntities.SupplyUnits.FirstOrDefault(t => t.Number == number),
                                shaft = StaticEntities.ShowEntities.Shafts.FirstOrDefault(t => t.Number == number),
                                sternSealing = StaticEntities.ShowEntities.SternSealings.FirstOrDefault(t => t.Number == number),
                                powerUnit = StaticEntities.ShowEntities.PowerUnits.FirstOrDefault(t => t.Number == number),
                                totalIndicator = StaticEntities.ShowEntities.TotalIndicators.FirstOrDefault(t => t.Number == number),
                                prediction = StaticEntities.ShowEntities.Predictions.FirstOrDefault(t => t.Number == number),
                                AssistantDecisions = StaticEntities.ShowEntities.AssistantDecisions.FirstOrDefault(t => t.Number == number),
                                CompositeBoilers = StaticEntities.ShowEntities.CompositeBoilers.FirstOrDefault(t => t.Number == number),
                                CompressedAirSupplies = StaticEntities.ShowEntities.CompressedAirSupplies.FirstOrDefault(t => t.Number == number),
                                CoolingFreshWaters = StaticEntities.ShowEntities.CoolingFreshWaters.FirstOrDefault(t => t.Number == number),
                                CoolingSeaWaters = StaticEntities.ShowEntities.CoolingSeaWaters.FirstOrDefault(t => t.Number == number),
                                CoolingWaters = StaticEntities.ShowEntities.CoolingWaters.FirstOrDefault(t => t.Number == number),
                                CylinderLubOils = StaticEntities.ShowEntities.CylinderLubOils.FirstOrDefault(t => t.Number == number),
                                ExhaustGases = StaticEntities.ShowEntities.ExhaustGases.FirstOrDefault(t => t.Number == number),
                                FOs = StaticEntities.ShowEntities.FOs.FirstOrDefault(t => t.Number == number),
                                FOSupplyUnits = StaticEntities.ShowEntities.FOSupplyUnits.FirstOrDefault(t => t.Number == number),
                                LubOilPurifyings = StaticEntities.ShowEntities.LubOilPurifyings.FirstOrDefault(t => t.Number == number),
                                LubOils = StaticEntities.ShowEntities.LubOils.FirstOrDefault(t => t.Number == number),
                                MainGeneratorSets = StaticEntities.ShowEntities.MainGeneratorSets.FirstOrDefault(t => t.Number == number),
                                MainSwitchboards = StaticEntities.ShowEntities.MainSwitchboards.FirstOrDefault(t => t.Number == number),
                                MERemoteControls = StaticEntities.ShowEntities.MERemoteControls.FirstOrDefault(t => t.Number == number),
                                Miscellaneouses = StaticEntities.ShowEntities.Miscellaneouses.FirstOrDefault(t => t.Number == number),
                                ScavengeAirs = StaticEntities.ShowEntities.ScavengeAirs.FirstOrDefault(t => t.Number == number),
                                ShaftClutchs = StaticEntities.ShowEntities.ShaftClutchs.FirstOrDefault(t => t.Number == number),
                            };
                            await ClientWebSocket.SendAsync(Encoding.UTF8.GetBytes(rtInfos.ToJson()), WebSocketMessageType.Text, true, CancellationToken.None);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "实时数据通过ws发送云端失败。");
                    }

                    #endregion 实时数据通过ws上传云端
                }
                else if (receiveMsg[0].Equals('|') && receiveMsg[receiveMsg.Length - 4].Equals('*'))
                {
                    Log.Information($"状态信息.");
                    var number = string.Empty;
                    var infoType = string.Empty;
                    try
                    {
                        var datas = receiveMsg.Split(",");
                        number = datas[0].Trim('|');    //采集系统设备序列号
                        infoType = datas[1];
                        if (infoType.Split('@').Length > 1)
                            infoType = infoType.Split('@')[0];
                        if (infoType.IndexOf("ERR") == 0)
                        {
                            if (receiveMsg.Split("@").Length > 1)
                            {
                                var strDevices = receiveMsg.Split("@")[1];
                                strDevices = strDevices.Substring(0, strDevices.Length - 4);
                                var lstDevice = strDevices.Split('$');
                                if (Convert.ToInt32(infoType.Replace("ERR", "")) != lstDevice.Length)
                                {
                                    return;
                                }

                                using var _channel = await _consulService.GetGrpcChannelAsync("base-srv");
                                var client = new Base.BaseClient(_channel);
                                var deviceResponse = await client.GetDeviceByNumberAsync(new DeviceRequest { Number = number });
                                shipId = deviceResponse.DeviceInfo.ShipId;
                                var logContents = new List<LogBook>();
                                var logContentsEn = new List<LogBook>();

                                foreach (var device in lstDevice)
                                {
                                    try
                                    {
                                        if (device.Split(',').Length > 1)
                                        {
                                            var code = device.Split(',')[0];
                                            var status = device.Split(',')[1];
                                            if (status == "0")
                                            {
                                                if (StaticEntities.StaticEntities.Configs.Any(t => t.Number == number && t.Code == code && t.IsDevice == 1))
                                                {
                                                    var dto = StaticEntities.StaticEntities.Configs.FirstOrDefault(t => t.Number == number && t.Code == code && t.IsDevice == 1);
                                                    var curDt = DateTime.Now;

                                                    var logBook = new LogBook();
                                                    LogInfo logInfo = new LogInfo();
                                                    logInfo.SerialNumber = number;
                                                    logBook.time = logInfo.RecordTime = curDt.ToString("yyyy-MM-dd HH:mm:ss");
                                                    logBook.content = logInfo.Content = $"{dto.Name}位置信息丢失。";
                                                    logBook.type = logInfo.LogType = "error";
                                                    logInfo.Creater = "auto";

                                                    //await client.CreateLogAsync(logInfo);
                                                    logContents.Add(logBook);

                                                    var logBookEn = new LogBook();
                                                    LogInfo logInfoEn = new LogInfo();
                                                    logInfoEn.SerialNumber = number;
                                                    logBookEn.time = logInfoEn.RecordTime = curDt.ToString("yyyy-MM-dd HH:mm:ss");
                                                    logBookEn.content = logInfoEn.Content = $"Lost {dto.Name} signal.";
                                                    logBookEn.type = logInfoEn.LogType = "error";
                                                    logInfoEn.Creater = "auto";

                                                    await client.CreateLogAsync(logInfoEn);
                                                    logContentsEn.Add(logBookEn);
                                                }
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Log.Error($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} 状态信息日志信息错误.");
                                        continue;
                                    }
                                }
                                await _clientWS.SendAsync(Encoding.UTF8.GetBytes((new { category = "eems", content = logContents, shipId = shipId, level = "曹" }).ToJson() + "$warningInfoEn$" + (new { category = "eems", content = logContentsEn, shipId = shipId, level = "曹" }).ToJson()), WebSocketMessageType.Text, true, CancellationToken.None);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, MethodBase.GetCurrentMethod().DeclaringType.Namespace + "_" + MethodBase.GetCurrentMethod().Name);
                    }
                    finally
                    {
                        SendAsync(endpoint, $"!{number};");
                        Log.Information($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {number}成功.");
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
                Log.Error(ex.Message);
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

        protected override void OnStopping()
        {
            if (_clientWS != null && _clientWS.State == WebSocketState.Open)
            {
                _clientWS.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
                _clientWS.Abort();
                _clientWS.Dispose();
            }

            if (ClientWebSocket != null && ClientWebSocket.State == WebSocketState.Open)
            {
                ClientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
                ClientWebSocket.Abort();
                ClientWebSocket.Dispose();
            }
        }
    }

    public class TabletUdpClient : UdpClient
    {
        public bool _stop;

        public TabletUdpClient(string address, int port) : base(address, port)
        {
        }

        public void DisconnectAndStop()
        {
            _stop = true;
            Disconnect();
            while (IsConnected)
                Thread.Yield();
        }

        protected override void OnConnected()
        {
            Console.WriteLine($"Echo UDP client connected a new session with Id {Id}");

            // Start receive datagrams
            ReceiveAsync();
        }

        protected override void OnDisconnected()
        {
            Console.WriteLine($"Echo UDP client disconnected a session with Id {Id}");

            // Wait for a while...
            Thread.Sleep(1000);

            // Try to connect again
            if (!_stop)
                Connect();
        }

        private static List<int> msgFromTabletIndex = new List<int>();
        private static List<AssistantDecisionDto> adsFromTablet = new List<AssistantDecisionDto>();

        protected override void OnReceived(EndPoint endpoint, byte[] buffer, long offset, long size)
        {
            //Console.WriteLine("Incoming: " + Encoding.UTF8.GetString(buffer, (int)offset, (int)size));

            // Continue receive datagrams

            var strMsg = Encoding.UTF8.GetString(buffer, (int)offset, (int)size).Replace("\r", "").Replace("\n", "");
            if (!string.IsNullOrWhiteSpace(strMsg))
            {
                if (strMsg == "$$$helloserver$$$")
                    Console.WriteLine("tablet hand shakes.");
                else if (strMsg.StartsWith("{") && strMsg.EndsWith("}"))
                {
                    try
                    {
                        var jsonMsg = strMsg.ToJObject();
                        if (jsonMsg.ContainsKey("index") && jsonMsg.ContainsKey("total"))
                        {
                            if (msgFromTabletIndex.Contains(Convert.ToInt32(jsonMsg["index"])))
                            {
                                var tempAssistantDecisionDtos = adsFromTablet;

                                var curAssistantDecisionDtos = StaticEntities.ShowEntities.AssistantDecisions.FirstOrDefault(t => t.Number == "SAD1").AssistantDecisionDtos;

                                foreach (var dto in tempAssistantDecisionDtos)
                                {
                                    if (curAssistantDecisionDtos.Any(t => t.Key == dto.Key))
                                    {
                                        var index = curAssistantDecisionDtos.IndexOf(curAssistantDecisionDtos.FirstOrDefault(t => t.Key == dto.Key));
                                        curAssistantDecisionDtos[index].Content = dto.Content;
                                        curAssistantDecisionDtos[index].State = dto.State;
                                    }
                                    else
                                    {
                                        curAssistantDecisionDtos.Add(new AssistantDecisionDto
                                        {
                                            Key = dto.Key,
                                            Content = dto.Content,
                                            State = dto.State,
                                            Number = "SAD1"
                                        });
                                    }
                                }
                                StaticEntities.ShowEntities.AssistantDecisions[StaticEntities.ShowEntities.AssistantDecisions.IndexOf(StaticEntities.ShowEntities.AssistantDecisions.FirstOrDefault(t => t.Number == "SAD1"))].AssistantDecisionDtos = StaticEntities.StaticEntities.AssistantDecisions.FirstOrDefault(t => t.Number == "SAD1").AssistantDecisionDtos = curAssistantDecisionDtos;

                                msgFromTabletIndex.Clear();
                                adsFromTablet.Clear();
                            }
                            else
                            {
                                msgFromTabletIndex.Add(Convert.ToInt32(jsonMsg["index"]));
                            }
                            adsFromTablet.AddRange(jsonMsg["list"].ToString().ToList<AssistantDecisionDto>());
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, MethodBase.GetCurrentMethod().DeclaringType.Namespace + "_" + MethodBase.GetCurrentMethod().Name);
                    }
                }
            }

            ReceiveAsync();
        }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"Echo UDP client caught an error with code {error}");

            if (!IsConnected)
            {
                Connect();
            }
        }
    }
}