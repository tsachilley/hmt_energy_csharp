using hmt_energy_csharp;
using hmt_energy_csharp.AnalyseStaticDatas;
using hmt_energy_csharp.Connections;
using hmt_energy_csharp.Energy.Batteries;
using hmt_energy_csharp.Energy.Flowmeters;
using hmt_energy_csharp.Energy.Generators;
using hmt_energy_csharp.Energy.LiquidLevels;
using hmt_energy_csharp.Energy.Shafts;
using hmt_energy_csharp.Energy.SternSealings;
using hmt_energy_csharp.Energy.SupplyUnits;
using hmt_energy_csharp.Entites;
using hmt_energy_csharp.IEC61162SX5s;
using hmt_energy_csharp.NotVDRs;
using hmt_energy_csharp.Sentences;
using hmt_energy_csharp.VdrDpts;
using hmt_energy_csharp.VdrGgas;
using hmt_energy_csharp.VdrGnss;
using hmt_energy_csharp.VdrHdgs;
using hmt_energy_csharp.VdrMwds;
using hmt_energy_csharp.VdrMwvs;
using hmt_energy_csharp.VdrRmcs;
using hmt_energy_csharp.VdrRpms;
using hmt_energy_csharp.VdrTrds;
using hmt_energy_csharp.VdrVbws;
using hmt_energy_csharp.VdrVlws;
using hmt_energy_csharp.VdrVtgs;
using hmt_energy_csharp.VesselInfos;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EventBus.Distributed;

namespace hmt_energy_csharp.VDRs
{
    public class VDRService : hmt_energy_csharpAppService, IVDRService
    {
        private readonly IVDRRepository _repository;
        private readonly ISentenceService _sentenceService;
        private readonly ILogger<VDRService> _logger;
        private readonly IVesselInfoService _vesselInfoService;

        public VDRService(IVDRRepository repository, ISentenceService sentenceService, ILogger<VDRService> logger, IVesselInfoService vesselInfoService)
        {
            _repository = repository;
            _sentenceService = sentenceService;
            _logger = logger;
            _vesselInfoService = vesselInfoService;
        }

        public async Task<int> AddAsync(string strSql)
        {
            var result = await _repository.AddAsync(strSql);
            return result;
        }

        public async Task<object> GetVoyageRealTimeAsync(string vdrId, float propPitch)
        {
            //var dptData = await _repository.GetRealTimeAsync(vdrId);
            var tempData = await _sentenceService.GetTop1TimeByVdrAsync(vdrId);
            if (tempData.Count() > 0)
            {
                VdrVbw vdrVbw = new VdrVbw((tempData.FirstOrDefault(t => t.category == "vbw"))?.data);
                var notvdrSentence = (tempData.FirstOrDefault(t => t.category == "notvdr"))?.data;
                _logger.LogInformation($"实时数据 非vdr数据：{notvdrSentence}");
                NotVdrEntity notVdrEntity = new NotVdrEntity(notvdrSentence, vdrId, StaticVoyageData.DictFilter);
                VdrMwd vdrMwd = new VdrMwd((tempData.FirstOrDefault(t => t.category == "mwd"))?.data);
                VdrDpt vdrDpt = new VdrDpt((tempData.FirstOrDefault(t => t.category == "dpt"))?.data);
                VdrGns vdrGns = new VdrGns((tempData.FirstOrDefault(t => t.category == "gns"))?.data);
                VdrRpm vdrRpm = new VdrRpm((tempData.FirstOrDefault(t => t.category == "rpm"))?.data);
                VdrVtg vdrVtg = new VdrVtg((tempData.FirstOrDefault(t => t.category == "vtg"))?.data);
                VdrVlw vdrVlw = new VdrVlw((tempData.FirstOrDefault(t => t.category == "vlw"))?.data);
                SX5 shaft = new SX5((tempData.FirstOrDefault(t => t.category == "sx5"))?.data);
                var slip = (1 - (vdrVbw?.watspd / (notVdrEntity?.MERpm * propPitch) * 1852f * 1000f / 60f)) * 100f;
                var draft = (notVdrEntity?.DraftBow + notVdrEntity?.DraftAstern) / 2f;

                var coor = GPSHelper.GetBdFrom84(new pointLatLon(Convert.ToDouble(vdrGns?.latitude), Convert.ToDouble(vdrGns?.longtitude)));

                var dptData = new
                {
                    history_currenttime = tempData.FirstOrDefault().time,
                    history_dpt_depth = vdrDpt?.depth,
                    history_dpt_offset = vdrDpt?.offset,
                    history_gns_longtitude = coor?.Lon,
                    history_gns_latitude = coor?.Lat,
                    /*history_gns_longtitude = (new Random().NextDouble() < 0.5 ? -1f : 1f) * new Random().NextDouble() * 180f,
                    history_gns_latitude = (new Random().NextDouble() < 0.5 ? -1f : 1f) * new Random().NextDouble() * 90f,*/
                    history_gns_satnum = vdrGns?.satnum,
                    history_gns_antennaaltitude = vdrGns?.antennaaltitude,
                    history_mwd_tdirection = vdrMwd?.tdirection,
                    history_mwd_magdirection = vdrMwd?.magdirection,
                    history_mwd_knspeed = vdrMwd?.knspeed,
                    history_mwd_speed = vdrMwd?.speed,
                    history_rpm_source = vdrRpm?.source,
                    history_rpm_number = vdrRpm?.number,
                    history_rpm_speed = vdrRpm?.speed,
                    history_rpm_propellerpitch = vdrRpm?.propellerpitch,
                    history_vbw_watspd = vdrVbw?.watspd,
                    history_vbw_grdspd = vdrVbw?.grdspd,
                    history_vtg_grdcoztrue = vdrVtg?.grdcoztrue,
                    history_vtg_grdcozmag = vdrVtg?.grdcozmag,
                    history_vtg_grdspdknot = vdrVtg?.grdspdknot,
                    history_vtg_grdspdkm = vdrVtg?.grdspdkm,
                    history_vlw_watdistotal = vdrVlw?.watdistotal,
                    history_vlw_watdisreset = vdrVlw?.watdisreset,
                    history_vlw_grddistotal = vdrVlw?.grddistotal,
                    history_vlw_grddisreset = vdrVlw?.grddisreset,
                    history_draft_trim = notVdrEntity?.DraftAstern - notVdrEntity?.DraftBow,
                    history_draft_heel = notVdrEntity?.DraftPort - notVdrEntity?.DraftStarboard,
                    history_draft_draft = draft,
                    history_power_rpm = (shaft?.rpm).ToString().IsNullOrWhiteSpace() ? notVdrEntity?.MERpm : shaft.rpm,
                    history_power_power = (shaft?.power).ToString().IsNullOrWhiteSpace() ? notVdrEntity?.MEPower : shaft.power,
                    history_power_torque = (shaft?.torque).ToString().IsNullOrWhiteSpace() ? notVdrEntity?.METorque : shaft.torque,
                    history_power_thrust = (shaft?.thrust).ToString().IsNullOrWhiteSpace() ? 0 : shaft.thrust,
                    history_power_slip = slip,
                    history_flowmeter_me_fcpernm = (notVdrEntity?.MEFC ?? 0) / (vdrVbw?.watspd ?? 0),
                    history_flowmeter_me_fcperpow = (notVdrEntity?.MEFC ?? 0) / (notVdrEntity?.MEPower ?? 0) * 1000f,
                    history_flowmeter_fcpernm = (notVdrEntity?.MEFC + notVdrEntity?.DGInFC - notVdrEntity?.DGOutFC + notVdrEntity?.DGMgoFC + notVdrEntity?.BlrFC + notVdrEntity?.BlrMGOFC) / vdrVbw?.watspd,
                    history_flowmeter_fcperpow = (notVdrEntity?.MEFC + notVdrEntity?.DGInFC - notVdrEntity?.DGOutFC + notVdrEntity?.DGMgoFC + notVdrEntity?.BlrFC + notVdrEntity?.BlrMGOFC) / (notVdrEntity?.MEPower + notVdrEntity?.DG1Power + notVdrEntity?.DG2Power + notVdrEntity?.DG3Power),
                    history_dg_power = notVdrEntity?.DG1Power + notVdrEntity?.DG2Power + notVdrEntity?.DG3Power,
                    history_draft_bow = notVdrEntity?.DraftBow,
                    history_draft_astern = notVdrEntity?.DraftAstern,
                    history_draft_port = notVdrEntity?.DraftPort,
                    history_draft_starboard = notVdrEntity?.DraftStarboard,
                    history_mefc = notVdrEntity?.MEFC,
                    history_meacc = notVdrEntity?.MEAcc,
                    history_dgfc = notVdrEntity?.DGInFC - notVdrEntity?.DGOutFC,
                    history_dgacc = notVdrEntity?.DGInAcc - notVdrEntity?.DGOutAcc,
                    history_blrfc = notVdrEntity?.BlrFC,
                    history_blracc = notVdrEntity?.BlrAcc
                };
                return dptData;
            }
            else
            {
                var dptData = new
                {
                    history_currenttime = "",
                    history_dpt_depth = "",
                    history_dpt_offset = "",
                    history_gns_longtitude = "",
                    history_gns_latitude = "",
                    history_gns_satnum = "",
                    history_gns_antennaaltitude = "",
                    history_mwd_tdirection = "",
                    history_mwd_magdirection = "",
                    history_mwd_knspeed = "",
                    history_mwd_speed = "",
                    history_rpm_source = "",
                    history_rpm_number = "",
                    history_rpm_speed = "",
                    history_rpm_propellerpitch = "",
                    history_vbw_watspd = "",
                    history_vbw_grdspd = "",
                    history_vtg_grdcoztrue = "",
                    history_vtg_grdcozmag = "",
                    history_vtg_grdspdknot = "",
                    history_vtg_grdspdkm = "",
                    history_vlw_watdistotal = "",
                    history_vlw_watdisreset = "",
                    history_vlw_grddistotal = "",
                    history_vlw_grddisreset = "",
                    history_draft_trim = "",
                    history_draft_heel = "",
                    history_draft_draft = "",
                    history_power_rpm = "",
                    history_power_power = "",
                    history_power_slip = "",
                    history_flowmeter_me_fcpernm = "",
                    history_flowmeter_me_fcperpow = "",
                    history_flowmeter_fcpernm = "",
                    history_flowmeter_fcperpow = "",
                    history_dg_power = "",
                    history_draft_bow = "",
                    history_draft_astern = "",
                    history_draft_port = "",
                    history_draft_starboard = "",
                    history_mefc = "",
                    history_meacc = "",
                    history_dgfc = "",
                    history_dgacc = "",
                    history_blrfc = "",
                    history_blracc = ""
                };
                return dptData;
            }
        }

        public async Task<object> GetVoyageHistoryAsync(string vdrId, int pageNum, int pageCount, string sorting, string asc, long dateFrom, long dateTo)
        {
            /*var resultData = await _repository.GetHistoryAsync(vdrId, pageNum, pageCount, sorting, asc, dateFrom, dateTo);
            return resultData;*/
            var tempData = await _sentenceService.GetPageListAsync(vdrId, pageNum, pageCount, sorting, asc, dateFrom, dateTo);
            var resultList = new List<object>();
            var TimeList = new List<long>();
            foreach (var item in (tempData ?? new List<SentenceDto>()))
            {
                if (TimeList.Contains(item.time))
                    continue;
                TimeList.Add(item.time);

                try
                {
                    VdrVbw vdrVbw = new VdrVbw((tempData.FirstOrDefault(t => t.category == "vbw" && t.time == item.time))?.data);
                    NotVdrEntity notVdrEntity = new NotVdrEntity((tempData.FirstOrDefault(t => t.category == "notvdr" && t.time == item.time)).data, vdrId, StaticVoyageData.DictFilter);
                    VdrMwd vdrMwd = new VdrMwd((tempData.FirstOrDefault(t => t.category == "mwd" && t.time == item.time))?.data);
                    VdrDpt vdrDpt = new VdrDpt((tempData.FirstOrDefault(t => t.category == "dpt" && t.time == item.time))?.data);
                    VdrGns vdrGns = new VdrGns((tempData.FirstOrDefault(t => t.category == "gns" && t.time == item.time))?.data);
                    VdrRpm vdrRpm = new VdrRpm((tempData.FirstOrDefault(t => t.category == "rpm" && t.time == item.time))?.data);
                    VdrVtg vdrVtg = new VdrVtg((tempData.FirstOrDefault(t => t.category == "vtg" && t.time == item.time))?.data);
                    VdrVlw vdrVlw = new VdrVlw((tempData.FirstOrDefault(t => t.category == "vlw" && t.time == item.time))?.data);
                    SX5 shaft = new SX5((tempData.FirstOrDefault(t => t.category == "sx5" && t.time == item.time))?.data);
                    var slip = (1 - (vdrVbw.watspd / (notVdrEntity.MERpm * 5128.77f) * 1852f * 1000f / 60f)) * 100f;
                    var draft = (notVdrEntity.DraftBow + notVdrEntity.DraftAstern) / 2f;
                    var dptData = new
                    {
                        history_currenttime = tempData.FirstOrDefault().time,
                        history_dpt_depth = vdrDpt.depth,
                        history_dpt_offset = vdrDpt.offset,
                        /*history_gns_longtitude = vdrGns.longtitude,
                        history_gns_latitude = vdrGns.latitude,*/
                        history_gns_longtitude = (new Random().NextDouble() < 0.5 ? -1f : 1f) * new Random().NextDouble() * 180f,
                        history_gns_latitude = (new Random().NextDouble() < 0.5 ? -1f : 1f) * new Random().NextDouble() * 90f,
                        history_gns_satnum = vdrGns.satnum,
                        history_gns_antennaaltitude = vdrGns.antennaaltitude,
                        history_mwd_tdirection = vdrMwd.tdirection,
                        history_mwd_magdirection = vdrMwd.magdirection,
                        history_mwd_knspeed = vdrMwd.knspeed,
                        history_mwd_speed = vdrMwd.speed,
                        history_rpm_source = vdrRpm.source,
                        history_rpm_number = vdrRpm.number,
                        history_rpm_speed = vdrRpm.speed,
                        history_rpm_propellerpitch = vdrRpm.propellerpitch,
                        history_vbw_watspd = vdrVbw.watspd,
                        history_vbw_grdspd = vdrVbw.grdspd,
                        history_vtg_grdcoztrue = vdrVtg.grdcoztrue,
                        history_vtg_grdcozmag = vdrVtg.grdcozmag,
                        history_vtg_grdspdknot = vdrVtg.grdspdknot,
                        history_vtg_grdspdkm = vdrVtg.grdspdkm,
                        history_vlw_watdistotal = vdrVlw.watdistotal,
                        history_vlw_watdisreset = vdrVlw.watdisreset,
                        history_vlw_grddistotal = vdrVlw.grddistotal,
                        history_vlw_grddisreset = vdrVlw.grddisreset,
                        history_draft_trim = notVdrEntity.DraftAstern - notVdrEntity.DraftBow,
                        history_draft_heel = notVdrEntity.DraftPort - notVdrEntity.DraftStarboard,
                        history_draft_draft = draft,
                        history_power_rpm = (shaft?.rpm).ToString().IsNullOrWhiteSpace() ? notVdrEntity?.MERpm : shaft.rpm,
                        history_power_power = (shaft?.power).ToString().IsNullOrWhiteSpace() ? notVdrEntity?.MEPower : shaft.power,
                        history_power_torque = (shaft?.torque).ToString().IsNullOrWhiteSpace() ? notVdrEntity?.METorque : shaft.torque,
                        history_power_thrust = (shaft?.thrust).ToString().IsNullOrWhiteSpace() ? 0 : shaft.thrust,
                        history_power_slip = slip,
                        history_flowmeter_me_fcpernm = notVdrEntity.MEFC / vdrVbw.watspd,
                        history_flowmeter_me_fcperpow = notVdrEntity.MEFC / notVdrEntity.MEPower,
                        history_flowmeter_fcpernm = (notVdrEntity.MEFC + notVdrEntity.DGInFC - notVdrEntity.DGOutFC + notVdrEntity.DGMgoFC + notVdrEntity.BlrFC + notVdrEntity.BlrMGOFC) / vdrVbw.watspd,
                        history_flowmeter_fcperpow = (notVdrEntity.MEFC + notVdrEntity.DGInFC - notVdrEntity.DGOutFC + notVdrEntity.DGMgoFC + notVdrEntity.BlrFC + notVdrEntity.BlrMGOFC) / (notVdrEntity.MEPower + notVdrEntity.DG1Power + notVdrEntity.DG2Power + notVdrEntity.DG3Power),
                        history_dg_power = notVdrEntity.DG1Power + notVdrEntity.DG2Power + notVdrEntity.DG3Power,
                        history_draft_bow = notVdrEntity.DraftBow,
                        history_draft_astern = notVdrEntity.DraftAstern,
                        history_draft_port = notVdrEntity.DraftPort,
                        history_draft_starboard = notVdrEntity.DraftStarboard,
                        history_mefc = notVdrEntity.MEFC,
                        history_meacc = notVdrEntity.MEAcc,
                        history_dgfc = notVdrEntity.DGInFC - notVdrEntity.DGOutFC,
                        history_dgacc = notVdrEntity.DGInAcc - notVdrEntity.DGOutAcc,
                        history_blrfc = notVdrEntity.BlrFC,
                        history_blracc = notVdrEntity.BlrAcc
                    };
                    resultList.Add(dptData);
                }
                catch (Exception ex)
                {
                    var dptData = new
                    {
                        history_currenttime = item.time,
                        history_dpt_depth = "",
                        history_dpt_offset = "",
                        history_gns_longtitude = "",
                        history_gns_latitude = "",
                        history_gns_satnum = "",
                        history_gns_antennaaltitude = "",
                        history_mwd_tdirection = "",
                        history_mwd_magdirection = "",
                        history_mwd_knspeed = "",
                        history_mwd_speed = "",
                        history_rpm_source = "",
                        history_rpm_number = "",
                        history_rpm_speed = "",
                        history_rpm_propellerpitch = "",
                        history_vbw_watspd = "",
                        history_vbw_grdspd = "",
                        history_vtg_grdcoztrue = "",
                        history_vtg_grdcozmag = "",
                        history_vtg_grdspdknot = "",
                        history_vtg_grdspdkm = "",
                        history_vlw_watdistotal = "",
                        history_vlw_watdisreset = "",
                        history_vlw_grddistotal = "",
                        history_vlw_grddisreset = "",
                        history_draft_trim = "",
                        history_draft_heel = "",
                        history_draft_draft = "",
                        history_power_rpm = "",
                        history_power_power = "",
                        history_power_slip = "",
                        history_flowmeter_me_fcpernm = "",
                        history_flowmeter_me_fcperpow = "",
                        history_flowmeter_fcpernm = "",
                        history_flowmeter_fcperpow = "",
                        history_dg_power = "",
                        history_draft_bow = "",
                        history_draft_astern = "",
                        history_draft_port = "",
                        history_draft_starboard = "",
                        history_mefc = "",
                        history_meacc = "",
                        history_dgfc = "",
                        history_dgacc = "",
                        history_blrfc = "",
                        history_blracc = ""
                    };
                    resultList.Add(dptData);
                }
            }
            return resultList;
        }

        public async Task<object> GetVoyageHistoryChartAsync(string vdrId, int pageNum, int pageCount, string sorting, string asc, long dateFrom, long dateTo, string strParams)
        {
            var tempData = await _sentenceService.GetPageListAsync(vdrId, pageNum, pageCount, sorting, asc, dateFrom, dateTo);
            var TimeList = new List<long>();
            var parameters = (JObject)JsonConvert.DeserializeObject(strParams);
            var options = new List<string>();
            var resultDict = new Dictionary<string, List<string>>();
            foreach (var code in parameters["options"].ToString().ToList<JObject>())
            {
                var strType = code.ToString().ToJObject()["code"].ToString();
                options.Add(strType);
                resultDict.Add(strType, new List<string>());
            }
            resultDict.Add("history_currenttime", new List<string>());
            foreach (var item in (tempData ?? new List<SentenceDto>()))
            {
                if (TimeList.Contains(item.time))
                    continue;

                if (TimeList.Count > 0)
                    if (Math.Abs(item.time - TimeList[TimeList.Count - 1]) < Convert.ToSingle(parameters["interval"]))
                        continue;

                TimeList.Add(item.time);

                try
                {
                    VdrVbw vdrVbw = null;
                    if (options.Contains("history_vbw_watspd") || options.Contains("history_vbw_grdspd") || options.Contains("history_power_slip"))
                        vdrVbw = new VdrVbw((tempData.FirstOrDefault(t => t.category == "vbw" && t.time == item.time))?.data);
                    NotVdrEntity notVdrEntity = null;
                    if (options.Contains("history_flowmeter_me_fcperpow") || options.Contains("history_power_rpm") || options.Contains("history_power_power") || options.Contains("history_flowmeter_fcpernm") || options.Contains("history_power_slip") || options.Contains("history_draft_heel") || options.Contains("history_draft_trim"))
                        notVdrEntity = new NotVdrEntity((tempData.FirstOrDefault(t => t.category == "notvdr" && t.time == item.time)).data, vdrId, StaticVoyageData.DictFilter);
                    VdrMwd vdrMwd = null;
                    if (options.Contains("history_mwd_speed"))
                        vdrMwd = new VdrMwd((tempData.FirstOrDefault(t => t.category == "mwd" && t.time == item.time))?.data);
                    VdrDpt vdrDpt = null;
                    vdrDpt = new VdrDpt((tempData.FirstOrDefault(t => t.category == "dpt" && t.time == item.time))?.data);
                    VdrGns vdrGns = null;
                    vdrGns = new VdrGns((tempData.FirstOrDefault(t => t.category == "gns" && t.time == item.time))?.data);
                    VdrRpm vdrRpm = null;
                    vdrRpm = new VdrRpm((tempData.FirstOrDefault(t => t.category == "rpm" && t.time == item.time))?.data);
                    VdrVtg vdrVtg = null;
                    vdrVtg = new VdrVtg((tempData.FirstOrDefault(t => t.category == "vtg" && t.time == item.time))?.data);
                    VdrVlw vdrVlw = null;
                    vdrVlw = new VdrVlw((tempData.FirstOrDefault(t => t.category == "vlw" && t.time == item.time))?.data);
                    float? slip = null;
                    if (options.Contains("history_power_slip"))
                        slip = (1 - (vdrVbw.watspd / (notVdrEntity.MERpm * 5128.77f) * 1852f * 1000f / 60f)) * 100f;
                    float? draft = null;
                    if (options.Contains("history_draft_draft"))
                        draft = (notVdrEntity.DraftBow + notVdrEntity.DraftAstern) / 2f;

                    foreach (var strType in options)
                    {
                        switch (strType)
                        {
                            case "history_flowmeter_me_fcperpow":
                                resultDict[strType].Add((notVdrEntity.MEFC / notVdrEntity.MEPower).ToString());
                                break;

                            case "history_power_rpm":
                                resultDict[strType].Add((notVdrEntity.MERpm).ToString());
                                break;

                            case "history_power_power":
                                resultDict[strType].Add((notVdrEntity.MEPower).ToString());
                                break;

                            case "history_flowmeter_fcpernm":
                                resultDict[strType].Add(((notVdrEntity.MEFC + notVdrEntity.DGInFC - notVdrEntity.DGOutFC + notVdrEntity.DGMgoFC + notVdrEntity.BlrFC + notVdrEntity.BlrMGOFC) / vdrVbw.watspd).ToString());
                                break;

                            case "history_power_slip":
                                resultDict[strType].Add((slip).ToString());
                                break;

                            case "history_vbw_watspd":
                                resultDict[strType].Add((vdrVbw.watspd).ToString());
                                break;

                            case "history_vbw_grdspd":
                                resultDict[strType].Add((vdrVbw.grdspd).ToString());
                                break;

                            case "history_draft_heel":
                                resultDict[strType].Add((notVdrEntity.DraftPort - notVdrEntity.DraftStarboard).ToString());
                                break;

                            case "history_draft_trim":
                                resultDict[strType].Add((notVdrEntity.DraftAstern - notVdrEntity.DraftBow).ToString());
                                break;

                            case "history_mwd_speed":
                                resultDict[strType].Add((vdrMwd.speed).ToString());
                                break;
                        }
                    }
                    resultDict["history_currenttime"].Add(DateTimeOffset.FromUnixTimeSeconds(item.time).LocalDateTime.ToString("yyyy-MM-dd HH:mm:ss"));
                }
                catch (Exception ex)
                {
                    continue;
                }
            }
            return resultDict;
        }

        public async Task<object> GetVoyageHistoryMapAsync(string vdrId, int pageNum, int pageCount, string sorting, string asc, long dateFrom, long dateTo, string strParams)
        {
            var tempData = await _sentenceService.GetPageListAsync(vdrId, pageNum, pageCount, sorting, asc, dateFrom, dateTo);
            var TimeList = new List<long>();
            var lineList = new List<List<string>>();
            float? meAcc = 0f;
            float? dgAcc = 0f;
            float? blrAcc = 0f;
            float? lastMEFC = 0f;
            foreach (var item in (tempData ?? new List<SentenceDto>()))
            {
                if (TimeList.Contains(item.time))
                    continue;

                TimeList.Add(item.time);

                try
                {
                    /*VdrMwd vdrMwd = new VdrMwd((tempData.FirstOrDefault(t => t.category == "mwd" && t.time == item.time))?.data);
                    VdrDpt vdrDpt = null;
                    vdrDpt = new VdrDpt((tempData.FirstOrDefault(t => t.category == "dpt" && t.time == item.time))?.data);*/
                    VdrVbw vdrVbw = new VdrVbw((tempData.FirstOrDefault(t => t.category == "vbw" && t.time == item.time))?.data);
                    NotVdrEntity notVdrEntity = new NotVdrEntity((tempData.FirstOrDefault(t => t.category == "notvdr" && t.time == item.time)).data, vdrId, StaticVoyageData.DictFilter);
                    VdrGns vdrGns = new VdrGns((tempData.FirstOrDefault(t => t.category == "gns" && t.time == item.time))?.data);
                    /*VdrRpm vdrRpm = null;
                    vdrRpm = new VdrRpm((tempData.FirstOrDefault(t => t.category == "rpm" && t.time == item.time))?.data);
                    VdrVtg vdrVtg = null;
                    vdrVtg = new VdrVtg((tempData.FirstOrDefault(t => t.category == "vtg" && t.time == item.time))?.data);
                    VdrVlw vdrVlw = null;
                    vdrVlw = new VdrVlw((tempData.FirstOrDefault(t => t.category == "vlw" && t.time == item.time))?.data);
                    float? slip = (1 - (vdrVbw.watspd / (notVdrEntity.MERpm * 5128.77f) * 1852f * 1000f / 60f)) * 100f;
                    float? draft = (notVdrEntity.DraftBow + notVdrEntity.DraftAstern) / 2f;*/

                    var coor = GPSHelper.GetBdFrom84(new pointLatLon(Convert.ToDouble(vdrGns.latitude), Convert.ToDouble(vdrGns.longtitude)));
                    if (lineList.Count == 0)
                    {
                        lineList.Add(new List<string> { (notVdrEntity.MEFC / vdrVbw.watspd).ToString() });
                        lineList[lineList.Count - 1].Add(coor.Lat + "," + coor.Lon);
                    }
                    else
                    {
                        if (notVdrEntity.MEFC / vdrVbw.watspd <= 100 && lastMEFC <= 100 || notVdrEntity.MEFC / vdrVbw.watspd > 200 && lastMEFC > 200 || notVdrEntity.MEFC / vdrVbw.watspd > 100 && notVdrEntity.MEFC / vdrVbw.watspd <= 200 && lastMEFC > 100 && lastMEFC <= 200)
                        {
                            lineList[lineList.Count - 1].Add(coor.Lat + "," + coor.Lon);
                        }
                        else
                        {
                            var temp = lineList[lineList.Count - 1][(lineList[lineList.Count - 1]).Count - 1];
                            lineList.Add(new List<string> { (notVdrEntity.MEFC / vdrVbw.watspd).ToString() });
                            lineList[lineList.Count - 1].Add(temp);
                            lineList[lineList.Count - 1].Add(coor.Lat + "," + coor.Lon);
                        }
                    }
                    lastMEFC = notVdrEntity.MEFC / vdrVbw.watspd;
                    meAcc += notVdrEntity.MEFC;
                    dgAcc += notVdrEntity.DGInFC - notVdrEntity.DGOutFC;
                    blrAcc += notVdrEntity.BlrFC;
                }
                catch (Exception ex)
                {
                    continue;
                }
            }
            var result = new
            {
                lineList = lineList,
                meAcc = meAcc,
                dgAcc = dgAcc,
                blrAcc = blrAcc
            };
            return result;
        }

        public async Task<int> GetVoyageHistoryCountAsync(string vdrId, long dateFrom, long dateTo)
        {
            var resultCount = await _sentenceService.GetResultCountAsync(vdrId, dateFrom, dateTo);
            return resultCount;
        }

        public async Task<IEnumerable<VdrEntityDto>> GetVoyageAnalyseAsync(string vdrId, long? dateFrom, long? dateTo, float? slipFrom, float? slipTo, float? draftFrom, float? draftTo, string analyseType, float propPitch)
        {
            try
            {
                var sentenceEntities = (await _sentenceService.GetListByDateVdrAsync(vdrId, dateFrom, dateTo));
                IList<VdrTotalEntity> resultEntities = new List<VdrTotalEntity>();
                var lstCurrentDatetime = new List<long>();
                foreach (var entity in sentenceEntities)
                {
                    if (lstCurrentDatetime.Contains(entity.time))
                        continue;
                    else
                    {
                        try
                        {
                            lstCurrentDatetime.Add(entity.time);
                            var SameTimeSet = sentenceEntities.Where(t => t.time == entity.time);
                            VdrVbw vdrVbw = new VdrVbw((SameTimeSet.FirstOrDefault(t => t.category == "vbw"))?.data);

                            NotVdrEntity notVdrEntity = new NotVdrEntity((SameTimeSet.FirstOrDefault(t => t.category == "notvdr")).data, vdrId, StaticVoyageData.DictFilter);

                            var slip = (1 - (vdrVbw.watspd / (notVdrEntity.MERpm * propPitch) * 1852f * 1000f / 60f)) * 100f;
                            if (slip < slipFrom || slip >= slipTo || slip == null)
                                continue;
                            var draft = (notVdrEntity.DraftBow + notVdrEntity.DraftAstern) / 2f;
                            if (draft < draftFrom || draft >= draftTo || draft == null)
                                continue;

                            VdrDpt vdrDpt = new VdrDpt((SameTimeSet.FirstOrDefault(t => t.category == "dpt"))?.data);
                            VdrGns vdrGns = new VdrGns((SameTimeSet.FirstOrDefault(t => t.category == "gns"))?.data);
                            VdrMwd vdrMwd = new VdrMwd((SameTimeSet.FirstOrDefault(t => t.category == "mwd"))?.data);
                            VdrRpm vdrRpm = new VdrRpm((SameTimeSet.FirstOrDefault(t => t.category == "rpm"))?.data);
                            VdrVtg vdrVtg = new VdrVtg((SameTimeSet.FirstOrDefault(t => t.category == "vtg"))?.data);
                            VdrVlw vdrVlw = new VdrVlw((SameTimeSet.FirstOrDefault(t => t.category == "vlw"))?.data);

                            resultEntities.Add(new VdrTotalEntity
                            {
                                time = DateTimeOffset.FromUnixTimeSeconds(entity.time).LocalDateTime,
                                history_dpt_depth = vdrDpt.depth,
                                history_dpt_offset = vdrDpt.offset,
                                history_gns_longtitude = vdrGns.longtitude,
                                history_gns_latitude = vdrGns.latitude,
                                history_gns_satnum = vdrGns.satnum,
                                history_gns_antennaaltitude = vdrGns.antennaaltitude,
                                history_mwd_tdirection = vdrMwd.tdirection,
                                history_mwd_magdirection = vdrMwd.magdirection,
                                history_mwd_knspeed = vdrMwd.knspeed,
                                history_mwd_speed = vdrMwd.speed,
                                history_rpm_source = vdrRpm.source,
                                history_rpm_number = vdrRpm.number,
                                history_rpm_speed = vdrRpm.speed,
                                history_rpm_propellerpitch = vdrRpm.propellerpitch,
                                history_vbw_watspd = vdrVbw.watspd,
                                history_vbw_grdspd = vdrVbw.grdspd,
                                history_vtg_grdcoztrue = vdrVtg.grdcoztrue,
                                history_vtg_grdcozmag = vdrVtg.grdcozmag,
                                history_vtg_grdspdknot = vdrVtg.grdspdknot,
                                history_vtg_grdspdkm = vdrVtg.grdspdkm,
                                history_vlw_watdistotal = vdrVlw.watdistotal,
                                history_vlw_watdisreset = vdrVlw.watdisreset,
                                history_vlw_grddistotal = vdrVlw.grddistotal,
                                history_vlw_grddisreset = vdrVlw.grddisreset,
                                history_draft_trim = notVdrEntity.DraftAstern - notVdrEntity.DraftBow,
                                history_draft_heel = notVdrEntity.DraftPort - notVdrEntity.DraftStarboard,
                                history_draft_draft = draft,
                                history_power_rpm = notVdrEntity.MERpm,
                                history_power_power = notVdrEntity.MEPower,
                                history_power_slip = slip,
                                history_flowmeter_me_fcpernm = notVdrEntity.MEFC / vdrVbw.watspd,
                                history_flowmeter_me_fcperpow = notVdrEntity.MEFC / notVdrEntity.MEPower,
                                history_flowmeter_fcpernm = (notVdrEntity.MEFC + notVdrEntity.DGInFC - notVdrEntity.DGOutFC + notVdrEntity.DGMgoFC + notVdrEntity.BlrFC + notVdrEntity.BlrMGOFC) / vdrVbw.watspd,
                                history_flowmeter_fcperpow = (notVdrEntity.MEFC + notVdrEntity.DGInFC - notVdrEntity.DGOutFC + notVdrEntity.DGMgoFC + notVdrEntity.BlrFC + notVdrEntity.BlrMGOFC) / (notVdrEntity.MEPower + notVdrEntity.DG1Power + notVdrEntity.DG2Power + notVdrEntity.DG3Power)
                            });
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Namespace + "_" + MethodBase.GetCurrentMethod().Name);
                            continue;
                        }
                    }
                }

                return ObjectMapper.Map<IEnumerable<VdrTotalEntity>, IEnumerable<VdrEntityDto>>(resultEntities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Namespace + "_" + MethodBase.GetCurrentMethod().Name);
                return null;
            }
        }

        public async Task<string> RequestVoyageAnalyseAsync(string vdrId, long? dateFrom, long? dateTo, float? slipFrom, float? slipTo, float? draftFrom, float? draftTo, float? windSpdFrom, float? windSpdTo, float? speedFrom, float? speedTo, float? mefcFrom, float? mefcTo, float? powerFrom, float? powerTo, float? windDirFrom, float? windDirTo, float? rpmFrom, float? rpmTo, string analyseType, float propPitch)
        {
            try
            {
                var sentenceEntities = await _sentenceService.GetListByDateVdrAsync(vdrId, dateFrom, dateTo);
                if (sentenceEntities.Count() > 0)
                {
                    string dictKey = Guid.NewGuid().ToString();
                    Task.Factory.StartNew(async () =>
                    {
                        await PushRingBuffer(dictKey, sentenceEntities, vdrId, slipFrom, slipTo, draftFrom, draftTo, windSpdFrom, windSpdTo, speedFrom, speedTo, mefcFrom, mefcTo, powerFrom, powerTo, windDirFrom, windDirTo, rpmFrom, rpmTo, analyseType, propPitch);
                    });
                    return dictKey;
                }
                else
                {
                    return "failure : 没有满足该条件的数据。";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Namespace + "_" + MethodBase.GetCurrentMethod().Name);
                return "failure : " + ex.Message;
            }
        }

        public async Task PushRingBuffer(string dictKey, IEnumerable<SentenceDto> sentenceEntities, string vdrId, float? slipFrom, float? slipTo, float? draftFrom, float? draftTo, float? windSpdFrom, float? windSpdTo, float? speedFrom, float? speedTo, float? mefcFrom, float? mefcTo, float? powerFrom, float? powerTo, float? windDirFrom, float? windDirTo, float? rpmFrom, float? rpmTo, string analyseType, float propPitch)
        {
            int RingBufferSize = 1024 * 1024;
            var analyseRingBuffer = new ByteRingBuffer(RingBufferSize);
            AnalyseStaticData.AnalyseRingBuffers.Add(dictKey, analyseRingBuffer);

            var lstCurrentDatetime = new List<long>();
            Stopwatch sw = Stopwatch.StartNew();
            foreach (var entity in sentenceEntities)
            {
                if (lstCurrentDatetime.Contains(entity.time))
                    continue;
                else
                {
                    lstCurrentDatetime.Add(entity.time);
                    var InSameTimeSet = sentenceEntities.Where(t => t.time == entity.time);
                    string strResult = string.Empty;
                    if (analyseType == "winddirdist")
                    {
                        VdrMwd vdrMwd = new VdrMwd((InSameTimeSet.FirstOrDefault(t => t.category == "mwd"))?.data);
                        var result = new VdrTotalEntity
                        {
                            history_mwd_tdirection = Convert.ToInt32(vdrMwd.tdirection)
                        };
                        strResult = result.ToJson();
                    }
                    else if (analyseType == "speeddist")
                    {
                        VdrVbw vdrVbw = new VdrVbw((InSameTimeSet.FirstOrDefault(t => t.category == "vbw"))?.data);
                        var speed = vdrVbw.watspd;
                        if (speed < speedFrom || speed >= speedTo)
                            continue;
                        var result = new VdrTotalEntity
                        {
                            history_vbw_watspd = Convert.ToInt32(vdrVbw.watspd)
                        };
                        strResult = result.ToJson();
                    }
                    else if (analyseType == "winddirdist")
                    {
                        VdrMwd vdrMwd = new VdrMwd((InSameTimeSet.FirstOrDefault(t => t.category == "mwd"))?.data);
                        var result = new VdrTotalEntity
                        {
                            history_mwd_knspeed = Convert.ToInt32(vdrMwd.knspeed)
                        };
                        strResult = result.ToJson();
                    }
                    else
                    {
                        #region 分析类数据处理

                        VdrVbw vdrVbw = new VdrVbw((InSameTimeSet.FirstOrDefault(t => t.category == "vbw"))?.data);
                        NotVdrEntity notVdrEntity = new NotVdrEntity((InSameTimeSet.FirstOrDefault(t => t.category == "notvdr")).data, vdrId, StaticVoyageData.DictFilter);
                        VdrMwd vdrMwd = new VdrMwd((InSameTimeSet.FirstOrDefault(t => t.category == "mwd"))?.data);

                        float? slip = null;
                        float? draft = null;
                        float windSpd = 0;
                        float speed = 0;
                        float? mefc = 0;
                        float? power = 0;
                        float windDir = 0;
                        float? rpm = 0;
                        switch (analyseType)
                        {
                            case "speed":
                            case "vfcmespd":
                            case "powspd":
                                slip = (1 - (vdrVbw.watspd / (notVdrEntity.MERpm * propPitch) * 1852f * 1000f / 60f)) * 100f;
                                if (slip < slipFrom || slip >= slipTo || slip == null)
                                    continue;
                                draft = (notVdrEntity.DraftBow + notVdrEntity.DraftAstern) / 2f;
                                if (draft < draftFrom || draft >= draftTo || draft == null)
                                    continue;
                                break;

                            case "vfcspd":
                            case "powrpm":
                                slip = (1 - (vdrVbw.watspd / (notVdrEntity.MERpm * propPitch) * 1852f * 1000f / 60f)) * 100f;
                                if (slip < slipFrom || slip >= slipTo || slip == null)
                                    continue;
                                draft = (notVdrEntity.DraftBow + notVdrEntity.DraftAstern) / 2f;
                                if (draft < draftFrom || draft >= draftTo || draft == null)
                                    continue;
                                windSpd = vdrMwd.knspeed;
                                if (windSpd < windSpdFrom || windSpd >= windSpdTo)
                                    continue;
                                break;

                            case "mefcpow":
                                slip = (1 - (vdrVbw.watspd / (notVdrEntity.MERpm * propPitch) * 1852f * 1000f / 60f)) * 100f;
                                if (slip < slipFrom || slip >= slipTo || slip == null)
                                    continue;
                                draft = (notVdrEntity.DraftBow + notVdrEntity.DraftAstern) / 2f;
                                if (draft < draftFrom || draft >= draftTo || draft == null)
                                    continue;
                                break;

                            case "trim":
                                slip = (1 - (vdrVbw.watspd / (notVdrEntity.MERpm * propPitch) * 1852f * 1000f / 60f)) * 100f;
                                if (slip < slipFrom || slip >= slipTo || slip == null)
                                    continue;
                                draft = (notVdrEntity.DraftBow + notVdrEntity.DraftAstern) / 2f;
                                if (draft < draftFrom || draft >= draftTo || draft == null)
                                    continue;
                                speed = vdrVbw.watspd;
                                if (speed < speedFrom || speed >= speedTo)
                                    continue;
                                break;

                            case "mespdprop":
                                mefc = notVdrEntity.MEFC;
                                if (mefc < mefcFrom || mefc >= mefcTo || mefc == null)
                                    continue;
                                windSpd = vdrMwd.knspeed;
                                if (windSpd < windSpdFrom || windSpd >= windSpdTo)
                                    continue;
                                draft = (notVdrEntity.DraftBow + notVdrEntity.DraftAstern) / 2f;
                                if (draft < draftFrom || draft >= draftTo || draft == null)
                                    continue;
                                break;

                            case "hull":
                                power = notVdrEntity.MEPower;
                                if (power < powerFrom || power >= powerTo || power == null)
                                    continue;
                                slip = (1 - (vdrVbw.watspd / (notVdrEntity.MERpm * propPitch) * 1852f * 1000f / 60f)) * 100f;
                                if (slip < slipFrom || slip >= slipTo || slip == null)
                                    continue;
                                break;

                            case "hullpropeller":
                                speed = vdrVbw.watspd;
                                if (speed < speedFrom || speed >= speedTo)
                                    continue;
                                windSpd = vdrMwd.knspeed;
                                if (windSpd < windSpdFrom || windSpd >= windSpdTo)
                                    continue;
                                windDir = vdrMwd.tdirection;
                                if (windDir < windDirFrom || windDir >= windDirTo)
                                    continue;
                                slip = (1 - (vdrVbw.watspd / (notVdrEntity.MERpm * propPitch) * 1852f * 1000f / 60f)) * 100f;
                                if (slip < slipFrom || slip >= slipTo || slip == null)
                                    continue;
                                break;

                            case "meloadprop":
                                rpm = notVdrEntity.MERpm;
                                if (rpm < rpmFrom || rpm >= rpmTo || rpm == null)
                                    continue;
                                windSpd = vdrMwd.knspeed;
                                if (windSpd < windSpdFrom || windSpd >= windSpdTo)
                                    continue;
                                draft = (notVdrEntity.DraftBow + notVdrEntity.DraftAstern) / 2f;
                                if (draft < draftFrom || draft >= draftTo || draft == null)
                                    continue;
                                break;

                            case "powerdist":
                                power = notVdrEntity.MEPower;
                                if (power < powerFrom || power >= powerTo || draft == null)
                                    continue;
                                break;

                            case "draftdist":
                                draft = (notVdrEntity.DraftBow + notVdrEntity.DraftAstern) / 2f;
                                if (draft < draftFrom || draft >= draftTo || draft == null)
                                    continue;
                                break;
                        }

                        VdrDpt vdrDpt = new VdrDpt((InSameTimeSet.FirstOrDefault(t => t.category == "dpt"))?.data);
                        VdrGns vdrGns = new VdrGns((InSameTimeSet.FirstOrDefault(t => t.category == "gns"))?.data);
                        VdrRpm vdrRpm = new VdrRpm((InSameTimeSet.FirstOrDefault(t => t.category == "rpm"))?.data);
                        VdrVtg vdrVtg = new VdrVtg((InSameTimeSet.FirstOrDefault(t => t.category == "vtg"))?.data);
                        VdrVlw vdrVlw = new VdrVlw((InSameTimeSet.FirstOrDefault(t => t.category == "vlw"))?.data);

                        var result = new VdrTotalEntity
                        {
                            time = (analyseType == "hull" || analyseType == "metuning" || analyseType == "hullpropeller") ? Convert.ToDateTime(DateTimeOffset.FromUnixTimeSeconds(entity.time).LocalDateTime.ToShortDateString()) : DateTimeOffset.FromUnixTimeSeconds(entity.time).LocalDateTime,
                            history_dpt_depth = vdrDpt.depth,
                            history_dpt_offset = vdrDpt.offset,
                            history_gns_longtitude = vdrGns.longtitude,
                            history_gns_latitude = vdrGns.latitude,
                            history_gns_satnum = vdrGns.satnum,
                            history_gns_antennaaltitude = vdrGns.antennaaltitude,
                            history_mwd_tdirection = vdrMwd.tdirection,
                            history_mwd_magdirection = vdrMwd.magdirection,
                            history_mwd_knspeed = vdrMwd.knspeed,
                            history_mwd_speed = vdrMwd.speed,
                            history_rpm_source = vdrRpm.source,
                            history_rpm_number = vdrRpm.number,
                            history_rpm_speed = vdrRpm.speed,
                            history_rpm_propellerpitch = vdrRpm.propellerpitch,
                            history_vbw_watspd = vdrVbw.watspd,
                            history_vbw_grdspd = vdrVbw.grdspd,
                            history_vtg_grdcoztrue = vdrVtg.grdcoztrue,
                            history_vtg_grdcozmag = vdrVtg.grdcozmag,
                            history_vtg_grdspdknot = vdrVtg.grdspdknot,
                            history_vtg_grdspdkm = vdrVtg.grdspdkm,
                            history_vlw_watdistotal = vdrVlw.watdistotal,
                            history_vlw_watdisreset = vdrVlw.watdisreset,
                            history_vlw_grddistotal = vdrVlw.grddistotal,
                            history_vlw_grddisreset = vdrVlw.grddisreset,
                            history_draft_trim = notVdrEntity.DraftAstern - notVdrEntity.DraftBow,
                            history_draft_heel = notVdrEntity.DraftPort - notVdrEntity.DraftStarboard,
                            history_draft_draft = draft,
                            history_power_rpm = notVdrEntity.MERpm,
                            history_power_power = notVdrEntity.MEPower,
                            history_power_slip = slip,
                            history_flowmeter_me_fcpernm = notVdrEntity.MEFC / vdrVbw.watspd,
                            history_flowmeter_me_fcperpow = notVdrEntity.MEFC / notVdrEntity.MEPower,
                            history_flowmeter_fcpernm = (notVdrEntity.MEFC + notVdrEntity.DGInFC - notVdrEntity.DGOutFC + notVdrEntity.DGMgoFC + notVdrEntity.BlrFC + notVdrEntity.BlrMGOFC) / vdrVbw.watspd,
                            history_flowmeter_fcperpow = (notVdrEntity.MEFC + notVdrEntity.DGInFC - notVdrEntity.DGOutFC + notVdrEntity.DGMgoFC + notVdrEntity.BlrFC + notVdrEntity.BlrMGOFC) / (notVdrEntity.MEPower + notVdrEntity.DG1Power + notVdrEntity.DG2Power + notVdrEntity.DG3Power),
                            history_dg_power = notVdrEntity.DG1Power + notVdrEntity.DG2Power + notVdrEntity.DG3Power
                        };
                        strResult = result.ToJson();

                        #endregion 分析类数据处理
                    }
                    var bResult = Encoding.UTF8.GetBytes(strResult);

                    /*sw.Start();
                    while (!analyseRingBuffer.Empty())
                    {
                        if (sw.ElapsedMilliseconds > 180000)
                        {
                            sw.Stop();
                            AnalyseStaticData.AnalyseRingBuffers.Remove(dictKey);
                            return;
                        }
                        await Task.Delay(100);
                    }
                    sw.Stop();*/
                    sw.Start();
                    while (true)
                    {
                        if (analyseRingBuffer.CanPush(bResult.Length))
                        {
                            foreach (var b in bResult)
                            {
                                analyseRingBuffer.Push(b);
                            }
                            break;
                        }
                        else
                        {
                            if (sw.ElapsedMilliseconds > 180000)
                            {
                                sw.Stop();
                                AnalyseStaticData.AnalyseRingBuffers.Remove(dictKey);
                                return;
                            }
                            await Task.Delay(10);
                        }
                    }
                    sw.Stop();
                }
            }

            #region 结束

            var endMsg = Encoding.UTF8.GetBytes("^end^");

            sw.Start();
            while (!analyseRingBuffer.Empty())
            {
                if (sw.ElapsedMilliseconds > 180000)
                {
                    sw.Stop();
                    AnalyseStaticData.AnalyseRingBuffers.Remove(dictKey);
                    return;
                }
                await Task.Delay(10);
            }
            sw.Stop();
            foreach (var msg in endMsg)
            {
                analyseRingBuffer.Push(msg);
            }

            #endregion 结束
        }

        public async Task<List<VdrEntityDto>> RequestVoyageDistributionAsync(string vdrId, long? dateFrom, long? dateTo, float? slipFrom, float? slipTo, float? draftFrom, float? draftTo, float? windSpdFrom, float? windSpdTo, float? speedFrom, float? speedTo, float? mefcFrom, float? mefcTo, float? powerFrom, float? powerTo, float? windDirFrom, float? windDirTo, float? rpmFrom, float? rpmTo, string analyseType, float propPitch)
        {
            try
            {
                var sentenceEntities = await _sentenceService.GetListByDateVdrAsync(vdrId, dateFrom, dateTo);
                if (sentenceEntities.Count() > 0)
                {
                    var lstCurrentDatetime = new List<long>();
                    var lstResult = new List<VdrEntityDto>();
                    foreach (var entity in sentenceEntities)
                    {
                        if (lstCurrentDatetime.Contains(entity.time))
                            continue;
                        else
                        {
                            try
                            {
                                lstCurrentDatetime.Add(entity.time);
                                if (analyseType == "speeddist")
                                {
                                    var vdrVbw = new VdrVbw((sentenceEntities.FirstOrDefault(t => t.time == entity.time && t.category == "vbw"))?.data);
                                    if (vdrVbw.watspd < speedFrom || vdrVbw.watspd >= speedTo)
                                        continue;
                                    var result = new VdrEntityDto
                                    {
                                        history_vbw_watspd = vdrVbw.watspd
                                    };
                                    lstResult.Add(result);
                                }
                                else if (analyseType == "draftdist")
                                {
                                    var notVdrEntity = new NotVdrEntity((sentenceEntities.FirstOrDefault(t => t.time == entity.time && t.category == "notvdr")).data, vdrId, StaticVoyageData.DictFilter);
                                    var draft = (notVdrEntity.DraftBow + notVdrEntity.DraftAstern) / 2f;
                                    if (draft < draftFrom || draft >= draftTo)
                                        continue;
                                    var result = new VdrEntityDto
                                    {
                                        history_draft_draft = draft
                                    };
                                    lstResult.Add(result);
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Namespace + "_" + MethodBase.GetCurrentMethod().Name + " json:" + entity.ToJson());
                                continue;
                            }
                        }
                    }
                    return lstResult;
                }
                else
                {
                    throw new Exception("failure : 没有满足该条件的数据。");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Namespace + "_" + MethodBase.GetCurrentMethod().Name);
                throw new Exception("failure : " + ex.Message);
            }
        }

        #region 上传数据解析保存

        public static string tempStrLeft = "";

        public uint receiveCount = 0;

        /// <summary>
        /// 数据解析-VDR
        /// </summary>
        /// <param name="strDataO">解析语句 正常一条数据为[strSymbol]id,datetime,61162标准语句</param>
        /// <param name="strSymbol">语句开始符</param>
        /// <param name="_sentenceService"></param>
        /// <param name="vdrId"></param>
        public async Task<int> DataAnalysisAsync(string strDataO, string strSymbol, string vdrId)
        {
            int resultObject = 0;

            List<string> listStrData = new List<string>();
            if (strDataO.IndexOf(strSymbol) == strDataO.LastIndexOf(strSymbol) && strDataO.IndexOf(strSymbol) == 0 && strDataO.LastIndexOf('*') == (strDataO.Length - 2 - 1))
            {
                listStrData = strDataO.Split(strSymbol).ToList();
                tempStrLeft = "";
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(tempStrLeft))
                {
                    strDataO = tempStrLeft + strDataO;
                    tempStrLeft = "";
                }
                string[] strDatas = strDataO.Split(strSymbol);
                tempStrLeft = strDatas[strDatas.Length - 1];
                if (tempStrLeft.LastIndexOf('*') == (tempStrLeft.Length - 2 - 1))
                {
                    listStrData = strDatas.ToList();
                    tempStrLeft = string.Empty;
                }
                else
                    listStrData = strDatas.SkipLast(1).ToList();
            }
            try
            {
                foreach (string strData in listStrData)
                {
                    if (string.IsNullOrWhiteSpace(strData))
                        continue;
                    var strInfos = strData.Split(',').ToList();
                    string TimeFlag = strInfos[2];
                    strInfos.RemoveAt(2);
                    strInfos.RemoveAt(1);
                    strInfos.RemoveAt(0);
                    var strDataNew = "";
                    foreach (string str in strInfos)
                    {
                        strDataNew += str + ",";
                    }
                    strDataNew = strDataNew.Trim(',');

                    //保存sentence
                    CreateSentenceDto sentenceDto = new CreateSentenceDto();
                    //sentenceDto.time = new DateTimeOffset(Convert.ToDateTime(TimeFlag).ToUniversalTime()).ToUnixTimeSeconds();
                    sentenceDto.time = Convert.ToInt64(TimeFlag);
                    sentenceDto.data = strDataNew;
                    sentenceDto.vdr_id = vdrId;
                    var vesselInfo = new VesselInfoDto();
                    vesselInfo.SN = vdrId;
                    vesselInfo.ReceiveDatetime = StaticEntity.RealtimeVesselinfos[vdrId].ReceiveDatetime = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(TimeFlag)).LocalDateTime;

                    if ((strDataNew.Substring(0, 7) == "$modbus") && strDataNew.Substring(strDataNew.Length - 3, 3) == "*00")
                    {
                        strDataNew = strDataNew.Substring(8, strDataNew.Length - 12);
                        string[] strDataNews = strDataNew.Split(',');
                        if (strDataNews.Length > 2)
                        {
                            DataProccess(vdrId, strDataNews[1], strDataNews[2]);
                        }
                    }
                    else if ((strDataNew.Substring(0, 2) == "$0" || strDataNew.Substring(0, 2) == "$1") && strDataNew.Substring(strDataNew.Length - 3, 3) == "*00")
                    {
                        strDataNew = strDataNew.Substring(0, strDataNew.Length - 3);
                        //byte[] bytesData = Encoding.ASCII.GetBytes(strDataNew);
                        _logger.LogInformation(strDataNew);
                        string[] strDataNews = strDataNew.Split(',');
                        if (strDataNews.Length > 1)
                        {
                            sentenceDto.data = strDataNews[1];
                            sentenceDto.category = "notvdr";
                            resultObject = await _sentenceService.CreateAsync(sentenceDto) == null ? 0 : 1;
                            byte[] bytesData = StringToBytes(strDataNews[1]);
                            DataAnalysisNotVDR(bytesData, vdrId, strDataNews[0]);
                            var tempEntity = new NotVdrEntity(sentenceDto.data, vdrId, StaticVoyageData.DictFilter);
                            vesselInfo.MEHFOConsumption = tempEntity.METemperature >= 60 ? tempEntity.MEFC : null;
                            vesselInfo.MEMDOConsumption = tempEntity.METemperature < 60 ? tempEntity.MEFC : null;
                            vesselInfo.DGHFOConsumption = tempEntity.DGInFC - tempEntity.DGOutFC;
                            vesselInfo.DGMDOConsumption = tempEntity.DGOutFC;
                            vesselInfo.BLRHFOConsumption = tempEntity.BlrFC;
                            vesselInfo.BLRMDOConsumption = tempEntity.BlrMGOFC;
                            vesselInfo.MEPower = tempEntity.MEPower;
                            vesselInfo.MERpm = tempEntity.MERpm;
                            vesselInfo.Torque = tempEntity.METorque;
                            vesselInfo.BowDraft = tempEntity.DraftBow;
                            vesselInfo.AsternDraft = tempEntity.DraftAstern;
                            vesselInfo.PortDraft = tempEntity.DraftPort;
                            vesselInfo.StarBoardDraft = tempEntity.DraftStarboard;
                            vesselInfo.DGPower = tempEntity.DG1Power + tempEntity.DG2Power + tempEntity.DG3Power;

                            SetRealtimeValue(vdrId, "MEHFOConsumption", vesselInfo.MEHFOConsumption ?? 0);
                            SetRealtimeValue(vdrId, "MEMDOConsumption", vesselInfo.MEMDOConsumption ?? 0);
                            SetRealtimeValue(vdrId, "DGHFOConsumption", vesselInfo.DGHFOConsumption ?? 0);
                            SetRealtimeValue(vdrId, "DGMDOConsumption", vesselInfo.DGMDOConsumption ?? 0);
                            SetRealtimeValue(vdrId, "BLRHFOConsumption", vesselInfo.BLRHFOConsumption ?? 0);
                            SetRealtimeValue(vdrId, "BLRMDOConsumption", vesselInfo.BLRMDOConsumption ?? 0);
                            SetRealtimeValue(vdrId, "MEPower", vesselInfo.MEPower ?? 0);
                            SetRealtimeValue(vdrId, "MERpm", vesselInfo.MERpm ?? 0);
                            SetRealtimeValue(vdrId, "Torque", vesselInfo.Torque ?? 0);
                            SetRealtimeValue(vdrId, "BowDraft", vesselInfo.BowDraft ?? 0);
                            SetRealtimeValue(vdrId, "AsternDraft", vesselInfo.AsternDraft ?? 0);
                            SetRealtimeValue(vdrId, "PortDraft", vesselInfo.PortDraft ?? 0);
                            SetRealtimeValue(vdrId, "StarBoardDraft", vesselInfo.StarBoardDraft ?? 0);
                            SetRealtimeValue(vdrId, "DGPower", vesselInfo.DGPower ?? 0);
                            SetRealtimeValue(vdrId, "Heel", StaticEntity.RealtimeVesselinfos[vdrId].PortDraft ?? 0 - StaticEntity.RealtimeVesselinfos[vdrId].StarBoardDraft ?? 0);
                            SetRealtimeValue(vdrId, "Trim", StaticEntity.RealtimeVesselinfos[vdrId].AsternDraft ?? 0 - StaticEntity.RealtimeVesselinfos[vdrId].BowDraft ?? 0);
                            SetRealtimeValue(vdrId, "Draft", (StaticEntity.RealtimeVesselinfos[vdrId].BowDraft ?? 0 + StaticEntity.RealtimeVesselinfos[vdrId].AsternDraft ?? 0) / 2);
                            SetRealtimeValue(vdrId, "MESFOC", ((StaticEntity.RealtimeVesselinfos[vdrId].MEHFOConsumption ?? 0 + StaticEntity.RealtimeVesselinfos[vdrId].MEMDOConsumption ?? 0) / StaticEntity.RealtimeVesselinfos[vdrId].MEPower ?? 0) * 1000f);
                            SetRealtimeValue(vdrId, "DGSFOC", ((StaticEntity.RealtimeVesselinfos[vdrId].DGHFOConsumption ?? 0 + StaticEntity.RealtimeVesselinfos[vdrId].DGMDOConsumption ?? 0) / StaticEntity.RealtimeVesselinfos[vdrId].DGPower ?? 0) * 1000f);
                            SetRealtimeValue(vdrId, "SFOC", ((StaticEntity.RealtimeVesselinfos[vdrId].MEHFOConsumption ?? 0 + StaticEntity.RealtimeVesselinfos[vdrId].MEMDOConsumption ?? 0 + StaticEntity.RealtimeVesselinfos[vdrId].DGHFOConsumption ?? 0 + StaticEntity.RealtimeVesselinfos[vdrId].DGMDOConsumption ?? 0 + StaticEntity.RealtimeVesselinfos[vdrId].BLRHFOConsumption ?? 0 + StaticEntity.RealtimeVesselinfos[vdrId].BLRMDOConsumption ?? 0) / (StaticEntity.RealtimeVesselinfos[vdrId].MEPower ?? 0 + StaticEntity.RealtimeVesselinfos[vdrId].DGPower ?? 0)) * 1000f);
                        }
                        else
                            resultObject = 0;
                    }
                    else if (StringHelper.CRCCheck(strDataNew.Trim('$'), 2))
                    {
                        strDataNew = strDataNew.Substring(0, strDataNew.Length - 3);
                        string strDevice = strInfos[0].Substring(strInfos[0].Length - 3, 3);
                        switch (strDevice)
                        {
                            case "MWV":
                                sentenceDto.category = "mwv";
                                //resultObject = await DataMWVAnalysis(strDataNew, sentenceDto);
                                break;

                            case "VBW":
                                sentenceDto.category = "vbw";
                                var vbmEntity = new VdrVbw(sentenceDto.data);
                                vesselInfo.WaterSpeed = vbmEntity.watspd;
                                vesselInfo.GroundSpeed = vbmEntity.grdspd;
                                SetRealtimeValue(vdrId, "WaterSpeed", vesselInfo.WaterSpeed ?? 0);
                                SetRealtimeValue(vdrId, "GroundSpeed", vesselInfo.GroundSpeed ?? 0);
                                SetRealtimeValue(vdrId, "Slip", ((1 - (StaticEntity.RealtimeVesselinfos[vdrId].WaterSpeed / (StaticEntity.RealtimeVesselinfos[vdrId].MERpm * 5128.77) * 1852 * 1000 / 60)) * 100) ?? 0);
                                SetRealtimeValue(vdrId, "MEHFOCPerNm", (StaticEntity.RealtimeVesselinfos[vdrId].MEHFOConsumption / StaticEntity.RealtimeVesselinfos[vdrId].WaterSpeed) ?? 0);
                                SetRealtimeValue(vdrId, "MEMDOCPerNm", (StaticEntity.RealtimeVesselinfos[vdrId].MEMDOConsumption / StaticEntity.RealtimeVesselinfos[vdrId].WaterSpeed) ?? 0);
                                SetRealtimeValue(vdrId, "MEFCPerNm", ((StaticEntity.RealtimeVesselinfos[vdrId].MEHFOConsumption ?? 0 + StaticEntity.RealtimeVesselinfos[vdrId].MEMDOConsumption ?? 0) / StaticEntity.RealtimeVesselinfos[vdrId].WaterSpeed) ?? 0);
                                SetRealtimeValue(vdrId, "DGHFOCPerNm", (StaticEntity.RealtimeVesselinfos[vdrId].DGHFOConsumption / StaticEntity.RealtimeVesselinfos[vdrId].WaterSpeed) ?? 0);
                                SetRealtimeValue(vdrId, "DGMDOCPerNm", (StaticEntity.RealtimeVesselinfos[vdrId].DGMDOConsumption / StaticEntity.RealtimeVesselinfos[vdrId].WaterSpeed) ?? 0);
                                SetRealtimeValue(vdrId, "DGFCPerNm", ((StaticEntity.RealtimeVesselinfos[vdrId].DGHFOConsumption + StaticEntity.RealtimeVesselinfos[vdrId].DGMDOConsumption) / StaticEntity.RealtimeVesselinfos[vdrId].WaterSpeed) ?? 0);
                                SetRealtimeValue(vdrId, "BLRHFOCPerNm", (StaticEntity.RealtimeVesselinfos[vdrId].BLRHFOConsumption / StaticEntity.RealtimeVesselinfos[vdrId].WaterSpeed) ?? 0);
                                SetRealtimeValue(vdrId, "BLRMDOCPerNm", (StaticEntity.RealtimeVesselinfos[vdrId].BLRMDOConsumption / StaticEntity.RealtimeVesselinfos[vdrId].WaterSpeed) ?? 0);
                                SetRealtimeValue(vdrId, "BLRFCPerNm", ((StaticEntity.RealtimeVesselinfos[vdrId].BLRHFOConsumption + StaticEntity.RealtimeVesselinfos[vdrId].BLRMDOConsumption) / StaticEntity.RealtimeVesselinfos[vdrId].WaterSpeed) ?? 0);
                                SetRealtimeValue(vdrId, "HFOCPerNm", ((StaticEntity.RealtimeVesselinfos[vdrId].MEHFOConsumption + StaticEntity.RealtimeVesselinfos[vdrId].DGHFOConsumption + StaticEntity.RealtimeVesselinfos[vdrId].BLRHFOConsumption) / StaticEntity.RealtimeVesselinfos[vdrId].WaterSpeed) ?? 0);
                                SetRealtimeValue(vdrId, "MDOCPerNm", ((StaticEntity.RealtimeVesselinfos[vdrId].MEMDOConsumption + StaticEntity.RealtimeVesselinfos[vdrId].DGMDOConsumption + StaticEntity.RealtimeVesselinfos[vdrId].BLRMDOConsumption) / StaticEntity.RealtimeVesselinfos[vdrId].WaterSpeed) ?? 0);
                                /*SetRealtimeValue(vdrId, "FCPerNm", ((StaticEntity.RealtimeVesselinfos[vdrId].MEHFOConsumption + StaticEntity.RealtimeVesselinfos[vdrId].DGHFOConsumption + StaticEntity.RealtimeVesselinfos[vdrId].BLRHFOConsumption + StaticEntity.RealtimeVesselinfos[vdrId].MEMDOConsumption + StaticEntity.RealtimeVesselinfos[vdrId].DGMDOConsumption + StaticEntity.RealtimeVesselinfos[vdrId].BLRMDOConsumption) / StaticEntity.RealtimeVesselinfos[vdrId].WaterSpeed) ?? 0);*/
                                SetRealtimeValue(vdrId, "FCPerNm", (StaticEntity.RealtimeVesselinfos[vdrId].MEHFOConsumption ?? 0 / StaticEntity.RealtimeVesselinfos[vdrId].WaterSpeed) ?? 0);
                                //resultObject = await DataVBWAnalysis(strDataNew, sentenceDto);
                                break;

                            case "MWD":
                                sentenceDto.category = "mwd";
                                var mwdEntity = new VdrMwd(sentenceDto.data);
                                vesselInfo.WindDirection = mwdEntity.tdirection;
                                vesselInfo.WindSpeed = mwdEntity.knspeed;
                                SetRealtimeValue(vdrId, "WindDirection", vesselInfo.WindDirection ?? 0);
                                SetRealtimeValue(vdrId, "WindSpeed", vesselInfo.WindSpeed ?? 0);
                                //resultObject = await DataMWDAnalysis(strDataNew, sentenceDto);
                                break;

                            case "GGA":
                                sentenceDto.category = "gga";
                                var ggaEntity = new VdrGga(sentenceDto.data);
                                var coor = GPSHelper.GetBdFrom84(new pointLatLon(Convert.ToDouble(ggaEntity.latitude), Convert.ToDouble(ggaEntity.longitude)));
                                vesselInfo.Latitude = coor.Lat;
                                vesselInfo.Longitude = coor.Lon;
                                SetRealtimeValue(vdrId, "Latitude", vesselInfo.Latitude ?? 0);
                                SetRealtimeValue(vdrId, "Longitude", vesselInfo.Longitude ?? 0);
                                //resultObject = await DataGGAAnalysis(strDataNew, sentenceDto);
                                break;

                            case "RMC":
                                sentenceDto.category = "rmc";
                                var rmcEntity = new VdrRmc(sentenceDto.data);
                                var coor1 = GPSHelper.GetBdFrom84(new pointLatLon(Convert.ToDouble(rmcEntity.latitude), Convert.ToDouble(rmcEntity.longtitude)));
                                vesselInfo.Latitude = coor1.Lat;
                                vesselInfo.Longitude = coor1.Lon;
                                vesselInfo.Course = Convert.ToDouble(rmcEntity.grdcoz);
                                vesselInfo.MagneticVariation = Convert.ToDouble(rmcEntity.magvar);
                                SetRealtimeValue(vdrId, "Latitude", vesselInfo.Latitude ?? 0);
                                SetRealtimeValue(vdrId, "Longitude", vesselInfo.Longitude ?? 0);
                                SetRealtimeValue(vdrId, "Course", vesselInfo.Course ?? 0);
                                SetRealtimeValue(vdrId, "MagneticVariation", vesselInfo.MagneticVariation ?? 0);
                                //resultObject = await DataRMCAnalysis(strDataNew, sentenceDto);
                                break;

                            case "RPM":
                                sentenceDto.category = "rpm";
                                //resultObject = await DataRPMAnalysis(strDataNew, sentenceDto);
                                break;

                            case "TRC":
                                sentenceDto.category = "trc";
                                //resultObject = await DataTRCAnalysis(strDataNew, sentenceDto);
                                break;

                            case "TRD":
                                sentenceDto.category = "trd";
                                //resultObject = await DataTRDAnalysis(strDataNew, sentenceDto);
                                break;

                            case "VTG":
                                sentenceDto.category = "vtg";
                                var vtgEntity = new VdrVtg(sentenceDto.data);
                                vesselInfo.Course = vtgEntity.grdcoztrue;
                                vesselInfo.MagneticVariation = vtgEntity.grdcozmag;
                                vesselInfo.GroundSpeed = vtgEntity.grdspdknot;
                                SetRealtimeValue(vdrId, "Course", vesselInfo.Course ?? 0);
                                SetRealtimeValue(vdrId, "MagneticVariation", vesselInfo.MagneticVariation ?? 0);
                                SetRealtimeValue(vdrId, "GroundSpeed", vesselInfo.GroundSpeed ?? 0);
                                //resultObject = await DataVTGAnalysis(strDataNew, sentenceDto);
                                break;

                            case "GNS":
                                sentenceDto.category = "gns";
                                var gnsEntity = new VdrGns(sentenceDto.data);
                                var coor2 = GPSHelper.GetBdFrom84(new pointLatLon(Convert.ToDouble(gnsEntity.latitude), Convert.ToDouble(gnsEntity.longtitude)));
                                vesselInfo.Latitude = coor2.Lat;
                                vesselInfo.Longitude = coor2.Lon;
                                SetRealtimeValue(vdrId, "Latitude", vesselInfo.Latitude ?? 0);
                                SetRealtimeValue(vdrId, "Longitude", vesselInfo.Longitude ?? 0);
                                //resultObject = await DataGNSAnalysis(strDataNew, sentenceDto);
                                break;

                            case "XDR":
                                sentenceDto.category = "xdr";
                                //resultObject = await DataXDRAnalysis(strDataNew, sentenceDto);
                                break;

                            case "PRC":
                                sentenceDto.category = "prc";
                                //resultObject = await DataPRCAnalysis(strDataNew, sentenceDto);
                                break;

                            case "VLW":
                                sentenceDto.category = "vlw";
                                var vlwEntity = new VdrVlw(sentenceDto.data);
                                vesselInfo.TotalDistanceGrd = vlwEntity.grddistotal;
                                vesselInfo.ResetDistanceGrd = vlwEntity.grddisreset;
                                vesselInfo.TotalDistanceWat = vlwEntity.watdistotal;
                                vesselInfo.ResetDistanceWat = vlwEntity.watdisreset;
                                SetRealtimeValue(vdrId, "TotalDistanceGrd", vesselInfo.TotalDistanceGrd ?? 0);
                                SetRealtimeValue(vdrId, "ResetDistanceGrd", vesselInfo.ResetDistanceGrd ?? 0);
                                SetRealtimeValue(vdrId, "TotalDistanceWat", vesselInfo.TotalDistanceWat ?? 0);
                                SetRealtimeValue(vdrId, "ResetDistanceWat", vesselInfo.ResetDistanceWat ?? 0);
                                //resultObject = await DataVLWAnalysis(strDataNew, sentenceDto);
                                break;

                            case "HDG":
                                sentenceDto.category = "hdg";
                                //resultObject = await DataHDGAnalysis(strDataNew, sentenceDto);
                                break;

                            case "DPT":
                                sentenceDto.category = "dpt";
                                var dptEntity = new VdrDpt(sentenceDto.data);
                                vesselInfo.Depth = dptEntity.depth;
                                vesselInfo.DepthOffset = dptEntity.offset;
                                SetRealtimeValue(vdrId, "Depth", vesselInfo.Depth ?? 0);
                                SetRealtimeValue(vdrId, "DepthOffset", vesselInfo.DepthOffset ?? 0);
                                //resultObject = await DataDPTAnalysis(strDataNew, sentenceDto);
                                break;

                            default:
                                //resultObject = 1;
                                break;
                        }
                        resultObject = await _sentenceService.CreateAsync(sentenceDto) == null ? 0 : 1;
                    }
                    else if (StringHelper.GetBCCXorCode(strDataNew))
                    {
                        var strSplit = strDataNew.Split(",");
                        switch (strSplit[0])
                        {
                            case "$PSTKTSX5":
                                sentenceDto.category = "pstktsx5";
                                var dptsx5 = new SX5(sentenceDto.data);
                                vesselInfo.MEPower = dptsx5.power;
                                vesselInfo.MERpm = dptsx5.rpm;
                                vesselInfo.Torque = dptsx5.torque;
                                vesselInfo.Thrust = dptsx5.thrust;
                                SetRealtimeValue(vdrId, "MEPower", vesselInfo.MEPower ?? 0);
                                SetRealtimeValue(vdrId, "MERpm", vesselInfo.MERpm ?? 0);
                                SetRealtimeValue(vdrId, "Torque", vesselInfo.Torque ?? 0);
                                SetRealtimeValue(vdrId, "Thrust", vesselInfo.Thrust ?? 0);
                                break;

                            case "$HDWSEED":
                                sentenceDto.category = "hdwseed";
                                var hdwseed = new HDWSEED(sentenceDto.data);
                                vesselInfo.MEHFOConsumption = hdwseed.Instantaneous;
                                vesselInfo.MEHFOCACC = hdwseed.Accumulated;
                                SetRealtimeValue(vdrId, "MEHFOConsumption", vesselInfo.MEHFOConsumption ?? 0);
                                SetRealtimeValue(vdrId, "MEHFOCACC", vesselInfo.MEHFOCACC ?? 0);
                                break;

                            default:
                                //resultObject = 1;
                                break;
                        }
                        resultObject = await _sentenceService.CreateAsync(sentenceDto) == null ? 0 : 1;
                    }
                    await _vesselInfoService.InsertOrUpdateAsync(vesselInfo);
                }
                return resultObject;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "解析数据失败,接收语句:" + strDataO);
                return resultObject;
            }
        }

        /// <summary>
        /// 数据解析-非VDR
        /// </summary>
        /// <param name="receiveData">获取到的数据</param>
        /// <param name="dataType">数据类型</param>
        public void DataAnalysisNotVDR(byte[] receiveData, string vdrId, string dataType = "$0")
        {
            if (StringHelper.CRC_Check(receiveData))
            {
                if (dataType == "$0")
                {
                    if (receiveData[receiveData.Length - 3].ToString("D") == "0")
                    {
                        /*NaviParam naviParam = new NaviParam();
                        naviParam.DisplayName_cn = "";
                        naviParam.DisplayName_en = "";
                        naviParam.DisplayValue = Convert.ToInt32((receiveData[3] * 100 + receiveData[4]).ToString(), 16).ToString();*/
                    }
                }
                else if (dataType == "$1")
                {
                    /*NaviParam naviParam = new NaviParam();
                    naviParam.DisplayName_cn = "";
                    naviParam.DisplayName_en = "";
                    naviParam.DisplayValue = Convert.ToInt32(receiveData[3].ToString("X2") + receiveData[4].ToString("X2") + receiveData[5].ToString("X2") + receiveData[6].ToString("X2"), 16).ToString();
                    naviParam.DisplayValue = Convert.ToInt32(receiveData[7].ToString("X2") + receiveData[8].ToString("X2") + receiveData[9].ToString("X2") + receiveData[10].ToString("X2"), 16).ToString();*/
                }
            }
            else
            {
                int CanCount = receiveData[0];
                receiveData = receiveData.Skip(1).ToArray();
                for (int i = 0; i < CanCount; i++)
                {
                    //单个报文长度
                    int CanLength = 1;
                    string ZID = "";
                    int ZTypeCount = 1;
                    //帧类型
                    string ZType = (receiveData[0] & 0x80) != 0 ? "扩展帧" : "标准帧";
                    //帧格式
                    string ZFormat = (receiveData[0] & 0x40) != 0 ? "远程帧" : "数据帧";
                    //数据长度
                    int DataLength = (receiveData[0] & 0xf);
                    //CAN来源 0:CAN1 1:CAN2
                    string CanSource = (receiveData[0] & 0x10) == 0 ? "CAN1" : "CAN2";

                    CanLength += DataLength;
                    switch (ZType)
                    {
                        case "标准帧":
                            ZTypeCount = 2;
                            string s_str1 = receiveData[1].ToString("X2");
                            string s_str2 = receiveData[2].ToString("X2");
                            ZID = s_str1 + s_str2;
                            CanLength = CanLength + 2;
                            break;

                        case "扩展帧":
                            ZTypeCount = 4;
                            string e_str1 = receiveData[1].ToString("X2");
                            string e_str2 = receiveData[2].ToString("X2");
                            string e_str3 = receiveData[3].ToString("X2");
                            string e_str4 = receiveData[4].ToString("X2");
                            ZID = e_str1 + e_str2 + e_str3 + e_str4;
                            CanLength = CanLength + 4;
                            break;

                        default:
                            break;
                    }

                    var lstData = new List<string>();
                    for (int j = 0; j < DataLength; j++)
                    {
                        lstData.Add(receiveData[ZTypeCount + 1 + j].ToString("X2"));
                    }
                    switch (CanSource)
                    {
                        case "CAN1":
                            uint count = receiveCount;
                            string _zid = ZID;
                            string _zformat = ZFormat;
                            string _ztype = ZType;
                            int _dataLength = DataLength;

                            CanGetAI(_zid, lstData.ToArray(), vdrId);
                            break;

                        default:

                            break;
                    }
                    receiveData = receiveData.Skip(CanLength).Take(receiveData.Length - CanLength).ToArray();//去掉这个报文的内容
                }
            }
        }

        /// <summary>
        /// 数据解析
        /// </summary>
        /// <param name="number">采集系统id</param>
        /// <param name="deviceCode">设备代码</param>
        /// <param name="sentence">传输语句</param>
        public void DataProccess(string number, string deviceCode, string sentence)
        {
            CreateDics(number);

            var lstFm = StaticEntity.RealtimeFlowmeters[number];
            var lstBa = StaticEntity.RealtimeBatteries[number];
            var lstGe = StaticEntity.RealtimeGenerators[number];
            var lstSu = StaticEntity.RealtimeSupplyUnits[number];
            var lstSs = StaticEntity.RealtimeSternSealings[number];
            var lstSh = StaticEntity.RealtimeShafts[number];
            var lstLl = StaticEntity.RealtimeLiquidLevels[number];
            switch (deviceCode)
            {
                case "mefuel1_in":
                    if (lstFm.Any(t => t.Number == number && t.DeviceNo == "mefuel1" && t.DeviceType == "me"))
                    {
                        var fm = lstFm.First(t => t.Number == number && t.DeviceNo == "mefuel1" && t.DeviceType == "me");
                        fm.ConsAct = fm.ConsAct + Convert.ToDecimal(DecodeProtocalData(sentence)[0]);
                        fm.ConsAcc = fm.ConsAcc + Convert.ToDecimal(DecodeProtocalData(sentence)[1]);
                        fm.Temperature = Convert.ToDecimal(DecodeProtocalData(sentence)[2]);
                        fm.Density = Convert.ToDecimal(DecodeProtocalData(sentence)[3]);
                        fm.DeviceType = "me";
                        fm.FuelType = "HFO";
                        fm.Number = number;
                        fm.ReceiveDatetime = StaticEntity.RealtimeVesselinfos[number].ReceiveDatetime;
                        fm.DeviceNo = "mefuel1";
                    }
                    else
                    {
                        lstFm.Add(new Flowmeter
                        {
                            ConsAct = Convert.ToDecimal(DecodeProtocalData(sentence)[0]),
                            ConsAcc = Convert.ToDecimal(DecodeProtocalData(sentence)[1]),
                            Temperature = Convert.ToDecimal(DecodeProtocalData(sentence)[2]),
                            Density = Convert.ToDecimal(DecodeProtocalData(sentence)[3]),
                            DeviceType = "me",
                            FuelType = "HFO",
                            Number = number,
                            ReceiveDatetime = StaticEntity.RealtimeVesselinfos[number].ReceiveDatetime,
                            DeviceNo = "mefuel1"
                        });
                    }
                    break;

                case "mefuel1_out":
                    if (lstFm.Any(t => t.Number == number && t.DeviceNo == "mefuel1" && t.DeviceType == "me"))
                    {
                        var fm = lstFm.First(t => t.Number == number && t.DeviceNo == "mefuel1" && t.DeviceType == "me");
                        fm.ConsAct = fm.ConsAct - Convert.ToDecimal(DecodeProtocalData(sentence)[0]);
                        fm.ConsAcc = fm.ConsAcc - Convert.ToDecimal(DecodeProtocalData(sentence)[1]);
                    }
                    else
                    {
                        lstFm.Add(new Flowmeter
                        {
                            ConsAct = -Convert.ToDecimal(DecodeProtocalData(sentence)[0]),
                            ConsAcc = -Convert.ToDecimal(DecodeProtocalData(sentence)[1]),
                            Number = number,
                            DeviceNo = "mefuel1",
                            DeviceType = "me"
                        });
                    }
                    break;

                case "mefuel2_in":
                    if (lstFm.Any(t => t.Number == number && t.DeviceNo == "mefuel2" && t.DeviceType == "me"))
                    {
                        var fm = lstFm.First(t => t.Number == number && t.DeviceNo == "mefuel2" && t.DeviceType == "me");
                        fm.ConsAct = fm.ConsAct + Convert.ToDecimal(DecodeProtocalData(sentence)[0]);
                        fm.ConsAcc = fm.ConsAcc + Convert.ToDecimal(DecodeProtocalData(sentence)[1]);
                        fm.Temperature = Convert.ToDecimal(DecodeProtocalData(sentence)[2]);
                        fm.Density = Convert.ToDecimal(DecodeProtocalData(sentence)[3]);
                        fm.DeviceType = "me";
                        fm.FuelType = "HFO";
                        fm.Number = number;
                        fm.ReceiveDatetime = StaticEntity.RealtimeVesselinfos[number].ReceiveDatetime;
                        fm.DeviceNo = "mefuel2";
                    }
                    else
                    {
                        lstFm.Add(new Flowmeter
                        {
                            ConsAct = Convert.ToDecimal(DecodeProtocalData(sentence)[0]),
                            ConsAcc = Convert.ToDecimal(DecodeProtocalData(sentence)[1]),
                            Temperature = Convert.ToDecimal(DecodeProtocalData(sentence)[2]),
                            Density = Convert.ToDecimal(DecodeProtocalData(sentence)[3]),
                            DeviceType = "me",
                            FuelType = "HFO",
                            Number = number,
                            ReceiveDatetime = StaticEntity.RealtimeVesselinfos[number].ReceiveDatetime,
                            DeviceNo = "mefuel2"
                        });
                    }
                    break;

                case "mefuel2_out":
                    if (lstFm.Any(t => t.Number == number && t.DeviceNo == "mefuel2" && t.DeviceType == "me"))
                    {
                        var fm = lstFm.First(t => t.Number == number && t.DeviceNo == "mefuel2" && t.DeviceType == "me");
                        fm.ConsAct = fm.ConsAct - Convert.ToDecimal(DecodeProtocalData(sentence)[0]);
                        fm.ConsAcc = fm.ConsAcc - Convert.ToDecimal(DecodeProtocalData(sentence)[1]);
                    }
                    else
                    {
                        lstFm.Add(new Flowmeter
                        {
                            ConsAct = -Convert.ToDecimal(DecodeProtocalData(sentence)[0]),
                            ConsAcc = -Convert.ToDecimal(DecodeProtocalData(sentence)[1]),
                            Number = number,
                            DeviceNo = "mefuel2",
                            DeviceType = "me"
                        });
                    }
                    break;

                case "memethanol":
                    if (lstFm.Any(t => t.Number == number && t.DeviceNo == "memethanol" && t.DeviceType == "me"))
                    {
                        var fm = lstFm.First(t => t.Number == number && t.DeviceNo == "memethanol" && t.DeviceType == "me");
                        fm.ConsAct = fm.ConsAct + Convert.ToDecimal(DecodeProtocalData(sentence)[0]);
                        fm.ConsAcc = fm.ConsAcc + Convert.ToDecimal(DecodeProtocalData(sentence)[1]);
                        fm.Temperature = Convert.ToDecimal(DecodeProtocalData(sentence)[2]);
                        fm.Density = Convert.ToDecimal(DecodeProtocalData(sentence)[3]);
                        fm.DeviceType = "me";
                        fm.FuelType = "Methanol";
                        fm.Number = number;
                        fm.ReceiveDatetime = StaticEntity.RealtimeVesselinfos[number].ReceiveDatetime;
                        fm.DeviceNo = "memethanol";
                    }
                    else
                    {
                        lstFm.Add(new Flowmeter
                        {
                            ConsAct = Convert.ToDecimal(DecodeProtocalData(sentence)[0]),
                            ConsAcc = Convert.ToDecimal(DecodeProtocalData(sentence)[1]),
                            Temperature = Convert.ToDecimal(DecodeProtocalData(sentence)[2]),
                            Density = Convert.ToDecimal(DecodeProtocalData(sentence)[3]),
                            DeviceType = "me",
                            FuelType = "Methanol",
                            Number = number,
                            ReceiveDatetime = StaticEntity.RealtimeVesselinfos[number].ReceiveDatetime,
                            DeviceNo = "memethanol",
                        });
                    }
                    break;

                case "draft":
                    StaticEntity.RealtimeVesselinfos[number].BowDraft = Convert.ToDouble(DecodeProtocalData(sentence)[0]);
                    StaticEntity.RealtimeVesselinfos[number].AsternDraft = Convert.ToDouble(DecodeProtocalData(sentence)[1]);
                    StaticEntity.RealtimeVesselinfos[number].PortDraft = Convert.ToDouble(DecodeProtocalData(sentence)[2]);
                    StaticEntity.RealtimeVesselinfos[number].StarBoardDraft = Convert.ToDouble(DecodeProtocalData(sentence)[3]);
                    StaticEntity.RealtimeVesselinfos[number].Draft = (StaticEntity.RealtimeVesselinfos[number].BowDraft + StaticEntity.RealtimeVesselinfos[number].AsternDraft) / 2d;
                    StaticEntity.RealtimeVesselinfos[number].Trim = StaticEntity.RealtimeVesselinfos[number].AsternDraft + StaticEntity.RealtimeVesselinfos[number].BowDraft;
                    StaticEntity.RealtimeVesselinfos[number].Heel = StaticEntity.RealtimeVesselinfos[number].PortDraft + StaticEntity.RealtimeVesselinfos[number].StarBoardDraft;
                    break;

                case "shaft1":
                case "shaft2":
                    if (lstSh.Any(t => t.Number == number && t.DeviceNo == deviceCode))
                    {
                        var shaft = lstSh.First(t => t.Number == number && t.DeviceNo == deviceCode);
                        shaft.Power = Convert.ToDecimal(DecodeProtocalData(sentence)[0]);
                        shaft.RPM = Convert.ToDecimal(DecodeProtocalData(sentence)[1]);
                        shaft.Torque = Convert.ToDecimal(DecodeProtocalData(sentence)[2]);
                        shaft.Thrust = Convert.ToDecimal(DecodeProtocalData(sentence)[3]);
                        shaft.Number = number;
                        shaft.ReceiveDatetime = StaticEntity.RealtimeVesselinfos[number].ReceiveDatetime;
                        shaft.DeviceNo = deviceCode;
                    }
                    else
                    {
                        lstSh.Add(new Shaft
                        {
                            Power = Convert.ToDecimal(DecodeProtocalData(sentence)[0]),
                            RPM = Convert.ToDecimal(DecodeProtocalData(sentence)[1]),
                            Torque = Convert.ToDecimal(DecodeProtocalData(sentence)[2]),
                            Thrust = Convert.ToDecimal(DecodeProtocalData(sentence)[3]),
                            Number = number,
                            ReceiveDatetime = StaticEntity.RealtimeVesselinfos[number].ReceiveDatetime,
                            DeviceNo = deviceCode
                        });
                    }
                    break;

                case "generator1":
                case "generator2":
                case "generator3":
                case "generator4":
                    if (lstGe.Any(t => t.Number == number && t.DeviceNo == deviceCode))
                    {
                        var generator = lstGe.First(t => t.Number == number && t.DeviceNo == deviceCode);
                        generator.IsRuning = Convert.ToByte(DecodeProtocalData(sentence)[0]);
                        generator.RPM = Convert.ToDecimal(DecodeProtocalData(sentence)[1]);
                        generator.StartPressure = Convert.ToDecimal(DecodeProtocalData(sentence)[2]);
                        generator.ControlPressure = Convert.ToDecimal(DecodeProtocalData(sentence)[3]);
                        generator.ScavengingPressure = Convert.ToDecimal(DecodeProtocalData(sentence)[4]);
                        generator.LubePressure = Convert.ToDecimal(DecodeProtocalData(sentence)[5]);
                        generator.LubeTEMP = Convert.ToDecimal(DecodeProtocalData(sentence)[6]);
                        generator.FuelPressure = Convert.ToDecimal(DecodeProtocalData(sentence)[7]);
                        generator.FuelTEMP = Convert.ToDecimal(DecodeProtocalData(sentence)[8]);
                        generator.FreshWaterPressure = Convert.ToDecimal(DecodeProtocalData(sentence)[9]);
                        generator.FreshWaterTEMPIn = Convert.ToDecimal(DecodeProtocalData(sentence)[10]);
                        generator.FreshWaterTEMPOut = Convert.ToDecimal(DecodeProtocalData(sentence)[11]);
                        generator.CoolingWaterPressure = Convert.ToDecimal(DecodeProtocalData(sentence)[12]);
                        generator.CoolingWaterTEMPIn = Convert.ToDecimal(DecodeProtocalData(sentence)[13]);
                        generator.CoolingWaterTEMPOut = Convert.ToDecimal(DecodeProtocalData(sentence)[14]);
                        generator.CylinderTEMP1 = Convert.ToDecimal(DecodeProtocalData(sentence)[15]);
                        generator.CylinderTEMP2 = Convert.ToDecimal(DecodeProtocalData(sentence)[16]);
                        generator.CylinderTEMP3 = Convert.ToDecimal(DecodeProtocalData(sentence)[17]);
                        generator.CylinderTEMP4 = Convert.ToDecimal(DecodeProtocalData(sentence)[18]);
                        generator.CylinderTEMP5 = Convert.ToDecimal(DecodeProtocalData(sentence)[19]);
                        generator.CylinderTEMP6 = Convert.ToDecimal(DecodeProtocalData(sentence)[20]);
                        generator.SuperchargerTEMPIn = Convert.ToDecimal(DecodeProtocalData(sentence)[21]);
                        generator.SuperchargerTEMPOut = Convert.ToDecimal(DecodeProtocalData(sentence)[22]);
                        generator.ScavengingTEMP = Convert.ToDecimal(DecodeProtocalData(sentence)[23]);
                        generator.BearingTEMP = Convert.ToDecimal(DecodeProtocalData(sentence)[24]);
                        generator.BearingTEMPFront = Convert.ToDecimal(DecodeProtocalData(sentence)[25]);
                        generator.BearingTEMPBack = Convert.ToDecimal(DecodeProtocalData(sentence)[26]);
                        generator.Power = Convert.ToDecimal(DecodeProtocalData(sentence)[27]);
                        generator.WindingTEMPL1 = Convert.ToDecimal(DecodeProtocalData(sentence)[28]);
                        generator.WindingTEMPL2 = Convert.ToDecimal(DecodeProtocalData(sentence)[29]);
                        generator.WindingTEMPL3 = Convert.ToDecimal(DecodeProtocalData(sentence)[30]);
                        generator.VoltageL1L2 = Convert.ToDecimal(DecodeProtocalData(sentence)[31]);
                        generator.VoltageL2L3 = Convert.ToDecimal(DecodeProtocalData(sentence)[32]);
                        generator.VoltageL1L3 = Convert.ToDecimal(DecodeProtocalData(sentence)[33]);
                        generator.FrequencyL1 = Convert.ToDecimal(DecodeProtocalData(sentence)[34]);
                        generator.FrequencyL2 = Convert.ToDecimal(DecodeProtocalData(sentence)[35]);
                        generator.FrequencyL3 = Convert.ToDecimal(DecodeProtocalData(sentence)[36]);
                        generator.CurrentL1 = Convert.ToDecimal(DecodeProtocalData(sentence)[37]);
                        generator.CurrentL2 = Convert.ToDecimal(DecodeProtocalData(sentence)[38]);
                        generator.CurrentL3 = Convert.ToDecimal(DecodeProtocalData(sentence)[39]);
                        generator.ReactivePower = Convert.ToDecimal(DecodeProtocalData(sentence)[40]);
                        generator.PowerFactor = Convert.ToDecimal(DecodeProtocalData(sentence)[41]);
                        generator.LoadRatio = Convert.ToDecimal(DecodeProtocalData(sentence)[42]);
                        generator.ReceiveDatetime = StaticEntity.RealtimeVesselinfos[number].ReceiveDatetime;
                    }
                    else
                    {
                        lstGe.Add(new Generator
                        {
                            IsRuning = Convert.ToByte(DecodeProtocalData(sentence)[0]),
                            RPM = Convert.ToDecimal(DecodeProtocalData(sentence)[1]),
                            StartPressure = Convert.ToDecimal(DecodeProtocalData(sentence)[2]),
                            ControlPressure = Convert.ToDecimal(DecodeProtocalData(sentence)[3]),
                            ScavengingPressure = Convert.ToDecimal(DecodeProtocalData(sentence)[4]),
                            LubePressure = Convert.ToDecimal(DecodeProtocalData(sentence)[5]),
                            LubeTEMP = Convert.ToDecimal(DecodeProtocalData(sentence)[6]),
                            FuelPressure = Convert.ToDecimal(DecodeProtocalData(sentence)[7]),
                            FuelTEMP = Convert.ToDecimal(DecodeProtocalData(sentence)[8]),
                            FreshWaterPressure = Convert.ToDecimal(DecodeProtocalData(sentence)[9]),
                            FreshWaterTEMPIn = Convert.ToDecimal(DecodeProtocalData(sentence)[10]),
                            FreshWaterTEMPOut = Convert.ToDecimal(DecodeProtocalData(sentence)[11]),
                            CoolingWaterPressure = Convert.ToDecimal(DecodeProtocalData(sentence)[12]),
                            CoolingWaterTEMPIn = Convert.ToDecimal(DecodeProtocalData(sentence)[13]),
                            CoolingWaterTEMPOut = Convert.ToDecimal(DecodeProtocalData(sentence)[14]),
                            CylinderTEMP1 = Convert.ToDecimal(DecodeProtocalData(sentence)[15]),
                            CylinderTEMP2 = Convert.ToDecimal(DecodeProtocalData(sentence)[16]),
                            CylinderTEMP3 = Convert.ToDecimal(DecodeProtocalData(sentence)[17]),
                            CylinderTEMP4 = Convert.ToDecimal(DecodeProtocalData(sentence)[18]),
                            CylinderTEMP5 = Convert.ToDecimal(DecodeProtocalData(sentence)[19]),
                            CylinderTEMP6 = Convert.ToDecimal(DecodeProtocalData(sentence)[20]),
                            SuperchargerTEMPIn = Convert.ToDecimal(DecodeProtocalData(sentence)[21]),
                            SuperchargerTEMPOut = Convert.ToDecimal(DecodeProtocalData(sentence)[22]),
                            ScavengingTEMP = Convert.ToDecimal(DecodeProtocalData(sentence)[23]),
                            BearingTEMP = Convert.ToDecimal(DecodeProtocalData(sentence)[24]),
                            BearingTEMPFront = Convert.ToDecimal(DecodeProtocalData(sentence)[25]),
                            BearingTEMPBack = Convert.ToDecimal(DecodeProtocalData(sentence)[26]),
                            Power = Convert.ToDecimal(DecodeProtocalData(sentence)[27]),
                            WindingTEMPL1 = Convert.ToDecimal(DecodeProtocalData(sentence)[28]),
                            WindingTEMPL2 = Convert.ToDecimal(DecodeProtocalData(sentence)[29]),
                            WindingTEMPL3 = Convert.ToDecimal(DecodeProtocalData(sentence)[30]),
                            VoltageL1L2 = Convert.ToDecimal(DecodeProtocalData(sentence)[31]),
                            VoltageL2L3 = Convert.ToDecimal(DecodeProtocalData(sentence)[32]),
                            VoltageL1L3 = Convert.ToDecimal(DecodeProtocalData(sentence)[33]),
                            FrequencyL1 = Convert.ToDecimal(DecodeProtocalData(sentence)[34]),
                            FrequencyL2 = Convert.ToDecimal(DecodeProtocalData(sentence)[35]),
                            FrequencyL3 = Convert.ToDecimal(DecodeProtocalData(sentence)[36]),
                            CurrentL1 = Convert.ToDecimal(DecodeProtocalData(sentence)[37]),
                            CurrentL2 = Convert.ToDecimal(DecodeProtocalData(sentence)[38]),
                            CurrentL3 = Convert.ToDecimal(DecodeProtocalData(sentence)[39]),
                            ReactivePower = Convert.ToDecimal(DecodeProtocalData(sentence)[40]),
                            PowerFactor = Convert.ToDecimal(DecodeProtocalData(sentence)[41]),
                            LoadRatio = Convert.ToDecimal(DecodeProtocalData(sentence)[42]),
                            ReceiveDatetime = StaticEntity.RealtimeVesselinfos[number].ReceiveDatetime,
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
                        battery.SOC = Convert.ToDecimal(DecodeProtocalData(sentence)[0]);
                        battery.SOH = Convert.ToDecimal(DecodeProtocalData(sentence)[1]);
                        battery.MaxTEMP = Convert.ToDecimal(DecodeProtocalData(sentence)[2]);
                        battery.MaxTEMPBox = DecodeProtocalData(sentence)[3].ToString();
                        battery.MaxTEMPNo = DecodeProtocalData(sentence)[4].ToString();
                        battery.MinTEMP = Convert.ToDecimal(DecodeProtocalData(sentence)[5]);
                        battery.MinTEMPBox = DecodeProtocalData(sentence)[6].ToString();
                        battery.MinTEMPNo = DecodeProtocalData(sentence)[7].ToString();
                        battery.MaxVoltage = Convert.ToDecimal(DecodeProtocalData(sentence)[8]);
                        battery.MaxVoltageBox = DecodeProtocalData(sentence)[9].ToString();
                        battery.MaxVoltageNo = DecodeProtocalData(sentence)[10].ToString();
                        battery.MinVoltage = Convert.ToDecimal(DecodeProtocalData(sentence)[11]);
                        battery.MinVoltageBox = DecodeProtocalData(sentence)[12].ToString();
                        battery.MinVoltageNo = DecodeProtocalData(sentence)[13].ToString();
                        battery.ReceiveDatetime = StaticEntity.RealtimeVesselinfos[number].ReceiveDatetime;
                    }
                    else
                    {
                        lstBa.Add(new Battery
                        {
                            SOC = Convert.ToDecimal(DecodeProtocalData(sentence)[0]),
                            SOH = Convert.ToDecimal(DecodeProtocalData(sentence)[1]),
                            MaxTEMP = Convert.ToDecimal(DecodeProtocalData(sentence)[2]),
                            MaxTEMPBox = DecodeProtocalData(sentence)[3].ToString(),
                            MaxTEMPNo = DecodeProtocalData(sentence)[4].ToString(),
                            MinTEMP = Convert.ToDecimal(DecodeProtocalData(sentence)[5]),
                            MinTEMPBox = DecodeProtocalData(sentence)[6].ToString(),
                            MinTEMPNo = DecodeProtocalData(sentence)[7].ToString(),
                            MaxVoltage = Convert.ToDecimal(DecodeProtocalData(sentence)[8]),
                            MaxVoltageBox = DecodeProtocalData(sentence)[9].ToString(),
                            MaxVoltageNo = DecodeProtocalData(sentence)[10].ToString(),
                            MinVoltage = Convert.ToDecimal(DecodeProtocalData(sentence)[11]),
                            MinVoltageBox = DecodeProtocalData(sentence)[12].ToString(),
                            MinVoltageNo = DecodeProtocalData(sentence)[13].ToString(),
                            ReceiveDatetime = StaticEntity.RealtimeVesselinfos[number].ReceiveDatetime,
                            DeviceNo = deviceCode,
                            Number = number
                        });
                    }
                    break;
            }
        }

        /// <summary>
        /// 创建船舶状态信息字典
        /// </summary>
        /// <param name="number"></param>
        public void CreateDics(string number)
        {
            if (!StaticEntity.RealtimeFlowmeters.ContainsKey(number))
                StaticEntity.RealtimeFlowmeters.Add(number, new List<Flowmeter>());
            if (!StaticEntity.RealtimeBatteries.ContainsKey(number))
                StaticEntity.RealtimeBatteries.Add(number, new List<Battery>());
            if (!StaticEntity.RealtimeGenerators.ContainsKey(number))
                StaticEntity.RealtimeGenerators.Add(number, new List<Generator>());
            if (!StaticEntity.RealtimeLiquidLevels.ContainsKey(number))
                StaticEntity.RealtimeLiquidLevels.Add(number, new List<LiquidLevel>());
            if (!StaticEntity.RealtimeSupplyUnits.ContainsKey(number))
                StaticEntity.RealtimeSupplyUnits.Add(number, new List<SupplyUnit>());
            if (!StaticEntity.RealtimeShafts.ContainsKey(number))
                StaticEntity.RealtimeShafts.Add(number, new List<Shaft>());
            if (!StaticEntity.RealtimeSternSealings.ContainsKey(number))
                StaticEntity.RealtimeSternSealings.Add(number, new List<SternSealing>());
        }

        /// <summary>
        /// 解析协议数据
        /// </summary>
        /// <param name="sentence"></param>
        /// <returns></returns>
        public object[] DecodeProtocalData(string sentence)
        {
            return new object[] { };
        }

        public byte[] StringToBytes(string s)
        {
            string[] str = s.Split(' ');
            int n = str.Length;

            byte[] cmdBytes = null;
            int p = 0;

            for (int k = 0; k < n; k++)
            {
                int sLen = str[k].Length;
                int bytesLen = sLen / 2;
                int position = 0;
                byte[] bytes = new byte[bytesLen];
                for (int i = 0; i < bytesLen; i++)
                {
                    string abyte = str[k].Substring(position, 2);
                    bytes[i] = Convert.ToByte(abyte, 16);
                    position += 2;
                }

                if (position >= 2)
                {
                    byte[] cmdBytes2 = new byte[p + bytesLen];
                    if (cmdBytes != null)
                    {
                        Array.Copy(cmdBytes, 0, cmdBytes2, 0, p);
                    }
                    Array.Copy(bytes, 0, cmdBytes2, p, bytesLen);
                    cmdBytes = cmdBytes2;
                    p += bytesLen;
                }
            }

            return cmdBytes;
        }

        #region 参数计算

        /// <summary>
        /// 获取参数计算
        /// </summary>
        /// <param name="real_WidgetItems"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void CalcParams(object real_WidgetItems)
        {
            var dictionary = real_WidgetItems as IDictionary<string, object>;
        }

        /// <summary>
        /// 经纬度小数转度分秒
        /// </summary>
        /// <param name="strLatLongValue">格式如:E 123.4234</param>
        /// <returns></returns>
        private string TLatLong(string strLatLongValue)
        {
            string result = "";
            try
            {
                string[] tempStrs = strLatLongValue.Split(' ');
                double result_degree = Math.Floor(Convert.ToDouble(tempStrs[1]) / 100);
                double result_minute = Math.Floor(Convert.ToDouble(tempStrs[1])) - result_degree * 100;
                double result_second = Math.Round((Convert.ToDouble(tempStrs[1]) - Math.Floor(Convert.ToDouble(tempStrs[1]))) * 60, 4);
                result = tempStrs[0] + result_degree + "°" + result_minute + "′" + result_second + "″";
            }
            catch (Exception ex)
            {
                //Log.Error(ex, "经纬度转换失败");
            }
            return result;
        }

        /// <summary>
        /// 获取速度
        /// </summary>
        /// <param name="lngSpd"></param>
        /// <param name="tvsSpd"></param>
        /// <returns></returns>
        private string TSpeed(string lngSpd, string tvsSpd)
        {
            string result = "";
            try
            {
                double result_d = Math.Sqrt(Math.Pow(Convert.ToDouble(lngSpd), 2) + Math.Pow(Convert.ToDouble(tvsSpd), 2));
                result = Math.Round(result_d, 4).ToString();
            }
            catch (Exception ex)
            {
                //Log.Error(ex, "航速转换失败");
            }
            return result;
        }

        /// <summary>
        /// 计算纵倾
        /// </summary>
        /// <param name="shipDraughtBow"></param>
        /// <param name="shipDraughtAstern"></param>
        /// <returns></returns>
        private string CalcShipTrim(string shipDraughtBow, string shipDraughtAstern)
        {
            string result = "";
            try
            {
                double shipDraughtBow_d = Convert.ToDouble(shipDraughtBow);
                double shipDraughtAstern_d = Convert.ToDouble(shipDraughtAstern);
                result = Math.Round(shipDraughtAstern_d - shipDraughtBow_d, 2).ToString();
            }
            catch (Exception ex)
            {
                //Log.Error(ex, "纵倾计算失败");
            }
            return result;
        }

        /// <summary>
        /// 计算横倾
        /// </summary>
        /// <param name="shipDraughtMidLft"></param>
        /// <param name="shipDraughtMidRgt"></param>
        /// <returns></returns>
        private string CalcShipHeel(string shipDraughtMidLft, string shipDraughtMidRgt)
        {
            string result = "";
            try
            {
                double shipDraughtMidLft_d = Convert.ToDouble(shipDraughtMidLft);
                double shipDraughtMidRgt_d = Convert.ToDouble(shipDraughtMidRgt);
                result = Math.Round(shipDraughtMidLft_d - shipDraughtMidRgt_d, 2).ToString();
            }
            catch (Exception ex)
            {
                //Log.Error(ex, "横倾计算失败");
            }
            return result;
        }

        /// <summary>
        /// 计算吃水
        /// </summary>
        /// <param name="shipDraughtBow"></param>
        /// <param name="shipDraughtAstern"></param>
        /// <returns></returns>
        private string CalcShipDraft(string shipDraughtBow, string shipDraughtAstern)
        {
            string result = "";
            try
            {
                double shipDraughtBow_d = Convert.ToDouble(shipDraughtBow);
                double shipDraughtAstern_d = Convert.ToDouble(shipDraughtAstern);
                result = Math.Round((shipDraughtAstern_d + shipDraughtBow_d) / 2, 2).ToString();
            }
            catch (Exception ex)
            {
                //Log.Error(ex, "吃水计算失败");
            }
            return result;
        }

        /// <summary>
        /// 通过对水航速计算滑失比
        /// </summary>
        /// <param name="meRpm">主机转速</param>
        /// <param name="meShaftPow">主机轴功率</param>
        /// <param name="propPitch">螺距</param>
        /// <param name="watSpd">对水航速</param>
        /// <param name="limitMERpm">主机转速限定值 自定义</param>
        /// <param name="limitMEShaftPow">主机轴功率限定值 自定义</param>
        /// <returns></returns>
        private string CalcShipSlip(string meRpm, string meShaftPow, string propPitch, string watSpd, double limitMERpm = 0.5, double limitMEShaftPow = 16)
        {
            string result = "";
            try
            {
                double meRpm_d = Convert.ToDouble(meRpm);
                double meShaftPow_d = Convert.ToDouble(meShaftPow);
                double propPitch_d = Convert.ToDouble(propPitch);
                double watSpd_d = Convert.ToDouble(watSpd);

                if (Math.Abs(meRpm_d) > limitMERpm && meShaftPow_d > limitMEShaftPow)
                    //船速换算为 m/min 滑失比为百分比 要乘100
                    result = Math.Round((1 - watSpd_d / (meRpm_d * propPitch_d) * 1852 * 1000 / 60d) * 100d, 2) + "%";
                else
                {
                    if (meShaftPow_d < limitMEShaftPow)
                        //小于限定值视为停车
                        result = "--%";
                    else
                    {
                        if (meRpm_d != 0)
                            result = Math.Round((1 - watSpd_d / (meRpm_d * propPitch_d) * 1852 * 1000 / 60d) * 100d, 2) + "%";
                        else
                            result = "--%";
                    }
                }
            }
            catch (Exception ex)
            {
                //Log.Error(ex, "滑失比计算失败");
            }
            return result;
        }

        /// <summary>
        /// 计算主机燃油消耗率(每千瓦)
        /// </summary>
        /// <param name="meShaftPow">主机轴功率</param>
        /// <param name="meMGOCons">重油消耗</param>
        /// <param name="meFOCons">轻油消耗</param>
        /// <param name="limitMEShaftPow">主机轴功率限定值</param>
        /// <returns></returns>
        private string CalcMESFOC_kw(string meShaftPow, string meMGOCons, string meFOCons, double limitMEShaftPow = 16)
        {
            string result = "";
            try
            {
                double meShaftPow_d = Convert.ToDouble(meShaftPow);
                double meMGOCons_d = Convert.ToDouble(meMGOCons);
                double meFOCons_d = Convert.ToDouble(meFOCons);
                if (meShaftPow_d > limitMEShaftPow)
                    result = Math.Round((meMGOCons_d + meFOCons_d) * 1000 / meShaftPow_d, 2).ToString();
                else
                    result = "0";
            }
            catch (Exception ex)
            {
                result = "0";
                //Log.Error(ex, "主机燃油消耗率(每千瓦)计算失败");
            }
            return result;
        }

        /// <summary>
        /// 计算主机燃油消耗率(每海里)
        /// </summary>
        /// <param name="watSpd">对水航速</param>
        /// <param name="meRpm">主机转速</param>
        /// <param name="meShaftPow">主机轴功率</param>
        /// <param name="meMGOCons">主机轻油消耗</param>
        /// <param name="meFOCons">主机重油消耗</param>
        /// <param name="limitMERpm">主机转速限定值 默认0.5</param>
        /// <param name="limitMEShaftPow">主机轴功率限定值 默认16</param>
        /// <returns></returns>
        private string CalcMESFOC_nmile(string watSpd, string meRpm, string meShaftPow, string meMGOCons, string meFOCons, double limitMERpm = 0.5, double limitMEShaftPow = 16)
        {
            string result = "";
            try
            {
                double watSpd_d = Convert.ToDouble(watSpd);
                double meRpm_d = Convert.ToDouble(meRpm);
                double meShaftPow_d = Convert.ToDouble(meShaftPow);
                double meMGOCons_d = Convert.ToDouble(meMGOCons);
                double meFOCons_d = Convert.ToDouble(meFOCons);
                if (watSpd_d != 0 && Math.Abs(meRpm_d) > limitMERpm && meShaftPow_d > limitMEShaftPow)
                {
                    result = Math.Round(meMGOCons_d + meFOCons_d / watSpd_d, 2).ToString();
                }
                else
                {
                    result = "0";
                }
            }
            catch (Exception ex)
            {
                //Log.Error(ex, "主机燃油消耗率(每海里)计算失败");
            }
            return result;
        }

        /// <summary>
        /// 计算整船燃油消耗率(每海里)
        /// </summary>
        /// <param name="watSpd">水速</param>
        /// <param name="meRpm">主机转算</param>
        /// <param name="meShaftPow">主机轴功率</param>
        /// <param name="meMGOCons">主机轻油消耗</param>
        /// <param name="meFOCons">主机重油消耗</param>
        /// <param name="dgMGOCons">辅机轻油消耗</param>
        /// <param name="dgFOCons">辅机重要消耗</param>
        /// <param name="blrMGOCons">锅炉轻油消耗</param>
        /// <param name="blrFOCons">锅炉重油消耗</param>
        /// <param name="limitMERpm">主机转速限定值 默认0.5</param>
        /// <param name="limitMEShaftPow">主机轴功率限定值 默认16</param>
        /// <returns></returns>
        private string CalcShipSFOC_nmile(string watSpd, string meRpm, string meShaftPow, string meMGOCons, string meFOCons, string dgMGOCons, string dgFOCons, string blrMGOCons, string blrFOCons, double limitMERpm = 0.5, double limitMEShaftPow = 16)
        {
            string result = "";
            try
            {
                double watSpd_d = Convert.ToDouble(watSpd);
                double meRpm_d = Convert.ToDouble(meRpm);
                double meShaftPow_d = Convert.ToDouble(meShaftPow);
                double meMGOCons_d = Convert.ToDouble(meMGOCons);
                double meFOCons_d = Convert.ToDouble(meFOCons);
                double dgMGOCons_d = Convert.ToDouble(dgMGOCons);
                double dgFOCons_d = Convert.ToDouble(dgFOCons);
                double blrMGOCons_d = Convert.ToDouble(blrMGOCons);
                double blrFOCons_d = Convert.ToDouble(blrFOCons);
                if (watSpd_d != 0 && Math.Abs(meRpm_d) > limitMERpm && meShaftPow_d > limitMEShaftPow)
                    result = Math.Round((meMGOCons_d + meFOCons_d + dgMGOCons_d + dgFOCons_d + blrMGOCons_d + blrFOCons_d) / watSpd_d, 2).ToString();
            }
            catch (Exception ex)
            {
                //Log.Error(ex, "整船燃油消耗率(每海里)计算失败");
            }
            return result;
        }

        /// <summary>
        /// 计算二氧化碳排放量
        /// </summary>
        /// <param name="meFOCons">主机重油油耗</param>
        /// <param name="dgFOCons">辅机重油油耗</param>
        /// <param name="blrFOCons">锅炉重油油耗</param>
        /// <param name="meMDOCons">主机柴油油耗</param>
        /// <param name="dgMDOCons">电机柴油油耗</param>
        /// <param name="blrMDOCons">锅炉柴油油耗</param>
        /// <param name="DWT">载重</param>
        /// <param name="grdSpd">航速</param>
        /// <param name="HFO_CF">重油碳排放系数 默认3.114400</param>
        /// <param name="MDO_CF">轻油碳排放系数据 默认3.206000</param>
        /// <returns></returns>
        private string CalcDWTCO2(string meFOCons, string dgFOCons, string blrFOCons, string meMDOCons, string dgMDOCons, string blrMDOCons, string DWT, string grdSpd, double HFO_CF = 3.114400, double MDO_CF = 3.206000)
        {
            string result = "0";
            try
            {
                double meFOCons_d = Convert.ToDouble(meFOCons);
                double dgFOCons_d = Convert.ToDouble(dgFOCons);
                double blrFOCons_d = Convert.ToDouble(blrFOCons);
                double meMDOCons_d = Convert.ToDouble(meMDOCons);
                double dgMDOCons_d = Convert.ToDouble(dgMDOCons);
                double blrMDOCons_d = Convert.ToDouble(blrMDOCons);
                double DWT_d = Convert.ToDouble(DWT);
                double grdSpd_d = Convert.ToDouble(grdSpd);

                double tempD = DWT_d * grdSpd_d * 1.852;
                if (tempD > 0)
                {
                    result = Math.Round(((meFOCons_d + dgFOCons_d + blrFOCons_d) * HFO_CF + (meMDOCons_d + dgMDOCons_d + blrMDOCons_d) * MDO_CF) * 1000 / tempD, 2).ToString();
                }
            }
            catch (Exception ex)
            {
                //Log.Error(ex, "二氧化碳排放量计算失败");
            }
            return result;
        }

        private string CalcEEOI(string meFOCons_ACC, string dgFOCons_ACC, string blrFOCons_ACC, string meMDOCons_ACC, string dgMDOCons_ACC, string blrMDOCons_ACC, string mcargo, string distance, double HFO_CF = 3.114400, double MDO_CF = 3.206000)
        {
            string result = "";
            try
            {
                double meFOCons_ACC_d = Convert.ToDouble(meFOCons_ACC);
                double dgFOCons_ACC_d = Convert.ToDouble(dgFOCons_ACC);
                double blrFOCons_ACC_d = Convert.ToDouble(blrFOCons_ACC);
                double meMDOCons_ACC_d = Convert.ToDouble(meMDOCons_ACC);
                double dgMDOCons_ACC_d = Convert.ToDouble(dgMDOCons_ACC);
                double blrMDOCons_ACC_d = Convert.ToDouble(blrMDOCons_ACC);
                double mcargo_d = Convert.ToDouble(mcargo);
                double distance_d = Convert.ToDouble(distance);

                double tempD = mcargo_d * distance_d;
                if (tempD > 0)
                {
                    result = Math.Round(((meFOCons_ACC_d + dgFOCons_ACC_d + blrFOCons_ACC_d) * HFO_CF + (meMDOCons_ACC_d + dgMDOCons_ACC_d + blrMDOCons_ACC_d) * MDO_CF) * 1000 / tempD, 2).ToString();
                }
            }
            catch (Exception ex)
            {
                //Log.Error(ex, "EEOI计算失败");
            }
            return result;
        }

        #endregion 参数计算

        #region CRC校验

        /// <summary>
        /// CRC校验
        /// </summary>
        /// <param name="checkContent">校验内容 不含$</param>
        /// <param name="contentLength">校验码长度</param>
        /// <returns></returns>
        public bool CRCCheck(string checkContent, int contentLength)
        {
            bool result = false;

            string[] checkContents = checkContent.Split('*');
            if (checkContents.Length > 0)
            {
                byte[] bytesContent = Encoding.ASCII.GetBytes(checkContents[0]);
                byte[] bytes = Encoding.ASCII.GetBytes(checkContents[1]);
                if (bytes.Length == contentLength)
                {
                    if (bytes[0] > 0x40)
                        bytes[0] = Convert.ToByte((bytes[0] - 0x41 + 10) & 0xff);
                    else
                        bytes[0] = Convert.ToByte(bytes[0] - 0x30);
                    if (bytes[1] > 0x40)
                        bytes[1] = Convert.ToByte((bytes[1] - 0x41 + 10) & 0xff);
                    else
                        bytes[1] = Convert.ToByte(bytes[1] - 0x30);
                    int checkResult = bytes[0] * 16 + bytes[1];
                    int crc = bytesContent[0];
                    for (int i = 1; i < bytesContent.Length; i++)
                        crc = crc ^ bytesContent[i];

                    if (crc == checkResult)
                        result = true;
                }
            }
            //return result;
            return true;
        }

        /// <summary>
        /// CRC校验MODBUS
        /// </summary>
        /// <param name="byteData"></param>
        /// <returns></returns>
        public bool CRC_Check(byte[] byteData)
        {
            bool Flag = false;
            byte[] CRC = new byte[2];

            UInt16 wCrc = 0xFFFF;
            for (int i = 0; i < byteData.Length - 2; i++)
            {
                wCrc ^= Convert.ToUInt16(byteData[i]);
                for (int j = 0; j < 8; j++)
                {
                    if ((wCrc & 0x0001) == 1)
                    {
                        wCrc >>= 1;
                        wCrc ^= 0xA001;
                    }
                    else
                    {
                        wCrc >>= 1;
                    }
                }
            }

            CRC[1] = (byte)((wCrc & 0xFF00) >> 8);
            CRC[0] = (byte)(wCrc & 0x00FF);
            if (CRC[1] == byteData[byteData.Length - 1]
                && CRC[0] == byteData[byteData.Length - 2])
            {
                Flag = true;
            }
            return Flag;
        }

        #endregion CRC校验

        #region 设备分类解析

        /// <summary>
        /// MWV 风速角度
        /// </summary>
        /// <param name="strData"></param>
        /// <param name="Result"></param>
        /// <returns></returns>
        private async Task<int> DataMWVAnalysis(string strData, CreateSentenceDto sentenceDto)
        {
            var result = 0;
            try
            {
                string[] strMWVInfo = strData.Split(',');
                if (strMWVInfo[5].ToUpper() == "V")
                {
                    return 0;
                }
                string guid = Guid.NewGuid().ToString();
                StringBuilder sbSql = new StringBuilder();
                sbSql.Append("insert into vdr_mwv (create_time,sentenceid,type,angle,reference,speed,unit)" +
                    $" values (SYSDATE(),'{guid}','{strMWVInfo[0].Trim('$')}','{strMWVInfo[1]}','{strMWVInfo[2]}','{strMWVInfo[3]}','{strMWVInfo[4]}')");
                result = await AddAsync(sbSql.ToString());
                if (result > 0)
                {
                    /*sentenceDto.sentenceid = guid;
                    sentenceDto.isdecoded = 1;*/
                }

                result = await _sentenceService.CreateAsync(sentenceDto) == null ? 0 : 1;
                return result;
            }
            catch (Exception ex)
            {
                //Log.Error(ex, "DataMWVAnalysis:" + strData);
                return 0;
            }
        }

        /// <summary>
        /// VBW 航速
        /// </summary>
        /// <param name="strData"></param>
        /// <param name="Result"></param>
        /// <returns></returns>
        private async Task<int> DataVBWAnalysis(string strData, CreateSentenceDto sentenceDto)
        {
            var result = 0;
            try
            {
                string[] strVBWInfo = strData.Split(',');
                string guid = Guid.NewGuid().ToString();
                StringBuilder sbSql = new StringBuilder();
                sbSql.Append("insert into vdr_vbw (create_time,sentenceid,type,lngwatspd,tvswatspd,watspdstatus,lnggrdspd,tvsgrdspd,grdspdstatus,tvswatspdstern,watspdstatusstern,tvsgrdspdstern,grdspdstatusstern,watspd,grdspd)" +
                    $" values (SYSDATE(),'{guid}','{strVBWInfo[0].Trim('$')}','{strVBWInfo[1]}','{strVBWInfo[2]}','{strVBWInfo[3]}','{strVBWInfo[4]}','{strVBWInfo[5]}','{strVBWInfo[6]}','{strVBWInfo[7]}','{strVBWInfo[8]}','{strVBWInfo[9]}','{strVBWInfo[10]}','{TSpeed(strVBWInfo[1], strVBWInfo[2])}','{TSpeed(strVBWInfo[4], strVBWInfo[5])}')");
                result = await AddAsync(sbSql.ToString());
                if (result > 0)
                {
                    /*sentenceDto.sentenceid = guid;
                    sentenceDto.isdecoded = 1;*/
                }
                result = await _sentenceService.CreateAsync(sentenceDto) == null ? 0 : 1;
                return result;
            }
            catch (Exception ex)
            {
                //Log.Error(ex, "DataVBWAnalysis:" + strData);
                return 0;
            }
        }

        /// <summary>
        /// MWD 风向风速
        /// </summary>
        /// <param name="strData"></param>
        private async Task<int> DataMWDAnalysis(string strData, CreateSentenceDto sentenceDto)
        {
            var result = 0;
            try
            {
                string[] strMWDInfo = strData.Split(',');
                string guid = Guid.NewGuid().ToString();
                StringBuilder sbSql = new StringBuilder();
                sbSql.Append("insert into vdr_mwd (create_time,sentenceid,type,tdirection,magdirection,knspeed,speed)" +
                    $" values (SYSDATE(),'{guid}','{strMWDInfo[0].Trim('$')}','{strMWDInfo[2] + strMWDInfo[1]}','{strMWDInfo[4] + strMWDInfo[3]}','{strMWDInfo[5] + strMWDInfo[6]}','{strMWDInfo[7] + strMWDInfo[8]}')");
                result = await AddAsync(sbSql.ToString());
                if (result > 0)
                {
                    /*sentenceDto.sentenceid = guid;
                    sentenceDto.isdecoded = 1;*/
                }
                result = await _sentenceService.CreateAsync(sentenceDto) == null ? 0 : 1;
                return result;
            }
            catch (Exception ex)
            {
                //Log.Error(ex, "获取MWD信息错误,接收语句:" + strData);
                return 0;
            }
        }

        /// <summary>
        /// GGA GPS定位信息
        /// </summary>
        /// <param name="strData"></param>
        private async Task<int> DataGGAAnalysis(string strData, CreateSentenceDto sentenceDto)
        {
            var result = 0;
            try
            {
                string[] strGGAInfo = strData.Split(',');
                string guid = Guid.NewGuid().ToString();
                StringBuilder sbSql = new StringBuilder();
                sbSql.Append("insert into vdr_gga (create_time,sentenceid,type,latitude,longitude,satnum)" +
                    $" values (SYSDATE(),'{guid}','{strGGAInfo[0].Trim('$')}','{TLatLong(strGGAInfo[3] + " " + strGGAInfo[2])}','{TLatLong(strGGAInfo[5] + " " + strGGAInfo[4])}','{strGGAInfo[7]}')");
                result = await AddAsync(sbSql.ToString());
                if (result > 0)
                {
                    /*sentenceDto.sentenceid = guid;
                    sentenceDto.isdecoded = 1;*/
                }
                result = await _sentenceService.CreateAsync(sentenceDto) == null ? 0 : 1;
                return result;
            }
            catch (Exception ex)
            {
                //Log.Error(ex, "获取GGA信息错误,接收语句:" + strData);
                return 0;
            }
        }

        /// <summary>
        /// DPT 吃水
        /// </summary>
        /// <param name="strData"></param>
        private async Task<int> DataDPTAnalysis(string strData, CreateSentenceDto sentenceDto)
        {
            var result = 0;
            try
            {
                string[] strDPTInfo = strData.Split(',');
                string guid = Guid.NewGuid().ToString();
                StringBuilder sbSql = new StringBuilder();
                sbSql.Append("insert into vdr_dpt (create_time,sentenceid,type,depth,offset,mrs)" +
                    $" values (SYSDATE(),'{guid}','{strDPTInfo[0].Trim('$')}','{strDPTInfo[1]}','{strDPTInfo[2]}','{strDPTInfo[3]}')");
                result = await AddAsync(sbSql.ToString());
                if (result > 0)
                {
                    /*sentenceDto.sentenceid = guid;
                    sentenceDto.isdecoded = 1;*/
                }
                result = await _sentenceService.CreateAsync(sentenceDto) == null ? 0 : 1;
                return result;
            }
            catch (Exception ex)
            {
                //Log.Error(ex, "获取DPT信息错误,接收语句:" + strData);
                return 0;
            }
        }

        /// <summary>
        /// HDG
        /// </summary>
        /// <param name="strData"></param>
        private async Task<int> DataHDGAnalysis(string strData, CreateSentenceDto sentenceDto)
        {
            var result = 0;
            try
            {
                string[] strHDGInfo = strData.Split(',');
                string guid = Guid.NewGuid().ToString();
                StringBuilder sbSql = new StringBuilder();
                sbSql.Append("insert into vdr_hdg (create_time,sentenceid,type,msh,md,mv)" +
                    $" values (SYSDATE(),'{guid}','{strHDGInfo[0].Trim('$')}','{strHDGInfo[1]}','{strHDGInfo[3] + " " + strHDGInfo[2]}','{strHDGInfo[5] + " " + strHDGInfo[4]}')");
                result = await AddAsync(sbSql.ToString());
                if (result > 0)
                {
                    /*sentenceDto.sentenceid = guid;
                    sentenceDto.isdecoded = 1;*/
                }
                result = await _sentenceService.CreateAsync(sentenceDto) == null ? 0 : 1;
                return result;
            }
            catch (Exception ex)
            {
                //Log.Error(ex, "获取HDG信息错误,接收语句:" + strData);
                return 0;
            }
        }

        /// <summary>
        /// VLW 里程
        /// </summary>
        /// <param name="strData"></param>
        private async Task<int> DataVLWAnalysis(string strData, CreateSentenceDto sentenceDto)
        {
            var result = 0;
            try
            {
                string[] strVLWInfo = strData.Split(',');
                string guid = Guid.NewGuid().ToString();
                StringBuilder sbSql = new StringBuilder();
                sbSql.Append("insert into vdr_vlw (create_time,sentenceid,type,watdistotal,watdisreset,grddistotal,grddisreset)" +
                    $" values (SYSDATE(),'{guid}','{strVLWInfo[0].Trim('$')}','{strVLWInfo[1] + " " + strVLWInfo[2]}','{strVLWInfo[3] + " " + strVLWInfo[4]}','{strVLWInfo[5] + " " + strVLWInfo[6]}','{strVLWInfo[7] + " " + strVLWInfo[8]}')");
                result = await AddAsync(sbSql.ToString());
                if (result > 0)
                {
                    /*sentenceDto.sentenceid = guid;
                    sentenceDto.isdecoded = 1;*/
                }
                result = await _sentenceService.CreateAsync(sentenceDto) == null ? 0 : 1;
                return result;
            }
            catch (Exception ex)
            {
                //Log.Error(ex, "获取VLW信息错误,接收语句:" + strData);
                return 0;
            }
        }

        /// <summary>
        /// PRC
        /// </summary>
        /// <param name="strData"></param>
        private async Task<int> DataPRCAnalysis(string strData, CreateSentenceDto sentenceDto)
        {
            var result = 0;
            try
            {
                string[] strPRCInfo = strData.Split(',');
                string guid = Guid.NewGuid().ToString();
                StringBuilder sbSql = new StringBuilder();
                sbSql.Append("insert into vdr_prc (create_time,sentenceid,type,leverdemand,leverstatus,rpmdemand,rpmstatus,pitchdemand,pitchstatus,number)" +
                    $" values (SYSDATE(),'{guid}','{strPRCInfo[0].Trim('$')}','{strPRCInfo[1]}','{strPRCInfo[2]}','{strPRCInfo[3]}','{strPRCInfo[4]}','{strPRCInfo[5]}','{strPRCInfo[6]}','{strPRCInfo[8]}')");
                result = await AddAsync(sbSql.ToString());
                if (result > 0)
                {
                    /*sentenceDto.sentenceid = guid;
                    sentenceDto.isdecoded = 1;*/
                }
                result = await _sentenceService.CreateAsync(sentenceDto) == null ? 0 : 1;
                return result;
            }
            catch (Exception ex)
            {
                //Log.Error(ex, "获取PRC信息错误,接收语句:" + strData);
                return 0;
            }
        }

        /// <summary>
        /// XDR 多项参数
        /// </summary>
        /// <param name="strData"></param>
        private async Task<int> DataXDRAnalysis(string strData, CreateSentenceDto sentenceDto)
        {
            var result = 0;
            try
            {
                string[] strXDRInfo = strData.Split(',');
                string guid = Guid.NewGuid().ToString();
                StringBuilder sbSql = new StringBuilder();
                sbSql.Append("insert into vdr_xdr (create_time,sentenceid,type,sensortype,sensorvalue,sensorunit,sensorid)" +
                    $" values (SYSDATE(),'{guid}','{strXDRInfo[0].Trim('$')}','{strXDRInfo[1]}','{strXDRInfo[2]}','{strXDRInfo[3]}','{strXDRInfo[4]}')");
                result = await AddAsync(sbSql.ToString());
                if (result > 0)
                {
                    /*sentenceDto.sentenceid = guid;
                    sentenceDto.isdecoded = 1;*/
                }
                result = await _sentenceService.CreateAsync(sentenceDto) == null ? 0 : 1;
                return result;
            }
            catch (Exception ex)
            {
                //Log.Error(ex, "获取XDR信息错误,接收语句:" + strData);
                return 0;
            }
        }

        /// <summary>
        /// GNS GPS
        /// </summary>
        /// <param name="strData"></param>
        private async Task<int> DataGNSAnalysis(string strData, CreateSentenceDto sentenceDto)
        {
            var result = 0;
            try
            {
                string[] strGNSInfo = strData.Split(',');
                if (strGNSInfo[13].ToUpper() == "N")
                {
                    return 0;
                }
                string guid = Guid.NewGuid().ToString();
                StringBuilder sbSql = new StringBuilder();
                sbSql.Append("insert into vdr_gns (create_time,sentenceid,type,gnsdatetime,latitude,longtitude,satnum,hdop,antennaaltitude,geoidalseparation)" +
                    $" values (SYSDATE(),'{guid}','{strGNSInfo[0].Trim('$')}','{strGNSInfo[1]}','{TLatLong(strGNSInfo[3] + " " + strGNSInfo[2])}','{TLatLong(strGNSInfo[5] + " " + strGNSInfo[4])}','{strGNSInfo[7]}','{strGNSInfo[8]}','{strGNSInfo[9]}','{strGNSInfo[10]}')");
                result = await AddAsync(sbSql.ToString());
                if (result > 0)
                {
                    /*sentenceDto.sentenceid = guid;
                    sentenceDto.isdecoded = 1;*/
                }
                result = await _sentenceService.CreateAsync(sentenceDto) == null ? 0 : 1;
                return result;
            }
            catch (Exception ex)
            {
                //Log.Error(ex, "获取GNS信息错误,接收语句:" + strData);
                return 0;
            }
        }

        /// <summary>
        /// VTG 航向航速
        /// </summary>
        /// <param name="strData"></param>
        private async Task<int> DataVTGAnalysis(string strData, CreateSentenceDto sentenceDto)
        {
            var result = 0;
            try
            {
                string[] strVTGInfo = strData.Split(',');
                if (strVTGInfo[9].ToUpper() == "N")
                {
                    return 0;
                }
                string guid = Guid.NewGuid().ToString();
                StringBuilder sbSql = new StringBuilder();
                sbSql.Append("insert into vdr_vtg (create_time,sentenceid,type,grdcoztrue,grdcozmag,grdspdknot,grdspdkm)" +
                    $" values (SYSDATE(),'{guid}','{strVTGInfo[0].Trim('$')}','{strVTGInfo[2] + " " + strVTGInfo[1]}','{strVTGInfo[4] + " " + strVTGInfo[3]}','{strVTGInfo[5] + " " + strVTGInfo[6]}','{strVTGInfo[7] + " " + strVTGInfo[8]}')");
                result = await AddAsync(sbSql.ToString());
                if (result > 0)
                {
                    /*sentenceDto.sentenceid = guid;
                    sentenceDto.isdecoded = 1;*/
                }
                result = await _sentenceService.CreateAsync(sentenceDto) == null ? 0 : 1;
                return result;
            }
            catch (Exception ex)
            {
                //Log.Error(ex, "获取VTG信息错误,接收语句:" + strData);
                return 0;
            }
        }

        /// <summary>
        /// TRD
        /// </summary>
        /// <param name="strData"></param>
        private async Task<int> DataTRDAnalysis(string strData, CreateSentenceDto sentenceDto)
        {
            var result = 0;
            try
            {
                string[] strTRDInfo = strData.Split(',');
                string guid = Guid.NewGuid().ToString();
                StringBuilder sbSql = new StringBuilder();
                sbSql.Append("insert into vdr_trd (create_time,sentenceid,type,number,rpmresponse,rpmindicator,pitchresponse,pitchindicator,azimuth)" +
                    $" values (SYSDATE(),'{guid}','{strTRDInfo[0].Trim('$')}','{strTRDInfo[1]}','{strTRDInfo[2]}','{strTRDInfo[3]}','{strTRDInfo[4]}','{strTRDInfo[5]}','{strTRDInfo[6]}')");
                result = await AddAsync(sbSql.ToString());
                if (result > 0)
                {
                    /*sentenceDto.sentenceid = guid;
                    sentenceDto.isdecoded = 1;*/
                }
                result = await _sentenceService.CreateAsync(sentenceDto) == null ? 0 : 1;
                return result;
            }
            catch (Exception ex)
            {
                //Log.Error(ex, "获取TRD信息错误,接收语句:" + strData);
                return 0;
            }
        }

        /// <summary>
        /// TRC
        /// </summary>
        /// <param name="strData"></param>
        private async Task<int> DataTRCAnalysis(string strData, CreateSentenceDto sentenceDto)
        {
            var result = 0;
            try
            {
                string[] strTRCInfo = strData.Split(',');
                if (strTRCInfo[8].ToLower() == "C")
                    //C是命令控制语句 不解析
                    return 0;
                string guid = Guid.NewGuid().ToString();
                StringBuilder sbSql = new StringBuilder();
                sbSql.Append("insert into vdr_trc (create_time,sentenceid,type,number,rpmdemand,rpmindicator,pitchdemand,pitchindicator,azimuth,oli)" +
                    $" values (SYSDATE(),'{guid}','{strTRCInfo[0].Trim('$')}','{strTRCInfo[1]}','{strTRCInfo[2]}','{strTRCInfo[3]}','{strTRCInfo[4]}','{strTRCInfo[5]}','{strTRCInfo[6]}','{strTRCInfo[7]}')");
                result = await AddAsync(sbSql.ToString());
                if (result > 0)
                {
                    /*sentenceDto.sentenceid = guid;
                    sentenceDto.isdecoded = 1;*/
                }
                result = await _sentenceService.CreateAsync(sentenceDto) == null ? 0 : 1;
                return result;
            }
            catch (Exception ex)
            {
                //Log.Error(ex, "获取TRC信息错误,接收语句:" + strData);
                return 0;
            }
        }

        /// <summary>
        /// RPM 转速
        /// </summary>
        /// <param name="strData"></param>
        private async Task<int> DataRPMAnalysis(string strData, CreateSentenceDto sentenceDto)
        {
            var result = 0;
            try
            {
                string[] strRPMInfo = strData.Split(',');
                if (strRPMInfo[5].ToLower() == "V")
                    //数据状态不正常
                    return 0;
                string guid = Guid.NewGuid().ToString();
                StringBuilder sbSql = new StringBuilder();
                sbSql.Append("insert into vdr_rpm (create_time,sentenceid,type,source,number,speed,propellerpitch)" +
                    $" values (SYSDATE(),'{guid}','{strRPMInfo[0].Trim('$')}','{strRPMInfo[1]}','{strRPMInfo[2]}','{strRPMInfo[3]}','{strRPMInfo[4]}')");
                result = await AddAsync(sbSql.ToString());
                if (result > 0)
                {
                    /*sentenceDto.sentenceid = guid;
                    sentenceDto.isdecoded = 1;*/
                }
                result = await _sentenceService.CreateAsync(sentenceDto) == null ? 0 : 1;
                return result;
            }
            catch (Exception ex)
            {
                //Log.Error(ex, "获取RPM信息错误,接收语句:" + strData);
                return 0;
            }
        }

        /// <summary>
        /// RMC 定位信息
        /// </summary>
        /// <param name="strData"></param>
        private async Task<int> DataRMCAnalysis(string strData, CreateSentenceDto sentenceDto)
        {
            var result = 0;
            try
            {
                string[] strRMCInfo = strData.Split(',');
                if (strRMCInfo[2].ToLower() == "V")
                    //数据状态不正常
                    return 0;
                string guid = Guid.NewGuid().ToString();
                StringBuilder sbSql = new StringBuilder();
                sbSql.Append("insert into vdr_rmc (create_time,sentenceid,type,rmcdatetime,latitude,longtitude,grdspeed,grdcoz,magvar)" +
                    $" values (SYSDATE(),'{guid}','{strRMCInfo[0].Trim('$')}','{strRMCInfo[9] + " " + strRMCInfo[1]}','{TLatLong(strRMCInfo[4] + " " + strRMCInfo[3])}','{TLatLong(strRMCInfo[6] + " " + strRMCInfo[5])}','{strRMCInfo[7]}','{strRMCInfo[8]}','{strRMCInfo[11] + " " + strRMCInfo[10]}')");
                result = await AddAsync(sbSql.ToString());
                if (result > 0)
                {
                    /*sentenceDto.sentenceid = guid;
                    sentenceDto.isdecoded = 1;*/
                }
                result = await _sentenceService.CreateAsync(sentenceDto) == null ? 0 : 1;
                return result;
            }
            catch (Exception ex)
            {
                //Log.Error(ex, "获取RMC信息错误,接收语句:" + strData);
                return 0;
            }
        }

        #endregion 设备分类解析

        #region 流量计相关数据解析

        /// <summary>
        /// CAN报文数据类型分割
        /// </summary>
        /// <param name="str1"></param>
        /// <returns></returns>
        public string GetSplitStr(string str1)
        {
            string str = "";
            string str2 = " ";
            for (int i = 0; i < str1.Length; i++)
            {
                if (i % 2 != 0)
                {
                    str += (str1[i] + str2);
                }
                else
                {
                    str += str1[i];
                }
            }
            return str;
        }

        /// <summary>
        /// 根据表格获取can数据
        /// </summary>
        /// <param name="zid">帧ID</param>
        private void CanGetAI(string zid, string[] candata, string vdrId)
        {
            dynamic tempEntity = new ExpandoObject();
            //根据帧ID获取相应的值
            if (zid == "1888C481" || zid == "1888E481" || zid == "18890481")
            {
                //主机
                switch (zid)
                {
                    case "1888C481":
                        var meTemp = BytesToSingle("KROHNE", new List<string> { candata[4], candata[5], candata[6], candata[7] }); //k 减273.15
                        var meVelocity = Filter("MEVelocity", BytesToSingle("KROHNE", new List<string> { candata[0], candata[1], candata[2], candata[3] }), 12, vdrId); //kg/s
                        break;

                    case "1888E481":
                        var meDensity = BytesToSingle("KROHNE", new List<string> { candata[0], candata[1], candata[2], candata[3] }); //kg/m3
                        break;

                    case "18890481":
                        var meFCAcc = BytesToSingle("KROHNE", new List<string> { candata[0], candata[1], candata[2], candata[3] }); //kg
                        break;
                }
            }
            else if (zid == "18880461" || zid == "18882461")
            {
                //辅机进
                switch (zid)
                {
                    case "18880461":
                        var dgDensityIn = BytesToSingle("EMERSON", new List<string> { candata[4], candata[5], candata[6], candata[7] }); //kg/m3
                        var dgVelocityIn = Filter("DGVelocityIn", BytesToSingle("EMERSON", new List<string> { candata[0], candata[1], candata[2], candata[3] }), 12, vdrId); //kg/h
                        break;

                    case "18882461":
                        var dgFCACCIn = BytesToSingle("EMERSON", new List<string> { candata[4], candata[5], candata[6], candata[7] }); //kg
                        var dgTempIn = BytesToSingle("EMERSON", new List<string> { candata[0], candata[1], candata[2], candata[3] }); //℃
                        break;
                }
            }
            else if (zid == "18884461" || zid == "18886461")
            {
                //辅机出
                switch (zid)
                {
                    case "18884461":
                        var dgTempOut = BytesToSingle("EMERSON", new List<string> { candata[4], candata[5], candata[6], candata[7] }); //℃
                        var dgVelocityOut = Filter("DGVelocityOut", BytesToSingle("EMERSON", new List<string> { candata[0], candata[1], candata[2], candata[3] }), 12, vdrId); //kg/h
                        break;

                    case "18886461":
                        var dgFCACCOut = BytesToSingle("EMERSON", new List<string> { candata[4], candata[5], candata[6], candata[7] }); //kg
                        var dgDensityOut = BytesToSingle("EMERSON", new List<string> { candata[0], candata[1], candata[2], candata[3] }); //kg/m3
                        break;
                }
            }
            else if (zid == "18888461" || zid == "1888A461")
            {
                //辅机轻油进
                switch (zid)
                {
                    case "18888461":
                        var dgTempMgoIn = BytesToSingle("EMERSON", new List<string> { candata[4], candata[5], candata[6], candata[7] }); //℃
                        var dgVelocityMgoIn = Filter("DGVelocityMgoIn", BytesToSingle("EMERSON", new List<string> { candata[0], candata[1], candata[2], candata[3] }), 12, vdrId); //kg/h
                        break;

                    case "1888A461":
                        var dgFCACCMgoIn = BytesToSingle("EMERSON", new List<string> { candata[4], candata[5], candata[6], candata[7] }); //kg
                        var dgDensityMgoIn = BytesToSingle("EMERSON", new List<string> { candata[0], candata[1], candata[2], candata[3] }); //kg/m3
                        break;
                }
            }
            else if (zid == "18880481" || zid == "18882481" || zid == "18884481")
            {
                //锅炉进
                switch (zid)
                {
                    case "18880481":
                        var blrTempIn = BytesToSingle("KROHNE", new List<string> { candata[4], candata[5], candata[6], candata[7] }); //k 减273.15
                        var blrVelocityIn = Filter("BLRVelocityIn", BytesToSingle("KROHNE", new List<string> { candata[0], candata[1], candata[2], candata[3] }), 12, vdrId); //kg/s
                        break;

                    case "18882481":
                        var blrDensityIn = BytesToSingle("KROHNE", new List<string> { candata[0], candata[1], candata[2], candata[3] }); //kg/m3
                        break;

                    case "18884481":
                        var blrFCACCIn = BytesToSingle("KROHNE", new List<string> { candata[0], candata[1], candata[2], candata[3] }); //kg
                        break;
                }
            }
            else if (zid == "18886481" || zid == "18888481" || zid == "1888A481")
            {
                //锅炉轻油
                switch (zid)
                {
                    case "18886481":
                        var blrTempMgoIn = BytesToSingle("KROHNE", new List<string> { candata[4], candata[5], candata[6], candata[7] }); //k 减273.15
                        var blrVelocityMgoIn = Filter("BLRVelocityMgoIn", BytesToSingle("KROHNE", new List<string> { candata[0], candata[1], candata[2], candata[3] }), 12, vdrId); //kg/s
                        break;

                    case "18888481":
                        var blrDensityMgoIn = BytesToSingle("KROHNE", new List<string> { candata[0], candata[1], candata[2], candata[3] }); //kg/m3
                        break;

                    case "1888A481":
                        var blrFCACCMgoIn = BytesToSingle("KROHNE", new List<string> { candata[0], candata[1], candata[2], candata[3] }); //kg
                        break;
                }
            }
            else if (zid == "188804A1" || zid == "188824A1" || zid == "188844A1")
            {
                //轴功率仪、吃水监测
                switch (zid)
                {
                    case "188804A1":
                        int dg1 = Convert.ToInt32(candata[1] + candata[0], 16); //电机1功率 KW
                        int dg2 = Convert.ToInt32(candata[3] + candata[2], 16); //电机2功率 KW
                        int dg3 = Convert.ToInt32(candata[5] + candata[4], 16); //电机3功率 KW
                        float draftBow = Convert.ToInt32(candata[7] + candata[6], 16) / 100f; //船艏吃水 mm
                        break;

                    case "188824A1":
                        float draftPort = Convert.ToInt32(candata[1] + candata[0], 16) / 100f; //船艏吃水 mm
                        float draftStarboard = Convert.ToInt32(candata[3] + candata[2], 16) / 100f; //船艏吃水 mm
                        float draftAstern = Convert.ToInt32(candata[5] + candata[4], 16) / 100f; //船艏吃水 mm
                        int meRpm = Convert.ToInt32(candata[7] + candata[6], 16); //主机转速 rpm
                        break;

                    case "188844A1":
                        int meorque = Convert.ToInt32(candata[1] + candata[0], 16); //轴扭矩 KNm
                        int mePower = Convert.ToInt32(candata[3] + candata[2], 16); //主机功率 KW
                        break;
                }
            }
        }

        /// <summary>
        /// 滤波
        /// </summary>
        /// <param name="paramName">需要滤波的参数</param>
        /// <param name="tempValue">当前值</param>
        /// <param name="filterValue">阈值</param>
        /// <param name="vdrId">设备Id</param>
        /// <returns></returns>
        public double Filter(string paramName, double tempValue, double filterValue, string vdrId)
        {
            double result = 0.0;

            try
            {
                List<double> lstTemp = new List<double>();
                if (dictFilter.ContainsKey(vdrId + "_" + paramName))
                {
                    lstTemp = dictFilter[vdrId + "_" + paramName];
                }

                if (lstTemp.Count >= filterValue)
                {
                    lstTemp.Add(tempValue);
                    lstTemp.Remove(0);
                    result = lstTemp.Average();
                }
                else
                {
                    lstTemp.Add(tempValue);
                    result = tempValue;
                }

                if (!dictFilter.ContainsKey(vdrId + "_" + paramName))
                {
                    dictFilter.Add(vdrId + "_" + paramName, lstTemp);
                }
            }
            catch (Exception ex)
            {
                //Log.Error(ex, "滤波器计算错误=>" + paramName + ":" + tempValue);
            }

            return result;
        }

        private static Dictionary<string, List<double>> dictFilter = new Dictionary<string, List<double>>();

        /// <summary>
        /// 不同流量计解析方式不同
        /// </summary>
        /// <param name="FMType">流量计种类</param>
        /// <param name="nBytes">源数据</param>
        /// <returns></returns>
        private double BytesToSingle(string FMType, List<string> nBytes)
        {
            //值
            float value = 0;
            string[] sBytes = new string[4] { "0", "0", "0", "0" };

            switch (FMType)
            {
                case "YINUO":
                    sBytes[0] = nBytes[3];
                    sBytes[1] = nBytes[2];
                    sBytes[2] = nBytes[1];
                    sBytes[3] = nBytes[0];
                    break;

                case "KROHNE":
                    sBytes[0] = nBytes[0];
                    sBytes[1] = nBytes[1];
                    sBytes[2] = nBytes[2];
                    sBytes[3] = nBytes[3];
                    break;

                case "EMERSON":
                    sBytes[0] = nBytes[2];
                    sBytes[1] = nBytes[3];
                    sBytes[2] = nBytes[0];
                    sBytes[3] = nBytes[1];
                    break;

                case "OTHERS":
                    sBytes[0] = nBytes[0];
                    sBytes[1] = nBytes[1];
                    sBytes[2] = nBytes[2];
                    sBytes[3] = nBytes[3];
                    break;
            }
            UInt32 x = Convert.ToUInt32(sBytes[0] + sBytes[1] + sBytes[2] + sBytes[3], 16);//字符串转16进制32位无符号整数
            value = BitConverter.ToSingle(BitConverter.GetBytes(x), 0);//IEEE754 字节转换float

            return Math.Round(value, 2);
        }

        #endregion 流量计相关数据解析

        public void SetRealtimeValue(string vdrId, string propertyName, object propertyValue)
        {
            var tempEntity = StaticEntity.RealtimeVesselinfos[vdrId];
            var curValue = tempEntity.GetType().GetProperty(propertyName).GetValue(tempEntity);
            if (!StaticEntity.PropertyCounters.ContainsKey(vdrId))
                StaticEntity.PropertyCounters.Add(vdrId, new Dictionary<string, int>());
            if (curValue != null && propertyValue == null)
            {
                if (StaticEntity.PropertyCounters[vdrId].ContainsKey(propertyName))
                {
                    var count = StaticEntity.PropertyCounters[vdrId][propertyName] + 1;
                    if (count > 3)
                    {
                        StaticEntity.PropertyCounters[vdrId].Remove(propertyName);
                        StaticEntity.RealtimeVesselinfos[vdrId].GetType().GetProperty(propertyName).SetValue(tempEntity, propertyValue, null);
                    }
                    else
                        StaticEntity.PropertyCounters[vdrId][propertyName] = count;
                }
                else
                    StaticEntity.PropertyCounters[vdrId].Add(propertyName, 1);
            }
            else
            {
                StaticEntity.RealtimeVesselinfos[vdrId].GetType().GetProperty(propertyName).SetValue(tempEntity, propertyValue, null);
                if (StaticEntity.PropertyCounters[vdrId].ContainsKey(propertyName))
                    StaticEntity.PropertyCounters[vdrId].Remove(propertyName);
            }
        }

        #endregion 上传数据解析保存
    }
}