using hmt_energy_csharp.Dtos;
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
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace hmt_energy_csharp.VesselInfos
{
    public interface IVesselInfoService : IApplicationService
    {
        Task<VesselInfoDto> InsertAsync(VesselInfoDto dto);

        Task<VesselInfoDto> GetLatestAsync(string number);

        Task<IList<VesselInfoDto>> GetListChart(string number, string parameters);

        Task<IList<MapDto>> GetListMap(string number, string parameters);

        Task<int> GetTotalCount(string number, string parameters);

        Task<IList<VesselInfoDto>> GetListPage(string number, int pageNumber, int countPerPage, string sorting, string asc, string parameters);

        Task InsertOrUpdateAsync(VesselInfoDto dto);

        Task<IList<DraftDistributionDTO>> GetDraftDistribution(string parameters);

        Task<IList<PowerDistributionDto>> GetPowerDistribution(string parameters);

        Task<IList<SlipDistributionDto>> GetSlipDistribution(string parameters);

        Task<IList<SpeedDistributionDto>> GetSpeedDistribution(string parameters);

        Task<IList<WindDirDistributionDto>> GetWindDirDistribution(string parameters);

        Task<IList<WindSpeedDistributionDto>> GetWindSpeedDistribution(string parameters);

        Task<DataTable> GetStatisticList(string parameters);

        Task<IList<SpeedDto>> GetSpeedList(string parameters);

        Task<IList<VFCSpdDto>> GetVFCSpdList(string parameters);

        Task<IList<MEFCPowDto>> GetMEFCPowList(string parameters);

        Task<IList<TrimDto>> GetTrimList(string parameters);

        Task<IList<VFCMESpdDto>> GetVFCMESpdList(string parameters);

        Task<IList<MESpdPropDto>> GetMESpdPropList(string parameters);

        Task<IList<PowSpdDto>> GetPowSpdList(string parameters);

        Task<IList<PowRpmDto>> GetPowRpmList(string parameters);

        Task<IList<HullDto>> GetHullList(string parameters);

        Task<IList<METuningDto>> GetMETuningList(string parameters);

        Task<IList<HullPropellerDto>> GetHullPropellerList(string parameters);

        Task<IList<MELoadPropDto>> GetMELoadPropList(string parameters);

        Task<IEnumerable<object>> GetAnalysisList(string parameters);

        Task<NoonDto> GetNoonData(string number, DateTimeOffset dateTimeOffset, DateTimeOffset departureDTS);

        Task<object> GetWeekBaseAsync(string parameters, string lang);

        Task<EnergyDistribution> GetEnergyDistributionAsync(string parameters);

        Task<object> GetMRVAsync(string parameters);

        /// <summary>
        /// 保存船舶航行实时信息
        /// </summary>
        /// <param name="vesselInfo">船舶基础信息</param>
        /// <param name="totalIndicator">总计信息 能耗、功率等</param>
        /// <param name="prediction">预测油耗</param>
        /// <param name="batteries">电池</param>
        /// <param name="flowmeters">能耗</param>
        /// <param name="generators">发电机(双燃料船为主机)</param>
        /// <param name="liquidLevels">液位遥测</param>
        /// <param name="shafts">轴</param>
        /// <param name="sternSealings">艉密封</param>
        /// <param name="supplyUnits">供给单元</param>
        /// <param name="powerUnits">动力单元总计</param>
        /// <returns></returns>
        Task SaveVesselAllAsync(VesselInfoDto vesselInfo, TotalIndicatorDto totalIndicator, PredictionDto prediction, IList<BatteryDto> batteries, IList<FlowmeterDto> flowmeters, IList<GeneratorDto> generators, IList<LiquidLevelDto> liquidLevels, IList<ShaftDto> shafts, IList<SternSealingDto> sternSealings, IList<SupplyUnitDto> supplyUnits, IList<PowerUnitDto> powerUnits);

        Task SaveERAllAsync(IList<CompositeBoilerDto> CompositeBoilers, IList<CompressedAirSupplyDto> CompressedAirSupplys, IList<CoolingFreshWaterDto> CoolingFreshWaters, IList<CoolingSeaWaterDto> CoolingSeaWaters, IList<CoolingWaterDto> CoolingWaters, IList<CylinderLubOilDto> CylinderLubOils, IList<ExhaustGasDto> ExhaustGases, IList<FODto> FOs, IList<FOSupplyUnitDto> FOSupplyUnits, IList<LubOilPurifyingDto> LubOilPurifyings, IList<LubOilDto> LubOils, IList<MainGeneratorSetDto> MainGeneratorSets, IList<MainSwitchboardDto> MainSwitchboards, IList<MERemoteControlDto> MERemoteControls, IList<MiscellaneousDto> Miscellaneouses, IList<ScavengeAirDto> ScavengeAirs, IList<ShaftClutchDto> ShaftClutchs);

        Task SaveERADAsync(VesselInfoDto vesselInfo, IList<AssistantDecisionDto> AssistantDecisions);

        /// <summary>
        /// 根据采集系统序列号和接收时间获取对象
        /// </summary>
        /// <param name="number"></param>
        /// <param name="receiveDatetimeFmtDt"></param>
        /// <returns></returns>
        Task<VesselInfoDto> GetByNumberReceiveDatetime(string number, DateTime receiveDatetimeFmtDt);
    }
}