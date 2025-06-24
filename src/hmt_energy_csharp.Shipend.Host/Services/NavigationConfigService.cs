using Castle.Core.Logging;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using hmt_energy_csharp.Energy.Configs;
using hmt_energy_csharp.Protos;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace hmt_energy_csharp.Services
{
    public class NavigationConfigService : NavigationConfig.NavigationConfigBase
    {
        private readonly ILogger<NavigationConfigService> _logger;
        private readonly IConfigService _configService;

        public NavigationConfigService(ILogger<NavigationConfigService> logger, IConfigService configService)
        {
            _logger = logger;
            _configService = configService;
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
    }
}