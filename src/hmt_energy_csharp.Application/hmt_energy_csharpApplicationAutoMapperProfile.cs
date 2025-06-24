using AutoMapper;
using hmt_energy_csharp.Connections;
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
using hmt_energy_csharp.Sentences;
using hmt_energy_csharp.VDRs;
using hmt_energy_csharp.VesselInfos;
using hmt_energy_csharp.WhiteLists;

namespace hmt_energy_csharp;

public class hmt_energy_csharpApplicationAutoMapperProfile : Profile
{
    public hmt_energy_csharpApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */
        CreateMap<Connection, ConnectionDto>();
        CreateMap<Sentence, SentenceDto>();

        CreateMap<WhiteList, WhiteListDto>();

        CreateMap<VdrTotalEntity, VdrEntityDto>();

        CreateMap<VesselInfoDto, VesselInfo>();
        CreateMap<VesselInfo, VesselInfoDto>();
        CreateMap<VesselInfo, MapDto>();

        CreateMap<TotalIndicatorDto, TotalIndicator>();
        CreateMap<PredictionDto, Prediction>();
        CreateMap<BatteryDto, Battery>();
        CreateMap<FlowmeterDto, Flowmeter>();
        CreateMap<GeneratorDto, Generator>();
        CreateMap<LiquidLevelDto, LiquidLevel>();
        CreateMap<ShaftDto, Shaft>();
        CreateMap<SternSealingDto, SternSealing>();
        CreateMap<SupplyUnitDto, SupplyUnit>();
        CreateMap<PowerUnitDto, PowerUnit>();

        CreateMap<TotalIndicator, TotalIndicatorDto>();
        CreateMap<Prediction, PredictionDto>();
        CreateMap<Battery, BatteryDto>();
        CreateMap<Flowmeter, FlowmeterDto>();
        CreateMap<Generator, GeneratorDto>();
        CreateMap<LiquidLevel, LiquidLevelDto>();
        CreateMap<Shaft, ShaftDto>();
        CreateMap<SternSealing, SternSealingDto>();
        CreateMap<PowerUnit, PowerUnitDto>();

        CreateMap<ConfigDto, Config>();
        CreateMap<Config, ConfigDto>();

        CreateMap<CompositeBoilerDto, CompositeBoiler>();
        CreateMap<CompositeBoiler, CompositeBoilerDto>();
        CreateMap<CompressedAirSupplyDto, CompressedAirSupply>();
        CreateMap<CompressedAirSupply, CompressedAirSupplyDto>();
        CreateMap<CoolingFreshWaterDto, CoolingFreshWater>();
        CreateMap<CoolingFreshWater, CoolingFreshWaterDto>();
        CreateMap<CoolingSeaWaterDto, CoolingSeaWater>();
        CreateMap<CoolingSeaWater, CoolingSeaWaterDto>();
        CreateMap<CoolingWaterDto, CoolingWater>();
        CreateMap<CoolingWater, CoolingWaterDto>();
        CreateMap<CylinderLubOilDto, CylinderLubOil>();
        CreateMap<CylinderLubOil, CylinderLubOilDto>();
        CreateMap<ExhaustGasDto, ExhaustGas>();
        CreateMap<ExhaustGas, ExhaustGasDto>();
        CreateMap<FODto, FO>();
        CreateMap<FO, FODto>();
        CreateMap<FOSupplyUnitDto, FOSupplyUnit>();
        CreateMap<FOSupplyUnit, FOSupplyUnitDto>();
        CreateMap<LubOilPurifyingDto, LubOilPurifying>();
        CreateMap<LubOilPurifying, LubOilPurifyingDto>();
        CreateMap<LubOilDto, LubOil>();
        CreateMap<LubOil, LubOilDto>();
        CreateMap<MainGeneratorSetDto, MainGeneratorSet>();
        CreateMap<MainGeneratorSet, MainGeneratorSetDto>();
        CreateMap<MainSwitchboardDto, MainSwitchboard>();
        CreateMap<MainSwitchboard, MainSwitchboardDto>();
        CreateMap<MERemoteControlDto, MERemoteControl>();
        CreateMap<MERemoteControl, MERemoteControlDto>();
        CreateMap<MiscellaneousDto, Miscellaneous>();
        CreateMap<Miscellaneous, MiscellaneousDto>();
        CreateMap<ScavengeAirDto, ScavengeAir>();
        CreateMap<ScavengeAir, ScavengeAirDto>();
        CreateMap<ShaftClutchDto, ShaftClutch>();
        CreateMap<ShaftClutch, ShaftClutchDto>();
        CreateMap<AssistantDecisionDto, AssistantDecision>();
        CreateMap<AssistantDecision, AssistantDecisionDto>();
    }
}