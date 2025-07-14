using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using hmt_energy_csharp.Dtos;
using hmt_energy_csharp.Energy.Batteries;
using hmt_energy_csharp.Energy.Criterias;
using hmt_energy_csharp.Energy.Flowmeters;
using hmt_energy_csharp.Energy.Generators;
using hmt_energy_csharp.Energy.LiquidLevels;
using hmt_energy_csharp.Energy.Shafts;
using hmt_energy_csharp.Energy.SternSealings;
using hmt_energy_csharp.Energy.SupplyUnits;
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
using hmt_energy_csharp.Protos;
using hmt_energy_csharp.VesselInfos;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace hmt_energy_csharp.Services;

public class NavigationDataService : NavigationData.NavigationDataBase
{
    private readonly ILogger<NavigationDataService> _logger;
    private readonly IVesselInfoService _vesselInfoService;
    private readonly IConsulService _consulService;

    public NavigationDataService(ILogger<NavigationDataService> logger, IVesselInfoService vesselInfoService, IConsulService consulService)
    {
        _logger = logger;
        _vesselInfoService = vesselInfoService;
        _consulService = consulService;
    }

    public override async Task<NavigationDataRealTimeSingleResponse> NavigationDataRealTimeSingle(NavigationDataRealTimeSingleRequest request, ServerCallContext context)
    {
        var resultResponse = new NavigationDataRealTimeSingleResponse();
        try
        {
            var result = new object();
            if (StaticEntities.ShowEntities.Vessels.Any(t => t.SN == request.Number))
            {
                switch (request.Type)
                {
                    case "generator":
                        var gDtos = StaticEntities.ShowEntities.Generators.FirstOrDefault(t => t.Number == request.Number).GeneratorDtos;
                        foreach (var dto in gDtos)
                        {
                            foreach (var prop in dto.GetType().GetProperties())
                            {
                                if (prop.PropertyType == typeof(double) ||
                                    prop.PropertyType == typeof(float) ||
                                    prop.PropertyType == typeof(decimal) ||
                                    prop.PropertyType == typeof(double?) ||
                                    prop.PropertyType == typeof(float?) ||
                                    prop.PropertyType == typeof(decimal?))
                                    prop.SetValue(dto, (decimal?)Math.Round(Convert.ToDouble(prop.GetValue(dto)), 4));
                            }
                        }
                        result = gDtos;
                        break;

                    case "shaft":
                        var shafts = StaticEntities.ShowEntities.Shafts.FirstOrDefault(t => t.Number == request.Number).ShaftDtos;
                        foreach (var dto in shafts)
                        {
                            foreach (var prop in dto.GetType().GetProperties())
                            {
                                if (prop.PropertyType == typeof(double) ||
                                    prop.PropertyType == typeof(float) ||
                                    prop.PropertyType == typeof(decimal) ||
                                    prop.PropertyType == typeof(double?) ||
                                    prop.PropertyType == typeof(float?) ||
                                    prop.PropertyType == typeof(decimal?))
                                    prop.SetValue(dto, (decimal?)Math.Round(Convert.ToDouble(prop.GetValue(dto)), 4));
                            }
                        }
                        var stern = StaticEntities.ShowEntities.SternSealings.FirstOrDefault(t => t.Number == request.Number).SternSealingDtos;
                        foreach (var dto in stern)
                        {
                            foreach (var prop in dto.GetType().GetProperties())
                            {
                                if (prop.PropertyType == typeof(double) ||
                                    prop.PropertyType == typeof(float) ||
                                    prop.PropertyType == typeof(decimal) ||
                                    prop.PropertyType == typeof(double?) ||
                                    prop.PropertyType == typeof(float?) ||
                                    prop.PropertyType == typeof(decimal?))
                                    prop.SetValue(dto, (decimal?)Math.Round(Convert.ToDouble(prop.GetValue(dto)), 4));
                            }
                        }
                        result = new
                        {
                            sternSealings = stern,
                            shafts = shafts
                        };
                        break;

                    case "liquid":
                        var llDtos = StaticEntities.ShowEntities.LiquidLevels.FirstOrDefault(t => t.Number == request.Number).LiquidLevelDtos;
                        foreach (var dto in llDtos)
                        {
                            foreach (var prop in dto.GetType().GetProperties())
                            {
                                if (prop.PropertyType == typeof(double) ||
                                    prop.PropertyType == typeof(float) ||
                                    prop.PropertyType == typeof(decimal) ||
                                    prop.PropertyType == typeof(double?) ||
                                    prop.PropertyType == typeof(float?) ||
                                    prop.PropertyType == typeof(decimal?))
                                    prop.SetValue(dto, (decimal?)Math.Round(Convert.ToDouble(prop.GetValue(dto)), 4));
                            }
                        }
                        result = llDtos;
                        break;

                    case "supply":
                        var suDtps = StaticEntities.ShowEntities.SupplyUnits.FirstOrDefault(t => t.Number == request.Number).SupplyUnitDtos;
                        foreach (var dto in suDtps)
                        {
                            foreach (var prop in dto.GetType().GetProperties())
                            {
                                if (prop.PropertyType == typeof(double) ||
                                    prop.PropertyType == typeof(float) ||
                                    prop.PropertyType == typeof(decimal) ||
                                    prop.PropertyType == typeof(double?) ||
                                    prop.PropertyType == typeof(float?) ||
                                    prop.PropertyType == typeof(decimal?))
                                    prop.SetValue(dto, (decimal?)Math.Round(Convert.ToDouble(prop.GetValue(dto)), 4));
                            }
                        }
                        result = suDtps;
                        break;

                    case "battery":
                        var bDtos = StaticEntities.ShowEntities.Batteries.FirstOrDefault(t => t.Number == request.Number).BatteryDtos;
                        foreach (var dto in bDtos)
                        {
                            foreach (var prop in dto.GetType().GetProperties())
                            {
                                if (prop.PropertyType == typeof(double) ||
                                    prop.PropertyType == typeof(float) ||
                                    prop.PropertyType == typeof(decimal) ||
                                    prop.PropertyType == typeof(double?) ||
                                    prop.PropertyType == typeof(float?) ||
                                    prop.PropertyType == typeof(decimal?))
                                    prop.SetValue(dto, (decimal?)Math.Round(Convert.ToDouble(prop.GetValue(dto)), 4));
                            }
                        }
                        result = bDtos;
                        break;

                    case "ed":
                        result = await _vesselInfoService.GetEnergyDistributionAsync(request.Number);
                        break;

                    case "er":
                        result = new
                        {
                            CompositeBoiler = StaticEntities.ShowEntities.CompositeBoilers.FirstOrDefault(t => t.Number == request.Number).CompositeBoilerDtos,
                            CompressedAirSupply = StaticEntities.ShowEntities.CompressedAirSupplies.FirstOrDefault(t => t.Number == request.Number).CompressedAirSupplyDtos,
                            CoolingFreshWater = StaticEntities.ShowEntities.CoolingFreshWaters.FirstOrDefault(t => t.Number == request.Number).CoolingFreshWaterDtos,
                            CoolingSeaWater = StaticEntities.ShowEntities.CoolingSeaWaters.FirstOrDefault(t => t.Number == request.Number).CoolingSeaWaterDtos,
                            CoolingWater = StaticEntities.ShowEntities.CoolingWaters.FirstOrDefault(t => t.Number == request.Number).CoolingWaterDtos,
                            CylinderLubOil = StaticEntities.ShowEntities.CylinderLubOils.FirstOrDefault(t => t.Number == request.Number).CylinderLubOilDtos,
                            ExhaustGas = StaticEntities.ShowEntities.ExhaustGases.FirstOrDefault(t => t.Number == request.Number).ExhaustGasDtos,
                            FO = StaticEntities.ShowEntities.FOs.FirstOrDefault(t => t.Number == request.Number).FODtos,
                            FOSupplyUnit = StaticEntities.ShowEntities.FOSupplyUnits.FirstOrDefault(t => t.Number == request.Number).FOSupplyUnitDtos,
                            LubOilPurifying = StaticEntities.ShowEntities.LubOilPurifyings.FirstOrDefault(t => t.Number == request.Number).LubOilPurifyingDtos,
                            LubOil = StaticEntities.ShowEntities.LubOils.FirstOrDefault(t => t.Number == request.Number).LubOilDtos,
                            MainGeneratorSet = StaticEntities.ShowEntities.MainGeneratorSets.FirstOrDefault(t => t.Number == request.Number).MainGeneratorSetDtos,
                            MainSwitchboard = StaticEntities.ShowEntities.MainSwitchboards.FirstOrDefault(t => t.Number == request.Number).MainSwitchboardDtos,
                            MERemoteControl = StaticEntities.ShowEntities.MERemoteControls.FirstOrDefault(t => t.Number == request.Number).MERemoteControlDtos,
                            Miscellaneous = StaticEntities.ShowEntities.Miscellaneouses.FirstOrDefault(t => t.Number == request.Number).MiscellaneousDtos,
                            ScavengeAir = StaticEntities.ShowEntities.ScavengeAirs.FirstOrDefault(t => t.Number == request.Number).ScavengeAirDtos,
                            ShaftClutch = StaticEntities.ShowEntities.ShaftClutchs.FirstOrDefault(t => t.Number == request.Number).ShaftClutchDtos,
                            AssistantDecision = StaticEntities.ShowEntities.AssistantDecisions.FirstOrDefault(t => t.Number == request.Number).AssistantDecisionDtos,
                        };
                        break;

                    case "ee":
                    default:
                        var vesselEntity = StaticEntities.ShowEntities.Vessels.FirstOrDefault(t => t.SN == request.Number);
                        foreach (var prop in vesselEntity.GetType().GetProperties())
                        {
                            if (prop.PropertyType == typeof(double) ||
                                prop.PropertyType == typeof(float) ||
                                prop.PropertyType == typeof(decimal) ||
                                prop.PropertyType == typeof(double?) ||
                                prop.PropertyType == typeof(float?) ||
                                prop.PropertyType == typeof(decimal?))
                                prop.SetValue(vesselEntity, Math.Round(Convert.ToDouble(prop.GetValue(vesselEntity)), 4));
                        }
                        result = vesselEntity;
                        var tempResult = result.ToJson().ToJObject();
                        var fsDtos = StaticEntities.ShowEntities.Flowmeters.FirstOrDefault(t => t.Number == request.Number).FlowmeterDtos;
                        var fsDtosClear = new List<FlowmeterDto> {
                            new FlowmeterDto { DeviceNo = "mefuel1", ConsAcc=0, ConsAct=0 },
                            new FlowmeterDto { DeviceNo = "mefuel2", ConsAcc=0, ConsAct=0 },
                            new FlowmeterDto { DeviceNo = "memethanol", ConsAcc=0, ConsAct=0 }
                        };

                        /*国能长江01
                        foreach (var dto in fsDtos)
                        {
                            if (!dto.DeviceNo.Contains("in") && !dto.DeviceNo.Contains("out"))
                            {
                                fsDtosClear[fsDtosClear.IndexOf(fsDtosClear.FirstOrDefault(t => t.DeviceNo == dto.DeviceNo.Replace("_", "")))].ConsAct = dto.ConsAct;
                                fsDtosClear[fsDtosClear.IndexOf(fsDtosClear.FirstOrDefault(t => t.DeviceNo == dto.DeviceNo.Replace("_", "")))].ConsAcc = dto.ConsAcc;
                            }
                            else if (dto.DeviceNo.Contains("in"))
                            {
                                fsDtosClear[fsDtosClear.IndexOf(fsDtosClear.FirstOrDefault(t => t.DeviceNo == dto.DeviceNo.Replace("in", "").Replace("_", "")))].ConsAct += dto.ConsAct;
                                fsDtosClear[fsDtosClear.IndexOf(fsDtosClear.FirstOrDefault(t => t.DeviceNo == dto.DeviceNo.Replace("in", "").Replace("_", "")))].ConsAcc += dto.ConsAcc;
                            }
                            else if (dto.DeviceNo.Contains("out"))
                            {
                                fsDtosClear[fsDtosClear.IndexOf(fsDtosClear.FirstOrDefault(t => t.DeviceNo == dto.DeviceNo.Replace("out", "").Replace("_", "")))].ConsAct -= dto.ConsAct;
                                fsDtosClear[fsDtosClear.IndexOf(fsDtosClear.FirstOrDefault(t => t.DeviceNo == dto.DeviceNo.Replace("out", "").Replace("_", "")))].ConsAcc -= dto.ConsAcc;
                            }
                        }*/

                        //me_fuel_in_1 主机 me_fuel_out_1 锅炉1 me_fuel_in_2 辅机进 me_fuel_in_2 辅机出 me_methanol 锅炉2
                        foreach (var dto in fsDtos)
                        {
                            /* dubai glamour
                             * if (dto.DeviceNo == "me_fuel_out_1" || dto.DeviceNo == "me_methanol")
                            {
                                fsDtosClear[fsDtosClear.IndexOf(fsDtosClear.FirstOrDefault(t => t.DeviceNo == "memethanol"))].ConsAct += dto.ConsAct;
                                fsDtosClear[fsDtosClear.IndexOf(fsDtosClear.FirstOrDefault(t => t.DeviceNo == "memethanol"))].ConsAcc += dto.ConsAcc;
                            }
                            else if (dto.DeviceNo == "me_fuel_in_2")
                            {
                                fsDtosClear[fsDtosClear.IndexOf(fsDtosClear.FirstOrDefault(t => t.DeviceNo == "mefuel2"))].ConsAct += dto.ConsAct;
                                fsDtosClear[fsDtosClear.IndexOf(fsDtosClear.FirstOrDefault(t => t.DeviceNo == "mefuel2"))].ConsAcc += dto.ConsAcc;
                            }
                            else if (dto.DeviceNo == "me_fuel_out_2")
                            {
                                fsDtosClear[fsDtosClear.IndexOf(fsDtosClear.FirstOrDefault(t => t.DeviceNo == "mefuel2"))].ConsAct -= dto.ConsAct;
                                fsDtosClear[fsDtosClear.IndexOf(fsDtosClear.FirstOrDefault(t => t.DeviceNo == "mefuel2"))].ConsAcc -= dto.ConsAcc;
                            }
                            else if (dto.DeviceNo == "me_fuel_in_1")
                            {
                                fsDtosClear[fsDtosClear.IndexOf(fsDtosClear.FirstOrDefault(t => t.DeviceNo == "mefuel1"))].ConsAct = dto.ConsAct;
                                fsDtosClear[fsDtosClear.IndexOf(fsDtosClear.FirstOrDefault(t => t.DeviceNo == "mefuel1"))].ConsAcc = dto.ConsAcc;
                            }*/
                            if (dto.DeviceNo == "blr_fuel")
                            {
                                fsDtosClear[fsDtosClear.IndexOf(fsDtosClear.FirstOrDefault(t => t.DeviceNo == "memethanol"))].ConsAct = dto.ConsAct;
                                fsDtosClear[fsDtosClear.IndexOf(fsDtosClear.FirstOrDefault(t => t.DeviceNo == "memethanol"))].ConsAcc = dto.ConsAcc;
                                fsDtosClear[fsDtosClear.IndexOf(fsDtosClear.FirstOrDefault(t => t.DeviceNo == "memethanol"))].FuelType = dto.FuelType;
                            }
                            else if (dto.DeviceNo == "ae_fuel")
                            {
                                fsDtosClear[fsDtosClear.IndexOf(fsDtosClear.FirstOrDefault(t => t.DeviceNo == "mefuel2"))].ConsAct = dto.ConsAct;
                                fsDtosClear[fsDtosClear.IndexOf(fsDtosClear.FirstOrDefault(t => t.DeviceNo == "mefuel2"))].ConsAcc = dto.ConsAcc;
                                fsDtosClear[fsDtosClear.IndexOf(fsDtosClear.FirstOrDefault(t => t.DeviceNo == "mefuel2"))].FuelType = dto.FuelType;
                            }
                            else if (dto.DeviceNo == "me_fuel")
                            {
                                fsDtosClear[fsDtosClear.IndexOf(fsDtosClear.FirstOrDefault(t => t.DeviceNo == "mefuel1"))].ConsAct = dto.ConsAct;
                                fsDtosClear[fsDtosClear.IndexOf(fsDtosClear.FirstOrDefault(t => t.DeviceNo == "mefuel1"))].ConsAcc = dto.ConsAcc;
                                fsDtosClear[fsDtosClear.IndexOf(fsDtosClear.FirstOrDefault(t => t.DeviceNo == "mefuel1"))].FuelType = dto.FuelType;
                            }
                        }

                        foreach (var dto in fsDtosClear)
                        {
                            foreach (var prop in dto.GetType().GetProperties())
                            {
                                if (prop.PropertyType == typeof(double) ||
                                    prop.PropertyType == typeof(float) ||
                                    prop.PropertyType == typeof(decimal) ||
                                    prop.PropertyType == typeof(double?) ||
                                    prop.PropertyType == typeof(float?) ||
                                    prop.PropertyType == typeof(decimal?))
                                    prop.SetValue(dto, (decimal?)Math.Round(Convert.ToDouble(prop.GetValue(dto)), 4));
                            }
                        }
                        var fs = new JProperty("Flowmeters", JArray.FromObject(fsDtosClear));
                        tempResult.Add(fs);
                        var ssDtos = StaticEntities.ShowEntities.Shafts.FirstOrDefault(t => t.Number == request.Number).ShaftDtos;
                        var ssDtosClear = new List<ShaftDto> {
                            new ShaftDto { DeviceNo = "shaft_1", Power=0, RPM=0, Torque=0, Thrust=0 },
                            new ShaftDto { DeviceNo = "shaft_2", Power=0, RPM=0, Torque=0, Thrust=0 }
                        };

                        foreach (var dto in ssDtos)
                        {
                            /* dubai glamour
                             * ssDtosClear[ssDtosClear.IndexOf(ssDtosClear.FirstOrDefault(t => t.DeviceNo == dto.DeviceNo))].Power = dto.Power;
                            ssDtosClear[ssDtosClear.IndexOf(ssDtosClear.FirstOrDefault(t => t.DeviceNo == dto.DeviceNo))].RPM = dto.RPM;
                            ssDtosClear[ssDtosClear.IndexOf(ssDtosClear.FirstOrDefault(t => t.DeviceNo == dto.DeviceNo))].Torque = dto.Torque;
                            ssDtosClear[ssDtosClear.IndexOf(ssDtosClear.FirstOrDefault(t => t.DeviceNo == dto.DeviceNo))].Thrust = dto.Thrust;*/
                            ssDtosClear[ssDtosClear.IndexOf(ssDtosClear.FirstOrDefault(t => t.DeviceNo == "shaft_1"))].Power = dto.Power;
                            ssDtosClear[ssDtosClear.IndexOf(ssDtosClear.FirstOrDefault(t => t.DeviceNo == "shaft_1"))].RPM = dto.RPM;
                            ssDtosClear[ssDtosClear.IndexOf(ssDtosClear.FirstOrDefault(t => t.DeviceNo == "shaft_1"))].Torque = dto.Torque;
                            ssDtosClear[ssDtosClear.IndexOf(ssDtosClear.FirstOrDefault(t => t.DeviceNo == "shaft_1"))].Thrust = dto.Thrust;
                        }

                        foreach (var dto in ssDtosClear)
                        {
                            foreach (var prop in dto.GetType().GetProperties())
                            {
                                try
                                {
                                    if ((prop.PropertyType == typeof(double) ||
                                                                prop.PropertyType == typeof(float) ||
                                                                prop.PropertyType == typeof(decimal) ||
                                                                prop.PropertyType == typeof(double?) ||
                                                                prop.PropertyType == typeof(float?) ||
                                                                prop.PropertyType == typeof(decimal?)) && prop.GetValue(dto) != null)
                                        prop.SetValue(dto, (decimal?)Math.Round(Convert.ToDouble(prop.GetValue(dto)), 4));
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("转换值错误：" + prop.GetValue(dto));
                                    Debug.WriteLine("转换值错误：" + prop.GetValue(dto));
                                    _logger.LogError("转换值错误：" + prop.GetValue(dto), ex);
                                    throw;
                                }
                            }
                        }
                        var ss = new JProperty("Shafts", JArray.FromObject(ssDtosClear));
                        tempResult.Add(ss);

                        var ECStatus = "";
                        var EEStatus = "";
                        var criteriaDto = new CriteriaDto();
                        if (StaticEntities.StaticEntities.Configs.Any(t => t.Number == request.Number && t.IsDevice == 0 && t.Code == "CriteriaHFO"))
                        {
                            criteriaDto.HFO = (double)StaticEntities.StaticEntities.Configs.FirstOrDefault(t => t.Number == request.Number && t.IsDevice == 0 && t.Code == "CriteriaHFO").HighLimit;
                        }
                        if (StaticEntities.StaticEntities.Configs.Any(t => t.Number == request.Number && t.IsDevice == 0 && t.Code == "CriteriaMethanol"))
                        {
                            criteriaDto.Methanol = (double)StaticEntities.StaticEntities.Configs.FirstOrDefault(t => t.Number == request.Number && t.IsDevice == 0 && t.Code == "CriteriaMethanol").HighLimit;
                        }
                        if (StaticEntities.StaticEntities.Configs.Any(t => t.Number == request.Number && t.IsDevice == 0 && t.Code == "CriteriaPower"))
                        {
                            criteriaDto.Power = (double)StaticEntities.StaticEntities.Configs.FirstOrDefault(t => t.Number == request.Number && t.IsDevice == 0 && t.Code == "CriteriaPower").HighLimit;
                        }
                        if (StaticEntities.StaticEntities.Configs.Any(t => t.Number == request.Number && t.IsDevice == 0 && t.Code == "CriteriaSpeed"))
                        {
                            criteriaDto.Speed = (double)StaticEntities.StaticEntities.Configs.FirstOrDefault(t => t.Number == request.Number && t.IsDevice == 0 && t.Code == "CriteriaSpeed").HighLimit;
                        }
                        if (Convert.ToDouble(vesselEntity.SFOC) * Convert.ToDouble(vesselEntity.FCPerNm) != 0)
                        {
                            var ecValue = (vesselEntity.SFOC / (criteriaDto.HFO / criteriaDto.Power) + vesselEntity.FCPerNm / (criteriaDto.HFO / criteriaDto.Speed)) / 2;
                            if (ecValue > 0.99 && ecValue <= 1.09)
                                ECStatus = request.Language == "en_US" ? "Normal" : "正常";
                            else if (ecValue > 0.94 && ecValue <= 0.99)
                                ECStatus = (request.Language == "en_US" ? "Good,Fuel saving:" : "良好，节油率为") + $"{Math.Round(100 - Convert.ToDouble(ecValue) * 100, 1)}%";
                            else if (ecValue <= 0.94)
                                ECStatus = (request.Language == "en_US" ? "Excellent,Fuel saving:" : "极佳，节油率为") + $"{Math.Round(100 - Convert.ToDouble(ecValue) * 100, 1)}%";
                            else
                                ECStatus = request.Language == "en_US" ? "Bad" : "差";
                        }
                        if (Convert.ToDouble(vesselEntity.RtCII) != 0)
                        {
                            var eeValue = vesselEntity.RtCII / ((criteriaDto.HFO * 3.114d + criteriaDto.Methanol * 1.375d) * 1000d / (10000d * 18d));
                            if (eeValue > 0.95 && eeValue <= 1.05)
                                EEStatus = request.Language == "en_US" ? "Normal" : "正常";
                            else if (eeValue <= 0.95)
                                EEStatus = request.Language == "en_US" ? "Good" : "良好";
                            else
                                EEStatus = request.Language == "en_US" ? "Bad" : "差";
                        }

                        var HFOC = 0m;
                        var HFOCPre = 0m;

                        var dHFOAcc = 0d;
                        var dMethanolAcc = 0d;

                        var dMEHFOAcc = 0d;
                        var dAEHFOAcc = 0d;
                        var dBLRHFOAcc = 0d;
                        var dMEDGOAcc = 0d;
                        var dAEDGOAcc = 0d;
                        var dBLRDGOAcc = 0d;

                        try
                        {
                            var totalIndicator = StaticEntities.ShowEntities.TotalIndicators.FirstOrDefault(t => t.Number == request.Number);
                            var powerUnits = StaticEntities.ShowEntities.PowerUnits.FirstOrDefault(t => t.Number == request.Number).PowerUnitDtos;
                            var meFuel = powerUnits.FirstOrDefault(t => t.DeviceType == "me", new());
                            var aeFuel = powerUnits.FirstOrDefault(t => t.DeviceType == "ae", new());
                            var blrFuel = powerUnits.FirstOrDefault(t => t.DeviceType == "blr", new());
                            var prediction = StaticEntities.ShowEntities.Predictions.FirstOrDefault(t => t.Number == request.Number);
                            var shipinfo = new BaseShipInfo();
                            var cargoWeight = 0d;

                            var CEmission = Math.Round((totalIndicator.HFO ?? 0) * 3.114m + (totalIndicator.Methanol ?? 0) * 1.375m + (totalIndicator.DGO ?? 0) * 3.206m, 2);
                            tempResult.Add(new JProperty("CEmission", CEmission.ToString()));

                            using (var _channel = await _consulService.GetGrpcChannelAsync("base-srv"))
                            {
                                var client = new Base.BaseClient(_channel);
                                var deviceResponse = await client.GetDeviceByNumberAsync(new DeviceRequest { Number = request.Number });
                                var shipResponse = await client.GetShipByIdAsync(new IdRequest { Id = deviceResponse.DeviceInfo.ShipId });
                                var voyageResponse = await client.GetLatestVoyageInfoByDeviceNumberAsync(new DeviceRequest { Number = request.Number });
                                shipinfo.ShipType = shipResponse.ShipInfo.TypeName;
                                shipinfo.DWT = shipResponse.ShipInfo.Dwt;
                                shipinfo.GT = shipResponse.ShipInfo.Gt;
                                cargoWeight = voyageResponse.Boatload;
                            }
                            var tonnage = 0f;
                            if (shipinfo.ShipType.Contains("LNG carrier") ||
                                shipinfo.ShipType.Contains("bulk carrier") ||
                                shipinfo.ShipType.Contains("combination carrier") ||
                                shipinfo.ShipType.Contains("container ship") ||
                                shipinfo.ShipType.Contains("gas carrier") ||
                                shipinfo.ShipType.Contains("general cargo ship") ||
                                shipinfo.ShipType.Contains("refrigerated cargo carrier") ||
                                shipinfo.ShipType.Contains("tanker"))
                                tonnage = shipinfo.DWT ?? 0;
                            else
                                tonnage = shipinfo.GT ?? 0;
                            var PreRtCII = Math.Round(Convert.ToDouble((prediction.HFO ?? 0) * 3.114m + (prediction.DGO ?? 0) * 3.206m) * 1000 / Convert.ToDouble(tonnage * vesselEntity.GroundSpeed), 2);
                            var PreHFO = Math.Round((vesselEntity.GroundSpeed ?? 0) == 0 ? 0 : Convert.ToDouble(prediction.HFO) / Convert.ToDouble(vesselEntity.GroundSpeed), 2);
                            var PreTW = Math.Round((vesselEntity.GroundSpeed ?? 0) == 0 ? 0 : Convert.ToDouble(prediction.HFO) * 1000 / Convert.ToDouble(vesselEntity.GroundSpeed) / tonnage, 2);
                            var PreCO2 = Math.Round((vesselEntity.GroundSpeed ?? 0) == 0 ? 0 : Convert.ToDouble(prediction.HFO) * 3.114 / Convert.ToDouble(vesselEntity.GroundSpeed), 2);
                            var PreCEmission = Math.Round((vesselEntity.GroundSpeed ?? 0) == 0 ? 0 : Convert.ToDouble(prediction.HFO) * 3.114, 2);
                            var HFOTW = Math.Round((vesselEntity.GroundSpeed ?? 0) == 0 ? 0 : Convert.ToDouble(totalIndicator.HFO) * 1000 / Convert.ToDouble(vesselEntity.GroundSpeed) / tonnage, 2);
                            var CO2TW = Math.Round((vesselEntity.GroundSpeed ?? 0) == 0 ? 0 : Convert.ToDouble(totalIndicator.HFO * 3.114m + totalIndicator.Methanol * 1.375m) * 1000 / Convert.ToDouble(vesselEntity.GroundSpeed) / tonnage, 2);
                            var EEOI = cargoWeight == 0 ? 0 : Math.Round((vesselEntity.GroundSpeed ?? 0) == 0 ? 0 : Convert.ToDouble(totalIndicator.HFO * 3.114m + totalIndicator.Methanol * 1.375m) * 1000 / Convert.ToDouble(vesselEntity.GroundSpeed) / cargoWeight, 2);
                            var PreEEOI = cargoWeight == 0 ? 0 : Math.Round((vesselEntity.GroundSpeed ?? 0) == 0 ? 0 : Convert.ToDouble(prediction.HFO * 3.114m) * 1000 / Convert.ToDouble(vesselEntity.GroundSpeed) / cargoWeight, 2);
                            var CO2PerNm = Math.Round((totalIndicator.HFO ?? 0) * 3.114m + (totalIndicator.Methanol ?? 0) * 1.375m, 2);
                            tempResult.Add(new JProperty("PreRtCII", PreRtCII));
                            tempResult.Add(new JProperty("PreHFO", PreHFO));
                            tempResult.Add(new JProperty("PreTW", PreTW));
                            tempResult.Add(new JProperty("PreCO2", PreCO2));
                            tempResult.Add(new JProperty("PreCEmission", PreCEmission));

                            tempResult.Add(new JProperty("HFOTW", HFOTW));
                            tempResult.Add(new JProperty("CO2TW", CO2TW));
                            tempResult.Add(new JProperty("EEOI", EEOI));
                            tempResult.Add(new JProperty("PreEEOI", PreEEOI));
                            tempResult.Add(new JProperty("CO2PerNm", CO2PerNm));

                            var eeRating = prediction.HFO / totalIndicator.HFO;
                            if (eeRating > 0.95m && eeRating <= 1.05m)
                                EEStatus = request.Language == "en_US" ? "Normal" : "正常";
                            else if (eeRating <= 0.99m)
                                EEStatus = request.Language == "en_US" ? "Good" : "良好";
                            else
                                EEStatus = request.Language == "en_US" ? "Bad" : "差";

                            HFOC = Math.Round(totalIndicator.HFO ?? 0, 2);
                            HFOCPre = Math.Round(prediction.HFO ?? 0, 2);

                            var dailyConsumptionIndex = StaticEntities.StaticEntities.DailyConsumptions.IndexOf(StaticEntities.StaticEntities.DailyConsumptions.FirstOrDefault(t => t.Number == request.Number));
                            dHFOAcc = Convert.ToDouble(totalIndicator.HFOAccumulated) - StaticEntities.StaticEntities.DailyConsumptions[dailyConsumptionIndex].HFOAcc;
                            dMethanolAcc = Convert.ToDouble(totalIndicator.MethanolAccumulated) - StaticEntities.StaticEntities.DailyConsumptions[dailyConsumptionIndex].MethanolAcc;

                            dMEHFOAcc = Convert.ToDouble(meFuel.HFOAccumulated) - StaticEntities.StaticEntities.DailyConsumptions[dailyConsumptionIndex].MEHFOAcc;
                            dAEHFOAcc = Convert.ToDouble(aeFuel.HFOAccumulated) - StaticEntities.StaticEntities.DailyConsumptions[dailyConsumptionIndex].AEHFOAcc;
                            dBLRHFOAcc = Convert.ToDouble(blrFuel.HFOAccumulated) - StaticEntities.StaticEntities.DailyConsumptions[dailyConsumptionIndex].BLRHFOAcc;
                            dMEDGOAcc = Convert.ToDouble(meFuel.DGOAccumulated) - StaticEntities.StaticEntities.DailyConsumptions[dailyConsumptionIndex].MEDGOAcc;
                            dAEDGOAcc = Convert.ToDouble(aeFuel.DGOAccumulated) - StaticEntities.StaticEntities.DailyConsumptions[dailyConsumptionIndex].AEDGOAcc;
                            dBLRDGOAcc = Convert.ToDouble(blrFuel.DGOAccumulated) - StaticEntities.StaticEntities.DailyConsumptions[dailyConsumptionIndex].BLRDGOAcc;
                        }
                        catch (Exception) { }
                        tempResult.Add(new JProperty("ECStatus", ECStatus));
                        tempResult.Add(new JProperty("EEStatus", EEStatus));

                        tempResult.Add(new JProperty("HFOC", HFOC));
                        tempResult.Add(new JProperty("HFOCPre", HFOCPre));

                        tempResult.Add(new JProperty("dHFOAcc", dHFOAcc));
                        tempResult.Add(new JProperty("dMethanolAcc", dMethanolAcc));

                        tempResult.Add(new JProperty("dMEHFOAcc", dMEHFOAcc));
                        tempResult.Add(new JProperty("dAEHFOAcc", dAEHFOAcc));
                        tempResult.Add(new JProperty("dBLRHFOAcc", dBLRHFOAcc));
                        tempResult.Add(new JProperty("dMEDGOAcc", dMEDGOAcc));
                        tempResult.Add(new JProperty("dAEDGOAcc", dAEDGOAcc));
                        tempResult.Add(new JProperty("dBLRDGOAcc", dBLRDGOAcc));

                        result = tempResult;
                        break;
                }
            }
            else
            {
                switch (request.Type)
                {
                    case "generator":
                        result = new List<Generator> {
                                new Generator{
                                    Number="SAD1",
                                    ReceiveDatetime=DateTime.UtcNow,
                                    DeviceNo="G1",
                                    IsRuning=1,
                                    RPM=0,
                                    StartPressure=0,
                                    ControlPressure=0,
                                    ScavengingPressure=0,
                                    LubePressure=0,
                                    LubeTEMP=0,
                                    FuelPressure=0,
                                    FuelTEMP=0,
                                    FreshWaterPressure=0,
                                    FreshWaterTEMPIn=0,
                                    FreshWaterTEMPOut=0,
                                    CoolingWaterPressure=0,
                                    CoolingWaterTEMPIn=0,
                                    CoolingWaterTEMPOut=0,
                                    CylinderTEMP1=0,
                                    CylinderTEMP2=0,
                                    CylinderTEMP3=0,
                                    CylinderTEMP4=0,
                                    CylinderTEMP5=0,
                                    CylinderTEMP6=0,
                                    SuperchargerTEMPIn=0,
                                    SuperchargerTEMPOut=0,
                                    ScavengingTEMP=0,
                                    BearingTEMP=0,
                                    BearingTEMPFront=0,
                                    BearingTEMPBack=0,
                                    Power=400,
                                    WindingTEMPL1=0,
                                    WindingTEMPL2=0,
                                    WindingTEMPL3=0,
                                    VoltageL1L2=0,
                                    VoltageL2L3=0,
                                    VoltageL1L3=0,
                                    FrequencyL1=0,
                                    FrequencyL2=0,
                                    FrequencyL3=0,
                                    CurrentL1=0,
                                    CurrentL2=0,
                                    CurrentL3=0,
                                    ReactivePower=0,
                                    PowerFactor=0,
                                    LoadRatio=0
                                },
                                new Generator{
                                    Number="SAD1",
                                    ReceiveDatetime=DateTime.UtcNow,
                                    DeviceNo="G2",
                                    IsRuning=1,
                                    RPM=0,
                                    StartPressure=0,
                                    ControlPressure=0,
                                    ScavengingPressure=0,
                                    LubePressure=0,
                                    LubeTEMP=0,
                                    FuelPressure=0,
                                    FuelTEMP=0,
                                    FreshWaterPressure=0,
                                    FreshWaterTEMPIn=0,
                                    FreshWaterTEMPOut=0,
                                    CoolingWaterPressure=0,
                                    CoolingWaterTEMPIn=0,
                                    CoolingWaterTEMPOut=0,
                                    CylinderTEMP1=0,
                                    CylinderTEMP2=0,
                                    CylinderTEMP3=0,
                                    CylinderTEMP4=0,
                                    CylinderTEMP5=0,
                                    CylinderTEMP6=0,
                                    SuperchargerTEMPIn=0,
                                    SuperchargerTEMPOut=0,
                                    ScavengingTEMP=0,
                                    BearingTEMP=0,
                                    BearingTEMPFront=0,
                                    BearingTEMPBack=0,
                                    Power=350,
                                    WindingTEMPL1=0,
                                    WindingTEMPL2=0,
                                    WindingTEMPL3=0,
                                    VoltageL1L2=0,
                                    VoltageL2L3=0,
                                    VoltageL1L3=0,
                                    FrequencyL1=0,
                                    FrequencyL2=0,
                                    FrequencyL3=0,
                                    CurrentL1=0,
                                    CurrentL2=0,
                                    CurrentL3=0,
                                    ReactivePower=0,
                                    PowerFactor=0,
                                    LoadRatio=0
                                },
                                new Generator{
                                    Number="SAD1",
                                    ReceiveDatetime=DateTime.UtcNow,
                                    DeviceNo="G3",
                                    IsRuning=0,
                                    RPM=0,
                                    StartPressure=0,
                                    ControlPressure=0,
                                    ScavengingPressure=0,
                                    LubePressure=0,
                                    LubeTEMP=0,
                                    FuelPressure=0,
                                    FuelTEMP=0,
                                    FreshWaterPressure=0,
                                    FreshWaterTEMPIn=0,
                                    FreshWaterTEMPOut=0,
                                    CoolingWaterPressure=0,
                                    CoolingWaterTEMPIn=0,
                                    CoolingWaterTEMPOut=0,
                                    CylinderTEMP1=0,
                                    CylinderTEMP2=0,
                                    CylinderTEMP3=0,
                                    CylinderTEMP4=0,
                                    CylinderTEMP5=0,
                                    CylinderTEMP6=0,
                                    SuperchargerTEMPIn=0,
                                    SuperchargerTEMPOut=0,
                                    ScavengingTEMP=0,
                                    BearingTEMP=0,
                                    BearingTEMPFront=0,
                                    BearingTEMPBack=0,
                                    Power=0,
                                    WindingTEMPL1=0,
                                    WindingTEMPL2=0,
                                    WindingTEMPL3=0,
                                    VoltageL1L2=0,
                                    VoltageL2L3=0,
                                    VoltageL1L3=0,
                                    FrequencyL1=0,
                                    FrequencyL2=0,
                                    FrequencyL3=0,
                                    CurrentL1=0,
                                    CurrentL2=0,
                                    CurrentL3=0,
                                    ReactivePower=0,
                                    PowerFactor=0,
                                    LoadRatio=0
                                },
                                new Generator{
                                    Number="SAD1",
                                    ReceiveDatetime=DateTime.UtcNow,
                                    DeviceNo="G4",
                                    IsRuning=1,
                                    RPM=0,
                                    StartPressure=0,
                                    ControlPressure=0,
                                    ScavengingPressure=0,
                                    LubePressure=0,
                                    LubeTEMP=0,
                                    FuelPressure=0,
                                    FuelTEMP=0,
                                    FreshWaterPressure=0,
                                    FreshWaterTEMPIn=0,
                                    FreshWaterTEMPOut=0,
                                    CoolingWaterPressure=0,
                                    CoolingWaterTEMPIn=0,
                                    CoolingWaterTEMPOut=0,
                                    CylinderTEMP1=0,
                                    CylinderTEMP2=0,
                                    CylinderTEMP3=0,
                                    CylinderTEMP4=0,
                                    CylinderTEMP5=0,
                                    CylinderTEMP6=0,
                                    SuperchargerTEMPIn=0,
                                    SuperchargerTEMPOut=0,
                                    ScavengingTEMP=0,
                                    BearingTEMP=0,
                                    BearingTEMPFront=0,
                                    BearingTEMPBack=0,
                                    Power=370,
                                    WindingTEMPL1=0,
                                    WindingTEMPL2=0,
                                    WindingTEMPL3=0,
                                    VoltageL1L2=0,
                                    VoltageL2L3=0,
                                    VoltageL1L3=0,
                                    FrequencyL1=0,
                                    FrequencyL2=0,
                                    FrequencyL3=0,
                                    CurrentL1=0,
                                    CurrentL2=0,
                                    CurrentL3=0,
                                    ReactivePower=0,
                                    PowerFactor=0,
                                    LoadRatio=0
                                }
                            };
                        break;

                    case "shaft":
                        result = new
                        {
                            sternSealings = new List<SternSealing> {
                                new SternSealing
                                {
                                    Number = "SAD1",
                                    ReceiveDatetime = DateTime.UtcNow,
                                    DeviceNo = "SternSealing1",
                                    FrontTEMP = 0,
                                    BackTEMP = 0,
                                    BackLeftTEMP = 0,
                                    BackRightTEMP = 0
                                }
                            },
                            shafts = new List<Shaft> {
                                new Shaft{
                                    Number="SAD1",
                                    ReceiveDatetime=DateTime.UtcNow,
                                    DeviceNo="Shaft1",
                                    Power=0,
                                    RPM=0,
                                    Torque=0,
                                    Thrust=0
                                }
                            }
                        };
                        break;

                    case "liquid":
                        result = new List<LiquidLevel> {
                                new LiquidLevel{
                                    Number="SAD1",
                                    ReceiveDatetime=DateTime.UtcNow,
                                    DeviceNo="l1",
                                    Level=0,
                                    Temperature=0
                                }
                            };
                        break;

                    case "supply":
                        result = new List<SupplyUnit> {
                                new SupplyUnit{
                                    Number="SAD1",
                                    ReceiveDatetime=DateTime.UtcNow,
                                    DeviceNo="s1",
                                    IsRuning=1,
                                    Temperature=0,
                                    Pressure=0
                                }
                            };
                        break;

                    case "battery":
                        result = new List<Battery> {
                                new Battery{
                                    Number="SAD1",
                                    ReceiveDatetime=DateTime.UtcNow,
                                    DeviceNo="b1",
                                    SOC=100,
                                    SOH=100,
                                    MaxTEMP=0,
                                    MaxTEMPBox="0",
                                    MaxTEMPNo="0",
                                    MinTEMP=0,
                                    MinTEMPBox="0",
                                    MinTEMPNo="0",
                                    MaxVoltage=0,
                                    MaxVoltageBox="0",
                                    MaxVoltageNo="0",
                                    MinVoltage=0,
                                    MinVoltageBox="0",
                                    MinVoltageNo="0"
                                },
                                new Battery{
                                    Number="SAD1",
                                    ReceiveDatetime=DateTime.UtcNow,
                                    DeviceNo="b2",
                                    SOC=120,
                                    SOH=120,
                                    MaxTEMP=0,
                                    MaxTEMPBox="0",
                                    MaxTEMPNo="0",
                                    MinTEMP=0,
                                    MinTEMPBox="0",
                                    MinTEMPNo="0",
                                    MaxVoltage=0,
                                    MaxVoltageBox="0",
                                    MaxVoltageNo="0",
                                    MinVoltage=0,
                                    MinVoltageBox="0",
                                    MinVoltageNo="0"
                                },
                            };
                        break;

                    case "er":
                        result = new
                        {
                            CompositeBoiler = new List<CompositeBoilerDto>() { new CompositeBoilerDto { Id = 50950, BLRBurnerRunning = 0, BLRHFOService = 0, BLRDGOService = 1, BLRFOP1On = 0, BLRFOP2On = 0, BLRFOTempLow = 0, BLRFOPressHigh = 0, BLRFOTempHigh = 0, BLRDGOTempHigh = 0, BLRHFOTempHigh = null, BLRGE1EXTempHigh = 0, BLRGE2EXTempHigh = 0, Uploaded = 0, Number = "SAD1", ReceiveDatetime = DateTime.UtcNow, DeviceNo = "COMPOSITEBOILER" } },
                            CompressedAirSupply = new List<CompressedAirSupplyDto>() { new CompressedAirSupplyDto { Id = 48778, MEStartPress = 27.4, MEControlPress = 6.9, ExhaustValuePress = 6.9, Uploaded = 0, Number = "SAD1", ReceiveDatetime = DateTime.UtcNow, DeviceNo = "COMPRESSEDAIRSUPPLY" } },
                            CoolingFreshWater = new List<CoolingFreshWaterDto>() { new CoolingFreshWaterDto { Id = 59244, LTCFWPress = 2.3, CCLTCFWOutTemp = 12.2, LTCFW1Press = null, LTCFW2Press = null, LTCFW3Press = null, MEJWCOutPress = null, Uploaded = 0, Number = "SAD1", ReceiveDatetime = DateTime.UtcNow, DeviceNo = "COOLINGFRESHWATER" } },
                            CoolingSeaWater = new List<CoolingSeaWaterDto>() { new CoolingSeaWaterDto { Id = 47539, CSWOutPress = 2.3, CSWOutTemp = 2.3, Uploaded = 0, Number = "SAD1", ReceiveDatetime = DateTime.UtcNow, DeviceNo = "COOLINGSEAWATER" } },
                            CoolingWater = new List<CoolingWaterDto>() { new CoolingWaterDto { Id = 51635, MEJacketInPress = 4.1, MEPressDrop = 1.8, MEOutPress = 2.5, MEJacketPressDrop = 1.8, MEInTemp = 84, MEJacketCyl1OutTemp = 82, MEJacketCyl2OutTemp = 83, MEJacketCyl3OutTemp = 83, MEJacketCyl4OutTemp = 83, MEJacketCyl5OutTemp = 83, MEJacketCyl6OutTemp = 83, MECCCyl1OutTemp = 84, MECCCyl2OutTemp = 84, MECCCyl3OutTemp = 83, MECCCyl4OutTemp = 84, MECCCyl5OutTemp = 84, MECCCyl6OutTemp = 84, MEACInPress = 2.2, MEACInTemp = 36, MEACOutTemp = 36, Uploaded = 0, Number = "SAD1", ReceiveDatetime = DateTime.UtcNow, DeviceNo = "COOLINGWATER" } },
                            CylinderLubOil = new List<CylinderLubOilDto>() { new CylinderLubOilDto { Id = 56253, MEInTemp = 42.7003550613796, Uploaded = 0, Number = "SAD1", ReceiveDatetime = DateTime.UtcNow, DeviceNo = "CYLINDERLUBOIL" } },
                            ExhaustGas = new List<ExhaustGasDto>() { new ExhaustGasDto { Id = 51016, METCInTemp = 139, MECyl1AfterTemp = null, MECyl2AfterTemp = null, MECyl3AfterTemp = null, MECyl4AfterTemp = null, MECyl5AfterTemp = null, MECyl6AfterTemp = null, METCOutTemp = 130, MEReceiverPress = 0.1, METurbBackPress = 0, MEACInTemp = 25, MEACOutTemp = 36, Uploaded = 0, Number = "SAD1", ReceiveDatetime = DateTime.UtcNow, DeviceNo = "EXHAUSTGAS", MECyl1AfterTempDev = -8, MECyl2AfterTempDev = -5, MECyl3AfterTempDev = -5, MECyl4AfterTempDev = 3, MECyl5AfterTempDev = 8, MECyl6AfterTempDev = 6 } },
                            FO = new List<FODto>() { new FODto { Id = 52551, MEInPressure = 8.1, MEInTemp = 35, MEHPOPLeakage = 0, Uploaded = 0, Number = "SAD1", ReceiveDatetime = DateTime.UtcNow, DeviceNo = "FOS" } },
                            FOSupplyUnit = new List<FOSupplyUnitDto>() { new FOSupplyUnitDto { Id = 46281, HFOService = 1, DGOService = 1, Uploaded = 0, Number = "SAD1", ReceiveDatetime = DateTime.UtcNow, DeviceNo = "FOSUPPLYUNIT" } },
                            LubOilPurifying = new List<LubOilPurifyingDto>() { new LubOilPurifyingDto { Id = 45732, MEFilterPressHigh = 0, Uploaded = 0, Number = "SAD1", ReceiveDatetime = DateTime.UtcNow, DeviceNo = "LUBOILPURIFYING" } },
                            LubOil = new List<LubOilDto>() { new LubOilDto { Id = 49594, METCInPress = 2.1, METBSTemp = 46, MEMBTBInPress = 2.1, MEPistonCOInPress = 2.1, MEInTemp = 46, MECYL1PistonCOOutTemp = 47, MECYL2PistonCOOutTemp = 47, MECYL3PistonCOOutTemp = 47, MECYL4PistonCOOutTemp = 47, MECYL5PistonCOOutTemp = 47, MECYL6PistonCOOutTemp = 47, MECYL1PistonCOOutNoFlow = 0, MECYL2PistonCOOutNoFlow = 0, MECYL3PistonCOOutNoFlow = 0, MECYL4PistonCOOutNoFlow = 0, MECYL5PistonCOOutNoFlow = 0, MECYL6PistonCOOutNoFlow = 0, METCOutTemp = 47, MEWaterHigh = 0, Uploaded = 0, Number = "SAD1", ReceiveDatetime = DateTime.UtcNow, DeviceNo = "LUBOIL" } },
                            MainGeneratorSet = new List<MainGeneratorSetDto>() { new MainGeneratorSetDto { Id = 160136, DGCFWInPress = 0.2, DGStartAirPress = 2.8, DGLOPress = 0.5, DGLOInTemp = 944.2, DGCFWOutTemp = 78, DGEGTC1To3InTemp = 366, DGEGTC4To6InTemp = 362, DGEngineSpeed = 902, DGEngineLoad = 206, DGEngineRunHour = 110, DGLOInPress = 0, DGLOFilterInPress = 0, DGControlAirPress = 0, DGTCLOPress = 0, DGEngineRunning = 1, DGUTemp = 39.3, DGVTemp = 39.4, DGWTemp = 37.9, DGBTDTemp = 40.1, DGCyl1ExTemp = 276, DGCyl2ExTemp = 284, DGCyl3ExTemp = 272, DGCyl4ExTemp = 284, DGCyl5ExTemp = 277, DGCyl6ExTemp = 268, DGTCEXOutTemp = 296, DGBoostAirPress = 0.1, DGFOInPress = 0.8, Uploaded = 0, Number = "SAD1", ReceiveDatetime = DateTime.UtcNow, DeviceNo = "MAINGENERATORSET1" }, new MainGeneratorSetDto { Id = 160134, DGCFWInPress = 0.1, DGStartAirPress = 2.7, DGLOPress = 0.1, DGLOInTemp = 37, DGCFWOutTemp = 69, DGEGTC1To3InTemp = 20, DGEGTC4To6InTemp = 23, DGEngineSpeed = 0, DGEngineLoad = 0, DGEngineRunHour = 11, DGLOInPress = 0, DGLOFilterInPress = 0, DGControlAirPress = 0, DGTCLOPress = 0, DGEngineRunning = 0, DGUTemp = 17.1, DGVTemp = 17.2, DGWTemp = 17.5, DGBTDTemp = 18.2, DGCyl1ExTemp = 66, DGCyl2ExTemp = 67, DGCyl3ExTemp = 67, DGCyl4ExTemp = 68, DGCyl5ExTemp = 68, DGCyl6ExTemp = 67, DGTCEXOutTemp = 17, DGBoostAirPress = 0, DGFOInPress = 0.7, Uploaded = 0, Number = "SAD1", ReceiveDatetime = DateTime.UtcNow, DeviceNo = "MAINGENERATORSET2" }, new MainGeneratorSetDto { Id = 160135, DGCFWInPress = 0.2, DGStartAirPress = 2.7, DGLOPress = 0.5, DGLOInTemp = 57, DGCFWOutTemp = 78, DGEGTC1To3InTemp = 366, DGEGTC4To6InTemp = 356, DGEngineSpeed = 902, DGEngineLoad = 193, DGEngineRunHour = 17, DGLOInPress = 0, DGLOFilterInPress = 0, DGControlAirPress = 0, DGTCLOPress = 0, DGEngineRunning = 1, DGUTemp = 39.2, DGVTemp = 38.7, DGWTemp = 37.4, DGBTDTemp = 38, DGCyl1ExTemp = 279, DGCyl2ExTemp = 294, DGCyl3ExTemp = 283, DGCyl4ExTemp = 277, DGCyl5ExTemp = 282, DGCyl6ExTemp = 291, DGTCEXOutTemp = 296, DGBoostAirPress = 0.1, DGFOInPress = 0.7, Uploaded = 0, Number = "SAD1", ReceiveDatetime = DateTime.UtcNow, DeviceNo = "MAINGENERATORSET3" } },
                            MainSwitchboard = new List<MainSwitchboardDto>() { new MainSwitchboardDto { Id = 162352, MBVoltageHigh = 0, MBVoltageLow = 0, MBFrequencyHigh = 0, MBFrequencyLow = 0, DGRunning = 1, DGPower = 208, DGVoltageL1L2 = 449.6, DGVoltageL2L3 = 449.7, DGVoltageL3L1 = 449.7, DGCurrentL1 = 326.3, DGCurrentL2 = 328.8, DGCurrentL3 = 328.5, DGFrequency = 60.1, DGPowerFactor = 0.8, Uploaded = 0, Number = "SAD1", ReceiveDatetime = DateTime.UtcNow, DeviceNo = "MAINSWITCHBOARD1" }, new MainSwitchboardDto { Id = 162353, MBVoltageHigh = 0, MBVoltageLow = 0, MBFrequencyHigh = 0, MBFrequencyLow = 0, DGRunning = 0, DGPower = 0, DGVoltageL1L2 = 0, DGVoltageL2L3 = 0, DGVoltageL3L1 = 0, DGCurrentL1 = 0, DGCurrentL2 = 0, DGCurrentL3 = 0, DGFrequency = 0, DGPowerFactor = 1, Uploaded = 0, Number = "SAD1", ReceiveDatetime = DateTime.UtcNow, DeviceNo = "MAINSWITCHBOARD2" }, new MainSwitchboardDto { Id = 162354, MBVoltageHigh = 0, MBVoltageLow = 0, MBFrequencyHigh = 0, MBFrequencyLow = 0, DGRunning = 1, DGPower = 190.1, DGVoltageL1L2 = 449.9, DGVoltageL2L3 = 450.1, DGVoltageL3L1 = 449.8, DGCurrentL1 = 310.8, DGCurrentL2 = 313.3, DGCurrentL3 = 313.3, DGFrequency = 60.1, DGPowerFactor = 0.7, Uploaded = 0, Number = "SAD1", ReceiveDatetime = DateTime.UtcNow, DeviceNo = "MAINSWITCHBOARD3" } },
                            MERemoteControl = new List<MERemoteControlDto>() { new MERemoteControlDto { Id = 53212, MERpm = -0.5, Uploaded = 0, Number = "SAD1", ReceiveDatetime = DateTime.UtcNow, DeviceNo = "MEREMOTECONTROL" } },
                            Miscellaneous = new List<MiscellaneousDto>() { new MiscellaneousDto { Id = 53398, MECCOMHigh = 0, MEAxialVibration = 0.6, MELoad = 0, METCSpeed = 327.68, Uploaded = 0, Number = "SAD1", ReceiveDatetime = DateTime.UtcNow, DeviceNo = "MISCELLANEOUS" } },
                            ScavengeAir = new List<ScavengeAirDto>() { new ScavengeAirDto { Id = 51091, MEReceiverTemp = 43, MEFBCyl1Temp = 43, MEFBCyl2Temp = 44, MEFBCyl3Temp = 45, MEFBCyl4Temp = 44, MEFBCyl5Temp = 44, MEFBCyl6Temp = 45, MEPress = 0.1, MECoolerPressDrop = 0, METCInTempA = 26, METCInTempB = 26, ERRelativeHumidity = 34, ERTemp = 14, ERAmbientPress = 1030.8, Uploaded = 0, Number = "SAD1", ReceiveDatetime = DateTime.UtcNow, DeviceNo = "SCAVENGEAIR" } },
                            ShaftClutch = new List<ShaftClutchDto>() { new ShaftClutchDto { Id = 53235, SternAftTemp = 21, InterTemp = 31, Uploaded = 0, Number = "SAD1", ReceiveDatetime = DateTime.UtcNow, DeviceNo = "SHAFTCLUTCH" } },
                        };
                        break;

                    case "ee":
                        result = new VesselInfo();
                        break;

                    default:
                        result = await _vesselInfoService.GetLatestAsync(request.Number);
                        //result = new VesselInfo();
                        var tempResult = result.ToJson().ToJObject();
                        var fs = new JProperty("Flowmeters", JArray.FromObject(new List<Flowmeter>
                        {
                            new Flowmeter{
                                ConsAct=222,
                                ConsAcc=333,
                                Temperature=30,
                                Density=1.1m,
                                DeviceType="me",
                                FuelType="HFO",
                                DeviceNo="mefuel1"
                            },
                            new Flowmeter{
                                ConsAct=234,
                                ConsAcc=357,
                                Temperature=30,
                                Density=1.1m,
                                DeviceType="me",
                                FuelType="HFO",
                                DeviceNo="mefuel2"
                            },
                            new Flowmeter{
                                ConsAct=45,
                                ConsAcc=95,
                                Temperature=20,
                                Density=0.8m,
                                DeviceType="me",
                                FuelType="Methanol",
                                DeviceNo="memethanol"
                            }
                        }));
                        tempResult.Add(fs);
                        var ss = new JProperty("Shafts", JArray.FromObject(new List<Shaft>
                        {
                            new Shaft{
                                Power=300,
                                RPM=240,
                                Torque=240,
                                Thrust=240,
                                DeviceNo="shaft2"
                            },
                            new Shaft{
                                Power=200,
                                RPM=180,
                                Torque=180,
                                Thrust=180,
                                DeviceNo="shaft1"
                            }
                        }));
                        tempResult.Add(ss);

                        tempResult.Add(new JProperty("CEmission", 0));
                        tempResult.Add(new JProperty("PreRtCII", 0));
                        tempResult.Add(new JProperty("PreHFO", 0));
                        tempResult.Add(new JProperty("PreTW", 0));
                        tempResult.Add(new JProperty("PreCO2", 0));
                        tempResult.Add(new JProperty("PreCEmission", 0));
                        tempResult.Add(new JProperty("ECStatus", "--"));
                        tempResult.Add(new JProperty("EEStatus", "--"));

                        result = tempResult;
                        break;
                }
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
                        if (tempDatetime.AddSeconds(interval) > item.ReceiveDatetime)
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
                                resultDict[strType].Add(((item.MEPower ?? 0) * 1000).ToString());
                                break;

                            case "FCPerNm":
                                resultDict[strType].Add(item.FCPerNm.ToString());
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

                            case "MEFCPerNm":
                                resultDict[strType].Add((item.MEFCPerNm ?? 0).ToString());
                                break;

                            case "AEFCPerNm":
                                resultDict[strType].Add((item.DGFCPerNm ?? 0).ToString());
                                break;

                            case "BLRFCPerNm":
                                resultDict[strType].Add((item.BLRFCPerNm ?? 0).ToString());
                                break;

                            case "MEHFOCPerNm":
                                resultDict[strType].Add((item.MEHFOCPerNm ?? 0).ToString());
                                break;

                            case "DGHFOCPerNm":
                                resultDict[strType].Add((item.DGHFOCPerNm ?? 0).ToString());
                                break;

                            case "BLRHFOCPerNm":
                                resultDict[strType].Add((item.BLRHFOCPerNm ?? 0).ToString());
                                break;

                            case "MEMDOCPerNm":
                                resultDict[strType].Add((item.MEMDOCPerNm ?? 0).ToString());
                                break;

                            case "DGMDOCPerNm":
                                resultDict[strType].Add((item.DGMDOCPerNm ?? 0).ToString());
                                break;

                            case "BLRMDOCPerNm":
                                resultDict[strType].Add((item.BLRMDOCPerNm ?? 0).ToString());
                                break;
                        }
                    }
                    resultDict["history_currenttime"].Add(item.ReceiveDatetime.ToString("yyyy-MM-dd HH:mm:ss"));
                }
                resultResponse.Result = Value.Parser.ParseJson(resultDict.ToJson());
            }
            else if (request.Method == "map")
            {
                var voyageListResponse = new VoyageInfoListResp();
                var deviceNo = "SAD1";
                var hasLoadStatus = false;
                var queryParams = request.Parameters.ToJObject();
                if (queryParams.ContainsKey("number") && !string.IsNullOrWhiteSpace(queryParams["number"].ToString()))
                {
                    deviceNo = queryParams["number"].ToString();
                }

                if (queryParams.ContainsKey("LoadStatus") && !string.IsNullOrWhiteSpace(queryParams["LoadStatus"].ToString()))
                {
                    hasLoadStatus = true;
                    var loadStatus = queryParams["LoadStatus"].ToString();
                    var beginDt = DateTime.UtcNow.AddYears(-30).ToString("yyyy-MM-dd HH:mm:ss");
                    var endDt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                    if (queryParams.ContainsKey("StartDate") && !string.IsNullOrWhiteSpace(queryParams["StartDate"].ToString()))
                        beginDt = Convert.ToDateTime(queryParams["StartDate"]).ToString("yyyy-MM-dd HH:mm:ss");
                    if (queryParams.ContainsKey("EndDate") && !string.IsNullOrWhiteSpace(queryParams["EndDate"].ToString()))
                        endDt = Convert.ToDateTime(queryParams["EndDate"]).ToString("yyyy-MM-dd HH:mm:ss");

                    using (var _channel = await _consulService.GetGrpcChannelAsync("base-srv"))
                    {
                        var client = new Base.BaseClient(_channel);
                        var deviceResponse = await client.GetDeviceByNumberAsync(new DeviceRequest { Number = deviceNo });
                        var shipId = deviceResponse.DeviceInfo.ShipId;
                        voyageListResponse = await client.GetVoyageInfoListAsync(new VoyageInfoListReq { ShipId = shipId, DateFrom = beginDt, DateTo = endDt, LoadStatus = loadStatus == "loaded" ? 2 : 1, PageInfo = new PageInfo { Pn = 1, Ps = 1000 } });
                    }
                }

                var lineList = new List<List<string>>();
                double? lastMEFC = 0f;
                double? meAcc = 0f;
                double? dgAcc = 0f;
                double? blrAcc = 0f;

                var paramDts = string.Empty;
                var paramDtLast = string.Empty;

                double? meAcc1 = null;
                double? dgAcc1 = null;
                double? blrAcc1 = null;
                double? meAccDGO = 0f;
                double? dgAccDGO = 0f;
                double? blrAccDGO = 0f;
                double? meAccDGO1 = null;
                double? dgAccDGO1 = null;
                double? blrAccDGO1 = null;

                if (!hasLoadStatus || hasLoadStatus && voyageListResponse.Total > 0)
                {
                    var result = await _vesselInfoService.GetListMap(request.Number, request.Parameters);
                    var notMatch = false;

                    foreach (var item in result)
                    {
                        if (item?.Latitude == null || item?.Longitude == null)
                            continue;
                        var i = 0;
                        foreach (var voyage in voyageListResponse.List)
                        {
                            if (i == 0)
                            {
                                if (voyage.ArrivalTime == 0)
                                    voyage.ArrivalTime = (int)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                            }
                            i++;
                            if (item.ReceiveDatetime >= DateTimeOffset.FromUnixTimeSeconds(voyage.DepartureTime).UtcDateTime && item.ReceiveDatetime <= DateTimeOffset.FromUnixTimeSeconds(voyage.ArrivalTime).UtcDateTime)
                            {
                                notMatch = false;
                                break;
                            }
                            notMatch = true;
                        }
                        if (notMatch)
                        {
                            notMatch = false;
                            continue;
                        }
                        var coor = new pointLatLon(Convert.ToDouble(item?.Latitude), Convert.ToDouble(item?.Longitude));
                        if (lineList.Count == 0)
                        {
                            lineList.Add(new List<string> { (item?.FCPerNm).ToString() });
                            lineList[lineList.Count - 1].Add(coor.Lat + "," + coor.Lon);
                            paramDts = item.ReceiveDatetime.ToString("yyyy-MM-dd HH:mm:ss");
                            paramDtLast = item.ReceiveDatetime.ToString("yyyy-MM-dd HH:mm:ss");
                        }
                        else
                        {
                            if (item?.FCPerNm <= 1000 && lastMEFC <= 1000 || item?.FCPerNm > 1500 && lastMEFC > 1500 || item?.FCPerNm > 1000 && item?.FCPerNm <= 1500 && lastMEFC > 1000 && lastMEFC <= 1500)
                            {
                                lineList[lineList.Count - 1].Add(coor.Lat + "," + coor.Lon);
                                paramDtLast = item.ReceiveDatetime.ToString("yyyy-MM-dd HH:mm:ss");
                            }
                            else
                            {
                                paramDts += "," + item.ReceiveDatetime.ToString("yyyy-MM-dd HH:mm:ss");
                                lineList[lineList.Count - 1].Add(paramDts);
                                var temp = lineList[lineList.Count - 1][(lineList[lineList.Count - 1]).Count - 2];
                                lineList.Add(new List<string> { item?.FCPerNm.ToString() });
                                lineList[lineList.Count - 1].Add(temp);
                                lineList[lineList.Count - 1].Add(coor.Lat + "," + coor.Lon);
                                paramDts = item.ReceiveDatetime.ToString("yyyy-MM-dd HH:mm:ss");
                                paramDtLast = item.ReceiveDatetime.ToString("yyyy-MM-dd HH:mm:ss");
                            }
                        }
                        lastMEFC = (item?.WaterSpeed ?? 0) != 0 ? item?.FCPerNm : lastMEFC;
                        meAcc = item?.MEHFOCACC ?? 0;
                        dgAcc = item?.DGHFOCACC ?? 0;
                        blrAcc = item?.BLGHFOCACC ?? 0;

                        meAccDGO = item?.MEMDOCACC ?? 0;
                        dgAccDGO = item?.DGMDOCACC ?? 0;
                        blrAccDGO = item?.BLGMDOCACC ?? 0;

                        if (meAcc1 == null)
                            meAcc1 = item?.MEHFOCACC ?? 0;
                        if (dgAcc1 == null)
                            dgAcc1 = item?.DGHFOCACC ?? 0;
                        if (blrAcc1 == null)
                            blrAcc1 = item?.BLGHFOCACC ?? 0;
                        if (meAccDGO1 == null)
                            meAccDGO1 = item?.MEMDOCACC ?? 0;
                        if (dgAccDGO1 == null)
                            dgAccDGO1 = item?.DGMDOCACC ?? 0;
                        if (blrAccDGO1 == null)
                            blrAccDGO1 = item?.BLGMDOCACC ?? 0;
                    }
                }
                if (lineList.Count > 0)
                    lineList[lineList.Count - 1].Add(paramDts + "," + paramDtLast);
                var newresult = new
                {
                    lineList,
                    meAcc = meAcc - (meAcc1 ?? 0),
                    dgAcc = dgAcc - (dgAcc1 ?? 0),
                    blrAcc = blrAcc - (blrAcc1 ?? 0),
                    meAccDGO = meAccDGO - (meAccDGO1 ?? 0),
                    dgAccDGO = dgAccDGO - (dgAccDGO1 ?? 0),
                    blrAccDGO = blrAccDGO - (blrAccDGO1 ?? 0)
                };
                resultResponse.Result = Value.Parser.ParseJson(newresult.ToJson());
            }
            else if (request.Method == "mapWeather")
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
            return resultResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Namespace + "_" + MethodBase.GetCurrentMethod().Name);
            throw new Exception("210001");
        }
    }

    public override async Task<EnergyDistributionResponse> NavigationEnergyDistribution(EnergyDistributionRequest request, ServerCallContext context)
    {
        var response = new EnergyDistributionResponse();
        var parameters = request.Parameters.ToJObject();
        if (parameters.ContainsKey("number") && !string.IsNullOrWhiteSpace("number"))
        {
            try
            {
                var result = await _vesselInfoService.GetEnergyDistributionAsync(request.Parameters);
                var η = Math.Round(result.Eo / (result.Et == 0 ? 1 : result.Et) * new Random().Next(90, 110), 3);
                foreach (var prop in result.GetType().GetProperties())
                {
                    if (prop.PropertyType == typeof(double) ||
                        prop.PropertyType == typeof(float) ||
                        prop.PropertyType == typeof(decimal) ||
                        prop.PropertyType == typeof(double?) ||
                        prop.PropertyType == typeof(float?) ||
                        prop.PropertyType == typeof(decimal?))
                        prop.SetValue(result, (decimal)Math.Round(Convert.ToDouble(prop.GetValue(result)), 2));
                }
                var calcResult = result.ToJson().ToJObject();
                var AdvMsg = string.Empty;
                if (request.Language == "en_US")
                    AdvMsg = $"The energy utilization efficiency of the entire ship is {η}%。\nThe main fuel consumption units of this ship are the main engine, auxiliary engine, and boiler, which consume energy and energy {result.Emei}MJ。Effective work is {result.Eo}Mj。Work loss {result.Et - result.Eo}MJ。\nEnergy conservation and efficient utilization of ships should focus on the following aspects: improving the efficiency of the main engine, enhancing propulsion efficiency, reducing ship resistance, recovering exhaust heat, and applying clean energy.";
                else
                    AdvMsg = $"船舶整船能量利用效率为{η}%。\n本船主要燃料消耗单元为四台双燃料发动机，消耗能源能量{result.Emei}MJ。有效做功分布在主机T/C回收、发电机传动推进、发电及电池充电等，有效做功为{result.Eo}Mj。损失分布在排烟、冷却、传动及推进损失、电站损失、电网及负载损失，损失功{result.Et - result.Eo}MJ。\n船舶节约能量、提高能量有效利用应着重从以下几个方面开展：提高主机效率、提高推进效率、降低船体阻力、排烟余热回收、清洁能源应用等。";
                calcResult.Add(new JProperty("Analysis", AdvMsg));
                response.Result = Value.Parser.ParseJson(calcResult.ToJson());
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Namespace + "_" + MethodBase.GetCurrentMethod().Name);
                throw new Exception("210001");
            }
        }
        else
            throw new Exception("未传入参数船舶采集系统设备编号。");
    }
}