using Google.Protobuf.WellKnownTypes;
using hmt_energy_csharp.AnalyseStaticDatas;
using hmt_energy_csharp.Connections;
using hmt_energy_csharp.Protos;
using hmt_energy_csharp.Services;
using hmt_energy_csharp.VDRs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc;

namespace hmt_energy_csharp.Controllers;

public class HomeController : AbpController
{
    private readonly IVDRService _vdrService;
    private readonly IConnectionService _connection;

    public HomeController(IVDRService vdrService, IConnectionService connection)
    {
        _vdrService = vdrService;
        this._connection = connection;
    }

    public ActionResult Index()
    {
        return Redirect("~/swagger");
    }

    [HttpPost]
    public async Task<ActionResult> GetAnalyseResult([FromQuery] string strParams)
    {
        try
        {
            var time1 = DateTime.Now;

            JObject keyValues = (JObject)JsonConvert.DeserializeObject(strParams);

            var request = new AnalyseRequest();
            request.AnalyseType = keyValues["AnalyseType"].ToString();
            request.PropPitch = 5128.77f;
            request.Params = Value.Parser.ParseJson(strParams);

            var service = new VoyageDataService(_connection, _vdrService);
            var response = await service.Analyse(request, null);

            Debug.WriteLine(DateTime.Now - time1);
            return Content(response.ToJson());
        }
        catch (Exception ex)
        {
            var result = new
            {
                result = "",
                status = false,
                error_message = ex.Message
            };
            return Content(result.ToJson());
        }
    }

    [HttpPost]
    public ActionResult GetAnalyseSingle([FromQuery] string strParams)
    {
        try
        {
            var dictParams = JObject.Parse(strParams);
            var dictKey = dictParams.GetOrDefault("dictKey").ToString();

            var buffer = AnalyseStaticData.AnalyseRingBuffers[dictKey];
            var resultBytes = new List<byte>();
            while (!buffer.Empty())
            {
                resultBytes.Add(buffer.Pop());
            }
            var result = new
            {
                Result = Encoding.UTF8.GetString(resultBytes.ToArray()),
                Status = true,
                ErrorMessage = ""
            };
            return Content(result.ToJson());
        }
        catch (Exception ex)
        {
            var result = new
            {
                Result = "",
                Status = false,
                ErrorMessage = ex.Message
            };
            return Content(result.ToJson());
        }
    }

    public ActionResult EndAnalyseSingle([FromQuery] string strParams)
    {
        try
        {
            var dictParams = JObject.Parse(strParams);
            var dictKey = dictParams.GetOrDefault("dictKey").ToString();
            AnalyseStaticData.AnalyseRingBuffers.Remove(dictKey);
            var result = new
            {
                Result = "",
                Status = true,
                ErrorMessage = ""
            };
            return Content(result.ToJson());
        }
        catch (Exception ex)
        {
            var result = new
            {
                Result = "",
                Status = false,
                ErrorMessage = ex.Message
            };
            return Content(result.ToJson());
        }
    }

    [HttpPost]
    public async Task<ActionResult> GetDistribution([FromQuery] string strParams)
    {
        try
        {
            var time1 = DateTime.Now;

            JObject keyValues = (JObject)JsonConvert.DeserializeObject(strParams);

            var request = new DistributionRequest();
            request.AnalyseType = keyValues["AnalyseType"].ToString();
            request.PropPitch = 5128.77f;
            request.Params = Value.Parser.ParseJson(strParams);

            var service = new VoyageDataService(_connection, _vdrService);
            var response = await service.Distribution(request, null);

            Debug.WriteLine(DateTime.Now - time1);
            return Content(response.ToJson());
        }
        catch (Exception ex)
        {
            var result = new
            {
                result = "",
                status = false,
                error_message = ex.Message
            };
            return Content(result.ToJson());
        }
    }
}