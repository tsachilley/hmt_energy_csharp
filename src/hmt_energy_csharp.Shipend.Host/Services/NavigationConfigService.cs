using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using hmt_energy_csharp.Energy.Configs;
using hmt_energy_csharp.Protos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace hmt_energy_csharp.Services
{
    public class NavigationConfigService : NavigationConfig.NavigationConfigBase
    {
        private readonly ILogger<NavigationConfigService> _logger;
        private readonly IConfigService _configService;
        private readonly IConfiguration _configuration;

        public NavigationConfigService(ILogger<NavigationConfigService> logger, IConfigService configService, IConfiguration configuration)
        {
            _logger = logger;
            _configService = configService;
            _configuration = configuration;
        }

        public override async Task<InsertResponse> Insert(InsertRequest request, ServerCallContext context)
        {
            var response = new InsertResponse();
            try
            {
                var dto = request.Dto.ToString().ToObject<ConfigDto>();
                dto.create_time = DateTime.Now;
                await _configService.Add(dto);
                response.Result = 1;
                StaticEntities.StaticEntities.Configs.Clear();
                StaticEntities.StaticEntities.Configs = await _configService.GetList("{\"IsEnabled\":\"1\"}");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Namespace + "_" + MethodBase.GetCurrentMethod().Name);
                throw new Exception("210001");
            }
        }

        public override async Task<DeleteResponse> Delete(DeleteRequest request, ServerCallContext context)
        {
            var response = new DeleteResponse();
            try
            {
                var dto = request.Dto.ToString().ToObject<ConfigDto>();
                await _configService.Delete(Convert.ToInt32(dto.Id));
                response.Result = 1;
                StaticEntities.StaticEntities.Configs.Clear();
                StaticEntities.StaticEntities.Configs = await _configService.GetList("{\"IsEnabled\":\"1\"}");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Namespace + "_" + MethodBase.GetCurrentMethod().Name);
                throw new Exception("210001");
            }
        }

        public override async Task<UpdateResponse> Update(UpdateRequest request, ServerCallContext context)
        {
            var response = new UpdateResponse();
            try
            {
                var dto = request.Dto.ToString().ToObject<ConfigDto>();
                dto.update_time = DateTime.Now;
                await _configService.Update(Convert.ToInt32(dto.Id), dto);
                response.Result = 1;
                StaticEntities.StaticEntities.Configs.Clear();
                StaticEntities.StaticEntities.Configs = await _configService.GetList("{\"IsEnabled\":\"1\"}");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Namespace + "_" + MethodBase.GetCurrentMethod().Name);
                throw new Exception("210001");
            }
        }

        public override async Task<SelectOneResponse> SelectOne(SelectOneRequest request, ServerCallContext context)
        {
            var response = new SelectOneResponse();
            var parameters = request.Parameters.ToJObject();
            if (!parameters.ContainsKey("Id") || string.IsNullOrWhiteSpace(parameters.ContainsKey("Id").ToString()))
                throw new Exception("101005");
            var dto = new ConfigDto();
            dto.Id = Convert.ToInt32(parameters.ContainsKey("Id"));
            try
            {
                var result = await _configService.Get(dto);
                response.Result = Value.Parser.ParseJson(result.ToJson());
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Namespace + "_" + MethodBase.GetCurrentMethod().Name);
                throw new Exception("210001");
            }
        }

        public override async Task<SelectListResponse> SelectList(SelectListRequest request, ServerCallContext context)
        {
            var response = new SelectListResponse();
            try
            {
                var result = await _configService.GetPage(request.PageNumber, request.CountPerPage, request.Sorting, request.Asc, request.Parameters);
                response.Result = Value.Parser.ParseJson(result.ToJson());
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Namespace + "_" + MethodBase.GetCurrentMethod().Name);
                throw new Exception("210001");
            }
        }

        public override async Task<SelectManualParamsResponse> SelectManualParams(SelectManualParamsRequest request, ServerCallContext context)
        {
            var response = new SelectManualParamsResponse();
            try
            {
                var result = await _configService.GetList(new { IsDevice = 2, IsEnabled = 1, Number = _configuration["ShipInfo:SN"] ?? "SAD1" }.ToJson());
                StringBuilder returnResult = new();
                foreach (var entity in result)
                {
                    if (entity.Code == "ManualDraft")
                        returnResult.Append("\"" + entity.Code + "\":" + entity.HighLimit.ToString() + ",");
                    else if (entity.Code == "ManualBLRFC")
                        returnResult.Append("\"" + entity.Code + "\":" + entity.HighLimit.ToString() + ",");
                    else if (entity.Code == "ManualRunoff")
                        returnResult.Append("\"" + entity.Code + "\":" + entity.HighLimit.ToString() + ",");
                    else if (entity.Code == "ManualTidal")
                        returnResult.Append("\"" + entity.Code + "\":" + entity.HighLimit.ToString() + ",");
                }
                returnResult.Remove(returnResult.Length - 1, 1).Insert(0, '{').Append('}');
                response.Result = Value.Parser.ParseJson(returnResult.ToString());
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Namespace + "_" + MethodBase.GetCurrentMethod().Name);
                response.ErrMessage = "210001";
                return response;
            }
        }

        public override async Task<UpdateManualParamsResponse> UpdateManualParams(UpdateManualParamsRequest request, ServerCallContext context)
        {
            var response = new UpdateManualParamsResponse();
            try
            {
                var ManualParams = request.Parameters.ToJObject();
                var lst = await _configService.GetList(new { IsDevice = 2, IsEnabled = 1, Number = _configuration["ShipInfo:SN"] ?? "SAD1" }.ToJson());
                try
                {
                    foreach (var ManualParam in ManualParams)
                    {
                        try
                        {
                            if (lst.Any(t => t.Code == ManualParam.Key))
                            {
                                var dto = lst.FirstOrDefault(t => t.Code == ManualParam.Key);
                                dto.HighLimit = Convert.ToDecimal(ManualParam.Value);
                                dto.update_time = DateTime.UtcNow;
                                await _configService.Update(Convert.ToInt32(dto.Id), dto);
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex, "{Namespace}_{MethodName}", MethodBase.GetCurrentMethod()?.DeclaringType?.Namespace + "_" + ManualParam.Key, MethodBase.GetCurrentMethod()?.Name);
                        }
                    }

                    StaticEntities.StaticEntities.Configs.Clear();

                    response.Result = 1;
                }
                catch (Exception ex)
                {
                    response.Result = 0;
                    _logger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Namespace + "_" + MethodBase.GetCurrentMethod().Name);
                    response.ErrMessage = "210001";
                }
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Namespace + "_" + MethodBase.GetCurrentMethod().Name);
                response.ErrMessage = "210001";
                return response;
            }
        }
    }
}