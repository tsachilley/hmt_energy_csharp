using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using hmt_energy_csharp.Connections;
using hmt_energy_csharp.Protos;
using hmt_energy_csharp.VDRs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace hmt_energy_csharp.Services
{
    public class VoyageDataService : VoyageData.VoyageDataBase
    {
        private readonly IConnectionService _connectionService;
        private readonly IVDRService _vdrService;

        public VoyageDataService(IConnectionService connectionService, IVDRService vdrService)
        {
            _connectionService = connectionService;
            _vdrService = vdrService;
        }

        /// <summary>
        /// 船端数据接收并保存
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<ProcessResponse> Process(ProcessRequest request, ServerCallContext context)
        {
            var response = new ProcessResponse();

            switch (request.Sentence)
            {
                case "0":
                    response.Result = "握手";
                    break;

                case "1":
                    response.Result = "接收";
                    break;

                default:
                    response.Result = await SaveEntityAsync(request);
                    break;
            }
            return response;
        }

        /// <summary>
        /// 保存数据操作
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<string> SaveEntityAsync(ProcessRequest request)
        {
            string result = "failure";

            //var connectionHost = await _connectionService.GetByHost(request.Host);
            //await dataAnalysisHelper.DataAnalysisAsync("@", request.Sentence, _sentenceService, connectionHost.Result);
            try
            {
                var connection = await _connectionService.GetByHostAsync(request.Host);
                result = (await _vdrService.DataAnalysisAsync(request.Sentence, "@", connection.number)) > 0 ? "success" : "failure";

                return result;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public override async Task<RealTimeForCustomerResponse> RealTimeForCustomer(RealTimeForCustomerRequest request, ServerCallContext context)
        {
            Log.Information("RealTimeForCustomer:" + request.Numbers);
            var response = new RealTimeForCustomerResponse();
            var tempResult = new object();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var tempResultList = new List<object>();
            foreach (var vdrId in request.Numbers)
            {
                if (!string.IsNullOrWhiteSpace(vdrId))
                {
                    try
                    {
                        tempResult = new
                        {
                            number = vdrId,
                            data = await _vdrService.GetVoyageRealTimeAsync(vdrId, 5128.77f)
                        };
                        tempResultList.Add(tempResult);
                    }
                    catch (Exception ex)
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
                        tempResult = new
                        {
                            number = vdrId,
                            data = dptData
                        };
                        tempResultList.Add(tempResult);
                        continue;
                    }
                }
            }
            sw.Stop();
            Console.WriteLine("取数据:" + sw.Elapsed.ToString());
            response.Result = Value.Parser.ParseJson(tempResultList.ToJson());
            response.Code = "success";
            return response;
        }

        /// <summary>
        /// 获取船端实时数据
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<RealTimeResponse> RealTime(RealTimeRequest request, ServerCallContext context)
        {
            Log.Information("RealTimeForCustomer:" + request.VdrId);
            var response = new RealTimeResponse();

            try
            {
                if (string.IsNullOrWhiteSpace(request.VdrId))
                {
                    Log.Information("设备号不能为空:" + request.VdrId);
                    response.Code = "failure:设备号不能为空";
                }
                else
                {
                    var result = await _vdrService.GetVoyageRealTimeAsync(request.VdrId, 5128.77f);
                    response.Result = Value.Parser.ParseJson(result.ToJson());
                    response.Code = "success";
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "设备" + request.VdrId + "获取实时数据错误。");
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
                response.Result = Value.Parser.ParseJson(dptData.ToJson());
                response.Code = "success";
            }

            return response;
        }

        /// <summary>
        /// 根据条件获取船端历史数据
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<HistoryResponse> History(HistoryRequest request, ServerCallContext context)
        {
            var response = new HistoryResponse();

            try
            {
                var startdate = long.MinValue;
                if (!string.IsNullOrWhiteSpace(request.StartDate))
                    startdate = new DateTimeOffset(Convert.ToDateTime(request.StartDate)).ToUnixTimeSeconds();
                var enddate = long.MaxValue;
                if (!string.IsNullOrWhiteSpace(request.EndDate))
                    enddate = new DateTimeOffset(Convert.ToDateTime(request.EndDate)).ToUnixTimeSeconds();

                if (request.Method == "list")
                {
                    if (request.PageNum >= 0)
                    {
                        var resultList = await _vdrService.GetVoyageHistoryAsync(request.VdrId, request.PageNum, request.PageCount, request.Sorting, request.Asc, startdate, enddate);
                        var resultCount = await _vdrService.GetVoyageHistoryCountAsync(request.VdrId, startdate, enddate);

                        var result = new
                        {
                            data = resultList,
                            total = resultCount
                        };

                        response.Result = Value.Parser.ParseJson(result.ToJson());
                    }
                    else
                    {
                        var result = await _vdrService.GetVoyageHistoryAsync(request.VdrId, request.PageNum, request.PageCount, request.Sorting, request.Asc, startdate, enddate);
                        response.Result = Value.Parser.ParseJson(result.ToJson());
                    }
                }
                else if (request.Method == "chart")
                {
                    var result = await _vdrService.GetVoyageHistoryChartAsync(request.VdrId, request.PageNum, request.PageCount, request.Sorting, request.Asc, startdate, enddate, request.Parameters);
                    response.Result = Value.Parser.ParseJson(result.ToJson());
                }
                else if (request.Method == "map")
                {
                    var result = await _vdrService.GetVoyageHistoryMapAsync(request.VdrId, request.PageNum, request.PageCount, request.Sorting, request.Asc, startdate, enddate, request.Parameters);
                    response.Result = Value.Parser.ParseJson(result.ToJson());
                }
                response.Code = "success";
            }
            catch (Exception ex)
            {
                response.Code = $"failure:{ex.Message}";
            }

            return response;
        }

        /// <summary>
        /// 根据条件获取航行数据并进行数据拟合分析
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<AnalyseResponse> Analyse(AnalyseRequest request, ServerCallContext context)
        {
            var response = new AnalyseResponse();

            try
            {
                var time1 = DateTime.Now;

                JObject keyValues = (JObject)JsonConvert.DeserializeObject(JsonFormatter.Default.Format(request.Params));

                long? dateFrom = null;
                long? dateTo = null;
                long? dateTemp = 0;
                float? slipFrom = null;
                float? slipTo = null;
                float? slipTemp = 0;
                float? draftFrom = null;
                float? draftTo = null;
                float? draftTemp = 0;
                float? windSpdFrom = null;
                float? windSpdTo = null;
                float? windSpdTemp = 0;
                float? speedFrom = null;
                float? speedTo = null;
                float? speedTemp = 0;
                float? mefcFrom = null;
                float? mefcTo = null;
                float? mefcTemp = 0;
                float? powerFrom = null;
                float? powerTo = null;
                float? powerTemp = 0;
                float? windDirFrom = null;
                float? windDirTo = null;
                float? windDirTemp = 0;
                float? rpmFrom = null;
                float? rpmTo = null;
                float? rpmTemp = 0;

                if (!string.IsNullOrWhiteSpace(keyValues["DateFrom"].ToString()))
                    dateFrom = new DateTimeOffset(Convert.ToDateTime(keyValues["DateFrom"]).ToUniversalTime()).ToUnixTimeSeconds();
                if (!string.IsNullOrWhiteSpace(keyValues["DateTo"].ToString()))
                    dateTo = new DateTimeOffset(Convert.ToDateTime(keyValues["DateTo"]).ToUniversalTime()).ToUnixTimeSeconds();
                if (!string.IsNullOrWhiteSpace(keyValues["SlipFrom"].ToString()))
                    slipFrom = Convert.ToSingle(keyValues["SlipFrom"]);
                if (!string.IsNullOrWhiteSpace(keyValues["SlipTo"].ToString()))
                    slipTo = Convert.ToSingle(keyValues["SlipTo"]);
                if (!string.IsNullOrWhiteSpace(keyValues["DraftFrom"].ToString()))
                    draftFrom = Convert.ToSingle(keyValues["DraftFrom"]);
                if (!string.IsNullOrWhiteSpace(keyValues["DraftTo"].ToString()))
                    draftTo = Convert.ToSingle(keyValues["DraftTo"]);
                if (!string.IsNullOrWhiteSpace(keyValues["WindSpdFrom"].ToString()))
                    windSpdFrom = Convert.ToSingle(keyValues["WindSpdFrom"]);
                if (!string.IsNullOrWhiteSpace(keyValues["WindSpdTo"].ToString()))
                    windSpdTo = Convert.ToSingle(keyValues["WindSpdTo"]);
                if (!string.IsNullOrWhiteSpace(keyValues["SpeedFrom"].ToString()))
                    speedFrom = Convert.ToSingle(keyValues["SpeedFrom"]);
                if (!string.IsNullOrWhiteSpace(keyValues["SpeedTo"].ToString()))
                    speedTo = Convert.ToSingle(keyValues["SpeedTo"]);
                if (!string.IsNullOrWhiteSpace(keyValues["MEFCFrom"].ToString()))
                    mefcFrom = Convert.ToSingle(keyValues["MEFCFrom"]);
                if (!string.IsNullOrWhiteSpace(keyValues["MEFCTo"].ToString()))
                    mefcTo = Convert.ToSingle(keyValues["MEFCTo"]);
                if (!string.IsNullOrWhiteSpace(keyValues["PowerFrom"].ToString()))
                    powerFrom = Convert.ToSingle(keyValues["PowerFrom"]);
                if (!string.IsNullOrWhiteSpace(keyValues["PowerTo"].ToString()))
                    powerTo = Convert.ToSingle(keyValues["PowerTo"]);
                if (!string.IsNullOrWhiteSpace(keyValues["WindDirFrom"].ToString()))
                    windDirFrom = Convert.ToSingle(keyValues["WindDirFrom"]);
                if (!string.IsNullOrWhiteSpace(keyValues["WindDirTo"].ToString()))
                    windDirTo = Convert.ToSingle(keyValues["WindDirTo"]);
                if (!string.IsNullOrWhiteSpace(keyValues["RpmFrom"].ToString()))
                    rpmFrom = Convert.ToSingle(keyValues["RpmFrom"]);
                if (!string.IsNullOrWhiteSpace(keyValues["RpmTo"].ToString()))
                    rpmTo = Convert.ToSingle(keyValues["RpmTo"]);

                try
                {
                    //日期验证
                    if (dateFrom != null && dateTo != null && dateFrom > dateTo)
                    {
                        dateTemp = dateFrom;
                        dateFrom = dateTo;
                        dateTo = dateTemp;
                    }
                    //滑失比验证
                    if (slipFrom != null && slipTo != null && slipFrom > slipTo)
                    {
                        slipTemp = slipFrom;
                        slipFrom = slipTo;
                        slipTo = slipTemp;
                    }
                    //吃水验证
                    if (draftFrom != null && draftTo != null && draftFrom > draftTo)
                    {
                        draftTemp = draftFrom;
                        draftFrom = draftTo;
                        draftTo = draftTemp;
                    }
                    //风速验证
                    if (windSpdFrom != null && windSpdTo != null && windSpdFrom > windSpdTo)
                    {
                        windSpdTemp = windSpdFrom;
                        windSpdFrom = windSpdTo;
                        windSpdTo = windSpdTemp;
                    }
                    //船速验证
                    if (speedFrom != null && speedTo != null && speedFrom > speedTo)
                    {
                        speedTemp = speedFrom;
                        speedFrom = speedTo;
                        speedTo = speedTemp;
                    }
                    //主机能耗验证
                    if (mefcFrom != null && mefcTo != null && mefcFrom > mefcTo)
                    {
                        mefcTemp = mefcFrom;
                        mefcFrom = mefcTo;
                        mefcTo = mefcTemp;
                    }
                    //主机功率验证
                    if (powerFrom != null && powerTo != null && powerFrom > powerTo)
                    {
                        powerTemp = powerFrom;
                        powerFrom = powerTo;
                        powerTo = powerTemp;
                    }
                    //风向验证
                    if (windDirFrom != null && windDirTo != null && windDirFrom > windDirTo)
                    {
                        windDirTemp = windDirFrom;
                        windDirFrom = windDirTo;
                        windDirTo = windDirTemp;
                    }
                    //主机转速验证
                    if (rpmFrom != null && rpmTo != null && rpmFrom > rpmTo)
                    {
                        rpmTemp = rpmFrom;
                        rpmFrom = rpmTo;
                        rpmTo = rpmTemp;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                //获取查询结果
                /*var tasks = new List<Task<IEnumerable<VdrEntityDto>>>();
                tasks.Add(_vdrService.GetVoyageAnalyseAsync(Convert.ToInt32(keyValues["VdrId"]), dateFrom, dateFrom + (dateTo - dateFrom) / 3, slipFrom, slipTo, draftFrom, draftTo, request.AnalyseType, request.PropPitch));
                tasks.Add(_vdrService.GetVoyageAnalyseAsync(Convert.ToInt32(keyValues["VdrId"]), dateFrom + (dateTo - dateFrom) / 3, dateFrom + (dateTo - dateFrom) / 3 * 2, slipFrom, slipTo, draftFrom, draftTo, request.AnalyseType, request.PropPitch));
                tasks.Add(_vdrService.GetVoyageAnalyseAsync(Convert.ToInt32(keyValues["VdrId"]), dateFrom + (dateTo - dateFrom) / 3 * 2, dateTo, slipFrom, slipTo, draftFrom, draftTo, request.AnalyseType, request.PropPitch));
                var testResult = await tasks.WhenAll();
                List<VdrEntityDto> checkResult = new List<VdrEntityDto>();
                foreach (var tempResult in testResult)
                {
                    checkResult.Concat(tempResult.ToList());
                }*/
                //List<VdrEntityDto> checkResult = (List<VdrEntityDto>)await _vdrService.GetVoyageAnalyseAsync(Convert.ToInt32(keyValues["VdrId"]), dateFrom, dateTo, slipFrom, slipTo, draftFrom, draftTo, request.AnalyseType, request.PropPitch);

                string dictKey = await _vdrService.RequestVoyageAnalyseAsync(keyValues["VdrId"].ToString(), dateFrom, dateTo, slipFrom, slipTo, draftFrom, draftTo, windSpdFrom, windSpdTo, speedFrom, speedTo, mefcFrom, mefcTo, powerFrom, powerTo, windDirFrom, windDirTo, rpmFrom, rpmTo, request.AnalyseType, request.PropPitch);
                if (dictKey.Contains("failure"))
                {
                    response.Status = false;
                    response.ErrorMessage = dictKey;
                }
                else
                {
                    response.Status = true;
                    response.Result = dictKey;
                }

                /*//取随机或平均结果
                var subResult = new List<VdrEntityDto>();
                int sourceCount = checkResult.Count();
                for (var i = 0; i < sourceCount; i++)
                {
                    if (subResult.Any(t => t.history_vbw_watspd == checkResult[i].history_vbw_watspd))
                    {
                        continue;
                    }
                    else
                    {
                        int count = 1;
                        for (var j = i + 1; j < checkResult.Count(); j++)
                        {
                            if (checkResult[i].history_vbw_watspd != checkResult[j].history_vbw_watspd)
                            {
                                count++;
                            }
                            else
                            {
                                break;
                            }
                        }
                        int rowindex = i + new Random().Next(count);
                        subResult.Add(checkResult[i]);
                    }
                }*/

                /*var result = new
                {
                    data = subResult,
                    sourceCount = sourceCount
                };

                response.Result = Value.Parser.ParseJson(result.ToJson());
                response.Status = true;*/

                Debug.WriteLine(DateTime.Now - time1);
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.ErrorMessage = "failure:" + ex.Message;
            }

            return response;
        }

        /// <summary>
        /// 根据条件获取航行数据分布
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<DistributionResponse> Distribution(DistributionRequest request, ServerCallContext context)
        {
            var response = new DistributionResponse();

            try
            {
                JObject keyValues = (JObject)JsonConvert.DeserializeObject(JsonFormatter.Default.Format(request.Params));

                #region 参数

                long? dateFrom = null;
                long? dateTo = null;
                long? dateTemp = 0;
                float? slipFrom = null;
                float? slipTo = null;
                float? slipTemp = 0;
                float? draftFrom = null;
                float? draftTo = null;
                float? draftTemp = 0;
                float? windSpdFrom = null;
                float? windSpdTo = null;
                float? windSpdTemp = 0;
                float? speedFrom = null;
                float? speedTo = null;
                float? speedTemp = 0;
                float? mefcFrom = null;
                float? mefcTo = null;
                float? mefcTemp = 0;
                float? powerFrom = null;
                float? powerTo = null;
                float? powerTemp = 0;
                float? windDirFrom = null;
                float? windDirTo = null;
                float? windDirTemp = 0;
                float? rpmFrom = null;
                float? rpmTo = null;
                float? rpmTemp = 0;

                if (!string.IsNullOrWhiteSpace(keyValues["DateFrom"].ToString()))
                    dateFrom = new DateTimeOffset(Convert.ToDateTime(keyValues["DateFrom"]).ToUniversalTime()).ToUnixTimeSeconds();
                if (!string.IsNullOrWhiteSpace(keyValues["DateTo"].ToString()))
                    dateTo = new DateTimeOffset(Convert.ToDateTime(keyValues["DateTo"]).ToUniversalTime()).ToUnixTimeSeconds();
                if (!string.IsNullOrWhiteSpace(keyValues["SlipFrom"].ToString()))
                    slipFrom = Convert.ToSingle(keyValues["SlipFrom"]);
                if (!string.IsNullOrWhiteSpace(keyValues["SlipTo"].ToString()))
                    slipTo = Convert.ToSingle(keyValues["SlipTo"]);
                if (!string.IsNullOrWhiteSpace(keyValues["DraftFrom"].ToString()))
                    draftFrom = Convert.ToSingle(keyValues["DraftFrom"]);
                if (!string.IsNullOrWhiteSpace(keyValues["DraftTo"].ToString()))
                    draftTo = Convert.ToSingle(keyValues["DraftTo"]);
                if (!string.IsNullOrWhiteSpace(keyValues["WindSpdFrom"].ToString()))
                    windSpdFrom = Convert.ToSingle(keyValues["WindSpdFrom"]);
                if (!string.IsNullOrWhiteSpace(keyValues["WindSpdTo"].ToString()))
                    windSpdTo = Convert.ToSingle(keyValues["WindSpdTo"]);
                if (!string.IsNullOrWhiteSpace(keyValues["SpeedFrom"].ToString()))
                    speedFrom = Convert.ToSingle(keyValues["SpeedFrom"]);
                if (!string.IsNullOrWhiteSpace(keyValues["SpeedTo"].ToString()))
                    speedTo = Convert.ToSingle(keyValues["SpeedTo"]);
                if (!string.IsNullOrWhiteSpace(keyValues["MEFCFrom"].ToString()))
                    mefcFrom = Convert.ToSingle(keyValues["MEFCFrom"]);
                if (!string.IsNullOrWhiteSpace(keyValues["MEFCTo"].ToString()))
                    mefcTo = Convert.ToSingle(keyValues["MEFCTo"]);
                if (!string.IsNullOrWhiteSpace(keyValues["PowerFrom"].ToString()))
                    powerFrom = Convert.ToSingle(keyValues["PowerFrom"]);
                if (!string.IsNullOrWhiteSpace(keyValues["PowerTo"].ToString()))
                    powerTo = Convert.ToSingle(keyValues["PowerTo"]);
                if (!string.IsNullOrWhiteSpace(keyValues["WindDirFrom"].ToString()))
                    windDirFrom = Convert.ToSingle(keyValues["WindDirFrom"]);
                if (!string.IsNullOrWhiteSpace(keyValues["WindDirTo"].ToString()))
                    windDirTo = Convert.ToSingle(keyValues["WindDirTo"]);
                if (!string.IsNullOrWhiteSpace(keyValues["RpmFrom"].ToString()))
                    rpmFrom = Convert.ToSingle(keyValues["RpmFrom"]);
                if (!string.IsNullOrWhiteSpace(keyValues["RpmTo"].ToString()))
                    rpmTo = Convert.ToSingle(keyValues["RpmTo"]);

                try
                {
                    //日期验证
                    if (dateFrom != null && dateTo != null && dateFrom > dateTo)
                    {
                        dateTemp = dateFrom;
                        dateFrom = dateTo;
                        dateTo = dateTemp;
                    }
                    //滑失比验证
                    if (slipFrom != null && slipTo != null && slipFrom > slipTo)
                    {
                        slipTemp = slipFrom;
                        slipFrom = slipTo;
                        slipTo = slipTemp;
                    }
                    //吃水验证
                    if (draftFrom != null && draftTo != null && draftFrom > draftTo)
                    {
                        draftTemp = draftFrom;
                        draftFrom = draftTo;
                        draftTo = draftTemp;
                    }
                    //风速验证
                    if (windSpdFrom != null && windSpdTo != null && windSpdFrom > windSpdTo)
                    {
                        windSpdTemp = windSpdFrom;
                        windSpdFrom = windSpdTo;
                        windSpdTo = windSpdTemp;
                    }
                    //船速验证
                    if (speedFrom != null && speedTo != null && speedFrom > speedTo)
                    {
                        speedTemp = speedFrom;
                        speedFrom = speedTo;
                        speedTo = speedTemp;
                    }
                    //主机能耗验证
                    if (mefcFrom != null && mefcTo != null && mefcFrom > mefcTo)
                    {
                        mefcTemp = mefcFrom;
                        mefcFrom = mefcTo;
                        mefcTo = mefcTemp;
                    }
                    //主机功率验证
                    if (powerFrom != null && powerTo != null && powerFrom > powerTo)
                    {
                        powerTemp = powerFrom;
                        powerFrom = powerTo;
                        powerTo = powerTemp;
                    }
                    //风向验证
                    if (windDirFrom != null && windDirTo != null && windDirFrom > windDirTo)
                    {
                        windDirTemp = windDirFrom;
                        windDirFrom = windDirTo;
                        windDirTo = windDirTemp;
                    }
                    //主机转速验证
                    if (rpmFrom != null && rpmTo != null && rpmFrom > rpmTo)
                    {
                        rpmTemp = rpmFrom;
                        rpmFrom = rpmTo;
                        rpmTo = rpmTemp;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                #endregion 参数

                var result = await _vdrService.RequestVoyageDistributionAsync(keyValues["VdrId"].ToString(), dateFrom, dateTo, slipFrom, slipTo, draftFrom, draftTo, windSpdFrom, windSpdTo, speedFrom, speedTo, mefcFrom, mefcTo, powerFrom, powerTo, windDirFrom, windDirTo, rpmFrom, rpmTo, request.AnalyseType, request.PropPitch);

                response.Status = true;
                response.Result = result.ToJson();
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.ErrorMessage = ex.Message;
            }

            return response;
        }
    }
}