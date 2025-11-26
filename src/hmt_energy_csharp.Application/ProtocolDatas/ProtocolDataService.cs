using hmt_energy_csharp.Devices;
using hmt_energy_csharp.Dtos;
using hmt_energy_csharp.Energy.Batteries;
using hmt_energy_csharp.Energy.Configs;
using hmt_energy_csharp.Energy.Criterias;
using hmt_energy_csharp.Energy.Flowmeters;
using hmt_energy_csharp.Energy.Generators;
using hmt_energy_csharp.Energy.LiquidLevels;
using hmt_energy_csharp.Energy.PowerUnits;
using hmt_energy_csharp.Energy.Predictions;
using hmt_energy_csharp.Energy.Shafts;
using hmt_energy_csharp.Energy.SternSealings;
using hmt_energy_csharp.Energy.SupplyUnits;
using hmt_energy_csharp.Energy.TotalIndicators;
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
using hmt_energy_csharp.HttpRequest;
using hmt_energy_csharp.IEC61162SX5s;
using hmt_energy_csharp.ResponseResults;
using hmt_energy_csharp.Sentences;
using hmt_energy_csharp.Sockets;
using hmt_energy_csharp.TempDatas.Drafts;
using hmt_energy_csharp.VdrDpts;
using hmt_energy_csharp.VdrGgas;
using hmt_energy_csharp.VdrGnss;
using hmt_energy_csharp.VdrMwds;
using hmt_energy_csharp.VdrMwvs;
using hmt_energy_csharp.VdrRmcs;
using hmt_energy_csharp.VdrVbws;
using hmt_energy_csharp.VdrVlws;
using hmt_energy_csharp.VdrVtgs;
using hmt_energy_csharp.VesselInfos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace hmt_energy_csharp.ProtocolDatas
{
    public class ProtocolDataService : hmt_energy_csharpAppService, IProtocolDataService
    {
        private readonly ILogger<ProtocolDataService> _logger;
        private readonly IVesselInfoService _vesselInfoService;
        private readonly ISentenceService _sentenceService;
        private readonly TcpClientService _eeTcpClient;
        private readonly IConfiguration _configuration;
        private readonly IFlowmeterService _flowmeterService;
        private readonly HttpRestClient _httpRestClient;

        public ProtocolDataService(ILogger<ProtocolDataService> logger, IVesselInfoService vesselInfoService, ISentenceService sentenceService, TcpClientService eeTcpClient, IConfiguration configuration, IFlowmeterService flowmeterService)
        {
            _logger = logger;
            _vesselInfoService = vesselInfoService;
            _sentenceService = sentenceService;
            _eeTcpClient = eeTcpClient;
            _configuration = configuration;
            _flowmeterService = flowmeterService;
            _httpRestClient = new HttpRestClient(_configuration["prediction:url"]);
        }

        private async Task TestRest()
        {
            var requestParams = new RequestParams
            {
                Route = _configuration["prediction:route"] + "?data=81,6980,812,6.635,-0.48,-1.26248,16.849,16.3,3.97,11,23.6",
                Parameter = new Dictionary<string, string>()
            };
            var content = await _httpRestClient.ExecuteAsync(requestParams);
            await Console.Out.WriteLineAsync("TestRest:" + content);
        }

        /// <summary>
        /// 解析
        /// </summary>
        /// <param name="number">采集系统序列号</param>
        /// <param name="sentence">语句</param>
        /// <param name="startChars">起始符</param>
        /// <param name="endChars">结束符</param>
        /// <param name="bsi">船舶基本信息</param>
        /// <returns></returns>
        public async Task<ResponseResult> DecodeAsync(string number, string sentence, string startChars, string endChars, BaseShipInfo bsi)
        {
            var log = new List<LogBook>();
            var logEn = new List<LogBook>();
            var result = 0;
            try
            {
                //协议语句临时列表初始化判断
                if (!StaticEntities.StaticEntities.ProtocolParams.Any(t => t.number == number))
                    StaticEntities.StaticEntities.ProtocolParams.Add(new StaticEntities.ProtocolParam { number = number });

                var tempProtocolParam = StaticEntities.StaticEntities.ProtocolParams.FirstOrDefault(t => t.number == number);
                StaticEntities.StaticEntities.ProtocolParams.Remove(tempProtocolParam);
                //处理粘包
                if (!string.IsNullOrWhiteSpace(tempProtocolParam.leftChars))
                {
                    sentence = tempProtocolParam.leftChars + sentence;
                    tempProtocolParam.leftChars = "";
                }
                string[] strDatas = sentence.Split(startChars); // 根据起始分隔符断句
                tempProtocolParam.leftChars = strDatas[strDatas.Length - 1];
                //判断断句列表最后一条语句是否完整 不完整判断为粘包数据
                if (string.IsNullOrWhiteSpace(endChars) ? tempProtocolParam.leftChars.IndexOf('*') == (tempProtocolParam.leftChars.Length - 3) : tempProtocolParam.leftChars.IndexOf(endChars) == (tempProtocolParam.leftChars.Length - endChars.Length))
                {
                    tempProtocolParam.sentences = strDatas.ToList();
                    tempProtocolParam.leftChars = "";
                }
                else
                    tempProtocolParam.sentences = strDatas.SkipLast(1).ToList();

                //遍历解析数据
                foreach (var singleSentence in tempProtocolParam.sentences)
                {
                    try
                    {
                        if (string.IsNullOrWhiteSpace(singleSentence))
                            continue;
                        var currentSentence = string.IsNullOrWhiteSpace(endChars) ? singleSentence : singleSentence.Replace(endChars, "");
                        var sentenceParams = currentSentence.Split(',').ToList();
                        var ReceiveDatetimeFmtInt = sentenceParams[2];    // 数据时间
                        var ReceiveDatetimeFmtDt = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(ReceiveDatetimeFmtInt)).UtcDateTime;    // 数据时间
                        var protocolType = sentenceParams[3];   // 协议类型
                        var Producer = sentenceParams[4];   // 生产商
                        sentenceParams.RemoveAt(4);
                        sentenceParams.RemoveAt(3);
                        sentenceParams.RemoveAt(2);
                        sentenceParams.RemoveAt(1);
                        sentenceParams.RemoveAt(0);
                        //包含实际数据的原始或初加工的语句
                        var oriSentence = string.Join(",", sentenceParams);

                        //保存源语句
                        CreateSentenceDto sentenceDto = new CreateSentenceDto();
                        sentenceDto.time = Convert.ToInt64(ReceiveDatetimeFmtInt);
                        sentenceDto.data = oriSentence;
                        sentenceDto.vdr_id = number;

                        if (!StaticEntities.StaticEntities.Vessels.Any(t => t.SN == number))
                        {
                            InitVesselAll(number, ReceiveDatetimeFmtDt);
                        }
                        //获取当前船舶信息
                        var currentVessel = StaticEntities.StaticEntities.Vessels.FirstOrDefault(t => t.SN == number);
                        var indexVessel = StaticEntities.StaticEntities.Vessels.IndexOf(currentVessel);
                        //判断是否新时间段信息 是就进行数据保存并对属性进行初始化
                        if (StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime.AddSeconds(10) <= ReceiveDatetimeFmtDt)
                        {
                            try { _eeTcpClient.SendAsync(StaticEntities.StaticEntities.Vessels[indexVessel].ToJson()); } catch (Exception ex) { }

                            try
                            {
                                await SaveVesselAll(number);
                            }
                            catch (Exception ex)
                            {
                            }

                            //log.AddRange(GetLossDevices(number));
                            //log.AddRange(GetLimitWarning(number));
                            try
                            {
                                GetLimitWarning(number, ref log, ref logEn);
                            }
                            catch (Exception ex)
                            {
                            }

                            UpdateVesselAll(number, ReceiveDatetimeFmtDt);
                        }

                        #region 解析过程

                        if (protocolType == "other")
                        {
                            var strDataNews = oriSentence.Split(',').ToList();
                            var deviceCode = strDataNews[0];
                            strDataNews.RemoveAt(0);
                            var EncodeSentence = string.Join(",", strDataNews);
                            log.AddRange(DataProccess(number, deviceCode, EncodeSentence));
                            sentenceDto.data = EncodeSentence;
                            sentenceDto.category = deviceCode;
                        }
                        else if (protocolType == "NMEA")
                        {
                            var strDataNews = oriSentence.Split(',').ToList();
                            var deviceCode = strDataNews[0];
                            strDataNews.RemoveAt(0);
                            var EncodeSentence = string.Join(",", strDataNews);
                            log.AddRange(DataProccessNEMA(number, deviceCode, EncodeSentence));
                            sentenceDto.data = EncodeSentence;
                            sentenceDto.category = deviceCode;
                        }
                        else if (protocolType == "modbus")
                        {
                            var strDataNews = oriSentence.Split(',').ToList();
                            var deviceCode = strDataNews[1];
                            strDataNews.RemoveAt(1);
                            strDataNews.RemoveAt(0);
                            var EncodeSentence = string.Join(",", strDataNews).Trim(',');
                            log.AddRange(DataProccessMODBUS(number, deviceCode, EncodeSentence));
                            sentenceDto.data = EncodeSentence;
                            sentenceDto.category = deviceCode;
                        }
                        //61162协议数据解析
                        else if (protocolType == "61162-1")
                        {
                            oriSentence = oriSentence.Substring(0, oriSentence.Length - 3);
                            var str61162s = oriSentence.Split(',');
                            if (Producer == "customize")
                            {
                                //EMARAT AMS点位
                                string strDevice = str61162s[0].Replace("$", "").ToUpper();

                                var monitoredDevicesItem = StaticEntities.StaticEntities.MonitoredDevices.FirstOrDefault(t => t.Number == number);
                                var monitoredDevicesIndex = StaticEntities.StaticEntities.MonitoredDevices.IndexOf(monitoredDevicesItem);

                                StaticEntities.StaticEntities.MonitoredDevices[monitoredDevicesIndex].Devices["ams"] = DateTime.UtcNow;
                                sentenceDto.category = "ams";

                                log.AddRange(await DataProccessEMARATAMS(number, strDevice, str61162s));
                            }
                            else
                            {
                                string strDevice = str61162s[0].Substring(str61162s[0].Length - 3, 3).ToUpper();

                                var monitoredDevicesItem = StaticEntities.StaticEntities.MonitoredDevices.FirstOrDefault(t => t.Number == number);
                                var monitoredDevicesIndex = StaticEntities.StaticEntities.MonitoredDevices.IndexOf(monitoredDevicesItem);

                                switch (strDevice)
                                {
                                    case "FEC":
                                        var ecEntity = new PFEC(sentenceDto.data);
                                        StaticEntities.StaticEntities.Vessels[indexVessel].X = ecEntity.X;
                                        StaticEntities.StaticEntities.Vessels[indexVessel].Y = ecEntity.Y;
                                        sentenceDto.category = "pfec";
                                        StaticEntities.StaticEntities.MonitoredDevices[monitoredDevicesIndex].Devices["pfec"] = DateTime.UtcNow;
                                        break;
                                    case "HRM":
                                        var hrmEntity = new ICHRM(sentenceDto.data);
                                        StaticEntities.StaticEntities.Vessels[indexVessel].X = hrmEntity.RollAngle;
                                        sentenceDto.category = "ichrm";
                                        StaticEntities.StaticEntities.MonitoredDevices[monitoredDevicesIndex].Devices["ichrm"] = DateTime.UtcNow;
                                        break;

                                    case "ABK":
                                    case "ABM":
                                    case "ACA":
                                    case "ACS":
                                    case "AIR":
                                    case "BBM":
                                    case "LR1":
                                    case "LR2":
                                    case "LR3":
                                    case "LRF":
                                    case "LRI":
                                    case "SSD":
                                    case "TRL":
                                    case "VDM":
                                    case "VDO":
                                    case "VSD":
                                        _eeTcpClient.SendAsync(sentenceDto.data);
                                        sentenceDto.category = strDevice.ToLower();
                                        break;

                                    case "MWV":
                                        var mwvEntity = new VdrMwv(sentenceDto.data);
                                        StaticEntities.StaticEntities.Vessels[indexVessel].WindDirection = mwvEntity.angle;
                                        StaticEntities.StaticEntities.Vessels[indexVessel].WindSpeed = mwvEntity.speed;
                                        sentenceDto.category = "mwv";
                                        StaticEntities.StaticEntities.MonitoredDevices[monitoredDevicesIndex].Devices["mwd"] = DateTime.UtcNow;
                                        break;

                                    case "VBW":
                                        var vbmEntity = new VdrVbw(sentenceDto.data);
                                        StaticEntities.StaticEntities.Vessels[indexVessel].WaterSpeed = vbmEntity.watspd;
                                        //StaticEntities.StaticEntities.Vessels[indexVessel].GroundSpeed = vbmEntity.grdspd;
                                        sentenceDto.category = "vbw";
                                        StaticEntities.StaticEntities.MonitoredDevices[monitoredDevicesIndex].Devices["vbw"] = DateTime.UtcNow;
                                        break;

                                    case "MWD":
                                        var mwdEntity = new VdrMwd(sentenceDto.data);
                                        StaticEntities.StaticEntities.Vessels[indexVessel].WindDirection = mwdEntity.tdirection;
                                        StaticEntities.StaticEntities.Vessels[indexVessel].WindSpeed = mwdEntity.speed;
                                        sentenceDto.category = "mwd";
                                        StaticEntities.StaticEntities.MonitoredDevices[monitoredDevicesIndex].Devices["mwd"] = DateTime.UtcNow;
                                        break;

                                    case "GGA":
                                        var ggaEntity = new VdrGga(sentenceDto.data);
                                        var coor = GPSHelper.GetBdFrom84(new pointLatLon(Convert.ToDouble(ggaEntity.latitude), Convert.ToDouble(ggaEntity.longitude), gpsType.wgs84));
                                        StaticEntities.StaticEntities.Vessels[indexVessel].Latitude = coor.Lat;
                                        StaticEntities.StaticEntities.Vessels[indexVessel].Longitude = coor.Lon;
                                        sentenceDto.category = "gga";
                                        StaticEntities.StaticEntities.MonitoredDevices[monitoredDevicesIndex].Devices["gps"] = DateTime.UtcNow;
                                        break;

                                    case "RMC":
                                        var rmcEntity = new VdrRmc(sentenceDto.data);
                                        var coor1 = GPSHelper.GetBdFrom84(new pointLatLon(Convert.ToDouble(rmcEntity.latitude), Convert.ToDouble(rmcEntity.longtitude), gpsType.wgs84));
                                        StaticEntities.StaticEntities.Vessels[indexVessel].Latitude = coor1.Lat;
                                        StaticEntities.StaticEntities.Vessels[indexVessel].Longitude = coor1.Lon;
                                        StaticEntities.StaticEntities.Vessels[indexVessel].GroundSpeed = Convert.ToDouble(rmcEntity.grdspeed.IsNullOrWhiteSpace() ? "0" : rmcEntity.grdspeed);
                                        StaticEntities.StaticEntities.Vessels[indexVessel].Course = Convert.ToDouble(rmcEntity.grdcoz.IsNullOrWhiteSpace() ? "0" : rmcEntity.grdcoz);
                                        StaticEntities.StaticEntities.Vessels[indexVessel].MagneticVariation = Convert.ToDouble(rmcEntity.magvar.IsNullOrWhiteSpace() ? "0" : rmcEntity.magvar);
                                        sentenceDto.category = "rmc";
                                        _logger.LogWarning("rmc解析3{mc}", StaticEntities.StaticEntities.MonitoredDevices.Count);
                                        _logger.LogWarning("rmc解析3{mdi}", monitoredDevicesIndex);
                                        StaticEntities.StaticEntities.MonitoredDevices[monitoredDevicesIndex].Devices["gps"] = DateTime.UtcNow;
                                        StaticEntities.StaticEntities.MonitoredDevices[monitoredDevicesIndex].Devices["course"] = DateTime.UtcNow;
                                        _logger.LogWarning("{CurDatetime}-结束解析rmc", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                                        break;

                                    case "RPM":
                                        sentenceDto.category = "rpm";
                                        break;

                                    case "TRC":
                                        sentenceDto.category = "trc";
                                        break;

                                    case "TRD":
                                        break;

                                    case "VTG":
                                        var vtgEntity = new VdrVtg(sentenceDto.data);
                                        StaticEntities.StaticEntities.Vessels[indexVessel].Course = vtgEntity.grdcoztrue;
                                        StaticEntities.StaticEntities.Vessels[indexVessel].MagneticVariation = vtgEntity.grdcozmag;
                                        sentenceDto.category = "vtg";
                                        break;

                                    case "GNS":
                                        var gnsEntity = new VdrGns(sentenceDto.data);
                                        var coor2 = GPSHelper.GetBdFrom84(new pointLatLon(Convert.ToDouble(gnsEntity.latitude), Convert.ToDouble(gnsEntity.longtitude), gpsType.wgs84));
                                        StaticEntities.StaticEntities.Vessels[indexVessel].Latitude = coor2.Lat;
                                        StaticEntities.StaticEntities.Vessels[indexVessel].Longitude = coor2.Lon;
                                        sentenceDto.category = "gns";
                                        StaticEntities.StaticEntities.MonitoredDevices[monitoredDevicesIndex].Devices["gps"] = DateTime.UtcNow;
                                        break;

                                    case "XDR":
                                    case "RSA":
                                        if (str61162s[0].Substring(0, 2) == "$J")
                                        {
                                            _eeTcpClient.SendAsync(sentenceDto.data);
                                            sentenceDto.category = strDevice.ToLower();
                                        }
                                        break;

                                    case "PRC":
                                        sentenceDto.category = "prc";
                                        break;

                                    case "VLW":
                                        var vlwEntity = new VdrVlw(sentenceDto.data);
                                        StaticEntities.StaticEntities.Vessels[indexVessel].TotalDistanceGrd = vlwEntity.grddistotal;
                                        StaticEntities.StaticEntities.Vessels[indexVessel].ResetDistanceGrd = vlwEntity.grddisreset;
                                        StaticEntities.StaticEntities.Vessels[indexVessel].TotalDistanceWat = vlwEntity.watdistotal;
                                        StaticEntities.StaticEntities.Vessels[indexVessel].ResetDistanceWat = vlwEntity.watdisreset;
                                        sentenceDto.category = "vlw";
                                        StaticEntities.StaticEntities.MonitoredDevices[monitoredDevicesIndex].Devices["vlw"] = DateTime.UtcNow;
                                        break;

                                    case "HDG":
                                        sentenceDto.category = "hdg";
                                        break;

                                    case "DPT":
                                        var dptEntity = new VdrDpt(sentenceDto.data);
                                        StaticEntities.StaticEntities.Vessels[indexVessel].Depth = dptEntity.depth;
                                        StaticEntities.StaticEntities.Vessels[indexVessel].DepthOffset = dptEntity.offset;
                                        sentenceDto.category = "dpt";
                                        StaticEntities.StaticEntities.MonitoredDevices[monitoredDevicesIndex].Devices["dpt"] = DateTime.UtcNow;
                                        break;

                                    case "DRA":
                                        var draEntity = new Draft(sentenceDto.data);
                                        StaticEntities.StaticEntities.Vessels[indexVessel].BowDraft = Convert.ToDouble(draEntity.Bow);
                                        StaticEntities.StaticEntities.Vessels[indexVessel].AsternDraft = Convert.ToDouble(draEntity.Astern);
                                        StaticEntities.StaticEntities.Vessels[indexVessel].PortDraft = Convert.ToDouble(draEntity.Port);
                                        StaticEntities.StaticEntities.Vessels[indexVessel].StarBoardDraft = Convert.ToDouble(draEntity.StartBoard);
                                        StaticEntities.StaticEntities.Vessels[indexVessel].Draft = (StaticEntities.StaticEntities.Vessels[indexVessel].BowDraft + StaticEntities.StaticEntities.Vessels[indexVessel].AsternDraft) / 2d;
                                        StaticEntities.StaticEntities.Vessels[indexVessel].Trim = StaticEntities.StaticEntities.Vessels[indexVessel].AsternDraft - StaticEntities.StaticEntities.Vessels[indexVessel].BowDraft;
                                        StaticEntities.StaticEntities.Vessels[indexVessel].Heel = StaticEntities.StaticEntities.Vessels[indexVessel].PortDraft - StaticEntities.StaticEntities.Vessels[indexVessel].StarBoardDraft;
                                        sentenceDto.category = "dra";
                                        StaticEntities.StaticEntities.MonitoredDevices[monitoredDevicesIndex].Devices["draft"] = DateTime.UtcNow;
                                        break;

                                    default:
                                        sentenceDto.category = "";
                                        break;
                                }
                            }
                        }
                        if (string.IsNullOrWhiteSpace(sentenceDto.category))
                            result = 1;
                        else
                            result = await _sentenceService.CreateAsync(sentenceDto) == null ? 0 : 1;

                        #endregion 解析过程
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "数据解析错误=>" + MethodBase.GetCurrentMethod().DeclaringType.Namespace + "_" + MethodBase.GetCurrentMethod().Name + ":" + sentence);
                        continue;
                    }
                }
                tempProtocolParam.sentences.Clear();
                StaticEntities.StaticEntities.ProtocolParams.Add(tempProtocolParam);
                for (var i = StaticEntities.StaticEntities.ProtocolParams.Count - 1; i >= 0; i--)
                {
                    if (StaticEntities.StaticEntities.ProtocolParams[i] == null)
                        StaticEntities.StaticEntities.ProtocolParams.RemoveAt(i);
                }

                PutShowEntities(number);

                log.AddRange(await CalcVesselInfo(number, bsi));
            }
            catch (Exception)
            {
                Console.WriteLine("err sentence:" + sentence);
                throw;
            }
            return new ResponseResult { IsSuccess = result > 0, LogContents = log, LogContentsEn = logEn };
        }

        private void PutShowEntities(string number)
        {
            StaticEntities.ShowEntities.Vessels[StaticEntities.ShowEntities.Vessels.IndexOf(StaticEntities.ShowEntities.Vessels.FirstOrDefault(t => t.SN == number))] = StaticEntities.StaticEntities.Vessels.FirstOrDefault(t => t.SN == number);

            StaticEntities.ShowEntities.Flowmeters[StaticEntities.ShowEntities.Flowmeters.IndexOf(StaticEntities.ShowEntities.Flowmeters.FirstOrDefault(t => t.Number == number))] = StaticEntities.StaticEntities.Flowmeters.FirstOrDefault(t => t.Number == number);
            StaticEntities.ShowEntities.Batteries[StaticEntities.ShowEntities.Batteries.IndexOf(StaticEntities.ShowEntities.Batteries.FirstOrDefault(t => t.Number == number))] = StaticEntities.StaticEntities.Batteries.FirstOrDefault(t => t.Number == number);
            //StaticEntities.ShowEntities.Generators[StaticEntities.ShowEntities.Generators.IndexOf(StaticEntities.ShowEntities.Generators.FirstOrDefault(t => t.Number == number))] = StaticEntities.StaticEntities.Generators.FirstOrDefault(t => t.Number == number);

            var showGes = StaticEntities.ShowEntities.Generators[StaticEntities.ShowEntities.Generators.IndexOf(StaticEntities.ShowEntities.Generators.FirstOrDefault(t => t.Number == number))];
            var getGes = StaticEntities.StaticEntities.Generators.FirstOrDefault(t => t.Number == number);
            showGes.GeneratorDtos.Clear();
            if (getGes.GeneratorDtos.Any(t => t.Number == number && t.DeviceNo == "generator_1"))
                showGes.GeneratorDtos.Add(getGes.GeneratorDtos.FirstOrDefault(t => t.Number == number && t.DeviceNo == "generator_1"));
            if (getGes.GeneratorDtos.Any(t => t.Number == number && t.DeviceNo == "generator_2"))
                showGes.GeneratorDtos.Add(getGes.GeneratorDtos.FirstOrDefault(t => t.Number == number && t.DeviceNo == "generator_2"));
            if (getGes.GeneratorDtos.Any(t => t.Number == number && t.DeviceNo == "generator_3"))
                showGes.GeneratorDtos.Add(getGes.GeneratorDtos.FirstOrDefault(t => t.Number == number && t.DeviceNo == "generator_3"));
            if (getGes.GeneratorDtos.Any(t => t.Number == number && t.DeviceNo == "generator_4"))
                showGes.GeneratorDtos.Add(getGes.GeneratorDtos.FirstOrDefault(t => t.Number == number && t.DeviceNo == "generator_4"));

            StaticEntities.ShowEntities.LiquidLevels[StaticEntities.ShowEntities.LiquidLevels.IndexOf(StaticEntities.ShowEntities.LiquidLevels.FirstOrDefault(t => t.Number == number))] = StaticEntities.StaticEntities.LiquidLevels.FirstOrDefault(t => t.Number == number);
            StaticEntities.ShowEntities.SupplyUnits[StaticEntities.ShowEntities.SupplyUnits.IndexOf(StaticEntities.ShowEntities.SupplyUnits.FirstOrDefault(t => t.Number == number))] = StaticEntities.StaticEntities.SupplyUnits.FirstOrDefault(t => t.Number == number);
            StaticEntities.ShowEntities.Shafts[StaticEntities.ShowEntities.Shafts.IndexOf(StaticEntities.ShowEntities.Shafts.FirstOrDefault(t => t.Number == number))] = StaticEntities.StaticEntities.Shafts.FirstOrDefault(t => t.Number == number);
            StaticEntities.ShowEntities.SternSealings[StaticEntities.ShowEntities.SternSealings.IndexOf(StaticEntities.ShowEntities.SternSealings.FirstOrDefault(t => t.Number == number))] = StaticEntities.StaticEntities.SternSealings.FirstOrDefault(t => t.Number == number);
            StaticEntities.ShowEntities.PowerUnits[StaticEntities.ShowEntities.PowerUnits.IndexOf(StaticEntities.ShowEntities.PowerUnits.FirstOrDefault(t => t.Number == number))] = StaticEntities.StaticEntities.PowerUnits.FirstOrDefault(t => t.Number == number);

            StaticEntities.ShowEntities.TotalIndicators[StaticEntities.ShowEntities.TotalIndicators.IndexOf(StaticEntities.ShowEntities.TotalIndicators.FirstOrDefault(t => t.Number == number))] = StaticEntities.StaticEntities.TotalIndicators.FirstOrDefault(t => t.Number == number);

            StaticEntities.ShowEntities.Predictions[StaticEntities.ShowEntities.Predictions.IndexOf(StaticEntities.ShowEntities.Predictions.FirstOrDefault(t => t.Number == number))] = StaticEntities.StaticEntities.Predictions.FirstOrDefault(t => t.Number == number);

            StaticEntities.ShowEntities.CompositeBoilers[StaticEntities.ShowEntities.CompositeBoilers.IndexOf(StaticEntities.ShowEntities.CompositeBoilers.FirstOrDefault(t => t.Number == number))] = StaticEntities.StaticEntities.CompositeBoilers.FirstOrDefault(t => t.Number == number);
            StaticEntities.ShowEntities.CompressedAirSupplies[StaticEntities.ShowEntities.CompressedAirSupplies.IndexOf(StaticEntities.ShowEntities.CompressedAirSupplies.FirstOrDefault(t => t.Number == number))] = StaticEntities.StaticEntities.CompressedAirSupplies.FirstOrDefault(t => t.Number == number);
            StaticEntities.ShowEntities.CoolingFreshWaters[StaticEntities.ShowEntities.CoolingFreshWaters.IndexOf(StaticEntities.ShowEntities.CoolingFreshWaters.FirstOrDefault(t => t.Number == number))] = StaticEntities.StaticEntities.CoolingFreshWaters.FirstOrDefault(t => t.Number == number);
            StaticEntities.ShowEntities.CoolingSeaWaters[StaticEntities.ShowEntities.CoolingSeaWaters.IndexOf(StaticEntities.ShowEntities.CoolingSeaWaters.FirstOrDefault(t => t.Number == number))] = StaticEntities.StaticEntities.CoolingSeaWaters.FirstOrDefault(t => t.Number == number);
            StaticEntities.ShowEntities.CoolingWaters[StaticEntities.ShowEntities.CoolingWaters.IndexOf(StaticEntities.ShowEntities.CoolingWaters.FirstOrDefault(t => t.Number == number))] = StaticEntities.StaticEntities.CoolingWaters.FirstOrDefault(t => t.Number == number);
            StaticEntities.ShowEntities.CylinderLubOils[StaticEntities.ShowEntities.CylinderLubOils.IndexOf(StaticEntities.ShowEntities.CylinderLubOils.FirstOrDefault(t => t.Number == number))] = StaticEntities.StaticEntities.CylinderLubOils.FirstOrDefault(t => t.Number == number);
            StaticEntities.ShowEntities.ExhaustGases[StaticEntities.ShowEntities.ExhaustGases.IndexOf(StaticEntities.ShowEntities.ExhaustGases.FirstOrDefault(t => t.Number == number))] = StaticEntities.StaticEntities.ExhaustGases.FirstOrDefault(t => t.Number == number);
            StaticEntities.ShowEntities.FOs[StaticEntities.ShowEntities.FOs.IndexOf(StaticEntities.ShowEntities.FOs.FirstOrDefault(t => t.Number == number))] = StaticEntities.StaticEntities.FOs.FirstOrDefault(t => t.Number == number);
            StaticEntities.ShowEntities.FOSupplyUnits[StaticEntities.ShowEntities.FOSupplyUnits.IndexOf(StaticEntities.ShowEntities.FOSupplyUnits.FirstOrDefault(t => t.Number == number))] = StaticEntities.StaticEntities.FOSupplyUnits.FirstOrDefault(t => t.Number == number);
            StaticEntities.ShowEntities.LubOilPurifyings[StaticEntities.ShowEntities.LubOilPurifyings.IndexOf(StaticEntities.ShowEntities.LubOilPurifyings.FirstOrDefault(t => t.Number == number))] = StaticEntities.StaticEntities.LubOilPurifyings.FirstOrDefault(t => t.Number == number);
            StaticEntities.ShowEntities.LubOils[StaticEntities.ShowEntities.LubOils.IndexOf(StaticEntities.ShowEntities.LubOils.FirstOrDefault(t => t.Number == number))] = StaticEntities.StaticEntities.LubOils.FirstOrDefault(t => t.Number == number);
            StaticEntities.ShowEntities.MainGeneratorSets[StaticEntities.ShowEntities.MainGeneratorSets.IndexOf(StaticEntities.ShowEntities.MainGeneratorSets.FirstOrDefault(t => t.Number == number))] = StaticEntities.StaticEntities.MainGeneratorSets.FirstOrDefault(t => t.Number == number);
            StaticEntities.ShowEntities.MainSwitchboards[StaticEntities.ShowEntities.MainSwitchboards.IndexOf(StaticEntities.ShowEntities.MainSwitchboards.FirstOrDefault(t => t.Number == number))] = StaticEntities.StaticEntities.MainSwitchboards.FirstOrDefault(t => t.Number == number);
            StaticEntities.ShowEntities.MERemoteControls[StaticEntities.ShowEntities.MERemoteControls.IndexOf(StaticEntities.ShowEntities.MERemoteControls.FirstOrDefault(t => t.Number == number))] = StaticEntities.StaticEntities.MERemoteControls.FirstOrDefault(t => t.Number == number);
            StaticEntities.ShowEntities.Miscellaneouses[StaticEntities.ShowEntities.Miscellaneouses.IndexOf(StaticEntities.ShowEntities.Miscellaneouses.FirstOrDefault(t => t.Number == number))] = StaticEntities.StaticEntities.Miscellaneouses.FirstOrDefault(t => t.Number == number);
            StaticEntities.ShowEntities.ScavengeAirs[StaticEntities.ShowEntities.ScavengeAirs.IndexOf(StaticEntities.ShowEntities.ScavengeAirs.FirstOrDefault(t => t.Number == number))] = StaticEntities.StaticEntities.ScavengeAirs.FirstOrDefault(t => t.Number == number);
            StaticEntities.ShowEntities.ShaftClutchs[StaticEntities.ShowEntities.ShaftClutchs.IndexOf(StaticEntities.ShowEntities.ShaftClutchs.FirstOrDefault(t => t.Number == number))] = StaticEntities.StaticEntities.ShaftClutchs.FirstOrDefault(t => t.Number == number);
        }

        /// <summary>
        /// 生成报警日志
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        private IList<LogBook> GetLossDevices(string number)
        {
            var result = new List<LogBook>();
            var MonitoredDevice = StaticEntities.StaticEntities.MonitoredDevices.FirstOrDefault(t => t.Number == number);
            if (MonitoredDevice == null)
                return result;
            var currentDT = DateTime.UtcNow;
            foreach (var device in MonitoredDevice.Devices)
            {
                var deviceConfig = StaticEntities.StaticEntities.Configs.FirstOrDefault(t => t.Number == number && t.Code == device.Key);

                if (deviceConfig != null)
                    //设备信息丢失日志
                    //if (device.Value.AddSeconds((int)deviceConfig.Interval) < currentDT)
                    if (device.Value.AddSeconds(15) < currentDT)
                        result.Add(new LogBook { type = "warning", content = $"{deviceConfig.Name}位置信息丢失。", time = DateTime.Now.ToString("yyyy-MM-dd HH:mm") });
            }
            return result;
        }

        private IList<LogBook> GetLimitWarning(string number, ref List<LogBook> logs, ref List<LogBook> logsEn)
        {
            if (!StaticEntities.StaticEntities.Vessels.Any(t => t.SN == number))
                return null;
            var currentVessel = StaticEntities.StaticEntities.Vessels[StaticEntities.StaticEntities.Vessels.IndexOf(StaticEntities.StaticEntities.Vessels.FirstOrDefault(t => t.SN == number))];
            var result = new List<LogBook>();
            GetLimitLog(number, "WaterSpeed", currentVessel.WaterSpeed ?? 0, ref logs, ref logsEn);
            GetLimitLog(number, "GroundSpeed", currentVessel.GroundSpeed ?? 0, ref logs, ref logsEn);
            GetLimitLog(number, "WindSpeed", currentVessel.WindSpeed ?? 0, ref logs, ref logsEn);
            GetLimitLog(number, "Trim", currentVessel.Trim ?? 0, ref logs, ref logsEn);
            GetLimitLog(number, "Heel", currentVessel.Heel ?? 0, ref logs, ref logsEn);
            GetLimitLog(number, "Draft", currentVessel.Draft ?? 0, ref logs, ref logsEn);

            GetLimitLog(number, "SFOC", currentVessel.SFOC ?? 0, ref logs, ref logsEn);
            GetLimitLog(number, "FCPerNm", currentVessel.FCPerNm ?? 0, ref logs, ref logsEn);
            GetLimitLog(number, "MESFOC", currentVessel.MESFOC ?? 0, ref logs, ref logsEn);
            GetLimitLog(number, "MEFCPerNm", currentVessel.MEFCPerNm ?? 0, ref logs, ref logsEn);
            GetLimitLog(number, "Slip", currentVessel.Slip ?? 0, ref logs, ref logsEn);
            return result;
        }

        /// <summary>
        /// 获取越限报警
        /// </summary>
        /// <param name="number"></param>
        /// <param name="param"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        private IList<LogBook> GetLimitLog(string number, string param, double val, ref List<LogBook> logs, ref List<LogBook> logsEn)
        {
            var result = new List<LogBook>();

            ConfigDto configDto = StaticEntities.StaticEntities.Configs.FirstOrDefault(t => t.Number == number && t.IsDevice == 0 && t.Code == param);
            if (val > Convert.ToSingle(configDto?.HighHighLimit ?? 99999999))
            {
                logs.Add(new LogBook { type = "warning", content = $"{configDto.Name}超过高高限值，当前:{val}，限值:{configDto.HighHighLimit}。", time = DateTime.Now.ToString("yyyy-MM-dd HH:mm") });
                logsEn.Add(new LogBook { type = "warning", content = $"{configDto.Name} HighHighLimit,current:{val}，limiting value:{configDto.HighHighLimit}。", time = DateTime.Now.ToString("yyyy-MM-dd HH:mm") });
            }
            else if (val > Convert.ToSingle(configDto?.HighLimit ?? 99999999))
            {
                logs.Add(new LogBook { type = "alert", content = $"{configDto.Name}超过高限值，当前:{val}，限值:{configDto.HighLimit}。", time = DateTime.Now.ToString("yyyy-MM-dd HH:mm") });
                logsEn.Add(new LogBook { type = "alert", content = $"{configDto.Name} HighLimit,current:{val}，limiting value:{configDto.HighLimit}。", time = DateTime.Now.ToString("yyyy-MM-dd HH:mm") });
            }

            return result;
        }

        /// <summary>
        /// 保存实时数据
        /// </summary>
        /// <param name="number"></param>
        private async Task SaveVesselAll(string number)
        {
            var vessel = StaticEntities.StaticEntities.Vessels.FirstOrDefault(t => t.SN == number);
            var ti = StaticEntities.StaticEntities.TotalIndicators.FirstOrDefault(t => t.Number == number);
            var p = StaticEntities.StaticEntities.Predictions.FirstOrDefault(t => t.Number == number);
            var bs = StaticEntities.StaticEntities.Batteries.FirstOrDefault(t => t.Number == number).BatteryDtos;
            var fs = StaticEntities.StaticEntities.Flowmeters.FirstOrDefault(t => t.Number == number).FlowmeterDtos;
            var gs = StaticEntities.StaticEntities.Generators.FirstOrDefault(t => t.Number == number).GeneratorDtos;
            var lls = StaticEntities.StaticEntities.LiquidLevels.FirstOrDefault(t => t.Number == number).LiquidLevelDtos;
            var ss = StaticEntities.StaticEntities.Shafts.FirstOrDefault(t => t.Number == number).ShaftDtos;
            var sss = StaticEntities.StaticEntities.SternSealings.FirstOrDefault(t => t.Number == number).SternSealingDtos;
            var sus = StaticEntities.StaticEntities.SupplyUnits.FirstOrDefault(t => t.Number == number).SupplyUnitDtos;
            var pus = StaticEntities.StaticEntities.PowerUnits.FirstOrDefault(t => t.Number == number).PowerUnitDtos;

            var CompositeBoilers = StaticEntities.StaticEntities.CompositeBoilers.FirstOrDefault(t => t.Number == number).CompositeBoilerDtos;
            var CompressedAirSupplies = StaticEntities.StaticEntities.CompressedAirSupplies.FirstOrDefault(t => t.Number == number).CompressedAirSupplyDtos;
            var CoolingFreshWaters = StaticEntities.StaticEntities.CoolingFreshWaters.FirstOrDefault(t => t.Number == number).CoolingFreshWaterDtos;
            var CoolingSeaWaters = StaticEntities.StaticEntities.CoolingSeaWaters.FirstOrDefault(t => t.Number == number).CoolingSeaWaterDtos;
            var CoolingWaters = StaticEntities.StaticEntities.CoolingWaters.FirstOrDefault(t => t.Number == number).CoolingWaterDtos;
            var CylinderLubOils = StaticEntities.StaticEntities.CylinderLubOils.FirstOrDefault(t => t.Number == number).CylinderLubOilDtos;
            var ExhaustGases = StaticEntities.StaticEntities.ExhaustGases.FirstOrDefault(t => t.Number == number).ExhaustGasDtos;
            var FOs = StaticEntities.StaticEntities.FOs.FirstOrDefault(t => t.Number == number).FODtos;
            var FOSupplyUnits = StaticEntities.StaticEntities.FOSupplyUnits.FirstOrDefault(t => t.Number == number).FOSupplyUnitDtos;
            var LubOilPurifyings = StaticEntities.StaticEntities.LubOilPurifyings.FirstOrDefault(t => t.Number == number).LubOilPurifyingDtos;
            var LubOils = StaticEntities.StaticEntities.LubOils.FirstOrDefault(t => t.Number == number).LubOilDtos;
            var MainGeneratorSets = StaticEntities.StaticEntities.MainGeneratorSets.FirstOrDefault(t => t.Number == number).MainGeneratorSetDtos;
            var MainSwitchboards = StaticEntities.StaticEntities.MainSwitchboards.FirstOrDefault(t => t.Number == number).MainSwitchboardDtos;
            var MERemoteControls = StaticEntities.StaticEntities.MERemoteControls.FirstOrDefault(t => t.Number == number).MERemoteControlDtos;
            var Miscellaneouses = StaticEntities.StaticEntities.Miscellaneouses.FirstOrDefault(t => t.Number == number).MiscellaneousDtos;
            var ScavengeAirs = StaticEntities.StaticEntities.ScavengeAirs.FirstOrDefault(t => t.Number == number).ScavengeAirDtos;
            var ShaftClutchs = StaticEntities.StaticEntities.ShaftClutchs.FirstOrDefault(t => t.Number == number).ShaftClutchDtos;
            var AssistantDecisions = StaticEntities.StaticEntities.AssistantDecisions.FirstOrDefault(t => t.Number == number)?.AssistantDecisionDtos;

            await _vesselInfoService.SaveVesselAllAsync(vessel, ti, p, bs, fs, gs, lls, ss, sss, sus, pus);

            await _vesselInfoService.SaveERAllAsync(CompositeBoilers, CompressedAirSupplies, CoolingFreshWaters, CoolingSeaWaters, CoolingWaters, CylinderLubOils, ExhaustGases, FOs, FOSupplyUnits, LubOilPurifyings, LubOils, MainGeneratorSets, MainSwitchboards, MERemoteControls, Miscellaneouses, ScavengeAirs, ShaftClutchs);

            await _vesselInfoService.SaveERADAsync(vessel, AssistantDecisions);
        }

        /// <summary>
        /// 初始化船舶所有属性
        /// </summary>
        /// <param name="number"></param>
        /// <param name="receiveDatetime"></param>
        /// <returns></returns>
        private void InitVesselAll(string number, DateTime receiveDatetime)
        {
            #region 当前实体

            StaticEntities.StaticEntities.Vessels.Add(new VesselInfoDto { SN = number, ReceiveDatetime = receiveDatetime });

            var fms = StaticEntities.StaticEntities.Flowmeters.FirstOrDefault(t => t.Number == number);
            if (fms == null)
                StaticEntities.StaticEntities.Flowmeters.Add(new VesselFlowmeterDto { Number = number });
            else
                StaticEntities.StaticEntities.Flowmeters[StaticEntities.StaticEntities.Flowmeters.IndexOf(fms)].FlowmeterDtos.Clear();
            var bs = StaticEntities.StaticEntities.Batteries.FirstOrDefault(t => t.Number == number);
            if (bs == null)
                StaticEntities.StaticEntities.Batteries.Add(new VesselBatteryDto { Number = number });
            else
                StaticEntities.StaticEntities.Batteries[StaticEntities.StaticEntities.Batteries.IndexOf(bs)].BatteryDtos.Clear();
            var gs = StaticEntities.StaticEntities.Generators.FirstOrDefault(t => t.Number == number);
            if (gs == null)
                StaticEntities.StaticEntities.Generators.Add(new VesselGeneratorDto { Number = number });
            else
                StaticEntities.StaticEntities.Generators[StaticEntities.StaticEntities.Generators.IndexOf(gs)].GeneratorDtos.Clear();
            var lls = StaticEntities.StaticEntities.LiquidLevels.FirstOrDefault(t => t.Number == number);
            if (lls == null)
                StaticEntities.StaticEntities.LiquidLevels.Add(new VesselLiquidLevelDto { Number = number });
            else
                StaticEntities.StaticEntities.LiquidLevels[StaticEntities.StaticEntities.LiquidLevels.IndexOf(lls)].LiquidLevelDtos.Clear();
            var sus = StaticEntities.StaticEntities.SupplyUnits.FirstOrDefault(t => t.Number == number);
            if (sus == null)
                StaticEntities.StaticEntities.SupplyUnits.Add(new VesselSupplyUnitDto { Number = number });
            else
                StaticEntities.StaticEntities.SupplyUnits[StaticEntities.StaticEntities.SupplyUnits.IndexOf(sus)].SupplyUnitDtos.Clear();
            var ss = StaticEntities.StaticEntities.Shafts.FirstOrDefault(t => t.Number == number);
            if (ss == null)
                StaticEntities.StaticEntities.Shafts.Add(new VesselShaftDto { Number = number });
            else
                StaticEntities.StaticEntities.Shafts[StaticEntities.StaticEntities.Shafts.IndexOf(ss)].ShaftDtos.Clear();
            var sss = StaticEntities.StaticEntities.SternSealings.FirstOrDefault(t => t.Number == number);
            if (sss == null)
                StaticEntities.StaticEntities.SternSealings.Add(new VesselSternSealingDto { Number = number });
            else
                StaticEntities.StaticEntities.SternSealings[StaticEntities.StaticEntities.SternSealings.IndexOf(sss)].SternSealingDtos.Clear();
            var pus = StaticEntities.StaticEntities.PowerUnits.FirstOrDefault(t => t.Number == number);
            if (pus == null)
                StaticEntities.StaticEntities.PowerUnits.Add(new VesselPowerUnitDto { Number = number });
            else
                StaticEntities.StaticEntities.PowerUnits[StaticEntities.StaticEntities.PowerUnits.IndexOf(pus)].PowerUnitDtos.Clear();

            var ti = StaticEntities.StaticEntities.TotalIndicators.FirstOrDefault(t => t.Number == number);
            if (ti == null)
                StaticEntities.StaticEntities.TotalIndicators.Add(new TotalIndicatorDto { Number = number, ReceiveDatetime = receiveDatetime });
            else
                StaticEntities.StaticEntities.TotalIndicators[StaticEntities.StaticEntities.TotalIndicators.IndexOf(ti)] = new TotalIndicatorDto { Number = number, ReceiveDatetime = receiveDatetime };

            var p = StaticEntities.StaticEntities.Predictions.FirstOrDefault(t => t.Number == number);
            if (p == null)
                StaticEntities.StaticEntities.Predictions.Add(new PredictionDto { Number = number, ReceiveDatetime = receiveDatetime });
            else
                StaticEntities.StaticEntities.Predictions[StaticEntities.StaticEntities.Predictions.IndexOf(p)] = new PredictionDto { Number = number, ReceiveDatetime = receiveDatetime };

            var dailyConsumption = StaticEntities.StaticEntities.DailyConsumptions.FirstOrDefault(t => t.Number == number);
            if (dailyConsumption == null)
                StaticEntities.StaticEntities.DailyConsumptions.Add(new DailyConsumption { Number = number });
            else
                StaticEntities.StaticEntities.DailyConsumptions[StaticEntities.StaticEntities.DailyConsumptions.IndexOf(dailyConsumption)] = new DailyConsumption { Number = number };

            var lstCompositeBoilers = StaticEntities.StaticEntities.CompositeBoilers.FirstOrDefault(t => t.Number == number);
            if (lstCompositeBoilers == null)
                StaticEntities.StaticEntities.CompositeBoilers.Add(new VesselCompositeBoilerDto { Number = number });
            else
                StaticEntities.StaticEntities.CompositeBoilers[StaticEntities.StaticEntities.CompositeBoilers.IndexOf(lstCompositeBoilers)].CompositeBoilerDtos.Clear();
            var lstCompressedAirSupplies = StaticEntities.StaticEntities.CompressedAirSupplies.FirstOrDefault(t => t.Number == number);
            if (lstCompressedAirSupplies == null)
                StaticEntities.StaticEntities.CompressedAirSupplies.Add(new VesselCompressedAirSupplyDto { Number = number });
            else
                StaticEntities.StaticEntities.CompressedAirSupplies[StaticEntities.StaticEntities.CompressedAirSupplies.IndexOf(lstCompressedAirSupplies)].CompressedAirSupplyDtos.Clear();
            var lstCoolingFreshWaters = StaticEntities.StaticEntities.CoolingFreshWaters.FirstOrDefault(t => t.Number == number);
            if (lstCoolingFreshWaters == null)
                StaticEntities.StaticEntities.CoolingFreshWaters.Add(new VesselCoolingFreshWaterDto { Number = number });
            else
                StaticEntities.StaticEntities.CoolingFreshWaters[StaticEntities.StaticEntities.CoolingFreshWaters.IndexOf(lstCoolingFreshWaters)].CoolingFreshWaterDtos.Clear();
            var lstCoolingSeaWaters = StaticEntities.StaticEntities.CoolingSeaWaters.FirstOrDefault(t => t.Number == number);
            if (lstCoolingSeaWaters == null)
                StaticEntities.StaticEntities.CoolingSeaWaters.Add(new VesselCoolingSeaWaterDto { Number = number });
            else
                StaticEntities.StaticEntities.CoolingSeaWaters[StaticEntities.StaticEntities.CoolingSeaWaters.IndexOf(lstCoolingSeaWaters)].CoolingSeaWaterDtos.Clear();
            var lstCoolingWaters = StaticEntities.StaticEntities.CoolingWaters.FirstOrDefault(t => t.Number == number);
            if (lstCoolingWaters == null)
                StaticEntities.StaticEntities.CoolingWaters.Add(new VesselCoolingWaterDto { Number = number });
            else
                StaticEntities.StaticEntities.CoolingWaters[StaticEntities.StaticEntities.CoolingWaters.IndexOf(lstCoolingWaters)].CoolingWaterDtos.Clear();
            var lstCylinderLubOils = StaticEntities.StaticEntities.CylinderLubOils.FirstOrDefault(t => t.Number == number);
            if (lstCylinderLubOils == null)
                StaticEntities.StaticEntities.CylinderLubOils.Add(new VesselCylinderLubOilDto { Number = number });
            else
                StaticEntities.StaticEntities.CylinderLubOils[StaticEntities.StaticEntities.CylinderLubOils.IndexOf(lstCylinderLubOils)].CylinderLubOilDtos.Clear();
            var lstExhaustGases = StaticEntities.StaticEntities.ExhaustGases.FirstOrDefault(t => t.Number == number);
            if (lstExhaustGases == null)
                StaticEntities.StaticEntities.ExhaustGases.Add(new VesselExhaustGasDto { Number = number });
            else
                StaticEntities.StaticEntities.ExhaustGases[StaticEntities.StaticEntities.ExhaustGases.IndexOf(lstExhaustGases)].ExhaustGasDtos.Clear();
            var lstFOs = StaticEntities.StaticEntities.FOs.FirstOrDefault(t => t.Number == number);
            if (lstFOs == null)
                StaticEntities.StaticEntities.FOs.Add(new VesselFODto { Number = number });
            else
                StaticEntities.StaticEntities.FOs[StaticEntities.StaticEntities.FOs.IndexOf(lstFOs)].FODtos.Clear();
            var lstFOSupplyUnits = StaticEntities.StaticEntities.FOSupplyUnits.FirstOrDefault(t => t.Number == number);
            if (lstFOSupplyUnits == null)
                StaticEntities.StaticEntities.FOSupplyUnits.Add(new VesselFOSupplyUnitDto { Number = number });
            else
                StaticEntities.StaticEntities.FOSupplyUnits[StaticEntities.StaticEntities.FOSupplyUnits.IndexOf(lstFOSupplyUnits)].FOSupplyUnitDtos.Clear();
            var lstLubOilPurifyings = StaticEntities.StaticEntities.LubOilPurifyings.FirstOrDefault(t => t.Number == number);
            if (lstLubOilPurifyings == null)
                StaticEntities.StaticEntities.LubOilPurifyings.Add(new VesselLubOilPurifyingDto { Number = number });
            else
                StaticEntities.StaticEntities.LubOilPurifyings[StaticEntities.StaticEntities.LubOilPurifyings.IndexOf(lstLubOilPurifyings)].LubOilPurifyingDtos.Clear();
            var lstLubOils = StaticEntities.StaticEntities.LubOils.FirstOrDefault(t => t.Number == number);
            if (lstLubOils == null)
                StaticEntities.StaticEntities.LubOils.Add(new VesselLubOilDto { Number = number });
            else
                StaticEntities.StaticEntities.LubOils[StaticEntities.StaticEntities.LubOils.IndexOf(lstLubOils)].LubOilDtos.Clear();
            var lstMainGeneratorSets = StaticEntities.StaticEntities.MainGeneratorSets.FirstOrDefault(t => t.Number == number);
            if (lstMainGeneratorSets == null)
                StaticEntities.StaticEntities.MainGeneratorSets.Add(new VesselMainGeneratorSetDto { Number = number });
            else
                StaticEntities.StaticEntities.MainGeneratorSets[StaticEntities.StaticEntities.MainGeneratorSets.IndexOf(lstMainGeneratorSets)].MainGeneratorSetDtos.Clear();
            var lstMainSwitchboards = StaticEntities.StaticEntities.MainSwitchboards.FirstOrDefault(t => t.Number == number);
            if (lstMainSwitchboards == null)
                StaticEntities.StaticEntities.MainSwitchboards.Add(new VesselMainSwitchboardDto { Number = number });
            else
                StaticEntities.StaticEntities.MainSwitchboards[StaticEntities.StaticEntities.MainSwitchboards.IndexOf(lstMainSwitchboards)].MainSwitchboardDtos.Clear();
            var lstMERemoteControls = StaticEntities.StaticEntities.MERemoteControls.FirstOrDefault(t => t.Number == number);
            if (lstMERemoteControls == null)
                StaticEntities.StaticEntities.MERemoteControls.Add(new VesselMERemoteControlDto { Number = number });
            else
                StaticEntities.StaticEntities.MERemoteControls[StaticEntities.StaticEntities.MERemoteControls.IndexOf(lstMERemoteControls)].MERemoteControlDtos.Clear();
            var lstMiscellaneouses = StaticEntities.StaticEntities.Miscellaneouses.FirstOrDefault(t => t.Number == number);
            if (lstMiscellaneouses == null)
                StaticEntities.StaticEntities.Miscellaneouses.Add(new VesselMiscellaneousDto { Number = number });
            else
                StaticEntities.StaticEntities.Miscellaneouses[StaticEntities.StaticEntities.Miscellaneouses.IndexOf(lstMiscellaneouses)].MiscellaneousDtos.Clear();
            var lstScavengeAirs = StaticEntities.StaticEntities.ScavengeAirs.FirstOrDefault(t => t.Number == number);
            if (lstScavengeAirs == null)
                StaticEntities.StaticEntities.ScavengeAirs.Add(new VesselScavengeAirDto { Number = number });
            else
                StaticEntities.StaticEntities.ScavengeAirs[StaticEntities.StaticEntities.ScavengeAirs.IndexOf(lstScavengeAirs)].ScavengeAirDtos.Clear();
            var lstShaftClutchs = StaticEntities.StaticEntities.ShaftClutchs.FirstOrDefault(t => t.Number == number);
            if (lstShaftClutchs == null)
                StaticEntities.StaticEntities.ShaftClutchs.Add(new VesselShaftClutchDto { Number = number });
            else
                StaticEntities.StaticEntities.ShaftClutchs[StaticEntities.StaticEntities.ShaftClutchs.IndexOf(lstShaftClutchs)].ShaftClutchDtos.Clear();
            var lstAssistantDecisions = StaticEntities.StaticEntities.AssistantDecisions.FirstOrDefault(t => t.Number == number);
            if (lstAssistantDecisions == null)
                StaticEntities.StaticEntities.AssistantDecisions.Add(new VesselAssistantDecisionDto { Number = number });
            else
                StaticEntities.StaticEntities.AssistantDecisions[StaticEntities.StaticEntities.AssistantDecisions.IndexOf(lstAssistantDecisions)].AssistantDecisionDtos.Clear();

            #endregion 当前实体

            /*#region 显示实体

            StaticEntities.ShowEntities.Vessels.Add(new VesselInfoDto { SN = number, ReceiveDatetime = receiveDatetime });

            var fmsShow = StaticEntities.ShowEntities.Flowmeters.FirstOrDefault(t => t.Number == number);
            if (fmsShow == null)
                StaticEntities.ShowEntities.Flowmeters.Add(new VesselFlowmeterDto { Number = number });
            else
                StaticEntities.ShowEntities.Flowmeters[StaticEntities.ShowEntities.Flowmeters.IndexOf(fmsShow)].FlowmeterDtos.Clear();
            var bsShow = StaticEntities.ShowEntities.Batteries.FirstOrDefault(t => t.Number == number);
            if (bsShow == null)
                StaticEntities.ShowEntities.Batteries.Add(new VesselBatteryDto { Number = number });
            else
                StaticEntities.ShowEntities.Batteries[StaticEntities.ShowEntities.Batteries.IndexOf(bsShow)].BatteryDtos.Clear();
            var gsShow = StaticEntities.ShowEntities.Generators.FirstOrDefault(t => t.Number == number);
            if (gsShow == null)
                StaticEntities.ShowEntities.Generators.Add(new VesselGeneratorDto { Number = number });
            else
                StaticEntities.ShowEntities.Generators[StaticEntities.ShowEntities.Generators.IndexOf(gsShow)].GeneratorDtos.Clear();
            var llsShow = StaticEntities.ShowEntities.LiquidLevels.FirstOrDefault(t => t.Number == number);
            if (llsShow == null)
                StaticEntities.ShowEntities.LiquidLevels.Add(new VesselLiquidLevelDto { Number = number });
            else
                StaticEntities.ShowEntities.LiquidLevels[StaticEntities.ShowEntities.LiquidLevels.IndexOf(llsShow)].LiquidLevelDtos.Clear();
            var susShow = StaticEntities.ShowEntities.SupplyUnits.FirstOrDefault(t => t.Number == number);
            if (susShow == null)
                StaticEntities.ShowEntities.SupplyUnits.Add(new VesselSupplyUnitDto { Number = number });
            else
                StaticEntities.ShowEntities.SupplyUnits[StaticEntities.ShowEntities.SupplyUnits.IndexOf(susShow)].SupplyUnitDtos.Clear();
            var ssShow = StaticEntities.ShowEntities.Shafts.FirstOrDefault(t => t.Number == number);
            if (ssShow == null)
                StaticEntities.ShowEntities.Shafts.Add(new VesselShaftDto { Number = number });
            else
                StaticEntities.ShowEntities.Shafts[StaticEntities.ShowEntities.Shafts.IndexOf(ssShow)].ShaftDtos.Clear();
            var sssShow = StaticEntities.ShowEntities.SternSealings.FirstOrDefault(t => t.Number == number);
            if (sssShow == null)
                StaticEntities.ShowEntities.SternSealings.Add(new VesselSternSealingDto { Number = number });
            else
                StaticEntities.ShowEntities.SternSealings[StaticEntities.ShowEntities.SternSealings.IndexOf(sssShow)].SternSealingDtos.Clear();
            var pusShow = StaticEntities.ShowEntities.PowerUnits.FirstOrDefault(t => t.Number == number);
            if (pusShow == null)
                StaticEntities.ShowEntities.PowerUnits.Add(new VesselPowerUnitDto { Number = number });
            else
                StaticEntities.ShowEntities.PowerUnits[StaticEntities.ShowEntities.PowerUnits.IndexOf(pusShow)].PowerUnitDtos.Clear();

            var tiShow = StaticEntities.ShowEntities.TotalIndicators.FirstOrDefault(t => t.Number == number);
            if (tiShow == null)
                StaticEntities.ShowEntities.TotalIndicators.Add(new TotalIndicatorDto { Number = number, ReceiveDatetime = receiveDatetime });
            else
                StaticEntities.ShowEntities.TotalIndicators[StaticEntities.ShowEntities.TotalIndicators.IndexOf(tiShow)] = new TotalIndicatorDto { Number = number, ReceiveDatetime = receiveDatetime };

            var pShow = StaticEntities.ShowEntities.Predictions.FirstOrDefault(t => t.Number == number);
            if (pShow == null)
                StaticEntities.ShowEntities.Predictions.Add(new PredictionDto { Number = number, ReceiveDatetime = receiveDatetime });
            else
                StaticEntities.ShowEntities.Predictions[StaticEntities.ShowEntities.Predictions.IndexOf(pShow)] = new PredictionDto { Number = number, ReceiveDatetime = receiveDatetime };

            var lstCompositeBoilersShow = StaticEntities.ShowEntities.CompositeBoilers.FirstOrDefault(t => t.Number == number);
            if (lstCompositeBoilersShow == null)
                StaticEntities.ShowEntities.CompositeBoilers.Add(new VesselCompositeBoilerDto { Number = number });
            else
                StaticEntities.ShowEntities.CompositeBoilers[StaticEntities.ShowEntities.CompositeBoilers.IndexOf(lstCompositeBoilers)].CompositeBoilerDtos.Clear();
            var lstCompressedAirSuppliesShow = StaticEntities.ShowEntities.CompressedAirSupplies.FirstOrDefault(t => t.Number == number);
            if (lstCompressedAirSuppliesShow == null)
                StaticEntities.ShowEntities.CompressedAirSupplies.Add(new VesselCompressedAirSupplyDto { Number = number });
            else
                StaticEntities.ShowEntities.CompressedAirSupplies[StaticEntities.ShowEntities.CompressedAirSupplies.IndexOf(lstCompressedAirSuppliesShow)].CompressedAirSupplyDtos.Clear();
            var lstCoolingFreshWatersShow = StaticEntities.ShowEntities.CoolingFreshWaters.FirstOrDefault(t => t.Number == number);
            if (lstCoolingFreshWatersShow == null)
                StaticEntities.ShowEntities.CoolingFreshWaters.Add(new VesselCoolingFreshWaterDto { Number = number });
            else
                StaticEntities.ShowEntities.CoolingFreshWaters[StaticEntities.ShowEntities.CoolingFreshWaters.IndexOf(lstCoolingFreshWatersShow)].CoolingFreshWaterDtos.Clear();
            var lstCoolingSeaWatersShow = StaticEntities.ShowEntities.CoolingSeaWaters.FirstOrDefault(t => t.Number == number);
            if (lstCoolingSeaWatersShow == null)
                StaticEntities.ShowEntities.CoolingSeaWaters.Add(new VesselCoolingSeaWaterDto { Number = number });
            else
                StaticEntities.ShowEntities.CoolingSeaWaters[StaticEntities.ShowEntities.CoolingSeaWaters.IndexOf(lstCoolingSeaWatersShow)].CoolingSeaWaterDtos.Clear();
            var lstCoolingWatersShow = StaticEntities.ShowEntities.CoolingWaters.FirstOrDefault(t => t.Number == number);
            if (lstCoolingWatersShow == null)
                StaticEntities.ShowEntities.CoolingWaters.Add(new VesselCoolingWaterDto { Number = number });
            else
                StaticEntities.ShowEntities.CoolingWaters[StaticEntities.ShowEntities.CoolingWaters.IndexOf(lstCoolingWatersShow)].CoolingWaterDtos.Clear();
            var lstCylinderLubOilsShow = StaticEntities.ShowEntities.CylinderLubOils.FirstOrDefault(t => t.Number == number);
            if (lstCylinderLubOilsShow == null)
                StaticEntities.ShowEntities.CylinderLubOils.Add(new VesselCylinderLubOilDto { Number = number });
            else
                StaticEntities.ShowEntities.CylinderLubOils[StaticEntities.ShowEntities.CylinderLubOils.IndexOf(lstCylinderLubOilsShow)].CylinderLubOilDtos.Clear();
            var lstExhaustGasesShow = StaticEntities.ShowEntities.ExhaustGases.FirstOrDefault(t => t.Number == number);
            if (lstExhaustGasesShow == null)
                StaticEntities.ShowEntities.ExhaustGases.Add(new VesselExhaustGasDto { Number = number });
            else
                StaticEntities.ShowEntities.ExhaustGases[StaticEntities.ShowEntities.ExhaustGases.IndexOf(lstExhaustGasesShow)].ExhaustGasDtos.Clear();
            var lstFOsShow = StaticEntities.ShowEntities.FOs.FirstOrDefault(t => t.Number == number);
            if (lstFOsShow == null)
                StaticEntities.ShowEntities.FOs.Add(new VesselFODto { Number = number });
            else
                StaticEntities.ShowEntities.FOs[StaticEntities.ShowEntities.FOs.IndexOf(lstFOsShow)].FODtos.Clear();
            var lstFOSupplyUnitsShow = StaticEntities.ShowEntities.FOSupplyUnits.FirstOrDefault(t => t.Number == number);
            if (lstFOSupplyUnitsShow == null)
                StaticEntities.ShowEntities.FOSupplyUnits.Add(new VesselFOSupplyUnitDto { Number = number });
            else
                StaticEntities.ShowEntities.FOSupplyUnits[StaticEntities.ShowEntities.FOSupplyUnits.IndexOf(lstFOSupplyUnitsShow)].FOSupplyUnitDtos.Clear();
            var lstLubOilPurifyingsShow = StaticEntities.ShowEntities.LubOilPurifyings.FirstOrDefault(t => t.Number == number);
            if (lstLubOilPurifyingsShow == null)
                StaticEntities.ShowEntities.LubOilPurifyings.Add(new VesselLubOilPurifyingDto { Number = number });
            else
                StaticEntities.ShowEntities.LubOilPurifyings[StaticEntities.ShowEntities.LubOilPurifyings.IndexOf(lstLubOilPurifyingsShow)].LubOilPurifyingDtos.Clear();
            var lstLubOilsShow = StaticEntities.ShowEntities.LubOils.FirstOrDefault(t => t.Number == number);
            if (lstLubOilsShow == null)
                StaticEntities.ShowEntities.LubOils.Add(new VesselLubOilDto { Number = number });
            else
                StaticEntities.ShowEntities.LubOils[StaticEntities.ShowEntities.LubOils.IndexOf(lstLubOilsShow)].LubOilDtos.Clear();
            var lstMainGeneratorSetsShow = StaticEntities.ShowEntities.MainGeneratorSets.FirstOrDefault(t => t.Number == number);
            if (lstMainGeneratorSetsShow == null)
                StaticEntities.ShowEntities.MainGeneratorSets.Add(new VesselMainGeneratorSetDto { Number = number });
            else
                StaticEntities.ShowEntities.MainGeneratorSets[StaticEntities.ShowEntities.MainGeneratorSets.IndexOf(lstMainGeneratorSetsShow)].MainGeneratorSetDtos.Clear();
            var lstMainSwitchboardsShow = StaticEntities.ShowEntities.MainSwitchboards.FirstOrDefault(t => t.Number == number);
            if (lstMainSwitchboardsShow == null)
                StaticEntities.ShowEntities.MainSwitchboards.Add(new VesselMainSwitchboardDto { Number = number });
            else
                StaticEntities.ShowEntities.MainSwitchboards[StaticEntities.ShowEntities.MainSwitchboards.IndexOf(lstMainSwitchboardsShow)].MainSwitchboardDtos.Clear();
            var lstMERemoteControlsShow = StaticEntities.ShowEntities.MERemoteControls.FirstOrDefault(t => t.Number == number);
            if (lstMERemoteControlsShow == null)
                StaticEntities.ShowEntities.MERemoteControls.Add(new VesselMERemoteControlDto { Number = number });
            else
                StaticEntities.ShowEntities.MERemoteControls[StaticEntities.ShowEntities.MERemoteControls.IndexOf(lstMERemoteControlsShow)].MERemoteControlDtos.Clear();
            var lstMiscellaneousesShow = StaticEntities.ShowEntities.Miscellaneouses.FirstOrDefault(t => t.Number == number);
            if (lstMiscellaneousesShow == null)
                StaticEntities.ShowEntities.Miscellaneouses.Add(new VesselMiscellaneousDto { Number = number });
            else
                StaticEntities.ShowEntities.Miscellaneouses[StaticEntities.ShowEntities.Miscellaneouses.IndexOf(lstMiscellaneousesShow)].MiscellaneousDtos.Clear();
            var lstScavengeAirsShow = StaticEntities.ShowEntities.ScavengeAirs.FirstOrDefault(t => t.Number == number);
            if (lstScavengeAirsShow == null)
                StaticEntities.ShowEntities.ScavengeAirs.Add(new VesselScavengeAirDto { Number = number });
            else
                StaticEntities.ShowEntities.ScavengeAirs[StaticEntities.ShowEntities.ScavengeAirs.IndexOf(lstScavengeAirsShow)].ScavengeAirDtos.Clear();
            var lstShaftClutchsShow = StaticEntities.ShowEntities.ShaftClutchs.FirstOrDefault(t => t.Number == number);
            if (lstShaftClutchsShow == null)
                StaticEntities.ShowEntities.ShaftClutchs.Add(new VesselShaftClutchDto { Number = number });
            else
                StaticEntities.ShowEntities.ShaftClutchs[StaticEntities.ShowEntities.ShaftClutchs.IndexOf(lstShaftClutchsShow)].ShaftClutchDtos.Clear();
            var lstAssistantDecisionsShow = StaticEntities.ShowEntities.AssistantDecisions.FirstOrDefault(t => t.Number == number);
            if (lstAssistantDecisionsShow == null)
                StaticEntities.ShowEntities.AssistantDecisions.Add(new VesselAssistantDecisionDto { Number = number });
            else
                StaticEntities.ShowEntities.AssistantDecisions[StaticEntities.ShowEntities.AssistantDecisions.IndexOf(lstAssistantDecisionsShow)].AssistantDecisionDtos.Clear();

            #endregion 显示实体*/

            _logger.LogWarning("初始化当前船舶3");
            #region 设备监测

            var monitoredDevices = new Dictionary<string, DateTime>();
            foreach (var config in StaticEntities.StaticEntities.Configs.Where(t => t.Number == number && t.IsDevice == 1))
            {
                monitoredDevices.Add(config.Code, DateTime.UtcNow);
            }
            if (StaticEntities.StaticEntities.MonitoredDevices.Any(t => t.Number == number))
            {
                StaticEntities.StaticEntities.MonitoredDevices[StaticEntities.StaticEntities.MonitoredDevices.IndexOf(StaticEntities.StaticEntities.MonitoredDevices.FirstOrDefault(t => t.Number == number))].Devices = monitoredDevices;
            }
            else
            {
                StaticEntities.StaticEntities.MonitoredDevices.Add(new MonitoredDevice { Number = number, Devices = monitoredDevices });
            }

            #endregion 设备监测

            _logger.LogWarning("初始化当前船舶完成");
        }

        /// <summary>
        /// 更新船舶所有属性
        /// </summary>
        /// <param name="number"></param>
        /// <param name="receiveDatetime"></param>
        private void UpdateVesselAll(string number, DateTime receiveDatetime)
        {
            var vesselEntity = StaticEntities.StaticEntities.Vessels[StaticEntities.StaticEntities.Vessels.IndexOf(StaticEntities.StaticEntities.Vessels.FirstOrDefault(t => t.SN == number))];
            vesselEntity.ReceiveDatetime = receiveDatetime;

            var fms = StaticEntities.StaticEntities.Flowmeters[StaticEntities.StaticEntities.Flowmeters.IndexOf(StaticEntities.StaticEntities.Flowmeters.FirstOrDefault(t => t.Number == number))].FlowmeterDtos;
            foreach (var dto in fms) { dto.ReceiveDatetime = receiveDatetime; }
            var bs = StaticEntities.StaticEntities.Batteries[StaticEntities.StaticEntities.Batteries.IndexOf(StaticEntities.StaticEntities.Batteries.FirstOrDefault(t => t.Number == number))].BatteryDtos;
            foreach (var dto in bs) { dto.ReceiveDatetime = receiveDatetime; }
            var gs = StaticEntities.StaticEntities.Generators[StaticEntities.StaticEntities.Generators.IndexOf(StaticEntities.StaticEntities.Generators.FirstOrDefault(t => t.Number == number))].GeneratorDtos;
            foreach (var dto in gs) { dto.ReceiveDatetime = receiveDatetime; }
            var lls = StaticEntities.StaticEntities.LiquidLevels[StaticEntities.StaticEntities.LiquidLevels.IndexOf(StaticEntities.StaticEntities.LiquidLevels.FirstOrDefault(t => t.Number == number))].LiquidLevelDtos;
            foreach (var dto in lls) { dto.ReceiveDatetime = receiveDatetime; }
            var sus = StaticEntities.StaticEntities.SupplyUnits[StaticEntities.StaticEntities.SupplyUnits.IndexOf(StaticEntities.StaticEntities.SupplyUnits.FirstOrDefault(t => t.Number == number))].SupplyUnitDtos;
            foreach (var dto in sus) { dto.ReceiveDatetime = receiveDatetime; }
            var ss = StaticEntities.StaticEntities.Shafts[StaticEntities.StaticEntities.Shafts.IndexOf(StaticEntities.StaticEntities.Shafts.FirstOrDefault(t => t.Number == number))].ShaftDtos;
            foreach (var dto in ss) { dto.ReceiveDatetime = receiveDatetime; }
            var sss = StaticEntities.StaticEntities.SternSealings[StaticEntities.StaticEntities.SternSealings.IndexOf(StaticEntities.StaticEntities.SternSealings.FirstOrDefault(t => t.Number == number))].SternSealingDtos;
            foreach (var dto in sss) { dto.ReceiveDatetime = receiveDatetime; }
            var pus = StaticEntities.StaticEntities.PowerUnits[StaticEntities.StaticEntities.PowerUnits.IndexOf(StaticEntities.StaticEntities.PowerUnits.FirstOrDefault(t => t.Number == number))].PowerUnitDtos;
            foreach (var dto in pus) { dto.ReceiveDatetime = receiveDatetime; }

            var ti = StaticEntities.StaticEntities.TotalIndicators[StaticEntities.StaticEntities.TotalIndicators.IndexOf(StaticEntities.StaticEntities.TotalIndicators.FirstOrDefault(t => t.Number == number))];
            ti.ReceiveDatetime = receiveDatetime;

            var p = StaticEntities.StaticEntities.Predictions[StaticEntities.StaticEntities.Predictions.IndexOf(StaticEntities.StaticEntities.Predictions.FirstOrDefault(t => t.Number == number))];
            p.ReceiveDatetime = receiveDatetime;

            var CompositeBoilers = StaticEntities.StaticEntities.CompositeBoilers[StaticEntities.StaticEntities.CompositeBoilers.IndexOf(StaticEntities.StaticEntities.CompositeBoilers.FirstOrDefault(t => t.Number == number))].CompositeBoilerDtos;
            foreach (var dto in CompositeBoilers) { dto.ReceiveDatetime = receiveDatetime; }
            var CompressedAirSupplies = StaticEntities.StaticEntities.CompressedAirSupplies[StaticEntities.StaticEntities.CompressedAirSupplies.IndexOf(StaticEntities.StaticEntities.CompressedAirSupplies.FirstOrDefault(t => t.Number == number))].CompressedAirSupplyDtos;
            foreach (var dto in CompressedAirSupplies) { dto.ReceiveDatetime = receiveDatetime; }
            var CoolingFreshWaters = StaticEntities.StaticEntities.CoolingFreshWaters[StaticEntities.StaticEntities.CoolingFreshWaters.IndexOf(StaticEntities.StaticEntities.CoolingFreshWaters.FirstOrDefault(t => t.Number == number))].CoolingFreshWaterDtos;
            foreach (var dto in CoolingFreshWaters) { dto.ReceiveDatetime = receiveDatetime; }
            var CoolingSeaWaters = StaticEntities.StaticEntities.CoolingSeaWaters[StaticEntities.StaticEntities.CoolingSeaWaters.IndexOf(StaticEntities.StaticEntities.CoolingSeaWaters.FirstOrDefault(t => t.Number == number))].CoolingSeaWaterDtos;
            foreach (var dto in CoolingSeaWaters) { dto.ReceiveDatetime = receiveDatetime; }
            var CoolingWaters = StaticEntities.StaticEntities.CoolingWaters[StaticEntities.StaticEntities.CoolingWaters.IndexOf(StaticEntities.StaticEntities.CoolingWaters.FirstOrDefault(t => t.Number == number))].CoolingWaterDtos;
            foreach (var dto in CoolingWaters) { dto.ReceiveDatetime = receiveDatetime; }
            var CylinderLubOils = StaticEntities.StaticEntities.CylinderLubOils[StaticEntities.StaticEntities.CylinderLubOils.IndexOf(StaticEntities.StaticEntities.CylinderLubOils.FirstOrDefault(t => t.Number == number))].CylinderLubOilDtos;
            foreach (var dto in CylinderLubOils) { dto.ReceiveDatetime = receiveDatetime; }
            var ExhaustGases = StaticEntities.StaticEntities.ExhaustGases[StaticEntities.StaticEntities.ExhaustGases.IndexOf(StaticEntities.StaticEntities.ExhaustGases.FirstOrDefault(t => t.Number == number))].ExhaustGasDtos;
            foreach (var dto in ExhaustGases) { dto.ReceiveDatetime = receiveDatetime; }
            var FOs = StaticEntities.StaticEntities.FOs[StaticEntities.StaticEntities.FOs.IndexOf(StaticEntities.StaticEntities.FOs.FirstOrDefault(t => t.Number == number))].FODtos;
            foreach (var dto in FOs) { dto.ReceiveDatetime = receiveDatetime; }
            var FOSupplyUnits = StaticEntities.StaticEntities.FOSupplyUnits[StaticEntities.StaticEntities.FOSupplyUnits.IndexOf(StaticEntities.StaticEntities.FOSupplyUnits.FirstOrDefault(t => t.Number == number))].FOSupplyUnitDtos;
            foreach (var dto in FOSupplyUnits) { dto.ReceiveDatetime = receiveDatetime; }
            var LubOilPurifyings = StaticEntities.StaticEntities.LubOilPurifyings[StaticEntities.StaticEntities.LubOilPurifyings.IndexOf(StaticEntities.StaticEntities.LubOilPurifyings.FirstOrDefault(t => t.Number == number))].LubOilPurifyingDtos;
            foreach (var dto in LubOilPurifyings) { dto.ReceiveDatetime = receiveDatetime; }
            var LubOils = StaticEntities.StaticEntities.LubOils[StaticEntities.StaticEntities.LubOils.IndexOf(StaticEntities.StaticEntities.LubOils.FirstOrDefault(t => t.Number == number))].LubOilDtos;
            foreach (var dto in LubOils) { dto.ReceiveDatetime = receiveDatetime; }
            var MainGeneratorSets = StaticEntities.StaticEntities.MainGeneratorSets[StaticEntities.StaticEntities.MainGeneratorSets.IndexOf(StaticEntities.StaticEntities.MainGeneratorSets.FirstOrDefault(t => t.Number == number))].MainGeneratorSetDtos;
            foreach (var dto in MainGeneratorSets) { dto.ReceiveDatetime = receiveDatetime; }
            var MainSwitchboards = StaticEntities.StaticEntities.MainSwitchboards[StaticEntities.StaticEntities.MainSwitchboards.IndexOf(StaticEntities.StaticEntities.MainSwitchboards.FirstOrDefault(t => t.Number == number))].MainSwitchboardDtos;
            foreach (var dto in MainSwitchboards) { dto.ReceiveDatetime = receiveDatetime; }
            var MERemoteControls = StaticEntities.StaticEntities.MERemoteControls[StaticEntities.StaticEntities.MERemoteControls.IndexOf(StaticEntities.StaticEntities.MERemoteControls.FirstOrDefault(t => t.Number == number))].MERemoteControlDtos;
            foreach (var dto in MERemoteControls) { dto.ReceiveDatetime = receiveDatetime; }
            var Miscellaneouses = StaticEntities.StaticEntities.Miscellaneouses[StaticEntities.StaticEntities.Miscellaneouses.IndexOf(StaticEntities.StaticEntities.Miscellaneouses.FirstOrDefault(t => t.Number == number))].MiscellaneousDtos;
            foreach (var dto in Miscellaneouses) { dto.ReceiveDatetime = receiveDatetime; }
            var ScavengeAirs = StaticEntities.StaticEntities.ScavengeAirs[StaticEntities.StaticEntities.ScavengeAirs.IndexOf(StaticEntities.StaticEntities.ScavengeAirs.FirstOrDefault(t => t.Number == number))].ScavengeAirDtos;
            foreach (var dto in ScavengeAirs) { dto.ReceiveDatetime = receiveDatetime; }
            var ShaftClutchs = StaticEntities.StaticEntities.ShaftClutchs[StaticEntities.StaticEntities.ShaftClutchs.IndexOf(StaticEntities.StaticEntities.ShaftClutchs.FirstOrDefault(t => t.Number == number))].ShaftClutchDtos;
            foreach (var dto in ShaftClutchs) { dto.ReceiveDatetime = receiveDatetime; }
        }

        /// <summary>
        /// 船舶实时数据加工
        /// </summary>
        /// <param name="number"></param>
        /// <param name="bsi"></param>
        private async Task<IList<LogBook>> CalcVesselInfo(string number, BaseShipInfo bsi)
        {
            var result = new List<LogBook>();

            var vessel = StaticEntities.StaticEntities.Vessels.FirstOrDefault(t => t.SN == number);
            var indexVessel = StaticEntities.StaticEntities.Vessels.IndexOf(vessel);

            var flowmeters = StaticEntities.StaticEntities.Flowmeters.FirstOrDefault(t => t.Number == number).FlowmeterDtos;
            var shafts = StaticEntities.StaticEntities.Shafts.FirstOrDefault(t => t.Number == number).ShaftDtos;

            var lstPu = StaticEntities.StaticEntities.PowerUnits[StaticEntities.StaticEntities.PowerUnits.IndexOf(StaticEntities.StaticEntities.PowerUnits.FirstOrDefault(t => t.Number == number))].PowerUnitDtos;
            lstPu.Clear();

            var indexTotalIndicator = StaticEntities.StaticEntities.TotalIndicators.IndexOf(StaticEntities.StaticEntities.TotalIndicators.FirstOrDefault(t => t.Number == number));
            var totalIndicator = new TotalIndicatorDto { Number = number, ReceiveDatetime = vessel.ReceiveDatetime };

            var indexPredictions = StaticEntities.StaticEntities.Predictions.IndexOf(StaticEntities.StaticEntities.Predictions.FirstOrDefault(t => t.Number == number));
            var predictions = new PredictionDto { Number = number, ReceiveDatetime = vessel.ReceiveDatetime };

            //流量计合计数据
            foreach (var flowmeter in flowmeters)
            {
                /*#region Emarat
                if (!lstPu.Any(t => t.DeviceType == flowmeter.DeviceType))
                    lstPu.Add(new PowerUnitDto { Number = number, ReceiveDatetime = vessel.ReceiveDatetime, DeviceType = flowmeter.DeviceType });
                var pu = lstPu[lstPu.IndexOf(lstPu.FirstOrDefault(t => t.DeviceType == flowmeter.DeviceType))];

                switch (flowmeter.FuelType)
                {
                    case "HFO":
                        pu.HFO = flowmeter.ConsAct;
                        pu.HFOAccumulated = flowmeter.ConsAcc;

                        totalIndicator.HFO = (totalIndicator.HFO ?? 0) + flowmeter.ConsAct;
                        totalIndicator.HFOAccumulated = (totalIndicator.HFOAccumulated ?? 0) + flowmeter.ConsAcc;
                        break;

                    case "DGO":
                        pu.DGO = flowmeter.ConsAct;
                        pu.DGOAccumulated = flowmeter.ConsAcc;

                        totalIndicator.DGO = (totalIndicator.DGO ?? 0) + flowmeter.ConsAct;
                        totalIndicator.DGOAccumulated = (totalIndicator.DGOAccumulated ?? 0) + flowmeter.ConsAcc;
                        break;
                }

                #endregion Emarat*/

                /*#region 迪拜

                var deviceType = "me";
                if (flowmeter.DeviceNo == "me_fuel_in_1")
                    deviceType = "me";
                else if (flowmeter.DeviceNo == "me_fuel_out_1" || flowmeter.DeviceNo == "me_methanol")
                    deviceType = "blr";
                else if (flowmeter.DeviceNo == "me_fuel_in_2" || flowmeter.DeviceNo == "me_fuel_out_2")
                    deviceType = "ae";

                if (!lstPu.Any(t => t.DeviceType == deviceType))
                    lstPu.Add(new PowerUnitDto { Number = number, ReceiveDatetime = vessel.ReceiveDatetime, DeviceType = deviceType });
                var pu = lstPu[lstPu.IndexOf(lstPu.FirstOrDefault(t => t.DeviceType == deviceType))];

                switch (bsi.CurrentFuelType)
                {
                    case "DGO":
                        if (deviceType == "me")
                        {
                            pu.DGO = flowmeter.ConsAct;
                            pu.DGOAccumulated = flowmeter.ConsAcc;

                            totalIndicator.DGO = (totalIndicator.DGO ?? 0) + flowmeter.ConsAct;
                            totalIndicator.DGOAccumulated = (totalIndicator.DGOAccumulated ?? 0) + flowmeter.ConsAcc;
                        }
                        else if (deviceType == "blr")
                        {
                            pu.DGO = (pu.DGO ?? 0) + flowmeter.ConsAct;
                            pu.DGOAccumulated = (pu.DGOAccumulated ?? 0) + flowmeter.ConsAcc;

                            totalIndicator.DGO = (totalIndicator.DGO ?? 0) + flowmeter.ConsAct;
                            totalIndicator.DGOAccumulated = (totalIndicator.DGOAccumulated ?? 0) + flowmeter.ConsAcc;
                        }
                        else if (deviceType == "ae")
                        {
                            if (flowmeter.DeviceNo == "me_fuel_in_2")
                            {
                                pu.DGO = (pu.DGO ?? 0) + flowmeter.ConsAct;
                                pu.DGOAccumulated = (pu.DGOAccumulated ?? 0) + flowmeter.ConsAcc;

                                totalIndicator.DGO = (totalIndicator.DGO ?? 0) + flowmeter.ConsAct;
                                totalIndicator.DGOAccumulated = (totalIndicator.DGOAccumulated ?? 0) + flowmeter.ConsAcc;
                            }
                            else if (flowmeter.DeviceNo == "me_fuel_out_2")
                            {
                                pu.DGO = (pu.DGO ?? 0) - flowmeter.ConsAct;
                                pu.DGOAccumulated = (pu.DGOAccumulated ?? 0) - flowmeter.ConsAcc;

                                totalIndicator.DGO = (totalIndicator.DGO ?? 0) - flowmeter.ConsAct;
                                totalIndicator.DGOAccumulated = (totalIndicator.DGOAccumulated ?? 0) - flowmeter.ConsAcc;
                            }
                            else
                            {
                                pu.DGO = (pu.DGO ?? 0) + flowmeter.ConsAct;
                                pu.DGOAccumulated = (pu.DGOAccumulated ?? 0) + flowmeter.ConsAcc;

                                totalIndicator.DGO = (totalIndicator.DGO ?? 0) + flowmeter.ConsAct;
                                totalIndicator.DGOAccumulated = (totalIndicator.DGOAccumulated ?? 0) + flowmeter.ConsAcc;
                            }
                        }
                        else
                        {
                            pu.DGO = flowmeter.ConsAct;
                            pu.DGOAccumulated = flowmeter.ConsAcc;

                            totalIndicator.DGO = (totalIndicator.DGO ?? 0) + flowmeter.ConsAct;
                            totalIndicator.DGOAccumulated = (totalIndicator.DGOAccumulated ?? 0) + flowmeter.ConsAcc;
                        }
                        break;

                    case "LFO":
                        if (deviceType == "me")
                        {
                            pu.LFO = flowmeter.ConsAct;
                            pu.LFOAccumulated = flowmeter.ConsAcc;

                            totalIndicator.LFO = (totalIndicator.LFO ?? 0) + flowmeter.ConsAct;
                            totalIndicator.LFOAccumulated = (totalIndicator.LFOAccumulated ?? 0) + flowmeter.ConsAcc;
                        }
                        else if (deviceType == "blr")
                        {
                            pu.LFO = (pu.LFO ?? 0) + flowmeter.ConsAct;
                            pu.LFOAccumulated = (pu.LFOAccumulated ?? 0) + flowmeter.ConsAcc;

                            totalIndicator.LFO = (totalIndicator.LFO ?? 0) + flowmeter.ConsAct;
                            totalIndicator.LFOAccumulated = (totalIndicator.LFOAccumulated ?? 0) + flowmeter.ConsAcc;
                        }
                        else if (deviceType == "ae")
                        {
                            if (flowmeter.DeviceNo == "me_fuel_in_2")
                            {
                                pu.LFO = (pu.LFO ?? 0) + flowmeter.ConsAct;
                                pu.LFOAccumulated = (pu.LFOAccumulated ?? 0) + flowmeter.ConsAcc;

                                totalIndicator.LFO = (totalIndicator.LFO ?? 0) + flowmeter.ConsAct;
                                totalIndicator.LFOAccumulated = (totalIndicator.LFOAccumulated ?? 0) + flowmeter.ConsAcc;
                            }
                            else if (flowmeter.DeviceNo == "me_fuel_out_2")
                            {
                                pu.LFO = (pu.LFO ?? 0) - flowmeter.ConsAct;
                                pu.LFOAccumulated = (pu.LFOAccumulated ?? 0) - flowmeter.ConsAcc;

                                totalIndicator.LFO = (totalIndicator.LFO ?? 0) - flowmeter.ConsAct;
                                totalIndicator.LFOAccumulated = (totalIndicator.LFOAccumulated ?? 0) - flowmeter.ConsAcc;
                            }
                            else
                            {
                                pu.LFO = (pu.LFO ?? 0) + flowmeter.ConsAct;
                                pu.LFOAccumulated = (pu.LFOAccumulated ?? 0) + flowmeter.ConsAcc;

                                totalIndicator.LFO = (totalIndicator.LFO ?? 0) + flowmeter.ConsAct;
                                totalIndicator.LFOAccumulated = (totalIndicator.LFOAccumulated ?? 0) + flowmeter.ConsAcc;
                            }
                        }
                        else
                        {
                            pu.LFO = flowmeter.ConsAct;
                            pu.LFOAccumulated = flowmeter.ConsAcc;

                            totalIndicator.LFO = (totalIndicator.LFO ?? 0) + flowmeter.ConsAct;
                            totalIndicator.LFOAccumulated = (totalIndicator.LFOAccumulated ?? 0) + flowmeter.ConsAcc;
                        }
                        break;

                    case "HFO":
                        if (deviceType == "me")
                        {
                            pu.HFO = flowmeter.ConsAct;
                            pu.HFOAccumulated = flowmeter.ConsAcc;

                            totalIndicator.HFO = (totalIndicator.HFO ?? 0) + flowmeter.ConsAct;
                            totalIndicator.HFOAccumulated = (totalIndicator.HFOAccumulated ?? 0) + flowmeter.ConsAcc;
                        }
                        else if (deviceType == "blr")
                        {
                            pu.HFO = (pu.HFO ?? 0) + flowmeter.ConsAct;
                            pu.HFOAccumulated = (pu.HFOAccumulated ?? 0) + flowmeter.ConsAcc;

                            totalIndicator.HFO = (totalIndicator.HFO ?? 0) + flowmeter.ConsAct;
                            totalIndicator.HFOAccumulated = (totalIndicator.HFOAccumulated ?? 0) + flowmeter.ConsAcc;
                        }
                        else if (deviceType == "ae")
                        {
                            if (flowmeter.DeviceNo == "me_fuel_in_2")
                            {
                                pu.HFO = (pu.HFO ?? 0) + flowmeter.ConsAct;
                                pu.HFOAccumulated = (pu.HFOAccumulated ?? 0) + flowmeter.ConsAcc;

                                totalIndicator.HFO = (totalIndicator.HFO ?? 0) + flowmeter.ConsAct;
                                totalIndicator.HFOAccumulated = (totalIndicator.HFOAccumulated ?? 0) + flowmeter.ConsAcc;
                            }
                            else if (flowmeter.DeviceNo == "me_fuel_out_2")
                            {
                                pu.HFO = (pu.HFO ?? 0) - flowmeter.ConsAct;
                                pu.HFOAccumulated = (pu.HFOAccumulated ?? 0) - flowmeter.ConsAcc;

                                totalIndicator.HFO = (totalIndicator.HFO ?? 0) - flowmeter.ConsAct;
                                totalIndicator.HFOAccumulated = (totalIndicator.HFOAccumulated ?? 0) - flowmeter.ConsAcc;
                            }
                            else
                            {
                                pu.HFO = (pu.HFO ?? 0) + flowmeter.ConsAct;
                                pu.HFOAccumulated = (pu.HFOAccumulated ?? 0) + flowmeter.ConsAcc;

                                totalIndicator.HFO = (totalIndicator.HFO ?? 0) + flowmeter.ConsAct;
                                totalIndicator.HFOAccumulated = (totalIndicator.HFOAccumulated ?? 0) + flowmeter.ConsAcc;
                            }
                        }
                        else
                        {
                            pu.HFO = flowmeter.ConsAct;
                            pu.HFOAccumulated = flowmeter.ConsAcc;

                            totalIndicator.HFO = (totalIndicator.HFO ?? 0) + flowmeter.ConsAct;
                            totalIndicator.HFOAccumulated = (totalIndicator.HFOAccumulated ?? 0) + flowmeter.ConsAcc;
                        }
                        break;

                    case "LPG_P":
                        if (deviceType == "me")
                        {
                            pu.LPG_P = flowmeter.ConsAct;
                            pu.LPG_PAccumulated = flowmeter.ConsAcc;

                            totalIndicator.LPG_P = (totalIndicator.LPG_P ?? 0) + flowmeter.ConsAct;
                            totalIndicator.LPG_PAccumulated = (totalIndicator.LPG_PAccumulated ?? 0) + flowmeter.ConsAcc;
                        }
                        else if (deviceType == "blr")
                        {
                            pu.LPG_P = (pu.LPG_P ?? 0) + flowmeter.ConsAct;
                            pu.LPG_PAccumulated = (pu.LPG_PAccumulated ?? 0) + flowmeter.ConsAcc;

                            totalIndicator.LPG_P = (totalIndicator.LPG_P ?? 0) + flowmeter.ConsAct;
                            totalIndicator.LPG_PAccumulated = (totalIndicator.LPG_PAccumulated ?? 0) + flowmeter.ConsAcc;
                        }
                        else if (deviceType == "ae")
                        {
                            if (flowmeter.DeviceNo == "me_fuel_in_2")
                            {
                                pu.LPG_P = (pu.LPG_P ?? 0) + flowmeter.ConsAct;
                                pu.LPG_PAccumulated = (pu.LPG_PAccumulated ?? 0) + flowmeter.ConsAcc;

                                totalIndicator.LPG_P = (totalIndicator.LPG_P ?? 0) + flowmeter.ConsAct;
                                totalIndicator.LPG_PAccumulated = (totalIndicator.LPG_PAccumulated ?? 0) + flowmeter.ConsAcc;
                            }
                            else if (flowmeter.DeviceNo == "me_fuel_out_2")
                            {
                                pu.LPG_P = (pu.LPG_P ?? 0) - flowmeter.ConsAct;
                                pu.LPG_PAccumulated = (pu.LPG_PAccumulated ?? 0) - flowmeter.ConsAcc;

                                totalIndicator.LPG_P = (totalIndicator.LPG_P ?? 0) - flowmeter.ConsAct;
                                totalIndicator.LPG_PAccumulated = (totalIndicator.LPG_PAccumulated ?? 0) - flowmeter.ConsAcc;
                            }
                            else
                            {
                                pu.LPG_P = (pu.LPG_P ?? 0) + flowmeter.ConsAct;
                                pu.LPG_PAccumulated = (pu.LPG_PAccumulated ?? 0) + flowmeter.ConsAcc;

                                totalIndicator.LPG_P = (totalIndicator.LPG_P ?? 0) + flowmeter.ConsAct;
                                totalIndicator.LPG_PAccumulated = (totalIndicator.LPG_PAccumulated ?? 0) + flowmeter.ConsAcc;
                            }
                        }
                        else
                        {
                            pu.LPG_P = flowmeter.ConsAct;
                            pu.LPG_PAccumulated = flowmeter.ConsAcc;

                            totalIndicator.LPG_P = (totalIndicator.LPG_P ?? 0) + flowmeter.ConsAct;
                            totalIndicator.LPG_PAccumulated = (totalIndicator.LPG_PAccumulated ?? 0) + flowmeter.ConsAcc;
                        }
                        break;

                    case "LPG_B":
                        if (deviceType == "me")
                        {
                            pu.LPG_B = flowmeter.ConsAct;
                            pu.LPG_BAccumulated = flowmeter.ConsAcc;

                            totalIndicator.LPG_B = (totalIndicator.LPG_B ?? 0) + flowmeter.ConsAct;
                            totalIndicator.LPG_BAccumulated = (totalIndicator.LPG_BAccumulated ?? 0) + flowmeter.ConsAcc;
                        }
                        else if (deviceType == "blr")
                        {
                            pu.LPG_B = (pu.LPG_B ?? 0) + flowmeter.ConsAct;
                            pu.LPG_BAccumulated = (pu.LPG_BAccumulated ?? 0) + flowmeter.ConsAcc;

                            totalIndicator.LPG_B = (totalIndicator.LPG_B ?? 0) + flowmeter.ConsAct;
                            totalIndicator.LPG_BAccumulated = (totalIndicator.LPG_BAccumulated ?? 0) + flowmeter.ConsAcc;
                        }
                        else if (deviceType == "ae")
                        {
                            if (flowmeter.DeviceNo == "me_fuel_in_2")
                            {
                                pu.LPG_B = (pu.LPG_B ?? 0) + flowmeter.ConsAct;
                                pu.LPG_BAccumulated = (pu.LPG_BAccumulated ?? 0) + flowmeter.ConsAcc;

                                totalIndicator.LPG_B = (totalIndicator.LPG_B ?? 0) + flowmeter.ConsAct;
                                totalIndicator.LPG_BAccumulated = (totalIndicator.LPG_BAccumulated ?? 0) + flowmeter.ConsAcc;
                            }
                            else if (flowmeter.DeviceNo == "me_fuel_out_2")
                            {
                                pu.LPG_B = (pu.LPG_B ?? 0) - flowmeter.ConsAct;
                                pu.LPG_BAccumulated = (pu.LPG_BAccumulated ?? 0) - flowmeter.ConsAcc;

                                totalIndicator.LPG_B = (totalIndicator.LPG_B ?? 0) - flowmeter.ConsAct;
                                totalIndicator.LPG_BAccumulated = (totalIndicator.LPG_BAccumulated ?? 0) - flowmeter.ConsAcc;
                            }
                            else
                            {
                                pu.LPG_B = (pu.LPG_B ?? 0) + flowmeter.ConsAct;
                                pu.LPG_BAccumulated = (pu.LPG_BAccumulated ?? 0) + flowmeter.ConsAcc;

                                totalIndicator.LPG_B = (totalIndicator.LPG_B ?? 0) + flowmeter.ConsAct;
                                totalIndicator.LPG_BAccumulated = (totalIndicator.LPG_BAccumulated ?? 0) + flowmeter.ConsAcc;
                            }
                        }
                        else
                        {
                            pu.LPG_B = flowmeter.ConsAct;
                            pu.LPG_BAccumulated = flowmeter.ConsAcc;

                            totalIndicator.LPG_B = (totalIndicator.LPG_B ?? 0) + flowmeter.ConsAct;
                            totalIndicator.LPG_BAccumulated = (totalIndicator.LPG_BAccumulated ?? 0) + flowmeter.ConsAcc;
                        }
                        break;

                    case "LNG":
                        if (deviceType == "me")
                        {
                            pu.LNG = flowmeter.ConsAct;
                            pu.LNGAccumulated = flowmeter.ConsAcc;

                            totalIndicator.LNG = (totalIndicator.LNG ?? 0) + flowmeter.ConsAct;
                            totalIndicator.LNGAccumulated = (totalIndicator.LNGAccumulated ?? 0) + flowmeter.ConsAcc;
                        }
                        else if (deviceType == "blr")
                        {
                            pu.LNG = (pu.LNG ?? 0) + flowmeter.ConsAct;
                            pu.LNGAccumulated = (pu.LNGAccumulated ?? 0) + flowmeter.ConsAcc;

                            totalIndicator.LNG = (totalIndicator.LNG ?? 0) + flowmeter.ConsAct;
                            totalIndicator.LNGAccumulated = (totalIndicator.LNGAccumulated ?? 0) + flowmeter.ConsAcc;
                        }
                        else if (deviceType == "ae")
                        {
                            if (flowmeter.DeviceNo == "me_fuel_in_2")
                            {
                                pu.LNG = (pu.LNG ?? 0) + flowmeter.ConsAct;
                                pu.LNGAccumulated = (pu.LNGAccumulated ?? 0) + flowmeter.ConsAcc;

                                totalIndicator.LNG = (totalIndicator.LNG ?? 0) + flowmeter.ConsAct;
                                totalIndicator.LNGAccumulated = (totalIndicator.LNGAccumulated ?? 0) + flowmeter.ConsAcc;
                            }
                            else if (flowmeter.DeviceNo == "me_fuel_out_2")
                            {
                                pu.LNG = (pu.LNG ?? 0) - flowmeter.ConsAct;
                                pu.LNGAccumulated = (pu.LNGAccumulated ?? 0) - flowmeter.ConsAcc;

                                totalIndicator.LNG = (totalIndicator.LNG ?? 0) - flowmeter.ConsAct;
                                totalIndicator.LNGAccumulated = (totalIndicator.LNGAccumulated ?? 0) - flowmeter.ConsAcc;
                            }
                            else
                            {
                                pu.LNG = (pu.LNG ?? 0) + flowmeter.ConsAct;
                                pu.LNGAccumulated = (pu.LNGAccumulated ?? 0) + flowmeter.ConsAcc;

                                totalIndicator.LNG = (totalIndicator.LNG ?? 0) + flowmeter.ConsAct;
                                totalIndicator.LNGAccumulated = (totalIndicator.LNGAccumulated ?? 0) + flowmeter.ConsAcc;
                            }
                        }
                        else
                        {
                            pu.LNG = flowmeter.ConsAct;
                            pu.LNGAccumulated = flowmeter.ConsAcc;

                            totalIndicator.LNG = (totalIndicator.LNG ?? 0) + flowmeter.ConsAct;
                            totalIndicator.LNGAccumulated = (totalIndicator.LNGAccumulated ?? 0) + flowmeter.ConsAcc;
                        }
                        break;

                    case "Methanol":
                        if (deviceType == "me")
                        {
                            pu.Methanol = flowmeter.ConsAct;
                            pu.MethanolAccumulated = flowmeter.ConsAcc;

                            totalIndicator.Methanol = (totalIndicator.Methanol ?? 0) + flowmeter.ConsAct;
                            totalIndicator.MethanolAccumulated = (totalIndicator.MethanolAccumulated ?? 0) + flowmeter.ConsAcc;
                        }
                        else if (deviceType == "blr")
                        {
                            pu.Methanol = (pu.Methanol ?? 0) + flowmeter.ConsAct;
                            pu.MethanolAccumulated = (pu.MethanolAccumulated ?? 0) + flowmeter.ConsAcc;

                            totalIndicator.Methanol = (totalIndicator.Methanol ?? 0) + flowmeter.ConsAct;
                            totalIndicator.MethanolAccumulated = (totalIndicator.MethanolAccumulated ?? 0) + flowmeter.ConsAcc;
                        }
                        else if (deviceType == "ae")
                        {
                            if (flowmeter.DeviceNo == "me_fuel_in_2")
                            {
                                pu.Methanol = (pu.Methanol ?? 0) + flowmeter.ConsAct;
                                pu.MethanolAccumulated = (pu.MethanolAccumulated ?? 0) + flowmeter.ConsAcc;

                                totalIndicator.Methanol = (totalIndicator.Methanol ?? 0) + flowmeter.ConsAct;
                                totalIndicator.MethanolAccumulated = (totalIndicator.MethanolAccumulated ?? 0) + flowmeter.ConsAcc;
                            }
                            else if (flowmeter.DeviceNo == "me_fuel_out_2")
                            {
                                pu.Methanol = (pu.Methanol ?? 0) - flowmeter.ConsAct;
                                pu.MethanolAccumulated = (pu.MethanolAccumulated ?? 0) - flowmeter.ConsAcc;

                                totalIndicator.Methanol = (totalIndicator.Methanol ?? 0) - flowmeter.ConsAct;
                                totalIndicator.MethanolAccumulated = (totalIndicator.MethanolAccumulated ?? 0) - flowmeter.ConsAcc;
                            }
                            else
                            {
                                pu.Methanol = (pu.Methanol ?? 0) + flowmeter.ConsAct;
                                pu.MethanolAccumulated = (pu.MethanolAccumulated ?? 0) + flowmeter.ConsAcc;

                                totalIndicator.Methanol = (totalIndicator.Methanol ?? 0) + flowmeter.ConsAct;
                                totalIndicator.MethanolAccumulated = (totalIndicator.MethanolAccumulated ?? 0) + flowmeter.ConsAcc;
                            }
                        }
                        else
                        {
                            pu.Methanol = flowmeter.ConsAct;
                            pu.MethanolAccumulated = flowmeter.ConsAcc;

                            totalIndicator.Methanol = (totalIndicator.Methanol ?? 0) + flowmeter.ConsAct;
                            totalIndicator.MethanolAccumulated = (totalIndicator.MethanolAccumulated ?? 0) + flowmeter.ConsAcc;
                        }
                        break;

                    case "Ethanol":
                        if (deviceType == "me")
                        {
                            pu.Ethanol = flowmeter.ConsAct;
                            pu.EthanolAccumulated = flowmeter.ConsAcc;

                            totalIndicator.Ethanol = (totalIndicator.Ethanol ?? 0) + flowmeter.ConsAct;
                            totalIndicator.EthanolAccumulated = (totalIndicator.EthanolAccumulated ?? 0) + flowmeter.ConsAcc;
                        }
                        else if (deviceType == "blr")
                        {
                            pu.Ethanol = (pu.Ethanol ?? 0) + flowmeter.ConsAct;
                            pu.EthanolAccumulated = (pu.EthanolAccumulated ?? 0) + flowmeter.ConsAcc;

                            totalIndicator.Ethanol = (totalIndicator.Ethanol ?? 0) + flowmeter.ConsAct;
                            totalIndicator.EthanolAccumulated = (totalIndicator.EthanolAccumulated ?? 0) + flowmeter.ConsAcc;
                        }
                        else if (deviceType == "ae")
                        {
                            if (flowmeter.DeviceNo == "me_fuel_in_2")
                            {
                                pu.Ethanol = (pu.Ethanol ?? 0) + flowmeter.ConsAct;
                                pu.EthanolAccumulated = (pu.EthanolAccumulated ?? 0) + flowmeter.ConsAcc;

                                totalIndicator.Ethanol = (totalIndicator.Ethanol ?? 0) + flowmeter.ConsAct;
                                totalIndicator.EthanolAccumulated = (totalIndicator.EthanolAccumulated ?? 0) + flowmeter.ConsAcc;
                            }
                            else if (flowmeter.DeviceNo == "me_fuel_out_2")
                            {
                                pu.Ethanol = (pu.Ethanol ?? 0) - flowmeter.ConsAct;
                                pu.EthanolAccumulated = (pu.EthanolAccumulated ?? 0) - flowmeter.ConsAcc;

                                totalIndicator.Ethanol = (totalIndicator.Ethanol ?? 0) - flowmeter.ConsAct;
                                totalIndicator.EthanolAccumulated = (totalIndicator.EthanolAccumulated ?? 0) - flowmeter.ConsAcc;
                            }
                            else
                            {
                                pu.Ethanol = (pu.Ethanol ?? 0) + flowmeter.ConsAct;
                                pu.EthanolAccumulated = (pu.EthanolAccumulated ?? 0) + flowmeter.ConsAcc;

                                totalIndicator.Ethanol = (totalIndicator.Ethanol ?? 0) + flowmeter.ConsAct;
                                totalIndicator.EthanolAccumulated = (totalIndicator.EthanolAccumulated ?? 0) + flowmeter.ConsAcc;
                            }
                        }
                        else
                        {
                            pu.Ethanol = flowmeter.ConsAct;
                            pu.EthanolAccumulated = flowmeter.ConsAcc;

                            totalIndicator.Ethanol = (totalIndicator.Ethanol ?? 0) + flowmeter.ConsAct;
                            totalIndicator.EthanolAccumulated = (totalIndicator.EthanolAccumulated ?? 0) + flowmeter.ConsAcc;
                        }
                        break;
                }

                #endregion 迪拜*/

                /* 国能长江01
                if (!lstPu.Any(t => t.DeviceType == deviceType))
                    lstPu.Add(new PowerUnitDto { Number = number, ReceiveDatetime = vessel.ReceiveDatetime, DeviceType = deviceType });

                var pu = lstPu[lstPu.IndexOf(lstPu.FirstOrDefault(t => t.DeviceType == deviceType))];
                var flowmeterInfos = flowmeter.DeviceNo.Split('_');
                if (flowmeterInfos.Length > 0)
                {
                    #region 动力单元

                    if (!lstPu.Any(t => t.DeviceType == flowmeterInfos[0]))
                        lstPu.Add(new PowerUnitDto { Number = number, ReceiveDatetime = vessel.ReceiveDatetime, DeviceType = flowmeterInfos[0] });
                    var pu = lstPu[lstPu.IndexOf(lstPu.FirstOrDefault(t => t.DeviceType == flowmeterInfos[0]))];
                    if (flowmeterInfos[1] == "fuel")
                    {
                        if (flowmeterInfos[2] == "in")
                        {
                            pu.HFO = (pu.HFO ?? 0) + flowmeter.ConsAct;
                            pu.HFOAccumulated = (pu.HFOAccumulated ?? 0) + flowmeter.ConsAcc;
                        }
                        else if (flowmeterInfos[2] == "out")
                        {
                            pu.HFO = (pu.HFO ?? 0) - flowmeter.ConsAct;
                            pu.HFOAccumulated = (pu.HFOAccumulated ?? 0) - flowmeter.ConsAcc;
                        }
                    }
                    else if (flowmeterInfos[1] == "lfo")
                    {
                        pu.LFO = (pu.LFO ?? 0) + flowmeter.ConsAct;
                        pu.LFOAccumulated = (pu.LFOAccumulated ?? 0) + flowmeter.ConsAcc;
                    }
                    else if (flowmeterInfos[1] == "lpg_p")
                    {
                        pu.LPG_P = (pu.LPG_P ?? 0) + flowmeter.ConsAct;
                        pu.LPG_PAccumulated = (pu.LPG_PAccumulated ?? 0) + flowmeter.ConsAcc;
                    }
                    else if (flowmeterInfos[1] == "lpg_b")
                    {
                        pu.LPG_B = (pu.LPG_B ?? 0) + flowmeter.ConsAct;
                        pu.LPG_BAccumulated = (pu.LPG_BAccumulated ?? 0) + flowmeter.ConsAcc;
                    }
                    else if (flowmeterInfos[1] == "lng")
                    {
                        pu.LNG = (pu.LNG ?? 0) + flowmeter.ConsAct;
                        pu.LNGAccumulated = (pu.LNGAccumulated ?? 0) + flowmeter.ConsAcc;
                    }
                    else if (flowmeterInfos[1] == "methanol")
                    {
                        pu.Methanol = (pu.Methanol ?? 0) + flowmeter.ConsAct;
                        pu.MethanolAccumulated = (pu.MethanolAccumulated ?? 0) + flowmeter.ConsAcc;
                    }
                    else if (flowmeterInfos[1] == "ethanol")
                    {
                        pu.Ethanol = (pu.Ethanol ?? 0) + flowmeter.ConsAct;
                        pu.EthanolAccumulated = (pu.EthanolAccumulated ?? 0) + flowmeter.ConsAcc;
                    }

                    #endregion 动力单元

                    #region 总计

                    if (flowmeterInfos[1] == "fuel")
                    {
                        if (flowmeterInfos[2] == "in")
                        {
                            totalIndicator.HFO = (totalIndicator.HFO ?? 0) + flowmeter.ConsAct;
                            totalIndicator.HFOAccumulated = (totalIndicator.HFOAccumulated ?? 0) + flowmeter.ConsAcc;
                        }
                        else if (flowmeterInfos[2] == "out")
                        {
                            totalIndicator.HFO = (totalIndicator.HFO ?? 0) - flowmeter.ConsAct;
                            totalIndicator.HFOAccumulated = (totalIndicator.HFOAccumulated ?? 0) - flowmeter.ConsAcc;
                        }
                    }
                    else if (flowmeterInfos[1] == "lfo")
                    {
                        totalIndicator.LFO = (totalIndicator.LFO ?? 0) + flowmeter.ConsAct;
                        totalIndicator.LFOAccumulated = (totalIndicator.LFOAccumulated ?? 0) + flowmeter.ConsAcc;
                    }
                    else if (flowmeterInfos[1] == "lpg_p")
                    {
                        totalIndicator.LPG_P = (totalIndicator.LPG_P ?? 0) + flowmeter.ConsAct;
                        totalIndicator.LPG_PAccumulated = (totalIndicator.LPG_PAccumulated ?? 0) + flowmeter.ConsAcc;
                    }
                    else if (flowmeterInfos[1] == "lpg_b")
                    {
                        totalIndicator.LPG_B = (totalIndicator.LPG_B ?? 0) + flowmeter.ConsAct;
                        totalIndicator.LPG_BAccumulated = (totalIndicator.LPG_BAccumulated ?? 0) + flowmeter.ConsAcc;
                    }
                    else if (flowmeterInfos[1] == "lng")
                    {
                        totalIndicator.LNG = (totalIndicator.LNG ?? 0) + flowmeter.ConsAct;
                        totalIndicator.LNGAccumulated = (totalIndicator.LNGAccumulated ?? 0) + flowmeter.ConsAcc;
                    }
                    else if (flowmeterInfos[1] == "methanol")
                    {
                        totalIndicator.Methanol = (totalIndicator.Methanol ?? 0) + flowmeter.ConsAct;
                        totalIndicator.MethanolAccumulated = (totalIndicator.MethanolAccumulated ?? 0) + flowmeter.ConsAcc;
                    }
                    else if (flowmeterInfos[1] == "ethanol")
                    {
                        totalIndicator.Ethanol = (totalIndicator.Ethanol ?? 0) + flowmeter.ConsAct;
                        totalIndicator.EthanolAccumulated = (totalIndicator.EthanolAccumulated ?? 0) + flowmeter.ConsAcc;
                    }

                    #endregion 总计
                }*/
            }

            #region 长航处流量计合计数据 同上foreach功能一致 酌情选择
            var DeviceTypes = new string[] { "me", "ae" };
            foreach (var deviceType in DeviceTypes)
            {
                if (!lstPu.Any(t => t.DeviceType == deviceType))
                    lstPu.Add(new PowerUnitDto { Number = number, ReceiveDatetime = vessel.ReceiveDatetime, DeviceType = deviceType });
                var pu = lstPu[lstPu.IndexOf(lstPu.FirstOrDefault(t => t.DeviceType == deviceType))];
                pu.DGO = flowmeters.Sum(t => t.DeviceType == deviceType ? (t.ConsAct ?? 0) : 0);
                pu.DGOAccumulated = flowmeters.Sum(t => t.DeviceType == deviceType ? (t.ConsAcc ?? 0) : 0);
            }
            totalIndicator.DGO = flowmeters.Sum(t => t.DeviceType == "refuel" ? 0 : (t.ConsAct ?? 0));
            totalIndicator.DGOAccumulated = flowmeters.Sum(t => t.DeviceType == "refuel" ? 0 : (t.ConsAcc ?? 0));
            #endregion

            bsi.Pitch = (bsi.Pitch == 0 || bsi.Pitch is null) ? 5128.77 : bsi.Pitch;
            var tempSlip = 0m;
            CriteriaDto criteriaDto = new CriteriaDto();
            //轴功率仪合计数据
            foreach (var shaft in shafts)
            {
                totalIndicator.Power = (totalIndicator.Power ?? 0) + shaft.Power;
                totalIndicator.Torque = (totalIndicator.Torque ?? 0) + shaft.Torque;
                totalIndicator.Thrust = (totalIndicator.Thrust ?? 0) + shaft.Thrust;
                totalIndicator.Rpm = (totalIndicator.Rpm ?? 0) + shaft.RPM;
            }

            tempSlip += totalIndicator.Rpm == 0 ? 0 : ((decimal)vessel.WaterSpeed / (decimal)totalIndicator.Rpm / (decimal)bsi.Pitch * 1852 * 1000 / 60);
            vessel.Slip = Math.Round((double)(1 - tempSlip) * 100, 2);

            var TotalHFO = totalIndicator.HFO ?? 0;
            var TotalDGO = totalIndicator.DGO ?? 0;
            var TotalMethanol = totalIndicator.Methanol ?? 0;
            var TotalPower = totalIndicator.Power ?? 0;
            var MEFuel = lstPu.FirstOrDefault(t => t.DeviceType == "me", new());
            var AEFuel = lstPu.FirstOrDefault(t => t.DeviceType == "ae", new());
            var BLRFuel = lstPu.FirstOrDefault(t => t.DeviceType == "blr", new());
            vessel.MEPower = Convert.ToDouble(TotalPower);
            vessel.MERpm = Convert.ToDouble(totalIndicator.Rpm ?? 0);
            vessel.Torque = Convert.ToDouble(totalIndicator.Torque ?? 0);
            vessel.Thrust = Convert.ToDouble(totalIndicator.Thrust ?? 0);

            vessel.SFOC = TotalPower == 0 ? 0 : (double)(((totalIndicator.DGO ?? 0) + (totalIndicator.LFO ?? 0) + (totalIndicator.HFO ?? 0) + (totalIndicator.LPG_P ?? 0) + (totalIndicator.LPG_B ?? 0) + (totalIndicator.LNG ?? 0) + (totalIndicator.Methanol ?? 0) + (totalIndicator.Ethanol ?? 0)) / TotalPower);
            vessel.FCPerNm = (vessel.GroundSpeed ?? 0) == 0 ? 0 : (double)((totalIndicator.DGO ?? 0) + (totalIndicator.LFO ?? 0) + (totalIndicator.HFO ?? 0) + (totalIndicator.LPG_P ?? 0) + (totalIndicator.LPG_B ?? 0) + (totalIndicator.LNG ?? 0) + (totalIndicator.Methanol ?? 0) + (totalIndicator.Ethanol ?? 0)) / vessel.GroundSpeed;

            vessel.MESFOC = TotalPower == 0 ? 0 : (double)(((MEFuel.DGO ?? 0) + (MEFuel.LFO ?? 0) + (MEFuel.HFO ?? 0) + (MEFuel.LPG_P ?? 0) + (MEFuel.LPG_B ?? 0) + (MEFuel.LNG ?? 0) + (MEFuel.Methanol ?? 0) + (MEFuel.Ethanol ?? 0)) / TotalPower);
            vessel.MEFCPerNm = (vessel.GroundSpeed ?? 0) == 0 ? 0 : (double)((MEFuel.DGO ?? 0) + (MEFuel.LFO ?? 0) + (MEFuel.HFO ?? 0) + (MEFuel.LPG_P ?? 0) + (MEFuel.LPG_B ?? 0) + (MEFuel.LNG ?? 0) + (MEFuel.Methanol ?? 0) + (MEFuel.Ethanol ?? 0)) / vessel.GroundSpeed;

            vessel.DGFCPerNm = (vessel.GroundSpeed ?? 0) == 0 ? 0 : (double)((AEFuel.DGO ?? 0) + (AEFuel.LFO ?? 0) + (AEFuel.HFO ?? 0) + (AEFuel.LPG_P ?? 0) + (AEFuel.LPG_B ?? 0) + (AEFuel.LNG ?? 0) + (AEFuel.Methanol ?? 0) + (AEFuel.Ethanol ?? 0)) / vessel.GroundSpeed;
            vessel.BLRFCPerNm = (vessel.GroundSpeed ?? 0) == 0 ? 0 : (double)((BLRFuel.DGO ?? 0) + (BLRFuel.LFO ?? 0) + (BLRFuel.HFO ?? 0) + (BLRFuel.LPG_P ?? 0) + (BLRFuel.LPG_B ?? 0) + (BLRFuel.LNG ?? 0) + (BLRFuel.Methanol ?? 0) + (BLRFuel.Ethanol ?? 0)) / vessel.GroundSpeed;

            vessel.MEHFOConsumption = (double)(MEFuel.HFO ?? 0);
            vessel.DGHFOConsumption = (double)(AEFuel.HFO ?? 0);
            vessel.BLRHFOConsumption = (double)(BLRFuel.HFO ?? 0);

            vessel.MEHFOCPerNm = (vessel.GroundSpeed ?? 0) == 0 ? 0 : vessel.MEHFOConsumption / vessel.GroundSpeed;
            vessel.DGHFOCPerNm = (vessel.GroundSpeed ?? 0) == 0 ? 0 : vessel.DGHFOConsumption / vessel.GroundSpeed;
            vessel.BLRHFOCPerNm = (vessel.GroundSpeed ?? 0) == 0 ? 0 : vessel.DGHFOConsumption / vessel.GroundSpeed;

            vessel.MEHFOCACC = (double)(MEFuel.HFOAccumulated ?? 0);
            vessel.DGHFOCACC = (double)(AEFuel.HFOAccumulated ?? 0);
            vessel.BLGHFOCACC = (double)(BLRFuel.HFOAccumulated ?? 0);

            vessel.MEMDOConsumption = (double)(MEFuel.DGO ?? 0);
            vessel.DGMDOConsumption = (double)(AEFuel.DGO ?? 0);
            vessel.BLRMDOConsumption = (double)(BLRFuel.DGO ?? 0);

            vessel.MEMDOCPerNm = (vessel.GroundSpeed ?? 0) == 0 ? 0 : vessel.MEMDOConsumption / vessel.GroundSpeed;
            vessel.DGMDOCPerNm = (vessel.GroundSpeed ?? 0) == 0 ? 0 : vessel.DGMDOConsumption / vessel.GroundSpeed;
            vessel.BLRMDOCPerNm = (vessel.GroundSpeed ?? 0) == 0 ? 0 : vessel.BLRMDOConsumption / vessel.GroundSpeed;

            vessel.MEMDOCACC = (double)(MEFuel.DGOAccumulated ?? 0);
            vessel.DGMDOCACC = (double)(AEFuel.DGOAccumulated ?? 0);
            vessel.BLGMDOCACC = (double)(BLRFuel.DGOAccumulated ?? 0);

            vessel.Status = GetStatus(number);

            var tonnage = 0f;
            if (bsi.ShipType.Contains("LNG carrier") ||
                bsi.ShipType.Contains("bulk carrier") ||
                bsi.ShipType.Contains("combination carrier") ||
                bsi.ShipType.Contains("container ship") ||
                bsi.ShipType.Contains("gas carrier") ||
                bsi.ShipType.Contains("general cargo ship") ||
                bsi.ShipType.Contains("refrigerated cargo carrier") ||
                bsi.ShipType.Contains("tanker"))
                tonnage = bsi.DWT ?? 0;
            else
                tonnage = bsi.GT ?? 0;

            vessel.RtCII = Math.Round(Convert.ToDouble((totalIndicator.HFO ?? 0) * 3.114m + (totalIndicator.LFO ?? 0) * 3.151m + (totalIndicator.DGO ?? 0) * 3.206m + (totalIndicator.Methanol ?? 0) * 1.375m + (totalIndicator.Ethanol ?? 0) * 1.913m + (totalIndicator.LPG_P ?? 0) * 3m + (totalIndicator.LPG_B ?? 0) * 3.03m + (totalIndicator.LNG ?? 0) * 2.75m) * 1000 / Convert.ToDouble(tonnage * vessel.GroundSpeed), 2);

            //预测接口使用
            try
            {
                var requestParams = new RequestParams
                {
                    Route = _configuration["prediction:route"] + $"?data={totalIndicator.Rpm},{totalIndicator.Power},{totalIndicator.Torque},{vessel.Draft},{vessel.Heel},{vessel.Slip},{vessel.GroundSpeed},{vessel.WaterSpeed},{vessel.Trim},{vessel.WindDirection},{vessel.WindSpeed}",
                    Parameter = new Dictionary<string, string>()
                };
                var content = await _httpRestClient.ExecuteAsync(requestParams);
                predictions.HFO = Convert.ToDecimal(content.ToJObject()["result"].First);
            }
            catch (Exception ex) { }
            //没有预测接口时使用
            //predictions.HFO = 100;

            StaticEntities.StaticEntities.Predictions[indexPredictions] = predictions;
            StaticEntities.StaticEntities.TotalIndicators[indexTotalIndicator] = totalIndicator;
            StaticEntities.StaticEntities.Vessels[indexVessel] = vessel;

            //if (totalIndicator.HFOAccumulated > 0 && totalIndicator.MethanolAccumulated > 0)
            //{
            var dailyConsumptionIndex = StaticEntities.StaticEntities.DailyConsumptions.IndexOf(StaticEntities.StaticEntities.DailyConsumptions.FirstOrDefault(t => t.Number == number));
            if (StaticEntities.StaticEntities.DailyConsumptions[dailyConsumptionIndex].Today == DateTime.MinValue || vessel.ReceiveDatetime.ToString("yyyyMMdd").CompareTo(StaticEntities.StaticEntities.DailyConsumptions[dailyConsumptionIndex].Today.ToString("yyyyMMdd")) > 0)
            {
                StaticEntities.StaticEntities.DailyConsumptions[dailyConsumptionIndex].HFOAcc = Convert.ToDouble(totalIndicator.HFOAccumulated);
                StaticEntities.StaticEntities.DailyConsumptions[dailyConsumptionIndex].DGOAcc = Convert.ToDouble(totalIndicator.DGOAccumulated);
                StaticEntities.StaticEntities.DailyConsumptions[dailyConsumptionIndex].MethanolAcc = Convert.ToDouble(totalIndicator.MethanolAccumulated);

                StaticEntities.StaticEntities.DailyConsumptions[dailyConsumptionIndex].MEHFOAcc = Convert.ToDouble(MEFuel.HFOAccumulated);
                StaticEntities.StaticEntities.DailyConsumptions[dailyConsumptionIndex].AEHFOAcc = Convert.ToDouble(AEFuel.HFOAccumulated);
                StaticEntities.StaticEntities.DailyConsumptions[dailyConsumptionIndex].BLRHFOAcc = Convert.ToDouble(BLRFuel.HFOAccumulated);
                StaticEntities.StaticEntities.DailyConsumptions[dailyConsumptionIndex].MEDGOAcc = Convert.ToDouble(MEFuel.DGOAccumulated);
                StaticEntities.StaticEntities.DailyConsumptions[dailyConsumptionIndex].AEDGOAcc = Convert.ToDouble(AEFuel.DGOAccumulated);
                StaticEntities.StaticEntities.DailyConsumptions[dailyConsumptionIndex].BLRDGOAcc = Convert.ToDouble(BLRFuel.DGOAccumulated);

                StaticEntities.StaticEntities.DailyConsumptions[dailyConsumptionIndex].Today = vessel.ReceiveDatetime;

                foreach (var fm in flowmeters)
                {
                    if (fm.DeviceNo == "me_fuel_1" && fm.FuelType == "DGO")
                        StaticEntities.StaticEntities.DailyConsumptions[dailyConsumptionIndex].ME1DGOAcc = Convert.ToDouble(fm.ConsAcc);
                    else if (fm.DeviceNo == "me_fuel_2" && fm.FuelType == "DGO")
                        StaticEntities.StaticEntities.DailyConsumptions[dailyConsumptionIndex].ME2DGOAcc = Convert.ToDouble(fm.ConsAcc);
                    else if (fm.DeviceNo == "ae_fuel_1" && fm.FuelType == "DGO")
                        StaticEntities.StaticEntities.DailyConsumptions[dailyConsumptionIndex].AE1DGOAcc = Convert.ToDouble(fm.ConsAcc);
                    else if (fm.DeviceNo == "ae_fuel_2" && fm.FuelType == "DGO")
                        StaticEntities.StaticEntities.DailyConsumptions[dailyConsumptionIndex].AE2DGOAcc = Convert.ToDouble(fm.ConsAcc);
                }
            }
            //}

            return result;
        }

        /// <summary>
        /// 数据解析
        /// </summary>
        /// <param name="number">采集系统id</param>
        /// <param name="deviceCode">设备代码</param>
        /// <param name="sentence">传输语句</param>
        public IList<LogBook> DataProccess(string number, string deviceCode, string sentence)
        {
            var result = new List<LogBook>();
            var lstFm = StaticEntities.StaticEntities.Flowmeters[StaticEntities.StaticEntities.Flowmeters.IndexOf(StaticEntities.StaticEntities.Flowmeters.FirstOrDefault(t => t.Number == number))].FlowmeterDtos;
            var lstBa = StaticEntities.StaticEntities.Batteries[StaticEntities.StaticEntities.Batteries.IndexOf(StaticEntities.StaticEntities.Batteries.FirstOrDefault(t => t.Number == number))].BatteryDtos;
            var lstGe = StaticEntities.StaticEntities.Generators[StaticEntities.StaticEntities.Generators.IndexOf(StaticEntities.StaticEntities.Generators.FirstOrDefault(t => t.Number == number))].GeneratorDtos;
            var lstSu = StaticEntities.StaticEntities.SupplyUnits[StaticEntities.StaticEntities.SupplyUnits.IndexOf(StaticEntities.StaticEntities.SupplyUnits.FirstOrDefault(t => t.Number == number))].SupplyUnitDtos;
            var lstSs = StaticEntities.StaticEntities.SternSealings[StaticEntities.StaticEntities.SternSealings.IndexOf(StaticEntities.StaticEntities.SternSealings.FirstOrDefault(t => t.Number == number))].SternSealingDtos;
            var lstSh = StaticEntities.StaticEntities.Shafts[StaticEntities.StaticEntities.Shafts.IndexOf(StaticEntities.StaticEntities.Shafts.FirstOrDefault(t => t.Number == number))].ShaftDtos;
            var lstLl = StaticEntities.StaticEntities.LiquidLevels[StaticEntities.StaticEntities.LiquidLevels.IndexOf(StaticEntities.StaticEntities.LiquidLevels.FirstOrDefault(t => t.Number == number))].LiquidLevelDtos;

            var currentVessel = StaticEntities.StaticEntities.Vessels.FirstOrDefault(t => t.SN == number);
            var indexVessel = StaticEntities.StaticEntities.Vessels.IndexOf(currentVessel);

            var monitoredDevicesItem = StaticEntities.StaticEntities.MonitoredDevices.FirstOrDefault(t => t.Number == number);
            var monitoredDevicesIndex = StaticEntities.StaticEntities.MonitoredDevices.IndexOf(monitoredDevicesItem);

            var consAct = 0m;

            switch (deviceCode)
            {
                case "me_fuel_in_1":
                case "me_fuel_out_1":
                case "me_fuel_in_2":
                case "me_fuel_out_2":
                case "me_methanol":
                    StaticEntities.StaticEntities.MonitoredDevices[monitoredDevicesIndex].Devices[deviceCode] = DateTime.UtcNow;
                    if (lstFm.Any(t => t.Number == number && t.DeviceNo == deviceCode && t.DeviceType == "me"))
                    {
                        var fm = lstFm.First(t => t.Number == number && t.DeviceNo == deviceCode && t.DeviceType == "me");
                        var index = lstFm.IndexOf(fm);
                        lstFm[index].ConsAct = Filtering(number, deviceCode, Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[0]));
                        lstFm[index].ConsAcc = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[1]);
                        lstFm[index].Temperature = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[2]);
                        lstFm[index].Density = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[3]);
                        //需要非空验证
                        lstFm[index].DeviceType = deviceCode.Split('_')[0];
                        lstFm[index].FuelType = deviceCode.Split('_')[1] != "fuel" ? deviceCode.Split('_')[1] : "HFO"; //根据实际条件进行判断
                        lstFm[index].Number = number;
                        lstFm[index].ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime;
                        lstFm[index].DeviceNo = deviceCode;
                    }
                    else
                    {
                        lstFm.Add(new FlowmeterDto
                        {
                            ConsAct = Filtering(number, deviceCode, Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[0])),
                            ConsAcc = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[1]),
                            Temperature = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[2]),
                            Density = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[3]),
                            //需要非空验证
                            DeviceType = deviceCode.Split('_')[0],
                            FuelType = deviceCode.Split('_')[1] != "fuel" ? deviceCode.Split('_')[1] : "HFO", //根据实际条件进行判断
                            Number = number,
                            ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime,
                            DeviceNo = deviceCode
                        });
                    }
                    break;

                case "draft":
                    StaticEntities.StaticEntities.MonitoredDevices[monitoredDevicesIndex].Devices["draft"] = DateTime.UtcNow;
                    StaticEntities.StaticEntities.Vessels[indexVessel].BowDraft = Convert.ToDouble(DecodeProtocalData(deviceCode, sentence)[0]);
                    StaticEntities.StaticEntities.Vessels[indexVessel].AsternDraft = Convert.ToDouble(DecodeProtocalData(deviceCode, sentence)[1]);
                    StaticEntities.StaticEntities.Vessels[indexVessel].PortDraft = Convert.ToDouble(DecodeProtocalData(deviceCode, sentence)[2]);
                    StaticEntities.StaticEntities.Vessels[indexVessel].StarBoardDraft = Convert.ToDouble(DecodeProtocalData(deviceCode, sentence)[3]);
                    StaticEntities.StaticEntities.Vessels[indexVessel].Draft = (StaticEntities.StaticEntities.Vessels[indexVessel].BowDraft + StaticEntities.StaticEntities.Vessels[indexVessel].AsternDraft) / 2d;
                    StaticEntities.StaticEntities.Vessels[indexVessel].Trim = StaticEntities.StaticEntities.Vessels[indexVessel].AsternDraft - StaticEntities.StaticEntities.Vessels[indexVessel].BowDraft;
                    StaticEntities.StaticEntities.Vessels[indexVessel].Heel = StaticEntities.StaticEntities.Vessels[indexVessel].PortDraft - StaticEntities.StaticEntities.Vessels[indexVessel].StarBoardDraft;
                    break;

                case "shaft_1":
                case "shaft_2":
                    StaticEntities.StaticEntities.MonitoredDevices[monitoredDevicesIndex].Devices[deviceCode] = DateTime.UtcNow;
                    if (lstSh.Any(t => t.Number == number && t.DeviceNo == deviceCode))
                    {
                        var shaft = lstSh.First(t => t.Number == number && t.DeviceNo == deviceCode);
                        var index = lstSh.IndexOf(shaft);
                        lstSh[index].Power = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[0]);
                        lstSh[index].RPM = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[1]);
                        lstSh[index].Torque = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[2]);
                        lstSh[index].Thrust = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[3]);
                        lstSh[index].Number = number;
                        lstSh[index].ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime;
                        lstSh[index].DeviceNo = deviceCode;
                    }
                    else
                    {
                        lstSh.Add(new ShaftDto
                        {
                            Power = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[0]),
                            RPM = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[1]),
                            Torque = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[2]),
                            Thrust = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[3]),
                            Number = number,
                            ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime,
                            DeviceNo = deviceCode
                        });
                    }
                    break;

                case "generator_1":
                case "generator_2":
                case "generator_3":
                case "generator_4":
                    StaticEntities.StaticEntities.MonitoredDevices[monitoredDevicesIndex].Devices[deviceCode] = DateTime.UtcNow;
                    if (lstGe.Any(t => t.Number == number && t.DeviceNo == deviceCode))
                    {
                        var generator = lstGe.First(t => t.Number == number && t.DeviceNo == deviceCode);
                        var index = lstGe.IndexOf(generator);
                        lstGe[index].IsRuning = Convert.ToByte(DecodeProtocalData(deviceCode, sentence)[0]);
                        lstGe[index].RPM = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[1]);
                        lstGe[index].StartPressure = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[2]);
                        lstGe[index].ControlPressure = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[3]);
                        lstGe[index].ScavengingPressure = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[4]);
                        lstGe[index].LubePressure = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[5]);
                        lstGe[index].LubeTEMP = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[6]);
                        lstGe[index].FuelPressure = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[7]);
                        lstGe[index].FuelTEMP = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[8]);
                        lstGe[index].FreshWaterPressure = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[9]);
                        lstGe[index].FreshWaterTEMPIn = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[10]);
                        lstGe[index].FreshWaterTEMPOut = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[11]);
                        lstGe[index].CoolingWaterPressure = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[12]);
                        lstGe[index].CoolingWaterTEMPIn = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[13]);
                        lstGe[index].CoolingWaterTEMPOut = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[14]);
                        lstGe[index].CylinderTEMP1 = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[15]);
                        lstGe[index].CylinderTEMP2 = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[16]);
                        lstGe[index].CylinderTEMP3 = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[17]);
                        lstGe[index].CylinderTEMP4 = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[18]);
                        lstGe[index].CylinderTEMP5 = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[19]);
                        lstGe[index].CylinderTEMP6 = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[20]);
                        lstGe[index].SuperchargerTEMPIn = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[21]);
                        lstGe[index].SuperchargerTEMPOut = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[22]);
                        lstGe[index].ScavengingTEMP = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[23]);
                        lstGe[index].BearingTEMP = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[24]);
                        lstGe[index].BearingTEMPFront = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[25]);
                        lstGe[index].BearingTEMPBack = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[26]);
                        lstGe[index].Power = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[27]);
                        lstGe[index].WindingTEMPL1 = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[28]);
                        lstGe[index].WindingTEMPL2 = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[29]);
                        lstGe[index].WindingTEMPL3 = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[30]);
                        lstGe[index].VoltageL1L2 = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[31]);
                        lstGe[index].VoltageL2L3 = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[32]);
                        lstGe[index].VoltageL1L3 = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[33]);
                        lstGe[index].FrequencyL1 = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[34]);
                        lstGe[index].FrequencyL2 = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[35]);
                        lstGe[index].FrequencyL3 = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[36]);
                        lstGe[index].CurrentL1 = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[37]);
                        lstGe[index].CurrentL2 = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[38]);
                        lstGe[index].CurrentL3 = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[39]);
                        lstGe[index].ReactivePower = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[40]);
                        lstGe[index].PowerFactor = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[41]);
                        lstGe[index].LoadRatio = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[42]);
                        lstGe[index].ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime;
                    }
                    else
                    {
                        lstGe.Add(new GeneratorDto
                        {
                            IsRuning = Convert.ToByte(DecodeProtocalData(deviceCode, sentence)[0]),
                            RPM = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[1]),
                            StartPressure = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[2]),
                            ControlPressure = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[3]),
                            ScavengingPressure = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[4]),
                            LubePressure = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[5]),
                            LubeTEMP = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[6]),
                            FuelPressure = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[7]),
                            FuelTEMP = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[8]),
                            FreshWaterPressure = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[9]),
                            FreshWaterTEMPIn = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[10]),
                            FreshWaterTEMPOut = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[11]),
                            CoolingWaterPressure = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[12]),
                            CoolingWaterTEMPIn = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[13]),
                            CoolingWaterTEMPOut = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[14]),
                            CylinderTEMP1 = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[15]),
                            CylinderTEMP2 = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[16]),
                            CylinderTEMP3 = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[17]),
                            CylinderTEMP4 = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[18]),
                            CylinderTEMP5 = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[19]),
                            CylinderTEMP6 = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[20]),
                            SuperchargerTEMPIn = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[21]),
                            SuperchargerTEMPOut = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[22]),
                            ScavengingTEMP = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[23]),
                            BearingTEMP = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[24]),
                            BearingTEMPFront = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[25]),
                            BearingTEMPBack = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[26]),
                            Power = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[27]),
                            WindingTEMPL1 = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[28]),
                            WindingTEMPL2 = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[29]),
                            WindingTEMPL3 = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[30]),
                            VoltageL1L2 = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[31]),
                            VoltageL2L3 = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[32]),
                            VoltageL1L3 = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[33]),
                            FrequencyL1 = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[34]),
                            FrequencyL2 = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[35]),
                            FrequencyL3 = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[36]),
                            CurrentL1 = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[37]),
                            CurrentL2 = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[38]),
                            CurrentL3 = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[39]),
                            ReactivePower = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[40]),
                            PowerFactor = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[41]),
                            LoadRatio = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[42]),
                            ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime,
                            DeviceNo = deviceCode,
                            Number = number
                        });
                    }
                    break;

                case "battery_1":
                case "battery_2":
                    StaticEntities.StaticEntities.MonitoredDevices[monitoredDevicesIndex].Devices[deviceCode] = DateTime.UtcNow;
                    if (lstBa.Any(t => t.Number == number && t.DeviceNo == deviceCode))
                    {
                        var battery = lstBa.First(t => t.Number == number && t.DeviceNo == deviceCode);
                        var index = lstBa.IndexOf(battery);
                        lstBa[index].SOC = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[0]);
                        lstBa[index].SOH = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[1]);
                        lstBa[index].MaxTEMP = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[2]);
                        lstBa[index].MaxTEMPBox = DecodeProtocalData(deviceCode, sentence)[3].ToString();
                        lstBa[index].MaxTEMPNo = DecodeProtocalData(deviceCode, sentence)[4].ToString();
                        lstBa[index].MinTEMP = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[5]);
                        lstBa[index].MinTEMPBox = DecodeProtocalData(deviceCode, sentence)[6].ToString();
                        lstBa[index].MinTEMPNo = DecodeProtocalData(deviceCode, sentence)[7].ToString();
                        lstBa[index].MaxVoltage = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[8]);
                        lstBa[index].MaxVoltageBox = DecodeProtocalData(deviceCode, sentence)[9].ToString();
                        lstBa[index].MaxVoltageNo = DecodeProtocalData(deviceCode, sentence)[10].ToString();
                        lstBa[index].MinVoltage = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[11]);
                        lstBa[index].MinVoltageBox = DecodeProtocalData(deviceCode, sentence)[12].ToString();
                        lstBa[index].MinVoltageNo = DecodeProtocalData(deviceCode, sentence)[13].ToString();
                        lstBa[index].ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime;
                    }
                    else
                    {
                        lstBa.Add(new BatteryDto
                        {
                            SOC = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[0]),
                            SOH = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[1]),
                            MaxTEMP = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[2]),
                            MaxTEMPBox = DecodeProtocalData(deviceCode, sentence)[3].ToString(),
                            MaxTEMPNo = DecodeProtocalData(deviceCode, sentence)[4].ToString(),
                            MinTEMP = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[5]),
                            MinTEMPBox = DecodeProtocalData(deviceCode, sentence)[6].ToString(),
                            MinTEMPNo = DecodeProtocalData(deviceCode, sentence)[7].ToString(),
                            MaxVoltage = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[8]),
                            MaxVoltageBox = DecodeProtocalData(deviceCode, sentence)[9].ToString(),
                            MaxVoltageNo = DecodeProtocalData(deviceCode, sentence)[10].ToString(),
                            MinVoltage = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[11]),
                            MinVoltageBox = DecodeProtocalData(deviceCode, sentence)[12].ToString(),
                            MinVoltageNo = DecodeProtocalData(deviceCode, sentence)[13].ToString(),
                            ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime,
                            DeviceNo = deviceCode,
                            Number = number
                        });
                    }
                    break;

                case "sternsealing_1":
                case "sternsealing_2":
                    StaticEntities.StaticEntities.MonitoredDevices[monitoredDevicesIndex].Devices[deviceCode] = DateTime.UtcNow;
                    if (lstSs.Any(t => t.Number == number && t.DeviceNo == deviceCode))
                    {
                        var sternsealing = lstSs.First(t => t.Number == number && t.DeviceNo == deviceCode);
                        var index = lstSs.IndexOf(sternsealing);
                        lstSs[index].FrontTEMP = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[0]);
                        lstSs[index].BackTEMP = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[1]);
                        lstSs[index].BackLeftTEMP = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[2]);
                        lstSs[index].BackRightTEMP = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[3]);
                        lstSs[index].ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime;
                    }
                    else
                    {
                        lstSs.Add(new SternSealingDto
                        {
                            FrontTEMP = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[0]),
                            BackTEMP = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[1]),
                            BackLeftTEMP = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[2]),
                            BackRightTEMP = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[3]),
                            ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime,
                            DeviceNo = deviceCode,
                            Number = number
                        });
                    }
                    break;

                case "liquidlevel_1":
                case "liquidlevel_2":
                    StaticEntities.StaticEntities.MonitoredDevices[monitoredDevicesIndex].Devices[deviceCode] = DateTime.UtcNow;
                    if (lstLl.Any(t => t.Number == number && t.DeviceNo == deviceCode))
                    {
                        var liquidlevel = lstLl.First(t => t.Number == number && t.DeviceNo == deviceCode);
                        var index = lstLl.IndexOf(liquidlevel);
                        lstLl[index].Level = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[0]);
                        lstLl[index].Temperature = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[1]);
                        lstLl[index].ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime;
                    }
                    else
                    {
                        lstLl.Add(new LiquidLevelDto
                        {
                            Level = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[0]),
                            Temperature = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[1]),
                            ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime,
                            DeviceNo = deviceCode,
                            Number = number
                        });
                    }
                    break;

                case "supplyunit_1":
                case "supplyunit_2":
                case "supplyunit_3":
                    StaticEntities.StaticEntities.MonitoredDevices[monitoredDevicesIndex].Devices[deviceCode] = DateTime.UtcNow;
                    if (lstSu.Any(t => t.Number == number && t.DeviceNo == deviceCode))
                    {
                        var supplyunit = lstSu.First(t => t.Number == number && t.DeviceNo == deviceCode);
                        var index = lstSu.IndexOf(supplyunit);
                        lstSu[index].IsRuning = Convert.ToByte(DecodeProtocalData(deviceCode, sentence)[0]);
                        lstSu[index].Temperature = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[1]);
                        lstSu[index].Pressure = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[2]);
                        lstSu[index].ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime;
                    }
                    else
                    {
                        lstSu.Add(new SupplyUnitDto
                        {
                            IsRuning = Convert.ToByte(DecodeProtocalData(deviceCode, sentence)[0]),
                            Temperature = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[1]),
                            Pressure = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[2]),
                            ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime,
                            DeviceNo = deviceCode,
                            Number = number
                        });
                    }
                    break;
            }

            return result;
        }

        /// <summary>
        /// 数据解析
        /// </summary>
        /// <param name="number">采集系统id</param>
        /// <param name="deviceCode">设备代码</param>
        /// <param name="sentence">传输语句</param>
        public IList<LogBook> DataProccessNEMA(string number, string deviceCode, string sentence)
        {
            var result = new List<LogBook>();
            var lstSh = StaticEntities.StaticEntities.Shafts[StaticEntities.StaticEntities.Shafts.IndexOf(StaticEntities.StaticEntities.Shafts.FirstOrDefault(t => t.Number == number))].ShaftDtos;

            var currentVessel = StaticEntities.StaticEntities.Vessels.FirstOrDefault(t => t.SN == number);
            var indexVessel = StaticEntities.StaticEntities.Vessels.IndexOf(currentVessel);

            var monitoredDevicesItem = StaticEntities.StaticEntities.MonitoredDevices.FirstOrDefault(t => t.Number == number);
            var monitoredDevicesIndex = StaticEntities.StaticEntities.MonitoredDevices.IndexOf(monitoredDevicesItem);

            switch (deviceCode)
            {
                case "shaft_1":
                case "shaft_2":
                    StaticEntities.StaticEntities.MonitoredDevices[monitoredDevicesIndex].Devices[deviceCode] = DateTime.UtcNow;
                    if (lstSh.Any(t => t.Number == number && t.DeviceNo == deviceCode))
                    {
                        var shaft = lstSh.First(t => t.Number == number && t.DeviceNo == deviceCode);
                        var index = lstSh.IndexOf(shaft);
                        lstSh[index].Power = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[0]);
                        lstSh[index].RPM = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[1]);
                        lstSh[index].Torque = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[2]);
                        lstSh[index].Thrust = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[3]);
                        lstSh[index].Number = number;
                        lstSh[index].ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime;
                        lstSh[index].DeviceNo = deviceCode;
                    }
                    else
                    {
                        lstSh.Add(new ShaftDto
                        {
                            Power = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[0]),
                            RPM = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[1]),
                            Torque = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[2]),
                            Thrust = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[3]),
                            Number = number,
                            ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime,
                            DeviceNo = deviceCode
                        });
                    }
                    break;

                case "fec":
                    var pfecEntity = new PFEC(sentence);
                    StaticEntities.StaticEntities.Vessels[indexVessel].X = pfecEntity.X;
                    StaticEntities.StaticEntities.Vessels[indexVessel].Y = pfecEntity.Y;
                    StaticEntities.StaticEntities.MonitoredDevices[monitoredDevicesIndex].Devices[deviceCode] = DateTime.UtcNow;
                    break;
            }

            return result;
        }

        /// <summary>
        /// MODBUS数据解析
        /// </summary>
        /// <param name="number">采集系统id</param>
        /// <param name="deviceCode">设备代码</param>
        /// <param name="sentence">传输语句</param>
        public IList<LogBook> DataProccessMODBUS(string number, string deviceCode, string sentence)
        {
            var result = new List<LogBook>();
            var lstFm = StaticEntities.StaticEntities.Flowmeters[StaticEntities.StaticEntities.Flowmeters.IndexOf(StaticEntities.StaticEntities.Flowmeters.FirstOrDefault(t => t.Number == number))].FlowmeterDtos;
            var lstGe = StaticEntities.StaticEntities.Generators[StaticEntities.StaticEntities.Generators.IndexOf(StaticEntities.StaticEntities.Generators.FirstOrDefault(t => t.Number == number))].GeneratorDtos;
            var lstSs = StaticEntities.StaticEntities.SternSealings[StaticEntities.StaticEntities.SternSealings.IndexOf(StaticEntities.StaticEntities.SternSealings.FirstOrDefault(t => t.Number == number))].SternSealingDtos;

            var lstSh = StaticEntities.StaticEntities.Shafts[StaticEntities.StaticEntities.Shafts.IndexOf(StaticEntities.StaticEntities.Shafts.FirstOrDefault(t => t.Number == number))].ShaftDtos;

            var currentVessel = StaticEntities.StaticEntities.Vessels.FirstOrDefault(t => t.SN == number);
            var indexVessel = StaticEntities.StaticEntities.Vessels.IndexOf(currentVessel);

            var monitoredDevicesItem = StaticEntities.StaticEntities.MonitoredDevices.FirstOrDefault(t => t.Number == number);
            var monitoredDevicesIndex = StaticEntities.StaticEntities.MonitoredDevices.IndexOf(monitoredDevicesItem);

            switch (deviceCode)
            {
                case "me_fuel_in_1":
                case "me_fuel_out_1":
                case "me_fuel_in_2":
                case "me_fuel_out_2":
                case "me_methanol":
                    StaticEntities.StaticEntities.MonitoredDevices[monitoredDevicesIndex].Devices[deviceCode] = DateTime.UtcNow;
                    if (lstFm.Any(t => t.Number == number && t.DeviceNo == deviceCode && t.DeviceType == "me"))
                    {
                        var fm = lstFm.First(t => t.Number == number && t.DeviceNo == deviceCode && t.DeviceType == "me");
                        var index = lstFm.IndexOf(fm);
                        lstFm[index].ConsAct = Filtering(number, deviceCode, Convert.ToDecimal(DecodeProtocalDataMODBUS(deviceCode, sentence)[0]));
                        lstFm[index].ConsAcc = Convert.ToDecimal(DecodeProtocalDataMODBUS(deviceCode, sentence)[1]);
                        lstFm[index].Temperature = Convert.ToDecimal(DecodeProtocalDataMODBUS(deviceCode, sentence)[2]);
                        lstFm[index].Density = Convert.ToDecimal(DecodeProtocalDataMODBUS(deviceCode, sentence)[3]);
                        //需要非空验证
                        lstFm[index].DeviceType = deviceCode.Split('_')[0];
                        lstFm[index].FuelType = deviceCode.Split('_')[1] != "fuel" ? deviceCode.Split('_')[1] : "HFO"; //根据实际条件进行判断
                        lstFm[index].Number = number;
                        lstFm[index].ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime;
                        lstFm[index].DeviceNo = deviceCode;
                    }
                    else
                    {
                        lstFm.Add(new FlowmeterDto
                        {
                            ConsAct = Filtering(number, deviceCode, Convert.ToDecimal(DecodeProtocalDataMODBUS(deviceCode, sentence)[0])),
                            ConsAcc = Convert.ToDecimal(DecodeProtocalDataMODBUS(deviceCode, sentence)[1]),
                            Temperature = Convert.ToDecimal(DecodeProtocalDataMODBUS(deviceCode, sentence)[2]),
                            Density = Convert.ToDecimal(DecodeProtocalDataMODBUS(deviceCode, sentence)[3]),
                            //需要非空验证
                            DeviceType = deviceCode.Split('_')[0],
                            FuelType = deviceCode.Split('_')[1] != "fuel" ? deviceCode.Split('_')[1] : "HFO", //根据实际条件进行判断
                            Number = number,
                            ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime,
                            DeviceNo = deviceCode
                        });
                    }

                    break;

                case "draft":
                    StaticEntities.StaticEntities.MonitoredDevices[monitoredDevicesIndex].Devices["draft"] = DateTime.UtcNow;
                    StaticEntities.StaticEntities.Vessels[indexVessel].BowDraft = Convert.ToDouble(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[0]);
                    StaticEntities.StaticEntities.Vessels[indexVessel].AsternDraft = Convert.ToDouble(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[1]);
                    StaticEntities.StaticEntities.Vessels[indexVessel].PortDraft = Convert.ToDouble(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[2]);
                    StaticEntities.StaticEntities.Vessels[indexVessel].StarBoardDraft = Convert.ToDouble(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[3]);
                    StaticEntities.StaticEntities.Vessels[indexVessel].Draft = (StaticEntities.StaticEntities.Vessels[indexVessel].BowDraft + StaticEntities.StaticEntities.Vessels[indexVessel].AsternDraft) / 2d;
                    StaticEntities.StaticEntities.Vessels[indexVessel].Trim = StaticEntities.StaticEntities.Vessels[indexVessel].AsternDraft - StaticEntities.StaticEntities.Vessels[indexVessel].BowDraft;
                    StaticEntities.StaticEntities.Vessels[indexVessel].Heel = StaticEntities.StaticEntities.Vessels[indexVessel].PortDraft - StaticEntities.StaticEntities.Vessels[indexVessel].StarBoardDraft;

                    #region 轴功率仪临时数据

                    /*if (lstSh.Any(t => t.Number == number && t.DeviceNo == "shaft_1"))
                    {
                        var shaft = lstSh.First(t => t.Number == number && t.DeviceNo == "shaft_1");
                        var index = lstSh.IndexOf(shaft);
                        lstSh[index].Power = 0;
                        lstSh[index].RPM = 0;
                        lstSh[index].Torque = 0;
                        lstSh[index].Thrust = 0;
                        lstSh[index].Number = number;
                        lstSh[index].ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime;
                        lstSh[index].DeviceNo = "shaft_1";
                    }
                    else
                    {
                        lstSh.Add(new ShaftDto
                        {
                            Power = 0,
                            RPM = 0,
                            Torque = 0,
                            Thrust = 0,
                            Number = number,
                            ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime,
                            DeviceNo = "shaft_1"
                        });
                    }
                    if (lstSh.Any(t => t.Number == number && t.DeviceNo == "shaft_2"))
                    {
                        var shaft = lstSh.First(t => t.Number == number && t.DeviceNo == "shaft_2");
                        var index = lstSh.IndexOf(shaft);
                        lstSh[index].Power = 0;
                        lstSh[index].RPM = 0;
                        lstSh[index].Torque = 0;
                        lstSh[index].Thrust = 0;
                        lstSh[index].Number = number;
                        lstSh[index].ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime;
                        lstSh[index].DeviceNo = "shaft_2";
                    }
                    else
                    {
                        lstSh.Add(new ShaftDto
                        {
                            Power = 0,
                            RPM = 0,
                            Torque = 0,
                            Thrust = 0,
                            Number = number,
                            ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime,
                            DeviceNo = "shaft_2"
                        });
                    }*/

                    #endregion 轴功率仪临时数据

                    break;

                case "generator_1":
                case "generator_2":
                case "generator_3":
                case "generator_4":
                    StaticEntities.StaticEntities.MonitoredDevices[monitoredDevicesIndex].Devices[deviceCode] = DateTime.UtcNow;
                    if (lstGe.Any(t => t.Number == number && t.DeviceNo == deviceCode))
                    {
                        var generator = lstGe.First(t => t.Number == number && t.DeviceNo == deviceCode);
                        var index = lstGe.IndexOf(generator);
                        //lstGe[index].IsRuning = Convert.ToByte(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[0]);
                        lstGe[index].RPM = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[1]);
                        lstGe[index].StartPressure = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[2]);
                        lstGe[index].ControlPressure = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[3]);
                        lstGe[index].ScavengingPressure = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[4]);
                        lstGe[index].LubePressure = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[5]);
                        lstGe[index].LubeTEMP = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[6]);
                        lstGe[index].FuelPressure = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[7]);
                        lstGe[index].FuelTEMP = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[8]);
                        lstGe[index].FreshWaterPressure = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[9]);
                        lstGe[index].FreshWaterTEMPIn = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[10]);
                        lstGe[index].FreshWaterTEMPOut = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[11]);
                        lstGe[index].CoolingWaterPressure = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[12]);
                        lstGe[index].CoolingWaterTEMPIn = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[13]);
                        lstGe[index].CoolingWaterTEMPOut = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[14]);
                        lstGe[index].CylinderTEMP1 = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[15]);
                        lstGe[index].CylinderTEMP2 = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[16]);
                        lstGe[index].CylinderTEMP3 = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[17]);
                        lstGe[index].CylinderTEMP4 = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[18]);
                        lstGe[index].CylinderTEMP5 = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[19]);
                        lstGe[index].CylinderTEMP6 = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[20]);
                        lstGe[index].SuperchargerTEMPIn = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[21]);
                        lstGe[index].SuperchargerTEMPOut = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[22]);
                        lstGe[index].ScavengingTEMP = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[23]);
                        lstGe[index].BearingTEMP = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[24]);
                        lstGe[index].BearingTEMPFront = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[25]);
                        lstGe[index].BearingTEMPBack = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[26]);
                        lstGe[index].Power = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[27]);
                        lstGe[index].WindingTEMPL1 = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[28]);
                        lstGe[index].WindingTEMPL2 = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[29]);
                        lstGe[index].WindingTEMPL3 = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[30]);
                        lstGe[index].VoltageL1L2 = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[31]);
                        lstGe[index].VoltageL2L3 = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[32]);
                        lstGe[index].VoltageL1L3 = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[33]);
                        lstGe[index].FrequencyL1 = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[34]);
                        lstGe[index].FrequencyL2 = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[35]);
                        lstGe[index].FrequencyL3 = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[36]);
                        lstGe[index].CurrentL1 = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[37]);
                        lstGe[index].CurrentL2 = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[38]);
                        lstGe[index].CurrentL3 = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[39]);
                        lstGe[index].ReactivePower = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[40]);
                        lstGe[index].PowerFactor = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[41]);
                        lstGe[index].LoadRatio = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[42]);
                        lstGe[index].ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime;
                    }
                    else
                    {
                        lstGe.Add(new GeneratorDto
                        {
                            //IsRuning = Convert.ToByte(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[0]),
                            RPM = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[1]),
                            StartPressure = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[2]),
                            ControlPressure = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[3]),
                            ScavengingPressure = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[4]),
                            LubePressure = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[5]),
                            LubeTEMP = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[6]),
                            FuelPressure = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[7]),
                            FuelTEMP = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[8]),
                            FreshWaterPressure = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[9]),
                            FreshWaterTEMPIn = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[10]),
                            FreshWaterTEMPOut = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[11]),
                            CoolingWaterPressure = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[12]),
                            CoolingWaterTEMPIn = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[13]),
                            CoolingWaterTEMPOut = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[14]),
                            CylinderTEMP1 = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[15]),
                            CylinderTEMP2 = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[16]),
                            CylinderTEMP3 = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[17]),
                            CylinderTEMP4 = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[18]),
                            CylinderTEMP5 = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[19]),
                            CylinderTEMP6 = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[20]),
                            SuperchargerTEMPIn = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[21]),
                            SuperchargerTEMPOut = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[22]),
                            ScavengingTEMP = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[23]),
                            BearingTEMP = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[24]),
                            BearingTEMPFront = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[25]),
                            BearingTEMPBack = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[26]),
                            Power = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[27]),
                            WindingTEMPL1 = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[28]),
                            WindingTEMPL2 = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[29]),
                            WindingTEMPL3 = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[30]),
                            VoltageL1L2 = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[31]),
                            VoltageL2L3 = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[32]),
                            VoltageL1L3 = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[33]),
                            FrequencyL1 = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[34]),
                            FrequencyL2 = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[35]),
                            FrequencyL3 = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[36]),
                            CurrentL1 = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[37]),
                            CurrentL2 = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[38]),
                            CurrentL3 = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[39]),
                            ReactivePower = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[40]),
                            PowerFactor = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[41]),
                            LoadRatio = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[42]),
                            ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime,
                            DeviceNo = deviceCode,
                            Number = number
                        });
                    }
                    break;

                case "generator_status":
                    DateTime genDt = DateTime.UtcNow;
                    StaticEntities.StaticEntities.MonitoredDevices[monitoredDevicesIndex].Devices["generator_1"] = genDt;
                    StaticEntities.StaticEntities.MonitoredDevices[monitoredDevicesIndex].Devices["generator_2"] = genDt;
                    StaticEntities.StaticEntities.MonitoredDevices[monitoredDevicesIndex].Devices["generator_3"] = genDt;
                    StaticEntities.StaticEntities.MonitoredDevices[monitoredDevicesIndex].Devices["generator_4"] = genDt;

                    var tempStatuss = string.Empty;
                    string Statuss = string.Empty;
                    try
                    {
                        tempStatuss = sentence.Substring(20, 2);
                        foreach (char c in tempStatuss)
                        {
                            int v = Convert.ToInt32(c.ToString(), 16);
                            int v2 = int.Parse(Convert.ToString(v, 2));
                            // 去掉格式串中的空格，即可去掉每个4位二进制数之间的空格，
                            Statuss += string.Format("{0:d4} ", v2);
                        }
                    }
                    catch
                    {
                        Statuss = "10000000";
                    }
                    Statuss = Statuss.Replace(" ", "");

                    var notFull = true;
                    if (lstGe.Any(t => t.Number == number && t.DeviceNo == "generator_1"))
                    {
                        var generator = lstGe.First(t => t.Number == number && t.DeviceNo == "generator_1");
                        var index = lstGe.IndexOf(generator);
                        lstGe[index].IsRuning = Convert.ToByte(Statuss[7].ToString());
                        notFull = false;
                    }
                    if (lstGe.Any(t => t.Number == number && t.DeviceNo == "generator_2"))
                    {
                        var generator = lstGe.First(t => t.Number == number && t.DeviceNo == "generator_2");
                        var index = lstGe.IndexOf(generator);
                        lstGe[index].IsRuning = Convert.ToByte(Statuss[6].ToString());
                        notFull = false;
                    }
                    if (lstGe.Any(t => t.Number == number && t.DeviceNo == "generator_3"))
                    {
                        var generator = lstGe.First(t => t.Number == number && t.DeviceNo == "generator_3");
                        var index = lstGe.IndexOf(generator);
                        lstGe[index].IsRuning = Convert.ToByte(Statuss[5].ToString());
                        notFull = false;
                    }
                    if (lstGe.Any(t => t.Number == number && t.DeviceNo == "generator_4"))
                    {
                        var generator = lstGe.First(t => t.Number == number && t.DeviceNo == "generator_4");
                        var index = lstGe.IndexOf(generator);
                        lstGe[index].IsRuning = Convert.ToByte(Statuss[4].ToString());
                        notFull = false;
                    }
                    if (notFull)
                    {
                        lstGe.Add(new GeneratorDto
                        {
                            IsRuning = Convert.ToByte(Statuss[7].ToString()),
                            ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime,
                            DeviceNo = "generator_1",
                            Number = number
                        });
                        lstGe.Add(new GeneratorDto
                        {
                            IsRuning = Convert.ToByte(Statuss[6].ToString()),
                            ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime,
                            DeviceNo = "generator_2",
                            Number = number
                        });
                        lstGe.Add(new GeneratorDto
                        {
                            IsRuning = Convert.ToByte(Statuss[5].ToString()),
                            ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime,
                            DeviceNo = "generator_3",
                            Number = number
                        });
                        lstGe.Add(new GeneratorDto
                        {
                            IsRuning = Convert.ToByte(Statuss[4].ToString()),
                            ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime,
                            DeviceNo = "generator_4",
                            Number = number
                        });
                    }
                    break;

                case "sternsealing":
                    StaticEntities.StaticEntities.MonitoredDevices[monitoredDevicesIndex].Devices[deviceCode] = DateTime.UtcNow;
                    if (lstSs.Any(t => t.Number == number && t.DeviceNo == deviceCode))
                    {
                        var sternsealing = lstSs.First(t => t.Number == number && t.DeviceNo == deviceCode);
                        var index = lstSs.IndexOf(sternsealing);
                        lstSs[index].FrontTEMP = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[0]);
                        lstSs[index].BackTEMP = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[1]);
                        lstSs[index].BackLeftTEMP = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[2]);
                        lstSs[index].BackRightTEMP = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[3]);
                        lstSs[index].ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime;
                    }
                    else
                    {
                        lstSs.Add(new SternSealingDto
                        {
                            FrontTEMP = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[0]),
                            BackTEMP = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[1]),
                            BackLeftTEMP = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[2]),
                            BackRightTEMP = Convert.ToDecimal(DecodeProtocalDataMODBUSTCP(deviceCode, sentence)[3]),
                            ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime,
                            DeviceNo = deviceCode,
                            Number = number
                        });
                    }
                    break;
            }

            return result;
        }

        /// <summary>
        /// EMARAT AMS信号解析
        /// </summary>
        /// <param name="number">采集系统id</param>
        /// <param name="strDevice">设备代码</param>
        /// <param name="sentArr">指数组</param>
        /// <returns></returns>
        public async Task<IList<LogBook>> DataProccessEMARATAMS(string number, string strDevice, string[] sentArr)
        {
            var result = new List<LogBook>();
            var lstFm = StaticEntities.StaticEntities.Flowmeters[StaticEntities.StaticEntities.Flowmeters.IndexOf(StaticEntities.StaticEntities.Flowmeters.FirstOrDefault(t => t.Number == number))].FlowmeterDtos;
            var lstSh = StaticEntities.StaticEntities.Shafts[StaticEntities.StaticEntities.Shafts.IndexOf(StaticEntities.StaticEntities.Shafts.FirstOrDefault(t => t.Number == number))].ShaftDtos;

            var lstFO = StaticEntities.StaticEntities.FOs[StaticEntities.StaticEntities.FOs.IndexOf(StaticEntities.StaticEntities.FOs.FirstOrDefault(t => t.Number == number))].FODtos;
            var lstLUBOIL = StaticEntities.StaticEntities.LubOils[StaticEntities.StaticEntities.LubOils.IndexOf(StaticEntities.StaticEntities.LubOils.FirstOrDefault(t => t.Number == number))].LubOilDtos;
            var lstCYLINDERLUBOIL = StaticEntities.StaticEntities.CylinderLubOils[StaticEntities.StaticEntities.CylinderLubOils.IndexOf(StaticEntities.StaticEntities.CylinderLubOils.FirstOrDefault(t => t.Number == number))].CylinderLubOilDtos;
            var lstCOOLINGWATER = StaticEntities.StaticEntities.CoolingWaters[StaticEntities.StaticEntities.CoolingWaters.IndexOf(StaticEntities.StaticEntities.CoolingWaters.FirstOrDefault(t => t.Number == number))].CoolingWaterDtos;
            var lstCOMPRESSEDAIRSUPPLY = StaticEntities.StaticEntities.CompressedAirSupplies[StaticEntities.StaticEntities.CompressedAirSupplies.IndexOf(StaticEntities.StaticEntities.CompressedAirSupplies.FirstOrDefault(t => t.Number == number))].CompressedAirSupplyDtos;
            var lstSCAVENGEAIR = StaticEntities.StaticEntities.ScavengeAirs[StaticEntities.StaticEntities.ScavengeAirs.IndexOf(StaticEntities.StaticEntities.ScavengeAirs.FirstOrDefault(t => t.Number == number))].ScavengeAirDtos;
            var lstEXHAUSTGAS = StaticEntities.StaticEntities.ExhaustGases[StaticEntities.StaticEntities.ExhaustGases.IndexOf(StaticEntities.StaticEntities.ExhaustGases.FirstOrDefault(t => t.Number == number))].ExhaustGasDtos;
            var lstMISCELLANEOUS = StaticEntities.StaticEntities.Miscellaneouses[StaticEntities.StaticEntities.Miscellaneouses.IndexOf(StaticEntities.StaticEntities.Miscellaneouses.FirstOrDefault(t => t.Number == number))].MiscellaneousDtos;
            var lstSHAFTCLUTCH = StaticEntities.StaticEntities.ShaftClutchs[StaticEntities.StaticEntities.ShaftClutchs.IndexOf(StaticEntities.StaticEntities.ShaftClutchs.FirstOrDefault(t => t.Number == number))].ShaftClutchDtos;
            var lstMEREMOTECONTROL = StaticEntities.StaticEntities.MERemoteControls[StaticEntities.StaticEntities.MERemoteControls.IndexOf(StaticEntities.StaticEntities.MERemoteControls.FirstOrDefault(t => t.Number == number))].MERemoteControlDtos;
            var lstMAINGENERATORSET = StaticEntities.StaticEntities.MainGeneratorSets[StaticEntities.StaticEntities.MainGeneratorSets.IndexOf(StaticEntities.StaticEntities.MainGeneratorSets.FirstOrDefault(t => t.Number == number))].MainGeneratorSetDtos;
            var lstMAINSWITCHBOARD = StaticEntities.StaticEntities.MainSwitchboards[StaticEntities.StaticEntities.MainSwitchboards.IndexOf(StaticEntities.StaticEntities.MainSwitchboards.FirstOrDefault(t => t.Number == number))].MainSwitchboardDtos;
            var lstCOMPOSITEBOILER = StaticEntities.StaticEntities.CompositeBoilers[StaticEntities.StaticEntities.CompositeBoilers.IndexOf(StaticEntities.StaticEntities.CompositeBoilers.FirstOrDefault(t => t.Number == number))].CompositeBoilerDtos;
            var lstFOSUPPLYUNIT = StaticEntities.StaticEntities.FOSupplyUnits[StaticEntities.StaticEntities.FOSupplyUnits.IndexOf(StaticEntities.StaticEntities.FOSupplyUnits.FirstOrDefault(t => t.Number == number))].FOSupplyUnitDtos;
            var lstLUBOILPURIFYING = StaticEntities.StaticEntities.LubOilPurifyings[StaticEntities.StaticEntities.LubOilPurifyings.IndexOf(StaticEntities.StaticEntities.LubOilPurifyings.FirstOrDefault(t => t.Number == number))].LubOilPurifyingDtos;
            var lstCOOLINGSEAWATER = StaticEntities.StaticEntities.CoolingSeaWaters[StaticEntities.StaticEntities.CoolingSeaWaters.IndexOf(StaticEntities.StaticEntities.CoolingSeaWaters.FirstOrDefault(t => t.Number == number))].CoolingSeaWaterDtos;
            var lstCOOLINGFRESHWATER = StaticEntities.StaticEntities.CoolingFreshWaters[StaticEntities.StaticEntities.CoolingFreshWaters.IndexOf(StaticEntities.StaticEntities.CoolingFreshWaters.FirstOrDefault(t => t.Number == number))].CoolingFreshWaterDtos;

            var currentVessel = StaticEntities.StaticEntities.Vessels.FirstOrDefault(t => t.SN == number);
            var indexVessel = StaticEntities.StaticEntities.Vessels.IndexOf(currentVessel);

            var monitoredDevicesItem = StaticEntities.StaticEntities.MonitoredDevices.FirstOrDefault(t => t.Number == number);
            var monitoredDevicesIndex = StaticEntities.StaticEntities.MonitoredDevices.IndexOf(monitoredDevicesItem);

            switch (strDevice)
            {
                case "FOS":
                    try
                    {
                        var MEInPressure = Convert.ToDouble(sentArr[1]);
                        var MEInTemp = Convert.ToDouble(sentArr[2]);
                        var MEHPOPLeakage = Convert.ToInt16(sentArr[3]);
                        if (lstFO.Any(t => t.Number == number && t.DeviceNo == strDevice))
                        {
                            var fo = lstFO.First(t => t.Number == number && t.DeviceNo == strDevice);
                            var index = lstFO.IndexOf(fo);
                            lstFO[index].MEInPressure = MEInPressure;
                            lstFO[index].MEInTemp = MEInTemp;
                            lstFO[index].MEHPOPLeakage = MEHPOPLeakage;
                            lstFO[index].Number = number;
                            lstFO[index].ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime;
                            lstFO[index].DeviceNo = strDevice;
                            lstFO[index].Uploaded = 0;
                        }
                        else
                        {
                            lstFO.Add(new FODto
                            {
                                MEInPressure = MEInPressure,
                                MEInTemp = MEInTemp,
                                MEHPOPLeakage = MEHPOPLeakage,
                                Number = number,
                                ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime,
                                DeviceNo = strDevice,
                                Uploaded = 0
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "ams燃油系统错误=>" + MethodBase.GetCurrentMethod().DeclaringType.Namespace + "_" + MethodBase.GetCurrentMethod().Name + "_" + sentArr);
                    }
                    break;

                case "LUBOIL":
                    try
                    {
                        var METCInPress = Convert.ToDouble(sentArr[1]);
                        var METBSTemp = Convert.ToDouble(sentArr[2]);
                        var MEMBTBInPress = Convert.ToDouble(sentArr[3]);
                        var MEPistonCOInPress = Convert.ToDouble(sentArr[4]);
                        var MEInTemp = Convert.ToDouble(sentArr[5]);
                        var MECYL1PistonCOOutTemp = Convert.ToDouble(sentArr[6]);
                        var MECYL2PistonCOOutTemp = Convert.ToDouble(sentArr[7]);
                        var MECYL3PistonCOOutTemp = Convert.ToDouble(sentArr[8]);
                        var MECYL4PistonCOOutTemp = Convert.ToDouble(sentArr[9]);
                        var MECYL5PistonCOOutTemp = Convert.ToDouble(sentArr[10]);
                        var MECYL6PistonCOOutTemp = Convert.ToDouble(sentArr[11]);
                        var MECYL1PistonCOOutNoFlow = Convert.ToInt16(sentArr[12]);
                        var MECYL2PistonCOOutNoFlow = Convert.ToInt16(sentArr[13]);
                        var MECYL3PistonCOOutNoFlow = Convert.ToInt16(sentArr[14]);
                        var MECYL4PistonCOOutNoFlow = Convert.ToInt16(sentArr[15]);
                        var MECYL5PistonCOOutNoFlow = Convert.ToInt16(sentArr[16]);
                        var MECYL6PistonCOOutNoFlow = Convert.ToInt16(sentArr[17]);
                        var METCOutTemp = Convert.ToDouble(sentArr[18]);
                        var MEWaterHigh = Convert.ToInt16(sentArr[19]);

                        if (lstLUBOIL.Any(t => t.Number == number && t.DeviceNo == strDevice))
                        {
                            var LUBOIL = lstLUBOIL.First(t => t.Number == number && t.DeviceNo == strDevice);
                            var index = lstLUBOIL.IndexOf(LUBOIL);

                            lstLUBOIL[index].METCInPress = METCInPress;
                            lstLUBOIL[index].METBSTemp = METBSTemp;
                            lstLUBOIL[index].MEMBTBInPress = MEMBTBInPress;
                            lstLUBOIL[index].MEPistonCOInPress = MEPistonCOInPress;
                            lstLUBOIL[index].MEInTemp = MEInTemp;
                            lstLUBOIL[index].MECYL1PistonCOOutTemp = MECYL1PistonCOOutTemp;
                            lstLUBOIL[index].MECYL2PistonCOOutTemp = MECYL2PistonCOOutTemp;
                            lstLUBOIL[index].MECYL3PistonCOOutTemp = MECYL3PistonCOOutTemp;
                            lstLUBOIL[index].MECYL4PistonCOOutTemp = MECYL4PistonCOOutTemp;
                            lstLUBOIL[index].MECYL5PistonCOOutTemp = MECYL5PistonCOOutTemp;
                            lstLUBOIL[index].MECYL6PistonCOOutTemp = MECYL6PistonCOOutTemp;
                            lstLUBOIL[index].MECYL1PistonCOOutNoFlow = MECYL1PistonCOOutNoFlow;
                            lstLUBOIL[index].MECYL2PistonCOOutNoFlow = MECYL2PistonCOOutNoFlow;
                            lstLUBOIL[index].MECYL3PistonCOOutNoFlow = MECYL3PistonCOOutNoFlow;
                            lstLUBOIL[index].MECYL4PistonCOOutNoFlow = MECYL4PistonCOOutNoFlow;
                            lstLUBOIL[index].MECYL5PistonCOOutNoFlow = MECYL5PistonCOOutNoFlow;
                            lstLUBOIL[index].MECYL6PistonCOOutNoFlow = MECYL6PistonCOOutNoFlow;
                            lstLUBOIL[index].METCOutTemp = METCOutTemp;
                            lstLUBOIL[index].MEWaterHigh = MEWaterHigh;

                            lstLUBOIL[index].Number = number;
                            lstLUBOIL[index].ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime;
                            lstLUBOIL[index].DeviceNo = strDevice;
                            lstLUBOIL[index].Uploaded = 0;
                        }
                        else
                        {
                            lstLUBOIL.Add(new LubOilDto
                            {
                                METCInPress = METCInPress,
                                METBSTemp = METBSTemp,
                                MEMBTBInPress = MEMBTBInPress,
                                MEPistonCOInPress = MEPistonCOInPress,
                                MEInTemp = MEInTemp,
                                MECYL1PistonCOOutTemp = MECYL1PistonCOOutTemp,
                                MECYL2PistonCOOutTemp = MECYL2PistonCOOutTemp,
                                MECYL3PistonCOOutTemp = MECYL3PistonCOOutTemp,
                                MECYL4PistonCOOutTemp = MECYL4PistonCOOutTemp,
                                MECYL5PistonCOOutTemp = MECYL5PistonCOOutTemp,
                                MECYL6PistonCOOutTemp = MECYL6PistonCOOutTemp,
                                MECYL1PistonCOOutNoFlow = MECYL1PistonCOOutNoFlow,
                                MECYL2PistonCOOutNoFlow = MECYL2PistonCOOutNoFlow,
                                MECYL3PistonCOOutNoFlow = MECYL3PistonCOOutNoFlow,
                                MECYL4PistonCOOutNoFlow = MECYL4PistonCOOutNoFlow,
                                MECYL5PistonCOOutNoFlow = MECYL5PistonCOOutNoFlow,
                                MECYL6PistonCOOutNoFlow = MECYL6PistonCOOutNoFlow,
                                METCOutTemp = METCOutTemp,
                                MEWaterHigh = MEWaterHigh,

                                Number = number,
                                ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime,
                                DeviceNo = strDevice,
                                Uploaded = 0
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "ams滑油系统错误=>" + MethodBase.GetCurrentMethod().DeclaringType.Namespace + "_" + MethodBase.GetCurrentMethod().Name + "_" + sentArr);
                    }
                    break;

                case "CYLINDERLUBOIL":
                    try
                    {
                        var MEInTemp = Convert.ToDouble(sentArr[1]);
                        if (lstCYLINDERLUBOIL.Any(t => t.Number == number && t.DeviceNo == strDevice))
                        {
                            var CYLINDERLUBOIL = lstCYLINDERLUBOIL.First(t => t.Number == number && t.DeviceNo == strDevice);
                            var index = lstCYLINDERLUBOIL.IndexOf(CYLINDERLUBOIL);
                            lstCYLINDERLUBOIL[index].MEInTemp = MEInTemp;
                            lstCYLINDERLUBOIL[index].Number = number;
                            lstCYLINDERLUBOIL[index].ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime;
                            lstCYLINDERLUBOIL[index].DeviceNo = strDevice;
                            lstCYLINDERLUBOIL[index].Uploaded = 0;
                        }
                        else
                        {
                            lstCYLINDERLUBOIL.Add(new CylinderLubOilDto
                            {
                                MEInTemp = MEInTemp,
                                Number = number,
                                ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime,
                                DeviceNo = strDevice,
                                Uploaded = 0
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "ams主机气缸滑油系统错误=>" + MethodBase.GetCurrentMethod().DeclaringType.Namespace + "_" + MethodBase.GetCurrentMethod().Name + "_" + sentArr);
                    }
                    break;

                case "COOLINGWATER":
                    try
                    {
                        var MEJacketInPress = Convert.ToDouble(sentArr[1]);
                        var MEPressDrop = Convert.ToDouble(sentArr[2]);
                        var MEOutPress = Convert.ToDouble(sentArr[3]);
                        var MEJacketPressDrop = Convert.ToDouble(sentArr[4]);
                        var MEInTemp = Convert.ToDouble(sentArr[5]);
                        var MEJacketCyl1OutTemp = Convert.ToDouble(sentArr[6]);
                        var MEJacketCyl2OutTemp = Convert.ToDouble(sentArr[7]);
                        var MEJacketCyl3OutTemp = Convert.ToDouble(sentArr[8]);
                        var MEJacketCyl4OutTemp = Convert.ToDouble(sentArr[9]);
                        var MEJacketCyl5OutTemp = Convert.ToDouble(sentArr[10]);
                        var MEJacketCyl6OutTemp = Convert.ToDouble(sentArr[11]);
                        var MECCCyl1OutTemp = Convert.ToDouble(sentArr[12]);
                        var MECCCyl2OutTemp = Convert.ToDouble(sentArr[13]);
                        var MECCCyl3OutTemp = Convert.ToDouble(sentArr[14]);
                        var MECCCyl4OutTemp = Convert.ToDouble(sentArr[15]);
                        var MECCCyl5OutTemp = Convert.ToDouble(sentArr[16]);
                        var MECCCyl6OutTemp = Convert.ToDouble(sentArr[17]);
                        var MEACInPress = Convert.ToDouble(sentArr[18]);
                        var MEACInTemp = Convert.ToDouble(sentArr[19]);
                        var MEACOutTemp = Convert.ToDouble(sentArr[20]);

                        if (lstCOOLINGWATER.Any(t => t.Number == number && t.DeviceNo == strDevice))
                        {
                            var COOLINGWATER = lstCOOLINGWATER.First(t => t.Number == number && t.DeviceNo == strDevice);
                            var index = lstCOOLINGWATER.IndexOf(COOLINGWATER);

                            lstCOOLINGWATER[index].MEJacketInPress = MEJacketInPress;
                            lstCOOLINGWATER[index].MEPressDrop = MEPressDrop;
                            lstCOOLINGWATER[index].MEOutPress = MEOutPress;
                            lstCOOLINGWATER[index].MEJacketPressDrop = MEJacketPressDrop;
                            lstCOOLINGWATER[index].MEInTemp = MEInTemp;
                            lstCOOLINGWATER[index].MEJacketCyl1OutTemp = MEJacketCyl1OutTemp;
                            lstCOOLINGWATER[index].MEJacketCyl2OutTemp = MEJacketCyl2OutTemp;
                            lstCOOLINGWATER[index].MEJacketCyl3OutTemp = MEJacketCyl3OutTemp;
                            lstCOOLINGWATER[index].MEJacketCyl4OutTemp = MEJacketCyl4OutTemp;
                            lstCOOLINGWATER[index].MEJacketCyl5OutTemp = MEJacketCyl5OutTemp;
                            lstCOOLINGWATER[index].MEJacketCyl6OutTemp = MEJacketCyl6OutTemp;
                            lstCOOLINGWATER[index].MECCCyl1OutTemp = MECCCyl1OutTemp;
                            lstCOOLINGWATER[index].MECCCyl2OutTemp = MECCCyl2OutTemp;
                            lstCOOLINGWATER[index].MECCCyl3OutTemp = MECCCyl3OutTemp;
                            lstCOOLINGWATER[index].MECCCyl4OutTemp = MECCCyl4OutTemp;
                            lstCOOLINGWATER[index].MECCCyl5OutTemp = MECCCyl5OutTemp;
                            lstCOOLINGWATER[index].MECCCyl6OutTemp = MECCCyl6OutTemp;
                            lstCOOLINGWATER[index].MEACInPress = MEACInPress;
                            lstCOOLINGWATER[index].MEACInTemp = MEACInTemp;
                            lstCOOLINGWATER[index].MEACOutTemp = MEACOutTemp;

                            lstCOOLINGWATER[index].Number = number;
                            lstCOOLINGWATER[index].ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime;
                            lstCOOLINGWATER[index].DeviceNo = strDevice;
                            lstCOOLINGWATER[index].Uploaded = 0;
                        }
                        else
                        {
                            lstCOOLINGWATER.Add(new CoolingWaterDto
                            {
                                MEJacketInPress = MEJacketInPress,
                                MEPressDrop = MEPressDrop,
                                MEOutPress = MEOutPress,
                                MEJacketPressDrop = MEJacketPressDrop,
                                MEInTemp = MEInTemp,
                                MEJacketCyl1OutTemp = MEJacketCyl1OutTemp,
                                MEJacketCyl2OutTemp = MEJacketCyl2OutTemp,
                                MEJacketCyl3OutTemp = MEJacketCyl3OutTemp,
                                MEJacketCyl4OutTemp = MEJacketCyl4OutTemp,
                                MEJacketCyl5OutTemp = MEJacketCyl5OutTemp,
                                MEJacketCyl6OutTemp = MEJacketCyl6OutTemp,
                                MECCCyl1OutTemp = MECCCyl1OutTemp,
                                MECCCyl2OutTemp = MECCCyl2OutTemp,
                                MECCCyl3OutTemp = MECCCyl3OutTemp,
                                MECCCyl4OutTemp = MECCCyl4OutTemp,
                                MECCCyl5OutTemp = MECCCyl5OutTemp,
                                MECCCyl6OutTemp = MECCCyl6OutTemp,
                                MEACInPress = MEACInPress,
                                MEACInTemp = MEACInTemp,
                                MEACOutTemp = MEACOutTemp,

                                Number = number,
                                ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime,
                                DeviceNo = strDevice,
                                Uploaded = 0
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "ams主机冷却水系统错误=>" + MethodBase.GetCurrentMethod().DeclaringType.Namespace + "_" + MethodBase.GetCurrentMethod().Name + "_" + sentArr);
                    }
                    break;

                case "COMPRESSEDAIRSUPPLY":
                    try
                    {
                        var MEStartPress = Convert.ToDouble(sentArr[1]);
                        var MEControlPress = Convert.ToDouble(sentArr[2]);
                        var ExhaustValuePress = Convert.ToDouble(sentArr[3]);

                        if (lstCOMPRESSEDAIRSUPPLY.Any(t => t.Number == number && t.DeviceNo == strDevice))
                        {
                            var COMPRESSEDAIRSUPPLY = lstCOMPRESSEDAIRSUPPLY.First(t => t.Number == number && t.DeviceNo == strDevice);
                            var index = lstCOMPRESSEDAIRSUPPLY.IndexOf(COMPRESSEDAIRSUPPLY);

                            lstCOMPRESSEDAIRSUPPLY[index].MEStartPress = MEStartPress;
                            lstCOMPRESSEDAIRSUPPLY[index].MEControlPress = MEControlPress;
                            lstCOMPRESSEDAIRSUPPLY[index].ExhaustValuePress = ExhaustValuePress;

                            lstCOMPRESSEDAIRSUPPLY[index].Number = number;
                            lstCOMPRESSEDAIRSUPPLY[index].ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime;
                            lstCOMPRESSEDAIRSUPPLY[index].DeviceNo = strDevice;
                            lstCOMPRESSEDAIRSUPPLY[index].Uploaded = 0;
                        }
                        else
                        {
                            lstCOMPRESSEDAIRSUPPLY.Add(new CompressedAirSupplyDto
                            {
                                MEStartPress = MEStartPress,
                                MEControlPress = MEControlPress,
                                ExhaustValuePress = ExhaustValuePress,

                                Number = number,
                                ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime,
                                DeviceNo = strDevice,
                                Uploaded = 0
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "ams主机压缩空气系统错误=>" + MethodBase.GetCurrentMethod().DeclaringType.Namespace + "_" + MethodBase.GetCurrentMethod().Name + "_" + sentArr);
                    }
                    break;

                case "SCAVENGEAIR":
                    try
                    {
                        var MEReceiverTemp = Convert.ToDouble(sentArr[1]);
                        var MEFBCyl1Temp = Convert.ToDouble(sentArr[2]);
                        var MEFBCyl2Temp = Convert.ToDouble(sentArr[3]);
                        var MEFBCyl3Temp = Convert.ToDouble(sentArr[4]);
                        var MEFBCyl4Temp = Convert.ToDouble(sentArr[5]);
                        var MEFBCyl5Temp = Convert.ToDouble(sentArr[6]);
                        var MEFBCyl6Temp = Convert.ToDouble(sentArr[7]);
                        var MEPress = Convert.ToDouble(sentArr[8]);
                        var MECoolerPressDrop = Convert.ToDouble(sentArr[9]);
                        var METCInTempA = Convert.ToDouble(sentArr[10]);
                        var METCInTempB = Convert.ToDouble(sentArr[11]);
                        var ERRelativeHumidity = Convert.ToDouble(sentArr[12]);
                        var ERTemp = Convert.ToDouble(sentArr[13]);
                        var ERAmbientPress = Convert.ToDouble(sentArr[14]);

                        if (lstSCAVENGEAIR.Any(t => t.Number == number && t.DeviceNo == strDevice))
                        {
                            var SCAVENGEAIR = lstSCAVENGEAIR.First(t => t.Number == number && t.DeviceNo == strDevice);
                            var index = lstSCAVENGEAIR.IndexOf(SCAVENGEAIR);

                            lstSCAVENGEAIR[index].MEReceiverTemp = MEReceiverTemp;
                            lstSCAVENGEAIR[index].MEFBCyl1Temp = MEFBCyl1Temp;
                            lstSCAVENGEAIR[index].MEFBCyl2Temp = MEFBCyl2Temp;
                            lstSCAVENGEAIR[index].MEFBCyl3Temp = MEFBCyl3Temp;
                            lstSCAVENGEAIR[index].MEFBCyl4Temp = MEFBCyl4Temp;
                            lstSCAVENGEAIR[index].MEFBCyl5Temp = MEFBCyl5Temp;
                            lstSCAVENGEAIR[index].MEFBCyl6Temp = MEFBCyl6Temp;
                            lstSCAVENGEAIR[index].MEPress = MEPress;
                            lstSCAVENGEAIR[index].MECoolerPressDrop = MECoolerPressDrop;
                            lstSCAVENGEAIR[index].METCInTempA = METCInTempA;
                            lstSCAVENGEAIR[index].METCInTempB = METCInTempB;
                            lstSCAVENGEAIR[index].ERRelativeHumidity = ERRelativeHumidity;
                            lstSCAVENGEAIR[index].ERTemp = ERTemp;
                            lstSCAVENGEAIR[index].ERAmbientPress = ERAmbientPress;

                            lstSCAVENGEAIR[index].Number = number;
                            lstSCAVENGEAIR[index].ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime;
                            lstSCAVENGEAIR[index].DeviceNo = strDevice;
                            lstSCAVENGEAIR[index].Uploaded = 0;
                        }
                        else
                        {
                            lstSCAVENGEAIR.Add(new ScavengeAirDto
                            {
                                MEReceiverTemp = MEReceiverTemp,
                                MEFBCyl1Temp = MEFBCyl1Temp,
                                MEFBCyl2Temp = MEFBCyl2Temp,
                                MEFBCyl3Temp = MEFBCyl3Temp,
                                MEFBCyl4Temp = MEFBCyl4Temp,
                                MEFBCyl5Temp = MEFBCyl5Temp,
                                MEFBCyl6Temp = MEFBCyl6Temp,
                                MEPress = MEPress,
                                MECoolerPressDrop = MECoolerPressDrop,
                                METCInTempA = METCInTempA,
                                METCInTempB = METCInTempB,
                                ERRelativeHumidity = ERRelativeHumidity,
                                ERTemp = ERTemp,
                                ERAmbientPress = ERAmbientPress,

                                Number = number,
                                ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime,
                                DeviceNo = strDevice,
                                Uploaded = 0
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "ams主机扫气系统错误=>" + MethodBase.GetCurrentMethod().DeclaringType.Namespace + "_" + MethodBase.GetCurrentMethod().Name + "_" + sentArr);
                    }
                    break;

                case "EXHAUSTGAS":
                    try
                    {
                        var METCInTemp = Convert.ToDouble(sentArr[1]);
                        var MECyl1AfterTemp = Convert.ToDouble(sentArr[13]);
                        var MECyl2AfterTemp = Convert.ToDouble(sentArr[14]);
                        var MECyl3AfterTemp = Convert.ToDouble(sentArr[15]);
                        var MECyl4AfterTemp = Convert.ToDouble(sentArr[16]);
                        var MECyl5AfterTemp = Convert.ToDouble(sentArr[17]);
                        var MECyl6AfterTemp = Convert.ToDouble(sentArr[18]);
                        var METCOutTemp = Convert.ToDouble(sentArr[8]);
                        var MEReceiverPress = Convert.ToDouble(sentArr[9]);
                        var METurbBackPress = Convert.ToDouble(sentArr[10]);
                        var MEACInTemp = Convert.ToDouble(sentArr[11]);
                        var MEACOutTemp = Convert.ToDouble(sentArr[12]);
                        var MECyl1AfterTempDev = Convert.ToDouble(sentArr[2]);
                        var MECyl2AfterTempDev = Convert.ToDouble(sentArr[3]);
                        var MECyl3AfterTempDev = Convert.ToDouble(sentArr[4]);
                        var MECyl4AfterTempDev = Convert.ToDouble(sentArr[5]);
                        var MECyl5AfterTempDev = Convert.ToDouble(sentArr[6]);
                        var MECyl6AfterTempDev = Convert.ToDouble(sentArr[7]);

                        if (lstEXHAUSTGAS.Any(t => t.Number == number && t.DeviceNo == strDevice))
                        {
                            var EXHAUSTGAS = lstEXHAUSTGAS.First(t => t.Number == number && t.DeviceNo == strDevice);
                            var index = lstEXHAUSTGAS.IndexOf(EXHAUSTGAS);

                            lstEXHAUSTGAS[index].METCInTemp = METCInTemp;
                            lstEXHAUSTGAS[index].MECyl1AfterTemp = MECyl1AfterTemp;
                            lstEXHAUSTGAS[index].MECyl2AfterTemp = MECyl2AfterTemp;
                            lstEXHAUSTGAS[index].MECyl3AfterTemp = MECyl3AfterTemp;
                            lstEXHAUSTGAS[index].MECyl4AfterTemp = MECyl4AfterTemp;
                            lstEXHAUSTGAS[index].MECyl5AfterTemp = MECyl5AfterTemp;
                            lstEXHAUSTGAS[index].MECyl6AfterTemp = MECyl6AfterTemp;
                            lstEXHAUSTGAS[index].METCOutTemp = METCOutTemp;
                            lstEXHAUSTGAS[index].MEReceiverPress = MEReceiverPress;
                            lstEXHAUSTGAS[index].METurbBackPress = METurbBackPress;
                            lstEXHAUSTGAS[index].MEACInTemp = MEACInTemp;
                            lstEXHAUSTGAS[index].MEACOutTemp = MEACOutTemp;
                            lstEXHAUSTGAS[index].MECyl1AfterTempDev = MECyl1AfterTempDev;
                            lstEXHAUSTGAS[index].MECyl2AfterTempDev = MECyl2AfterTempDev;
                            lstEXHAUSTGAS[index].MECyl3AfterTempDev = MECyl3AfterTempDev;
                            lstEXHAUSTGAS[index].MECyl4AfterTempDev = MECyl4AfterTempDev;
                            lstEXHAUSTGAS[index].MECyl5AfterTempDev = MECyl5AfterTempDev;
                            lstEXHAUSTGAS[index].MECyl6AfterTempDev = MECyl6AfterTempDev;

                            lstEXHAUSTGAS[index].Number = number;
                            lstEXHAUSTGAS[index].ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime;
                            lstEXHAUSTGAS[index].DeviceNo = strDevice;
                            lstEXHAUSTGAS[index].Uploaded = 0;
                        }
                        else
                        {
                            lstEXHAUSTGAS.Add(new ExhaustGasDto
                            {
                                METCInTemp = METCInTemp,
                                MECyl1AfterTemp = MECyl1AfterTemp,
                                MECyl2AfterTemp = MECyl2AfterTemp,
                                MECyl3AfterTemp = MECyl3AfterTemp,
                                MECyl4AfterTemp = MECyl4AfterTemp,
                                MECyl5AfterTemp = MECyl5AfterTemp,
                                MECyl6AfterTemp = MECyl6AfterTemp,
                                METCOutTemp = METCOutTemp,
                                MEReceiverPress = MEReceiverPress,
                                METurbBackPress = METurbBackPress,
                                MEACInTemp = MEACInTemp,
                                MEACOutTemp = MEACOutTemp,
                                MECyl1AfterTempDev = MECyl1AfterTempDev,
                                MECyl2AfterTempDev = MECyl2AfterTempDev,
                                MECyl3AfterTempDev = MECyl3AfterTempDev,
                                MECyl4AfterTempDev = MECyl4AfterTempDev,
                                MECyl5AfterTempDev = MECyl5AfterTempDev,
                                MECyl6AfterTempDev = MECyl6AfterTempDev,

                                Number = number,
                                ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime,
                                DeviceNo = strDevice,
                                Uploaded = 0
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "ams排气系统错误=>" + MethodBase.GetCurrentMethod().DeclaringType.Namespace + "_" + MethodBase.GetCurrentMethod().Name + "_" + sentArr);
                    }
                    break;

                case "MISCELLANEOUS":
                    try
                    {
                        var MECCOMHigh = Convert.ToInt16(sentArr[1]);
                        var MEAxialVibration = Convert.ToDouble(sentArr[2]);
                        var MELoad = Convert.ToDouble(sentArr[4]);
                        var METCSpeed = Convert.ToDouble(sentArr[5]);

                        if (lstMISCELLANEOUS.Any(t => t.Number == number && t.DeviceNo == strDevice))
                        {
                            var MISCELLANEOUS = lstMISCELLANEOUS.First(t => t.Number == number && t.DeviceNo == strDevice);
                            var index = lstMISCELLANEOUS.IndexOf(MISCELLANEOUS);

                            lstMISCELLANEOUS[index].MECCOMHigh = MECCOMHigh;
                            lstMISCELLANEOUS[index].MEAxialVibration = MEAxialVibration;
                            lstMISCELLANEOUS[index].MELoad = MELoad;
                            lstMISCELLANEOUS[index].METCSpeed = METCSpeed;

                            lstMISCELLANEOUS[index].Number = number;
                            lstMISCELLANEOUS[index].ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime;
                            lstMISCELLANEOUS[index].DeviceNo = strDevice;
                            lstMISCELLANEOUS[index].Uploaded = 0;
                        }
                        else
                        {
                            lstMISCELLANEOUS.Add(new MiscellaneousDto
                            {
                                MECCOMHigh = MECCOMHigh,
                                MEAxialVibration = MEAxialVibration,
                                MELoad = MELoad,
                                METCSpeed = METCSpeed,

                                Number = number,
                                ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime,
                                DeviceNo = strDevice,
                                Uploaded = 0
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "ams其它系统错误=>" + MethodBase.GetCurrentMethod().DeclaringType.Namespace + "_" + MethodBase.GetCurrentMethod().Name + "_" + sentArr);
                    }
                    break;

                case "SHAFTCLUTCH":
                    try
                    {
                        var SternAftTemp = Convert.ToDouble(sentArr[1]);
                        var InterTemp = Convert.ToDouble(sentArr[2]);

                        if (lstSHAFTCLUTCH.Any(t => t.Number == number && t.DeviceNo == strDevice))
                        {
                            var SHAFTCLUTCH = lstSHAFTCLUTCH.First(t => t.Number == number && t.DeviceNo == strDevice);
                            var index = lstSHAFTCLUTCH.IndexOf(SHAFTCLUTCH);

                            lstSHAFTCLUTCH[index].SternAftTemp = SternAftTemp;
                            lstSHAFTCLUTCH[index].InterTemp = InterTemp;

                            lstSHAFTCLUTCH[index].Number = number;
                            lstSHAFTCLUTCH[index].ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime;
                            lstSHAFTCLUTCH[index].DeviceNo = strDevice;
                            lstSHAFTCLUTCH[index].Uploaded = 0;
                        }
                        else
                        {
                            lstSHAFTCLUTCH.Add(new ShaftClutchDto
                            {
                                SternAftTemp = SternAftTemp,
                                InterTemp = InterTemp,

                                Number = number,
                                ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime,
                                DeviceNo = strDevice,
                                Uploaded = 0
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "ams推进机械轴系与离合器系统错误=>" + MethodBase.GetCurrentMethod().DeclaringType.Namespace + "_" + MethodBase.GetCurrentMethod().Name + "_" + sentArr);
                    }
                    break;

                case "MEREMOTECONTROL":
                    try
                    {
                        var MERpm = Convert.ToDouble(sentArr[1]);

                        if (lstMEREMOTECONTROL.Any(t => t.Number == number && t.DeviceNo == strDevice))
                        {
                            var MEREMOTECONTROL = lstMEREMOTECONTROL.First(t => t.Number == number && t.DeviceNo == strDevice);
                            var index = lstMEREMOTECONTROL.IndexOf(MEREMOTECONTROL);

                            lstMEREMOTECONTROL[index].MERpm = MERpm;

                            lstMEREMOTECONTROL[index].Number = number;
                            lstMEREMOTECONTROL[index].ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime;
                            lstMEREMOTECONTROL[index].DeviceNo = strDevice;
                            lstMEREMOTECONTROL[index].Uploaded = 0;
                        }
                        else
                        {
                            lstMEREMOTECONTROL.Add(new MERemoteControlDto
                            {
                                MERpm = MERpm,

                                Number = number,
                                ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime,
                                DeviceNo = strDevice,
                                Uploaded = 0
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "ams主机遥控系统错误=>" + MethodBase.GetCurrentMethod().DeclaringType.Namespace + "_" + MethodBase.GetCurrentMethod().Name + "_" + sentArr);
                    }
                    break;

                case "MAINGENERATORSET1":
                case "MAINGENERATORSET2":
                case "MAINGENERATORSET3":
                    try
                    {
                        var DGCFWInPress = Convert.ToDouble(sentArr[1]);
                        var DGStartAirPress = Convert.ToDouble(sentArr[2]);
                        var DGLOPress = Convert.ToDouble(sentArr[3]);
                        var DGLOInTemp = Convert.ToDouble(sentArr[4]);
                        var DGCFWOutTemp = Convert.ToDouble(sentArr[5]);
                        var DGEGTC1To3InTemp = Convert.ToDouble(sentArr[6]);
                        var DGEGTC4To6InTemp = Convert.ToDouble(sentArr[7]);
                        var DGEngineSpeed = Convert.ToInt16(sentArr[8]);
                        var DGEngineLoad = Convert.ToInt16(sentArr[9]);
                        var DGEngineRunHour = Convert.ToInt16(sentArr[10]);
                        var DGLOInPress = Convert.ToInt16(sentArr[12]);
                        var DGLOFilterInPress = Convert.ToInt16(sentArr[13]);
                        var DGControlAirPress = Convert.ToInt16(sentArr[14]);
                        var DGTCLOPress = Convert.ToInt16(sentArr[15]);
                        var DGEngineRunning = Convert.ToInt16(sentArr[16]);
                        var DGUTemp = Convert.ToDouble(sentArr[17]);
                        var DGVTemp = Convert.ToDouble(sentArr[18]);
                        var DGWTemp = Convert.ToDouble(sentArr[19]);
                        var DGBTDTemp = Convert.ToDouble(sentArr[20]);
                        var DGCyl1ExTemp = Convert.ToInt16(sentArr[21]);
                        var DGCyl2ExTemp = Convert.ToInt16(sentArr[22]);
                        var DGCyl3ExTemp = Convert.ToInt16(sentArr[23]);
                        var DGCyl4ExTemp = Convert.ToInt16(sentArr[24]);
                        var DGCyl5ExTemp = Convert.ToInt16(sentArr[25]);
                        var DGCyl6ExTemp = Convert.ToInt16(sentArr[26]);
                        var DGTCEXOutTemp = Convert.ToInt16(sentArr[27]);
                        var DGBoostAirPress = Convert.ToDouble(sentArr[28]);
                        var DGFOInPress = Convert.ToDouble(sentArr[29]);

                        if (lstMAINGENERATORSET.Any(t => t.Number == number && t.DeviceNo == strDevice))
                        {
                            var MAINGENERATORSET = lstMAINGENERATORSET.First(t => t.Number == number && t.DeviceNo == strDevice);
                            var index = lstMAINGENERATORSET.IndexOf(MAINGENERATORSET);

                            lstMAINGENERATORSET[index].DGCFWInPress = DGCFWInPress;
                            lstMAINGENERATORSET[index].DGStartAirPress = DGStartAirPress;
                            lstMAINGENERATORSET[index].DGLOPress = DGLOPress;
                            lstMAINGENERATORSET[index].DGLOInTemp = DGLOInTemp;
                            lstMAINGENERATORSET[index].DGCFWOutTemp = DGCFWOutTemp;
                            lstMAINGENERATORSET[index].DGEGTC1To3InTemp = DGEGTC1To3InTemp;
                            lstMAINGENERATORSET[index].DGEGTC4To6InTemp = DGEGTC4To6InTemp;
                            lstMAINGENERATORSET[index].DGEngineSpeed = DGEngineSpeed;
                            lstMAINGENERATORSET[index].DGEngineLoad = DGEngineLoad;
                            lstMAINGENERATORSET[index].DGEngineRunHour = DGEngineRunHour;
                            lstMAINGENERATORSET[index].DGLOInPress = DGLOInPress;
                            lstMAINGENERATORSET[index].DGLOFilterInPress = DGLOFilterInPress;
                            lstMAINGENERATORSET[index].DGControlAirPress = DGControlAirPress;
                            lstMAINGENERATORSET[index].DGTCLOPress = DGTCLOPress;
                            lstMAINGENERATORSET[index].DGEngineRunning = DGEngineRunning;
                            lstMAINGENERATORSET[index].DGUTemp = DGUTemp;
                            lstMAINGENERATORSET[index].DGVTemp = DGVTemp;
                            lstMAINGENERATORSET[index].DGWTemp = DGWTemp;
                            lstMAINGENERATORSET[index].DGBTDTemp = DGBTDTemp;
                            lstMAINGENERATORSET[index].DGCyl1ExTemp = DGCyl1ExTemp;
                            lstMAINGENERATORSET[index].DGCyl2ExTemp = DGCyl2ExTemp;
                            lstMAINGENERATORSET[index].DGCyl3ExTemp = DGCyl3ExTemp;
                            lstMAINGENERATORSET[index].DGCyl4ExTemp = DGCyl4ExTemp;
                            lstMAINGENERATORSET[index].DGCyl5ExTemp = DGCyl5ExTemp;
                            lstMAINGENERATORSET[index].DGCyl6ExTemp = DGCyl6ExTemp;
                            lstMAINGENERATORSET[index].DGTCEXOutTemp = DGTCEXOutTemp;
                            lstMAINGENERATORSET[index].DGBoostAirPress = DGBoostAirPress;
                            lstMAINGENERATORSET[index].DGFOInPress = DGFOInPress;

                            lstMAINGENERATORSET[index].Number = number;
                            lstMAINGENERATORSET[index].ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime;
                            lstMAINGENERATORSET[index].DeviceNo = strDevice;
                            lstMAINGENERATORSET[index].Uploaded = 0;
                        }
                        else
                        {
                            lstMAINGENERATORSET.Add(new MainGeneratorSetDto
                            {
                                DGCFWInPress = DGCFWInPress,
                                DGStartAirPress = DGStartAirPress,
                                DGLOPress = DGLOPress,
                                DGLOInTemp = DGLOInTemp,
                                DGCFWOutTemp = DGCFWOutTemp,
                                DGEGTC1To3InTemp = DGEGTC1To3InTemp,
                                DGEGTC4To6InTemp = DGEGTC4To6InTemp,
                                DGEngineSpeed = DGEngineSpeed,
                                DGEngineLoad = DGEngineLoad,
                                DGEngineRunHour = DGEngineRunHour,
                                DGLOInPress = DGLOInPress,
                                DGLOFilterInPress = DGLOFilterInPress,
                                DGControlAirPress = DGControlAirPress,
                                DGTCLOPress = DGTCLOPress,
                                DGEngineRunning = DGEngineRunning,
                                DGUTemp = DGUTemp,
                                DGVTemp = DGVTemp,
                                DGWTemp = DGWTemp,
                                DGBTDTemp = DGBTDTemp,
                                DGCyl1ExTemp = DGCyl1ExTemp,
                                DGCyl2ExTemp = DGCyl2ExTemp,
                                DGCyl3ExTemp = DGCyl3ExTemp,
                                DGCyl4ExTemp = DGCyl4ExTemp,
                                DGCyl5ExTemp = DGCyl5ExTemp,
                                DGCyl6ExTemp = DGCyl6ExTemp,
                                DGTCEXOutTemp = DGTCEXOutTemp,
                                DGBoostAirPress = DGBoostAirPress,
                                DGFOInPress = DGFOInPress,

                                Number = number,
                                ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime,
                                DeviceNo = strDevice,
                                Uploaded = 0
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "ams主发电机错误=>" + MethodBase.GetCurrentMethod().DeclaringType.Namespace + "_" + MethodBase.GetCurrentMethod().Name + "_" + sentArr);
                    }
                    break;

                case "MAINSWITCHBOARD1":
                case "MAINSWITCHBOARD2":
                case "MAINSWITCHBOARD3":
                    try
                    {
                        var MBVoltageHigh = Convert.ToDouble(sentArr[1]);
                        var MBVoltageLow = Convert.ToDouble(sentArr[2]);
                        var MBFrequencyHigh = Convert.ToDouble(sentArr[3]);
                        var MBFrequencyLow = Convert.ToDouble(sentArr[4]);
                        var DGRunning = Convert.ToDouble(sentArr[5]);
                        var DGPower = Convert.ToDouble(sentArr[6]);
                        var DGVoltageL1L2 = Convert.ToDouble(sentArr[7]);
                        var DGVoltageL2L3 = Convert.ToDouble(sentArr[8]);
                        var DGVoltageL3L1 = Convert.ToDouble(sentArr[9]);
                        var DGCurrentL1 = Convert.ToDouble(sentArr[10]);
                        var DGCurrentL2 = Convert.ToDouble(sentArr[11]);
                        var DGCurrentL3 = Convert.ToDouble(sentArr[12]);
                        var DGFrequency = Convert.ToDouble(sentArr[13]);
                        var DGPowerFactor = Convert.ToDouble(sentArr[14]);

                        if (lstMAINSWITCHBOARD.Any(t => t.Number == number && t.DeviceNo == strDevice))
                        {
                            var MAINSWITCHBOARD = lstMAINSWITCHBOARD.First(t => t.Number == number && t.DeviceNo == strDevice);
                            var index = lstMAINSWITCHBOARD.IndexOf(MAINSWITCHBOARD);

                            lstMAINSWITCHBOARD[index].MBVoltageHigh = MBVoltageHigh;
                            lstMAINSWITCHBOARD[index].MBVoltageLow = MBVoltageLow;
                            lstMAINSWITCHBOARD[index].MBFrequencyHigh = MBFrequencyHigh;
                            lstMAINSWITCHBOARD[index].MBFrequencyLow = MBFrequencyLow;
                            lstMAINSWITCHBOARD[index].DGRunning = DGRunning;
                            lstMAINSWITCHBOARD[index].DGPower = DGPower;
                            lstMAINSWITCHBOARD[index].DGVoltageL1L2 = DGVoltageL1L2;
                            lstMAINSWITCHBOARD[index].DGVoltageL2L3 = DGVoltageL2L3;
                            lstMAINSWITCHBOARD[index].DGVoltageL3L1 = DGVoltageL3L1;
                            lstMAINSWITCHBOARD[index].DGCurrentL1 = DGCurrentL1;
                            lstMAINSWITCHBOARD[index].DGCurrentL2 = DGCurrentL2;
                            lstMAINSWITCHBOARD[index].DGCurrentL3 = DGCurrentL3;
                            lstMAINSWITCHBOARD[index].DGFrequency = DGFrequency;
                            lstMAINSWITCHBOARD[index].DGPowerFactor = DGPowerFactor;

                            lstMAINSWITCHBOARD[index].Number = number;
                            lstMAINSWITCHBOARD[index].ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime;
                            lstMAINSWITCHBOARD[index].DeviceNo = strDevice;
                            lstMAINSWITCHBOARD[index].Uploaded = 0;
                        }
                        else
                        {
                            lstMAINSWITCHBOARD.Add(new MainSwitchboardDto
                            {
                                MBVoltageHigh = MBVoltageHigh,
                                MBVoltageLow = MBVoltageLow,
                                MBFrequencyHigh = MBFrequencyHigh,
                                MBFrequencyLow = MBFrequencyLow,
                                DGRunning = DGRunning,
                                DGPower = DGPower,
                                DGVoltageL1L2 = DGVoltageL1L2,
                                DGVoltageL2L3 = DGVoltageL2L3,
                                DGVoltageL3L1 = DGVoltageL3L1,
                                DGCurrentL1 = DGCurrentL1,
                                DGCurrentL2 = DGCurrentL2,
                                DGCurrentL3 = DGCurrentL3,
                                DGFrequency = DGFrequency,
                                DGPowerFactor = DGPowerFactor,

                                Number = number,
                                ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime,
                                DeviceNo = strDevice,
                                Uploaded = 0
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "ams主配电板错误=>" + MethodBase.GetCurrentMethod().DeclaringType.Namespace + "_" + MethodBase.GetCurrentMethod().Name + "_" + sentArr);
                    }
                    break;

                case "COMPOSITEBOILER":
                    try
                    {
                        var BLRBurnerRunning = Convert.ToDouble(sentArr[1]);
                        var BLRHFOService = Convert.ToDouble(sentArr[2]);
                        var BLRDGOService = Convert.ToDouble(sentArr[3]);
                        var BLRFOP1On = Convert.ToDouble(sentArr[4]);
                        var BLRFOP2On = Convert.ToDouble(sentArr[5]);
                        var BLRFOTempLow = Convert.ToDouble(sentArr[6]);
                        var BLRFOPressHigh = Convert.ToDouble(sentArr[7]);
                        var BLRFOTempHigh = Convert.ToDouble(sentArr[8]);
                        var BLRDGOTempHigh = Convert.ToDouble(sentArr[9]);
                        var BLRGE1EXTempHigh = Convert.ToDouble(sentArr[10]);
                        var BLRGE2EXTempHigh = Convert.ToDouble(sentArr[11]);

                        if (lstCOMPOSITEBOILER.Any(t => t.Number == number && t.DeviceNo == strDevice))
                        {
                            var COMPOSITEBOILER = lstCOMPOSITEBOILER.First(t => t.Number == number && t.DeviceNo == strDevice);
                            var index = lstCOMPOSITEBOILER.IndexOf(COMPOSITEBOILER);

                            lstCOMPOSITEBOILER[index].BLRBurnerRunning = BLRBurnerRunning;
                            lstCOMPOSITEBOILER[index].BLRHFOService = BLRHFOService;
                            lstCOMPOSITEBOILER[index].BLRDGOService = BLRDGOService;
                            lstCOMPOSITEBOILER[index].BLRFOP1On = BLRFOP1On;
                            lstCOMPOSITEBOILER[index].BLRFOP2On = BLRFOP2On;
                            lstCOMPOSITEBOILER[index].BLRFOTempLow = BLRFOTempLow;
                            lstCOMPOSITEBOILER[index].BLRFOPressHigh = BLRFOPressHigh;
                            lstCOMPOSITEBOILER[index].BLRFOTempHigh = BLRFOTempHigh;
                            lstCOMPOSITEBOILER[index].BLRDGOTempHigh = BLRDGOTempHigh;
                            lstCOMPOSITEBOILER[index].BLRGE1EXTempHigh = BLRGE1EXTempHigh;
                            lstCOMPOSITEBOILER[index].BLRGE2EXTempHigh = BLRGE2EXTempHigh;

                            lstCOMPOSITEBOILER[index].Number = number;
                            lstCOMPOSITEBOILER[index].ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime;
                            lstCOMPOSITEBOILER[index].DeviceNo = strDevice;
                            lstCOMPOSITEBOILER[index].Uploaded = 0;
                        }
                        else
                        {
                            lstCOMPOSITEBOILER.Add(new CompositeBoilerDto
                            {
                                BLRBurnerRunning = BLRBurnerRunning,
                                BLRHFOService = BLRHFOService,
                                BLRDGOService = BLRDGOService,
                                BLRFOP1On = BLRFOP1On,
                                BLRFOP2On = BLRFOP2On,
                                BLRFOTempLow = BLRFOTempLow,
                                BLRFOPressHigh = BLRFOPressHigh,
                                BLRFOTempHigh = BLRFOTempHigh,
                                BLRDGOTempHigh = BLRDGOTempHigh,
                                BLRGE1EXTempHigh = BLRGE1EXTempHigh,
                                BLRGE2EXTempHigh = BLRGE2EXTempHigh,

                                Number = number,
                                ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime,
                                DeviceNo = strDevice,
                                Uploaded = 0
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "ams燃油废气组合锅炉错误=>" + MethodBase.GetCurrentMethod().DeclaringType.Namespace + "_" + MethodBase.GetCurrentMethod().Name + "_" + sentArr);
                    }
                    break;

                case "FOSUPPLYUNIT":
                    try
                    {
                        var BLRHFO = Convert.ToDouble(sentArr[1]);
                        var BLRDGO = Convert.ToDouble(sentArr[2]);
                        var BLROilFlow = Convert.ToDecimal(sentArr[3]);
                        var BLRTotalHFO = Convert.ToDecimal(sentArr[4]);
                        var BLRTotalDGO = Convert.ToDecimal(sentArr[5]);
                        var HFOService = Convert.ToInt16(sentArr[6]);
                        var DGOService = Convert.ToInt16(sentArr[7]);
                        var MEInFlow = Convert.ToDecimal(sentArr[8]);
                        var MEOutFlow = Convert.ToDecimal(sentArr[9]);
                        var AEInFlow = Convert.ToDecimal(sentArr[11]);
                        var AEOutFlow = Convert.ToDecimal(sentArr[10]);
                        var MEInTotal = Convert.ToDecimal(sentArr[12]);
                        var MEOutTotal = Convert.ToDecimal(sentArr[13]);
                        var AEInTotal = Convert.ToDecimal(sentArr[14]);
                        var AEOutTotal = Convert.ToDecimal(sentArr[15]);

                        var MEOtherFuelTotal = await _flowmeterService.GetTotalFCBySNRDFTAsync(number, StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime, "me_fuel", HFOService == 1 ? "DGO" : "HFO");
                        var AEOtherFuelTotal = await _flowmeterService.GetTotalFCBySNRDFTAsync(number, StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime, "ae_fuel", HFOService == 1 ? "DGO" : "HFO");

                        if (lstFOSUPPLYUNIT.Any(t => t.Number == number && t.DeviceNo == strDevice))
                        {
                            var FOSUPPLYUNIT = lstFOSUPPLYUNIT.First(t => t.Number == number && t.DeviceNo == strDevice);
                            var index = lstFOSUPPLYUNIT.IndexOf(FOSUPPLYUNIT);

                            lstFOSUPPLYUNIT[index].HFOService = HFOService;
                            lstFOSUPPLYUNIT[index].DGOService = DGOService;

                            lstFOSUPPLYUNIT[index].Number = number;
                            lstFOSUPPLYUNIT[index].ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime;
                            lstFOSUPPLYUNIT[index].DeviceNo = strDevice;
                            lstFOSUPPLYUNIT[index].Uploaded = 0;
                        }
                        else
                        {
                            lstFOSUPPLYUNIT.Add(new FOSupplyUnitDto
                            {
                                HFOService = HFOService,
                                DGOService = DGOService,

                                Number = number,
                                ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime,
                                DeviceNo = strDevice,
                                Uploaded = 0
                            });
                        }

                        if (lstFm.Any(t => t.Number == number && t.DeviceNo == "me_fuel" && t.DeviceType == "me"))
                        {
                            var Fm = lstFm.First(t => t.Number == number && t.DeviceNo == "me_fuel" && t.DeviceType == "me");
                            var index = lstFm.IndexOf(Fm);

                            lstFm[index].ConsAct = MEInFlow - MEOutFlow;
                            lstFm[index].ConsAcc = MEInTotal - MEOutTotal - MEOtherFuelTotal;
                            lstFm[index].FuelType = HFOService == 1 ? "HFO" : "DGO";

                            lstFm[index].Number = number;
                            lstFm[index].ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime;
                            lstFm[index].DeviceType = "me";
                            lstFm[index].DeviceNo = "me_fuel";
                            lstFm[index].Uploaded = 0;
                        }
                        else
                        {
                            lstFm.Add(new FlowmeterDto
                            {
                                ConsAct = MEInFlow - MEOutFlow,
                                ConsAcc = MEInTotal - MEOutTotal - MEOtherFuelTotal,
                                FuelType = HFOService == 1 ? "HFO" : "DGO",

                                Number = number,
                                ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime,
                                DeviceType = "me",
                                DeviceNo = "me_fuel",
                                Uploaded = 0
                            });
                        }

                        if (lstFm.Any(t => t.Number == number && t.DeviceNo == "ae_fuel" && t.DeviceType == "ae"))
                        {
                            var Fm = lstFm.First(t => t.Number == number && t.DeviceNo == "ae_fuel" && t.DeviceType == "ae");
                            var index = lstFm.IndexOf(Fm);

                            lstFm[index].ConsAct = AEInFlow - AEOutFlow;
                            lstFm[index].ConsAcc = AEInTotal - AEOutTotal - AEOtherFuelTotal;
                            lstFm[index].FuelType = HFOService == 1 ? "HFO" : "DGO";

                            lstFm[index].Number = number;
                            lstFm[index].ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime;
                            lstFm[index].DeviceType = "ae";
                            lstFm[index].DeviceNo = "ae_fuel";
                            lstFm[index].Uploaded = 0;
                        }
                        else
                        {
                            lstFm.Add(new FlowmeterDto
                            {
                                ConsAct = AEInFlow - AEOutFlow,
                                ConsAcc = AEInTotal - AEOutTotal - AEOtherFuelTotal,
                                FuelType = HFOService == 1 ? "HFO" : "DGO",

                                Number = number,
                                ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime,
                                DeviceType = "ae",
                                DeviceNo = "ae_fuel",
                                Uploaded = 0
                            });
                        }

                        if (lstFm.Any(t => t.Number == number && t.DeviceNo == "blr_fuel" && t.DeviceType == "blr"))
                        {
                            var Fm = lstFm.First(t => t.Number == number && t.DeviceNo == "blr_fuel" && t.DeviceType == "blr");
                            var index = lstFm.IndexOf(Fm);

                            lstFm[index].ConsAct = BLROilFlow;
                            lstFm[index].ConsAcc = BLRHFO == 1 ? BLRTotalHFO : BLRTotalDGO;
                            lstFm[index].FuelType = BLRHFO == 1 ? "HFO" : "DGO";

                            lstFm[index].Number = number;
                            lstFm[index].ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime;
                            lstFm[index].DeviceType = "blr";
                            lstFm[index].DeviceNo = "blr_fuel";
                            lstFm[index].Uploaded = 0;
                        }
                        else
                        {
                            lstFm.Add(new FlowmeterDto
                            {
                                ConsAct = BLROilFlow,
                                ConsAcc = BLRHFO == 1 ? BLRTotalHFO : BLRTotalDGO,
                                FuelType = BLRHFO == 1 ? "HFO" : "DGO",

                                Number = number,
                                ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime,
                                DeviceType = "blr",
                                DeviceNo = "blr_fuel",
                                Uploaded = 0
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "ams燃油供油单元错误=>" + MethodBase.GetCurrentMethod().DeclaringType.Namespace + "_" + MethodBase.GetCurrentMethod().Name + "_" + sentArr);
                    }
                    break;

                case "FOCTRANS":
                    try
                    {
                        var ME1Flow = Convert.ToDecimal(sentArr[1]);
                        var ME1Total = Convert.ToDecimal(sentArr[2]);
                        var ME2Flow = Convert.ToDecimal(sentArr[3]);
                        var ME2Total = Convert.ToDecimal(sentArr[4]);
                        var AE1Flow = Convert.ToDecimal(sentArr[5]);
                        var AE1Total = Convert.ToDecimal(sentArr[6]);
                        var AE2Flow = Convert.ToDecimal(sentArr[7]);
                        var AE2Total = Convert.ToDecimal(sentArr[8]);
                        var RefuelFlow = Convert.ToDecimal(sentArr[9]);
                        var RefuelTotal = Convert.ToDecimal(sentArr[10]);

                        if (lstFm.Any(t => t.Number == number && t.DeviceNo == "me_fuel_1" && t.DeviceType == "me"))
                        {
                            var Fm = lstFm.First(t => t.Number == number && t.DeviceNo == "me_fuel_1" && t.DeviceType == "me");
                            var index = lstFm.IndexOf(Fm);

                            lstFm[index].ConsAct = ME1Flow;
                            lstFm[index].ConsAcc = ME1Total;
                            lstFm[index].FuelType = "DGO";

                            lstFm[index].Number = number;
                            lstFm[index].ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime;
                            lstFm[index].DeviceType = "me";
                            lstFm[index].DeviceNo = "me_fuel_1";
                            lstFm[index].Uploaded = 0;
                        }
                        else
                        {
                            lstFm.Add(new FlowmeterDto
                            {
                                ConsAct = ME1Flow,
                                ConsAcc = ME1Total,
                                FuelType = "DGO",

                                Number = number,
                                ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime,
                                DeviceType = "me",
                                DeviceNo = "me_fuel_1",
                                Uploaded = 0
                            });
                        }

                        if (lstFm.Any(t => t.Number == number && t.DeviceNo == "me_fuel_2" && t.DeviceType == "me"))
                        {
                            var Fm = lstFm.First(t => t.Number == number && t.DeviceNo == "me_fuel_2" && t.DeviceType == "me");
                            var index = lstFm.IndexOf(Fm);

                            lstFm[index].ConsAct = ME2Flow;
                            lstFm[index].ConsAcc = ME2Total;
                            lstFm[index].FuelType = "DGO";

                            lstFm[index].Number = number;
                            lstFm[index].ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime;
                            lstFm[index].DeviceType = "me";
                            lstFm[index].DeviceNo = "me_fuel_2";
                            lstFm[index].Uploaded = 0;
                        }
                        else
                        {
                            lstFm.Add(new FlowmeterDto
                            {
                                ConsAct = ME2Flow,
                                ConsAcc = ME2Total,
                                FuelType = "DGO",

                                Number = number,
                                ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime,
                                DeviceType = "me",
                                DeviceNo = "me_fuel_2",
                                Uploaded = 0
                            });
                        }

                        if (lstFm.Any(t => t.Number == number && t.DeviceNo == "ae_fuel_1" && t.DeviceType == "ae"))
                        {
                            var Fm = lstFm.First(t => t.Number == number && t.DeviceNo == "ae_fuel_1" && t.DeviceType == "ae");
                            var index = lstFm.IndexOf(Fm);

                            lstFm[index].ConsAct = AE1Flow;
                            lstFm[index].ConsAcc = AE1Total;
                            lstFm[index].FuelType = "DGO";

                            lstFm[index].Number = number;
                            lstFm[index].ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime;
                            lstFm[index].DeviceType = "ae";
                            lstFm[index].DeviceNo = "ae_fuel_1";
                            lstFm[index].Uploaded = 0;
                        }
                        else
                        {
                            lstFm.Add(new FlowmeterDto
                            {
                                ConsAct = AE1Flow,
                                ConsAcc = AE1Total,
                                FuelType = "DGO",

                                Number = number,
                                ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime,
                                DeviceType = "ae",
                                DeviceNo = "ae_fuel_1",
                                Uploaded = 0
                            });
                        }

                        if (lstFm.Any(t => t.Number == number && t.DeviceNo == "ae_fuel_2" && t.DeviceType == "ae"))
                        {
                            var Fm = lstFm.First(t => t.Number == number && t.DeviceNo == "ae_fuel_2" && t.DeviceType == "ae");
                            var index = lstFm.IndexOf(Fm);

                            lstFm[index].ConsAct = AE2Flow;
                            lstFm[index].ConsAcc = AE2Total;
                            lstFm[index].FuelType = "DGO";

                            lstFm[index].Number = number;
                            lstFm[index].ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime;
                            lstFm[index].DeviceType = "ae";
                            lstFm[index].DeviceNo = "ae_fuel_2";
                            lstFm[index].Uploaded = 0;
                        }
                        else
                        {
                            lstFm.Add(new FlowmeterDto
                            {
                                ConsAct = AE2Flow,
                                ConsAcc = AE2Total,
                                FuelType = "DGO",

                                Number = number,
                                ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime,
                                DeviceType = "ae",
                                DeviceNo = "ae_fuel_2",
                                Uploaded = 0
                            });
                        }

                        if (lstFm.Any(t => t.Number == number && t.DeviceNo == "refuel1" && t.DeviceType == "refuel"))
                        {
                            var Fm = lstFm.First(t => t.Number == number && t.DeviceNo == "refuel1" && t.DeviceType == "refuel");
                            var index = lstFm.IndexOf(Fm);

                            lstFm[index].ConsAct = RefuelFlow;
                            lstFm[index].ConsAcc = RefuelTotal;
                            lstFm[index].FuelType = "DGO";

                            lstFm[index].Number = number;
                            lstFm[index].ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime;
                            lstFm[index].DeviceType = "refuel";
                            lstFm[index].DeviceNo = "refuel1";
                            lstFm[index].Uploaded = 0;
                        }
                        else
                        {
                            lstFm.Add(new FlowmeterDto
                            {
                                ConsAct = RefuelFlow,
                                ConsAcc = RefuelTotal,
                                FuelType = "DGO",

                                Number = number,
                                ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime,
                                DeviceType = "refuel",
                                DeviceNo = "refuel1",
                                Uploaded = 0
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "流量计错误=>" + MethodBase.GetCurrentMethod().DeclaringType.Namespace + "_" + MethodBase.GetCurrentMethod().Name + "_" + sentArr);
                    }
                    break;

                case "SHAFT":
                    try
                    {
                        var Power = Convert.ToDecimal(sentArr[1]);
                        var RPM = Convert.ToDecimal(sentArr[2]);
                        var Torque = Convert.ToDecimal(sentArr[3]);
                        var Thrust = Convert.ToDecimal(sentArr[4]);
                        var Energy = Convert.ToDouble(sentArr[5]);
                        var Revolutions = Convert.ToDouble(sentArr[6]);

                        if (lstSh.Any(t => t.Number == number && t.DeviceNo == strDevice))
                        {
                            var sh = lstSh.First(t => t.Number == number && t.DeviceNo == strDevice);
                            var index = lstSh.IndexOf(sh);
                            lstSh[index].Number = number;

                            lstSh[index].Power = Power;
                            lstSh[index].RPM = RPM;
                            lstSh[index].Torque = Torque;
                            lstSh[index].Thrust = Thrust;
                            lstSh[index].Energy = Energy;
                            lstSh[index].Revolutions = Revolutions;

                            lstSh[index].ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime;
                            lstSh[index].DeviceNo = strDevice;
                            lstSh[index].Uploaded = 0;
                        }
                        else
                        {
                            lstSh.Add(new ShaftDto
                            {
                                Power = Power,
                                RPM = RPM,
                                Torque = Torque,
                                Thrust = Thrust,
                                Energy = Energy,
                                Revolutions = Revolutions,

                                Number = number,
                                ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime,
                                DeviceNo = strDevice,
                                Uploaded = 0
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "ams轴功率错误=>" + MethodBase.GetCurrentMethod().DeclaringType.Namespace + "_" + MethodBase.GetCurrentMethod().Name + "_" + sentArr);
                    }
                    break;

                case "1ERBMT":
                case "2ERBMT":
                    try
                    {
                        var Torque = Convert.ToDecimal(sentArr[1]); //kNm
                        var Thrust = Convert.ToDecimal(sentArr[2]); //kNm
                        var RPM = Convert.ToDecimal(sentArr[3]); //rev/min
                        var Power = Convert.ToDecimal(sentArr[4]); //kW

                        if (lstSh.Any(t => t.Number == number && t.DeviceNo == strDevice))
                        {
                            var sh = lstSh.First(t => t.Number == number && t.DeviceNo == strDevice);
                            var index = lstSh.IndexOf(sh);
                            lstSh[index].Number = number;

                            lstSh[index].Power = Power;
                            lstSh[index].RPM = RPM;
                            lstSh[index].Torque = Torque;
                            lstSh[index].Thrust = Thrust;

                            lstSh[index].ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime;
                            lstSh[index].DeviceNo = strDevice;
                            lstSh[index].Uploaded = 0;
                        }
                        else
                        {
                            lstSh.Add(new ShaftDto
                            {
                                Power = Power,
                                RPM = RPM,
                                Torque = Torque,
                                Thrust = Thrust,

                                Number = number,
                                ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime,
                                DeviceNo = strDevice,
                                Uploaded = 0
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "轴功率错误=>" + MethodBase.GetCurrentMethod().DeclaringType.Namespace + "_" + MethodBase.GetCurrentMethod().Name + "_" + sentArr);
                    }
                    break;

                case "LUBOILPURIFYING":
                    try
                    {
                        var MEFilterPressHigh = Convert.ToInt16(sentArr[1]);
                        if (lstLUBOILPURIFYING.Any(t => t.Number == number && t.DeviceNo == strDevice))
                        {
                            var LUBOILPURIFYING = lstLUBOILPURIFYING.First(t => t.Number == number && t.DeviceNo == strDevice);
                            var index = lstLUBOILPURIFYING.IndexOf(LUBOILPURIFYING);

                            lstLUBOILPURIFYING[index].MEFilterPressHigh = MEFilterPressHigh;

                            lstLUBOILPURIFYING[index].Number = number;
                            lstLUBOILPURIFYING[index].ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime;
                            lstLUBOILPURIFYING[index].DeviceNo = strDevice;
                            lstLUBOILPURIFYING[index].Uploaded = 0;
                        }
                        else
                        {
                            lstLUBOILPURIFYING.Add(new LubOilPurifyingDto
                            {
                                MEFilterPressHigh = MEFilterPressHigh,

                                Number = number,
                                ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime,
                                DeviceNo = strDevice,
                                Uploaded = 0
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "ams滑油净化系统错误=>" + MethodBase.GetCurrentMethod().DeclaringType.Namespace + "_" + MethodBase.GetCurrentMethod().Name + "_" + sentArr);
                    }
                    break;

                case "COOLINGSEAWATER":
                    try
                    {
                        var CSWOutPress = Convert.ToDouble(sentArr[1]);
                        var CSWOutTemp = Convert.ToDouble(sentArr[2]);

                        if (lstCOOLINGSEAWATER.Any(t => t.Number == number && t.DeviceNo == strDevice))
                        {
                            var COOLINGSEAWATER = lstCOOLINGSEAWATER.First(t => t.Number == number && t.DeviceNo == strDevice);
                            var index = lstCOOLINGSEAWATER.IndexOf(COOLINGSEAWATER);

                            lstCOOLINGSEAWATER[index].CSWOutPress = CSWOutPress;
                            lstCOOLINGSEAWATER[index].CSWOutTemp = CSWOutTemp;

                            lstCOOLINGSEAWATER[index].Number = number;
                            lstCOOLINGSEAWATER[index].ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime;
                            lstCOOLINGSEAWATER[index].DeviceNo = strDevice;
                            lstCOOLINGSEAWATER[index].Uploaded = 0;
                        }
                        else
                        {
                            lstCOOLINGSEAWATER.Add(new CoolingSeaWaterDto
                            {
                                CSWOutPress = CSWOutPress,
                                CSWOutTemp = CSWOutTemp,

                                Number = number,
                                ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime,
                                DeviceNo = strDevice,
                                Uploaded = 0
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "ams冷却海水系统错误=>" + MethodBase.GetCurrentMethod().DeclaringType.Namespace + "_" + MethodBase.GetCurrentMethod().Name + "_" + sentArr);
                    }
                    break;

                case "COOLINGFRESHWATER":
                    try
                    {
                        var LTCFWPress = Convert.ToDouble(sentArr[1]);
                        var CCLTCFWOutTemp = Convert.ToDouble(sentArr[2]);

                        if (lstCOOLINGFRESHWATER.Any(t => t.Number == number && t.DeviceNo == strDevice))
                        {
                            var COOLINGFRESHWATER = lstCOOLINGFRESHWATER.First(t => t.Number == number && t.DeviceNo == strDevice);
                            var index = lstCOOLINGFRESHWATER.IndexOf(COOLINGFRESHWATER);

                            lstCOOLINGFRESHWATER[index].LTCFWPress = LTCFWPress;
                            lstCOOLINGFRESHWATER[index].CCLTCFWOutTemp = CCLTCFWOutTemp;

                            lstCOOLINGFRESHWATER[index].Number = number;
                            lstCOOLINGFRESHWATER[index].ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime;
                            lstCOOLINGFRESHWATER[index].DeviceNo = strDevice;
                            lstCOOLINGFRESHWATER[index].Uploaded = 0;
                        }
                        else
                        {
                            lstCOOLINGFRESHWATER.Add(new CoolingFreshWaterDto
                            {
                                LTCFWPress = LTCFWPress,
                                CCLTCFWOutTemp = CCLTCFWOutTemp,

                                Number = number,
                                ReceiveDatetime = StaticEntities.StaticEntities.Vessels[indexVessel].ReceiveDatetime,
                                DeviceNo = strDevice,
                                Uploaded = 0
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "ams冷却淡水系统错误=>" + MethodBase.GetCurrentMethod().DeclaringType.Namespace + "_" + MethodBase.GetCurrentMethod().Name + "_" + sentArr);
                    }
                    break;

                case "DRAFT":
                    try
                    {
                        var bd = Convert.ToDouble(sentArr[1]);
                        var ad = Convert.ToDouble(sentArr[2]);
                        var pd = Convert.ToDouble(sentArr[3]);
                        var sd = Convert.ToDouble(sentArr[4]);
                        StaticEntities.StaticEntities.Vessels[indexVessel].BowDraft = bd;
                        StaticEntities.StaticEntities.Vessels[indexVessel].AsternDraft = ad;
                        StaticEntities.StaticEntities.Vessels[indexVessel].PortDraft = pd;
                        StaticEntities.StaticEntities.Vessels[indexVessel].StarBoardDraft = sd;
                        StaticEntities.StaticEntities.Vessels[indexVessel].Trim = ad - bd;
                        StaticEntities.StaticEntities.Vessels[indexVessel].Heel = pd - sd;
                        StaticEntities.StaticEntities.Vessels[indexVessel].Draft = (bd + ad) / 2;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "ams吃水数据错误=>" + MethodBase.GetCurrentMethod().DeclaringType.Namespace + "_" + MethodBase.GetCurrentMethod().Name + "_" + sentArr);
                    }
                    break;
            }

            return result;
        }

        /// <summary>
        /// 解析协议数据
        /// </summary>
        /// <param name="deviceCode"></param>
        /// <param name="sentence"></param>
        /// <returns></returns>
        public object[] DecodeProtocalData(string deviceCode, string sentence)
        {
            var result = new List<object>();
            switch (deviceCode)
            {
                case "me_fuel_in_1":
                case "me_fuel_out_1":
                case "me_fuel_in_2":
                case "me_fuel_out_2":
                case "me_methanol":
                    var flowmeter = new Flowmeter(sentence);
                    result.Add(flowmeter.ConsAct);
                    result.Add(flowmeter.ConsAcc);
                    result.Add(flowmeter.Temperature);
                    result.Add(flowmeter.Density);
                    break;

                case "draft":
                    var draft = new Draft(sentence);
                    result.Add(draft.Bow);
                    result.Add(draft.Astern);
                    result.Add(draft.Port);
                    result.Add(draft.StartBoard);
                    break;

                case "shaft_1":
                case "shaft_2":
                    var shaft = new Shaft(sentence);
                    result.Add(shaft.Power);
                    if (deviceCode == "shaft_1")
                        result.Add(shaft.RPM);
                    else
                        result.Add(-shaft.RPM);
                    result.Add(shaft.Torque);
                    result.Add(shaft.Thrust);
                    break;

                case "generator_1":
                case "generator_2":
                case "generator_3":
                case "generator_4":
                    var generator = new Generator(sentence);
                    result.Add(generator.IsRuning);
                    result.Add(generator.RPM);
                    result.Add(generator.StartPressure);
                    result.Add(generator.ControlPressure);
                    result.Add(generator.ScavengingPressure);
                    result.Add(generator.LubePressure);
                    result.Add(generator.LubeTEMP);
                    result.Add(generator.FuelPressure);
                    result.Add(generator.FuelTEMP);
                    result.Add(generator.FreshWaterPressure);
                    result.Add(generator.FreshWaterTEMPIn);
                    result.Add(generator.FreshWaterTEMPOut);
                    result.Add(generator.CoolingWaterPressure);
                    result.Add(generator.CoolingWaterTEMPIn);
                    result.Add(generator.CoolingWaterTEMPOut);
                    result.Add(generator.CylinderTEMP1);
                    result.Add(generator.CylinderTEMP2);
                    result.Add(generator.CylinderTEMP3);
                    result.Add(generator.CylinderTEMP4);
                    result.Add(generator.CylinderTEMP5);
                    result.Add(generator.CylinderTEMP6);
                    result.Add(generator.SuperchargerTEMPIn);
                    result.Add(generator.SuperchargerTEMPOut);
                    result.Add(generator.ScavengingTEMP);
                    result.Add(generator.BearingTEMP);
                    result.Add(generator.BearingTEMPFront);
                    result.Add(generator.BearingTEMPBack);
                    result.Add(generator.Power);
                    result.Add(generator.WindingTEMPL1);
                    result.Add(generator.WindingTEMPL2);
                    result.Add(generator.WindingTEMPL3);
                    result.Add(generator.VoltageL1L2);
                    result.Add(generator.VoltageL2L3);
                    result.Add(generator.VoltageL1L3);
                    result.Add(generator.FrequencyL1);
                    result.Add(generator.FrequencyL2);
                    result.Add(generator.FrequencyL3);
                    result.Add(generator.CurrentL1);
                    result.Add(generator.CurrentL2);
                    result.Add(generator.CurrentL3);
                    result.Add(generator.ReactivePower);
                    result.Add(generator.PowerFactor);
                    result.Add(generator.LoadRatio);
                    break;

                case "battery_1":
                case "battery_2":
                    var battery = new Battery(sentence);
                    result.Add(battery.SOC);
                    result.Add(battery.SOH);
                    result.Add(battery.MaxTEMP);
                    result.Add(battery.MaxTEMPBox);
                    result.Add(battery.MaxTEMPNo);
                    result.Add(battery.MinTEMP);
                    result.Add(battery.MinTEMPBox);
                    result.Add(battery.MinTEMPNo);
                    result.Add(battery.MaxVoltage);
                    result.Add(battery.MaxVoltageBox);
                    result.Add(battery.MaxVoltageNo);
                    result.Add(battery.MinVoltage);
                    result.Add(battery.MinVoltageBox);
                    result.Add(battery.MinVoltageNo);
                    break;

                case "sternsealing_1":
                case "sternsealing_2":
                    var sternsealing = new SternSealing(sentence);
                    result.Add(sternsealing.FrontTEMP);
                    result.Add(sternsealing.BackTEMP);
                    result.Add(sternsealing.BackLeftTEMP);
                    result.Add(sternsealing.BackRightTEMP);
                    break;

                case "liquidlevel_1":
                case "liquidlevel_2":
                    var liquidlevel = new LiquidLevel(sentence);
                    result.Add(liquidlevel.Level);
                    result.Add(liquidlevel.Temperature);
                    break;

                case "supplyunit_1":
                case "supplyunit_2":
                case "supplyunit_3":
                    var supplyunit = new SupplyUnit(sentence);
                    result.Add(supplyunit.IsRuning);
                    result.Add(supplyunit.Temperature);
                    result.Add(supplyunit.Pressure);
                    break;
            }
            return result.ToArray();
        }

        /// <summary>
        /// 解析协议数据
        /// </summary>
        /// <param name="deviceCode"></param>
        /// <param name="sentence"></param>
        /// <returns></returns>
        public object[] DecodeProtocalDataMODBUS(string deviceCode, string sentence)
        {
            sentence = sentence.Replace(" ", "");
            sentence = sentence[6..];
            var result = new List<object>();
            switch (deviceCode)
            {
                case "me_fuel_in_1":
                case "me_fuel_out_1":
                case "me_fuel_in_2":
                case "me_fuel_out_2":
                    //var flowmeter = new Flowmeter(sentence);
                    result.Add(GetFloatFromBytes(sentence[..8]) * 60);  //ConsAct
                    result.Add(GetFloatFromBytes(sentence.Substring(16, 8)));  //ConsAcc
                    result.Add(GetFloatFromBytes(sentence.Substring(8, 8)));  //Temperature
                    result.Add(GetFloatFromBytes(sentence.Substring(24, 8)));  //Density
                    break;

                case "me_methanol":
                    //var flowmeter = new Flowmeter(sentence);
                    result.Add(GetFloatHFFromBytes(sentence[..8]) * 60);  //ConsAct
                    result.Add(GetFloatHFFromBytes(sentence.Substring(16, 8)));  //ConsAcc
                    result.Add(GetFloatHFFromBytes(sentence.Substring(8, 8)));  //Temperature
                    result.Add(GetFloatHFFromBytes(sentence.Substring(24, 8)));  //Density
                    break;
            }
            return result.ToArray();
        }

        /// <summary>
        /// 解析协议数据
        /// </summary>
        /// <param name="deviceCode"></param>
        /// <param name="sentence"></param>
        /// <returns></returns>
        public object[] DecodeProtocalDataMODBUSTCP(string deviceCode, string sentence)
        {
            sentence = sentence.Replace(" ", "");
            sentence = sentence[18..];
            var result = new List<object>();
            switch (deviceCode)
            {
                case "draft":
                    //var draft = new Draft(sentence);
                    /*result.Add(GetFloatFromBytes(sentence.Substring(24, 8)));  //船艏
                    result.Add(GetFloatFromBytes(sentence.Substring(16, 8)));  //船艉
                    result.Add(GetFloatFromBytes(sentence[..8]));  //左舷
                    result.Add(GetFloatFromBytes(sentence.Substring(8, 8)));  //右舷*/

                    result.Add(GetFloatFromBytes(sentence[..8]));  //左舷
                    result.Add(GetFloatFromBytes(sentence.Substring(24, 8)));  //船艏
                    result.Add(GetFloatFromBytes(sentence.Substring(16, 8)));  //船艉
                    result.Add(GetFloatFromBytes(sentence.Substring(8, 8)));  //右舷
                    break;

                case "generator_1":
                case "generator_2":
                case "generator_3":
                case "generator_4":
                    //var generator = new Generator(sentence);
                    result.Add(0); //IsRuning
                    result.Add(GetFloatFromBytes(sentence[..8]));  //RPM
                    result.Add(GetFloatFromBytes(sentence.Substring(8, 8)));  //StartPressure
                    result.Add(0);  //ControlPressure
                    result.Add(0);  //ScavengingPressure
                    result.Add(GetFloatFromBytes(sentence.Substring(16, 8)));  //LubePressure
                    result.Add(GetFloatFromBytes(sentence.Substring(24, 8)));  //LubeTEMP
                    result.Add(GetFloatFromBytes(sentence.Substring(16, 8)));  //FuelPressure
                    result.Add(GetFloatFromBytes(sentence.Substring(24, 8)));  //FuelTEMP
                    result.Add(0);  //FreshWaterPressure
                    result.Add(0);  //FreshWaterTEMPIn
                    result.Add(0);  //FreshWaterTEMPOut
                    result.Add(0);  //CoolingWaterPressure
                    result.Add(0);  //CoolingWaterTEMPIn
                    result.Add(0);  //CoolingWaterTEMPOut
                    result.Add(GetFloatFromBytes(sentence.Substring(32, 8)));  //CylinderTEMP1
                    result.Add(GetFloatFromBytes(sentence.Substring(40, 8)));  //CylinderTEMP2
                    result.Add(GetFloatFromBytes(sentence.Substring(48, 8)));  //CylinderTEMP3
                    result.Add(GetFloatFromBytes(sentence.Substring(56, 8)));  //CylinderTEMP4
                    result.Add(GetFloatFromBytes(sentence.Substring(64, 8)));  //CylinderTEMP5
                    result.Add(GetFloatFromBytes(sentence.Substring(72, 8)));  //CylinderTEMP6
                    result.Add(GetFloatFromBytes(sentence.Substring(96, 8)));  //SuperchargerTEMPIn
                    result.Add(GetFloatFromBytes(sentence.Substring(104, 8)));  //SuperchargerTEMPOut
                    result.Add(0);  //ScavengingTEMP
                    result.Add(GetFloatFromBytes(sentence.Substring(112, 8)));  //BearingTEMP
                    result.Add(GetFloatFromBytes(sentence.Substring(120, 8)));  //BearingTEMPFront
                    result.Add(GetFloatFromBytes(sentence.Substring(128, 8)));  //BearingTEMPBack
                    result.Add(0);  //Power
                    result.Add(GetFloatFromBytes(sentence.Substring(184, 8)));  //WindingTEMPL1
                    result.Add(GetFloatFromBytes(sentence.Substring(192, 8)));  //WindingTEMPL2
                    result.Add(GetFloatFromBytes(sentence.Substring(200, 8)));  //WindingTEMPL3
                    result.Add(GetFloatFromBytes(sentence.Substring(208, 8)));  //VoltageL1L2
                    result.Add(GetFloatFromBytes(sentence.Substring(216, 8)));  //VoltageL2L3
                    result.Add(GetFloatFromBytes(sentence.Substring(224, 8)));  //VoltageL1L3
                    result.Add(0);  //FrequencyL1
                    result.Add(0);  //FrequencyL2
                    result.Add(0);  //FrequencyL3
                    result.Add(GetFloatFromBytes(sentence.Substring(232, 8)));  //CurrentL1
                    result.Add(GetFloatFromBytes(sentence.Substring(240, 8)));  //CurrentL2
                    result.Add(GetFloatFromBytes(sentence.Substring(248, 8)));  //CurrentL3
                    result.Add(0);  //ReactivePower
                    result.Add(0);  //PowerFactor
                    result.Add(GetFloatFromBytes(sentence.Substring(256, 8)));  //LoadRatio
                    result.Add(GetFloatFromBytes(sentence.Substring(80, 8)));  //CylinderTEMP7
                    result.Add(GetFloatFromBytes(sentence.Substring(88, 8)));  //CylinderTEMP8
                    result.Add(GetFloatFromBytes(sentence.Substring(136, 8)));  //BearingTEMP4
                    result.Add(GetFloatFromBytes(sentence.Substring(144, 8)));  //BearingTEMP5
                    result.Add(GetFloatFromBytes(sentence.Substring(152, 8)));  //BearingTEMP6
                    result.Add(GetFloatFromBytes(sentence.Substring(160, 8)));  //BearingTEMP7
                    result.Add(GetFloatFromBytes(sentence.Substring(168, 8)));  //BearingTEMP8
                    result.Add(GetFloatFromBytes(sentence.Substring(176, 8)));  //BearingTEMP9
                    break;

                case "sternsealing":
                    //var sternsealing = new SternSealing(sentence);
                    result.Add(GetFloatFromBytes(sentence.Substring(0, 8)));  //FrontTEMP
                    result.Add(GetFloatFromBytes(sentence.Substring(16, 8)));  //BackTEMP
                    result.Add(GetFloatFromBytes(sentence.Substring(8, 8)));  //BackLeftTEMP
                    result.Add(GetFloatFromBytes(sentence.Substring(24, 8)));  //BackRightTEMP
                    break;
            }
            return result.ToArray();
        }

        /// <summary>
        /// 高位在前
        /// </summary>
        /// <param name="strChars"></param>
        /// <returns></returns>
        private float GetFloatHFFromBytes(string strChars)
        {
            if (strChars.Length != 8)
                return 0;
            var result = 0f;
            var fixChars = strChars;
            result = BitConverter.ToSingle(BitConverter.GetBytes(uint.Parse(fixChars, System.Globalization.NumberStyles.AllowHexSpecifier)), 0);
            return result;
        }

        /// <summary>
        /// 低位在前
        /// </summary>
        /// <param name="strChars"></param>
        /// <returns></returns>
        private float GetFloatFromBytes(string strChars)
        {
            if (strChars.Length != 8)
                return 0;
            var result = 0f;
            var fixChars = strChars.Substring(4) + strChars[..4];
            result = BitConverter.ToSingle(BitConverter.GetBytes(uint.Parse(fixChars, System.Globalization.NumberStyles.AllowHexSpecifier)), 0);
            return result;
        }

        /// <summary>
        /// 滤波
        /// </summary>
        /// <param name="number">采集系统序列号</param>
        /// <param name="deviceCode">设备代号</param>
        /// <param name="value">要滤波的数值</param>
        /// <returns></returns>
        public decimal Filtering(string number, string deviceCode, decimal value)
        {
            var result = 0m;
            if (!StaticEntities.StaticEntities.FilteringParams.Any(t => t.Number == number && t.DeviceNo == deviceCode))
                StaticEntities.StaticEntities.FilteringParams.Add(new StaticEntities.FilteringParam { Number = number, DeviceNo = deviceCode });
            var entity = StaticEntities.StaticEntities.FilteringParams.FirstOrDefault(t => t.Number == number && t.DeviceNo == deviceCode);
            var index = StaticEntities.StaticEntities.FilteringParams.IndexOf(entity);
            StaticEntities.StaticEntities.FilteringParams[index].Values.Add(value);
            if (StaticEntities.StaticEntities.FilteringParams[index].Values.Count > 12)
            {
                StaticEntities.StaticEntities.FilteringParams[index].Values.RemoveAt(0);
                result = StaticEntities.StaticEntities.FilteringParams[index].Values.Average();
            }
            else
                result = value;
            return result;
        }

        /// <summary>
        /// 0:无意义;1:靠泊;2:机动航行;3:定速航行
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public string GetStatus(string number)
        {
            var result = string.Empty;

            if (StaticEntities.StaticEntities.Vessels.Any(t => t.SN == number))
            {
                try
                {
                    var criteria = new CriteriaDto();
                    var currentIndex = StaticEntities.StaticEntities.Vessels.IndexOf(StaticEntities.StaticEntities.Vessels.FirstOrDefault(t => t.SN == number));

                    if (!StaticEntities.StaticEntities.SpeedParams.Any(t => t.Number == number))
                        StaticEntities.StaticEntities.SpeedParams.Add(new StaticEntities.SpeedParam { Number = number });
                    var entity = StaticEntities.StaticEntities.SpeedParams.FirstOrDefault(t => t.Number == number);
                    var index = StaticEntities.StaticEntities.SpeedParams.IndexOf(entity);
                    StaticEntities.StaticEntities.SpeedParams[index].Values.Add(StaticEntities.StaticEntities.Vessels[currentIndex].GroundSpeed ?? 0);
                    if (StaticEntities.StaticEntities.SpeedParams[index].Values.Count > 60)
                        StaticEntities.StaticEntities.SpeedParams[index].Values.RemoveAt(0);

                    if (StaticEntities.StaticEntities.Vessels[currentIndex].GroundSpeed <= 3)
                        result = "Berthing";
                    if (StaticEntities.StaticEntities.Vessels[currentIndex].GroundSpeed > 3 && StaticEntities.StaticEntities.Vessels[currentIndex].GroundSpeed <= 5)
                        result = "Manoeuvring";
                    if (StaticEntities.StaticEntities.Vessels[currentIndex].GroundSpeed > 5)
                    {
                        if (StaticEntities.StaticEntities.TotalIndicators.Any(t => t.Number == number))
                        {
                            var totalIndicator = StaticEntities.StaticEntities.TotalIndicators.FirstOrDefault(t => t.Number == number);
                            var rate = Convert.ToDouble(totalIndicator.Power) / criteria.Power;
                            if (rate < 0.65)
                                result = "Manoeuvring";
                            else
                            {
                                var avg = StaticEntities.StaticEntities.SpeedParams[index].Values.Average();
                                if (StaticEntities.StaticEntities.Vessels[currentIndex].GroundSpeed / avg >= 0.95)
                                    result = "Cruising";
                                else
                                    result = "Manoeuvring";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "获取船舶航行状态失败=>" + MethodBase.GetCurrentMethod().DeclaringType.Namespace + "_" + MethodBase.GetCurrentMethod().Name);
                }
            }

            return result;
        }
    }
}