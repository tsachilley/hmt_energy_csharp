using Grpc.Core;
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
using hmt_energy_csharp.Engineroom.AssistantDecisions;
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
using System;
using System.Linq;
using System.Threading.Tasks;

namespace hmt_energy_csharp.Services;

public class CloudService : Cloud.CloudBase
{
    public override async Task<RecieveResponse> Recieve(RecieveRequest request, ServerCallContext context)
    {
        var response = new RecieveResponse();
        response.Result = 0;

        var number = string.Empty;

        try
        {
            var strMsg = request.RecieveMsg.ToJObject();
            var vessel = strMsg["vessel"].ToJson().ToObject<VesselInfoDto>();
            if (vessel != null)
            {
                number = vessel.SN;

                var flowmeter = strMsg["flowmeter"].ToJson().ToObject<VesselFlowmeterDto>();
                var battery = strMsg["battery"].ToJson().ToObject<VesselBatteryDto>();
                var generator = strMsg["generator"].ToJson().ToObject<VesselGeneratorDto>();
                var liquidLevel = strMsg["liquidLevel"].ToJson().ToObject<VesselLiquidLevelDto>();
                var supplyUnit = strMsg["supplyUnit"].ToJson().ToObject<VesselSupplyUnitDto>();
                var shaft = strMsg["shaft"].ToJson().ToObject<VesselShaftDto>();
                var sternSealing = strMsg["sternSealing"].ToJson().ToObject<VesselSternSealingDto>();
                var powerUnit = strMsg["powerUnit"].ToJson().ToObject<VesselPowerUnitDto>();

                var totalIndicator = strMsg["totalIndicator"].ToJson().ToObject<TotalIndicatorDto>();

                var prediction = strMsg["prediction"].ToJson().ToObject<PredictionDto>();

                var AssistantDecisions = strMsg["AssistantDecisions"].ToJson().ToObject<VesselAssistantDecisionDto>();
                var CompositeBoilers = strMsg["CompositeBoilers"].ToJson().ToObject<VesselCompositeBoilerDto>();
                var CompressedAirSupplies = strMsg["CompressedAirSupplies"].ToJson().ToObject<VesselCompressedAirSupplyDto>();
                var CoolingFreshWaters = strMsg["CoolingFreshWaters"].ToJson().ToObject<VesselCoolingFreshWaterDto>();
                var CoolingSeaWaters = strMsg["CoolingSeaWaters"].ToJson().ToObject<VesselCoolingSeaWaterDto>();
                var CoolingWaters = strMsg["CoolingWaters"].ToJson().ToObject<VesselCoolingWaterDto>();
                var CylinderLubOils = strMsg["CylinderLubOils"].ToJson().ToObject<VesselCylinderLubOilDto>();
                var ExhaustGases = strMsg["ExhaustGases"].ToJson().ToObject<VesselExhaustGasDto>();
                var FOs = strMsg["FOs"].ToJson().ToObject<VesselFODto>();
                var FOSupplyUnits = strMsg["FOSupplyUnits"].ToJson().ToObject<VesselFOSupplyUnitDto>();
                var LubOilPurifyings = strMsg["LubOilPurifyings"].ToJson().ToObject<VesselLubOilPurifyingDto>();
                var LubOils = strMsg["LubOils"].ToJson().ToObject<VesselLubOilDto>();
                var MainGeneratorSets = strMsg["MainGeneratorSets"].ToJson().ToObject<VesselMainGeneratorSetDto>();
                var MainSwitchboards = strMsg["MainSwitchboards"].ToJson().ToObject<VesselMainSwitchboardDto>();
                var MERemoteControls = strMsg["MERemoteControls"].ToJson().ToObject<VesselMERemoteControlDto>();
                var Miscellaneouses = strMsg["Miscellaneouses"].ToJson().ToObject<VesselMiscellaneousDto>();
                var ScavengeAirs = strMsg["ScavengeAirs"].ToJson().ToObject<VesselScavengeAirDto>();
                var ShaftClutchs = strMsg["ShaftClutchs"].ToJson().ToObject<VesselShaftClutchDto>();

                if (StaticEntities.ShowEntities.Vessels.Any(t => t.SN == number))
                    StaticEntities.ShowEntities.Vessels[StaticEntities.ShowEntities.Vessels.IndexOf(StaticEntities.ShowEntities.Vessels.FirstOrDefault(t => t.SN == number))] = vessel;
                else
                    StaticEntities.ShowEntities.Vessels.Add(vessel);

                if (StaticEntities.ShowEntities.Flowmeters.Any(t => t.Number == number))
                    StaticEntities.ShowEntities.Flowmeters[StaticEntities.ShowEntities.Flowmeters.IndexOf(StaticEntities.ShowEntities.Flowmeters.FirstOrDefault(t => t.Number == number))] = flowmeter;
                else
                    StaticEntities.ShowEntities.Flowmeters.Add(flowmeter);
                if (StaticEntities.ShowEntities.Batteries.Any(t => t.Number == number))
                    StaticEntities.ShowEntities.Batteries[StaticEntities.ShowEntities.Batteries.IndexOf(StaticEntities.ShowEntities.Batteries.FirstOrDefault(t => t.Number == number))] = battery;
                else
                    StaticEntities.ShowEntities.Batteries.Add(battery);
                if (StaticEntities.ShowEntities.Generators.Any(t => t.Number == number))
                    StaticEntities.ShowEntities.Generators[StaticEntities.ShowEntities.Generators.IndexOf(StaticEntities.ShowEntities.Generators.FirstOrDefault(t => t.Number == number))] = generator;
                else
                    StaticEntities.ShowEntities.Generators.Add(generator);
                if (StaticEntities.ShowEntities.LiquidLevels.Any(t => t.Number == number))
                    StaticEntities.ShowEntities.LiquidLevels[StaticEntities.ShowEntities.LiquidLevels.IndexOf(StaticEntities.ShowEntities.LiquidLevels.FirstOrDefault(t => t.Number == number))] = liquidLevel;
                else
                    StaticEntities.ShowEntities.LiquidLevels.Add(liquidLevel);
                if (StaticEntities.ShowEntities.SupplyUnits.Any(t => t.Number == number))
                    StaticEntities.ShowEntities.SupplyUnits[StaticEntities.ShowEntities.SupplyUnits.IndexOf(StaticEntities.ShowEntities.SupplyUnits.FirstOrDefault(t => t.Number == number))] = supplyUnit;
                else
                    StaticEntities.ShowEntities.SupplyUnits.Add(supplyUnit);
                if (StaticEntities.ShowEntities.Shafts.Any(t => t.Number == number))
                    StaticEntities.ShowEntities.Shafts[StaticEntities.ShowEntities.Shafts.IndexOf(StaticEntities.ShowEntities.Shafts.FirstOrDefault(t => t.Number == number))] = shaft;
                else
                    StaticEntities.ShowEntities.Shafts.Add(shaft);
                if (StaticEntities.ShowEntities.SternSealings.Any(t => t.Number == number))
                    StaticEntities.ShowEntities.SternSealings[StaticEntities.ShowEntities.SternSealings.IndexOf(StaticEntities.ShowEntities.SternSealings.FirstOrDefault(t => t.Number == number))] = sternSealing;
                else
                    StaticEntities.ShowEntities.SternSealings.Add(sternSealing);
                if (StaticEntities.ShowEntities.PowerUnits.Any(t => t.Number == number))
                    StaticEntities.ShowEntities.PowerUnits[StaticEntities.ShowEntities.PowerUnits.IndexOf(StaticEntities.ShowEntities.PowerUnits.FirstOrDefault(t => t.Number == number))] = powerUnit;
                else
                    StaticEntities.ShowEntities.PowerUnits.Add(powerUnit);

                if (StaticEntities.ShowEntities.TotalIndicators.Any(t => t.Number == number))
                    StaticEntities.ShowEntities.TotalIndicators[StaticEntities.ShowEntities.TotalIndicators.IndexOf(StaticEntities.ShowEntities.TotalIndicators.FirstOrDefault(t => t.Number == number))] = totalIndicator;
                else
                    StaticEntities.ShowEntities.TotalIndicators.Add(totalIndicator);

                if (StaticEntities.ShowEntities.Predictions.Any(t => t.Number == number))
                    StaticEntities.ShowEntities.Predictions[StaticEntities.ShowEntities.Predictions.IndexOf(StaticEntities.ShowEntities.Predictions.FirstOrDefault(t => t.Number == number))] = prediction;
                else
                    StaticEntities.ShowEntities.Predictions.Add(prediction);

                if (StaticEntities.ShowEntities.AssistantDecisions.Any(t => t.Number == number))
                    StaticEntities.ShowEntities.AssistantDecisions[StaticEntities.ShowEntities.AssistantDecisions.IndexOf(StaticEntities.ShowEntities.AssistantDecisions.FirstOrDefault(t => t.Number == number))] = AssistantDecisions;
                else
                    StaticEntities.ShowEntities.AssistantDecisions.Add(AssistantDecisions);

                if (StaticEntities.ShowEntities.CompositeBoilers.Any(t => t.Number == number))
                    StaticEntities.ShowEntities.CompositeBoilers[StaticEntities.ShowEntities.CompositeBoilers.IndexOf(StaticEntities.ShowEntities.CompositeBoilers.FirstOrDefault(t => t.Number == number))] = CompositeBoilers;
                else
                    StaticEntities.ShowEntities.CompositeBoilers.Add(CompositeBoilers);

                if (StaticEntities.ShowEntities.CompressedAirSupplies.Any(t => t.Number == number))
                    StaticEntities.ShowEntities.CompressedAirSupplies[StaticEntities.ShowEntities.CompressedAirSupplies.IndexOf(StaticEntities.ShowEntities.CompressedAirSupplies.FirstOrDefault(t => t.Number == number))] = CompressedAirSupplies;
                else
                    StaticEntities.ShowEntities.CompressedAirSupplies.Add(CompressedAirSupplies);

                if (StaticEntities.ShowEntities.CoolingFreshWaters.Any(t => t.Number == number))
                    StaticEntities.ShowEntities.CoolingFreshWaters[StaticEntities.ShowEntities.CoolingFreshWaters.IndexOf(StaticEntities.ShowEntities.CoolingFreshWaters.FirstOrDefault(t => t.Number == number))] = CoolingFreshWaters;
                else
                    StaticEntities.ShowEntities.CoolingFreshWaters.Add(CoolingFreshWaters);

                if (StaticEntities.ShowEntities.CoolingSeaWaters.Any(t => t.Number == number))
                    StaticEntities.ShowEntities.CoolingSeaWaters[StaticEntities.ShowEntities.CoolingSeaWaters.IndexOf(StaticEntities.ShowEntities.CoolingSeaWaters.FirstOrDefault(t => t.Number == number))] = CoolingSeaWaters;
                else
                    StaticEntities.ShowEntities.CoolingSeaWaters.Add(CoolingSeaWaters);

                if (StaticEntities.ShowEntities.CoolingWaters.Any(t => t.Number == number))
                    StaticEntities.ShowEntities.CoolingWaters[StaticEntities.ShowEntities.CoolingWaters.IndexOf(StaticEntities.ShowEntities.CoolingWaters.FirstOrDefault(t => t.Number == number))] = CoolingWaters;
                else
                    StaticEntities.ShowEntities.CoolingWaters.Add(CoolingWaters);

                if (StaticEntities.ShowEntities.CylinderLubOils.Any(t => t.Number == number))
                    StaticEntities.ShowEntities.CylinderLubOils[StaticEntities.ShowEntities.CylinderLubOils.IndexOf(StaticEntities.ShowEntities.CylinderLubOils.FirstOrDefault(t => t.Number == number))] = CylinderLubOils;
                else
                    StaticEntities.ShowEntities.CylinderLubOils.Add(CylinderLubOils);

                if (StaticEntities.ShowEntities.ExhaustGases.Any(t => t.Number == number))
                    StaticEntities.ShowEntities.ExhaustGases[StaticEntities.ShowEntities.ExhaustGases.IndexOf(StaticEntities.ShowEntities.ExhaustGases.FirstOrDefault(t => t.Number == number))] = ExhaustGases;
                else
                    StaticEntities.ShowEntities.ExhaustGases.Add(ExhaustGases);

                if (StaticEntities.ShowEntities.FOs.Any(t => t.Number == number))
                    StaticEntities.ShowEntities.FOs[StaticEntities.ShowEntities.FOs.IndexOf(StaticEntities.ShowEntities.FOs.FirstOrDefault(t => t.Number == number))] = FOs;
                else
                    StaticEntities.ShowEntities.FOs.Add(FOs);

                if (StaticEntities.ShowEntities.FOSupplyUnits.Any(t => t.Number == number))
                    StaticEntities.ShowEntities.FOSupplyUnits[StaticEntities.ShowEntities.FOSupplyUnits.IndexOf(StaticEntities.ShowEntities.FOSupplyUnits.FirstOrDefault(t => t.Number == number))] = FOSupplyUnits;
                else
                    StaticEntities.ShowEntities.FOSupplyUnits.Add(FOSupplyUnits);

                if (StaticEntities.ShowEntities.LubOilPurifyings.Any(t => t.Number == number))
                    StaticEntities.ShowEntities.LubOilPurifyings[StaticEntities.ShowEntities.LubOilPurifyings.IndexOf(StaticEntities.ShowEntities.LubOilPurifyings.FirstOrDefault(t => t.Number == number))] = LubOilPurifyings;
                else
                    StaticEntities.ShowEntities.LubOilPurifyings.Add(LubOilPurifyings);

                if (StaticEntities.ShowEntities.LubOils.Any(t => t.Number == number))
                    StaticEntities.ShowEntities.LubOils[StaticEntities.ShowEntities.LubOils.IndexOf(StaticEntities.ShowEntities.LubOils.FirstOrDefault(t => t.Number == number))] = LubOils;
                else
                    StaticEntities.ShowEntities.LubOils.Add(LubOils);

                if (StaticEntities.ShowEntities.MainGeneratorSets.Any(t => t.Number == number))
                    StaticEntities.ShowEntities.MainGeneratorSets[StaticEntities.ShowEntities.MainGeneratorSets.IndexOf(StaticEntities.ShowEntities.MainGeneratorSets.FirstOrDefault(t => t.Number == number))] = MainGeneratorSets;
                else
                    StaticEntities.ShowEntities.MainGeneratorSets.Add(MainGeneratorSets);

                if (StaticEntities.ShowEntities.MainSwitchboards.Any(t => t.Number == number))
                    StaticEntities.ShowEntities.MainSwitchboards[StaticEntities.ShowEntities.MainSwitchboards.IndexOf(StaticEntities.ShowEntities.MainSwitchboards.FirstOrDefault(t => t.Number == number))] = MainSwitchboards;
                else
                    StaticEntities.ShowEntities.MainSwitchboards.Add(MainSwitchboards);

                if (StaticEntities.ShowEntities.MERemoteControls.Any(t => t.Number == number))
                    StaticEntities.ShowEntities.MERemoteControls[StaticEntities.ShowEntities.MERemoteControls.IndexOf(StaticEntities.ShowEntities.MERemoteControls.FirstOrDefault(t => t.Number == number))] = MERemoteControls;
                else
                    StaticEntities.ShowEntities.MERemoteControls.Add(MERemoteControls);

                if (StaticEntities.ShowEntities.Miscellaneouses.Any(t => t.Number == number))
                    StaticEntities.ShowEntities.Miscellaneouses[StaticEntities.ShowEntities.Miscellaneouses.IndexOf(StaticEntities.ShowEntities.Miscellaneouses.FirstOrDefault(t => t.Number == number))] = Miscellaneouses;
                else
                    StaticEntities.ShowEntities.Miscellaneouses.Add(Miscellaneouses);

                if (StaticEntities.ShowEntities.ScavengeAirs.Any(t => t.Number == number))
                    StaticEntities.ShowEntities.ScavengeAirs[StaticEntities.ShowEntities.ScavengeAirs.IndexOf(StaticEntities.ShowEntities.ScavengeAirs.FirstOrDefault(t => t.Number == number))] = ScavengeAirs;
                else
                    StaticEntities.ShowEntities.ScavengeAirs.Add(ScavengeAirs);

                if (StaticEntities.ShowEntities.ShaftClutchs.Any(t => t.Number == number))
                    StaticEntities.ShowEntities.ShaftClutchs[StaticEntities.ShowEntities.ShaftClutchs.IndexOf(StaticEntities.ShowEntities.ShaftClutchs.FirstOrDefault(t => t.Number == number))] = ShaftClutchs;
                else
                    StaticEntities.ShowEntities.ShaftClutchs.Add(ShaftClutchs);

                response.Result = 1;
            }
            else
            {
                response.Result = 0;
                response.ErrMessage = "未收到数据";
            }
        }
        catch (Exception ex)
        {
            throw;
        }

        return response;
    }
}