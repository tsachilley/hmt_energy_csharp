using hmt_energy_csharp.Energy.Batteries;
using hmt_energy_csharp.Energy.Flowmeters;
using hmt_energy_csharp.Energy.Generators;
using hmt_energy_csharp.Energy.LiquidLevels;
using hmt_energy_csharp.Energy.PowerUnits;
using hmt_energy_csharp.Energy.Predictions;
using hmt_energy_csharp.Energy.Shafts;
using hmt_energy_csharp.Energy.SternSealings;
using hmt_energy_csharp.Energy.SupplyUnits;
using hmt_energy_csharp.Energy.TotalIndicators;
using hmt_energy_csharp.IEC61162SX5s;
using hmt_energy_csharp.ResponseResults;
using hmt_energy_csharp.Sentences;
using hmt_energy_csharp.TempDatas.Drafts;
using hmt_energy_csharp.VdrDpts;
using hmt_energy_csharp.VdrGgas;
using hmt_energy_csharp.VdrGnss;
using hmt_energy_csharp.VdrMwds;
using hmt_energy_csharp.VdrRmcs;
using hmt_energy_csharp.VdrVbws;
using hmt_energy_csharp.VdrVlws;
using hmt_energy_csharp.VdrVtgs;
using hmt_energy_csharp.VesselInfos;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace hmt_energy_csharp.ProtocolDatas
{
    public class ProtocolDataBTService : hmt_energy_csharpAppService, IProtocolDataBTService
    {
        private readonly ILogger<ProtocolDataBTService> _logger;
        private readonly IVesselInfoService _vesselInfoService;
        private readonly IFlowmeterService _flowmeterService;
        private readonly IBatteryService _batteryService;
        private readonly IGeneratorService _generatorService;
        private readonly ILiquidLevelService _liquidLevelService;
        private readonly ISupplyUnitsService _supplyUnitsService;
        private readonly IShaftService _shaftService;
        private readonly ISternSealingService _sternSealingService;
        private readonly IPowerUnitService _powerUnitService;
        private readonly ITotalIndicatorService _totalIndicatorService;
        private readonly IPredictionService _predictionService;
        private readonly ISentenceService _sentenceService;

        public ProtocolDataBTService(
            ILogger<ProtocolDataBTService> logger,
            IVesselInfoService vesselInfoService,
            IFlowmeterService flowmeterService,
            IBatteryService batteryService,
            IGeneratorService generatorService,
            ILiquidLevelService liquidLevelService,
            ISupplyUnitsService supplyUnitsService,
            IShaftService shaftService,
            ISternSealingService sternSealingService,
            IPowerUnitService powerUnitService,
            ITotalIndicatorService totalIndicatorService,
            IPredictionService predictionService,
            ISentenceService sentenceService)
        {
            _logger = logger;
            _vesselInfoService = vesselInfoService;
            _flowmeterService = flowmeterService;
            _batteryService = batteryService;
            _generatorService = generatorService;
            _liquidLevelService = liquidLevelService;
            _supplyUnitsService = supplyUnitsService;
            _shaftService = shaftService;
            _sternSealingService = sternSealingService;
            _powerUnitService = powerUnitService;
            _totalIndicatorService = totalIndicatorService;
            _predictionService = predictionService;
            _sentenceService = sentenceService;
        }

        /// <summary>
        /// 解析
        /// </summary>
        /// <param name="number">采集系统序列号</param>
        /// <param name="sentence">语句</param>
        /// <param name="startChars">起始符</param>
        /// <param name="endChars">结束符</param>
        /// <returns></returns>
        public async Task<ResponseResult> DecodeAsync(string number, string sentence, string startChars, string endChars)
        {
            var result = 0;
            try
            {
                //协议语句临时列表初始化判断
                if (!StaticEntities.StaticBTEntities.ProtocolParams.Any(t => t.number == number))
                    StaticEntities.StaticBTEntities.ProtocolParams.Add(new StaticEntities.ProtocolParam { number = number });

                var tempProtocolParam = StaticEntities.StaticBTEntities.ProtocolParams.FirstOrDefault(t => t.number == number);
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

                    if (!StaticEntities.StaticBTEntities.Vessels.Any(t => t.SN == number))
                        StaticEntities.StaticBTEntities.Vessels.Add(await InitVesselAll(number, ReceiveDatetimeFmtDt));
                    //获取当前船舶信息
                    var currentVessel = StaticEntities.StaticBTEntities.Vessels.FirstOrDefault(t => t.SN == number);
                    //判断是否新时间段信息 是就进行数据保存并对属性进行初始化
                    if (currentVessel.ReceiveDatetime != ReceiveDatetimeFmtDt)
                    {
                        CalcVesselInfo(number);

                        await SaveVesselAll(number);

                        currentVessel = await InitVesselAll(number, ReceiveDatetimeFmtDt);
                    }

                    #region 解析过程

                    if (protocolType == "other")
                    {
                        var strDataNews = oriSentence.Split(',').ToList();
                        var deviceCode = strDataNews[0];
                        strDataNews.RemoveAt(0);
                        var EncodeSentence = string.Join(",", strDataNews);
                        DataProccess(number, deviceCode, EncodeSentence);
                        sentenceDto.category = deviceCode;
                    }
                    else if (protocolType == "MODBUS")
                    {
                        /*oriSentence = oriSentence.Substring(8, oriSentence.Length - 12);
                        string[] strDataNews = oriSentence.Split(',');
                        if (strDataNews.Length > 2)
                        {
                            DataProccess(number, strDataNews[1], strDataNews[2]);
                            sentenceDto.category = strDataNews[1];
                        }*/
                    }
                    //61162协议数据解析
                    else if (protocolType == "61162-1")
                    {
                        oriSentence = oriSentence.Substring(0, oriSentence.Length - 3);
                        var str61162s = oriSentence.Split(',');
                        string strDevice = str61162s[0].Substring(str61162s[0].Length - 3, 3);
                        switch (strDevice)
                        {
                            case "MWV":
                                sentenceDto.category = "mwv";
                                break;

                            case "VBW":
                                var vbmEntity = new VdrVbw(sentenceDto.data);
                                currentVessel.WaterSpeed = vbmEntity.watspd;
                                currentVessel.GroundSpeed = vbmEntity.grdspd;
                                sentenceDto.category = "vbw";
                                break;

                            case "MWD":
                                var mwdEntity = new VdrMwd(sentenceDto.data);
                                currentVessel.WindDirection = mwdEntity.tdirection;
                                currentVessel.WindSpeed = mwdEntity.knspeed;
                                sentenceDto.category = "mwd";
                                break;

                            case "GGA":
                                var ggaEntity = new VdrGga(sentenceDto.data);
                                var coor = GPSHelper.GetBdFrom84(new pointLatLon(Convert.ToDouble(ggaEntity.latitude), Convert.ToDouble(ggaEntity.longitude)));
                                currentVessel.Latitude = coor.Lat;
                                currentVessel.Longitude = coor.Lon;
                                sentenceDto.category = "gga";
                                break;

                            case "RMC":
                                var rmcEntity = new VdrRmc(sentenceDto.data);
                                var coor1 = GPSHelper.GetBdFrom84(new pointLatLon(Convert.ToDouble(rmcEntity.latitude), Convert.ToDouble(rmcEntity.longtitude)));
                                currentVessel.Latitude = coor1.Lat;
                                currentVessel.Longitude = coor1.Lon;
                                currentVessel.Course = Convert.ToDouble(rmcEntity.grdcoz);
                                currentVessel.MagneticVariation = Convert.ToDouble(rmcEntity.magvar);
                                sentenceDto.category = "rmc";
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
                                currentVessel.Course = vtgEntity.grdcoztrue;
                                currentVessel.MagneticVariation = vtgEntity.grdcozmag;
                                currentVessel.GroundSpeed = vtgEntity.grdspdknot;
                                sentenceDto.category = "vtg";
                                break;

                            case "GNS":
                                var gnsEntity = new VdrGns(sentenceDto.data);
                                var coor2 = GPSHelper.GetBdFrom84(new pointLatLon(Convert.ToDouble(gnsEntity.latitude), Convert.ToDouble(gnsEntity.longtitude)));
                                currentVessel.Latitude = coor2.Lat;
                                currentVessel.Longitude = coor2.Lon;
                                sentenceDto.category = "gns";
                                break;

                            case "XDR":
                                sentenceDto.category = "xdr";
                                break;

                            case "PRC":
                                sentenceDto.category = "prc";
                                break;

                            case "VLW":
                                var vlwEntity = new VdrVlw(sentenceDto.data);
                                currentVessel.TotalDistanceGrd = vlwEntity.grddistotal;
                                currentVessel.ResetDistanceGrd = vlwEntity.grddisreset;
                                currentVessel.TotalDistanceWat = vlwEntity.watdistotal;
                                currentVessel.ResetDistanceWat = vlwEntity.watdisreset;
                                sentenceDto.category = "vlw";
                                break;

                            case "HDG":
                                sentenceDto.category = "hdg";
                                break;

                            case "DPT":
                                var dptEntity = new VdrDpt(sentenceDto.data);
                                currentVessel.Depth = dptEntity.depth;
                                currentVessel.DepthOffset = dptEntity.offset;
                                sentenceDto.category = "dpt";
                                break;

                            case "DRA":
                                var draEntity = new Draft(sentenceDto.data);
                                currentVessel.BowDraft = Convert.ToDouble(draEntity.Bow);
                                currentVessel.AsternDraft = Convert.ToDouble(draEntity.Astern);
                                currentVessel.PortDraft = Convert.ToDouble(draEntity.Port);
                                currentVessel.StarBoardDraft = Convert.ToDouble(draEntity.StartBoard);
                                sentenceDto.category = "dra";
                                break;
                        }
                    }
                    result = await _sentenceService.CreateAsync(sentenceDto) == null ? 0 : 1;

                    #endregion 解析过程
                }
            }
            catch (Exception)
            {
                throw;
            }
            return new ResponseResult { IsSuccess = result > 0 };
        }

        /// <summary>
        /// 保存实时数据
        /// </summary>
        /// <param name="number"></param>
        private async Task SaveVesselAll(string number)
        {
            var vessel = StaticEntities.StaticBTEntities.Vessels.FirstOrDefault(t => t.SN == number);
            var ti = StaticEntities.StaticBTEntities.TotalIndicators.FirstOrDefault(t => t.Number == number);
            var p = StaticEntities.StaticBTEntities.Predictions.FirstOrDefault(t => t.Number == number);
            var bs = StaticEntities.StaticBTEntities.Batteries.FirstOrDefault(t => t.Number == number).BatteryDtos;
            var fs = StaticEntities.StaticBTEntities.Flowmeters.FirstOrDefault(t => t.Number == number).FlowmeterDtos;
            var gs = StaticEntities.StaticBTEntities.Generators.FirstOrDefault(t => t.Number == number).GeneratorDtos;
            var lls = StaticEntities.StaticBTEntities.LiquidLevels.FirstOrDefault(t => t.Number == number).LiquidLevelDtos;
            var ss = StaticEntities.StaticBTEntities.Shafts.FirstOrDefault(t => t.Number == number).ShaftDtos;
            var sss = StaticEntities.StaticBTEntities.SternSealings.FirstOrDefault(t => t.Number == number).SternSealingDtos;
            var sus = StaticEntities.StaticBTEntities.SupplyUnits.FirstOrDefault(t => t.Number == number).SupplyUnitDtos;
            var pus = StaticEntities.StaticBTEntities.PowerUnits.FirstOrDefault(t => t.Number == number).PowerUnitDtos;

            await _vesselInfoService.SaveVesselAllAsync(vessel, ti, p, bs, fs, gs, lls, ss, sss, sus, pus);
        }

        /// <summary>
        /// 初始化船舶所有属性
        /// </summary>
        /// <param name="number"></param>
        /// <param name="receiveDatetime"></param>
        /// <returns></returns>
        private async Task<VesselInfoDto> InitVesselAll(string number, DateTime receiveDatetime)
        {
            var tempVessel = await _vesselInfoService.GetByNumberReceiveDatetime(number, receiveDatetime);
            if (tempVessel == null)
                tempVessel = new VesselInfoDto { SN = number, ReceiveDatetime = receiveDatetime };

            var fmsExist = await _flowmeterService.GetListByNumberReceiveDatetimeAsync(number, receiveDatetime);
            var fms = StaticEntities.StaticBTEntities.Flowmeters.FirstOrDefault(t => t.Number == number);
            if (fms == null)
            {
                if (fmsExist != null && fmsExist.Count > 0)
                    StaticEntities.StaticBTEntities.Flowmeters.Add(new VesselFlowmeterDto { Number = number, FlowmeterDtos = fmsExist });
                else
                    StaticEntities.StaticBTEntities.Flowmeters.Add(new VesselFlowmeterDto { Number = number });
            }
            else
            {
                fms.FlowmeterDtos.Clear();
                if (fmsExist != null && fmsExist.Count > 0)
                    fms.FlowmeterDtos = fmsExist;
            }

            var bsExist = await _batteryService.GetListByNumberReceiveDatetimeAsync(number, receiveDatetime);
            var bs = StaticEntities.StaticBTEntities.Batteries.FirstOrDefault(t => t.Number == number);
            if (bs == null)
            {
                if (bsExist != null && bsExist.Count > 0)
                    StaticEntities.StaticBTEntities.Batteries.Add(new VesselBatteryDto { Number = number, BatteryDtos = bsExist });
                else
                    StaticEntities.StaticBTEntities.Batteries.Add(new VesselBatteryDto { Number = number });
            }
            else
            {
                bs.BatteryDtos.Clear();
                if (bsExist != null && bsExist.Count > 0)
                    bs.BatteryDtos = bsExist;
            }

            var gsExist = await _generatorService.GetListByNumberReceiveDatetimeAsync(number, receiveDatetime);
            var gs = StaticEntities.StaticBTEntities.Generators.FirstOrDefault(t => t.Number == number);
            if (gs == null)
            {
                if (gsExist != null && gsExist.Count > 0)
                    StaticEntities.StaticBTEntities.Generators.Add(new VesselGeneratorDto { Number = number, GeneratorDtos = gsExist });
                else
                    StaticEntities.StaticBTEntities.Generators.Add(new VesselGeneratorDto { Number = number });
            }
            else
            {
                gs.GeneratorDtos.Clear();
                if (gsExist != null && gsExist.Count > 0)
                    gs.GeneratorDtos = gsExist;
            }

            var llsExist = await _liquidLevelService.GetListByNumberReceiveDatetimeAsync(number, receiveDatetime);
            var lls = StaticEntities.StaticBTEntities.LiquidLevels.FirstOrDefault(t => t.Number == number);
            if (lls == null)
            {
                if (llsExist != null && llsExist.Count > 0)
                    StaticEntities.StaticBTEntities.LiquidLevels.Add(new VesselLiquidLevelDto { Number = number, LiquidLevelDtos = llsExist });
                else
                    StaticEntities.StaticBTEntities.LiquidLevels.Add(new VesselLiquidLevelDto { Number = number });
            }
            else
            {
                lls.LiquidLevelDtos.Clear();
                if (llsExist != null && llsExist.Count > 0)
                    lls.LiquidLevelDtos = llsExist;
            }

            var susExist = await _supplyUnitsService.GetListByNumberReceiveDatetimeAsync(number, receiveDatetime);
            var sus = StaticEntities.StaticBTEntities.SupplyUnits.FirstOrDefault(t => t.Number == number);
            if (sus == null)
            {
                if (susExist != null && susExist.Count > 0)
                    StaticEntities.StaticBTEntities.SupplyUnits.Add(new VesselSupplyUnitDto { Number = number, SupplyUnitDtos = susExist });
                else
                    StaticEntities.StaticBTEntities.SupplyUnits.Add(new VesselSupplyUnitDto { Number = number });
            }
            else
            {
                sus.SupplyUnitDtos.Clear();
                if (susExist != null && susExist.Count > 0)
                    sus.SupplyUnitDtos = susExist;
            }

            var ssExist = await _shaftService.GetListByNumberReceiveDatetimeAsync(number, receiveDatetime);
            var ss = StaticEntities.StaticBTEntities.Shafts.FirstOrDefault(t => t.Number == number);
            if (ss == null)
            {
                if (ssExist != null && ssExist.Count > 0)
                    StaticEntities.StaticBTEntities.Shafts.Add(new VesselShaftDto { Number = number, ShaftDtos = ssExist });
                else
                    StaticEntities.StaticBTEntities.Shafts.Add(new VesselShaftDto { Number = number });
            }
            else
            {
                ss.ShaftDtos.Clear();
                if (ssExist != null && ssExist.Count > 0)
                    ss.ShaftDtos = ssExist;
            }

            var sssExist = await _sternSealingService.GetListByNumberReceiveDatetimeAsync(number, receiveDatetime);
            var sss = StaticEntities.StaticBTEntities.SternSealings.FirstOrDefault(t => t.Number == number);
            if (sss == null)
            {
                if (sssExist != null && sssExist.Count > 0)
                    StaticEntities.StaticBTEntities.SternSealings.Add(new VesselSternSealingDto { Number = number, SternSealingDtos = sssExist });
                else
                    StaticEntities.StaticBTEntities.SternSealings.Add(new VesselSternSealingDto { Number = number });
            }
            else
            {
                sss.SternSealingDtos.Clear();
                if (sssExist != null && sssExist.Count > 0)
                    sss.SternSealingDtos = sssExist;
            }

            var pusExist = await _powerUnitService.GetListByNumberReceiveDatetimeAsync(number, receiveDatetime);
            var pus = StaticEntities.StaticBTEntities.PowerUnits.FirstOrDefault(t => t.Number == number);
            if (pus == null)
            {
                if (pusExist != null && pusExist.Count > 0)
                    StaticEntities.StaticBTEntities.PowerUnits.Add(new VesselPowerUnitDto { Number = number, PowerUnitDtos = pusExist });
                else
                    StaticEntities.StaticBTEntities.PowerUnits.Add(new VesselPowerUnitDto { Number = number });
            }
            else
            {
                pus.PowerUnitDtos.Clear();
                if (pusExist != null && pusExist.Count > 0)
                    pus.PowerUnitDtos = pusExist;
            }

            var tiExist = await _totalIndicatorService.GetByNumberReceiveDatetimeAsync(number, receiveDatetime);
            var ti = StaticEntities.StaticBTEntities.TotalIndicators.FirstOrDefault(t => t.Number == number);
            if (ti == null)
            {
                if (tiExist != null)
                    StaticEntities.StaticBTEntities.TotalIndicators.Add(tiExist);
                else
                    StaticEntities.StaticBTEntities.TotalIndicators.Add(new TotalIndicatorDto { Number = number, ReceiveDatetime = receiveDatetime });
            }
            else
            {
                if (tiExist != null)
                    ti = tiExist;
                else
                    ti = new TotalIndicatorDto { Number = number, ReceiveDatetime = receiveDatetime };
            }

            var pExist = await _predictionService.GetByNumberReceiveDatetimeAsync(number, receiveDatetime);
            var p = StaticEntities.StaticBTEntities.Predictions.FirstOrDefault(t => t.Number == number);
            if (p == null)
            {
                if (pExist != null)
                    StaticEntities.StaticBTEntities.Predictions.Add(pExist);
                else
                    StaticEntities.StaticBTEntities.Predictions.Add(new PredictionDto { Number = number, ReceiveDatetime = receiveDatetime });
            }
            else
            {
                if (pExist != null)
                    p = pExist;
                else
                    p = new PredictionDto { Number = number, ReceiveDatetime = receiveDatetime };
            }

            return tempVessel;
        }

        /// <summary>
        /// 船舶实时数据加工
        /// </summary>
        /// <param name="number"></param>
        private void CalcVesselInfo(string number)
        {
            var vessel = StaticEntities.StaticBTEntities.Vessels.FirstOrDefault(t => t.SN == number);
            var totalIndicator = StaticEntities.StaticBTEntities.TotalIndicators.FirstOrDefault(t => t.Number == number);
            var flowmeters = StaticEntities.StaticBTEntities.Flowmeters.FirstOrDefault(t => t.Number == number).FlowmeterDtos;
            var shafts = StaticEntities.StaticBTEntities.Shafts.FirstOrDefault(t => t.Number == number).ShaftDtos;
            var powerUnits = StaticEntities.StaticBTEntities.PowerUnits.FirstOrDefault(t => t.Number == number).PowerUnitDtos;

            var TotalHFO = flowmeters.Where(t => t.DeviceType == "me" && t.FuelType == "HFO").Sum(t => t.ConsAct);
            var TotalPower = shafts.Sum(t => t.Power);
            var MEHFO = powerUnits.FirstOrDefault(t => t.DeviceType == "me")?.HFO;
            vessel.SFOC = (double)(TotalHFO / TotalPower);
            vessel.FCPerNm = (double)TotalHFO / vessel.GroundSpeed;
            vessel.MESFOC = (double)(MEHFO / TotalPower);
            vessel.MEFCPerNm = (double)MEHFO / vessel.GroundSpeed;
            var tempSlip = 0m;
            foreach (var dto in shafts)
            {
                tempSlip += (decimal)vessel.WaterSpeed / (decimal)dto.RPM / 6000 * 1852 * 1000 / 60;
            }
            vessel.Slip = (double)(1 - tempSlip) * 100;
        }

        /// <summary>
        /// 数据解析
        /// </summary>
        /// <param name="number">采集系统id</param>
        /// <param name="deviceCode">设备代码</param>
        /// <param name="sentence">传输语句</param>
        public void DataProccess(string number, string deviceCode, string sentence)
        {
            var lstFm = StaticEntities.StaticBTEntities.Flowmeters.FirstOrDefault(t => t.Number == number).FlowmeterDtos;
            var lstBa = StaticEntities.StaticBTEntities.Batteries.FirstOrDefault(t => t.Number == number).BatteryDtos;
            var lstGe = StaticEntities.StaticBTEntities.Generators.FirstOrDefault(t => t.Number == number).GeneratorDtos;
            var lstSu = StaticEntities.StaticBTEntities.SupplyUnits.FirstOrDefault(t => t.Number == number).SupplyUnitDtos;
            var lstSs = StaticEntities.StaticBTEntities.SternSealings.FirstOrDefault(t => t.Number == number).SternSealingDtos;
            var lstSh = StaticEntities.StaticBTEntities.Shafts.FirstOrDefault(t => t.Number == number).ShaftDtos;
            var lstLl = StaticEntities.StaticBTEntities.LiquidLevels.FirstOrDefault(t => t.Number == number).LiquidLevelDtos;
            var lstPu = StaticEntities.StaticBTEntities.PowerUnits.FirstOrDefault(t => t.Number == number).PowerUnitDtos;

            var currentVessel = StaticEntities.StaticBTEntities.Vessels.FirstOrDefault(t => t.SN == number);
            var currentTotalIndicator = StaticEntities.StaticBTEntities.TotalIndicators.FirstOrDefault(t => t.Number == number);

            var consAct = 0m;

            switch (deviceCode)
            {
                case "mefuel1_in":
                    consAct = Filtering(number, deviceCode, Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[0]));
                    if (lstFm.Any(t => t.Number == number && t.DeviceNo == "mefuel1" && t.DeviceType == "me"))
                    {
                        var fm = lstFm.First(t => t.Number == number && t.DeviceNo == "mefuel1" && t.DeviceType == "me");
                        fm.ConsAct = fm.ConsAct ?? 0 + consAct;
                        fm.ConsAcc = fm.ConsAcc ?? 0 + Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[1]);
                        fm.Temperature = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[2]);
                        fm.Density = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[3]);
                        fm.DeviceType = "me";
                        fm.FuelType = "HFO";
                        fm.Number = number;
                        fm.ReceiveDatetime = currentVessel.ReceiveDatetime;
                        fm.DeviceNo = "mefuel1";
                    }
                    else
                    {
                        lstFm.Add(new FlowmeterDto
                        {
                            ConsAct = consAct,
                            ConsAcc = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[1]),
                            Temperature = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[2]),
                            Density = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[3]),
                            DeviceType = "me",
                            FuelType = "HFO",
                            Number = number,
                            ReceiveDatetime = currentVessel.ReceiveDatetime,
                            DeviceNo = "mefuel1"
                        });
                    }

                    if (lstPu.Any(t => t.Number == number && t.DeviceType == "me"))
                    {
                        var pu = lstPu.First(t => t.Number == number && t.DeviceType == "me");
                        pu.HFO = pu.HFO ?? 0 + consAct;
                        pu.HFOAccumulated = pu.HFOAccumulated ?? 0 + Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[1]);
                    }
                    else
                    {
                        lstPu.Add(new PowerUnitDto
                        {
                            HFO = consAct,
                            HFOAccumulated = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[1]),
                            DeviceType = "me",
                            Number = number,
                            ReceiveDatetime = currentVessel.ReceiveDatetime
                        });
                    }

                    currentTotalIndicator.HFO = currentTotalIndicator.HFO ?? 0 + consAct;
                    currentTotalIndicator.HFOAccumulated = currentTotalIndicator.HFOAccumulated ?? 0 + Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[1]);
                    break;

                case "mefuel1_out":
                    consAct = Filtering(number, deviceCode, Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[0]));
                    if (lstFm.Any(t => t.Number == number && t.DeviceNo == "mefuel1" && t.DeviceType == "me"))
                    {
                        var fm = lstFm.First(t => t.Number == number && t.DeviceNo == "mefuel1" && t.DeviceType == "me");
                        fm.ConsAct = fm.ConsAct ?? 0 - consAct;
                        fm.ConsAcc = fm.ConsAcc ?? 0 - Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[1]);
                    }
                    else
                    {
                        lstFm.Add(new FlowmeterDto
                        {
                            ConsAct = -consAct,
                            ConsAcc = -Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[1]),
                            Number = number,
                            DeviceNo = "mefuel1",
                            DeviceType = "me"
                        });
                    }

                    if (lstPu.Any(t => t.Number == number && t.DeviceType == "me"))
                    {
                        var pu = lstPu.First(t => t.Number == number && t.DeviceType == "me");
                        pu.HFO = pu.HFO ?? 0 - consAct;
                        pu.HFOAccumulated = pu.HFOAccumulated ?? 0 - Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[1]);
                    }
                    else
                    {
                        lstPu.Add(new PowerUnitDto
                        {
                            HFO = -consAct,
                            HFOAccumulated = -Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[1]),
                            DeviceType = "me",
                            Number = number,
                            ReceiveDatetime = currentVessel.ReceiveDatetime
                        });
                    }

                    currentTotalIndicator.HFO = currentTotalIndicator.HFO ?? 0 - consAct;
                    currentTotalIndicator.HFOAccumulated = currentTotalIndicator.HFOAccumulated ?? 0 - Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[1]);
                    break;

                case "mefuel2_in":
                    consAct = Filtering(number, deviceCode, Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[0]));
                    if (lstFm.Any(t => t.Number == number && t.DeviceNo == "mefuel2" && t.DeviceType == "me"))
                    {
                        var fm = lstFm.First(t => t.Number == number && t.DeviceNo == "mefuel2" && t.DeviceType == "me");
                        fm.ConsAct = fm.ConsAct ?? 0 + consAct;
                        fm.ConsAcc = fm.ConsAcc ?? 0 + Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[1]);
                        fm.Temperature = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[2]);
                        fm.Density = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[3]);
                        fm.DeviceType = "me";
                        fm.FuelType = "HFO";
                        fm.Number = number;
                        fm.ReceiveDatetime = currentVessel.ReceiveDatetime;
                        fm.DeviceNo = "mefuel2";
                    }
                    else
                    {
                        lstFm.Add(new FlowmeterDto
                        {
                            ConsAct = consAct,
                            ConsAcc = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[1]),
                            Temperature = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[2]),
                            Density = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[3]),
                            DeviceType = "me",
                            FuelType = "HFO",
                            Number = number,
                            ReceiveDatetime = currentVessel.ReceiveDatetime,
                            DeviceNo = "mefuel2"
                        });
                    }

                    if (lstPu.Any(t => t.Number == number && t.DeviceType == "me"))
                    {
                        var pu = lstPu.First(t => t.Number == number && t.DeviceType == "me");
                        pu.HFO = pu.HFO ?? 0 + consAct;
                        pu.HFOAccumulated = pu.HFOAccumulated ?? 0 + Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[1]);
                    }
                    else
                    {
                        lstPu.Add(new PowerUnitDto
                        {
                            HFO = consAct,
                            HFOAccumulated = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[1]),
                            DeviceType = "me",
                            Number = number,
                            ReceiveDatetime = currentVessel.ReceiveDatetime
                        });
                    }

                    currentTotalIndicator.HFO = currentTotalIndicator.HFO ?? 0 + consAct;
                    currentTotalIndicator.HFOAccumulated = currentTotalIndicator.HFOAccumulated ?? 0 + Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[1]);
                    break;

                case "mefuel2_out":
                    consAct = Filtering(number, deviceCode, Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[0]));
                    if (lstFm.Any(t => t.Number == number && t.DeviceNo == "mefuel2" && t.DeviceType == "me"))
                    {
                        var fm = lstFm.First(t => t.Number == number && t.DeviceNo == "mefuel2" && t.DeviceType == "me");
                        fm.ConsAct = fm.ConsAct ?? 0 - consAct;
                        fm.ConsAcc = fm.ConsAcc ?? 0 - Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[1]);
                    }
                    else
                    {
                        lstFm.Add(new FlowmeterDto
                        {
                            ConsAct = -consAct,
                            ConsAcc = -Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[1]),
                            Number = number,
                            DeviceNo = "mefuel2",
                            DeviceType = "me"
                        });
                    }

                    if (lstPu.Any(t => t.Number == number && t.DeviceType == "me"))
                    {
                        var pu = lstPu.First(t => t.Number == number && t.DeviceType == "me");
                        pu.HFO = pu.HFO ?? 0 - consAct;
                        pu.HFOAccumulated = pu.HFOAccumulated ?? 0 - Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[1]);
                    }
                    else
                    {
                        lstPu.Add(new PowerUnitDto
                        {
                            HFO = -consAct,
                            HFOAccumulated = -Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[1]),
                            DeviceType = "me",
                            Number = number,
                            ReceiveDatetime = currentVessel.ReceiveDatetime
                        });
                    }

                    currentTotalIndicator.HFO = currentTotalIndicator.HFO ?? 0 - consAct;
                    currentTotalIndicator.HFOAccumulated = currentTotalIndicator.HFOAccumulated ?? 0 - Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[1]);
                    break;

                case "memethanol":
                    consAct = Filtering(number, deviceCode, Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[0]));
                    if (lstFm.Any(t => t.Number == number && t.DeviceNo == "memethanol" && t.DeviceType == "me"))
                    {
                        var fm = lstFm.First(t => t.Number == number && t.DeviceNo == "memethanol" && t.DeviceType == "me");
                        fm.ConsAct = fm.ConsAct ?? 0 + consAct;
                        fm.ConsAcc = fm.ConsAcc ?? 0 + Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[1]);
                        fm.Temperature = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[2]);
                        fm.Density = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[3]);
                        fm.DeviceType = "me";
                        fm.FuelType = "Methanol";
                        fm.Number = number;
                        fm.ReceiveDatetime = currentVessel.ReceiveDatetime;
                        fm.DeviceNo = "memethanol";
                    }
                    else
                    {
                        lstFm.Add(new FlowmeterDto
                        {
                            ConsAct = consAct,
                            ConsAcc = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[1]),
                            Temperature = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[2]),
                            Density = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[3]),
                            DeviceType = "me",
                            FuelType = "Methanol",
                            Number = number,
                            ReceiveDatetime = currentVessel.ReceiveDatetime,
                            DeviceNo = "memethanol",
                        });
                    }

                    if (lstPu.Any(t => t.Number == number && t.DeviceType == "me"))
                    {
                        var pu = lstPu.First(t => t.Number == number && t.DeviceType == "me");
                        pu.Methanol = pu.Methanol ?? 0 + consAct;
                        pu.MethanolAccumulated = pu.MethanolAccumulated ?? 0 + Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[1]);
                    }
                    else
                    {
                        lstPu.Add(new PowerUnitDto
                        {
                            Methanol = consAct,
                            MethanolAccumulated = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[1]),
                            DeviceType = "me",
                            Number = number,
                            ReceiveDatetime = currentVessel.ReceiveDatetime
                        });
                    }

                    currentTotalIndicator.Methanol = currentTotalIndicator.Methanol ?? 0 + consAct;
                    currentTotalIndicator.MethanolAccumulated = currentTotalIndicator.MethanolAccumulated ?? 0 + Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[1]);
                    break;

                case "draft":
                    currentVessel.BowDraft = Convert.ToDouble(DecodeProtocalData(deviceCode, sentence)[0]);
                    currentVessel.AsternDraft = Convert.ToDouble(DecodeProtocalData(deviceCode, sentence)[1]);
                    currentVessel.PortDraft = Convert.ToDouble(DecodeProtocalData(deviceCode, sentence)[2]);
                    currentVessel.StarBoardDraft = Convert.ToDouble(DecodeProtocalData(deviceCode, sentence)[3]);
                    currentVessel.Draft = (currentVessel.BowDraft + currentVessel.AsternDraft) / 2d;
                    currentVessel.Trim = currentVessel.AsternDraft - currentVessel.BowDraft;
                    currentVessel.Heel = currentVessel.PortDraft - currentVessel.StarBoardDraft;
                    break;

                case "shaft1":
                case "shaft2":
                    if (lstSh.Any(t => t.Number == number && t.DeviceNo == deviceCode))
                    {
                        var shaft = lstSh.First(t => t.Number == number && t.DeviceNo == deviceCode);
                        shaft.Power = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[0]);
                        shaft.RPM = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[1]);
                        shaft.Torque = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[2]);
                        shaft.Thrust = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[3]);
                        shaft.Number = number;
                        shaft.ReceiveDatetime = currentVessel.ReceiveDatetime;
                        shaft.DeviceNo = deviceCode;
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
                            ReceiveDatetime = currentVessel.ReceiveDatetime,
                            DeviceNo = deviceCode
                        });
                    }

                    currentTotalIndicator.Power = currentTotalIndicator.Power ?? 0 + Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[0]);
                    currentTotalIndicator.Torque = currentTotalIndicator.Torque ?? 0 + Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[2]);
                    currentTotalIndicator.Thrust = currentTotalIndicator.Thrust ?? 0 + Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[3]);
                    break;

                case "generator1":
                case "generator2":
                case "generator3":
                case "generator4":
                    if (lstGe.Any(t => t.Number == number && t.DeviceNo == deviceCode))
                    {
                        var generator = lstGe.First(t => t.Number == number && t.DeviceNo == deviceCode);
                        generator.IsRuning = Convert.ToByte(DecodeProtocalData(deviceCode, sentence)[0]);
                        generator.RPM = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[1]);
                        generator.StartPressure = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[2]);
                        generator.ControlPressure = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[3]);
                        generator.ScavengingPressure = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[4]);
                        generator.LubePressure = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[5]);
                        generator.LubeTEMP = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[6]);
                        generator.FuelPressure = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[7]);
                        generator.FuelTEMP = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[8]);
                        generator.FreshWaterPressure = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[9]);
                        generator.FreshWaterTEMPIn = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[10]);
                        generator.FreshWaterTEMPOut = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[11]);
                        generator.CoolingWaterPressure = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[12]);
                        generator.CoolingWaterTEMPIn = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[13]);
                        generator.CoolingWaterTEMPOut = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[14]);
                        generator.CylinderTEMP1 = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[15]);
                        generator.CylinderTEMP2 = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[16]);
                        generator.CylinderTEMP3 = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[17]);
                        generator.CylinderTEMP4 = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[18]);
                        generator.CylinderTEMP5 = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[19]);
                        generator.CylinderTEMP6 = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[20]);
                        generator.SuperchargerTEMPIn = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[21]);
                        generator.SuperchargerTEMPOut = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[22]);
                        generator.ScavengingTEMP = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[23]);
                        generator.BearingTEMP = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[24]);
                        generator.BearingTEMPFront = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[25]);
                        generator.BearingTEMPBack = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[26]);
                        generator.Power = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[27]);
                        generator.WindingTEMPL1 = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[28]);
                        generator.WindingTEMPL2 = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[29]);
                        generator.WindingTEMPL3 = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[30]);
                        generator.VoltageL1L2 = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[31]);
                        generator.VoltageL2L3 = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[32]);
                        generator.VoltageL1L3 = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[33]);
                        generator.FrequencyL1 = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[34]);
                        generator.FrequencyL2 = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[35]);
                        generator.FrequencyL3 = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[36]);
                        generator.CurrentL1 = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[37]);
                        generator.CurrentL2 = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[38]);
                        generator.CurrentL3 = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[39]);
                        generator.ReactivePower = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[40]);
                        generator.PowerFactor = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[41]);
                        generator.LoadRatio = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[42]);
                        generator.ReceiveDatetime = currentVessel.ReceiveDatetime;
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
                            ReceiveDatetime = currentVessel.ReceiveDatetime,
                            DeviceNo = deviceCode,
                            Number = number
                        });
                    }
                    break;

                case "battery1":
                case "battery2":
                    if (lstBa.Any(t => t.Number == number && t.DeviceNo == deviceCode))
                    {
                        var battery = lstBa.First(t => t.Number == number && t.DeviceNo == deviceCode);
                        battery.SOC = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[0]);
                        battery.SOH = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[1]);
                        battery.MaxTEMP = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[2]);
                        battery.MaxTEMPBox = DecodeProtocalData(deviceCode, sentence)[3].ToString();
                        battery.MaxTEMPNo = DecodeProtocalData(deviceCode, sentence)[4].ToString();
                        battery.MinTEMP = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[5]);
                        battery.MinTEMPBox = DecodeProtocalData(deviceCode, sentence)[6].ToString();
                        battery.MinTEMPNo = DecodeProtocalData(deviceCode, sentence)[7].ToString();
                        battery.MaxVoltage = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[8]);
                        battery.MaxVoltageBox = DecodeProtocalData(deviceCode, sentence)[9].ToString();
                        battery.MaxVoltageNo = DecodeProtocalData(deviceCode, sentence)[10].ToString();
                        battery.MinVoltage = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[11]);
                        battery.MinVoltageBox = DecodeProtocalData(deviceCode, sentence)[12].ToString();
                        battery.MinVoltageNo = DecodeProtocalData(deviceCode, sentence)[13].ToString();
                        battery.ReceiveDatetime = currentVessel.ReceiveDatetime;
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
                            ReceiveDatetime = currentVessel.ReceiveDatetime,
                            DeviceNo = deviceCode,
                            Number = number
                        });
                    }
                    break;

                case "sternsealing1":
                case "sternsealing2":
                    if (lstSs.Any(t => t.Number == number && t.DeviceNo == deviceCode))
                    {
                        var sternsealing = lstSs.First(t => t.Number == number && t.DeviceNo == deviceCode);
                        sternsealing.FrontTEMP = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[0]);
                        sternsealing.BackTEMP = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[1]);
                        sternsealing.BackLeftTEMP = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[2]);
                        sternsealing.BackRightTEMP = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[3]);
                        sternsealing.ReceiveDatetime = currentVessel.ReceiveDatetime;
                    }
                    else
                    {
                        lstSs.Add(new SternSealingDto
                        {
                            FrontTEMP = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[0]),
                            BackTEMP = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[1]),
                            BackLeftTEMP = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[2]),
                            BackRightTEMP = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[3]),
                            ReceiveDatetime = currentVessel.ReceiveDatetime,
                            DeviceNo = deviceCode,
                            Number = number
                        });
                    }
                    break;

                case "liquidlevel1":
                case "liquidlevel2":
                    if (lstLl.Any(t => t.Number == number && t.DeviceNo == deviceCode))
                    {
                        var liquidlevel = lstLl.First(t => t.Number == number && t.DeviceNo == deviceCode);
                        liquidlevel.Level = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[0]);
                        liquidlevel.Temperature = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[1]);
                        liquidlevel.ReceiveDatetime = currentVessel.ReceiveDatetime;
                    }
                    else
                    {
                        lstLl.Add(new LiquidLevelDto
                        {
                            Level = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[0]),
                            Temperature = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[1]),
                            ReceiveDatetime = currentVessel.ReceiveDatetime,
                            DeviceNo = deviceCode,
                            Number = number
                        });
                    }
                    break;

                case "supplyunit1":
                case "supplyunit2":
                case "supplyunit3":
                    if (lstSu.Any(t => t.Number == number && t.DeviceNo == deviceCode))
                    {
                        var supplyunit = lstSu.First(t => t.Number == number && t.DeviceNo == deviceCode);
                        supplyunit.IsRuning = Convert.ToByte(DecodeProtocalData(deviceCode, sentence)[0]);
                        supplyunit.Temperature = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[1]);
                        supplyunit.Pressure = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[2]);
                        supplyunit.ReceiveDatetime = currentVessel.ReceiveDatetime;
                    }
                    else
                    {
                        lstSu.Add(new SupplyUnitDto
                        {
                            IsRuning = Convert.ToByte(DecodeProtocalData(deviceCode, sentence)[0]),
                            Temperature = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[1]),
                            Pressure = Convert.ToDecimal(DecodeProtocalData(deviceCode, sentence)[2]),
                            ReceiveDatetime = currentVessel.ReceiveDatetime,
                            DeviceNo = deviceCode,
                            Number = number
                        });
                    }
                    break;
            }
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
                case "mefuel1_in":
                case "mefuel1_out":
                case "mefuel2_in":
                case "mefuel2_out":
                case "memethanol":
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

                case "shaft1":
                case "shaft2":
                    var shaft = new Shaft(sentence);
                    result.Add(shaft.Power);
                    result.Add(shaft.RPM);
                    result.Add(shaft.Torque);
                    result.Add(shaft.Thrust);
                    break;

                case "generator1":
                case "generator2":
                case "generator3":
                case "generator4":
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

                case "battery1":
                case "battery2":
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

                case "sternsealing1":
                case "sternsealing2":
                    var sternsealing = new SternSealing(sentence);
                    result.Add(sternsealing.FrontTEMP);
                    result.Add(sternsealing.BackTEMP);
                    result.Add(sternsealing.BackLeftTEMP);
                    result.Add(sternsealing.BackRightTEMP);
                    break;

                case "liquidlevel1":
                case "liquidlevel2":
                    var liquidlevel = new LiquidLevel(sentence);
                    result.Add(liquidlevel.Level);
                    result.Add(liquidlevel.Temperature);
                    break;

                case "supplyunit1":
                case "supplyunit2":
                case "supplyunit3":
                    var supplyunit = new SupplyUnit(sentence);
                    result.Add(supplyunit.IsRuning);
                    result.Add(supplyunit.Temperature);
                    result.Add(supplyunit.Pressure);
                    break;
            }
            return result.ToArray();
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
            if (StaticEntities.StaticBTEntities.FilteringParams.Any(t => t.Number == number && t.DeviceNo == deviceCode))
                StaticEntities.StaticBTEntities.FilteringParams.Add(new StaticEntities.FilteringParam { Number = number, DeviceNo = deviceCode });
            var curList = StaticEntities.StaticBTEntities.FilteringParams.FirstOrDefault(t => t.Number == number && t.DeviceNo == deviceCode).Values;
            curList.Add(value);
            if (curList.Count > 12)
            {
                curList.RemoveAt(0);
                result = curList.Average();
            }
            else
                result = value;
            return result;
        }
    }
}