using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using hmt_energy_csharp.Protos;
using hmt_energy_csharp.VesselInfos;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace hmt_energy_csharp.Services
{
    public class StatisticsAnalysisService : StatisticsAnalysis.StatisticsAnalysisBase
    {
        private readonly ILogger<StatisticsAnalysisService> _logger;
        private readonly IVesselInfoService _vesselInfoService;

        public StatisticsAnalysisService(ILogger<StatisticsAnalysisService> logger, IVesselInfoService vesselInfoService)
        {
            _logger = logger;
            _vesselInfoService = vesselInfoService;
        }

        public override async Task<StatisticsResponse> Statistics(StatisticsRequest request, ServerCallContext context)
        {
            var response = new StatisticsResponse();
            try
            {
                if (request.Category == "WindDirDistribution")
                {
                    var result = await _vesselInfoService.GetWindDirDistribution(request.Parameters);
                    response.Result = Value.Parser.ParseJson(result.ToJson());
                }
                else if (request.Category == "SlipDistribution")
                {
                    var result = await _vesselInfoService.GetSlipDistribution(request.Parameters);
                    response.Result = Value.Parser.ParseJson(result.ToJson());
                }
                else if (request.Category == "PowerDistribution")
                {
                    var result = await _vesselInfoService.GetPowerDistribution(request.Parameters);
                    response.Result = Value.Parser.ParseJson(result.ToJson());
                }
                else if (request.Category == "SpeedDistribution")
                {
                    var result = await _vesselInfoService.GetSpeedDistribution(request.Parameters);
                    response.Result = Value.Parser.ParseJson(result.ToJson());
                }
                else if (request.Category == "DraftDistribution")
                {
                    var result = await _vesselInfoService.GetDraftDistribution(request.Parameters);
                    response.Result = Value.Parser.ParseJson(result.ToJson());
                }
                else if (request.Category == "WindSpeedDistribution")
                {
                    var result = await _vesselInfoService.GetWindSpeedDistribution(request.Parameters);
                    response.Result = Value.Parser.ParseJson(result.ToJson());
                }
                else
                {
                    var result = await _vesselInfoService.GetStatisticList(request.Parameters);
                    response.Result = Value.Parser.ParseJson(result.ToJson());
                }
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Namespace + "_" + MethodBase.GetCurrentMethod().Name);
                throw new Exception("210001");
            }
        }

        public override async Task<AnalysisResponse> Analysis(AnalysisRequest request, ServerCallContext context)
        {
            var response = new AnalysisResponse();
            try
            {
                if (request.Category == "Speed")
                {
                    var result = await _vesselInfoService.GetSpeedList(request.Parameters);
                    response.Result = Value.Parser.ParseJson(result.ToJson());
                }
                else if (request.Category == "VFCSpd")
                {
                    var result = await _vesselInfoService.GetVFCSpdList(request.Parameters);
                    response.Result = Value.Parser.ParseJson(result.ToJson());
                }
                else if (request.Category == "MEFCPow")
                {
                    var result = await _vesselInfoService.GetMEFCPowList(request.Parameters);
                    response.Result = Value.Parser.ParseJson(result.ToJson());
                }
                else if (request.Category == "Trim")
                {
                    var result = await _vesselInfoService.GetTrimList(request.Parameters);
                    response.Result = Value.Parser.ParseJson(result.ToJson());
                }
                else if (request.Category == "VFCMESpd")
                {
                    var result = await _vesselInfoService.GetVFCMESpdList(request.Parameters);
                    response.Result = Value.Parser.ParseJson(result.ToJson());
                }
                else if (request.Category == "MESpdProp")
                {
                    var result = await _vesselInfoService.GetMESpdPropList(request.Parameters);
                    response.Result = Value.Parser.ParseJson(result.ToJson());
                }
                else if (request.Category == "PowSpd")
                {
                    var result = await _vesselInfoService.GetPowSpdList(request.Parameters);
                    response.Result = Value.Parser.ParseJson(result.ToJson());
                }
                else if (request.Category == "PowRpm")
                {
                    var result = await _vesselInfoService.GetPowRpmList(request.Parameters);
                    response.Result = Value.Parser.ParseJson(result.ToJson());
                }
                else if (request.Category == "Hull")
                {
                    var result = await _vesselInfoService.GetHullList(request.Parameters);
                    response.Result = Value.Parser.ParseJson(result.ToJson());
                }
                else if (request.Category == "METuning")
                {
                    var result = await _vesselInfoService.GetMETuningList(request.Parameters);
                    response.Result = Value.Parser.ParseJson(result.ToJson());
                }
                else if (request.Category == "HullPropeller")
                {
                    var result = await _vesselInfoService.GetHullPropellerList(request.Parameters);
                    response.Result = Value.Parser.ParseJson(result.ToJson());
                }
                else if (request.Category == "MELoadProp")
                {
                    var result = await _vesselInfoService.GetMELoadPropList(request.Parameters);
                    response.Result = Value.Parser.ParseJson(result.ToJson());
                }
                else
                {
                    var result = await _vesselInfoService.GetAnalysisList(request.Parameters);
                    response.Result = Value.Parser.ParseJson(result.ToJson());
                }
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Namespace + "_" + MethodBase.GetCurrentMethod().Name);
                throw new Exception("210001:" + ex.Message);
            }
        }
    }
}