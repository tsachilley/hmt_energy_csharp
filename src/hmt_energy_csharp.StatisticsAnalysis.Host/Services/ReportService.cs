using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using hmt_energy_csharp.Protos;
using hmt_energy_csharp.VesselInfos;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace hmt_energy_csharp.Services;

public class ReportService : Report.ReportBase
{
    private readonly ILogger<ReportService> _logger;
    private readonly IVesselInfoService _vesselInfoService;

    public ReportService(ILogger<ReportService> logger, IVesselInfoService vesselInfoService)
    {
        _logger = logger;
        _vesselInfoService = vesselInfoService;
    }

    public override async Task<NoonResponse> Noon(NoonRequest request, ServerCallContext context)
    {
        var response = new NoonResponse();
        try
        {
            if (string.IsNullOrWhiteSpace(request.Number))
            {
                response.ErrMessage = "需要指定船的设备号:number";
                return response;
            }
            if (string.IsNullOrWhiteSpace(request.Date.ToString()))
            {
                response.ErrMessage = "需要指定日期:date";
                return response;
            }
            if (string.IsNullOrWhiteSpace(request.DepartureTime.ToString()))
            {
                response.ErrMessage = "需要指定航次开始日期:departure_time";
                return response;
            }

            var dto = await _vesselInfoService.GetNoonData(request.Number, request.Date.ToDateTimeOffset(), request.DepartureTime.ToDateTimeOffset());
            var result = new
            {
                daily_distance = dto.Distance,
                daily_distance_water = dto.DistanceWater,
                daily_time = dto.Duration,
                daily_speed = dto.Speed,
                daily_speed_water = dto.SpeedWater,
                daily_merpm_avg = dto.MERpm,
                daily_slip = dto.Slip,
                total_distance = dto.DistanceTotally,
                total_time = dto.DurationTotally,
                total_speed = dto.SpeedTotally,
                do_consumption = dto.DOConsumption,
                fo_consumption = dto.FOConsumption,
                me_fuel_consumption = dto.MEFuelConsumption,
                ae_fuel_consumption = dto.DGFuelConsumption,
                blr_fuel_consumption = dto.BLRFuelConsumption,
                wind_speed = dto.WindSpeed,
                wind_directin = dto.WindDirection,
                longitude = dto.Longitude,
                latitude = dto.Latitude,
                course = dto.Course,
                bow_direction = "",
                speed = dto.Speed,
                ae_power = "",
                number_of_ae = "",
                sea_temperature = dto.SeaTemperature,
                wave_height = dto.WaveHeight,
                wave_direction = dto.WaveDirection,
                weather = dto.Weather,
                temperature = dto.Temperature,
                pressure = dto.Pressure,
                visibility = dto.Visibility
            };
            response.Result = Value.Parser.ParseJson(result.ToJson());
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Namespace + "_" + MethodBase.GetCurrentMethod().Name);
            throw new Exception("210001");
        }
    }

    public override async Task<WeekResponse> Week(WeekRequest request, ServerCallContext context)
    {
        var response = new WeekResponse();
        try
        {
            var queryParams = request.Parameters.ToJObject();
            if (!queryParams.ContainsKey("number") || string.IsNullOrWhiteSpace(queryParams["number"].ToString()))
            {
                response.ErrMessage = "设备号不能为空";
            }
            else
            {
                response.Result = Value.Parser.ParseJson((await _vesselInfoService.GetWeekBaseAsync(request.Parameters, request.Language)).ToJson());
            }
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Namespace + "_" + MethodBase.GetCurrentMethod().Name);
            throw new Exception("210001");
        }
    }

    public override async Task<MRVResponse> MRV(MRVRequest request, ServerCallContext context)
    {
        var response = new MRVResponse();
        var queryParams = request.Parameters.ToJObject();
        if (!queryParams.ContainsKey("number") || string.IsNullOrWhiteSpace(queryParams["number"].ToString()))
        {
            response.ErrMessage = "设备号不能为空";
        }
        else
        {
            try
            {
                var result = await _vesselInfoService.GetMRVAsync(request.Parameters);
                if (result == null)
                    result = new
                    {
                        hfoType = "123",
                        lfoType = "23",
                        chaiType = "154",
                        jiaType = "123",
                        yiType = "6",
                        bingType = "53",
                        dingType = "3",
                        tianType = "43",
                        totalCo2 = "",
                        totalCo2All = "",
                        totalCo2StartAll = "",
                        totalCo2EndAll = "",
                        totalCo2PortAll = "",
                        totalFulePeopleCo2 = "",
                        totalCargoCo2 = "",
                        fuleTypeCargo = "",
                        cargoCo2 = "",
                        fuleDynamic = "",

                        distanceTotal = "41",
                        ditincelceTotal = "",
                        timeTotal = "",
                        timelce = "",
                        cargozwTotal = "412",
                        secondParam1 = "",
                        averDensity = "",

                        oilDistance = "5",
                        oilTonDistance = "2",
                        co2Distance = "42",
                        co2TonDistance = "2",
                        secondParam2 = "",
                        oiff = "",
                        addition = "",
                    };
                response.Result = Value.Parser.ParseJson(result.ToJson());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Namespace + "_" + MethodBase.GetCurrentMethod().Name);
                throw new Exception("210001");
            }
        }
        return response;
    }
}