using hmt_energy_csharp.Devices;
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
using hmt_energy_csharp.VesselInfos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hmt_energy_csharp.StaticEntities
{
    public static class StaticBTEntities
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
    }
}