using hmt_energy_csharp.Devices;
using hmt_energy_csharp.Dtos;
using hmt_energy_csharp.Energy.Batteries;
using hmt_energy_csharp.Energy.Configs;
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
using hmt_energy_csharp.VesselInfos;
using System.Collections.Generic;

namespace hmt_energy_csharp.StaticEntities
{
    public static class StaticEntities
    {
        public static IList<ProtocolParam> ProtocolParams { get; set; } = new List<ProtocolParam>();

        public static IList<VesselInfoDto> Vessels { get; set; } = new List<VesselInfoDto>();

        public static IList<TotalIndicatorDto> TotalIndicators { get; set; } = new List<TotalIndicatorDto>();

        public static IList<PredictionDto> Predictions { get; set; } = new List<PredictionDto>();

        public static IList<VesselBatteryDto> Batteries { get; set; } = new List<VesselBatteryDto>();

        public static IList<VesselFlowmeterDto> Flowmeters { get; set; } = new List<VesselFlowmeterDto>();

        public static IList<VesselGeneratorDto> Generators { get; set; } = new List<VesselGeneratorDto>();

        public static IList<VesselLiquidLevelDto> LiquidLevels { get; set; } = new List<VesselLiquidLevelDto>();

        public static IList<VesselShaftDto> Shafts { get; set; } = new List<VesselShaftDto>();

        public static IList<VesselSternSealingDto> SternSealings { get; set; } = new List<VesselSternSealingDto>();

        public static IList<VesselSupplyUnitDto> SupplyUnits { get; set; } = new List<VesselSupplyUnitDto>();

        public static IList<VesselPowerUnitDto> PowerUnits { get; set; } = new List<VesselPowerUnitDto>();

        public static IList<FilteringParam> FilteringParams { get; set; } = new List<FilteringParam>();

        public static IList<MonitoredDevice> MonitoredDevices { get; set; } = new List<MonitoredDevice>();

        public static IList<ConfigDto> Configs { get; set; } = new List<ConfigDto>();

        public static IList<SpeedParam> SpeedParams { get; set; } = new List<SpeedParam>();

        public static TcpConfigParam tcpConfigParam { get; set; } = new TcpConfigParam();

        public static IList<DailyConsumption> DailyConsumptions { get; set; } = new List<DailyConsumption>();

        public static IList<VesselCompositeBoilerDto> CompositeBoilers { get; set; } = new List<VesselCompositeBoilerDto>();
        public static IList<VesselCompressedAirSupplyDto> CompressedAirSupplies { get; set; } = new List<VesselCompressedAirSupplyDto>();
        public static IList<VesselCoolingFreshWaterDto> CoolingFreshWaters { get; set; } = new List<VesselCoolingFreshWaterDto>();
        public static IList<VesselCoolingSeaWaterDto> CoolingSeaWaters { get; set; } = new List<VesselCoolingSeaWaterDto>();
        public static IList<VesselCoolingWaterDto> CoolingWaters { get; set; } = new List<VesselCoolingWaterDto>();
        public static IList<VesselCylinderLubOilDto> CylinderLubOils { get; set; } = new List<VesselCylinderLubOilDto>();
        public static IList<VesselExhaustGasDto> ExhaustGases { get; set; } = new List<VesselExhaustGasDto>();
        public static IList<VesselFODto> FOs { get; set; } = new List<VesselFODto>();
        public static IList<VesselFOSupplyUnitDto> FOSupplyUnits { get; set; } = new List<VesselFOSupplyUnitDto>();
        public static IList<VesselLubOilPurifyingDto> LubOilPurifyings { get; set; } = new List<VesselLubOilPurifyingDto>();
        public static IList<VesselLubOilDto> LubOils { get; set; } = new List<VesselLubOilDto>();
        public static IList<VesselMainGeneratorSetDto> MainGeneratorSets { get; set; } = new List<VesselMainGeneratorSetDto>();
        public static IList<VesselMainSwitchboardDto> MainSwitchboards { get; set; } = new List<VesselMainSwitchboardDto>();
        public static IList<VesselMERemoteControlDto> MERemoteControls { get; set; } = new List<VesselMERemoteControlDto>();
        public static IList<VesselMiscellaneousDto> Miscellaneouses { get; set; } = new List<VesselMiscellaneousDto>();
        public static IList<VesselScavengeAirDto> ScavengeAirs { get; set; } = new List<VesselScavengeAirDto>();
        public static IList<VesselShaftClutchDto> ShaftClutchs { get; set; } = new List<VesselShaftClutchDto>();
        public static IList<VesselAssistantDecisionDto> AssistantDecisions { get; set; } = new List<VesselAssistantDecisionDto>() { new VesselAssistantDecisionDto() { Number = "NDY1273" } };
    }
}