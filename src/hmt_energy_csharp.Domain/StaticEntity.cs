using hmt_energy_csharp.Energy.Batteries;
using hmt_energy_csharp.Energy.Flowmeters;
using hmt_energy_csharp.Energy.Generators;
using hmt_energy_csharp.Energy.LiquidLevels;
using hmt_energy_csharp.Energy.Shafts;
using hmt_energy_csharp.Energy.SternSealings;
using hmt_energy_csharp.Energy.SupplyUnits;
using hmt_energy_csharp.VesselInfos;
using System.Collections;
using System.Collections.Generic;

namespace hmt_energy_csharp
{
    public static class StaticEntity
    {
        public static Dictionary<string, Dictionary<string, int>> PropertyCounters { get; set; } = new Dictionary<string, Dictionary<string, int>>();

        //实时能效
        public static Dictionary<string, VesselInfo> RealtimeVesselinfos { get; set; } = new Dictionary<string, VesselInfo>();

        //实时流量计
        public static Dictionary<string, IList<Flowmeter>> RealtimeFlowmeters { get; set; } = new Dictionary<string, IList<Flowmeter>>();

        //实时发电机信息
        public static Dictionary<string, IList<Generator>> RealtimeGenerators { get; set; } = new Dictionary<string, IList<Generator>>();

        //实时电池信息
        public static Dictionary<string, IList<Battery>> RealtimeBatteries { get; set; } = new Dictionary<string, IList<Battery>>();

        //实时轴信息
        public static Dictionary<string, IList<Shaft>> RealtimeShafts { get; set; } = new Dictionary<string, IList<Shaft>>();

        //实时液位设备信息
        public static Dictionary<string, IList<LiquidLevel>> RealtimeLiquidLevels { get; set; } = new Dictionary<string, IList<LiquidLevel>>();

        //实时供应单元信息
        public static Dictionary<string, IList<SupplyUnit>> RealtimeSupplyUnits { get; set; } = new Dictionary<string, IList<SupplyUnit>>();

        //实时供应单元信息
        public static Dictionary<string, IList<SternSealing>> RealtimeSternSealings { get; set; } = new Dictionary<string, IList<SternSealing>>();
    }
}