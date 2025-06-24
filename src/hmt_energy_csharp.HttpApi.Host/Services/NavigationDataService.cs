using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using hmt_energy_csharp.Protos;
using hmt_energy_csharp.VesselInfos;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace hmt_energy_csharp.Services;

public class NavigationDataService : NavigationData.NavigationDataBase
{
    private readonly ILogger<NavigationDataService> _logger;
    private readonly IVesselInfoService _vesselInfoService;

    public NavigationDataService(ILogger<NavigationDataService> logger, IVesselInfoService vesselInfoService)
    {
        _logger = logger;
        _vesselInfoService = vesselInfoService;
    }

    public override async Task<NavigationDataRealTimeSingleResponse> NavigationDataRealTimeSingle(NavigationDataRealTimeSingleRequest request, ServerCallContext context)
    {
        var resultResponse = new NavigationDataRealTimeSingleResponse();
        try
        {
            var result = new VesselInfo();
            if (StaticEntity.RealtimeVesselinfos.ContainsKey(request.Number))
            {
                result = StaticEntity.RealtimeVesselinfos[request.Number];
            }
            resultResponse.Result = Value.Parser.ParseJson(result.ToJson());
            return resultResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Namespace + "_" + MethodBase.GetCurrentMethod().Name);
            throw new Exception("210001");
        }
    }

    /// <summary>
    /// 船舶实时数据
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public override async Task<NavigationDataRealTimeResponse> NavigationDataRealTime(NavigationDataRealTimeRequest request, ServerCallContext context)
    {
        var resultResponse = new NavigationDataRealTimeResponse();
        var resultList = new List<object>();
        try
        {
            foreach (var number in request.Numbers)
            {
                if (StaticEntity.RealtimeVesselinfos.ContainsKey(number))
                {
                    resultList.Add(StaticEntity.RealtimeVesselinfos[number]);
                }
                else
                {
                    resultList.Add(new VesselInfo());
                }
            }
            resultResponse.Result = Value.Parser.ParseJson(resultList.ToJson());
            return resultResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Namespace + "_" + MethodBase.GetCurrentMethod().Name);
            throw new Exception("210001");
        }
    }

    /// <summary>
    /// 船舶数据查询
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public override async Task<NavigationDataQueryResponse> NavigationDataQuery(NavigationDataQueryRequest request, ServerCallContext context)
    {
        var resultResponse = new NavigationDataQueryResponse();
        try
        {
            if (string.IsNullOrWhiteSpace(request.Parameters))
                request.Parameters = "{}";
            if (request.Method == "list")
            {
                var result = await _vesselInfoService.GetListPage(request.Number, request.PageNumber, request.CountPerPage, request.Sorting, request.Asc, request.Parameters);
                var totalCount = await _vesselInfoService.GetTotalCount(request.Number, request.Parameters);
                var newresult = new
                {
                    result = result,
                    totalCount = totalCount
                };
                resultResponse.Result = Value.Parser.ParseJson(newresult.ToJson());
            }
            else if (request.Method == "chart")
            {
                var result = await _vesselInfoService.GetListChart(request.Number, request.Parameters);
                var history_currenttime = new List<DateTime>();
                var queryParams = request.Parameters.ToJObject();
                var resultDict = new Dictionary<string, List<string>>();
                var options = new List<string>();
                var interval = 10;
                if (queryParams.ContainsKey("Options") && !string.IsNullOrWhiteSpace(queryParams["Options"].ToString()))
                {
                    foreach (var code in queryParams["Options"].ToString().Split(','))
                    {
                        if (string.IsNullOrWhiteSpace(code))
                            continue;
                        options.Add(code);
                        resultDict.Add(code, new List<string>());
                    }
                }
                if (queryParams.ContainsKey("Interval") && !string.IsNullOrWhiteSpace(queryParams["Interval"].ToString()))
                {
                    try
                    {
                        interval = Convert.ToInt32(queryParams["Interval"].ToString());
                    }
                    catch (Exception ex)
                    {
                    }
                }
                resultDict.Add("history_currenttime", new List<string>());
                var tempDatetime = DateTime.Now;
                var i = 0;
                foreach (var item in result)
                {
                    if (i == 0)
                    {
                        tempDatetime = item.ReceiveDatetime;
                        i++;
                    }
                    else
                    {
                        if (item.ReceiveDatetime.AddSeconds(interval) < tempDatetime)
                        {
                            i++;
                            continue;
                        }
                        else
                        {
                            tempDatetime = item.ReceiveDatetime;
                            i++;
                        }
                    }
                    foreach (var strType in options)
                    {
                        switch (strType)
                        {
                            case "MESFOC":
                                resultDict[strType].Add((item.MESFOC ?? 0).ToString());
                                break;

                            case "MERpm":
                                resultDict[strType].Add((item.MERpm ?? 0).ToString());
                                break;

                            case "MEPower":
                                resultDict[strType].Add((item.MEPower ?? 0).ToString());
                                break;

                            case "FCPerNm":
                                resultDict[strType].Add(((item.MEHFOConsumption + item.DGHFOConsumption + item.BLRHFOConsumption) / item.WaterSpeed).ToString());
                                break;

                            case "Slip":
                                resultDict[strType].Add((item.Slip ?? 0).ToString());
                                break;

                            case "WaterSpeed":
                                resultDict[strType].Add((item.WaterSpeed ?? 0).ToString());
                                break;

                            case "GroundSpeed":
                                resultDict[strType].Add((item.GroundSpeed ?? 0).ToString());
                                break;

                            case "Heel":
                                resultDict[strType].Add((item.Heel ?? 0).ToString());
                                break;

                            case "Trim":
                                resultDict[strType].Add((item.Trim ?? 0).ToString());
                                break;

                            case "WindSpeed":
                                resultDict[strType].Add((item.WindSpeed ?? 0).ToString());
                                break;
                        }
                    }
                    resultDict["history_currenttime"].Add(item.ReceiveDatetime.ToString("yyyy-MM-dd HH:mm:ss"));
                }
                resultResponse.Result = Value.Parser.ParseJson(resultDict.ToJson());
            }
            else if (request.Method == "map")
            {
                var result = await _vesselInfoService.GetListMap(request.Number, request.Parameters);
                var lineList = new List<List<string>>();
                double? lastMEFC = 0f;
                double? meAcc = 0f;
                double? dgAcc = 0f;
                double? blrAcc = 0f;
                foreach (var item in result)
                {
                    var coor = GPSHelper.GetBdFrom84(new pointLatLon(Convert.ToDouble(item?.Latitude), Convert.ToDouble(item?.Longitude)));
                    if (lineList.Count == 0)
                    {
                        lineList.Add(new List<string> { (item?.MEHFOConsumption / item?.WaterSpeed).ToString() });
                        lineList[lineList.Count - 1].Add(coor.Lat + "," + coor.Lon);
                    }
                    else
                    {
                        if (item?.MEHFOConsumption / item?.WaterSpeed <= 1000 && lastMEFC <= 1000 || item?.MEHFOConsumption / item?.WaterSpeed > 1500 && lastMEFC > 1500 || item?.MEHFOConsumption / item?.WaterSpeed > 1000 && item?.MEHFOConsumption / item?.WaterSpeed <= 1500 && lastMEFC > 1000 && lastMEFC <= 1500)
                        {
                            lineList[lineList.Count - 1].Add(coor.Lat + "," + coor.Lon);
                        }
                        else
                        {
                            var temp = lineList[lineList.Count - 1][(lineList[lineList.Count - 1]).Count - 1];
                            lineList.Add(new List<string> { (item?.MEHFOConsumption / item?.WaterSpeed).ToString() });
                            lineList[lineList.Count - 1].Add(temp);
                            lineList[lineList.Count - 1].Add(coor.Lat + "," + coor.Lon);
                        }
                    }
                    lastMEFC = (item?.WaterSpeed ?? 0) != 0 ? (item?.MEHFOConsumption ?? 0 / item?.WaterSpeed) : lastMEFC;
                    meAcc += item?.MEHFOConsumption ?? 0;
                    dgAcc += item?.DGHFOConsumption ?? 0;
                    blrAcc += item?.BLRHFOConsumption ?? 0;
                }
                var newresult = new
                {
                    lineList = lineList,
                    meAcc = meAcc,
                    dgAcc = dgAcc,
                    blrAcc = blrAcc
                };
                resultResponse.Result = Value.Parser.ParseJson(newresult.ToJson());
            }
            return resultResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Namespace + "_" + MethodBase.GetCurrentMethod().Name);
            throw new Exception("210001");
        }
    }
}