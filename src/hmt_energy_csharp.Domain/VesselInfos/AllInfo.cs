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
using System.Collections.Generic;

namespace hmt_energy_csharp.VesselInfos
{
    public class AllInfo
    {
        public IList<Flowmeter> flowmeters { get; set; }
        public IList<Battery> batteries { get; set; }
        public IList<Generator> generators { get; set; }
        public IList<LiquidLevel> liquidlevels { get; set; }
        public IList<PowerUnit> powerunits { get; set; }
        public IList<Prediction> predictions { get; set; }
        public IList<Shaft> shafts { get; set; }
        public IList<SternSealing> sternsealings { get; set; }
        public IList<SupplyUnit> supplyunits { get; set; }
        public IList<TotalIndicator> totalindicators { get; set; }
        public IList<AssistantDecision> assistantdecisions { get; set; }
        public IList<CompositeBoiler> compositeboilers { get; set; }
        public IList<CompressedAirSupply> compressedairsupplies { get; set; }
        public IList<CoolingFreshWater> coolingfreshwaters { get; set; }
        public IList<CoolingSeaWater> coolingseawaters { get; set; }
        public IList<CoolingWater> coolingwaters { get; set; }
        public IList<CylinderLubOil> cylinderluboils { get; set; }
        public IList<ExhaustGas> exhaustgases { get; set; }
        public IList<FO> fos { get; set; }
        public IList<FOSupplyUnit> fosupplyunits { get; set; }
        public IList<LubOil> luboils { get; set; }
        public IList<LubOilPurifying> luboilpurifyings { get; set; }
        public IList<MainGeneratorSet> maingeneratorsets { get; set; }
        public IList<MainSwitchboard> mainswitchboards { get; set; }
        public IList<MERemoteControl> meremotecontrols { get; set; }
        public IList<Miscellaneous> miscellaneous { get; set; }
        public IList<ScavengeAir> scavengeairs { get; set; }
        public IList<ShaftClutch> shaftclutches { get; set; }
    }
}
