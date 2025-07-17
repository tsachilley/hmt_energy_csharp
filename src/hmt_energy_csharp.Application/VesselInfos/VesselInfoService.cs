using hmt_energy_csharp.Dtos;
using hmt_energy_csharp.Energy.Batteries;
using hmt_energy_csharp.Energy.Criterias;
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
using hmt_energy_csharp.Indexes;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OracleClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace hmt_energy_csharp.VesselInfos
{
    public class VesselInfoService : hmt_energy_csharpAppService, IVesselInfoService
    {
        private readonly ILogger<VesselInfoService> _logger;
        private readonly IVesselInfoRepository _repository;
        private readonly ITotalIndicatorRepository _totalIndicatorRepository;
        private readonly IPredictionRepository _predictionRepository;
        private readonly IBatteryRepository _batteryRepository;
        private readonly IFlowmeterRepository _flowmeterRepository;
        private readonly IGeneratorRepository _generatorRepository;
        private readonly ILiquidLevelRepository _liquidLevelRepository;
        private readonly IShaftRepository _shaftRepository;
        private readonly ISternSealingRepository _sternSealingRepository;
        private readonly ISupplyUnitRepository _supplyUnitRepository;
        private readonly IPowerUnitRepository _powerUnitRepository;
        private readonly ICIIService _ciiService;
        private readonly ISqlRepository _sqlRepository;

        private readonly ICompositeBoilerRepository _compositeBoilerRepository;
        private readonly ICompressedAirSupplyRepository _compressedAirSupplyRepository;
        private readonly ICoolingFreshWaterRepository _coolingFreshWaterRepository;
        private readonly ICoolingSeaWaterRepository _coolingSeaWaterRepository;
        private readonly ICoolingWaterRepository _coolingWaterRepository;
        private readonly ICylinderLubOilRepository _cylinderLubOilRepository;
        private readonly IExhaustGasRepository _exhaustGasRepository;
        private readonly IFORepository _fORepository;
        private readonly IFOSupplyUnitRepository _fOSupplyUnitRepository;
        private readonly ILubOilPurifyingRepository _lubOilPurifyingRepository;
        private readonly ILubOilRepository _lubOilRepository;
        private readonly IMainGeneratorSetRepository _mainGeneratorSetRepository;
        private readonly IMainSwitchboardRepository _mainSwitchboardRepository;
        private readonly IMERemoteControlRepository _mERemoteControlRepository;
        private readonly IMiscellaneousRepository _miscellaneousRepository;
        private readonly IScavengeAirRepository _scavengeAirRepository;
        private readonly IShaftClutchRepository _shaftClutchRepository;
        private readonly IAssistantDecisionRepository _assistantDecisionRepository;

        public VesselInfoService(
            ILogger<VesselInfoService> logger,
            IVesselInfoRepository repository,
            ITotalIndicatorRepository totalIndicatorRepository,
            IPredictionRepository predictionRepository,
            IBatteryRepository batteryRepository,
            IFlowmeterRepository flowmeterRepository,
            IGeneratorRepository generatorRepository,
            ILiquidLevelRepository liquidLevelRepository,
            IShaftRepository shaftRepository,
            ISternSealingRepository sternSealingRepository,
            ISupplyUnitRepository supplyUnitRepository,
            IPowerUnitRepository powerUnitRepository,
            ICIIService ciiService,
            ISqlRepository sqlRepository,
            ICompositeBoilerRepository compositeBoilerRepository,
            ICompressedAirSupplyRepository compressedAirSupplyRepository,
            ICoolingFreshWaterRepository coolingFreshWaterRepository,
            ICoolingSeaWaterRepository coolingSeaWaterRepository,
            ICoolingWaterRepository coolingWaterRepository,
            ICylinderLubOilRepository cylinderLubOilRepository,
            IExhaustGasRepository exhaustGasRepository,
            IFORepository fORepository,
            IFOSupplyUnitRepository fOSupplyUnitRepository,
            ILubOilPurifyingRepository lubOilPurifyingRepository,
            ILubOilRepository lubOilRepository,
            IMainGeneratorSetRepository mainGeneratorSetRepository,
            IMainSwitchboardRepository mainSwitchboardRepository,
            IMERemoteControlRepository mERemoteControlRepository,
            IMiscellaneousRepository miscellaneousRepository,
            IScavengeAirRepository scavengeAirRepository,
            IShaftClutchRepository shaftClutchRepository,
            IAssistantDecisionRepository assistantDecisionRepository)
        {
            _logger = logger;
            _repository = repository;
            _totalIndicatorRepository = totalIndicatorRepository;
            _predictionRepository = predictionRepository;
            _batteryRepository = batteryRepository;
            _flowmeterRepository = flowmeterRepository;
            _generatorRepository = generatorRepository;
            _liquidLevelRepository = liquidLevelRepository;
            _shaftRepository = shaftRepository;
            _sternSealingRepository = sternSealingRepository;
            _supplyUnitRepository = supplyUnitRepository;
            _powerUnitRepository = powerUnitRepository;
            _ciiService = ciiService;
            _sqlRepository = sqlRepository;

            _compositeBoilerRepository = compositeBoilerRepository;
            _compressedAirSupplyRepository = compressedAirSupplyRepository;
            _coolingFreshWaterRepository = coolingFreshWaterRepository;
            _coolingSeaWaterRepository = coolingSeaWaterRepository;
            _coolingWaterRepository = coolingWaterRepository;
            _cylinderLubOilRepository = cylinderLubOilRepository;
            _exhaustGasRepository = exhaustGasRepository;
            _fORepository = fORepository;
            _fOSupplyUnitRepository = fOSupplyUnitRepository;
            _lubOilPurifyingRepository = lubOilPurifyingRepository;
            _lubOilRepository = lubOilRepository;
            _mainGeneratorSetRepository = mainGeneratorSetRepository;
            _mainSwitchboardRepository = mainSwitchboardRepository;
            _mERemoteControlRepository = mERemoteControlRepository;
            _miscellaneousRepository = miscellaneousRepository;
            _scavengeAirRepository = scavengeAirRepository;
            _shaftClutchRepository = shaftClutchRepository;
            _assistantDecisionRepository = assistantDecisionRepository;
        }

        public async Task<VesselInfoDto> InsertAsync(VesselInfoDto dto)
        {
            var entity = ObjectMapper.Map<VesselInfoDto, VesselInfo>(dto);
            await _repository.InsertAsync(entity);
            return ObjectMapper.Map<VesselInfo, VesselInfoDto>(entity);
        }

        public async Task<VesselInfoDto> GetLatestAsync(string number)
        {
            try
            {
                var entities = await _repository.QueryFromSql($"SELECT * FROM (SELECT * FROM \"vesselinfo\" WHERE SN = '{number}' AND NVL( \"delete_time\", TO_TIMESTAMP( '1949-10-01', 'YYYY-MM-DD' ) ) = TO_TIMESTAMP( '1949-10-01', 'YYYY-MM-DD' ) ORDER BY \"ReceiveDatetime\" DESC) WHERE ROWNUM =1");
                VesselInfo entity = entities?.First().ToJson().ToObject<VesselInfo>();
                return ObjectMapper.Map<VesselInfo, VesselInfoDto>(entity);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task GetLatestInfosAsync(string number, DateTime receiveDatetime)
        {
            try
            {
                var result = await _repository.GetLatestInfosAsync(number, receiveDatetime);

                StaticEntities.ShowEntities.Flowmeters.Add(new VesselFlowmeterDto { Number = number, FlowmeterDtos = ObjectMapper.Map<IList<Flowmeter>, IList<FlowmeterDto>>(result.flowmeters) });
                StaticEntities.ShowEntities.Batteries.Add(new VesselBatteryDto { Number = number, BatteryDtos = ObjectMapper.Map<IList<Battery>, IList<BatteryDto>>(result.batteries) });
                StaticEntities.ShowEntities.Generators.Add(new VesselGeneratorDto { Number = number, GeneratorDtos = ObjectMapper.Map<IList<Generator>, IList<GeneratorDto>>(result.generators) });
                StaticEntities.ShowEntities.LiquidLevels.Add(new VesselLiquidLevelDto { Number = number, LiquidLevelDtos = ObjectMapper.Map<IList<LiquidLevel>, IList<LiquidLevelDto>>(result.liquidlevels) });
                StaticEntities.ShowEntities.SupplyUnits.Add(new VesselSupplyUnitDto { Number = number, SupplyUnitDtos = ObjectMapper.Map<IList<SupplyUnit>, IList<SupplyUnitDto>>(result.supplyunits) });
                StaticEntities.ShowEntities.Shafts.Add(new VesselShaftDto { Number = number, ShaftDtos = ObjectMapper.Map<IList<Shaft>, IList<ShaftDto>>(result.shafts) });
                StaticEntities.ShowEntities.SternSealings.Add(new VesselSternSealingDto { Number = number, SternSealingDtos = ObjectMapper.Map<IList<SternSealing>, IList<SternSealingDto>>(result.sternsealings) });
                StaticEntities.ShowEntities.PowerUnits.Add(new VesselPowerUnitDto { Number = number, PowerUnitDtos = ObjectMapper.Map<IList<PowerUnit>, IList<PowerUnitDto>>(result.powerunits) });
                StaticEntities.ShowEntities.TotalIndicators.Add(ObjectMapper.Map<IList<TotalIndicator>, IList<TotalIndicatorDto>>(result.totalindicators).FirstOrDefault());
                StaticEntities.ShowEntities.Predictions.Add(ObjectMapper.Map<IList<Prediction>, IList<PredictionDto>>(result.predictions).FirstOrDefault());
                StaticEntities.ShowEntities.AssistantDecisions.Add(new VesselAssistantDecisionDto { Number = number, AssistantDecisionDtos = ObjectMapper.Map<IList<AssistantDecision>, IList<AssistantDecisionDto>>(result.assistantdecisions) });
                StaticEntities.ShowEntities.CompositeBoilers.Add(new VesselCompositeBoilerDto { Number = number, CompositeBoilerDtos = ObjectMapper.Map<IList<CompositeBoiler>, IList<CompositeBoilerDto>>(result.compositeboilers) });
                StaticEntities.ShowEntities.CompressedAirSupplies.Add(new VesselCompressedAirSupplyDto { Number = number, CompressedAirSupplyDtos = ObjectMapper.Map<IList<CompressedAirSupply>, IList<CompressedAirSupplyDto>>(result.compressedairsupplies) });
                StaticEntities.ShowEntities.CoolingFreshWaters.Add(new VesselCoolingFreshWaterDto { Number = number, CoolingFreshWaterDtos = ObjectMapper.Map<IList<CoolingFreshWater>, IList<CoolingFreshWaterDto>>(result.coolingfreshwaters) });
                StaticEntities.ShowEntities.CoolingSeaWaters.Add(new VesselCoolingSeaWaterDto { Number = number, CoolingSeaWaterDtos = ObjectMapper.Map<IList<CoolingSeaWater>, IList<CoolingSeaWaterDto>>(result.coolingseawaters) });
                StaticEntities.ShowEntities.CoolingWaters.Add(new VesselCoolingWaterDto { Number = number, CoolingWaterDtos = ObjectMapper.Map<IList<CoolingWater>, IList<CoolingWaterDto>>(result.coolingwaters) });
                StaticEntities.ShowEntities.CylinderLubOils.Add(new VesselCylinderLubOilDto { Number = number, CylinderLubOilDtos = ObjectMapper.Map<IList<CylinderLubOil>, IList<CylinderLubOilDto>>(result.cylinderluboils) });
                StaticEntities.ShowEntities.ExhaustGases.Add(new VesselExhaustGasDto { Number = number, ExhaustGasDtos = ObjectMapper.Map<IList<ExhaustGas>, IList<ExhaustGasDto>>(result.exhaustgases) });
                StaticEntities.ShowEntities.FOs.Add(new VesselFODto { Number = number, FODtos = ObjectMapper.Map<IList<FO>, IList<FODto>>(result.fos) });
                StaticEntities.ShowEntities.FOSupplyUnits.Add(new VesselFOSupplyUnitDto { Number = number, FOSupplyUnitDtos = ObjectMapper.Map<IList<FOSupplyUnit>, IList<FOSupplyUnitDto>>(result.fosupplyunits) });
                StaticEntities.ShowEntities.LubOilPurifyings.Add(new VesselLubOilPurifyingDto { Number = number, LubOilPurifyingDtos = ObjectMapper.Map<IList<LubOilPurifying>, IList<LubOilPurifyingDto>>(result.luboilpurifyings) });
                StaticEntities.ShowEntities.LubOils.Add(new VesselLubOilDto { Number = number, LubOilDtos = ObjectMapper.Map<IList<LubOil>, IList<LubOilDto>>(result.luboils) });
                StaticEntities.ShowEntities.MainGeneratorSets.Add(new VesselMainGeneratorSetDto { Number = number, MainGeneratorSetDtos = ObjectMapper.Map<IList<MainGeneratorSet>, IList<MainGeneratorSetDto>>(result.maingeneratorsets) });
                StaticEntities.ShowEntities.MainSwitchboards.Add(new VesselMainSwitchboardDto { Number = number, MainSwitchboardDtos = ObjectMapper.Map<IList<MainSwitchboard>, IList<MainSwitchboardDto>>(result.mainswitchboards) });
                StaticEntities.ShowEntities.MERemoteControls.Add(new VesselMERemoteControlDto { Number = number, MERemoteControlDtos = ObjectMapper.Map<IList<MERemoteControl>, IList<MERemoteControlDto>>(result.meremotecontrols) });
                StaticEntities.ShowEntities.Miscellaneouses.Add(new VesselMiscellaneousDto { Number = number, MiscellaneousDtos = ObjectMapper.Map<IList<Miscellaneous>, IList<MiscellaneousDto>>(result.miscellaneous) });
                StaticEntities.ShowEntities.ScavengeAirs.Add(new VesselScavengeAirDto { Number = number, ScavengeAirDtos = ObjectMapper.Map<IList<ScavengeAir>, IList<ScavengeAirDto>>(result.scavengeairs) });
                StaticEntities.ShowEntities.ShaftClutchs.Add(new VesselShaftClutchDto { Number = number, ShaftClutchDtos = ObjectMapper.Map<IList<ShaftClutch>, IList<ShaftClutchDto>>(result.shaftclutches) });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<IList<VesselInfoDto>> GetListChart(string number, string parameters)
        {
            var result = await _repository.GetListChart(number, parameters);
            return ObjectMapper.Map<IQueryable<VesselInfo>, IList<VesselInfoDto>>(result);
        }

        public async Task<IList<VesselInfoDto>> GetListPage(string number, int pageNumber, int countPerPage, string sorting, string asc, string parameters)
        {
            var result = (await _repository.GetListPage(number, pageNumber, countPerPage, sorting, asc, parameters)).ToList();
            return ObjectMapper.Map<IList<VesselInfo>, IList<VesselInfoDto>>(result);
        }

        public async Task<IList<MapDto>> GetListMap(string number, string parameters)
        {
            var result = await _repository.GetListMap(number, parameters);
            return ObjectMapper.Map<IQueryable<VesselInfo>, IList<MapDto>>(result);
        }

        public async Task<int> GetTotalCount(string number, string parameters)
        {
            var result = await _repository.GetTotalCount(number, parameters);
            return result;
        }

        public async Task InsertOrUpdateAsync(VesselInfoDto dto)
        {
            var entity = await _repository.FirstOrDefaultAsync(t => t.SN == dto.SN && t.ReceiveDatetime == dto.ReceiveDatetime);
            var needInsert = 0;
            if (entity == null)
            {
                needInsert = 1;
                entity = new VesselInfo();
            }
            foreach (var prop in dto.GetType().GetProperties())
            {
                if (prop.Name.Equals("Id")
                    || prop.Name.Equals("create_time")
                    || prop.Name.Equals("update_time")
                    || prop.Name.Equals("delete_time")
                    || prop.GetValue(dto, null) == null)
                    continue;
                else
                    entity.GetType().GetProperty(prop.Name).SetValue(entity, prop.GetValue(dto, null));
            }
            if (needInsert == 1)
            {
                await _repository.CalcProperties(entity.SN);
                await _repository.InsertAsync(entity);
            }
            else
            {
                entity.update_time = DateTime.Now;
                await _repository.UpdateAsync(entity);
            }
        }

        public async Task<IList<DraftDistributionDTO>> GetDraftDistribution(string parameters)
        {
            var queryParams = parameters.ToJObject();

            double interval = 1;
            DateTime dateFrom = DateTime.Now;
            DateTime dateTo = DateTime.Now;
            double draftFrom = 0;
            double draftTo = 0;
            string sn = "";
            if (queryParams.ContainsKey("Interval"))
                interval = Convert.ToDouble(queryParams["Interval"]);
            if (queryParams.ContainsKey("DateFrom"))
                dateFrom = Convert.ToDateTime(queryParams["DateFrom"]);
            if (queryParams.ContainsKey("DateTo"))
                dateTo = Convert.ToDateTime(queryParams["DateTo"]);
            if (queryParams.ContainsKey("DraftFrom"))
                draftFrom = Convert.ToDouble(queryParams["DraftFrom"]);
            if (queryParams.ContainsKey("DraftTo"))
                draftTo = Convert.ToDouble(queryParams["DraftTo"]);
            if (queryParams.ContainsKey("SN"))
                sn = queryParams["SN"].ToString();
            var result = new List<DraftDistributionDTO>();
            for (double i = draftFrom; i <= draftTo; i = i + interval)
            {
                result.Add(new DraftDistributionDTO(i, await _repository.CountAsync(t => t.delete_time == null && t.SN == sn && t.ReceiveDatetime >= dateFrom && t.ReceiveDatetime < dateTo.AddDays(1) && t.Draft >= i && t.Draft < i + interval)));
            }
            return result;
        }

        public async Task<IList<PowerDistributionDto>> GetPowerDistribution(string parameters)
        {
            var queryParams = parameters.ToJObject();

            double interval = 1;
            DateTime dateFrom = DateTime.Now;
            DateTime dateTo = DateTime.Now;
            double powerFrom = 0;
            double powerTo = 0;
            string sn = "";
            if (queryParams.ContainsKey("Interval"))
                interval = Convert.ToDouble(queryParams["Interval"]);
            if (queryParams.ContainsKey("DateFrom"))
                dateFrom = Convert.ToDateTime(queryParams["DateFrom"]);
            if (queryParams.ContainsKey("DateTo"))
                dateTo = Convert.ToDateTime(queryParams["DateTo"]);
            if (queryParams.ContainsKey("PowerFrom"))
                powerFrom = Convert.ToDouble(queryParams["PowerFrom"]);
            if (queryParams.ContainsKey("PowerTo"))
                powerTo = Convert.ToDouble(queryParams["PowerTo"]);
            if (queryParams.ContainsKey("SN"))
                sn = queryParams["SN"].ToString();
            var result = new List<PowerDistributionDto>();
            for (double i = powerFrom; i <= powerTo; i = i + interval)
            {
                result.Add(new PowerDistributionDto(i, await _repository.CountAsync(t => t.delete_time == null && t.SN == sn && t.ReceiveDatetime >= dateFrom && t.ReceiveDatetime < dateTo.AddDays(1) && t.MEPower >= i && t.MEPower < i + interval)));
            }
            return result;
        }

        public async Task<IList<SlipDistributionDto>> GetSlipDistribution(string parameters)
        {
            var queryParams = parameters.ToJObject();

            double interval = 1;
            DateTime dateFrom = DateTime.Now;
            DateTime dateTo = DateTime.Now;
            string sn = "";
            if (queryParams.ContainsKey("Interval"))
                interval = Convert.ToDouble(queryParams["Interval"]);
            if (queryParams.ContainsKey("DateFrom"))
                dateFrom = Convert.ToDateTime(queryParams["DateFrom"]);
            if (queryParams.ContainsKey("DateTo"))
                dateTo = Convert.ToDateTime(queryParams["DateTo"]);
            if (queryParams.ContainsKey("SN"))
                sn = queryParams["SN"].ToString();
            var result = new List<SlipDistributionDto>();
            double slipFrom = (double)(await _repository.GetListAsync(t => t.delete_time == null && t.SN == sn)).Min(t => t.Slip);
            double slipTo = (double)(await _repository.GetListAsync(t => t.delete_time == null && t.SN == sn)).Max(t => t.Slip);
            for (double i = slipFrom; i <= slipTo; i = i + interval)
            {
                result.Add(new SlipDistributionDto(i, await _repository.CountAsync(t => t.delete_time == null && t.SN == sn && t.ReceiveDatetime >= dateFrom && t.ReceiveDatetime < dateTo.AddDays(1) && t.Slip >= i && t.Slip < i + interval)));
            }
            return result;
        }

        public async Task<IList<SpeedDistributionDto>> GetSpeedDistribution(string parameters)
        {
            var queryParams = parameters.ToJObject();

            double interval = 1;
            DateTime dateFrom = DateTime.Now;
            DateTime dateTo = DateTime.Now;
            double speedFrom = 0;
            double speedTo = 0;
            string sn = "";
            if (queryParams.ContainsKey("Interval"))
                interval = Convert.ToDouble(queryParams["Interval"]);
            if (queryParams.ContainsKey("DateFrom"))
                dateFrom = Convert.ToDateTime(queryParams["DateFrom"]);
            if (queryParams.ContainsKey("DateTo"))
                dateTo = Convert.ToDateTime(queryParams["DateTo"]);
            if (queryParams.ContainsKey("SpeedFrom"))
                speedFrom = Convert.ToDouble(queryParams["SpeedFrom"]);
            if (queryParams.ContainsKey("SpeedTo"))
                speedTo = Convert.ToDouble(queryParams["SpeedTo"]);
            if (queryParams.ContainsKey("SN"))
                sn = queryParams["SN"].ToString();
            var result = new List<SpeedDistributionDto>();
            for (double i = speedFrom; i <= speedTo; i = i + interval)
            {
                result.Add(new SpeedDistributionDto(i, await _repository.CountAsync(t => t.delete_time == null && t.SN == sn && t.ReceiveDatetime >= dateFrom && t.ReceiveDatetime < dateTo.AddDays(1) && t.WaterSpeed >= i && t.WaterSpeed < i + interval)));
            }
            return result;
        }

        public async Task<IList<WindDirDistributionDto>> GetWindDirDistribution(string parameters)
        {
            var queryParams = parameters.ToJObject();

            double interval = 1;
            DateTime dateFrom = DateTime.Now;
            DateTime dateTo = DateTime.Now;
            string sn = "";
            if (queryParams.ContainsKey("Interval"))
                interval = Convert.ToDouble(queryParams["Interval"]);
            if (queryParams.ContainsKey("DateFrom"))
                dateFrom = Convert.ToDateTime(queryParams["DateFrom"]);
            if (queryParams.ContainsKey("DateTo"))
                dateTo = Convert.ToDateTime(queryParams["DateTo"]);
            if (queryParams.ContainsKey("SN"))
                sn = queryParams["SN"].ToString();
            var result = new List<WindDirDistributionDto>();
            double directionFrom = 0;
            double directionTo = 360;
            for (double i = directionFrom; i < directionTo; i = i + interval)
            {
                result.Add(new WindDirDistributionDto(i, await _repository.CountAsync(t => t.delete_time == null && t.SN == sn && t.ReceiveDatetime >= dateFrom && t.ReceiveDatetime < dateTo.AddDays(1) && t.WindDirection >= i && t.WindDirection < i + interval)));
            }
            return result;
        }

        public async Task<IList<WindSpeedDistributionDto>> GetWindSpeedDistribution(string parameters)
        {
            var queryParams = parameters.ToJObject();

            double interval = 1;
            DateTime dateFrom = DateTime.Now;
            DateTime dateTo = DateTime.Now;
            double windSpeedFrom = 0;
            double windSpeedTo = 0;
            string sn = "";
            if (queryParams.ContainsKey("Interval"))
                interval = Convert.ToDouble(queryParams["Interval"]);
            if (queryParams.ContainsKey("DateFrom"))
                dateFrom = Convert.ToDateTime(queryParams["DateFrom"]);
            if (queryParams.ContainsKey("DateTo"))
                dateTo = Convert.ToDateTime(queryParams["DateTo"]);
            if (queryParams.ContainsKey("WindSpeedFrom"))
                windSpeedFrom = Convert.ToDouble(queryParams["WindSpeedFrom"]);
            if (queryParams.ContainsKey("WindSpeedTo"))
                windSpeedTo = Convert.ToDouble(queryParams["WindSpeedTo"]);
            if (queryParams.ContainsKey("SN"))
                sn = queryParams["SN"].ToString();
            var result = new List<WindSpeedDistributionDto>();
            for (double i = windSpeedFrom; i <= windSpeedTo; i = i + interval)
            {
                result.Add(new WindSpeedDistributionDto(i, await _repository.CountAsync(t => t.delete_time == null && t.SN == sn && t.ReceiveDatetime >= dateFrom && t.ReceiveDatetime < dateTo.AddDays(1) && t.WindSpeed >= i && t.WindSpeed < i + interval)));
            }
            return result;
        }

        public async Task<DataTable> GetStatisticList(string parameters)
        {
            var queryParams = parameters.ToJObject();
            var dbParameters = new List<DbParameter>();

            StringBuilder sbSql = new StringBuilder();
            if (!queryParams.ContainsKey("x"))
                throw new Exception("缺少参数x");
            if (!queryParams.ContainsKey("interval"))
                throw new Exception("缺少参数interval");
            if (!queryParams.ContainsKey("vdrId"))
                throw new Exception("缺少参数vdrId");
            sbSql.AppendFormat(@"SELECT
	                                    FLOOR( {0} / {1} ) * {1} ""{2}"",
	                                    COUNT( 1 ) ""Count""
                                    FROM
	                                    ""vesselinfo"" t0,
	                                    ""energy_totalindicator"" t1
                                    WHERE
	                                    NVL( t0.""delete_time"", TO_TIMESTAMP( '1949-10-01', 'YYYY-MM-DD' ) ) = TO_TIMESTAMP( '1949-10-01', 'YYYY-MM-DD' )
	                                    AND t0.""SN""=t1.""Number""
	                                    AND t0.""ReceiveDatetime""=t1.""ReceiveDatetime""
	                                    AND ABS( {0} ) >= 0 ", queryParams["x"].ToString() == "MEPower" ? @"t1.""Power""" : @$"t0.""{queryParams["x"].ToString()}""", queryParams["interval"].ToString(), queryParams["x"].ToString());
            if (queryParams.ContainsKey("x") && !string.IsNullOrWhiteSpace(queryParams["x"].ToString()))
            {
                dbParameters.Add(new OracleParameter(":target", queryParams["x"].ToString()));
            }
            if (queryParams.ContainsKey("interval") && !string.IsNullOrWhiteSpace(queryParams["interval"].ToString()))
            {
                dbParameters.Add(new OracleParameter(":interval", queryParams["interval"].ToString()));
            }
            if (queryParams.ContainsKey("vdrId") && !string.IsNullOrWhiteSpace(queryParams["vdrId"].ToString()))
            {
                sbSql.AppendFormat(@"AND t0.""SN"" = '{0}' ", queryParams["vdrId"].ToString());
                dbParameters.Add(new OracleParameter(":SN", queryParams["vdrId"].ToString()));
            }
            if (queryParams.ContainsKey("valueFrom") && !string.IsNullOrWhiteSpace(queryParams["valueFrom"].ToString()))
            {
                sbSql.AppendFormat(@"AND {0} >= {1} ", queryParams["x"].ToString() == "MEPower" ? @"t1.""Power""" : @$"t0.""{queryParams["x"].ToString()}""", queryParams["valueFrom"].ToString());
                dbParameters.Add(new OracleParameter(":valueFrom", queryParams["valueFrom"].ToString()));
            }
            if (queryParams.ContainsKey("valueTo") && !string.IsNullOrWhiteSpace(queryParams["valueTo"].ToString()))
            {
                sbSql.AppendFormat(@"AND {0} < {1} ", queryParams["x"].ToString() == "MEPower" ? @"t1.""Power""" : @$"t0.""{queryParams["x"].ToString()}""", queryParams["valueTo"].ToString());
                dbParameters.Add(new OracleParameter(":valueTo", queryParams["valueTo"].ToString()));
            }
            if (queryParams.ContainsKey("dateFrom") && !string.IsNullOrWhiteSpace(queryParams["dateFrom"].ToString()))
            {
                sbSql.AppendFormat(@"AND t0.""ReceiveDatetime"" >= TO_TIMESTAMP( '{0}', 'YYYY-MM-DD HH24:MI:SS' ) ", queryParams["dateFrom"].ToString());
                dbParameters.Add(new OracleParameter(":dateFrom", queryParams["dateFrom"].ToString()));
            }
            if (queryParams.ContainsKey("dateTo") && !string.IsNullOrWhiteSpace(queryParams["dateTo"].ToString()))
            {
                sbSql.AppendFormat(@"AND t0.""ReceiveDatetime"" < TO_TIMESTAMP( '{0}', 'YYYY-MM-DD HH24:MI:SS' ) ", queryParams["dateTo"].ToString());
                dbParameters.Add(new OracleParameter(":dateTo", queryParams["dateTo"].ToString()));
            }
            sbSql.AppendFormat(@"GROUP BY
	                                    FLOOR( {0} / {1} ) * {1}
                                    ORDER BY
	                                    FLOOR( {0} / {1} ) * {1}", queryParams["x"].ToString() == "MEPower" ? @"t1.""Power""" : $@"t0.""{queryParams["x"].ToString()}""", queryParams["interval"].ToString());
            var result = await _sqlRepository.ExecuteDataTable(sbSql.ToString());
            return result;
        }

        public async Task<IList<SpeedDto>> GetSpeedList(string parameters)
        {
            var queryParams = parameters.ToJObject();
            DateTime dateFrom = DateTime.Now;
            DateTime dateTo = DateTime.Now;
            double slipFrom = 0;
            double slipTo = 0;
            double draftFrom = 0;
            double draftTo = 0;
            string sn = "";
            if (queryParams.ContainsKey("SN"))
                sn = queryParams["SN"].ToString();
            if (queryParams.ContainsKey("DateFrom"))
                dateFrom = Convert.ToDateTime(queryParams["DateFrom"]);
            if (queryParams.ContainsKey("DateTo"))
                dateTo = Convert.ToDateTime(queryParams["DateTo"]);
            if (queryParams.ContainsKey("SlipFrom"))
                slipFrom = Convert.ToDouble(queryParams["SlipFrom"]);
            if (queryParams.ContainsKey("SlipTo"))
                slipTo = Convert.ToDouble(queryParams["SlipTo"]);
            if (queryParams.ContainsKey("DraftFrom"))
                draftFrom = Convert.ToDouble(queryParams["DraftFrom"]);
            if (queryParams.ContainsKey("DraftTo"))
                draftTo = Convert.ToDouble(queryParams["DraftTo"]);

            var result = (await _repository.GetListAsync(t => t.delete_time == null && t.SN == sn && t.ReceiveDatetime >= dateFrom && t.ReceiveDatetime < dateTo.AddDays(1) && t.Slip >= slipFrom && t.Slip <= slipTo && t.Draft >= draftFrom && t.Draft <= draftTo)).Select(t => new SpeedDto
            {
                ReceiveDatetime = t.ReceiveDatetime,
                WaterSpeed = t.WaterSpeed,
                MEFCPerNm = t.MEFCPerNm,
                MESFOC = t.MESFOC,
                Trim = t.Trim,
                Draft = t.Draft,
                Slip = t.Slip,
                MEPower = t.MEPower,
                MERpm = t.MERpm,
                WindDirection = t.WindDirection,
                WindSpeed = t.WindSpeed
            });

            return result.ToList();
        }

        public async Task<IList<VFCSpdDto>> GetVFCSpdList(string parameters)
        {
            var queryParams = parameters.ToJObject();
            DateTime dateFrom = DateTime.Now;
            DateTime dateTo = DateTime.Now;
            double slipFrom = 0;
            double slipTo = 0;
            double draftFrom = 0;
            double draftTo = 0;
            double windSpeedFrom = 0;
            double windSpeedTo = 0;
            string sn = "";
            if (queryParams.ContainsKey("SN"))
                sn = queryParams["SN"].ToString();
            if (queryParams.ContainsKey("DateFrom"))
                dateFrom = Convert.ToDateTime(queryParams["DateFrom"]);
            if (queryParams.ContainsKey("DateTo"))
                dateTo = Convert.ToDateTime(queryParams["DateTo"]);
            if (queryParams.ContainsKey("SlipFrom"))
                slipFrom = Convert.ToDouble(queryParams["SlipFrom"]);
            if (queryParams.ContainsKey("SlipTo"))
                slipTo = Convert.ToDouble(queryParams["SlipTo"]);
            if (queryParams.ContainsKey("DraftFrom"))
                draftFrom = Convert.ToDouble(queryParams["DraftFrom"]);
            if (queryParams.ContainsKey("DraftTo"))
                draftTo = Convert.ToDouble(queryParams["DraftTo"]);
            if (queryParams.ContainsKey("WindSpeedFrom"))
                windSpeedFrom = Convert.ToDouble(queryParams["WindSpeedFrom"]);
            if (queryParams.ContainsKey("WindSpeedTo"))
                windSpeedTo = Convert.ToDouble(queryParams["WindSpeedTo"]);

            var result = (await _repository.GetListAsync(t => t.delete_time == null && t.SN == sn && t.ReceiveDatetime >= dateFrom && t.ReceiveDatetime < dateTo.AddDays(1) && t.Slip >= slipFrom && t.Slip <= slipTo && t.Draft >= draftFrom && t.Draft <= draftTo && t.WindSpeed >= windSpeedFrom && t.WindSpeed <= windSpeedTo)).Select(t => new VFCSpdDto
            {
                ReceiveDatetime = t.ReceiveDatetime,
                WaterSpeed = t.WaterSpeed,
                MEFCPerNm = t.MEFCPerNm,
                Trim = t.Trim,
                Draft = t.Draft,
                Slip = t.Slip,
                MEPower = t.MEPower,
                MERpm = t.MERpm,
                WindDirection = t.WindDirection,
                WindSpeed = t.WindSpeed,
                FCPerNm = t.FCPerNm,
                BLRFCPerNm = t.BLRFCPerNm,
                DGFCPerNm = t.DGFCPerNm
            });

            return result.ToList();
        }

        public async Task<IList<MEFCPowDto>> GetMEFCPowList(string parameters)
        {
            var queryParams = parameters.ToJObject();
            DateTime dateFrom = DateTime.Now;
            DateTime dateTo = DateTime.Now;
            double slipFrom = 0;
            double slipTo = 0;
            double draftFrom = 0;
            double draftTo = 0;
            string sn = "";
            if (queryParams.ContainsKey("SN"))
                sn = queryParams["SN"].ToString();
            if (queryParams.ContainsKey("DateFrom"))
                dateFrom = Convert.ToDateTime(queryParams["DateFrom"]);
            if (queryParams.ContainsKey("DateTo"))
                dateTo = Convert.ToDateTime(queryParams["DateTo"]);
            if (queryParams.ContainsKey("SlipFrom"))
                slipFrom = Convert.ToDouble(queryParams["SlipFrom"]);
            if (queryParams.ContainsKey("SlipTo"))
                slipTo = Convert.ToDouble(queryParams["SlipTo"]);
            if (queryParams.ContainsKey("DraftFrom"))
                draftFrom = Convert.ToDouble(queryParams["DraftFrom"]);
            if (queryParams.ContainsKey("DraftTo"))
                draftTo = Convert.ToDouble(queryParams["DraftTo"]);

            var result = (await _repository.GetListAsync(t => t.delete_time == null && t.SN == sn && t.ReceiveDatetime >= dateFrom && t.ReceiveDatetime < dateTo.AddDays(1) && t.Slip >= slipFrom && t.Slip <= slipTo && t.Draft >= draftFrom && t.Draft <= draftTo)).Select(t => new MEFCPowDto
            {
                ReceiveDatetime = t.ReceiveDatetime,
                MEFCPerNm = t.MEFCPerNm,
                Trim = t.Trim,
                Draft = t.Draft,
                Slip = t.Slip,
                MEPower = t.MEPower,
                MERpm = t.MERpm,
                MESFOC = t.MESFOC
            });

            return result.ToList();
        }

        public async Task<IList<TrimDto>> GetTrimList(string parameters)
        {
            var queryParams = parameters.ToJObject();
            DateTime dateFrom = DateTime.Now;
            DateTime dateTo = DateTime.Now;
            double slipFrom = 0;
            double slipTo = 0;
            double draftFrom = 0;
            double draftTo = 0;
            double speedFrom = 0;
            double speedTo = 0;
            string sn = "";
            if (queryParams.ContainsKey("SN"))
                sn = queryParams["SN"].ToString();
            if (queryParams.ContainsKey("DateFrom"))
                dateFrom = Convert.ToDateTime(queryParams["DateFrom"]);
            if (queryParams.ContainsKey("DateTo"))
                dateTo = Convert.ToDateTime(queryParams["DateTo"]);
            if (queryParams.ContainsKey("SlipFrom"))
                slipFrom = Convert.ToDouble(queryParams["SlipFrom"]);
            if (queryParams.ContainsKey("SlipTo"))
                slipTo = Convert.ToDouble(queryParams["SlipTo"]);
            if (queryParams.ContainsKey("DraftFrom"))
                draftFrom = Convert.ToDouble(queryParams["DraftFrom"]);
            if (queryParams.ContainsKey("DraftTo"))
                draftTo = Convert.ToDouble(queryParams["DraftTo"]);
            if (queryParams.ContainsKey("SpeedFrom"))
                speedFrom = Convert.ToDouble(queryParams["SpeedFrom"]);
            if (queryParams.ContainsKey("SpeedTo"))
                speedTo = Convert.ToDouble(queryParams["SpeedTo"]);

            var result = (await _repository.GetListAsync(t => t.delete_time == null && t.SN == sn && t.ReceiveDatetime >= dateFrom && t.ReceiveDatetime < dateTo.AddDays(1) && t.Slip >= slipFrom && t.Slip <= slipTo && t.Draft >= draftFrom && t.Draft <= draftTo && t.WaterSpeed >= speedFrom && t.WaterSpeed <= speedTo)).Select(t => new TrimDto
            {
                ReceiveDatetime = t.ReceiveDatetime,
                MEFCPerNm = t.MEFCPerNm,
                Trim = t.Trim,
                Draft = t.Draft,
                Slip = t.Slip,
                MEPower = t.MEPower,
                MERpm = t.MERpm,
                MESFOC = t.MESFOC,
                WaterSpeed = t.WaterSpeed,
                WindDirection = t.WindDirection,
                WindSpeed = t.WindSpeed
            });

            return result.ToList();
        }

        public async Task<IList<VFCMESpdDto>> GetVFCMESpdList(string parameters)
        {
            var queryParams = parameters.ToJObject();
            DateTime dateFrom = DateTime.Now;
            DateTime dateTo = DateTime.Now;
            double slipFrom = 0;
            double slipTo = 0;
            double draftFrom = 0;
            double draftTo = 0;
            string sn = "";
            if (queryParams.ContainsKey("SN"))
                sn = queryParams["SN"].ToString();
            if (queryParams.ContainsKey("DateFrom"))
                dateFrom = Convert.ToDateTime(queryParams["DateFrom"]);
            if (queryParams.ContainsKey("DateTo"))
                dateTo = Convert.ToDateTime(queryParams["DateTo"]);
            if (queryParams.ContainsKey("SlipFrom"))
                slipFrom = Convert.ToDouble(queryParams["SlipFrom"]);
            if (queryParams.ContainsKey("SlipTo"))
                slipTo = Convert.ToDouble(queryParams["SlipTo"]);
            if (queryParams.ContainsKey("DraftFrom"))
                draftFrom = Convert.ToDouble(queryParams["DraftFrom"]);
            if (queryParams.ContainsKey("DraftTo"))
                draftTo = Convert.ToDouble(queryParams["DraftTo"]);

            var result = (await _repository.GetListAsync(t => t.delete_time == null && t.SN == sn && t.ReceiveDatetime >= dateFrom && t.ReceiveDatetime < dateTo.AddDays(1) && t.Slip >= slipFrom && t.Slip <= slipTo && t.Draft >= draftFrom && t.Draft <= draftTo)).Select(t => new VFCMESpdDto
            {
                ReceiveDatetime = t.ReceiveDatetime,
                WaterSpeed = t.WaterSpeed,
                FCPerNm = t.FCPerNm,
                SFOC = t.SFOC
            });

            return result.ToList();
        }

        public async Task<IList<MESpdPropDto>> GetMESpdPropList(string parameters)
        {
            var queryParams = parameters.ToJObject();
            DateTime dateFrom = DateTime.Now;
            DateTime dateTo = DateTime.Now;
            double mEFCFrom = 0;
            double mEFCTo = 0;
            double windSpdFrom = 0;
            double windSpdTo = 0;
            double draftFrom = 0;
            double draftTo = 0;
            string sn = "";
            if (queryParams.ContainsKey("SN"))
                sn = queryParams["SN"].ToString();
            if (queryParams.ContainsKey("DateFrom"))
                dateFrom = Convert.ToDateTime(queryParams["DateFrom"]);
            if (queryParams.ContainsKey("DateTo"))
                dateTo = Convert.ToDateTime(queryParams["DateTo"]);
            if (queryParams.ContainsKey("MEFCFrom"))
                mEFCFrom = Convert.ToDouble(queryParams["MEFCFrom"]);
            if (queryParams.ContainsKey("MEFCTo"))
                mEFCTo = Convert.ToDouble(queryParams["MEFCTo"]);
            if (queryParams.ContainsKey("WindSpdFrom"))
                windSpdFrom = Convert.ToDouble(queryParams["WindSpdFrom"]);
            if (queryParams.ContainsKey("WindSpdTo"))
                windSpdTo = Convert.ToDouble(queryParams["WindSpdTo"]);
            if (queryParams.ContainsKey("DraftFrom"))
                draftFrom = Convert.ToDouble(queryParams["DraftFrom"]);
            if (queryParams.ContainsKey("DraftTo"))
                draftTo = Convert.ToDouble(queryParams["DraftTo"]);

            var result = (await _repository.GetListAsync(t => t.delete_time == null && t.SN == sn && t.ReceiveDatetime >= dateFrom && t.ReceiveDatetime < dateTo.AddDays(1) && t.MESFOC >= mEFCFrom && t.MESFOC <= mEFCTo && t.WindSpeed >= windSpdFrom && t.WindSpeed <= windSpdTo && t.Draft >= draftFrom && t.Draft <= draftTo)).Select(t => new MESpdPropDto
            {
                ReceiveDatetime = t.ReceiveDatetime,
                WaterSpeed = t.WaterSpeed,
                Draft = t.Draft,
                MERpm = t.MERpm,
                Trim = t.Trim,
                MESFOC = t.MESFOC,
                MEPower = t.MEPower,
                Slip = t.Slip
            });

            return result.ToList();
        }

        public async Task<IList<PowSpdDto>> GetPowSpdList(string parameters)
        {
            var queryParams = parameters.ToJObject();
            DateTime dateFrom = DateTime.Now;
            DateTime dateTo = DateTime.Now;
            double slipFrom = 0;
            double slipTo = 0;
            double draftFrom = 0;
            double draftTo = 0;
            string sn = "";
            if (queryParams.ContainsKey("SN"))
                sn = queryParams["SN"].ToString();
            if (queryParams.ContainsKey("DateFrom"))
                dateFrom = Convert.ToDateTime(queryParams["DateFrom"]);
            if (queryParams.ContainsKey("DateTo"))
                dateTo = Convert.ToDateTime(queryParams["DateTo"]);
            if (queryParams.ContainsKey("SlipFrom"))
                slipFrom = Convert.ToDouble(queryParams["SlipFrom"]);
            if (queryParams.ContainsKey("SlipTo"))
                slipTo = Convert.ToDouble(queryParams["SlipTo"]);
            if (queryParams.ContainsKey("DraftFrom"))
                draftFrom = Convert.ToDouble(queryParams["DraftFrom"]);
            if (queryParams.ContainsKey("DraftTo"))
                draftTo = Convert.ToDouble(queryParams["DraftTo"]);

            var result = (await _repository.GetListAsync(t => t.delete_time == null && t.SN == sn && t.ReceiveDatetime >= dateFrom && t.ReceiveDatetime < dateTo.AddDays(1) && t.Slip >= slipFrom && t.Slip <= slipTo && t.Draft >= draftFrom && t.Draft <= draftTo)).Select(t => new PowSpdDto
            {
                ReceiveDatetime = t.ReceiveDatetime,
                WaterSpeed = t.WaterSpeed,
                Draft = t.Draft,
                MERpm = t.MERpm,
                Trim = t.Trim,
                MEPower = t.MEPower,
                Slip = t.Slip
            });

            return result.ToList();
        }

        public async Task<IList<PowRpmDto>> GetPowRpmList(string parameters)
        {
            var queryParams = parameters.ToJObject();
            DateTime dateFrom = DateTime.Now;
            DateTime dateTo = DateTime.Now;
            double slipFrom = 0;
            double slipTo = 0;
            double draftFrom = 0;
            double draftTo = 0;
            double windSpdFrom = 0;
            double windSpdTo = 0;
            string sn = "";
            if (queryParams.ContainsKey("SN"))
                sn = queryParams["SN"].ToString();
            if (queryParams.ContainsKey("DateFrom"))
                dateFrom = Convert.ToDateTime(queryParams["DateFrom"]);
            if (queryParams.ContainsKey("DateTo"))
                dateTo = Convert.ToDateTime(queryParams["DateTo"]);
            if (queryParams.ContainsKey("SlipFrom"))
                slipFrom = Convert.ToDouble(queryParams["SlipFrom"]);
            if (queryParams.ContainsKey("SlipTo"))
                slipTo = Convert.ToDouble(queryParams["SlipTo"]);
            if (queryParams.ContainsKey("DraftFrom"))
                draftFrom = Convert.ToDouble(queryParams["DraftFrom"]);
            if (queryParams.ContainsKey("DraftTo"))
                draftTo = Convert.ToDouble(queryParams["DraftTo"]);
            if (queryParams.ContainsKey("WindSpdFrom"))
                windSpdFrom = Convert.ToDouble(queryParams["WindSpdFrom"]);
            if (queryParams.ContainsKey("WindSpdTo"))
                windSpdTo = Convert.ToDouble(queryParams["WindSpdTo"]);

            var result = (await _repository.GetListAsync(t => t.delete_time == null && t.SN == sn && t.ReceiveDatetime >= dateFrom && t.ReceiveDatetime < dateTo.AddDays(1) && t.Slip >= slipFrom && t.Slip <= slipTo && t.Draft >= draftFrom && t.Draft <= draftTo && t.WindSpeed >= windSpdFrom && t.WindSpeed <= windSpdTo)).Select(t => new PowRpmDto
            {
                ReceiveDatetime = t.ReceiveDatetime,
                WaterSpeed = t.WaterSpeed,
                Draft = t.Draft,
                MERpm = t.MERpm,
                Trim = t.Trim,
                MEPower = t.MEPower,
                Slip = t.Slip,
                MEFCPerNm = t.MEFCPerNm
            });

            return result.ToList();
        }

        public async Task<IList<HullDto>> GetHullList(string parameters)
        {
            var queryParams = parameters.ToJObject();
            DateTime dateFrom = DateTime.Now;
            DateTime dateTo = DateTime.Now;
            double slipFrom = 0;
            double slipTo = 0;
            double powerFrom = 0;
            double powerTo = 0;
            string sn = "";
            if (queryParams.ContainsKey("SN"))
                sn = queryParams["SN"].ToString();
            if (queryParams.ContainsKey("DateFrom"))
                dateFrom = Convert.ToDateTime(queryParams["DateFrom"]);
            if (queryParams.ContainsKey("DateTo"))
                dateTo = Convert.ToDateTime(queryParams["DateTo"]);
            if (queryParams.ContainsKey("SlipFrom"))
                slipFrom = Convert.ToDouble(queryParams["SlipFrom"]);
            if (queryParams.ContainsKey("SlipTo"))
                slipTo = Convert.ToDouble(queryParams["SlipTo"]);
            if (queryParams.ContainsKey("PowerFrom"))
                powerFrom = Convert.ToDouble(queryParams["PowerFrom"]);
            if (queryParams.ContainsKey("PowerTo"))
                powerTo = Convert.ToDouble(queryParams["PowerTo"]);

            var result = (await _repository.GetListAsync(t => t.delete_time == null && t.SN == sn && t.ReceiveDatetime >= dateFrom && t.ReceiveDatetime < dateTo.AddDays(1) && t.Slip >= slipFrom && t.Slip <= slipTo && t.MEPower >= powerFrom && t.MEPower <= powerTo)).Select(t => new HullDto
            {
                ReceiveDatetime = t.ReceiveDatetime,
                WaterSpeed = t.WaterSpeed,
                MERpm = t.MERpm,
                MEPower = t.MEPower,
                Slip = t.Slip
            });

            return result.ToList();
        }

        public async Task<IList<METuningDto>> GetMETuningList(string parameters)
        {
            var queryParams = parameters.ToJObject();
            DateTime dateFrom = DateTime.Now;
            DateTime dateTo = DateTime.Now;
            double slipFrom = 0;
            double slipTo = 0;
            double powerFrom = 0;
            double powerTo = 0;
            string sn = "";
            if (queryParams.ContainsKey("SN"))
                sn = queryParams["SN"].ToString();
            if (queryParams.ContainsKey("DateFrom"))
                dateFrom = Convert.ToDateTime(queryParams["DateFrom"]);
            if (queryParams.ContainsKey("DateTo"))
                dateTo = Convert.ToDateTime(queryParams["DateTo"]);
            if (queryParams.ContainsKey("SlipFrom"))
                slipFrom = Convert.ToDouble(queryParams["SlipFrom"]);
            if (queryParams.ContainsKey("SlipTo"))
                slipTo = Convert.ToDouble(queryParams["SlipTo"]);
            if (queryParams.ContainsKey("PowerFrom"))
                powerFrom = Convert.ToDouble(queryParams["PowerFrom"]);
            if (queryParams.ContainsKey("PowerTo"))
                powerTo = Convert.ToDouble(queryParams["PowerTo"]);

            var result = (await _repository.GetListAsync(t => t.delete_time == null && t.SN == sn && t.ReceiveDatetime >= dateFrom && t.ReceiveDatetime < dateTo.AddDays(1) && t.Slip >= slipFrom && t.Slip <= slipTo && t.MEPower >= powerFrom && t.MEPower <= powerTo)).Select(t => new METuningDto
            {
                ReceiveDatetime = t.ReceiveDatetime,
                WaterSpeed = t.WaterSpeed,
                MERpm = t.MERpm,
                MEPower = t.MEPower,
                Slip = t.Slip,
                Draft = t.Draft,
                MEFCPerNm = t.MEFCPerNm,
                MESFOC = t.MESFOC,
                Trim = t.Trim,
                WindDirection = t.WindDirection,
                WindSpeed = t.WindSpeed
            });

            return result.ToList();
        }

        public async Task<IList<HullPropellerDto>> GetHullPropellerList(string parameters)
        {
            var queryParams = parameters.ToJObject();
            DateTime dateFrom = DateTime.Now;
            DateTime dateTo = DateTime.Now;
            double slipFrom = 0;
            double slipTo = 0;
            double speedFrom = 0;
            double speedTo = 0;
            double windSpdFrom = 0;
            double windSpdTo = 0;
            double windDirFrom = 0;
            double windDirTo = 0;
            string sn = "";
            if (queryParams.ContainsKey("SN"))
                sn = queryParams["SN"].ToString();
            if (queryParams.ContainsKey("DateFrom"))
                dateFrom = Convert.ToDateTime(queryParams["DateFrom"]);
            if (queryParams.ContainsKey("DateTo"))
                dateTo = Convert.ToDateTime(queryParams["DateTo"]);
            if (queryParams.ContainsKey("SlipFrom"))
                slipFrom = Convert.ToDouble(queryParams["SlipFrom"]);
            if (queryParams.ContainsKey("SlipTo"))
                slipTo = Convert.ToDouble(queryParams["SlipTo"]);
            if (queryParams.ContainsKey("SpeedFrom"))
                speedFrom = Convert.ToDouble(queryParams["SpeedFrom"]);
            if (queryParams.ContainsKey("SpeedTo"))
                speedTo = Convert.ToDouble(queryParams["SpeedTo"]);
            if (queryParams.ContainsKey("WindSpdFrom"))
                windSpdFrom = Convert.ToDouble(queryParams["WindSpdFrom"]);
            if (queryParams.ContainsKey("WindSpdTo"))
                windSpdTo = Convert.ToDouble(queryParams["WindSpdTo"]);
            if (queryParams.ContainsKey("WindDirFrom"))
                windDirFrom = Convert.ToDouble(queryParams["WindDirFrom"]);
            if (queryParams.ContainsKey("WindDirTo"))
                windDirTo = Convert.ToDouble(queryParams["WindDirTo"]);

            var result = (await _repository.GetListAsync(t => t.delete_time == null && t.SN == sn && t.ReceiveDatetime >= dateFrom && t.ReceiveDatetime < dateTo.AddDays(1) && t.Slip >= slipFrom && t.Slip <= slipTo && t.WaterSpeed >= speedFrom && t.WaterSpeed <= speedTo && t.WindSpeed >= windSpdFrom && t.WindSpeed <= windSpdTo && t.WindDirection >= windDirFrom && t.WindDirection <= windDirTo)).Select(t => new HullPropellerDto
            {
                ReceiveDatetime = t.ReceiveDatetime,
                WaterSpeed = t.WaterSpeed,
                MERpm = t.MERpm,
                MEPower = t.MEPower,
                Slip = t.Slip,
                Draft = t.Draft,
                MEFCPerNm = t.MEFCPerNm,
                MESFOC = t.MESFOC,
                Trim = t.Trim,
                WindDirection = t.WindDirection,
                WindSpeed = t.WindSpeed
            });

            return result.ToList();
        }

        public async Task<IList<MELoadPropDto>> GetMELoadPropList(string parameters)
        {
            var queryParams = parameters.ToJObject();
            DateTime dateFrom = DateTime.Now;
            DateTime dateTo = DateTime.Now;
            double rpmFrom = 0;
            double rpmTo = 0;
            double windSpdFrom = 0;
            double windSpdTo = 0;
            double draftFrom = 0;
            double draftTo = 0;
            string sn = "";
            if (queryParams.ContainsKey("SN"))
                sn = queryParams["SN"].ToString();
            if (queryParams.ContainsKey("DateFrom"))
                dateFrom = Convert.ToDateTime(queryParams["DateFrom"]);
            if (queryParams.ContainsKey("DateTo"))
                dateTo = Convert.ToDateTime(queryParams["DateTo"]);
            if (queryParams.ContainsKey("RpmFrom"))
                rpmFrom = Convert.ToDouble(queryParams["RpmFrom"]);
            if (queryParams.ContainsKey("RpmTo"))
                rpmTo = Convert.ToDouble(queryParams["RpmTo"]);
            if (queryParams.ContainsKey("WindSpdFrom"))
                windSpdFrom = Convert.ToDouble(queryParams["WindSpdFrom"]);
            if (queryParams.ContainsKey("WindSpdTo"))
                windSpdTo = Convert.ToDouble(queryParams["WindSpdTo"]);
            if (queryParams.ContainsKey("DraftFrom"))
                draftFrom = Convert.ToDouble(queryParams["DraftFrom"]);
            if (queryParams.ContainsKey("DraftTo"))
                draftTo = Convert.ToDouble(queryParams["DraftTo"]);

            var result = (await _repository.GetListAsync(t => t.delete_time == null && t.SN == sn && t.ReceiveDatetime >= dateFrom && t.ReceiveDatetime < dateTo.AddDays(1) && t.MERpm >= rpmFrom && t.MERpm <= rpmTo && t.WindSpeed >= windSpdFrom && t.WindSpeed <= windSpdTo && t.Draft >= draftFrom && t.Draft <= draftTo)).Select(t => new MELoadPropDto
            {
                ReceiveDatetime = t.ReceiveDatetime,
                WaterSpeed = t.WaterSpeed,
                MERpm = t.MERpm,
                MEPower = t.MEPower,
                Slip = t.Slip,
                Draft = t.Draft,
                MEFCPerNm = t.MEFCPerNm,
                MESFOC = t.MESFOC,
                Trim = t.Trim
            });

            return result.ToList();
        }

        public async Task<IEnumerable<object>> GetAnalysisList(string parameters)
        {
            var queryParams = parameters.ToJObject();
            /*Expression<Func<VesselInfo, bool>> expression = t => t.delete_time == null;
            if (queryParams.ContainsKey("vdrId") && !string.IsNullOrWhiteSpace(queryParams["vdrId"].ToString()))
                expression = expression.And(t => t.SN == queryParams["vdrId"].ToString());
            if (queryParams.ContainsKey("dateFrom") && !string.IsNullOrWhiteSpace(queryParams["dateFrom"].ToString()))
                expression = expression.And(t => t.ReceiveDatetime >= DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt32(queryParams["dateFrom"])));
            if (queryParams.ContainsKey("dateTo") && !string.IsNullOrWhiteSpace(queryParams["dateTo"].ToString()))
                expression = expression.And(t => t.ReceiveDatetime < DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt32(queryParams["dateTo"])));
            if (queryParams.ContainsKey("slipFrom") && !string.IsNullOrWhiteSpace(queryParams["slipFrom"].ToString()))
                expression = expression.And(t => t.Slip >= Convert.ToDouble(queryParams["slipFrom"]));
            if (queryParams.ContainsKey("slipTo") && !string.IsNullOrWhiteSpace(queryParams["slipTo"].ToString()))
                expression = expression.And(t => t.Slip < Convert.ToDouble(queryParams["slipTo"]));
            if (queryParams.ContainsKey("draftFrom") && !string.IsNullOrWhiteSpace(queryParams["draftFrom"].ToString()))
                expression = expression.And(t => t.Draft >= Convert.ToDouble(queryParams["draftFrom"]));
            if (queryParams.ContainsKey("draftTo") && !string.IsNullOrWhiteSpace(queryParams["draftTo"].ToString()))
                expression = expression.And(t => t.Draft < Convert.ToDouble(queryParams["draftTo"]));
            if (queryParams.ContainsKey("windSpeedFrom") && !string.IsNullOrWhiteSpace(queryParams["windSpeedFrom"].ToString()))
                expression = expression.And(t => t.WindSpeed >= Convert.ToDouble(queryParams["windSpeedFrom"]));
            if (queryParams.ContainsKey("windSpeedTo") && !string.IsNullOrWhiteSpace(queryParams["windSpeedTo"].ToString()))
                expression = expression.And(t => t.WindSpeed < Convert.ToDouble(queryParams["windSpeedTo"]));
            if (queryParams.ContainsKey("windDirectionFrom") && !string.IsNullOrWhiteSpace(queryParams["windDirectionFrom"].ToString()))
                expression = expression.And(t => t.WindDirection >= Convert.ToDouble(queryParams["windDirectionFrom"]));
            if (queryParams.ContainsKey("windDirectionTo") && !string.IsNullOrWhiteSpace(queryParams["windDirectionTo"].ToString()))
                expression = expression.And(t => t.WindDirection < Convert.ToDouble(queryParams["windDirectionTo"]));
            if (queryParams.ContainsKey("groundSpeedFrom") && !string.IsNullOrWhiteSpace(queryParams["groundSpeedFrom"].ToString()))
                expression = expression.And(t => t.GroundSpeed >= Convert.ToDouble(queryParams["groundSpeedFrom"]));
            if (queryParams.ContainsKey("groundSpeedTo") && !string.IsNullOrWhiteSpace(queryParams["groundSpeedTo"].ToString()))
                expression = expression.And(t => t.GroundSpeed < Convert.ToDouble(queryParams["groundSpeedTo"]));
            if (queryParams.ContainsKey("fCFrom") && !string.IsNullOrWhiteSpace(queryParams["fCFrom"].ToString()))
                expression = expression.And(t => t.MEHFOCACC >= Convert.ToDouble(queryParams["fCFrom"]));
            if (queryParams.ContainsKey("fCTo") && !string.IsNullOrWhiteSpace(queryParams["fCTo"].ToString()))
                expression = expression.And(t => t.MEHFOCACC < Convert.ToDouble(queryParams["fCTo"]));
            if (queryParams.ContainsKey("mEPowerFrom") && !string.IsNullOrWhiteSpace(queryParams["mEPowerFrom"].ToString()))
                expression = expression.And(t => t.MEPower >= Convert.ToDouble(queryParams["mEPowerFrom"]));
            if (queryParams.ContainsKey("mEPowerTo") && !string.IsNullOrWhiteSpace(queryParams["mEPowerTo"].ToString()))
                expression = expression.And(t => t.MEPower < Convert.ToDouble(queryParams["mEPowerTo"]));
            if (queryParams.ContainsKey("mERpmFrom") && !string.IsNullOrWhiteSpace(queryParams["mERpmFrom"].ToString()))
                expression = expression.And(t => t.MERpm >= Convert.ToDouble(queryParams["mERpmFrom"]));
            if (queryParams.ContainsKey("mERpmTo") && !string.IsNullOrWhiteSpace(queryParams["mERpmTo"].ToString()))
                expression = expression.And(t => t.MERpm < Convert.ToDouble(queryParams["mERpmTo"]));
            string x = "";
            string y = "";
            if (queryParams.ContainsKey("x") && !string.IsNullOrWhiteSpace(queryParams["x"].ToString()))
                x = queryParams["x"].ToString().Trim();
            if (queryParams.ContainsKey("y") && !string.IsNullOrWhiteSpace(queryParams["y"].ToString()))
                y = queryParams["y"].ToString().Trim();

            var result = (await _repository.GetListAsync(expression)).Select(t => new
            {
                xvalue = x.Equals("GroundSpeed") ? t.GroundSpeed : x.Equals("MESFOC") ? t.MESFOC : x.Equals("Trim") ? t.Trim : x.Equals("MERpm") ? t.MERpm : x.Equals("date") ? Convert.ToDouble(new DateTimeOffset(t.ReceiveDatetime).ToUnixTimeSeconds) : t.MESFOC,
                yvalue = y.Equals("MEFCPerNm") ? t.MEFCPerNm : x.Equals("FCPerNm") ? t.FCPerNm : x.Equals("MEPower") ? t.MEPower : x.Equals("MESFOC") ? t.MESFOC : x.Equals("GroundSpeed") ? t.GroundSpeed : t.MESFOC
            }).OrderBy(t => t.xvalue);

            return result;*/

            var sbSql = new StringBuilder();
            sbSql.Append("SELECT ");
            if (queryParams.ContainsKey("x") && !string.IsNullOrWhiteSpace(queryParams["x"].ToString()))
                switch (queryParams["x"].ToString().Trim())
                {
                    case "GroundSpeed":
                        sbSql.Append(@"t0.""GroundSpeed"" ""xvalue"",");
                        break;

                    case "MESFOC":
                        sbSql.Append(@"t0.""MESFOC"" ""xvalue"",");
                        break;

                    case "Trim":
                        sbSql.Append(@"t0.""Trim"" ""xvalue"",");
                        break;

                    case "MERpm":
                        sbSql.Append(@"t1.""Rpm"" ""xvalue"",");
                        break;

                    case "Date":
                        sbSql.Append(@"t0.""ReceiveDatetime"" ""xvalue"",");
                        break;

                    default:
                        sbSql.Append(@"t0.""MESFOC"" ""xvalue"",");
                        break;
                }
            else
                sbSql.Append(@"t0.""MESFOC"" ""xvalue"",");

            if (queryParams.ContainsKey("y") && !string.IsNullOrWhiteSpace(queryParams["y"].ToString()))
                switch (queryParams["y"].ToString().Trim())
                {
                    case "MEFCPerNm":
                        sbSql.Append(@"t0.""MEFCPerNm"" ""yvalue"" ");
                        break;

                    case "FCPerNm":
                        sbSql.Append(@"t0.""FCPerNm"" ""yvalue"" ");
                        break;

                    case "MEPower":
                        sbSql.Append(@"t1.""Power"" ""yvalue"" ");
                        break;

                    case "MESFOC":
                        sbSql.Append(@"t1.""MESFOC"" ""yvalue"" ");
                        break;

                    case "GroundSpeed":
                        sbSql.Append(@"t0.""GroundSpeed"" ""yvalue"" ");
                        break;

                    default:
                        sbSql.Append(@"t0.""MESFOC"" ""yvalue"" ");
                        break;
                }
            else
                sbSql.Append(@"t0.""MESFOC"" ""yvalue"" ");
            sbSql.Append(@"FROM ""vesselinfo"" t0,""energy_totalindicator"" t1 WHERE NVL( t0.""delete_time"", TO_TIMESTAMP( '1949-10-01', 'YYYY-MM-DD' ) ) = TO_TIMESTAMP( '1949-10-01', 'YYYY-MM-DD' ) AND t0.SN = t1.""Number"" AND t0.""ReceiveDatetime"" = t1.""ReceiveDatetime"" ");
            if (queryParams.ContainsKey("vdrId") && !string.IsNullOrWhiteSpace(queryParams["vdrId"].ToString()))
                sbSql.AppendFormat(@"AND t0.""SN""='{0}' ", queryParams["vdrId"].ToString());
            if (queryParams.ContainsKey("dateFrom") && !string.IsNullOrWhiteSpace(queryParams["dateFrom"].ToString()))
                sbSql.AppendFormat(@"AND t0.""ReceiveDatetime"" >= TO_TIMESTAMP( '{0}', 'YYYY-MM-DD HH24:MI:SS' ) ", DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt32(queryParams["dateFrom"])).ToString("yyyy-MM-dd HH:mm:ss"));
            if (queryParams.ContainsKey("dateTo") && !string.IsNullOrWhiteSpace(queryParams["dateTo"].ToString()))
                sbSql.AppendFormat(@"AND t0.""ReceiveDatetime"" < TO_TIMESTAMP( '{0}', 'YYYY-MM-DD HH24:MI:SS' ) ", DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt32(queryParams["dateTo"])).ToString("yyyy-MM-dd HH:mm:ss"));
            if (queryParams.ContainsKey("slipFrom") && !string.IsNullOrWhiteSpace(queryParams["slipFrom"].ToString()))
                sbSql.AppendFormat(@"AND t0.""Slip"">={0} ", Convert.ToDouble(queryParams["slipFrom"]));
            if (queryParams.ContainsKey("slipTo") && !string.IsNullOrWhiteSpace(queryParams["slipTo"].ToString()))
                sbSql.AppendFormat(@"AND t0.""Slip""<{0} ", Convert.ToDouble(queryParams["slipTo"]));
            if (queryParams.ContainsKey("draftFrom") && !string.IsNullOrWhiteSpace(queryParams["draftFrom"].ToString()))
                sbSql.AppendFormat(@"AND t0.""Draft"">={0} ", Convert.ToDouble(queryParams["draftFrom"]));
            if (queryParams.ContainsKey("draftTo") && !string.IsNullOrWhiteSpace(queryParams["draftTo"].ToString()))
                sbSql.AppendFormat(@"AND t0.""Draft""<{0} ", Convert.ToDouble(queryParams["draftTo"]));
            if (queryParams.ContainsKey("windSpeedFrom") && !string.IsNullOrWhiteSpace(queryParams["windSpeedFrom"].ToString()))
                sbSql.AppendFormat(@"AND t0.""WindSpeed"">={0} ", Convert.ToDouble(queryParams["windSpeedFrom"]));
            if (queryParams.ContainsKey("windSpeedTo") && !string.IsNullOrWhiteSpace(queryParams["windSpeedTo"].ToString()))
                sbSql.AppendFormat(@"AND t0.""WindSpeed""<{0} ", Convert.ToDouble(queryParams["windSpeedTo"]));
            if (queryParams.ContainsKey("windDirectionFrom") && !string.IsNullOrWhiteSpace(queryParams["windDirectionFrom"].ToString()))
                sbSql.AppendFormat(@"AND t0.""WindDirection"">={0} ", Convert.ToDouble(queryParams["windDirectionFrom"]));
            if (queryParams.ContainsKey("windDirectionTo") && !string.IsNullOrWhiteSpace(queryParams["windDirectionTo"].ToString()))
                sbSql.AppendFormat(@"AND t0.""WindDirection""<{0} ", Convert.ToDouble(queryParams["windDirectionTo"]));
            if (queryParams.ContainsKey("groundSpeedFrom") && !string.IsNullOrWhiteSpace(queryParams["groundSpeedFrom"].ToString()))
                sbSql.AppendFormat(@"AND t0.""GroundSpeed"">={0} ", Convert.ToDouble(queryParams["groundSpeedFrom"]));
            if (queryParams.ContainsKey("groundSpeedTo") && !string.IsNullOrWhiteSpace(queryParams["groundSpeedTo"].ToString()))
                sbSql.AppendFormat(@"AND t0.""GroundSpeed""<{0} ", Convert.ToDouble(queryParams["groundSpeedTo"]));
            if (queryParams.ContainsKey("fCFrom") && !string.IsNullOrWhiteSpace(queryParams["fCFrom"].ToString()))
                sbSql.AppendFormat(@"AND t1.""HFO"">={0} ", Convert.ToDouble(queryParams["fCFrom"]));
            if (queryParams.ContainsKey("fCTo") && !string.IsNullOrWhiteSpace(queryParams["fCTo"].ToString()))
                sbSql.AppendFormat(@"AND t1.""HFO""<{0} ", Convert.ToDouble(queryParams["fCTo"]));
            if (queryParams.ContainsKey("mEPowerFrom") && !string.IsNullOrWhiteSpace(queryParams["mEPowerFrom"].ToString()))
                sbSql.AppendFormat(@"AND t1.""Power"">={0} ", Convert.ToDouble(queryParams["mEPowerFrom"]));
            if (queryParams.ContainsKey("mEPowerTo") && !string.IsNullOrWhiteSpace(queryParams["mEPowerTo"].ToString()))
                sbSql.AppendFormat(@"AND t1.""Power""<{0} ", Convert.ToDouble(queryParams["mEPowerTo"]));
            if (queryParams.ContainsKey("mERpmFrom") && !string.IsNullOrWhiteSpace(queryParams["mERpmFrom"].ToString()))
                sbSql.AppendFormat(@"AND t1.""Rpm"">={0} ", Convert.ToDouble(queryParams["mERpmFrom"]));
            if (queryParams.ContainsKey("mERpmTo") && !string.IsNullOrWhiteSpace(queryParams["mERpmTo"].ToString()))
                sbSql.AppendFormat(@"AND t1.""Rpm""<{0} ", Convert.ToDouble(queryParams["mERpmTo"]));

            if (queryParams.ContainsKey("x") && !string.IsNullOrWhiteSpace(queryParams["x"].ToString()))
                switch (queryParams["x"].ToString().Trim())
                {
                    case "MESFOC":
                        sbSql.AppendFormat(@"AND t1.""Power"">0 ");
                        break;
                }

            if (queryParams.ContainsKey("y") && !string.IsNullOrWhiteSpace(queryParams["y"].ToString()))
                switch (queryParams["y"].ToString().Trim())
                {
                    case "MEFCPerNm":
                        sbSql.AppendFormat(@"AND t0.""GroundSpeed"">0 ");
                        break;

                    case "FCPerNm":
                        sbSql.AppendFormat(@"AND t0.""GroundSpeed"">0 ");
                        break;

                    case "MESFOC":
                        sbSql.AppendFormat(@"AND t1.""Power"">0 ");
                        break;
                }

            sbSql.Append(@"ORDER BY ""xvalue"",""yvalue""");
            return await _repository.QueryFromSql(sbSql.ToString());
        }

        public async Task<NoonDto> GetNoonData(string number, DateTimeOffset dateTimeOffset, DateTimeOffset departureDTS)
        {
            var entity = new NoonDto();

            var yesterday = await _repository.GetYesterdayAsync(number, dateTimeOffset);
            var today = await _repository.GetTodayAsync(number, dateTimeOffset);

            if (today == null)
                return new NoonDto();

            var departure = await _repository.GetDepartureAsync(number, departureDTS);
            var vesselInfos = await _repository.GetListAsync(t => t.delete_time == null && t.SN == number && t.ReceiveDatetime >= Convert.ToDateTime(dateTimeOffset.AddDays(-1).ToString("yyyy-MM-dd 12:00:00")) && t.ReceiveDatetime < Convert.ToDateTime(dateTimeOffset.ToString("yyyy-MM-dd 12:00:00")) && t.WaterSpeed > 1);

            entity.Distance = today.TotalDistanceGrd ?? 0 - yesterday.TotalDistanceGrd ?? 0;
            entity.DistanceWater = today.TotalDistanceWat ?? 0 - yesterday.TotalDistanceWat ?? 0;
            entity.Duration = Convert.ToDouble(vesselInfos.Count) * 10d / 3600d;
            entity.Speed = entity.Distance / entity.Duration;
            if (entity.Speed == 0)
                entity.Speed = vesselInfos.Average(t => t.GroundSpeed) ?? 0;
            entity.SpeedWater = entity.DistanceWater / entity.Duration;
            if (entity.SpeedWater == 0)
                entity.SpeedWater = vesselInfos.Average(t => t.WaterSpeed) ?? 0;
            entity.MERpm = vesselInfos.Average(t => t.MERpm) ?? 0;
            entity.Slip = vesselInfos.Average(t => t.Slip) ?? 0;
            entity.DistanceTotally = today.TotalDistanceGrd ?? 0 - departure.TotalDistanceGrd ?? 0;
            entity.DurationTotally = Convert.ToDouble(await _repository.CountAsync(t => t.delete_time == null && t.SN == number && t.ReceiveDatetime >= departureDTS.DateTime && t.ReceiveDatetime < Convert.ToDateTime(dateTimeOffset.ToString("yyyy-MM-dd 12:00:00")) && t.WaterSpeed > 0)) * 10d / 3600d;
            entity.SpeedTotally = entity.DistanceTotally / entity.DurationTotally;
            if (entity.SpeedTotally == 0)
                entity.SpeedTotally = (await _repository.GetListAsync(t => t.delete_time == null && t.SN == number && t.ReceiveDatetime >= departureDTS.DateTime && t.ReceiveDatetime < Convert.ToDateTime(dateTimeOffset.ToString("yyyy-MM-dd 12:00:00")) && t.WaterSpeed > 0)).Average(t => t.GroundSpeed) ?? 0;
            entity.DOConsumption = today.MEMDOCACC ?? 0 + today.DGMDOCACC ?? 0 + today.BLGMDOCACC ?? 0 - yesterday.MEMDOCACC ?? 0 - yesterday.DGMDOCACC ?? 0 - yesterday.BLGMDOCACC ?? 0;
            entity.FOConsumption = today.MEHFOCACC ?? 0 + today.DGHFOCACC ?? 0 + today.BLGHFOCACC ?? 0 - yesterday.MEHFOCACC ?? 0 - yesterday.DGHFOCACC ?? 0 - yesterday.BLGHFOCACC ?? 0;
            entity.MEFuelConsumption = today.MEMDOCACC ?? 0 + today.MEHFOCACC ?? 0 - yesterday.MEMDOCACC ?? 0 - yesterday.MEHFOCACC ?? 0;
            entity.DGFuelConsumption = today.DGMDOCACC ?? 0 + today.DGHFOCACC ?? 0 - yesterday.DGMDOCACC ?? 0 - yesterday.DGHFOCACC ?? 0;
            entity.BLRFuelConsumption = today.BLGMDOCACC ?? 0 + today.BLGHFOCACC ?? 0 - yesterday.BLGMDOCACC ?? 0 - yesterday.BLGHFOCACC ?? 0;

            entity.Longitude = today.Longitude ?? 0;
            entity.Latitude = today.Latitude ?? 0;
            entity.WindDirection = today.WindDirection ?? 0;
            entity.WindSpeed = today.WindSpeed ?? 0;
            entity.SeaTemperature = today.SeaTemperature ?? 0;
            entity.WaveDirection = today.WaveDirection ?? 0;
            entity.WaveHeight = today.WaveHeight ?? 0;
            entity.Weather = today.Weather;
            entity.Temperature = today.Temperature ?? 0;
            entity.Pressure = today.Pressure ?? 0;
            entity.Visibility = today.Visibility ?? 0;

            return entity;
        }

        public async Task<object> GetWeekBaseAsync(string parameters, string lang)
        {
            try
            {
                var queryParams = parameters.ToJObject();
                string number = "";
                DateTime dateFrom = DateTime.UtcNow.AddDays(-7);
                DateTime dateTo = DateTime.UtcNow;
                number = queryParams["number"].ToString();
                if (queryParams.ContainsKey("dateFrom") && !string.IsNullOrWhiteSpace(queryParams["dateFrom"].ToString()))
                {
                    dateFrom = Convert.ToDateTime(queryParams["dateFrom"].ToString());
                }
                if (queryParams.ContainsKey("dateTo") && !string.IsNullOrWhiteSpace(queryParams["dateTo"].ToString()))
                {
                    dateTo = Convert.ToDateTime(queryParams["dateTo"].ToString()).AddDays(1);
                }
                var shipType = 0;
                if (queryParams.ContainsKey("shipType") && !string.IsNullOrWhiteSpace(queryParams["shipType"].ToString()))
                {
                    shipType = Convert.ToInt32(queryParams["shipType"].ToString());
                }
                var weights = new DataTable();
                if (queryParams.ContainsKey("weights") && !string.IsNullOrWhiteSpace(queryParams["weights"].ToString()))
                {
                    weights = queryParams["weights"].ToString().ToTable();
                }
                var dwt = 1f;
                if (queryParams.ContainsKey("dwt") && !string.IsNullOrWhiteSpace(queryParams["dwt"].ToString()))
                {
                    dwt = Convert.ToSingle(queryParams["dwt"]);
                }
                var gt = 1f;
                if (queryParams.ContainsKey("gt") && !string.IsNullOrWhiteSpace(queryParams["gt"].ToString()))
                {
                    gt = Convert.ToSingle(queryParams["gt"]);
                }
                var cargoCapacity = 1d;
                if (queryParams.ContainsKey("cargoCapacity") && !string.IsNullOrWhiteSpace(queryParams["cargoCapacity"].ToString()))
                {
                    cargoCapacity = Convert.ToSingle(queryParams["cargoCapacity"]);
                }

                if (number == "HC-A2304") //国能长江01
                {
                    #region 国能长江01

                    StringBuilder sbSql = new StringBuilder();

                    //起始日期流量计总量
                    sbSql.AppendFormat(@"SELECT
	                                    tf.""ConsAct"",
	                                    tf.""ConsAcc"",
	                                    tf.""FuelType"",
	                                    tf.""DeviceNo""
                                    FROM
	                                    ""energy_flowmeter"" tf,
	                                    ""vesselinfo"" t0
                                    WHERE
	                                    NVL(""delete_time"", TO_TIMESTAMP('1949-10-01', 'YYYY-MM-DD')) = TO_TIMESTAMP('1949-10-01', 'YYYY-MM-DD')
	                                    AND t0.""SN"" = tf.""Number""
	                                    AND t0.""ReceiveDatetime"" = tf.""ReceiveDatetime""
	                                    AND tf.""Number"" = '{0}'
	                                    AND tf.""ReceiveDatetime"" = (
	                                    SELECT
		                                    ""ReceiveDatetime""
	                                    FROM
		                                    (
		                                    SELECT
			                                    tf.""ReceiveDatetime""
		                                    FROM
			                                    ""energy_flowmeter"" tf,
			                                    ""vesselinfo"" t0
		                                    WHERE
			                                    NVL(t0.""delete_time"", TO_TIMESTAMP('1949-10-01', 'YYYY-MM-DD')) = TO_TIMESTAMP('1949-10-01', 'YYYY-MM-DD')
			                                    AND t0.""SN"" = tf.""Number""
			                                    AND t0.""ReceiveDatetime"" = tf.""ReceiveDatetime""
			                                    AND tf.""Number"" = '{0}'
			                                    AND tf.""ReceiveDatetime"" >= TO_TIMESTAMP('{1}', 'YYYY-MM-DD HH24:MI:SS')
		                                    GROUP BY
			                                    tf.""Number"",
			                                    tf.""ReceiveDatetime""
		                                    HAVING
			                                    COUNT(1) >= 3
		                                    ORDER BY
			                                    tf.""ReceiveDatetime""
		                                    )
	                                    WHERE
	                                    ROWNUM = 1
	                                    )", number, dateFrom.ToString("yyyy-MM-dd 00:00:00"));
                    var firstFlowmeters = await _sqlRepository.ExecuteDataTable(sbSql.ToString());

                    sbSql.Clear();

                    //终止日期流量计总量
                    sbSql.AppendFormat(@"SELECT
	                                    tf.""ConsAct"",
	                                    tf.""ConsAcc"",
	                                    tf.""FuelType"",
	                                    tf.""DeviceNo""
                                    FROM
	                                    ""energy_flowmeter"" tf,
	                                    ""vesselinfo"" t0
                                    WHERE
	                                    NVL(""delete_time"", TO_TIMESTAMP('1949-10-01', 'YYYY-MM-DD')) = TO_TIMESTAMP('1949-10-01', 'YYYY-MM-DD')
	                                    AND t0.""SN"" = tf.""Number""
	                                    AND t0.""ReceiveDatetime"" = tf.""ReceiveDatetime""
	                                    AND tf.""Number"" = '{0}'
	                                    AND tf.""ReceiveDatetime"" = (
	                                    SELECT
		                                    ""ReceiveDatetime""
	                                    FROM
		                                    (
		                                    SELECT
			                                    tf.""ReceiveDatetime""
		                                    FROM
			                                    ""energy_flowmeter"" tf,
			                                    ""vesselinfo"" t0
		                                    WHERE
			                                    NVL(t0.""delete_time"", TO_TIMESTAMP('1949-10-01', 'YYYY-MM-DD')) = TO_TIMESTAMP('1949-10-01', 'YYYY-MM-DD')
			                                    AND t0.""SN"" = tf.""Number""
			                                    AND t0.""ReceiveDatetime"" = tf.""ReceiveDatetime""
			                                    AND tf.""Number"" = '{0}'
			                                    AND tf.""ReceiveDatetime"" < TO_TIMESTAMP('{1}', 'YYYY-MM-DD HH24:MI:SS')
		                                    GROUP BY
			                                    tf.""Number"",
			                                    tf.""ReceiveDatetime""
		                                    HAVING
			                                    COUNT(1) >= 3
		                                    ORDER BY
			                                    tf.""ReceiveDatetime"" DESC
		                                    )
	                                    WHERE
	                                    ROWNUM = 1
	                                    )", number, dateTo.ToString("yyyy-MM-dd 00:00:00"));
                    var lastFlowmeters = await _sqlRepository.ExecuteDataTable(sbSql.ToString());

                    sbSql.Clear();

                    //燃料消耗列表
                    sbSql.AppendFormat(@"SELECT
	                                t1.""DeviceType"",
	                                CAST(MAX(t1.HFO)-MIN(t1.HFO) AS DECIMAL(20,4)) HFO,
	                                CAST(MAX(t1.""Methanol"")-MIN(t1.""Methanol"") AS DECIMAL(20,4)) ""Methanol""
                                FROM
	                                ""vesselinfo"" t0,
	                                ""energy_powerunit"" t1
                                WHERE
	                                NVL( t0.""delete_time"", TO_TIMESTAMP( '1949-10-01', 'YYYY-MM-DD' ) ) = TO_TIMESTAMP( '1949-10-01', 'YYYY-MM-DD' )
	                                AND t0.SN=t1.""Number""
	                                AND t0.""ReceiveDatetime""=t1.""ReceiveDatetime""
	                                AND t1.""Number""='{0}'
	                                AND t1.""ReceiveDatetime"" >= TO_TIMESTAMP('{1}', 'YYYY-MM-DD HH24:MI:SS')
	                                AND t1.""ReceiveDatetime"" < TO_TIMESTAMP('{2}', 'YYYY-MM-DD HH24:MI:SS')
	                                GROUP BY t1.""DeviceType""", number, dateFrom.ToString("yyyy-MM-dd 00:00:00"), dateTo.ToString("yyyy-MM-dd 00:00:00"));
                    var PUs = await _sqlRepository.ExecuteDataTable(sbSql.ToString());
                    sbSql.Clear();

                    //平均燃油效率
                    sbSql.AppendFormat(@$"SELECT
	                                    t.*,
	                                    CAST(t.""DistanceGrd"" / t.""RunTime"" AS DECIMAL(14,4)) ""CalcSpeedGrd"",
	                                    CAST(t.""AVGDGO"" / DECODE(t.""AVGSpeedGrd"",0,1000000000,t.""AVGSpeedGrd"") AS DECIMAL(14,4)) ""AVGDGOpNM"",
	                                    CAST(t.""AVGLFO"" / DECODE(t.""AVGSpeedGrd"",0,1000000000,t.""AVGSpeedGrd"") AS DECIMAL(14,4)) ""AVGLFOpNM"",
	                                    CAST(t.""AVGHFO"" / DECODE(t.""AVGSpeedGrd"",0,1000000000,t.""AVGSpeedGrd"") AS DECIMAL(14,4)) ""AVGHFOpNM"",
	                                    CAST(t.""AVGLPG_P"" / DECODE(t.""AVGSpeedGrd"",0,1000000000,t.""AVGSpeedGrd"") AS DECIMAL(14,4)) ""AVGLPG_PpNM"",
	                                    CAST(t.""AVGLPG_B"" / DECODE(t.""AVGSpeedGrd"",0,1000000000,t.""AVGSpeedGrd"") AS DECIMAL(14,4)) ""AVGLPG_BpNM"",
	                                    CAST(t.""AVGLNG"" / DECODE(t.""AVGSpeedGrd"",0,1000000000,t.""AVGSpeedGrd"") AS DECIMAL(14,4)) ""AVGLNGpNM"",
	                                    CAST(t.""AVGMethanol"" / DECODE(t.""AVGSpeedGrd"",0,1000000000,t.""AVGSpeedGrd"") AS DECIMAL(14,4)) ""AVGMethanolpNM"",
	                                    CAST(t.""AVGEthanol"" / DECODE(t.""AVGSpeedGrd"",0,1000000000,t.""AVGSpeedGrd"") AS DECIMAL(14,4)) ""AVGEthanolpNM"",
	                                    CAST(t.""AVGDGO"" / DECODE(t.""AVGPower"",0,1000000000,t.""AVGPower"") AS DECIMAL(14,4)) ""AVGDGOpPower"",
	                                    CAST(t.""AVGLFO"" / DECODE(t.""AVGPower"",0,1000000000,t.""AVGPower"") AS DECIMAL(14,4)) ""AVGLFOpPower"",
	                                    CAST(t.""AVGHFO"" / DECODE(t.""AVGPower"",0,1000000000,t.""AVGPower"") AS DECIMAL(14,4)) ""AVGHFOpPower"",
	                                    CAST(t.""AVGLPG_P"" / DECODE(t.""AVGPower"",0,1000000000,t.""AVGPower"") AS DECIMAL(14,4)) ""AVGLPG_PpPower"",
	                                    CAST(t.""AVGLPG_B"" / DECODE(t.""AVGPower"",0,1000000000,t.""AVGPower"") AS DECIMAL(14,4)) ""AVGLPG_BpPower"",
	                                    CAST(t.""AVGLNG"" / DECODE(t.""AVGPower"",0,1000000000,t.""AVGPower"") AS DECIMAL(14,4)) ""AVGLNGpPower"",
	                                    CAST(t.""AVGMethanol"" / DECODE(t.""AVGPower"",0,1000000000,t.""AVGPower"") AS DECIMAL(14,4)) ""AVGMethanolpPower"",
	                                    CAST(t.""AVGEthanol"" / DECODE(t.""AVGPower"",0,1000000000,t.""AVGPower"") AS DECIMAL(14,4)) ""AVGEthanolpPower""
                                    FROM
	                                    (
	                                    SELECT
		                                    CAST(MAX(t0.""TotalDistanceGrd"") - MIN(t0.""TotalDistanceGrd"") AS DECIMAL(14,4)) ""DistanceGrd"",
		                                    CAST(AVG(t0.""GroundSpeed"") AS DECIMAL(14,4)) ""AVGSpeedGrd"",
		                                    CAST(AVG(t0.""WaterSpeed"") AS DECIMAL(14,4)) ""AVGWaterSpeed"",
		                                    CAST(AVG(t1.DGO) AS DECIMAL(14,4)) ""AVGDGO"",
		                                    CAST(AVG(t1.LFO) AS DECIMAL(14,4)) ""AVGLFO"",
		                                    CAST(AVG(t1.HFO) AS DECIMAL(14,4)) ""AVGHFO"",
		                                    CAST(AVG(t1.LPG_P) AS DECIMAL(14,4)) ""AVGLPG_P"",
		                                    CAST(AVG(t1.LPG_B) AS DECIMAL(14,4)) ""AVGLPG_B"",
		                                    CAST(AVG(t1.LNG) AS DECIMAL(14,4)) ""AVGLNG"",
		                                    CAST(AVG(t1.""Methanol"") AS DECIMAL(14,4)) ""AVGMethanol"",
		                                    CAST(AVG(t1.""Ethanol"") AS DECIMAL(14,4)) ""AVGEthanol"",
		                                    CAST(AVG(t1.""Power"") AS DECIMAL(14,4)) ""AVGPower"",
		                                    CAST(COUNT(1) / 360 AS DECIMAL(14,4)) ""RunTime"",
		                                    CAST(MAX(t1.DGO) - MIN(t1.DGO) AS DECIMAL(14,4)) ""DGOACC"",
		                                    CAST(MAX(t1.LFO) - MIN(t1.LFO) AS DECIMAL(14,4)) ""LFOACC"",
		                                    CAST(MAX(t1.HFO) - MIN(t1.HFO) AS DECIMAL(14,4)) ""HFOACC"",
		                                    CAST(MAX(t1.LPG_P) - MIN(t1.LPG_P) AS DECIMAL(14,4)) ""LPG_PACC"",
		                                    CAST(MAX(t1.LPG_B) - MIN(t1.LPG_B) AS DECIMAL(14,4)) ""LPG_BACC"",
		                                    CAST(MAX(t1.LNG) - MIN(t1.LNG) AS DECIMAL(14,4)) ""LNGACC"",
		                                    CAST(MAX(t1.""Methanol"") - MIN(t1.""Methanol"") AS DECIMAL(14,4)) ""MethanolACC"",
		                                    CAST(MAX(t1.""Ethanol"") - MIN(t1.""Ethanol"") AS DECIMAL(14,4)) ""EthanolACC""
	                                    FROM
		                                    ""vesselinfo"" t0,
		                                    ""energy_totalindicator"" t1
	                                    WHERE
		                                    NVL(t0.""delete_time"", TO_TIMESTAMP('1949-10-01', 'YYYY-MM-DD')) = TO_TIMESTAMP('1949-10-01', 'YYYY-MM-DD')
		                                    AND t0.""ReceiveDatetime"" = t1.""ReceiveDatetime""
		                                    AND t0.SN = t1.""Number""
		                                    AND t0.SN = '{0}'
		                                    AND t1.""ReceiveDatetime"" >= TO_TIMESTAMP('{1}', 'YYYY-MM-DD HH24:MI:SS')
		                                    AND t1.""ReceiveDatetime"" < TO_TIMESTAMP('{2}', 'YYYY-MM-DD HH24:MI:SS')
	                                        AND t0.""GroundSpeed"" > 0
	                                        GROUP BY t0.""SN""
	                                    ) t", number, dateFrom.ToString("yyyy-MM-dd 00:00:00"), dateTo.ToString("yyyy-MM-dd 00:00:00"));
                    var baseData = await _sqlRepository.ExecuteDataTable(sbSql.ToString());

                    sbSql.Clear();

                    //燃料消耗率曲线
                    sbSql.AppendFormat(@"SELECT
	                                        t.*,
	                                        CAST((t.HFO + t.""Methanol"") / DECODE(t.""GroundSpeed"",0,1000000000,t.""GroundSpeed"") AS DECIMAL(14,4)) AS ""PerNmGrd"",
	                                        CAST((t.HFO + t.""Methanol"") / DECODE(t.""WaterSpeed"",0,1000000000,t.""WaterSpeed"") AS DECIMAL(14,4)) AS ""PerNmWat"",
	                                        CAST((t.HFO + t.""Methanol"") / DECODE(t.""Power"",0,1000000000,t.""Power"") AS DECIMAL(14,4)) AS ""PerPower""
                                        FROM
	                                        (
	                                        SELECT
		                                        t0.""ReceiveDatetime"",
		                                        t0.""GroundSpeed"",
		                                        t0.""WaterSpeed"",
		                                        t1.DGO,
		                                        t1.LFO,
		                                        t1.HFO,
		                                        t1.LPG_P,
		                                        t1.LPG_B,
		                                        t1.LNG,
		                                        t1.""Methanol"",
		                                        t1.""Ethanol"",
		                                        t1.""Power""
	                                        FROM
		                                        ""vesselinfo"" t0,
		                                        ""energy_totalindicator"" t1
	                                        WHERE
		                                        NVL(t0.""delete_time"", TO_TIMESTAMP('1949-10-01', 'YYYY-MM-DD')) = TO_TIMESTAMP('1949-10-01', 'YYYY-MM-DD')
		                                        AND t0.SN = t1.""Number""
		                                        AND t0.""ReceiveDatetime"" = t1.""ReceiveDatetime""
		                                        AND t0.""GroundSpeed"" > 0
		                                        AND t0.""WaterSpeed"" > 0
		                                        AND t1.""Number"" = '{0}'
		                                        AND t1.""ReceiveDatetime"" >= TO_TIMESTAMP('{1}', 'YYYY-MM-DD HH24:MI:SS')
		                                        AND t1.""ReceiveDatetime"" < TO_TIMESTAMP('{2}', 'YYYY-MM-DD HH24:MI:SS')) t
                                        ORDER BY
	                                        ""ReceiveDatetime""", number, dateFrom.ToString("yyyy-MM-dd 00:00:00"), dateTo.ToString("yyyy-MM-dd 00:00:00"));
                    var eeData = await _sqlRepository.ExecuteDataTable(sbSql.ToString());

                    sbSql.Clear();

                    //获取与预测油耗偏差较大时间点航行状况信息
                    sbSql.AppendFormat(@"SELECT
	                                    t.*
                                    FROM
	                                    (
	                                    SELECT
		                                    t0.""ReceiveDatetime"",
		                                    CAST(t0.""Longitude"" AS DECIMAL(14,4)) ""Longitude"",
		                                    CAST(t0.""Latitude"" AS DECIMAL(14,4)) ""Latitude"",
		                                    CAST(t0.""Course"" AS DECIMAL(14,4)) ""Course"",
		                                    CAST(t0.""WaterSpeed"" AS DECIMAL(14,4)) ""WaterSpeed"",
		                                    CAST(t0.""GroundSpeed"" AS DECIMAL(14,4)) ""GroundSpeed"",
		                                    CAST(t0.""WindSpeed"" AS DECIMAL(14,4)) ""WindSpeed"",
		                                    CAST(t0.""WindDirection"" AS DECIMAL(14,4)) ""WindDirection"",
		                                    CAST(t0.""Trim"" AS DECIMAL(14,4)) ""Trim"",
		                                    CAST(t0.""Heel"" AS DECIMAL(14,4)) ""Heel"",
		                                    CAST(t0.""Draft"" AS DECIMAL(14,4)) ""Draft"",
		                                    CAST(t1.""Power"" AS DECIMAL(14,4)) ""Power"",
		                                    CAST(t1.DGO AS DECIMAL(14,4)) ""DGO"",
		                                    CAST(t1.LFO AS DECIMAL(14,4)) ""LFO"",
		                                    CAST(t1.HFO AS DECIMAL(14,4)) ""HFO"",
		                                    CAST(t1.LPG_P AS DECIMAL(14,4)) ""LPG_P"",
		                                    CAST(t1.LPG_B AS DECIMAL(14,4)) ""LPG_B"",
		                                    CAST(t1.LNG AS DECIMAL(14,4)) ""LNG"",
		                                    CAST(t1.""Methanol"" AS DECIMAL(14,4)) ""Methanol"",
		                                    CAST(t1.""Ethanol"" AS DECIMAL(14,4)) ""Ethanol""
	                                    FROM
		                                    ""vesselinfo"" t0,
		                                    ""energy_totalindicator"" t1,
		                                    ""energy_prediction"" t2
	                                    WHERE
		                                    NVL(t0.""delete_time"", TO_TIMESTAMP('1949-10-01', 'YYYY-MM-DD')) = TO_TIMESTAMP('1949-10-01', 'YYYY-MM-DD')
		                                    AND t0.SN = t1.""Number""
		                                    AND t0.""ReceiveDatetime"" = t1.""ReceiveDatetime""
		                                    AND t0.SN = t2.""Number""
		                                    AND t0.""ReceiveDatetime"" = t2.""ReceiveDatetime""
		                                    AND t0.""GroundSpeed"" > 0
		                                    AND t1.""Number"" = '{0}'
		                                    AND t1.""ReceiveDatetime"" >= TO_TIMESTAMP('{1}', 'YYYY-MM-DD HH24:MI:SS')
		                                    AND t1.""ReceiveDatetime"" < TO_TIMESTAMP('{2}', 'YYYY-MM-DD HH24:MI:SS')
		                                    AND ABS(t2.DGO - t1.DGO) / DECODE(t2.DGO, 0, 1) > 0.8 UNION
	                                    SELECT
		                                    t0.""ReceiveDatetime"",
		                                    CAST(t0.""Longitude"" AS DECIMAL(14,4)) ""Longitude"",
		                                    CAST(t0.""Latitude"" AS DECIMAL(14,4)) ""Latitude"",
		                                    CAST(t0.""Course"" AS DECIMAL(14,4)) ""Course"",
		                                    CAST(t0.""WaterSpeed"" AS DECIMAL(14,4)) ""WaterSpeed"",
		                                    CAST(t0.""GroundSpeed"" AS DECIMAL(14,4)) ""GroundSpeed"",
		                                    CAST(t0.""WindSpeed"" AS DECIMAL(14,4)) ""WindSpeed"",
		                                    CAST(t0.""WindDirection"" AS DECIMAL(14,4)) ""WindDirection"",
		                                    CAST(t0.""Trim"" AS DECIMAL(14,4)) ""Trim"",
		                                    CAST(t0.""Heel"" AS DECIMAL(14,4)) ""Heel"",
		                                    CAST(t0.""Draft"" AS DECIMAL(14,4)) ""Draft"",
		                                    CAST(t1.""Power"" AS DECIMAL(14,4)) ""Power"",
		                                    CAST(t1.DGO AS DECIMAL(14,4)) ""DGO"",
		                                    CAST(t1.LFO AS DECIMAL(14,4)) ""LFO"",
		                                    CAST(t1.HFO AS DECIMAL(14,4)) ""HFO"",
		                                    CAST(t1.LPG_P AS DECIMAL(14,4)) ""LPG_P"",
		                                    CAST(t1.LPG_B AS DECIMAL(14,4)) ""LPG_B"",
		                                    CAST(t1.LNG AS DECIMAL(14,4)) ""LNG"",
		                                    CAST(t1.""Methanol"" AS DECIMAL(14,4)) ""Methanol"",
		                                    CAST(t1.""Ethanol"" AS DECIMAL(14,4)) ""Ethanol""
	                                    FROM
		                                    ""vesselinfo"" t0,
		                                    ""energy_totalindicator"" t1,
		                                    ""energy_prediction"" t2
	                                    WHERE
		                                    NVL(t0.""delete_time"", TO_TIMESTAMP('1949-10-01', 'YYYY-MM-DD')) = TO_TIMESTAMP('1949-10-01', 'YYYY-MM-DD')
		                                    AND t0.SN = t1.""Number""
		                                    AND t0.""ReceiveDatetime"" = t1.""ReceiveDatetime""
		                                    AND t0.SN = t2.""Number""
		                                    AND t0.""ReceiveDatetime"" = t2.""ReceiveDatetime""
		                                    AND t0.""GroundSpeed"" > 0
		                                    AND t1.""Number"" = '{0}'
		                                    AND t1.""ReceiveDatetime"" >= TO_TIMESTAMP('{1}', 'YYYY-MM-DD HH24:MI:SS')
		                                    AND t1.""ReceiveDatetime"" < TO_TIMESTAMP('{2}', 'YYYY-MM-DD HH24:MI:SS')
		                                    AND ABS(t2.LFO - t1.LFO) / DECODE(t2.LFO, 0, 1) > 0.8 UNION
	                                    SELECT
		                                    t0.""ReceiveDatetime"",
		                                    CAST(t0.""Longitude"" AS DECIMAL(14,4)) ""Longitude"",
		                                    CAST(t0.""Latitude"" AS DECIMAL(14,4)) ""Latitude"",
		                                    CAST(t0.""Course"" AS DECIMAL(14,4)) ""Course"",
		                                    CAST(t0.""WaterSpeed"" AS DECIMAL(14,4)) ""WaterSpeed"",
		                                    CAST(t0.""GroundSpeed"" AS DECIMAL(14,4)) ""GroundSpeed"",
		                                    CAST(t0.""WindSpeed"" AS DECIMAL(14,4)) ""WindSpeed"",
		                                    CAST(t0.""WindDirection"" AS DECIMAL(14,4)) ""WindDirection"",
		                                    CAST(t0.""Trim"" AS DECIMAL(14,4)) ""Trim"",
		                                    CAST(t0.""Heel"" AS DECIMAL(14,4)) ""Heel"",
		                                    CAST(t0.""Draft"" AS DECIMAL(14,4)) ""Draft"",
		                                    CAST(t1.""Power"" AS DECIMAL(14,4)) ""Power"",
		                                    CAST(t1.DGO AS DECIMAL(14,4)) ""DGO"",
		                                    CAST(t1.LFO AS DECIMAL(14,4)) ""LFO"",
		                                    CAST(t1.HFO AS DECIMAL(14,4)) ""HFO"",
		                                    CAST(t1.LPG_P AS DECIMAL(14,4)) ""LPG_P"",
		                                    CAST(t1.LPG_B AS DECIMAL(14,4)) ""LPG_B"",
		                                    CAST(t1.LNG AS DECIMAL(14,4)) ""LNG"",
		                                    CAST(t1.""Methanol"" AS DECIMAL(14,4)) ""Methanol"",
		                                    CAST(t1.""Ethanol"" AS DECIMAL(14,4)) ""Ethanol""
	                                    FROM
		                                    ""vesselinfo"" t0,
		                                    ""energy_totalindicator"" t1,
		                                    ""energy_prediction"" t2
	                                    WHERE
		                                    NVL(t0.""delete_time"", TO_TIMESTAMP('1949-10-01', 'YYYY-MM-DD')) = TO_TIMESTAMP('1949-10-01', 'YYYY-MM-DD')
		                                    AND t0.SN = t1.""Number""
		                                    AND t0.""ReceiveDatetime"" = t1.""ReceiveDatetime""
		                                    AND t0.SN = t2.""Number""
		                                    AND t0.""ReceiveDatetime"" = t2.""ReceiveDatetime""
		                                    AND t0.""GroundSpeed"" > 0
		                                    AND t1.""Number"" = '{0}'
		                                    AND t1.""ReceiveDatetime"" >= TO_TIMESTAMP('{1}', 'YYYY-MM-DD HH24:MI:SS')
		                                    AND t1.""ReceiveDatetime"" < TO_TIMESTAMP('{2}', 'YYYY-MM-DD HH24:MI:SS')
		                                    AND ABS(t2.HFO - t1.HFO) / DECODE(t2.HFO, 0, 1) > 0.8 UNION
	                                    SELECT
		                                    t0.""ReceiveDatetime"",
		                                    CAST(t0.""Longitude"" AS DECIMAL(14,4)) ""Longitude"",
		                                    CAST(t0.""Latitude"" AS DECIMAL(14,4)) ""Latitude"",
		                                    CAST(t0.""Course"" AS DECIMAL(14,4)) ""Course"",
		                                    CAST(t0.""WaterSpeed"" AS DECIMAL(14,4)) ""WaterSpeed"",
		                                    CAST(t0.""GroundSpeed"" AS DECIMAL(14,4)) ""GroundSpeed"",
		                                    CAST(t0.""WindSpeed"" AS DECIMAL(14,4)) ""WindSpeed"",
		                                    CAST(t0.""WindDirection"" AS DECIMAL(14,4)) ""WindDirection"",
		                                    CAST(t0.""Trim"" AS DECIMAL(14,4)) ""Trim"",
		                                    CAST(t0.""Heel"" AS DECIMAL(14,4)) ""Heel"",
		                                    CAST(t0.""Draft"" AS DECIMAL(14,4)) ""Draft"",
		                                    CAST(t1.""Power"" AS DECIMAL(14,4)) ""Power"",
		                                    CAST(t1.DGO AS DECIMAL(14,4)) ""DGO"",
		                                    CAST(t1.LFO AS DECIMAL(14,4)) ""LFO"",
		                                    CAST(t1.HFO AS DECIMAL(14,4)) ""HFO"",
		                                    CAST(t1.LPG_P AS DECIMAL(14,4)) ""LPG_P"",
		                                    CAST(t1.LPG_B AS DECIMAL(14,4)) ""LPG_B"",
		                                    CAST(t1.LNG AS DECIMAL(14,4)) ""LNG"",
		                                    CAST(t1.""Methanol"" AS DECIMAL(14,4)) ""Methanol"",
		                                    CAST(t1.""Ethanol"" AS DECIMAL(14,4)) ""Ethanol""
	                                    FROM
		                                    ""vesselinfo"" t0,
		                                    ""energy_totalindicator"" t1,
		                                    ""energy_prediction"" t2
	                                    WHERE
		                                    NVL(t0.""delete_time"", TO_TIMESTAMP('1949-10-01', 'YYYY-MM-DD')) = TO_TIMESTAMP('1949-10-01', 'YYYY-MM-DD')
		                                    AND t0.SN = t1.""Number""
		                                    AND t0.""ReceiveDatetime"" = t1.""ReceiveDatetime""
		                                    AND t0.SN = t2.""Number""
		                                    AND t0.""ReceiveDatetime"" = t2.""ReceiveDatetime""
		                                    AND t0.""GroundSpeed"" > 0
		                                    AND t1.""Number"" = '{0}'
		                                    AND t1.""ReceiveDatetime"" >= TO_TIMESTAMP('{1}', 'YYYY-MM-DD HH24:MI:SS')
		                                    AND t1.""ReceiveDatetime"" < TO_TIMESTAMP('{2}', 'YYYY-MM-DD HH24:MI:SS')
		                                    AND ABS(t2.LPG_P - t1.LPG_P) / DECODE(t2.LPG_P, 0, 1) > 0.8 UNION
	                                    SELECT
		                                    t0.""ReceiveDatetime"",
		                                    CAST(t0.""Longitude"" AS DECIMAL(14,4)) ""Longitude"",
		                                    CAST(t0.""Latitude"" AS DECIMAL(14,4)) ""Latitude"",
		                                    CAST(t0.""Course"" AS DECIMAL(14,4)) ""Course"",
		                                    CAST(t0.""WaterSpeed"" AS DECIMAL(14,4)) ""WaterSpeed"",
		                                    CAST(t0.""GroundSpeed"" AS DECIMAL(14,4)) ""GroundSpeed"",
		                                    CAST(t0.""WindSpeed"" AS DECIMAL(14,4)) ""WindSpeed"",
		                                    CAST(t0.""WindDirection"" AS DECIMAL(14,4)) ""WindDirection"",
		                                    CAST(t0.""Trim"" AS DECIMAL(14,4)) ""Trim"",
		                                    CAST(t0.""Heel"" AS DECIMAL(14,4)) ""Heel"",
		                                    CAST(t0.""Draft"" AS DECIMAL(14,4)) ""Draft"",
		                                    CAST(t1.""Power"" AS DECIMAL(14,4)) ""Power"",
		                                    CAST(t1.DGO AS DECIMAL(14,4)) ""DGO"",
		                                    CAST(t1.LFO AS DECIMAL(14,4)) ""LFO"",
		                                    CAST(t1.HFO AS DECIMAL(14,4)) ""HFO"",
		                                    CAST(t1.LPG_P AS DECIMAL(14,4)) ""LPG_P"",
		                                    CAST(t1.LPG_B AS DECIMAL(14,4)) ""LPG_B"",
		                                    CAST(t1.LNG AS DECIMAL(14,4)) ""LNG"",
		                                    CAST(t1.""Methanol"" AS DECIMAL(14,4)) ""Methanol"",
		                                    CAST(t1.""Ethanol"" AS DECIMAL(14,4)) ""Ethanol""
	                                    FROM
		                                    ""vesselinfo"" t0,
		                                    ""energy_totalindicator"" t1,
		                                    ""energy_prediction"" t2
	                                    WHERE
		                                    NVL(t0.""delete_time"", TO_TIMESTAMP('1949-10-01', 'YYYY-MM-DD')) = TO_TIMESTAMP('1949-10-01', 'YYYY-MM-DD')
		                                    AND t0.SN = t1.""Number""
		                                    AND t0.""ReceiveDatetime"" = t1.""ReceiveDatetime""
		                                    AND t0.SN = t2.""Number""
		                                    AND t0.""ReceiveDatetime"" = t2.""ReceiveDatetime""
		                                    AND t0.""GroundSpeed"" > 0
		                                    AND t1.""Number"" = '{0}'
		                                    AND t1.""ReceiveDatetime"" >= TO_TIMESTAMP('{1}', 'YYYY-MM-DD HH24:MI:SS')
		                                    AND t1.""ReceiveDatetime"" < TO_TIMESTAMP('{2}', 'YYYY-MM-DD HH24:MI:SS')
		                                    AND ABS(t2.LPG_B - t1.LPG_B) / DECODE(t2.LPG_B, 0, 1) > 0.8 UNION
	                                    SELECT
		                                    t0.""ReceiveDatetime"",
		                                    CAST(t0.""Longitude"" AS DECIMAL(14,4)) ""Longitude"",
		                                    CAST(t0.""Latitude"" AS DECIMAL(14,4)) ""Latitude"",
		                                    CAST(t0.""Course"" AS DECIMAL(14,4)) ""Course"",
		                                    CAST(t0.""WaterSpeed"" AS DECIMAL(14,4)) ""WaterSpeed"",
		                                    CAST(t0.""GroundSpeed"" AS DECIMAL(14,4)) ""GroundSpeed"",
		                                    CAST(t0.""WindSpeed"" AS DECIMAL(14,4)) ""WindSpeed"",
		                                    CAST(t0.""WindDirection"" AS DECIMAL(14,4)) ""WindDirection"",
		                                    CAST(t0.""Trim"" AS DECIMAL(14,4)) ""Trim"",
		                                    CAST(t0.""Heel"" AS DECIMAL(14,4)) ""Heel"",
		                                    CAST(t0.""Draft"" AS DECIMAL(14,4)) ""Draft"",
		                                    CAST(t1.""Power"" AS DECIMAL(14,4)) ""Power"",
		                                    CAST(t1.DGO AS DECIMAL(14,4)) ""DGO"",
		                                    CAST(t1.LFO AS DECIMAL(14,4)) ""LFO"",
		                                    CAST(t1.HFO AS DECIMAL(14,4)) ""HFO"",
		                                    CAST(t1.LPG_P AS DECIMAL(14,4)) ""LPG_P"",
		                                    CAST(t1.LPG_B AS DECIMAL(14,4)) ""LPG_B"",
		                                    CAST(t1.LNG AS DECIMAL(14,4)) ""LNG"",
		                                    CAST(t1.""Methanol"" AS DECIMAL(14,4)) ""Methanol"",
		                                    CAST(t1.""Ethanol"" AS DECIMAL(14,4)) ""Ethanol""
	                                    FROM
		                                    ""vesselinfo"" t0,
		                                    ""energy_totalindicator"" t1,
		                                    ""energy_prediction"" t2
	                                    WHERE
		                                    NVL(t0.""delete_time"", TO_TIMESTAMP('1949-10-01', 'YYYY-MM-DD')) = TO_TIMESTAMP('1949-10-01', 'YYYY-MM-DD')
		                                    AND t0.SN = t1.""Number""
		                                    AND t0.""ReceiveDatetime"" = t1.""ReceiveDatetime""
		                                    AND t0.SN = t2.""Number""
		                                    AND t0.""ReceiveDatetime"" = t2.""ReceiveDatetime""
		                                    AND t0.""GroundSpeed"" > 0
		                                    AND t1.""Number"" = '{0}'
		                                    AND t1.""ReceiveDatetime"" >= TO_TIMESTAMP('{1}', 'YYYY-MM-DD HH24:MI:SS')
		                                    AND t1.""ReceiveDatetime"" < TO_TIMESTAMP('{2}', 'YYYY-MM-DD HH24:MI:SS')
		                                    AND ABS(t2.LNG - t1.LNG) / DECODE(t2.LNG, 0, 1) > 0.8 UNION
	                                    SELECT
		                                    t0.""ReceiveDatetime"",
		                                    CAST(t0.""Longitude"" AS DECIMAL(14,4)) ""Longitude"",
		                                    CAST(t0.""Latitude"" AS DECIMAL(14,4)) ""Latitude"",
		                                    CAST(t0.""Course"" AS DECIMAL(14,4)) ""Course"",
		                                    CAST(t0.""WaterSpeed"" AS DECIMAL(14,4)) ""WaterSpeed"",
		                                    CAST(t0.""GroundSpeed"" AS DECIMAL(14,4)) ""GroundSpeed"",
		                                    CAST(t0.""WindSpeed"" AS DECIMAL(14,4)) ""WindSpeed"",
		                                    CAST(t0.""WindDirection"" AS DECIMAL(14,4)) ""WindDirection"",
		                                    CAST(t0.""Trim"" AS DECIMAL(14,4)) ""Trim"",
		                                    CAST(t0.""Heel"" AS DECIMAL(14,4)) ""Heel"",
		                                    CAST(t0.""Draft"" AS DECIMAL(14,4)) ""Draft"",
		                                    CAST(t1.""Power"" AS DECIMAL(14,4)) ""Power"",
		                                    CAST(t1.DGO AS DECIMAL(14,4)) ""DGO"",
		                                    CAST(t1.LFO AS DECIMAL(14,4)) ""LFO"",
		                                    CAST(t1.HFO AS DECIMAL(14,4)) ""HFO"",
		                                    CAST(t1.LPG_P AS DECIMAL(14,4)) ""LPG_P"",
		                                    CAST(t1.LPG_B AS DECIMAL(14,4)) ""LPG_B"",
		                                    CAST(t1.LNG AS DECIMAL(14,4)) ""LNG"",
		                                    CAST(t1.""Methanol"" AS DECIMAL(14,4)) ""Methanol"",
		                                    CAST(t1.""Ethanol"" AS DECIMAL(14,4)) ""Ethanol""
	                                    FROM
		                                    ""vesselinfo"" t0,
		                                    ""energy_totalindicator"" t1,
		                                    ""energy_prediction"" t2
	                                    WHERE
		                                    NVL(t0.""delete_time"", TO_TIMESTAMP('1949-10-01', 'YYYY-MM-DD')) = TO_TIMESTAMP('1949-10-01', 'YYYY-MM-DD')
		                                    AND t0.SN = t1.""Number""
		                                    AND t0.""ReceiveDatetime"" = t1.""ReceiveDatetime""
		                                    AND t0.SN = t2.""Number""
		                                    AND t0.""ReceiveDatetime"" = t2.""ReceiveDatetime""
		                                    AND t0.""GroundSpeed"" > 0
		                                    AND t1.""Number"" = '{0}'
		                                    AND t1.""ReceiveDatetime"" >= TO_TIMESTAMP('{1}', 'YYYY-MM-DD HH24:MI:SS')
		                                    AND t1.""ReceiveDatetime"" < TO_TIMESTAMP('{2}', 'YYYY-MM-DD HH24:MI:SS')
		                                    AND ABS(t2.""Methanol"" - t1.""Methanol"") / DECODE(t2.""Methanol"", 0, 1) > 0.8 UNION
	                                    SELECT
		                                    t0.""ReceiveDatetime"",
		                                    CAST(t0.""Longitude"" AS DECIMAL(14,4)) ""Longitude"",
		                                    CAST(t0.""Latitude"" AS DECIMAL(14,4)) ""Latitude"",
		                                    CAST(t0.""Course"" AS DECIMAL(14,4)) ""Course"",
		                                    CAST(t0.""WaterSpeed"" AS DECIMAL(14,4)) ""WaterSpeed"",
		                                    CAST(t0.""GroundSpeed"" AS DECIMAL(14,4)) ""GroundSpeed"",
		                                    CAST(t0.""WindSpeed"" AS DECIMAL(14,4)) ""WindSpeed"",
		                                    CAST(t0.""WindDirection"" AS DECIMAL(14,4)) ""WindDirection"",
		                                    CAST(t0.""Trim"" AS DECIMAL(14,4)) ""Trim"",
		                                    CAST(t0.""Heel"" AS DECIMAL(14,4)) ""Heel"",
		                                    CAST(t0.""Draft"" AS DECIMAL(14,4)) ""Draft"",
		                                    CAST(t1.""Power"" AS DECIMAL(14,4)) ""Power"",
		                                    CAST(t1.DGO AS DECIMAL(14,4)) ""DGO"",
		                                    CAST(t1.LFO AS DECIMAL(14,4)) ""LFO"",
		                                    CAST(t1.HFO AS DECIMAL(14,4)) ""HFO"",
		                                    CAST(t1.LPG_P AS DECIMAL(14,4)) ""LPG_P"",
		                                    CAST(t1.LPG_B AS DECIMAL(14,4)) ""LPG_B"",
		                                    CAST(t1.LNG AS DECIMAL(14,4)) ""LNG"",
		                                    CAST(t1.""Methanol"" AS DECIMAL(14,4)) ""Methanol"",
		                                    CAST(t1.""Ethanol"" AS DECIMAL(14,4)) ""Ethanol""
	                                    FROM
		                                    ""vesselinfo"" t0,
		                                    ""energy_totalindicator"" t1,
		                                    ""energy_prediction"" t2
	                                    WHERE
		                                    NVL(t0.""delete_time"", TO_TIMESTAMP('1949-10-01', 'YYYY-MM-DD')) = TO_TIMESTAMP('1949-10-01', 'YYYY-MM-DD')
		                                    AND t0.SN = t1.""Number""
		                                    AND t0.""ReceiveDatetime"" = t1.""ReceiveDatetime""
		                                    AND t0.SN = t2.""Number""
		                                    AND t0.""ReceiveDatetime"" = t2.""ReceiveDatetime""
		                                    AND t0.""GroundSpeed"" > 0
		                                    AND t1.""Number"" = '{0}'
		                                    AND t1.""ReceiveDatetime"" >= TO_TIMESTAMP('{1}', 'YYYY-MM-DD HH24:MI:SS')
		                                    AND t1.""ReceiveDatetime"" < TO_TIMESTAMP('{2}', 'YYYY-MM-DD HH24:MI:SS')
		                                    AND ABS(t2.""Ethanol"" - t1.""Ethanol"") / DECODE(t2.""Ethanol"", 0, 1) > 0.8
	                                    ) t
                                    ORDER BY
	                                    t.""ReceiveDatetime""", number, dateFrom.ToString("yyyy-MM-dd 00:00:00"), dateTo.ToString("yyyy-MM-dd 00:00:00"));
                    var abnormalData = await _sqlRepository.ExecuteDataTable(sbSql.ToString());

                    sbSql.Clear();

                    //获取每天CII计算指标
                    sbSql.AppendFormat(@"SELECT
	                                    TRUNC(t0.""ReceiveDatetime"", 'DD') ""ReceiveDate"",
	                                    MAX(t0.""TotalDistanceGrd"" + NVL(t0.""ResetDistanceGrd"", 0)) ""DistanceGrd1"",
	                                    MIN(t0.""TotalDistanceGrd"" + NVL(t0.""ResetDistanceGrd"", 0)) ""DistanceGrd2"",
	                                    max(t1.""DGOAccumulated"") ""DGOAcc1"",
	                                    min(t1.""DGOAccumulated"") ""DGOAcc2"",
	                                    max(t1.""EthanolAccumulated"") ""EthanolAcc1"",
	                                    min(t1.""EthanolAccumulated"") ""EthanolAcc2"",
	                                    max(t1.""HFOAccumulated"") ""HFOAcc1"",
	                                    min(t1.""HFOAccumulated"") ""HFOAcc2"",
	                                    max(t1.""LFOAccumulated"") ""LFOAcc1"",
	                                    min(t1.""LFOAccumulated"") ""LFOAcc2"",
	                                    max(t1.""LNGAccumulated"") ""LNGAcc1"",
	                                    min(t1.""LNGAccumulated"") ""LNGAcc2"",
	                                    max(t1.""LPG_BAccumulated"") ""LPG_BAcc1"",
	                                    min(t1.""LPG_BAccumulated"") ""LPG_BAcc2"",
	                                    max(t1.""LPG_PAccumulated"") ""LPG_PAcc1"",
	                                    min(t1.""LPG_PAccumulated"") ""LPG_PAcc2"",
	                                    max(t1.""MethanolAccumulated"") ""MethanolAcc1"",
	                                    min(t1.""MethanolAccumulated"") ""MethanolAcc2""
                                    FROM
	                                    ""vesselinfo"" t0,
	                                    ""energy_totalindicator"" t1
                                    WHERE
	                                    NVL(t0.""delete_time"", TO_TIMESTAMP('1949-10-01', 'YYYY-MM-DD')) = TO_TIMESTAMP('1949-10-01', 'YYYY-MM-DD')
	                                    AND t0.SN = t1.""Number""
	                                    AND t0.""ReceiveDatetime"" = t1.""ReceiveDatetime""
	                                    AND t0.SN = '{0}'
	                                    AND t0.""ReceiveDatetime"" >= TO_TIMESTAMP('{1}', 'YYYY-MM-DD HH24:MI:SS')
	                                    AND t0.""ReceiveDatetime"" < TO_TIMESTAMP('{2}', 'YYYY-MM-DD HH24:MI:SS')
                                    GROUP BY
	                                    TRUNC(t0.""ReceiveDatetime"", 'DD')
                                    ORDER BY
	                                    TRUNC(t0.""ReceiveDatetime"", 'DD')", number, dateFrom.ToString("yyyy-MM-dd 00:00:00"), dateTo.ToString("yyyy-MM-dd 00:00:00"));
                    var ciiData = await _sqlRepository.ExecuteDataTable(sbSql.ToString());

                    sbSql.Clear();

                    //预测油耗平均值
                    sbSql.AppendFormat(@"SELECT
		                                    CAST(AVG(t2.DGO) AS DECIMAL(10,4)) AS DGO,
		                                    CAST(AVG(t2.LFO) AS DECIMAL(10,4)) AS LFO,
		                                    CAST(AVG(t2.HFO) AS DECIMAL(10,4)) AS HFO,
		                                    CAST(AVG(t2.LPG_P) AS DECIMAL(10,4)) AS LPG_P,
		                                    CAST(AVG(t2.LPG_B) AS DECIMAL(10,4)) AS LPG_B,
		                                    CAST(AVG(t2.LNG) AS DECIMAL(10,4)) AS LNG,
		                                    CAST(AVG(t2.""Methanol"") AS DECIMAL(10,4)) AS ""Methanol"",
		                                    CAST(AVG(t2.""Ethanol"") AS DECIMAL(10,4)) AS ""Ethanol""
	                                    FROM
		                                    ""vesselinfo"" t0,
		                                    ""energy_prediction"" t2
	                                    WHERE
		                                    NVL(t0.""delete_time"", TO_TIMESTAMP('1949-10-01', 'YYYY-MM-DD')) = TO_TIMESTAMP('1949-10-01', 'YYYY-MM-DD')
		                                    AND t0.SN = t2.""Number""
		                                    AND t0.""ReceiveDatetime"" = t2.""ReceiveDatetime""
		                                    AND t0.""GroundSpeed"" > 0
		                                    AND t2.""Number"" = '{0}'
		                                    AND t2.""ReceiveDatetime"" >= TO_TIMESTAMP('{1}', 'YYYY-MM-DD HH24:MI:SS')
		                                    AND t2.""ReceiveDatetime"" < TO_TIMESTAMP('{2}', 'YYYY-MM-DD HH24:MI:SS')", number, dateFrom.ToString("yyyy-MM-dd 00:00:00"), dateTo.ToString("yyyy-MM-dd 00:00:00"));
                    var preData = await _sqlRepository.ExecuteDataTable(sbSql.ToString());

                    //时间段内燃料消耗量
                    var fms1 = new List<FuelConsumption>();
                    foreach (DataRow dr in firstFlowmeters.Rows)
                    {
                        fms1.Add(new FuelConsumption
                        {
                            DeviceType = dr["DeviceNo"].ToString(),
                            FuelType = dr["FuelType"].ToString(),
                            Cons = Convert.ToDecimal(dr["ConsAcc"] is DBNull ? 0 : dr["ConsAcc"]) - Convert.ToDecimal(dr["ConsAct"] is DBNull ? 0 : dr["ConsAct"]),
                            DeviceNo = dr["DeviceNo"].ToString()
                        });
                    }
                    foreach (DataRow dr in lastFlowmeters.Rows)
                    {
                        if (fms1.Any(t => t.DeviceNo == dr["DeviceNo"].ToString() && t.FuelType == dr["FuelType"].ToString()))
                        {
                            var tempDto = fms1.FirstOrDefault(t => t.DeviceNo == dr["DeviceNo"].ToString() && t.FuelType == dr["FuelType"].ToString());
                            tempDto.Cons = Convert.ToDecimal(dr["ConsAcc"] is DBNull ? 0 : dr["ConsAcc"]) - tempDto.Cons;
                        }
                        else
                        {
                            fms1.Add(new FuelConsumption
                            {
                                DeviceType = dr["DeviceNo"].ToString(),
                                FuelType = dr["FuelType"].ToString(),
                                Cons = Convert.ToDecimal(dr["ConsAcc"] is DBNull ? 0 : dr["ConsAcc"]) - Convert.ToDecimal(dr["ConsAct"] is DBNull ? 0 : dr["ConsAct"]),
                                DeviceNo = dr["DeviceNo"].ToString()
                            });
                        }
                    }
                    fms1 = new List<FuelConsumption>();
                    foreach (DataRow dr in PUs.Rows)
                    {
                        fms1.Add(new FuelConsumption
                        {
                            DeviceType = dr["DeviceType"].ToString(),
                            FuelType = "HFO",
                            Cons = Convert.ToDecimal(dr["HFO"] is DBNull ? 0 : dr["HFO"]),
                            DeviceNo = dr["DeviceType"].ToString() == "me" ? (lang == "en_US" ? "Main Engine" : "主机") : dr["DeviceType"].ToString() == "blr" ? (lang == "en_US" ? "Main Engine" : "Boiler") : (lang == "en_US" ? "Main Engine" : "Auxiliary Engine")
                        });
                        fms1.Add(new FuelConsumption
                        {
                            DeviceType = dr["DeviceType"].ToString(),
                            FuelType = "Methanol",
                            Cons = Convert.ToDecimal(dr["Methanol"] is DBNull ? 0 : dr["Methanol"]),
                            DeviceNo = dr["DeviceType"].ToString() == "me" ? (lang == "en_US" ? "Main Engine" : "主机") : dr["DeviceType"].ToString() == "blr" ? (lang == "en_US" ? "Main Engine" : "Boiler") : (lang == "en_US" ? "Main Engine" : "Auxiliary Engine")
                        });
                    }

                    var ciis = new List<object>();
                    //计算CII
                    if (ciiData.Rows.Count > 0)
                        for (var i = 0; i < ciiData.Rows.Count; i++)
                        {
                            var ciisParams = new
                            {
                                shipType = shipType,
                                year = DateTime.Now.Year,
                                distance = Convert.ToSingle(ciiData.Rows[i]["DistanceGrd1"] is DBNull ? 0 : ciiData.Rows[i]["DistanceGrd1"]) - Convert.ToSingle(ciiData.Rows[i]["DistanceGrd2"] is DBNull ? 0 : ciiData.Rows[i]["DistanceGrd2"]),
                                DGO = Convert.ToSingle(ciiData.Rows[i]["DGOAcc1"] is DBNull ? 0 : ciiData.Rows[i]["DGOAcc1"]) - Convert.ToSingle(ciiData.Rows[i]["DGOAcc2"] is DBNull ? 0 : ciiData.Rows[i]["DGOAcc2"]),
                                LFO = Convert.ToSingle(ciiData.Rows[i]["LFOAcc1"] is DBNull ? 0 : ciiData.Rows[i]["LFOAcc1"]) - Convert.ToSingle(ciiData.Rows[i]["LFOAcc2"] is DBNull ? 0 : ciiData.Rows[i]["LFOAcc2"]),
                                HFO = Convert.ToSingle(ciiData.Rows[i]["HFOAcc1"] is DBNull ? 0 : ciiData.Rows[i]["HFOAcc1"]) - Convert.ToSingle(ciiData.Rows[i]["HFOAcc2"] is DBNull ? 0 : ciiData.Rows[i]["HFOAcc2"]),
                                LPG_P = Convert.ToSingle(ciiData.Rows[i]["LPG_PAcc1"] is DBNull ? 0 : ciiData.Rows[i]["LPG_PAcc1"]) - Convert.ToSingle(ciiData.Rows[i]["LPG_PAcc2"] is DBNull ? 0 : ciiData.Rows[i]["LPG_PAcc2"]),
                                LPG_B = Convert.ToSingle(ciiData.Rows[i]["LPG_BAcc1"] is DBNull ? 0 : ciiData.Rows[i]["LPG_BAcc1"]) - Convert.ToSingle(ciiData.Rows[i]["LPG_BAcc2"] is DBNull ? 0 : ciiData.Rows[i]["LPG_BAcc2"]),
                                LNG = Convert.ToSingle(ciiData.Rows[i]["LNGAcc1"] is DBNull ? 0 : ciiData.Rows[i]["LNGAcc1"]) - Convert.ToSingle(ciiData.Rows[i]["LNGAcc2"] is DBNull ? 0 : ciiData.Rows[i]["LNGAcc2"]),
                                Methanol = Convert.ToSingle(ciiData.Rows[i]["MethanolAcc1"] is DBNull ? 0 : ciiData.Rows[i]["MethanolAcc1"]) - Convert.ToSingle(ciiData.Rows[i]["MethanolAcc2"] is DBNull ? 0 : ciiData.Rows[i]["MethanolAcc2"]),
                                Ethanol = Convert.ToSingle(ciiData.Rows[i]["EthanolAcc1"] is DBNull ? 0 : ciiData.Rows[i]["EthanolAcc1"]) - Convert.ToSingle(ciiData.Rows[i]["EthanolAcc2"] is DBNull ? 0 : ciiData.Rows[i]["EthanolAcc2"]),
                                dwt = dwt,
                                gt = gt
                            };
                            var tempCII = await _ciiService.CalcCII(ciisParams.ToJson());
                            var tempObject = tempCII.ToJson().ToJObject();
                            var jo = new JProperty("ReceiveDatetime", ciiData.Rows[i]["ReceiveDate"]);

                            tempObject.Add(jo);
                            ciis.Add(tempObject);
                        }

                    object cii = new object();
                    if (baseData.Rows.Count > 0)
                    {
                        var ciiParams = new
                        {
                            shipType = shipType,
                            dwt = dwt,
                            gt = gt,
                            year = DateTime.Now.Year,
                            distance = Convert.ToSingle(baseData.Rows[0]["DistanceGrd"] is DBNull ? 0 : baseData.Rows[0]["DistanceGrd"]),
                            DGO = Convert.ToSingle(baseData.Rows[0]["DGOACC"] is DBNull ? 0 : baseData.Rows[0]["DGOACC"]),
                            LFO = Convert.ToSingle(baseData.Rows[0]["LFOACC"] is DBNull ? 0 : baseData.Rows[0]["LFOACC"]),
                            HFO = Convert.ToSingle(baseData.Rows[0]["HFOACC"] is DBNull ? 0 : baseData.Rows[0]["HFOACC"]),
                            LPG_P = Convert.ToSingle(baseData.Rows[0]["LPG_PACC"] is DBNull ? 0 : baseData.Rows[0]["LPG_PACC"]),
                            LPG_B = Convert.ToSingle(baseData.Rows[0]["LPG_BACC"] is DBNull ? 0 : baseData.Rows[0]["LPG_BACC"]),
                            LNG = Convert.ToSingle(baseData.Rows[0]["LNGACC"] is DBNull ? 0 : baseData.Rows[0]["LNGACC"]),
                            Methanol = Convert.ToSingle(baseData.Rows[0]["MethanolACC"] is DBNull ? 0 : baseData.Rows[0]["MethanolACC"]),
                            Ethanol = Convert.ToSingle(baseData.Rows[0]["EthanolACC"] is DBNull ? 0 : baseData.Rows[0]["EthanolACC"])
                        };
                        cii = await _ciiService.CalcCII(ciiParams.ToJson());
                    }
                    else
                    {
                        var ciiParams = new
                        {
                            shipType = shipType,
                            dwt = dwt,
                            gt = gt,
                            year = DateTime.Now.Year,
                            distance = 0.00001,
                            DGO = 0,
                            LFO = 0,
                            HFO = 0,
                            LPG_P = 0,
                            LPG_B = 0,
                            LNG = 0,
                            Methanol = 0,
                            Ethanol = 0
                        };
                        cii = await _ciiService.CalcCII(ciiParams.ToJson());
                    }

                    var ciiPrediction = "--";
                    if (ciis.Count > 1)
                        ciiPrediction = (ciis[ciis.Count - 1].ToJson().ToJObject())["CIIRating"].ToString();

                    var eeoi = 0.0;
                    var hfoTW = 0.0;
                    var methanolTW = 0.0;
                    var cTW = 0.0;
                    var eRating = 0.0;
                    try
                    {
                        eeoi = (Convert.ToDouble(baseData.Rows[0]["HFOACC"] is DBNull ? 0 : baseData.Rows[0]["HFOACC"]) * 3.114 + Convert.ToDouble(baseData.Rows[0]["MethanolACC"] is DBNull ? 0 : baseData.Rows[0]["MethanolACC"]) * 1.375) * 1000 / cargoCapacity / Convert.ToDouble(baseData.Rows[0]["DistanceGrd"] is DBNull ? 1 : Convert.ToDouble(baseData.Rows[0]["DistanceGrd"]) == 0 ? 1 : baseData.Rows[0]["DistanceGrd"]);
                        hfoTW = (Convert.ToDouble(baseData.Rows[0]["HFOACC"] is DBNull ? 0 : baseData.Rows[0]["HFOACC"])) * 1000 / dwt / Convert.ToDouble(baseData.Rows[0]["DistanceGrd"] is DBNull ? 1 : Convert.ToDouble(baseData.Rows[0]["DistanceGrd"]) == 0 ? 1 : baseData.Rows[0]["DistanceGrd"]);
                        methanolTW = (Convert.ToDouble(baseData.Rows[0]["MethanolACC"] is DBNull ? 0 : baseData.Rows[0]["MethanolACC"])) * 1000 / dwt / Convert.ToDouble(baseData.Rows[0]["DistanceGrd"] is DBNull ? 1 : Convert.ToDouble(baseData.Rows[0]["DistanceGrd"]) == 0 ? 1 : baseData.Rows[0]["DistanceGrd"]);
                        cTW = (Convert.ToDouble(baseData.Rows[0]["HFOACC"] is DBNull ? 0 : baseData.Rows[0]["HFOACC"]) * 3.114 + Convert.ToDouble(baseData.Rows[0]["MethanolACC"] is DBNull ? 0 : baseData.Rows[0]["MethanolACC"]) * 1.375) * 1000 / dwt / Convert.ToDouble(baseData.Rows[0]["DistanceGrd"] is DBNull ? 1 : Convert.ToDouble(baseData.Rows[0]["DistanceGrd"]) == 0 ? 1 : baseData.Rows[0]["DistanceGrd"]);
                        eRating = Convert.ToDouble(baseData.Rows[0]["AVGHFO"] is DBNull ? 0 : baseData.Rows[0]["AVGHFO"]) / Convert.ToDouble(preData.Rows[0]["HFO"] is DBNull ? 9999 : (Convert.ToDouble(preData.Rows[0]["HFO"]) == 0 ? 9999 : preData.Rows[0]["HFO"]));
                    }
                    catch (Exception ex) { }
                    var eRatingStr = "";

                    if (eRating > 0.99 && eRating <= 1.09)
                        eRatingStr = lang == "en_US" ? "Normal" : "正常";
                    else if (eRating > 0.94 && eRating <= 0.99)
                        eRatingStr = lang == "en_US" ? "Good. Energy efficiency level increased by " : "良好，能效水平提高" + $"{Math.Round(100 - Convert.ToDouble(eRating) * 100, 1)}%";
                    else if (eRating <= 0.94 && eRating > 0)
                        eRatingStr = lang == "en_US" ? "Excellent. Energy efficiency level increased by " : "极佳，能效水平提高" + $"{Math.Round(100 - Convert.ToDouble(eRating) * 100, 1)}%";
                    else if (eRating == 0)
                        eRatingStr = $"--";
                    else
                        eRatingStr = lang == "en_US" ? "Bad" : "差";

                    var criteriaDto = new CriteriaDto();
                    if (StaticEntities.StaticEntities.Configs.Any(t => t.Number == number && t.IsDevice == 0 && t.Code == "CriteriaHFO"))
                    {
                        criteriaDto.HFO = (double)StaticEntities.StaticEntities.Configs.FirstOrDefault(t => t.Number == number && t.IsDevice == 0 && t.Code == "CriteriaHFO").HighLimit;
                    }
                    if (StaticEntities.StaticEntities.Configs.Any(t => t.Number == number && t.IsDevice == 0 && t.Code == "CriteriaMethanol"))
                    {
                        criteriaDto.Methanol = (double)StaticEntities.StaticEntities.Configs.FirstOrDefault(t => t.Number == number && t.IsDevice == 0 && t.Code == "CriteriaMethanol").HighLimit;
                    }
                    if (StaticEntities.StaticEntities.Configs.Any(t => t.Number == number && t.IsDevice == 0 && t.Code == "CriteriaPower"))
                    {
                        criteriaDto.Power = (double)StaticEntities.StaticEntities.Configs.FirstOrDefault(t => t.Number == number && t.IsDevice == 0 && t.Code == "CriteriaPower").HighLimit;
                    }
                    if (StaticEntities.StaticEntities.Configs.Any(t => t.Number == number && t.IsDevice == 0 && t.Code == "CriteriaSpeed"))
                    {
                        criteriaDto.Speed = (double)StaticEntities.StaticEntities.Configs.FirstOrDefault(t => t.Number == number && t.IsDevice == 0 && t.Code == "CriteriaSpeed").HighLimit;
                    }

                    try
                    {
                        var hfopNM = Convert.ToDouble(baseData.Rows[0]["AVGHFOpNM"]);
                        var methanolpNM = Convert.ToDouble(baseData.Rows[0]["AVGMethanolpNM"]);
                        var hfopPower = Convert.ToDouble(baseData.Rows[0]["AVGHFOpPower"]);
                        var methanolpPower = Convert.ToDouble(baseData.Rows[0]["AVGMethanolpPower"]);

                        var ecValue = (hfopPower / (criteriaDto.HFO / criteriaDto.Power) + hfopNM / (criteriaDto.HFO / criteriaDto.Speed)) / 2;
                        if (ecValue > 0.99 && ecValue <= 1.09)
                            eRatingStr += lang == "en_US" ? " Normal energy consumption level" : " 能耗水平正常";
                        else if (ecValue > 0.94 && ecValue <= 0.99)
                            eRatingStr += lang == "en_US" ? " Good energy consumption level, with a fuel saving rate of " : " 能耗水平良好，节油率为" + $"{Math.Round(100 - Convert.ToDouble(ecValue) * 100, 1)}%";
                        else if (ecValue <= 0.94)
                            eRatingStr += lang == "en_US" ? " Excellent energy consumption level, with a fuel saving rate of " : " 能耗水平极佳，节油率为" + $"{Math.Round(100 - Convert.ToDouble(ecValue) * 100, 1)}%";
                        else
                            eRatingStr += lang == "en_US" ? " Poor energy consumption level" : " 能耗水平差";
                    }
                    catch (Exception)
                    {
                        eRatingStr += lang == "en_US" ? " Energy consumption level:--" : " 能耗水平--";
                    }

                    var result = new
                    {
                        baseinfo = new
                        {
                            DistanceGrd = baseData.Rows.Count <= 0 ? 0 : baseData.Rows[0]["DistanceGrd"],
                            AVGSpeedGrd = baseData.Rows.Count <= 0 ? 0 : baseData.Rows[0]["AVGSpeedGrd"],
                            AVGWaterSpeed = baseData.Rows.Count <= 0 ? 0 : baseData.Rows[0]["AVGWaterSpeed"],
                            AVGDGO = baseData.Rows.Count <= 0 ? 0 : baseData.Rows[0]["AVGDGO"],
                            AVGLFO = baseData.Rows.Count <= 0 ? 0 : baseData.Rows[0]["AVGLFO"],
                            AVGHFO = baseData.Rows.Count <= 0 ? 0 : baseData.Rows[0]["AVGHFO"],
                            AVGLPG_P = baseData.Rows.Count <= 0 ? 0 : baseData.Rows[0]["AVGLPG_P"],
                            AVGLPG_B = baseData.Rows.Count <= 0 ? 0 : baseData.Rows[0]["AVGLPG_B"],
                            AVGLNG = baseData.Rows.Count <= 0 ? 0 : baseData.Rows[0]["AVGLNG"],
                            AVGMethanol = baseData.Rows.Count <= 0 ? 0 : baseData.Rows[0]["AVGMethanol"],
                            AVGEthanol = baseData.Rows.Count <= 0 ? 0 : baseData.Rows[0]["AVGEthanol"],
                            AVGPower = baseData.Rows.Count <= 0 ? 0 : baseData.Rows[0]["AVGPower"],
                            RunTime = baseData.Rows.Count <= 0 ? 0 : baseData.Rows[0]["RunTime"],
                            DGOACC = baseData.Rows.Count <= 0 ? 0 : baseData.Rows[0]["DGOACC"],
                            LFOACC = baseData.Rows.Count <= 0 ? 0 : baseData.Rows[0]["LFOACC"],
                            HFOACC = baseData.Rows.Count <= 0 ? 0 : baseData.Rows[0]["HFOACC"],
                            LPG_PACC = baseData.Rows.Count <= 0 ? 0 : baseData.Rows[0]["LPG_PACC"],
                            LPG_BACC = baseData.Rows.Count <= 0 ? 0 : baseData.Rows[0]["LPG_BACC"],
                            LNGACC = baseData.Rows.Count <= 0 ? 0 : baseData.Rows[0]["LNGACC"],
                            MethanolACC = baseData.Rows.Count <= 0 ? 0 : baseData.Rows[0]["MethanolACC"],
                            EthanolACC = baseData.Rows.Count <= 0 ? 0 : baseData.Rows[0]["EthanolACC"],
                            CalcSpeedGrd = baseData.Rows.Count <= 0 ? 0 : baseData.Rows[0]["CalcSpeedGrd"],
                            AVGDGOpNM = baseData.Rows.Count <= 0 ? 0 : baseData.Rows[0]["AVGDGOpNM"],
                            AVGLFOpNM = baseData.Rows.Count <= 0 ? 0 : baseData.Rows[0]["AVGLFOpNM"],
                            AVGHFOpNM = baseData.Rows.Count <= 0 ? 0 : baseData.Rows[0]["AVGHFOpNM"],
                            AVGLPG_PpNM = baseData.Rows.Count <= 0 ? 0 : baseData.Rows[0]["AVGLPG_PpNM"],
                            AVGLPG_BpNM = baseData.Rows.Count <= 0 ? 0 : baseData.Rows[0]["AVGLPG_BpNM"],
                            AVGLNGpNM = baseData.Rows.Count <= 0 ? 0 : baseData.Rows[0]["AVGLNGpNM"],
                            AVGMethanolpNM = baseData.Rows.Count <= 0 ? 0 : baseData.Rows[0]["AVGMethanolpNM"],
                            AVGEthanolpNM = baseData.Rows.Count <= 0 ? 0 : baseData.Rows[0]["AVGEthanolpNM"],
                            AVGDGOpPower = baseData.Rows.Count <= 0 ? 0 : baseData.Rows[0]["AVGDGOpPower"],
                            AVGLFOpPower = baseData.Rows.Count <= 0 ? 0 : baseData.Rows[0]["AVGLFOpPower"],
                            AVGHFOpPower = baseData.Rows.Count <= 0 ? 0 : baseData.Rows[0]["AVGHFOpPower"],
                            AVGLPG_PpPower = baseData.Rows.Count <= 0 ? 0 : baseData.Rows[0]["AVGLPG_PpPower"],
                            AVGLPG_BpPower = baseData.Rows.Count <= 0 ? 0 : baseData.Rows[0]["AVGLPG_BpPower"],
                            AVGLNGpPower = baseData.Rows.Count <= 0 ? 0 : baseData.Rows[0]["AVGLNGpPower"],
                            AVGMethanolpPower = baseData.Rows.Count <= 0 ? 0 : baseData.Rows[0]["AVGMethanolpPower"],
                            AVGEthanolpPower = baseData.Rows.Count <= 0 ? 0 : baseData.Rows[0]["AVGEthanolpPower"],
                            EEOI = Math.Round(eeoi, 4),
                            HFOTw = Math.Round(hfoTW, 4),
                            MethanolTw = Math.Round(methanolTW, 4),
                            CO2Tw = Math.Round(cTW, 4),
                            Estimate = eRatingStr
                        },
                        cii = cii,
                        flowmeters = fms1,
                        abnormal = abnormalData,
                        ciis = ciis,
                        sfoc = eeData,
                        ciiPrediction = ciiPrediction
                    };

                    return result;

                    #endregion 国能长江01
                }
                else
                {
                    #region 其他船舶

                    StringBuilder sbSql = new StringBuilder();

                    //起始日期流量计总量
                    sbSql.AppendFormat(@$"SELECT
	                                            tf.""ConsAct"",
	                                            tf.""ConsAcc"",
	                                            tf.""DeviceType"",
	                                            tf.""FuelType"",
	                                            tf.""Number"",
	                                            tf.""ReceiveDatetime"",
	                                            tf.""DeviceNo""
                                            FROM
	                                            (
	                                            SELECT
		                                            tf.""Number"",
		                                            tf.""DeviceNo"",
		                                            tf.""FuelType"",
		                                            MIN( tf.""ReceiveDatetime"" ) ""ReceiveDatetime""
	                                            FROM
		                                            ""energy_flowmeter"" tf,
		                                            ""vesselinfo"" t0
	                                            WHERE
		                                            NVL( t0.""delete_time"", TO_TIMESTAMP( '1949-10-01', 'YYYY-MM-DD' ) ) = TO_TIMESTAMP( '1949-10-01', 'YYYY-MM-DD' )
		                                            AND t0.""SN"" = tf.""Number""
		                                            AND t0.""ReceiveDatetime"" = tf.""ReceiveDatetime""
		                                            AND tf.""Number"" = '{number}'
		                                            AND tf.""ReceiveDatetime"" >= TO_TIMESTAMP( '{dateFrom.ToString("yyyy-MM-dd 00:00:00")}', 'YYYY-MM-DD HH24:MI:SS' )
		                                            AND tf.""ReceiveDatetime"" < TO_TIMESTAMP( '{dateTo.ToString("yyyy-MM-dd 00:00:00")}', 'YYYY-MM-DD HH24:MI:SS' )
	                                            GROUP BY
		                                            tf.""Number"",
		                                            tf.""DeviceNo"",
		                                            tf.""FuelType""
	                                            ) tfg,
	                                            ""energy_flowmeter"" tf,
	                                            ""vesselinfo"" t0
                                            WHERE
	                                            NVL( ""delete_time"", TO_TIMESTAMP( '1949-10-01', 'YYYY-MM-DD' ) ) = TO_TIMESTAMP( '1949-10-01', 'YYYY-MM-DD' )
	                                            AND t0.""SN"" = tf.""Number""
	                                            AND t0.""ReceiveDatetime"" = tf.""ReceiveDatetime""
	                                            AND tf.""Number"" = '{number}'
	                                            AND tf.""Number"" = tfg.""Number""
	                                            AND tf.""DeviceNo"" = tfg.""DeviceNo""
	                                            AND tf.""FuelType"" = tfg.""FuelType""
	                                            AND tf.""ReceiveDatetime"" = tfg.""ReceiveDatetime""");
                    var firstFlowmeters = await _sqlRepository.ExecuteDataTable(sbSql.ToString());

                    sbSql.Clear();

                    //终止日期流量计总量
                    sbSql.AppendFormat(@$"SELECT
	                                            tf.""ConsAct"",
	                                            tf.""ConsAcc"",
	                                            tf.""DeviceType"",
	                                            tf.""FuelType"",
	                                            tf.""Number"",
	                                            tf.""ReceiveDatetime"",
	                                            tf.""DeviceNo""
                                            FROM
	                                            (
	                                            SELECT
		                                            tf.""Number"",
		                                            tf.""DeviceNo"",
		                                            tf.""FuelType"",
		                                            MAX( tf.""ReceiveDatetime"" ) ""ReceiveDatetime""
	                                            FROM
		                                            ""energy_flowmeter"" tf,
		                                            ""vesselinfo"" t0
	                                            WHERE
		                                            NVL( t0.""delete_time"", TO_TIMESTAMP( '1949-10-01', 'YYYY-MM-DD' ) ) = TO_TIMESTAMP( '1949-10-01', 'YYYY-MM-DD' )
		                                            AND t0.""SN"" = tf.""Number""
		                                            AND t0.""ReceiveDatetime"" = tf.""ReceiveDatetime""
		                                            AND tf.""Number"" = '{number}'
		                                            AND tf.""ReceiveDatetime"" >= TO_TIMESTAMP( '{dateFrom.ToString("yyyy-MM-dd 00:00:00")}', 'YYYY-MM-DD HH24:MI:SS' )
		                                            AND tf.""ReceiveDatetime"" < TO_TIMESTAMP( '{dateTo.ToString("yyyy-MM-dd 00:00:00")}', 'YYYY-MM-DD HH24:MI:SS' )
	                                            GROUP BY
		                                            tf.""Number"",
		                                            tf.""DeviceNo"",
		                                            tf.""FuelType""
	                                            ) tfg,
	                                            ""energy_flowmeter"" tf,
	                                            ""vesselinfo"" t0
                                            WHERE
	                                            NVL( ""delete_time"", TO_TIMESTAMP( '1949-10-01', 'YYYY-MM-DD' ) ) = TO_TIMESTAMP( '1949-10-01', 'YYYY-MM-DD' )
	                                            AND t0.""SN"" = tf.""Number""
	                                            AND t0.""ReceiveDatetime"" = tf.""ReceiveDatetime""
	                                            AND tf.""Number"" = '{number}'
	                                            AND tf.""Number"" = tfg.""Number""
	                                            AND tf.""DeviceNo"" = tfg.""DeviceNo""
	                                            AND tf.""FuelType"" = tfg.""FuelType""
	                                            AND tf.""ReceiveDatetime"" = tfg.""ReceiveDatetime""");
                    var lastFlowmeters = await _sqlRepository.ExecuteDataTable(sbSql.ToString());

                    sbSql.Clear();

                    //燃料消耗列表
                    var PUs = new DataTable();
                    foreach (DataRow drLastFm in lastFlowmeters.Rows)
                    {
                        var drFirstFm = firstFlowmeters.Select($"DeviceType='{drLastFm["DeviceType"]}' and FuelType='{drLastFm["FuelType"]}' and DeviceNo='{drLastFm["DeviceNo"]}'")?[0];

                        //表添加列
                        if (!PUs.Columns.Contains("DeviceType"))
                            PUs.Columns.Add("DeviceType", typeof(string));
                        if (!PUs.Columns.Contains(drLastFm["FuelType"].ToString()))
                            PUs.Columns.Add(drLastFm["FuelType"].ToString(), typeof(double));

                        //表添加内容
                        var drsCurrent = PUs.Select($"DeviceType='{drLastFm["DeviceType"]}'");
                        if (drsCurrent.Length > 0)
                        {
                            var drCurrent = drsCurrent[0];
                            drCurrent[drLastFm["FuelType"].ToString()] = Convert.ToDouble(StringHelper.GetDataColumnValue(drLastFm["ConsAcc"])) - Convert.ToDouble(StringHelper.GetDataColumnValue(drFirstFm?["ConsAcc"])) + Convert.ToDouble(StringHelper.GetDataColumnValue(drCurrent[drLastFm["FuelType"].ToString()]));
                        }
                        else
                        {
                            var drNew = PUs.NewRow();
                            drNew["DeviceType"] = drLastFm["DeviceType"].ToString();
                            drNew[drLastFm["FuelType"].ToString()] = Convert.ToDouble(StringHelper.GetDataColumnValue(drLastFm["ConsAcc"])) - Convert.ToDouble(StringHelper.GetDataColumnValue(drFirstFm?["ConsAcc"]));
                            PUs.Rows.Add(drNew);
                        }
                    }

                    sbSql.Clear();

                    //总燃油消耗定义
                    var DGOC = Convert.ToDouble(PUs.Columns.Contains("DGO") ? PUs.Compute("Sum(DGO)", "") : 0);
                    var LFOC = Convert.ToDouble(PUs.Columns.Contains("LFO") ? PUs.Compute("Sum(LFO)", "") : 0);
                    var HFOC = Convert.ToDouble(PUs.Columns.Contains("HFO") ? PUs.Compute("Sum(HFO)", "") : 0);
                    var LPG_PC = Convert.ToDouble(PUs.Columns.Contains("LPG_P") ? PUs.Compute("Sum(LPG_P)", "") : 0);
                    var LPG_BC = Convert.ToDouble(PUs.Columns.Contains("LPG_B") ? PUs.Compute("Sum(LPG_B)", "") : 0);
                    var LNGC = Convert.ToDouble(PUs.Columns.Contains("LNG") ? PUs.Compute("Sum(LNG)", "") : 0);
                    var MethanolC = Convert.ToDouble(PUs.Columns.Contains("Methanol") ? PUs.Compute("Sum(Methanol)", "") : 0);
                    var EthanolC = Convert.ToDouble(PUs.Columns.Contains("Ethanol") ? PUs.Compute("Sum(Ethanol)", "") : 0);

                    //平均燃油效率
                    sbSql.AppendFormat(@$"SELECT
	                                        CAST(AVG( CASE WHEN ABS( tt.DGO ) < 0.1 THEN NULL ELSE tt.DGO END ) AS DECIMAL(14,4)) ""AVGDGO"",
	                                        CAST(AVG( CASE WHEN ABS( tt.LFO ) < 0.1 THEN NULL ELSE LFO END ) AS DECIMAL(14,4)) ""AVGLFO"",
	                                        CAST(AVG( CASE WHEN ABS( tt.HFO ) < 0.1 THEN NULL ELSE HFO END ) AS DECIMAL(14,4)) ""AVGHFO"",
	                                        CAST(AVG( CASE WHEN ABS( tt.LPG_P ) < 0.1 THEN NULL ELSE LPG_P END ) AS DECIMAL(14,4)) ""AVGLPG_P"",
	                                        CAST(AVG( CASE WHEN ABS( tt.LPG_B ) < 0.1 THEN NULL ELSE LPG_B END ) AS DECIMAL(14,4)) ""AVGLPG_B"",
	                                        CAST(AVG( CASE WHEN ABS( tt.LNG ) < 0.1 THEN NULL ELSE LNG END ) AS DECIMAL(14,4)) ""AVGLNG"",
	                                        CAST(AVG( CASE WHEN ABS( tt.""Methanol"" ) < 0.1 THEN NULL ELSE tt.""Methanol"" END ) AS DECIMAL(14,4)) ""AVGMethanol"",
	                                        CAST(AVG( CASE WHEN ABS( tt.""Ethanol"" ) < 0.1 THEN NULL ELSE tt.""Ethanol"" END ) AS DECIMAL(14,4)) ""AVGEthanol"",
	                                        CAST(COUNT( 1 ) / 360.0 AS DECIMAL(14,4)) ""RunTime"",
	                                        CAST(COUNT( CASE WHEN ABS( tt.DGO ) < 0.1 THEN NULL ELSE tt.DGO END ) / 360.0 AS DECIMAL(14,4)) ""DGORunTime"",
	                                        CAST(COUNT( CASE WHEN ABS( tt.LFO ) < 0.1 THEN NULL ELSE LFO END ) / 360.0 AS DECIMAL(14,4)) ""LFORunTime"",
	                                        CAST(COUNT( CASE WHEN ABS( tt.HFO ) < 0.1 THEN NULL ELSE HFO END ) / 360.0 AS DECIMAL(14,4)) ""HFORunTime"",
	                                        CAST(COUNT( CASE WHEN ABS( tt.LPG_P ) < 0.1 THEN NULL ELSE LPG_P END ) / 360.0 AS DECIMAL(14,4)) ""LPG_PRunTime"",
	                                        CAST(COUNT( CASE WHEN ABS( tt.LPG_B ) < 0.1 THEN NULL ELSE LPG_B END ) / 360.0 AS DECIMAL(14,4)) ""LPG_BRunTime"",
	                                        CAST(COUNT( CASE WHEN ABS( tt.LNG ) < 0.1 THEN NULL ELSE LNG END ) / 360.0 AS DECIMAL(14,4)) ""LNGRunTime"",
	                                        CAST(COUNT( CASE WHEN ABS( tt.""Methanol"" ) < 0.1 THEN NULL ELSE tt.""Methanol"" END ) / 360.0 AS DECIMAL(14,4)) ""MethanolRunTime"",
	                                        CAST(COUNT( CASE WHEN ABS( tt.""Ethanol"" ) < 0.1 THEN NULL ELSE tt.""Ethanol"" END ) / 360.0 AS DECIMAL(14,4)) ""EthanolRunTime"",
	                                        CAST(AVG( tv.""GroundSpeed"" ) AS DECIMAL(14,4)) ""AVGSpeedGrd"",
	                                        CAST(AVG( tv.""WaterSpeed"" ) AS DECIMAL(14,4)) ""AVGWaterSpeed"",
	                                        CAST(MAX( tv.""TotalDistanceGrd"" ) - MIN( tv.""TotalDistanceGrd"" ) AS DECIMAL(14,4)) ""DistanceGrd"",
	                                        CAST(MAX( tv.""TotalDistanceWat"" ) - MIN( tv.""TotalDistanceWat"" ) AS DECIMAL(14,4)) ""DistanceWat"",
	                                        CAST(AVG( tt.""Power"" ) AS DECIMAL(14,4)) ""AVGPower"",
	                                        CAST(AVG( (CASE WHEN ABS( tt.DGO ) < 0.1 THEN NULL ELSE tt.DGO END) / tv.""GroundSpeed"" ) AS DECIMAL(14,4)) ""AVGDGOpNM"",
	                                        CAST(AVG( (CASE WHEN ABS( tt.LFO ) < 0.1 THEN NULL ELSE LFO END) / tv.""GroundSpeed"" ) AS DECIMAL(14,4)) ""AVGLFOpNM"",
	                                        CAST(AVG( (CASE WHEN ABS( tt.HFO ) < 0.1 THEN NULL ELSE HFO END) / tv.""GroundSpeed"" ) AS DECIMAL(14,4)) ""AVGHFOpNM"",
	                                        CAST(AVG( (CASE WHEN ABS( tt.LPG_P ) < 0.1 THEN NULL ELSE LPG_P END) / tv.""GroundSpeed"" ) AS DECIMAL(14,4)) ""AVGLPG_PpNM"",
	                                        CAST(AVG( (CASE WHEN ABS( tt.LPG_B ) < 0.1 THEN NULL ELSE LPG_B END) / tv.""GroundSpeed"" ) AS DECIMAL(14,4)) ""AVGLPG_BpNM"",
	                                        CAST(AVG( (CASE WHEN ABS( tt.LNG ) < 0.1 THEN NULL ELSE LNG END) / tv.""GroundSpeed"" ) AS DECIMAL(14,4)) ""AVGLNGpNM"",
	                                        CAST(AVG( (CASE WHEN ABS( tt.""Methanol"" ) < 0.1 THEN NULL ELSE tt.""Methanol"" END) / tv.""GroundSpeed"" ) AS DECIMAL(14,4)) ""AVGMethanolpNM"",
	                                        CAST(AVG( (CASE WHEN ABS( tt.""Ethanol"" ) < 0.1 THEN NULL ELSE tt.""Ethanol"" END) / tv.""GroundSpeed"" ) AS DECIMAL(14,4)) ""AVGEthanolpNM"",
	                                        CAST(AVG( (CASE WHEN ABS( tt.DGO ) < 0.1 THEN NULL ELSE tt.DGO END) / tt.""Power"" ) AS DECIMAL(14,4)) ""AVGDGOpPower"",
	                                        CAST(AVG( (CASE WHEN ABS( tt.LFO ) < 0.1 THEN NULL ELSE LFO END) / tt.""Power"" ) AS DECIMAL(14,4)) ""AVGLFOpPower"",
	                                        CAST(AVG( (CASE WHEN ABS( tt.HFO ) < 0.1 THEN NULL ELSE HFO END) / tt.""Power"" ) AS DECIMAL(14,4)) ""AVGHFOpPower"",
	                                        CAST(AVG( (CASE WHEN ABS( tt.LPG_P ) < 0.1 THEN NULL ELSE LPG_P END) / tt.""Power"" ) AS DECIMAL(14,4)) ""AVGLPG_PpPower"",
	                                        CAST(AVG( (CASE WHEN ABS( tt.LPG_B ) < 0.1 THEN NULL ELSE LPG_B END) / tt.""Power"" ) AS DECIMAL(14,4)) ""AVGLPG_BpPower"",
	                                        CAST(AVG( (CASE WHEN ABS( tt.LNG ) < 0.1 THEN NULL ELSE LNG END) / tt.""Power"" ) AS DECIMAL(14,4)) ""AVGLNGpPower"",
	                                        CAST(AVG( (CASE WHEN ABS( tt.""Methanol"" ) < 0.1 THEN NULL ELSE tt.""Methanol"" END) / tt.""Power"" ) AS DECIMAL(14,4)) ""AVGMethanolpPower"",
	                                        CAST(AVG( (CASE WHEN ABS( tt.""Ethanol"" ) < 0.1 THEN NULL ELSE tt.""Ethanol"" END) / tt.""Power"" ) AS DECIMAL(14,4)) ""AVGEthanolpPower""
                                        FROM
	                                        ""vesselinfo"" tv,
	                                        ""energy_totalindicator"" tt
                                        WHERE
	                                        NVL( tv.""delete_time"", TO_TIMESTAMP( '1949-10-01', 'YYYY-MM-DD' ) ) = TO_TIMESTAMP( '1949-10-01', 'YYYY-MM-DD' )
	                                        AND tv.SN = tt.""Number""
	                                        AND tv.""ReceiveDatetime"" = tt.""ReceiveDatetime""
	                                        AND tv.SN = '{number}'
	                                        AND tv.""ReceiveDatetime"" >= TO_TIMESTAMP( '{dateFrom.ToString("yyyy-MM-dd")}', 'YYYY-MM-DD' )
	                                        AND tv.""ReceiveDatetime"" < TO_TIMESTAMP( '{dateFrom.ToString("yyyy-MM-dd")}', 'YYYY-MM-DD' )
	                                        AND tv.""GroundSpeed"" > 0.5
	                                        AND tt.""Power"" > 0.5
	                                        AND tt.""Rpm"" > 0.5");
                    var baseData = await _repository.ExecuteDataTable(sbSql.ToString());

                    sbSql.Clear();

                    //基本信息定义
                    var runtime = Convert.ToDouble(StringHelper.GetDataColumnValue(baseData.Rows[0]["RunTime"])); //航行时间 单位:小时
                    var distance = Convert.ToDouble(StringHelper.GetDataColumnValue(baseData.Rows[0]["DistanceGrd"])); //航行距离 单位:海里
                    distance = distance == 0 ? Convert.ToDouble(StringHelper.GetDataColumnValue(baseData.Rows[0]["DistanceWat"])) : distance;

                    //燃料消耗率曲线
                    sbSql.AppendFormat(@$"SELECT
	                                        TRUNC(tv.""ReceiveDatetime"", 'DD') ""ReceiveDatetime"",
	                                        CAST(AVG( CASE WHEN ABS( tt.DGO ) < 0.1 THEN NULL ELSE tt.DGO END ) / AVG( tv.""GroundSpeed"" ) AS DECIMAL(14,4)) ""DGOPerNmGrd"",
	                                        CAST(AVG( CASE WHEN ABS( tt.LFO ) < 0.1 THEN NULL ELSE tt.LFO END ) / AVG( tv.""GroundSpeed"" ) AS DECIMAL(14,4)) ""LFOPerNmGrd"",
	                                        CAST(AVG( CASE WHEN ABS( tt.HFO ) < 0.1 THEN NULL ELSE tt.HFO END ) / AVG( tv.""GroundSpeed"" ) AS DECIMAL(14,4)) ""HFOPerNmGrd"",
	                                        CAST(AVG( CASE WHEN ABS( tt.LPG_P ) < 0.1 THEN NULL ELSE tt.LPG_P END ) / AVG( tv.""GroundSpeed"" ) AS DECIMAL(14,4)) ""LPG_PPerNmGrd"",
	                                        CAST(AVG( CASE WHEN ABS( tt.LPG_B ) < 0.1 THEN NULL ELSE tt.LPG_B END ) / AVG( tv.""GroundSpeed"" ) AS DECIMAL(14,4)) ""LPG_BPerNmGrd"",
	                                        CAST(AVG( CASE WHEN ABS( tt.LNG ) < 0.1 THEN NULL ELSE tt.LNG END ) / AVG( tv.""GroundSpeed"" ) AS DECIMAL(14,4)) ""LNGPerNmGrd"",
	                                        CAST(AVG( CASE WHEN ABS( tt.""Methanol"" ) < 0.1 THEN NULL ELSE tt.""Methanol"" END ) / AVG( tv.""GroundSpeed"" ) AS DECIMAL(14,4)) ""MethanolPerNmGrd"",
	                                        CAST(AVG( CASE WHEN ABS( tt.""Ethanol"" ) < 0.1 THEN NULL ELSE tt.""Ethanol"" END ) / AVG( tv.""GroundSpeed"" ) AS DECIMAL(14,4)) ""EthanolPerNmGrd"",
	                                        CAST(AVG( CASE WHEN ABS( tt.DGO ) < 0.1 THEN NULL ELSE tt.DGO END ) / AVG( tv.""WaterSpeed"" ) AS DECIMAL(14,4)) ""DGOPerNmWat"",
	                                        CAST(AVG( CASE WHEN ABS( tt.LFO ) < 0.1 THEN NULL ELSE tt.LFO END ) / AVG( tv.""WaterSpeed"" ) AS DECIMAL(14,4)) ""LFOPerNmWat"",
	                                        CAST(AVG( CASE WHEN ABS( tt.HFO ) < 0.1 THEN NULL ELSE tt.HFO END ) / AVG( tv.""WaterSpeed"" ) AS DECIMAL(14,4)) ""HFOPerNmWat"",
	                                        CAST(AVG( CASE WHEN ABS( tt.LPG_P ) < 0.1 THEN NULL ELSE tt.LPG_P END ) / AVG( tv.""WaterSpeed"" ) AS DECIMAL(14,4)) ""LPG_PPerNmWat"",
	                                        CAST(AVG( CASE WHEN ABS( tt.LPG_B ) < 0.1 THEN NULL ELSE tt.LPG_B END ) / AVG( tv.""WaterSpeed"" ) AS DECIMAL(14,4)) ""LPG_BPerNmWat"",
	                                        CAST(AVG( CASE WHEN ABS( tt.LNG ) < 0.1 THEN NULL ELSE tt.LNG END ) / AVG( tv.""WaterSpeed"" ) AS DECIMAL(14,4)) ""LNGPerNmWat"",
	                                        CAST(AVG( CASE WHEN ABS( tt.""Methanol"" ) < 0.1 THEN NULL ELSE tt.""Methanol"" END ) / AVG( tv.""WaterSpeed"" ) AS DECIMAL(14,4)) ""MethanolPerNmWat"",
	                                        CAST(AVG( CASE WHEN ABS( tt.""Ethanol"" ) < 0.1 THEN NULL ELSE tt.""Ethanol"" END ) / AVG( tv.""WaterSpeed"" ) AS DECIMAL(14,4)) ""EthanolPerNmWat"",
	                                        CAST(AVG( CASE WHEN ABS( tt.DGO ) < 0.1 THEN NULL ELSE tt.DGO END ) / AVG( tt.""Power"" ) AS DECIMAL(14,4)) ""DGOPerPower"",
	                                        CAST(AVG( CASE WHEN ABS( tt.LFO ) < 0.1 THEN NULL ELSE tt.LFO END ) / AVG( tt.""Power"" ) AS DECIMAL(14,4)) ""LFOPerPower"",
	                                        CAST(AVG( CASE WHEN ABS( tt.HFO ) < 0.1 THEN NULL ELSE tt.HFO END ) / AVG( tt.""Power"" ) AS DECIMAL(14,4)) ""HFOPerPower"",
	                                        CAST(AVG( CASE WHEN ABS( tt.LPG_P ) < 0.1 THEN NULL ELSE tt.LPG_P END ) / AVG( tt.""Power"" ) AS DECIMAL(14,4)) ""LPG_PPerPower"",
	                                        CAST(AVG( CASE WHEN ABS( tt.LPG_B ) < 0.1 THEN NULL ELSE tt.LPG_B END ) / AVG( tt.""Power"" ) AS DECIMAL(14,4)) ""LPG_BPerPower"",
	                                        CAST(AVG( CASE WHEN ABS( tt.LNG ) < 0.1 THEN NULL ELSE tt.LNG END ) / AVG( tt.""Power"" ) AS DECIMAL(14,4)) ""LNGPerPower"",
	                                        CAST(AVG( CASE WHEN ABS( tt.""Methanol"" ) < 0.1 THEN NULL ELSE tt.""Methanol"" END ) / AVG( tt.""Power"" ) AS DECIMAL(14,4)) ""MethanolPerPower"",
	                                        CAST(AVG( CASE WHEN ABS( tt.""Ethanol"" ) < 0.1 THEN NULL ELSE tt.""Ethanol"" END ) / AVG( tt.""Power"" ) AS DECIMAL(14,4)) ""EthanolPerPower""
                                        FROM
	                                        ""vesselinfo"" tv,
	                                        ""energy_totalindicator"" tt
                                        WHERE
	                                        NVL( tv.""delete_time"", TO_TIMESTAMP( '1949-10-01', 'YYYY-MM-DD' ) ) = TO_TIMESTAMP( '1949-10-01', 'YYYY-MM-DD' )
	                                        AND tv.SN = tt.""Number""
	                                        AND tv.""ReceiveDatetime"" = tt.""ReceiveDatetime""
	                                        AND tv.SN = '{number}'
	                                        AND tv.""ReceiveDatetime"" >= TO_TIMESTAMP( '{dateFrom.ToString("yyyy-MM-dd")}', 'YYYY-MM-DD' )
	                                        AND tv.""ReceiveDatetime"" < TO_TIMESTAMP( '{dateTo.ToString("yyyy-MM-dd")}', 'YYYY-MM-DD' )
	                                        AND tv.""GroundSpeed"" > 0.5
	                                        AND tt.""Power"" > 0.5
	                                        AND tt.""Rpm"" > 0.5
                                        GROUP BY
	                                        TRUNC(tv.""ReceiveDatetime"", 'DD')
                                        ORDER BY
	                                        TRUNC(tv.""ReceiveDatetime"", 'DD')");
                    var eeData = await _repository.ExecuteDataTable(sbSql.ToString());

                    sbSql.Clear();

                    //获取与预测油耗偏差较大时间点航行状况信息
                    sbSql.AppendFormat(@"SELECT
	* 
FROM
	(
	SELECT
		TRUNC( t0.""ReceiveDatetime"", 'DD' ) ""ReceiveDatetime"",
		CAST( AVG( t0.""WaterSpeed"" ) AS DECIMAL ( 14, 4 ) ) ""WaterSpeed"",
		CAST( AVG( t0.""GroundSpeed"" ) AS DECIMAL ( 14, 4 ) ) ""GroundSpeed"",
		CAST( AVG( t0.""Trim"" ) AS DECIMAL ( 14, 4 ) ) ""Trim"",
		CAST( AVG( t0.""Heel"" ) AS DECIMAL ( 14, 4 ) ) ""Heel"",
		CAST( AVG( t0.""Draft"" ) AS DECIMAL ( 14, 4 ) ) ""Draft"",
		CAST( AVG( t1.""Power"" ) AS DECIMAL ( 14, 4 ) ) ""Power"",
		CAST( AVG( t1.DGO ) AS DECIMAL ( 14, 4 ) ) ""DGO"",
		CAST( AVG( t1.LFO ) AS DECIMAL ( 14, 4 ) ) ""LFO"",
		CAST( AVG( t1.HFO ) AS DECIMAL ( 14, 4 ) ) ""HFO"",
		CAST( AVG( t1.LPG_P ) AS DECIMAL ( 14, 4 ) ) ""LPG_P"",
		CAST( AVG( t1.LPG_B ) AS DECIMAL ( 14, 4 ) ) ""LPG_B"",
		CAST( AVG( t1.LNG ) AS DECIMAL ( 14, 4 ) ) ""LNG"",
		CAST( AVG( t1.""Methanol"" ) AS DECIMAL ( 14, 4 ) ) ""Methanol"",
		CAST( AVG( t1.""Ethanol"" ) AS DECIMAL ( 14, 4 ) ) ""Ethanol"" 
	FROM
		""vesselinfo"" t0,
		""energy_totalindicator"" t1,
		""energy_prediction"" t2 
	WHERE
		NVL( t0.""delete_time"", TO_TIMESTAMP( '1949-10-01', 'YYYY-MM-DD' ) ) = TO_TIMESTAMP( '1949-10-01', 'YYYY-MM-DD' ) 
		AND t0.SN = t1.""Number"" 
		AND t0.""ReceiveDatetime"" = t1.""ReceiveDatetime"" 
		AND t0.SN = t2.""Number"" 
		AND t0.""ReceiveDatetime"" = t2.""ReceiveDatetime"" 
		AND t0.""GroundSpeed"" > 0 
		AND t1.""Number"" = '{0}' 
		AND t1.""ReceiveDatetime"" >= TO_TIMESTAMP( '{1}', 'YYYY-MM-DD HH24:MI:SS' ) 
		AND t1.""ReceiveDatetime"" < TO_TIMESTAMP( '{2}', 'YYYY-MM-DD HH24:MI:SS' ) 
		AND t1.""Power"" > 0.5 
		AND t1.""Rpm"" > 0.5 
		AND ABS( t2.DGO - t1.DGO ) / DECODE( t2.DGO, 0, 1 ) > 0.8 
	GROUP BY
		TRUNC( t0.""ReceiveDatetime"", 'DD' ) UNION
	SELECT
		TRUNC( t0.""ReceiveDatetime"", 'DD' ) ""ReceiveDatetime"",
		CAST( AVG( t0.""WaterSpeed"" ) AS DECIMAL ( 14, 4 ) ) ""WaterSpeed"",
		CAST( AVG( t0.""GroundSpeed"" ) AS DECIMAL ( 14, 4 ) ) ""GroundSpeed"",
		CAST( AVG( t0.""Trim"" ) AS DECIMAL ( 14, 4 ) ) ""Trim"",
		CAST( AVG( t0.""Heel"" ) AS DECIMAL ( 14, 4 ) ) ""Heel"",
		CAST( AVG( t0.""Draft"" ) AS DECIMAL ( 14, 4 ) ) ""Draft"",
		CAST( AVG( t1.""Power"" ) AS DECIMAL ( 14, 4 ) ) ""Power"",
		CAST( AVG( t1.DGO ) AS DECIMAL ( 14, 4 ) ) ""DGO"",
		CAST( AVG( t1.LFO ) AS DECIMAL ( 14, 4 ) ) ""LFO"",
		CAST( AVG( t1.HFO ) AS DECIMAL ( 14, 4 ) ) ""HFO"",
		CAST( AVG( t1.LPG_P ) AS DECIMAL ( 14, 4 ) ) ""LPG_P"",
		CAST( AVG( t1.LPG_B ) AS DECIMAL ( 14, 4 ) ) ""LPG_B"",
		CAST( AVG( t1.LNG ) AS DECIMAL ( 14, 4 ) ) ""LNG"",
		CAST( AVG( t1.""Methanol"" ) AS DECIMAL ( 14, 4 ) ) ""Methanol"",
		CAST( AVG( t1.""Ethanol"" ) AS DECIMAL ( 14, 4 ) ) ""Ethanol"" 
	FROM
		""vesselinfo"" t0,
		""energy_totalindicator"" t1,
		""energy_prediction"" t2 
	WHERE
		NVL( t0.""delete_time"", TO_TIMESTAMP( '1949-10-01', 'YYYY-MM-DD' ) ) = TO_TIMESTAMP( '1949-10-01', 'YYYY-MM-DD' ) 
		AND t0.SN = t1.""Number"" 
		AND t0.""ReceiveDatetime"" = t1.""ReceiveDatetime"" 
		AND t0.SN = t2.""Number"" 
		AND t0.""ReceiveDatetime"" = t2.""ReceiveDatetime"" 
		AND t0.""GroundSpeed"" > 0 
		AND t1.""Number"" = '{0}' 
		AND t1.""ReceiveDatetime"" >= TO_TIMESTAMP( '{1}', 'YYYY-MM-DD HH24:MI:SS' ) 
		AND t1.""ReceiveDatetime"" < TO_TIMESTAMP( '{2}', 'YYYY-MM-DD HH24:MI:SS' ) 
		AND t1.""Power"" > 0.5 
		AND t1.""Rpm"" > 0.5 
		AND ABS( t2.LFO - t1.LFO ) / DECODE( t2.LFO, 0, 1 ) > 0.8 
	GROUP BY
		TRUNC( t0.""ReceiveDatetime"", 'DD' ) UNION
	SELECT
		TRUNC( t0.""ReceiveDatetime"", 'DD' ) ""ReceiveDatetime"",
		CAST( AVG( t0.""WaterSpeed"" ) AS DECIMAL ( 14, 4 ) ) ""WaterSpeed"",
		CAST( AVG( t0.""GroundSpeed"" ) AS DECIMAL ( 14, 4 ) ) ""GroundSpeed"",
		CAST( AVG( t0.""Trim"" ) AS DECIMAL ( 14, 4 ) ) ""Trim"",
		CAST( AVG( t0.""Heel"" ) AS DECIMAL ( 14, 4 ) ) ""Heel"",
		CAST( AVG( t0.""Draft"" ) AS DECIMAL ( 14, 4 ) ) ""Draft"",
		CAST( AVG( t1.""Power"" ) AS DECIMAL ( 14, 4 ) ) ""Power"",
		CAST( AVG( t1.DGO ) AS DECIMAL ( 14, 4 ) ) ""DGO"",
		CAST( AVG( t1.LFO ) AS DECIMAL ( 14, 4 ) ) ""LFO"",
		CAST( AVG( t1.HFO ) AS DECIMAL ( 14, 4 ) ) ""HFO"",
		CAST( AVG( t1.LPG_P ) AS DECIMAL ( 14, 4 ) ) ""LPG_P"",
		CAST( AVG( t1.LPG_B ) AS DECIMAL ( 14, 4 ) ) ""LPG_B"",
		CAST( AVG( t1.LNG ) AS DECIMAL ( 14, 4 ) ) ""LNG"",
		CAST( AVG( t1.""Methanol"" ) AS DECIMAL ( 14, 4 ) ) ""Methanol"",
		CAST( AVG( t1.""Ethanol"" ) AS DECIMAL ( 14, 4 ) ) ""Ethanol"" 
	FROM
		""vesselinfo"" t0,
		""energy_totalindicator"" t1,
		""energy_prediction"" t2 
	WHERE
		NVL( t0.""delete_time"", TO_TIMESTAMP( '1949-10-01', 'YYYY-MM-DD' ) ) = TO_TIMESTAMP( '1949-10-01', 'YYYY-MM-DD' ) 
		AND t0.SN = t1.""Number"" 
		AND t0.""ReceiveDatetime"" = t1.""ReceiveDatetime"" 
		AND t0.SN = t2.""Number"" 
		AND t0.""ReceiveDatetime"" = t2.""ReceiveDatetime"" 
		AND t0.""GroundSpeed"" > 0 
		AND t1.""Number"" = '{0}' 
		AND t1.""ReceiveDatetime"" >= TO_TIMESTAMP( '{1}', 'YYYY-MM-DD HH24:MI:SS' ) 
		AND t1.""ReceiveDatetime"" < TO_TIMESTAMP( '{2}', 'YYYY-MM-DD HH24:MI:SS' ) 
		AND t1.""Power"" > 0.5 
		AND t1.""Rpm"" > 0.5 
		AND ABS( t2.HFO - t1.HFO ) / DECODE( t2.HFO, 0, 1 ) > 0.8 
	GROUP BY
		TRUNC( t0.""ReceiveDatetime"", 'DD' ) UNION
	SELECT
		TRUNC( t0.""ReceiveDatetime"", 'DD' ) ""ReceiveDatetime"",
		CAST( AVG( t0.""WaterSpeed"" ) AS DECIMAL ( 14, 4 ) ) ""WaterSpeed"",
		CAST( AVG( t0.""GroundSpeed"" ) AS DECIMAL ( 14, 4 ) ) ""GroundSpeed"",
		CAST( AVG( t0.""Trim"" ) AS DECIMAL ( 14, 4 ) ) ""Trim"",
		CAST( AVG( t0.""Heel"" ) AS DECIMAL ( 14, 4 ) ) ""Heel"",
		CAST( AVG( t0.""Draft"" ) AS DECIMAL ( 14, 4 ) ) ""Draft"",
		CAST( AVG( t1.""Power"" ) AS DECIMAL ( 14, 4 ) ) ""Power"",
		CAST( AVG( t1.DGO ) AS DECIMAL ( 14, 4 ) ) ""DGO"",
		CAST( AVG( t1.LFO ) AS DECIMAL ( 14, 4 ) ) ""LFO"",
		CAST( AVG( t1.HFO ) AS DECIMAL ( 14, 4 ) ) ""HFO"",
		CAST( AVG( t1.LPG_P ) AS DECIMAL ( 14, 4 ) ) ""LPG_P"",
		CAST( AVG( t1.LPG_B ) AS DECIMAL ( 14, 4 ) ) ""LPG_B"",
		CAST( AVG( t1.LNG ) AS DECIMAL ( 14, 4 ) ) ""LNG"",
		CAST( AVG( t1.""Methanol"" ) AS DECIMAL ( 14, 4 ) ) ""Methanol"",
		CAST( AVG( t1.""Ethanol"" ) AS DECIMAL ( 14, 4 ) ) ""Ethanol"" 
	FROM
		""vesselinfo"" t0,
		""energy_totalindicator"" t1,
		""energy_prediction"" t2 
	WHERE
		NVL( t0.""delete_time"", TO_TIMESTAMP( '1949-10-01', 'YYYY-MM-DD' ) ) = TO_TIMESTAMP( '1949-10-01', 'YYYY-MM-DD' ) 
		AND t0.SN = t1.""Number"" 
		AND t0.""ReceiveDatetime"" = t1.""ReceiveDatetime"" 
		AND t0.SN = t2.""Number"" 
		AND t0.""ReceiveDatetime"" = t2.""ReceiveDatetime"" 
		AND t0.""GroundSpeed"" > 0 
		AND t1.""Number"" = '{0}' 
		AND t1.""ReceiveDatetime"" >= TO_TIMESTAMP( '{1}', 'YYYY-MM-DD HH24:MI:SS' ) 
		AND t1.""ReceiveDatetime"" < TO_TIMESTAMP( '{2}', 'YYYY-MM-DD HH24:MI:SS' ) 
		AND t1.""Power"" > 0.5 
		AND t1.""Rpm"" > 0.5 
		AND ABS( t2.LPG_P - t1.LPG_P ) / DECODE( t2.LPG_P, 0, 1 ) > 0.8 
	GROUP BY
		TRUNC( t0.""ReceiveDatetime"", 'DD' ) UNION
	SELECT
		TRUNC( t0.""ReceiveDatetime"", 'DD' ) ""ReceiveDatetime"",
		CAST( AVG( t0.""WaterSpeed"" ) AS DECIMAL ( 14, 4 ) ) ""WaterSpeed"",
		CAST( AVG( t0.""GroundSpeed"" ) AS DECIMAL ( 14, 4 ) ) ""GroundSpeed"",
		CAST( AVG( t0.""Trim"" ) AS DECIMAL ( 14, 4 ) ) ""Trim"",
		CAST( AVG( t0.""Heel"" ) AS DECIMAL ( 14, 4 ) ) ""Heel"",
		CAST( AVG( t0.""Draft"" ) AS DECIMAL ( 14, 4 ) ) ""Draft"",
		CAST( AVG( t1.""Power"" ) AS DECIMAL ( 14, 4 ) ) ""Power"",
		CAST( AVG( t1.DGO ) AS DECIMAL ( 14, 4 ) ) ""DGO"",
		CAST( AVG( t1.LFO ) AS DECIMAL ( 14, 4 ) ) ""LFO"",
		CAST( AVG( t1.HFO ) AS DECIMAL ( 14, 4 ) ) ""HFO"",
		CAST( AVG( t1.LPG_P ) AS DECIMAL ( 14, 4 ) ) ""LPG_P"",
		CAST( AVG( t1.LPG_B ) AS DECIMAL ( 14, 4 ) ) ""LPG_B"",
		CAST( AVG( t1.LNG ) AS DECIMAL ( 14, 4 ) ) ""LNG"",
		CAST( AVG( t1.""Methanol"" ) AS DECIMAL ( 14, 4 ) ) ""Methanol"",
		CAST( AVG( t1.""Ethanol"" ) AS DECIMAL ( 14, 4 ) ) ""Ethanol"" 
	FROM
		""vesselinfo"" t0,
		""energy_totalindicator"" t1,
		""energy_prediction"" t2 
	WHERE
		NVL( t0.""delete_time"", TO_TIMESTAMP( '1949-10-01', 'YYYY-MM-DD' ) ) = TO_TIMESTAMP( '1949-10-01', 'YYYY-MM-DD' ) 
		AND t0.SN = t1.""Number"" 
		AND t0.""ReceiveDatetime"" = t1.""ReceiveDatetime"" 
		AND t0.SN = t2.""Number"" 
		AND t0.""ReceiveDatetime"" = t2.""ReceiveDatetime"" 
		AND t0.""GroundSpeed"" > 0 
		AND t1.""Number"" = '{0}' 
		AND t1.""ReceiveDatetime"" >= TO_TIMESTAMP( '{1}', 'YYYY-MM-DD HH24:MI:SS' ) 
		AND t1.""ReceiveDatetime"" < TO_TIMESTAMP( '{2}', 'YYYY-MM-DD HH24:MI:SS' ) 
		AND t1.""Power"" > 0.5 
		AND t1.""Rpm"" > 0.5 
		AND ABS( t2.LPG_B - t1.LPG_B ) / DECODE( t2.LPG_B, 0, 1 ) > 0.8 
	GROUP BY
		TRUNC( t0.""ReceiveDatetime"", 'DD' ) UNION
	SELECT
		TRUNC( t0.""ReceiveDatetime"", 'DD' ) ""ReceiveDatetime"",
		CAST( AVG( t0.""WaterSpeed"" ) AS DECIMAL ( 14, 4 ) ) ""WaterSpeed"",
		CAST( AVG( t0.""GroundSpeed"" ) AS DECIMAL ( 14, 4 ) ) ""GroundSpeed"",
		CAST( AVG( t0.""Trim"" ) AS DECIMAL ( 14, 4 ) ) ""Trim"",
		CAST( AVG( t0.""Heel"" ) AS DECIMAL ( 14, 4 ) ) ""Heel"",
		CAST( AVG( t0.""Draft"" ) AS DECIMAL ( 14, 4 ) ) ""Draft"",
		CAST( AVG( t1.""Power"" ) AS DECIMAL ( 14, 4 ) ) ""Power"",
		CAST( AVG( t1.DGO ) AS DECIMAL ( 14, 4 ) ) ""DGO"",
		CAST( AVG( t1.LFO ) AS DECIMAL ( 14, 4 ) ) ""LFO"",
		CAST( AVG( t1.HFO ) AS DECIMAL ( 14, 4 ) ) ""HFO"",
		CAST( AVG( t1.LPG_P ) AS DECIMAL ( 14, 4 ) ) ""LPG_P"",
		CAST( AVG( t1.LPG_B ) AS DECIMAL ( 14, 4 ) ) ""LPG_B"",
		CAST( AVG( t1.LNG ) AS DECIMAL ( 14, 4 ) ) ""LNG"",
		CAST( AVG( t1.""Methanol"" ) AS DECIMAL ( 14, 4 ) ) ""Methanol"",
		CAST( AVG( t1.""Ethanol"" ) AS DECIMAL ( 14, 4 ) ) ""Ethanol"" 
	FROM
		""vesselinfo"" t0,
		""energy_totalindicator"" t1,
		""energy_prediction"" t2 
	WHERE
		NVL( t0.""delete_time"", TO_TIMESTAMP( '1949-10-01', 'YYYY-MM-DD' ) ) = TO_TIMESTAMP( '1949-10-01', 'YYYY-MM-DD' ) 
		AND t0.SN = t1.""Number"" 
		AND t0.""ReceiveDatetime"" = t1.""ReceiveDatetime"" 
		AND t0.SN = t2.""Number"" 
		AND t0.""ReceiveDatetime"" = t2.""ReceiveDatetime"" 
		AND t0.""GroundSpeed"" > 0 
		AND t1.""Number"" = '{0}' 
		AND t1.""ReceiveDatetime"" >= TO_TIMESTAMP( '{1}', 'YYYY-MM-DD HH24:MI:SS' ) 
		AND t1.""ReceiveDatetime"" < TO_TIMESTAMP( '{2}', 'YYYY-MM-DD HH24:MI:SS' ) 
		AND t1.""Power"" > 0.5 
		AND t1.""Rpm"" > 0.5 
		AND ABS( t2.LNG - t1.LNG ) / DECODE( t2.LNG, 0, 1 ) > 0.8 
	GROUP BY
		TRUNC( t0.""ReceiveDatetime"", 'DD' ) UNION
	SELECT
		TRUNC( t0.""ReceiveDatetime"", 'DD' ) ""ReceiveDatetime"",
		CAST( AVG( t0.""WaterSpeed"" ) AS DECIMAL ( 14, 4 ) ) ""WaterSpeed"",
		CAST( AVG( t0.""GroundSpeed"" ) AS DECIMAL ( 14, 4 ) ) ""GroundSpeed"",
		CAST( AVG( t0.""Trim"" ) AS DECIMAL ( 14, 4 ) ) ""Trim"",
		CAST( AVG( t0.""Heel"" ) AS DECIMAL ( 14, 4 ) ) ""Heel"",
		CAST( AVG( t0.""Draft"" ) AS DECIMAL ( 14, 4 ) ) ""Draft"",
		CAST( AVG( t1.""Power"" ) AS DECIMAL ( 14, 4 ) ) ""Power"",
		CAST( AVG( t1.DGO ) AS DECIMAL ( 14, 4 ) ) ""DGO"",
		CAST( AVG( t1.LFO ) AS DECIMAL ( 14, 4 ) ) ""LFO"",
		CAST( AVG( t1.HFO ) AS DECIMAL ( 14, 4 ) ) ""HFO"",
		CAST( AVG( t1.LPG_P ) AS DECIMAL ( 14, 4 ) ) ""LPG_P"",
		CAST( AVG( t1.LPG_B ) AS DECIMAL ( 14, 4 ) ) ""LPG_B"",
		CAST( AVG( t1.LNG ) AS DECIMAL ( 14, 4 ) ) ""LNG"",
		CAST( AVG( t1.""Methanol"" ) AS DECIMAL ( 14, 4 ) ) ""Methanol"",
		CAST( AVG( t1.""Ethanol"" ) AS DECIMAL ( 14, 4 ) ) ""Ethanol"" 
	FROM
		""vesselinfo"" t0,
		""energy_totalindicator"" t1,
		""energy_prediction"" t2 
	WHERE
		NVL( t0.""delete_time"", TO_TIMESTAMP( '1949-10-01', 'YYYY-MM-DD' ) ) = TO_TIMESTAMP( '1949-10-01', 'YYYY-MM-DD' ) 
		AND t0.SN = t1.""Number"" 
		AND t0.""ReceiveDatetime"" = t1.""ReceiveDatetime"" 
		AND t0.SN = t2.""Number"" 
		AND t0.""ReceiveDatetime"" = t2.""ReceiveDatetime"" 
		AND t0.""GroundSpeed"" > 0 
		AND t1.""Number"" = '{0}' 
		AND t1.""ReceiveDatetime"" >= TO_TIMESTAMP( '{1}', 'YYYY-MM-DD HH24:MI:SS' ) 
		AND t1.""ReceiveDatetime"" < TO_TIMESTAMP( '{2}', 'YYYY-MM-DD HH24:MI:SS' ) 
		AND t1.""Power"" > 0.5 
		AND t1.""Rpm"" > 0.5 
		AND ABS( t2.""Methanol"" - t1.""Methanol"" ) / DECODE( t2.""Methanol"", 0, 1 ) > 0.8 
	GROUP BY
		TRUNC( t0.""ReceiveDatetime"", 'DD' ) UNION
	SELECT
		TRUNC( t0.""ReceiveDatetime"", 'DD' ) ""ReceiveDatetime"",
		CAST( AVG( t0.""WaterSpeed"" ) AS DECIMAL ( 14, 4 ) ) ""WaterSpeed"",
		CAST( AVG( t0.""GroundSpeed"" ) AS DECIMAL ( 14, 4 ) ) ""GroundSpeed"",
		CAST( AVG( t0.""Trim"" ) AS DECIMAL ( 14, 4 ) ) ""Trim"",
		CAST( AVG( t0.""Heel"" ) AS DECIMAL ( 14, 4 ) ) ""Heel"",
		CAST( AVG( t0.""Draft"" ) AS DECIMAL ( 14, 4 ) ) ""Draft"",
		CAST( AVG( t1.""Power"" ) AS DECIMAL ( 14, 4 ) ) ""Power"",
		CAST( AVG( t1.DGO ) AS DECIMAL ( 14, 4 ) ) ""DGO"",
		CAST( AVG( t1.LFO ) AS DECIMAL ( 14, 4 ) ) ""LFO"",
		CAST( AVG( t1.HFO ) AS DECIMAL ( 14, 4 ) ) ""HFO"",
		CAST( AVG( t1.LPG_P ) AS DECIMAL ( 14, 4 ) ) ""LPG_P"",
		CAST( AVG( t1.LPG_B ) AS DECIMAL ( 14, 4 ) ) ""LPG_B"",
		CAST( AVG( t1.LNG ) AS DECIMAL ( 14, 4 ) ) ""LNG"",
		CAST( AVG( t1.""Methanol"" ) AS DECIMAL ( 14, 4 ) ) ""Methanol"",
		CAST( AVG( t1.""Ethanol"" ) AS DECIMAL ( 14, 4 ) ) ""Ethanol"" 
	FROM
		""vesselinfo"" t0,
		""energy_totalindicator"" t1,
		""energy_prediction"" t2 
	WHERE
		NVL( t0.""delete_time"", TO_TIMESTAMP( '1949-10-01', 'YYYY-MM-DD' ) ) = TO_TIMESTAMP( '1949-10-01', 'YYYY-MM-DD' ) 
		AND t0.SN = t1.""Number"" 
		AND t0.""ReceiveDatetime"" = t1.""ReceiveDatetime"" 
		AND t0.SN = t2.""Number"" 
		AND t0.""ReceiveDatetime"" = t2.""ReceiveDatetime"" 
		AND t0.""GroundSpeed"" > 0 
		AND t1.""Number"" = '{0}' 
		AND t1.""ReceiveDatetime"" >= TO_TIMESTAMP( '{1}', 'YYYY-MM-DD HH24:MI:SS' ) 
		AND t1.""ReceiveDatetime"" < TO_TIMESTAMP( '{2}', 'YYYY-MM-DD HH24:MI:SS' ) 
		AND t1.""Power"" > 0.5 
		AND t1.""Rpm"" > 0.5 
		AND ABS( t2.""Ethanol"" - t1.""Ethanol"" ) / DECODE( t2.""Ethanol"", 0, 1 ) > 0.8 
	GROUP BY
		TRUNC( t0.""ReceiveDatetime"", 'DD' ) 
	) t 
ORDER BY
	t.""ReceiveDatetime""", number, dateFrom.ToString("yyyy-MM-dd 00:00:00"), dateTo.ToString("yyyy-MM-dd 00:00:00"));
                    var abnormalData = await _sqlRepository.ExecuteDataTable(sbSql.ToString());

                    sbSql.Clear();

                    //获取每天CII计算指标
                    sbSql.AppendFormat(@"SELECT
	                                    TRUNC(t0.""ReceiveDatetime"", 'DD') ""ReceiveDate"",
	                                    MAX(t0.""TotalDistanceGrd"" + NVL(t0.""ResetDistanceGrd"", 0)) ""DistanceGrd1"",
	                                    MIN(t0.""TotalDistanceGrd"" + NVL(t0.""ResetDistanceGrd"", 0)) ""DistanceGrd2"",
	                                    max(t1.""DGOAccumulated"") ""DGOAcc1"",
	                                    min(t1.""DGOAccumulated"") ""DGOAcc2"",
	                                    max(t1.""EthanolAccumulated"") ""EthanolAcc1"",
	                                    min(t1.""EthanolAccumulated"") ""EthanolAcc2"",
	                                    max(t1.""HFOAccumulated"") ""HFOAcc1"",
	                                    min(t1.""HFOAccumulated"") ""HFOAcc2"",
	                                    max(t1.""LFOAccumulated"") ""LFOAcc1"",
	                                    min(t1.""LFOAccumulated"") ""LFOAcc2"",
	                                    max(t1.""LNGAccumulated"") ""LNGAcc1"",
	                                    min(t1.""LNGAccumulated"") ""LNGAcc2"",
	                                    max(t1.""LPG_BAccumulated"") ""LPG_BAcc1"",
	                                    min(t1.""LPG_BAccumulated"") ""LPG_BAcc2"",
	                                    max(t1.""LPG_PAccumulated"") ""LPG_PAcc1"",
	                                    min(t1.""LPG_PAccumulated"") ""LPG_PAcc2"",
	                                    max(t1.""MethanolAccumulated"") ""MethanolAcc1"",
	                                    min(t1.""MethanolAccumulated"") ""MethanolAcc2""
                                    FROM
	                                    ""vesselinfo"" t0,
	                                    ""energy_totalindicator"" t1
                                    WHERE
	                                    NVL(t0.""delete_time"", TO_TIMESTAMP('1949-10-01', 'YYYY-MM-DD')) = TO_TIMESTAMP('1949-10-01', 'YYYY-MM-DD')
	                                    AND t0.SN = t1.""Number""
	                                    AND t0.""ReceiveDatetime"" = t1.""ReceiveDatetime""
	                                    AND t0.SN = '{0}'
	                                    AND t0.""ReceiveDatetime"" >= TO_TIMESTAMP('{1}', 'YYYY-MM-DD HH24:MI:SS')
	                                    AND t0.""ReceiveDatetime"" < TO_TIMESTAMP('{2}', 'YYYY-MM-DD HH24:MI:SS')
                                    GROUP BY
	                                    TRUNC(t0.""ReceiveDatetime"", 'DD')
                                    ORDER BY
	                                    TRUNC(t0.""ReceiveDatetime"", 'DD')", number, dateFrom.ToString("yyyy-MM-dd 00:00:00"), dateTo.ToString("yyyy-MM-dd 00:00:00"));
                    var ciiData = await _sqlRepository.ExecuteDataTable(sbSql.ToString());

                    sbSql.Clear();

                    //预测油耗平均值
                    sbSql.AppendFormat(@"SELECT
		                                    CAST(AVG(t2.DGO) AS DECIMAL(10,4)) AS DGO,
		                                    CAST(AVG(t2.LFO) AS DECIMAL(10,4)) AS LFO,
		                                    CAST(AVG(t2.HFO) AS DECIMAL(10,4)) AS HFO,
		                                    CAST(AVG(t2.LPG_P) AS DECIMAL(10,4)) AS LPG_P,
		                                    CAST(AVG(t2.LPG_B) AS DECIMAL(10,4)) AS LPG_B,
		                                    CAST(AVG(t2.LNG) AS DECIMAL(10,4)) AS LNG,
		                                    CAST(AVG(t2.""Methanol"") AS DECIMAL(10,4)) AS ""Methanol"",
		                                    CAST(AVG(t2.""Ethanol"") AS DECIMAL(10,4)) AS ""Ethanol""
	                                    FROM
		                                    ""vesselinfo"" t0,
		                                    ""energy_prediction"" t2
	                                    WHERE
		                                    NVL(t0.""delete_time"", TO_TIMESTAMP('1949-10-01', 'YYYY-MM-DD')) = TO_TIMESTAMP('1949-10-01', 'YYYY-MM-DD')
		                                    AND t0.SN = t2.""Number""
		                                    AND t0.""ReceiveDatetime"" = t2.""ReceiveDatetime""
		                                    AND t0.""GroundSpeed"" > 0
		                                    AND t2.""Number"" = '{0}'
		                                    AND t2.""ReceiveDatetime"" >= TO_TIMESTAMP('{1}', 'YYYY-MM-DD HH24:MI:SS')
		                                    AND t2.""ReceiveDatetime"" < TO_TIMESTAMP('{2}', 'YYYY-MM-DD HH24:MI:SS')", number, dateFrom.ToString("yyyy-MM-dd 00:00:00"), dateTo.ToString("yyyy-MM-dd 00:00:00"));
                    var preData = await _sqlRepository.ExecuteDataTable(sbSql.ToString());

                    //时间段内燃料消耗量
                    var fms1 = new List<FuelConsumption>();
                    foreach (DataRow dr in PUs.Rows)
                    {
                        foreach (DataColumn dc in PUs.Columns)
                        {
                            if (dc.ColumnName != "DeviceType")
                            {
                                fms1.Add(new FuelConsumption
                                {
                                    DeviceType = dr["DeviceType"].ToString(),
                                    FuelType = dc.ColumnName,
                                    Cons = Convert.ToDecimal(StringHelper.GetDataColumnValue(dr[dc.ColumnName])),
                                    DeviceNo = dr["DeviceType"].ToString() == "me" ? (lang == "en_US" ? "Main Engine" : "主机") : dr["DeviceType"].ToString() == "blr" ? (lang == "en_US" ? "Boiler" : "锅炉") : (lang == "en_US" ? "Auxiliary Engine" : "辅机")
                                });
                            }
                        }
                    }

                    var ciis = new List<object>();
                    //计算CII
                    if (ciiData.Rows.Count > 0)
                        for (var i = 0; i < ciiData.Rows.Count; i++)
                        {
                            var ciisParams = new
                            {
                                shipType = shipType,
                                year = DateTime.Now.Year,
                                distance = distance,
                                DGO = Convert.ToSingle(ciiData.Rows[i]["DGOAcc1"] is DBNull ? 0 : ciiData.Rows[i]["DGOAcc1"]) - Convert.ToSingle(ciiData.Rows[i]["DGOAcc2"] is DBNull ? 0 : ciiData.Rows[i]["DGOAcc2"]),
                                LFO = Convert.ToSingle(ciiData.Rows[i]["LFOAcc1"] is DBNull ? 0 : ciiData.Rows[i]["LFOAcc1"]) - Convert.ToSingle(ciiData.Rows[i]["LFOAcc2"] is DBNull ? 0 : ciiData.Rows[i]["LFOAcc2"]),
                                HFO = Convert.ToSingle(ciiData.Rows[i]["HFOAcc1"] is DBNull ? 0 : ciiData.Rows[i]["HFOAcc1"]) - Convert.ToSingle(ciiData.Rows[i]["HFOAcc2"] is DBNull ? 0 : ciiData.Rows[i]["HFOAcc2"]),
                                LPG_P = Convert.ToSingle(ciiData.Rows[i]["LPG_PAcc1"] is DBNull ? 0 : ciiData.Rows[i]["LPG_PAcc1"]) - Convert.ToSingle(ciiData.Rows[i]["LPG_PAcc2"] is DBNull ? 0 : ciiData.Rows[i]["LPG_PAcc2"]),
                                LPG_B = Convert.ToSingle(ciiData.Rows[i]["LPG_BAcc1"] is DBNull ? 0 : ciiData.Rows[i]["LPG_BAcc1"]) - Convert.ToSingle(ciiData.Rows[i]["LPG_BAcc2"] is DBNull ? 0 : ciiData.Rows[i]["LPG_BAcc2"]),
                                LNG = Convert.ToSingle(ciiData.Rows[i]["LNGAcc1"] is DBNull ? 0 : ciiData.Rows[i]["LNGAcc1"]) - Convert.ToSingle(ciiData.Rows[i]["LNGAcc2"] is DBNull ? 0 : ciiData.Rows[i]["LNGAcc2"]),
                                Methanol = Convert.ToSingle(ciiData.Rows[i]["MethanolAcc1"] is DBNull ? 0 : ciiData.Rows[i]["MethanolAcc1"]) - Convert.ToSingle(ciiData.Rows[i]["MethanolAcc2"] is DBNull ? 0 : ciiData.Rows[i]["MethanolAcc2"]),
                                Ethanol = Convert.ToSingle(ciiData.Rows[i]["EthanolAcc1"] is DBNull ? 0 : ciiData.Rows[i]["EthanolAcc1"]) - Convert.ToSingle(ciiData.Rows[i]["EthanolAcc2"] is DBNull ? 0 : ciiData.Rows[i]["EthanolAcc2"]),
                                dwt = dwt,
                                gt = gt
                            };
                            var tempCII = await _ciiService.CalcCII(ciisParams.ToJson());
                            var tempObject = tempCII.ToJson().ToJObject();
                            var jo = new JProperty("ReceiveDatetime", ciiData.Rows[i]["ReceiveDate"]);

                            tempObject.Add(jo);
                            ciis.Add(tempObject);
                        }

                    object cii = new object();
                    if (baseData.Rows.Count > 0)
                    {
                        var ciiParams = new
                        {
                            shipType = shipType,
                            dwt = dwt,
                            gt = gt,
                            year = DateTime.Now.Year,
                            distance = distance,
                            DGO = DGOC,
                            LFO = LFOC,
                            HFO = HFOC,
                            LPG_P = LPG_PC,
                            LPG_B = LPG_BC,
                            LNG = LNGC,
                            Methanol = MethanolC,
                            Ethanol = EthanolC,
                        };
                        cii = await _ciiService.CalcCII(ciiParams.ToJson());
                    }
                    else
                    {
                        var ciiParams = new
                        {
                            shipType = shipType,
                            dwt = dwt,
                            gt = gt,
                            year = DateTime.Now.Year,
                            distance = 0.00001,
                            DGO = 0,
                            LFO = 0,
                            HFO = 0,
                            LPG_P = 0,
                            LPG_B = 0,
                            LNG = 0,
                            Methanol = 0,
                            Ethanol = 0
                        };
                        cii = await _ciiService.CalcCII(ciiParams.ToJson());
                    }

                    var ciiPrediction = "--";
                    if (ciis.Count > 1)
                        ciiPrediction = (ciis[ciis.Count - 1].ToJson().ToJObject())["CIIRating"].ToString();

                    var eeoi = 0.0;
                    var dgoTW = 0.0;
                    var lfoTW = 0.0;
                    var hfoTW = 0.0;
                    var lpg_pTW = 0.0;
                    var lpg_bTW = 0.0;
                    var lngTW = 0.0;
                    var methanolTW = 0.0;
                    var ethanolTW = 0.0;
                    var cTW = 0.0;
                    var eRating = 0.0;
                    try
                    {
                        var avgdgo = Convert.ToDouble(baseData.Rows[0]["AVGDGO"]);
                        var avglfo = Convert.ToDouble(baseData.Rows[0]["AVGLFO"]);
                        var avghfo = Convert.ToDouble(baseData.Rows[0]["AVGHFO"]);
                        var avglpg_p = Convert.ToDouble(baseData.Rows[0]["AVGLPG_P"]);
                        var avglpg_b = Convert.ToDouble(baseData.Rows[0]["AVGLPG_B"]);
                        var avglng = Convert.ToDouble(baseData.Rows[0]["AVGLNG"]);
                        var avgmethanol = Convert.ToDouble(baseData.Rows[0]["AVGMethanol"]);
                        var avgethanol = Convert.ToDouble(baseData.Rows[0]["AVGEthanol"]);

                        var predgo = Convert.ToDouble(preData.Rows[0]["DGO"]);
                        var prelfo = Convert.ToDouble(preData.Rows[0]["LFO"]);
                        var prehfo = Convert.ToDouble(preData.Rows[0]["HFO"]);
                        var prelpg_p = Convert.ToDouble(preData.Rows[0]["LPG_P"]);
                        var prelpg_b = Convert.ToDouble(preData.Rows[0]["LPG_B"]);
                        var prelng = Convert.ToDouble(preData.Rows[0]["LNG"]);
                        var premethanol = Convert.ToDouble(preData.Rows[0]["Methanol"]);
                        var preethanol = Convert.ToDouble(preData.Rows[0]["Ethanol"]);

                        eeoi = (DGOC * 3.206 + LFOC * 3.151 + HFOC * 3.114 + LPG_PC * 3 + LPG_BC * 3.03 + LNGC * 2.75 + MethanolC * 1.375 + EthanolC * 1.913) * 1000 / cargoCapacity / distance;
                        dgoTW = DGOC / dwt / distance;
                        lfoTW = LFOC / dwt / distance;
                        hfoTW = HFOC / dwt / distance;
                        lpg_pTW = LPG_PC / dwt / distance;
                        lpg_bTW = LPG_BC / dwt / distance;
                        lngTW = LNGC / dwt / distance;
                        methanolTW = MethanolC / dwt / distance;
                        ethanolTW = EthanolC / dwt / distance;
                        cTW = (DGOC * 3.206 + LFOC * 3.151 + HFOC * 3.114 + LPG_PC * 3 + LPG_BC * 3.03 + LNGC * 2.75 + MethanolC * 1.375 + EthanolC * 1.913) * 1000 / dwt / distance;
                        eRating = Convert.ToDouble(baseData.Rows[0]["AVGHFO"] is DBNull ? 0 : baseData.Rows[0]["AVGHFO"]) / Convert.ToDouble(preData.Rows[0]["HFO"] is DBNull ? 9999 : (Convert.ToDouble(preData.Rows[0]["HFO"]) == 0 ? 9999 : preData.Rows[0]["HFO"]));
                    }
                    catch (Exception ex) { }
                    var eRatingStr = "";

                    if (eRating > 0.99 && eRating <= 1.09)
                        eRatingStr = lang == "en_US" ? "Normal" : "正常";
                    else if (eRating > 0.94 && eRating <= 0.99)
                        eRatingStr = lang == "en_US" ? "Good. Energy efficiency level increased by " : "良好，能效水平提高" + $"{Math.Round(100 - Convert.ToDouble(eRating) * 100, 1)}%";
                    else if (eRating <= 0.94 && eRating > 0)
                        eRatingStr = lang == "en_US" ? "Excellent. Energy efficiency level increased by " : "极佳，能效水平提高" + $"{Math.Round(100 - Convert.ToDouble(eRating) * 100, 1)}%";
                    else if (eRating == 0)
                        eRatingStr = $"--";
                    else
                        eRatingStr = lang == "en_US" ? "Bad" : "差";

                    var criteriaDto = new CriteriaDto();
                    if (StaticEntities.StaticEntities.Configs.Any(t => t.Number == number && t.IsDevice == 0 && t.Code == "CriteriaDGO"))
                    {
                        criteriaDto.DGO = (double)StaticEntities.StaticEntities.Configs.FirstOrDefault(t => t.Number == number && t.IsDevice == 0 && t.Code == "CriteriaDGO").HighLimit;
                    }
                    if (StaticEntities.StaticEntities.Configs.Any(t => t.Number == number && t.IsDevice == 0 && t.Code == "CriteriaLFO"))
                    {
                        criteriaDto.LFO = (double)StaticEntities.StaticEntities.Configs.FirstOrDefault(t => t.Number == number && t.IsDevice == 0 && t.Code == "CriteriaLFO").HighLimit;
                    }
                    if (StaticEntities.StaticEntities.Configs.Any(t => t.Number == number && t.IsDevice == 0 && t.Code == "CriteriaHFO"))
                    {
                        criteriaDto.HFO = (double)StaticEntities.StaticEntities.Configs.FirstOrDefault(t => t.Number == number && t.IsDevice == 0 && t.Code == "CriteriaHFO").HighLimit;
                    }
                    if (StaticEntities.StaticEntities.Configs.Any(t => t.Number == number && t.IsDevice == 0 && t.Code == "CriteriaLPG_P"))
                    {
                        criteriaDto.LPG_P = (double)StaticEntities.StaticEntities.Configs.FirstOrDefault(t => t.Number == number && t.IsDevice == 0 && t.Code == "CriteriaLPG_P").HighLimit;
                    }
                    if (StaticEntities.StaticEntities.Configs.Any(t => t.Number == number && t.IsDevice == 0 && t.Code == "CriteriaLPG_B"))
                    {
                        criteriaDto.LPG_B = (double)StaticEntities.StaticEntities.Configs.FirstOrDefault(t => t.Number == number && t.IsDevice == 0 && t.Code == "CriteriaLPG_B").HighLimit;
                    }
                    if (StaticEntities.StaticEntities.Configs.Any(t => t.Number == number && t.IsDevice == 0 && t.Code == "CriteriaLNG"))
                    {
                        criteriaDto.LNG = (double)StaticEntities.StaticEntities.Configs.FirstOrDefault(t => t.Number == number && t.IsDevice == 0 && t.Code == "CriteriaLNG").HighLimit;
                    }
                    if (StaticEntities.StaticEntities.Configs.Any(t => t.Number == number && t.IsDevice == 0 && t.Code == "CriteriaMethanol"))
                    {
                        criteriaDto.Methanol = (double)StaticEntities.StaticEntities.Configs.FirstOrDefault(t => t.Number == number && t.IsDevice == 0 && t.Code == "CriteriaMethanol").HighLimit;
                    }
                    if (StaticEntities.StaticEntities.Configs.Any(t => t.Number == number && t.IsDevice == 0 && t.Code == "CriteriaEthanol"))
                    {
                        criteriaDto.Ethanol = (double)StaticEntities.StaticEntities.Configs.FirstOrDefault(t => t.Number == number && t.IsDevice == 0 && t.Code == "CriteriaEthanol").HighLimit;
                    }
                    if (StaticEntities.StaticEntities.Configs.Any(t => t.Number == number && t.IsDevice == 0 && t.Code == "CriteriaPower"))
                    {
                        criteriaDto.Power = (double)StaticEntities.StaticEntities.Configs.FirstOrDefault(t => t.Number == number && t.IsDevice == 0 && t.Code == "CriteriaPower").HighLimit;
                    }
                    if (StaticEntities.StaticEntities.Configs.Any(t => t.Number == number && t.IsDevice == 0 && t.Code == "CriteriaSpeed"))
                    {
                        criteriaDto.Speed = (double)StaticEntities.StaticEntities.Configs.FirstOrDefault(t => t.Number == number && t.IsDevice == 0 && t.Code == "CriteriaSpeed").HighLimit;
                    }

                    try
                    {
                        var dgopNM = Convert.ToDouble(StringHelper.GetDataColumnValue(baseData.Rows[0]["AVGDGOpNM"]));
                        var lfopNM = Convert.ToDouble(StringHelper.GetDataColumnValue(baseData.Rows[0]["AVGLFOpNM"]));
                        var hfopNM = Convert.ToDouble(StringHelper.GetDataColumnValue(baseData.Rows[0]["AVGHFOpNM"]));
                        var lpg_ppNM = Convert.ToDouble(StringHelper.GetDataColumnValue(baseData.Rows[0]["AVGLPG_PpNM"]));
                        var lpg_bpNM = Convert.ToDouble(StringHelper.GetDataColumnValue(baseData.Rows[0]["AVGLPG_BpNM"]));
                        var lngpNM = Convert.ToDouble(StringHelper.GetDataColumnValue(baseData.Rows[0]["AVGLNGpNM"]));
                        var methanolpNM = Convert.ToDouble(StringHelper.GetDataColumnValue(baseData.Rows[0]["AVGMethanolpNM"]));
                        var ethanolpNM = Convert.ToDouble(StringHelper.GetDataColumnValue(baseData.Rows[0]["AVGEthanolpNM"]));

                        var dgopPower = Convert.ToDouble(StringHelper.GetDataColumnValue(baseData.Rows[0]["AVGDGOpPower"]));
                        var lfopPower = Convert.ToDouble(StringHelper.GetDataColumnValue(baseData.Rows[0]["AVGLFOpPower"]));
                        var hfopPower = Convert.ToDouble(StringHelper.GetDataColumnValue(baseData.Rows[0]["AVGHFOpPower"]));
                        var lpg_ppPower = Convert.ToDouble(StringHelper.GetDataColumnValue(baseData.Rows[0]["AVGLPG_PpPower"]));
                        var lpg_bpPower = Convert.ToDouble(StringHelper.GetDataColumnValue(baseData.Rows[0]["AVGLPG_BpPower"]));
                        var lngpPower = Convert.ToDouble(StringHelper.GetDataColumnValue(baseData.Rows[0]["AVGLNGpPower"]));
                        var methanolpPower = Convert.ToDouble(StringHelper.GetDataColumnValue(baseData.Rows[0]["AVGMethanolpPower"]));
                        var ethanolpPower = Convert.ToDouble(StringHelper.GetDataColumnValue(baseData.Rows[0]["AVGEthanolpPower"]));

                        var ecValue = (hfopPower / (criteriaDto.HFO / criteriaDto.Power) + hfopNM / (criteriaDto.HFO / criteriaDto.Speed)) / 2;
                        if (ecValue > 0.99 && ecValue <= 1.09)
                            eRatingStr += lang == "en_US" ? " Normal energy consumption level" : " 能耗水平正常";
                        else if (ecValue > 0.94 && ecValue <= 0.99)
                            eRatingStr += lang == "en_US" ? " Good energy consumption level, with a fuel saving rate of " : " 能耗水平良好，节油率为" + $"{Math.Round(100 - Convert.ToDouble(ecValue) * 100, 1)}%";
                        else if (ecValue <= 0.94)
                            eRatingStr += lang == "en_US" ? " Excellent energy consumption level, with a fuel saving rate of " : " 能耗水平极佳，节油率为" + $"{Math.Round(100 - Convert.ToDouble(ecValue) * 100, 1)}%";
                        else
                            eRatingStr += lang == "en_US" ? " Poor energy consumption level" : " 能耗水平差";
                    }
                    catch (Exception)
                    {
                        eRatingStr += lang == "en_US" ? " Energy consumption level:--" : " 能耗水平--";
                    }

                    var result = new
                    {
                        baseinfo = new
                        {
                            DistanceGrd = baseData.Rows.Count <= 0 ? 0 : baseData.Rows[0]["DistanceGrd"],
                            AVGSpeedGrd = baseData.Rows.Count <= 0 ? 0 : baseData.Rows[0]["AVGSpeedGrd"],
                            AVGWaterSpeed = baseData.Rows.Count <= 0 ? 0 : baseData.Rows[0]["AVGWaterSpeed"],
                            AVGDGO = baseData.Rows.Count <= 0 ? 0 : baseData.Rows[0]["AVGDGO"],
                            AVGLFO = baseData.Rows.Count <= 0 ? 0 : baseData.Rows[0]["AVGLFO"],
                            AVGHFO = baseData.Rows.Count <= 0 ? 0 : baseData.Rows[0]["AVGHFO"],
                            AVGLPG_P = baseData.Rows.Count <= 0 ? 0 : baseData.Rows[0]["AVGLPG_P"],
                            AVGLPG_B = baseData.Rows.Count <= 0 ? 0 : baseData.Rows[0]["AVGLPG_B"],
                            AVGLNG = baseData.Rows.Count <= 0 ? 0 : baseData.Rows[0]["AVGLNG"],
                            AVGMethanol = baseData.Rows.Count <= 0 ? 0 : baseData.Rows[0]["AVGMethanol"],
                            AVGEthanol = baseData.Rows.Count <= 0 ? 0 : baseData.Rows[0]["AVGEthanol"],
                            AVGPower = baseData.Rows.Count <= 0 ? 0 : baseData.Rows[0]["AVGPower"],
                            RunTime = baseData.Rows.Count <= 0 ? 0 : baseData.Rows[0]["RunTime"],
                            DGOACC = DGOC,
                            LFOACC = LFOC,
                            HFOACC = HFOC,
                            LPG_PACC = LPG_PC,
                            LPG_BACC = LPG_BC,
                            LNGACC = LNGC,
                            MethanolACC = MethanolC,
                            EthanolACC = EthanolC,
                            CalcSpeedGrd = baseData.Rows.Count <= 0 ? 0 : baseData.Rows[0]["AVGSpeedGrd"],
                            AVGDGOpNM = baseData.Rows.Count <= 0 ? 0 : baseData.Rows[0]["AVGDGOpNM"],
                            AVGLFOpNM = baseData.Rows.Count <= 0 ? 0 : baseData.Rows[0]["AVGLFOpNM"],
                            AVGHFOpNM = baseData.Rows.Count <= 0 ? 0 : baseData.Rows[0]["AVGHFOpNM"],
                            AVGLPG_PpNM = baseData.Rows.Count <= 0 ? 0 : baseData.Rows[0]["AVGLPG_PpNM"],
                            AVGLPG_BpNM = baseData.Rows.Count <= 0 ? 0 : baseData.Rows[0]["AVGLPG_BpNM"],
                            AVGLNGpNM = baseData.Rows.Count <= 0 ? 0 : baseData.Rows[0]["AVGLNGpNM"],
                            AVGMethanolpNM = baseData.Rows.Count <= 0 ? 0 : baseData.Rows[0]["AVGMethanolpNM"],
                            AVGEthanolpNM = baseData.Rows.Count <= 0 ? 0 : baseData.Rows[0]["AVGEthanolpNM"],
                            AVGDGOpPower = baseData.Rows.Count <= 0 ? 0 : baseData.Rows[0]["AVGDGOpPower"],
                            AVGLFOpPower = baseData.Rows.Count <= 0 ? 0 : baseData.Rows[0]["AVGLFOpPower"],
                            AVGHFOpPower = baseData.Rows.Count <= 0 ? 0 : baseData.Rows[0]["AVGHFOpPower"],
                            AVGLPG_PpPower = baseData.Rows.Count <= 0 ? 0 : baseData.Rows[0]["AVGLPG_PpPower"],
                            AVGLPG_BpPower = baseData.Rows.Count <= 0 ? 0 : baseData.Rows[0]["AVGLPG_BpPower"],
                            AVGLNGpPower = baseData.Rows.Count <= 0 ? 0 : baseData.Rows[0]["AVGLNGpPower"],
                            AVGMethanolpPower = baseData.Rows.Count <= 0 ? 0 : baseData.Rows[0]["AVGMethanolpPower"],
                            AVGEthanolpPower = baseData.Rows.Count <= 0 ? 0 : baseData.Rows[0]["AVGEthanolpPower"],
                            EEOI = Math.Round(eeoi, 4),
                            HFOTw = Math.Round(hfoTW, 4),
                            MethanolTw = Math.Round(methanolTW, 4),
                            CO2Tw = Math.Round(cTW, 4),
                            Estimate = eRatingStr
                        },
                        cii = cii,
                        flowmeters = fms1,
                        abnormal = abnormalData,
                        ciis = ciis,
                        sfoc = eeData,
                        ciiPrediction = ciiPrediction
                    };

                    return result;

                    #endregion 其他船舶
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 获取能量消耗分布信息
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<EnergyDistribution> GetEnergyDistributionAsync(string parameters)
        {
            var queryParams = parameters.ToJObject();
            var number = "";
            number = queryParams["number"].ToString();
            var ed = new EnergyDistribution();
            //if (StaticEntity.RealtimeVesselinfos.ContainsKey(number))
            //{
            //主机
            var Nme = 0m;
            if (queryParams.ContainsKey("nme") && !string.IsNullOrWhiteSpace(queryParams["nme"].ToString()))
                Nme = Convert.ToDecimal(queryParams["nme"]);
            var gme = 0m;
            if (queryParams.ContainsKey("gme") && !string.IsNullOrWhiteSpace(queryParams["gme"].ToString()))
                gme = Convert.ToDecimal(queryParams["gme"]);
            var tme = 0m;
            if (queryParams.ContainsKey("tme") && !string.IsNullOrWhiteSpace(queryParams["tme"].ToString()))
                tme = Convert.ToDecimal(queryParams["tme"]);
            var Kme = 0m;
            if (queryParams.ContainsKey("kme") && !string.IsNullOrWhiteSpace(queryParams["kme"].ToString()))
                Kme = Convert.ToDecimal(queryParams["kme"]);
            ed.Et = ed.Emei = Nme * gme * tme * Kme * 4m;
            if (StaticEntities.StaticEntities.TotalIndicators.Any(t => t.Number == number))
            {
                var tempEntity = StaticEntities.StaticEntities.TotalIndicators.FirstOrDefault(t => t.Number == number);
                ed.Et = ed.Emei = (tempEntity.HFO ?? 0) * 40.176m + (tempEntity.Methanol ?? 0) * 20.088m;
            }

            //排烟
            var C = 0m;
            if (queryParams.ContainsKey("c") && !string.IsNullOrWhiteSpace(queryParams["c"].ToString()))
                C = Convert.ToDecimal(queryParams["c"]);
            var Tmeo = 0m;
            if (queryParams.ContainsKey("tmeo") && !string.IsNullOrWhiteSpace(queryParams["tmeo"].ToString()))
                Tmeo = Convert.ToDecimal(queryParams["tmeo"]);
            var Tmei = 0m;
            if (queryParams.ContainsKey("tmei") && !string.IsNullOrWhiteSpace(queryParams["tmei"].ToString()))
                Tmei = Convert.ToDecimal(queryParams["tmei"]);
            ed.Emes = 3600m * 0.0022m * Nme * C * (Tmeo - Tmei) * 4m;
            if (StaticEntities.StaticEntities.TotalIndicators.Any(t => t.Number == number))
            {
                var tempEntity = StaticEntities.StaticEntities.TotalIndicators.FirstOrDefault(t => t.Number == number);
                ed.Emes = ed.Emei * 0.2369687m;
            }

            //冷却
            var Nmeac = 0m;
            if (queryParams.ContainsKey("nmeac") && !string.IsNullOrWhiteSpace(queryParams["nmeac"].ToString()))
                Nmeac = Convert.ToDecimal(queryParams["nmeac"]);
            var Nmejc = 0m;
            if (queryParams.ContainsKey("nmejc") && !string.IsNullOrWhiteSpace(queryParams["nmejc"].ToString()))
                Nmejc = Convert.ToDecimal(queryParams["nmejc"]);
            var Nmelc = 0m;
            if (queryParams.ContainsKey("nmelc") && !string.IsNullOrWhiteSpace(queryParams["nmelc"].ToString()))
                Nmelc = Convert.ToDecimal(queryParams["nmelc"]);
            ed.Emec = 3600m * (Nmeac + Nmejc + Nmelc) * 4m;
            if (StaticEntities.StaticEntities.TotalIndicators.Any(t => t.Number == number))
            {
                var tempEntity = StaticEntities.StaticEntities.TotalIndicators.FirstOrDefault(t => t.Number == number);
                ed.Emec = ed.Emei * 0.2101043m;
            }

            //其他损失
            ed.Emeol = 0m;
            if (StaticEntities.StaticEntities.TotalIndicators.Any(t => t.Number == number))
            {
                var tempEntity = StaticEntities.StaticEntities.TotalIndicators.FirstOrDefault(t => t.Number == number);
                ed.Emeol = ed.Emei * (1 - 0.4430634m - 0.2369687m - 0.2101043m);
            }

            //发电机
            var Nae = 0m;
            if (queryParams.ContainsKey("nae") && !string.IsNullOrWhiteSpace(queryParams["nae"].ToString()))
                Nae = Convert.ToDecimal(queryParams["nae"]);
            var gae = 0m;
            if (queryParams.ContainsKey("gae") && !string.IsNullOrWhiteSpace(queryParams["gae"].ToString()))
                gae = Convert.ToDecimal(queryParams["gae"]);
            var tae = 0m;
            if (queryParams.ContainsKey("tae") && !string.IsNullOrWhiteSpace(queryParams["tae"].ToString()))
                tae = Convert.ToDecimal(queryParams["tae"]);
            var Kae = 0m;
            if (queryParams.ContainsKey("kae") && !string.IsNullOrWhiteSpace(queryParams["kae"].ToString()))
                Kae = Convert.ToDecimal(queryParams["kae"]);
            ed.Eaei = Nae * gae * tae * Kae * 4m;
            if (StaticEntities.StaticEntities.TotalIndicators.Any(t => t.Number == number))
            {
                var tempEntity = StaticEntities.StaticEntities.TotalIndicators.FirstOrDefault(t => t.Number == number);
                ed.Eaei = ed.Emei * 0.4430634m;
            }

            //排烟损失
            if (StaticEntities.StaticEntities.TotalIndicators.Any(t => t.Number == number))
            {
                var tempEntity = StaticEntities.StaticEntities.TotalIndicators.FirstOrDefault(t => t.Number == number);
                ed.Epssl = ed.Emes * 0.671179m;
            }

            //传动系统
            ed.Emeo = 3600m * Nae * tae * 4m;
            if (StaticEntities.StaticEntities.TotalIndicators.Any(t => t.Number == number))
            {
                var tempEntity = StaticEntities.StaticEntities.TotalIndicators.FirstOrDefault(t => t.Number == number);
                ed.Emeo = ed.Eaei * 0.5552351m;
            }

            //船舶电网
            var Naeo = 0m;
            if (queryParams.ContainsKey("naeo") && !string.IsNullOrWhiteSpace(queryParams["naeo"].ToString()))
                Naeo = Convert.ToDecimal(queryParams["naeo"]);
            ed.Eaeeni = 3600m * Naeo * tae * 4m;
            if (StaticEntities.StaticEntities.TotalIndicators.Any(t => t.Number == number))
            {
                var tempEntity = StaticEntities.StaticEntities.TotalIndicators.FirstOrDefault(t => t.Number == number);
                ed.Eaeeni = ed.Eaei * 0.3852351m;
            }

            //电站其他损失
            if (StaticEntities.StaticEntities.TotalIndicators.Any(t => t.Number == number))
            {
                var tempEntity = StaticEntities.StaticEntities.TotalIndicators.FirstOrDefault(t => t.Number == number);
                ed.Eael = ed.Eaei * 0.0595298m;
            }

            //冷却损失
            var Naeac = 0m;
            if (queryParams.ContainsKey("naeac") && !string.IsNullOrWhiteSpace(queryParams["naeac"].ToString()))
                Naeac = Convert.ToDecimal(queryParams["naeac"]);
            var Naejc = 0m;
            if (queryParams.ContainsKey("naejc") && !string.IsNullOrWhiteSpace(queryParams["naejc"].ToString()))
                Naejc = Convert.ToDecimal(queryParams["naejc"]);
            var Naelc = 0m;
            if (queryParams.ContainsKey("naelc") && !string.IsNullOrWhiteSpace(queryParams["naelc"].ToString()))
                Naelc = Convert.ToDecimal(queryParams["naelc"]);
            ed.Eaedc = 3600m * Nae * (Naeac + Naejc + Naelc) * tae * 4m;
            if (StaticEntities.StaticEntities.TotalIndicators.Any(t => t.Number == number))
            {
                var tempEntity = StaticEntities.StaticEntities.TotalIndicators.FirstOrDefault(t => t.Number == number);
                ed.Eaedc = ed.Emei * 0.2101043m;
            }

            //螺旋桨
            var ηmegb = 0m;
            if (queryParams.ContainsKey("etamegb") && !string.IsNullOrWhiteSpace(queryParams["etamegb"].ToString()))
                ηmegb = Convert.ToDecimal(queryParams["etamegb"]);
            var ηmeb = 0m;
            if (queryParams.ContainsKey("etameb") && !string.IsNullOrWhiteSpace(queryParams["etameb"].ToString()))
                ηmeb = Convert.ToDecimal(queryParams["etameb"]);
            ed.Emetse = 3600m * Nae * ηmegb * ηmeb * tae * 4m;
            if (StaticEntities.StaticEntities.TotalIndicators.Any(t => t.Number == number))
            {
                var tempEntity = StaticEntities.StaticEntities.TotalIndicators.FirstOrDefault(t => t.Number == number);
                ed.Emetse = ed.Emeo * 0.9405732m;
            }

            //传动损失
            ed.Emetsl = 3600m * Nae * (1 - ηmegb * ηmeb) * tae * 4m;
            if (StaticEntities.StaticEntities.TotalIndicators.Any(t => t.Number == number))
            {
                var tempEntity = StaticEntities.StaticEntities.TotalIndicators.FirstOrDefault(t => t.Number == number);
                ed.Emetsl = ed.Emeo * 0.0594268m;
            }

            //电力负载
            var ηaeen = 0m;
            if (queryParams.ContainsKey("etaaeen") && !string.IsNullOrWhiteSpace(queryParams["etaaeen"].ToString()))
                ηaeen = Convert.ToDecimal(queryParams["etaaeen"]);
            ed.Eaeepli = 3600M * Naeo * ηaeen * tae * 4m;
            if (StaticEntities.StaticEntities.TotalIndicators.Any(t => t.Number == number))
            {
                var tempEntity = StaticEntities.StaticEntities.TotalIndicators.FirstOrDefault(t => t.Number == number);
                ed.Eaeepli = ed.Eaeeni * 0.9493088m;
            }

            //电网损失
            ed.Eaeenl = 3600M * Naeo * (1 - ηaeen) * tae * 4m;
            if (StaticEntities.StaticEntities.TotalIndicators.Any(t => t.Number == number))
            {
                var tempEntity = StaticEntities.StaticEntities.TotalIndicators.FirstOrDefault(t => t.Number == number);
                ed.Eaeenl = ed.Eaeeni * 0.0506912m;
            }

            //推进损失
            var ηmepe = 0m;
            if (queryParams.ContainsKey("etamepe") && !string.IsNullOrWhiteSpace(queryParams["etamepe"].ToString()))
                ηmepe = Convert.ToDecimal(queryParams["etamepe"]);
            ed.Emepel = 3600m * Nae * tae * ηmegb * ηmeb * (1 - ηmepe) * 4m;
            if (StaticEntities.StaticEntities.TotalIndicators.Any(t => t.Number == number))
            {
                var tempEntity = StaticEntities.StaticEntities.TotalIndicators.FirstOrDefault(t => t.Number == number);
                ed.Emepel = ed.Emetse * 0.5m;
            }

            //负载损失
            var ηaeepl = 0m;
            if (queryParams.ContainsKey("etaaeepl") && !string.IsNullOrWhiteSpace(queryParams["etaaeepl"].ToString()))
                ηaeepl = Convert.ToDecimal(queryParams["etaaeepl"]);
            ed.Eaeepll = 3600m * Naeo * ηaeen * tae * (1 - ηaeepl) * 4m;
            if (StaticEntities.StaticEntities.TotalIndicators.Any(t => t.Number == number))
            {
                var tempEntity = StaticEntities.StaticEntities.TotalIndicators.FirstOrDefault(t => t.Number == number);
                ed.Eaeepll = ed.Eaeepli * 0.1003236m;
            }

            //主机T/C回收
            var Tmetci = 0m;
            if (queryParams.ContainsKey("tmetci") && !string.IsNullOrWhiteSpace(queryParams["tmetci"].ToString()))
                Tmetci = Convert.ToDecimal(queryParams["tmetci"]);
            var Tmetco = 0m;
            if (queryParams.ContainsKey("tmetco") && !string.IsNullOrWhiteSpace(queryParams["tmetco"].ToString()))
                Tmetco = Convert.ToDecimal(queryParams["tmetco"]);
            ed.Emetc = 3600m * 0.0022m * Nme * C * (Tmetci - Tmetco) * tme * 4m;
            if (StaticEntities.StaticEntities.TotalIndicators.Any(t => t.Number == number))
            {
                var tempEntity = StaticEntities.StaticEntities.TotalIndicators.FirstOrDefault(t => t.Number == number);
                ed.Emetc = ed.Emes * 0.328821m;
            }

            //推进做功
            ed.Wmepee = 3600M * Nme * tme * ηmegb * ηmeb * ηmepe * 4m;
            if (StaticEntities.StaticEntities.TotalIndicators.Any(t => t.Number == number))
            {
                var tempEntity = StaticEntities.StaticEntities.TotalIndicators.FirstOrDefault(t => t.Number == number);
                ed.Wmepee = ed.Emetse * 0.5m;
            }

            //发电机T/C回收
            var Taetci = 0m;
            if (queryParams.ContainsKey("taetci") && !string.IsNullOrWhiteSpace(queryParams["taetci"].ToString()))
                Taetci = Convert.ToDecimal(queryParams["taetci"]);
            var Taetco = 0m;
            if (queryParams.ContainsKey("taetco") && !string.IsNullOrWhiteSpace(queryParams["taetco"].ToString()))
                Taetco = Convert.ToDecimal(queryParams["taetco"]);
            ed.Eaetc = 3600m * 0.0022m * Nae * C * (Taetci - Taetco) * tae * 4m;
            if (StaticEntities.StaticEntities.TotalIndicators.Any(t => t.Number == number))
            {
                var tempEntity = StaticEntities.StaticEntities.TotalIndicators.FirstOrDefault(t => t.Number == number);
                ed.Eaetc = 0;
            }

            //日用设备
            ed.Waee = 3600m * Naeo * ηaeen * ηaeepl * tae * 4m;
            if (StaticEntities.StaticEntities.TotalIndicators.Any(t => t.Number == number))
            {
                var tempEntity = StaticEntities.StaticEntities.TotalIndicators.FirstOrDefault(t => t.Number == number);
                ed.Waee = ed.Eaeepli * 0.6m;
            }

            //锂电池充电
            ed.Waech = 3600m * Naeo * ηaeen * ηaeepl * tae * 4m;//需要在乘以个充电转换效率
            if (StaticEntities.StaticEntities.TotalIndicators.Any(t => t.Number == number))
            {
                var tempEntity = StaticEntities.StaticEntities.TotalIndicators.FirstOrDefault(t => t.Number == number);
                ed.Waech = ed.Eaeepli * 0.2996764m;
            }
            //}

            //有效做功
            if (StaticEntities.StaticEntities.TotalIndicators.Any(t => t.Number == number))
            {
                var tempEntity = StaticEntities.StaticEntities.TotalIndicators.FirstOrDefault(t => t.Number == number);
                ed.Eo = ed.Emetc + ed.Wmepee + ed.Eaetc + ed.Waee + ed.Waech;
            }

            return ed;
        }

        public async Task<object> GetMRVAsync(string parameters)
        {
            var queryParams = parameters.ToJObject();
            var number = "";
            var dateFrom = "";
            var dateTo = "";
            var DWT = 0f;
            var GT = 0f;
            var shipType = "";
            number = queryParams["number"].ToString();
            if (queryParams.ContainsKey("dateFrom") && !string.IsNullOrWhiteSpace(queryParams["dateFrom"].ToString()))
            {
                dateFrom = Convert.ToDateTime(queryParams["dateFrom"]).ToString("yyyy-MM-dd HH:mm:ss");
            }
            if (queryParams.ContainsKey("dateTo") && !string.IsNullOrWhiteSpace(queryParams["dateTo"].ToString()))
            {
                dateTo = Convert.ToDateTime(queryParams["dateTo"]).ToString("yyyy-MM-dd HH:mm:ss");
            }
            if (queryParams.ContainsKey("dwt") && !string.IsNullOrWhiteSpace(queryParams["dwt"].ToString()))
            {
                DWT = Convert.ToSingle(queryParams["dwt"]);
            }
            if (queryParams.ContainsKey("gt") && !string.IsNullOrWhiteSpace(queryParams["gt"].ToString()))
            {
                GT = Convert.ToSingle(queryParams["gt"]);
            }
            if (queryParams.ContainsKey("shipType") && !string.IsNullOrWhiteSpace(queryParams["shipType"].ToString()))
            {
                shipType = queryParams["shipType"].ToString();
            }

            StringBuilder sbSql = new StringBuilder();

            //起始日期流量计总量
            sbSql.AppendFormat(@"SELECT
	                                MAX( t1.""DGOAccumulated"" ) - MIN( t1.""DGOAccumulated"" ) AS ""chaiType"",
	                                MAX( t1.""LFOAccumulated"" ) - MIN( t1.""LFOAccumulated"" ) AS ""lfoType"",
	                                MAX( t1.""HFOAccumulated"" ) - MIN( t1.""HFOAccumulated"" ) AS ""hfoType"",
	                                MAX( t1.""LPG_PAccumulated"" ) - MIN( t1.""LPG_PAccumulated"" ) AS ""bingType"",
	                                MAX( t1.""LPG_BAccumulated"" ) - MIN( t1.""LPG_BAccumulated"" ) AS ""dingType"",
	                                MAX( t1.""LNGAccumulated"" ) - MIN( t1.""LNGAccumulated"" ) AS ""tianType"",
	                                MAX( t1.""MethanolAccumulated"" ) - MIN( t1.""MethanolAccumulated"" ) AS ""jiaType"",
	                                MAX( t1.""EthanolAccumulated"" ) - MIN( t1.""EthanolAccumulated"" ) AS ""yiType"",
	                                MAX( t0.""TotalDistanceGrd"" ) - MIN( t0.""TotalDistanceGrd"" ) AS ""distanceTotal""
                                FROM
	                                ""vesselinfo"" t0,
	                                ""energy_totalindicator"" t1
                                WHERE
	                                NVL( t0.""delete_time"", TO_TIMESTAMP( '1949-10-01', 'YYYY-MM-DD' ) ) = TO_TIMESTAMP( '1949-10-01', 'YYYY-MM-DD' )
	                                AND t0.SN = t1.""Number""
	                                AND t0.""ReceiveDatetime"" = t1.""ReceiveDatetime""
	                                AND t0.SN = '{0}' ", number);
            if (!string.IsNullOrWhiteSpace(dateFrom))
                sbSql.AppendFormat(@"AND t0.""ReceiveDatetime"" >= TO_TIMESTAMP( '{0}', 'YYYY-MM-DD HH24:MI:SS' ) ", dateFrom);
            if (!string.IsNullOrWhiteSpace(dateTo))
                sbSql.AppendFormat(@"AND t0.""ReceiveDatetime"" < TO_TIMESTAMP( '{0}', 'YYYY-MM-DD HH24:MI:SS' ) ", dateTo);
            sbSql.AppendFormat(@"GROUP BY
	                                t0.SN");
            var dt = await _sqlRepository.ExecuteDataTable(sbSql.ToString());
            var tw = 0f;
            if (dt != null && dt.Rows.Count > 0)
            {
                if (shipType.Contains("LNG carrier") ||
                    shipType.Contains("bulk carrier") ||
                    shipType.Contains("combination carrier") ||
                    shipType.Contains("container ship") ||
                    shipType.Contains("gas carrier") ||
                    shipType.Contains("general cargo ship") ||
                    shipType.Contains("refrigerated cargo carrier") ||
                    shipType.Contains("tanker"))
                    tw = Convert.ToSingle(dt.Rows[0]["distanceTotal"]) * DWT;
                else
                    tw = Convert.ToSingle(dt.Rows[0]["distanceTotal"]) * GT;

                var result = new
                {
                    hfoType = dt.Rows[0]["hfoType"].ToString(),
                    lfoType = dt.Rows[0]["lfoType"].ToString(),
                    chaiType = dt.Rows[0]["chaiType"].ToString(),
                    jiaType = dt.Rows[0]["jiaType"].ToString(),
                    yiType = dt.Rows[0]["yiType"].ToString(),
                    bingType = dt.Rows[0]["bingType"].ToString(),
                    dingType = dt.Rows[0]["dingType"].ToString(),
                    tianType = dt.Rows[0]["tianType"].ToString(),
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
                    distanceTotal = dt.Rows[0]["distanceTotal"].ToString(),
                    ditincelceTotal = "",
                    timeTotal = "",
                    timelce = "",
                    cargozwTotal = tw.ToString(),
                    secondParam1 = "",
                    averDensity = "",
                    oilDistance = Math.Round((Convert.ToSingle(dt.Rows[0]["hfoType"] == DBNull.Value ? null : dt.Rows[0]["hfoType"]) + Convert.ToSingle(dt.Rows[0]["lfoType"] == DBNull.Value ? null : dt.Rows[0]["lfoType"]) + Convert.ToSingle(dt.Rows[0]["chaiType"] == DBNull.Value ? null : dt.Rows[0]["chaiType"]) + Convert.ToSingle(dt.Rows[0]["jiaType"] == DBNull.Value ? null : dt.Rows[0]["jiaType"]) + Convert.ToSingle(dt.Rows[0]["yiType"] == DBNull.Value ? null : dt.Rows[0]["yiType"]) + Convert.ToSingle(dt.Rows[0]["bingType"] == DBNull.Value ? null : dt.Rows[0]["bingType"]) + Convert.ToSingle(dt.Rows[0]["dingType"] == DBNull.Value ? null : dt.Rows[0]["dingType"]) + Convert.ToSingle(dt.Rows[0]["tianType"] == DBNull.Value ? null : dt.Rows[0]["tianType"])) / Convert.ToSingle(dt.Rows[0]["distanceTotal"]), 4).ToString(),
                    oilTonDistance = Math.Round((Convert.ToSingle(dt.Rows[0]["hfoType"] == DBNull.Value ? null : dt.Rows[0]["hfoType"]) + Convert.ToSingle(dt.Rows[0]["lfoType"] == DBNull.Value ? null : dt.Rows[0]["lfoType"]) + Convert.ToSingle(dt.Rows[0]["chaiType"] == DBNull.Value ? null : dt.Rows[0]["chaiType"]) + Convert.ToSingle(dt.Rows[0]["jiaType"] == DBNull.Value ? null : dt.Rows[0]["jiaType"]) + Convert.ToSingle(dt.Rows[0]["yiType"] == DBNull.Value ? null : dt.Rows[0]["yiType"]) + Convert.ToSingle(dt.Rows[0]["bingType"] == DBNull.Value ? null : dt.Rows[0]["bingType"]) + Convert.ToSingle(dt.Rows[0]["dingType"] == DBNull.Value ? null : dt.Rows[0]["dingType"]) + Convert.ToSingle(dt.Rows[0]["tianType"] == DBNull.Value ? null : dt.Rows[0]["tianType"])) / tw, 4),
                    co2Distance = Math.Round((Convert.ToSingle(dt.Rows[0]["hfoType"] == DBNull.Value ? null : dt.Rows[0]["hfoType"]) * 3.114 + Convert.ToSingle(dt.Rows[0]["lfoType"] == DBNull.Value ? null : dt.Rows[0]["lfoType"]) * 3.151 + Convert.ToSingle(dt.Rows[0]["chaiType"] == DBNull.Value ? null : dt.Rows[0]["chaiType"]) * 3.206 + Convert.ToSingle(dt.Rows[0]["jiaType"] == DBNull.Value ? null : dt.Rows[0]["jiaType"]) * 1.375 + Convert.ToSingle(dt.Rows[0]["yiType"] == DBNull.Value ? null : dt.Rows[0]["yiType"]) * 1.913 + Convert.ToSingle(dt.Rows[0]["bingType"] == DBNull.Value ? null : dt.Rows[0]["bingType"]) * 3 + Convert.ToSingle(dt.Rows[0]["dingType"] == DBNull.Value ? null : dt.Rows[0]["dingType"]) * 3.03 + Convert.ToSingle(dt.Rows[0]["tianType"] == DBNull.Value ? null : dt.Rows[0]["tianType"]) * 2.75) / Convert.ToSingle(dt.Rows[0]["distanceTotal"]), 4).ToString(),
                    co2TonDistance = Math.Round((Convert.ToSingle(dt.Rows[0]["hfoType"] == DBNull.Value ? null : dt.Rows[0]["hfoType"]) * 3.114 + Convert.ToSingle(dt.Rows[0]["lfoType"] == DBNull.Value ? null : dt.Rows[0]["lfoType"]) * 3.151 + Convert.ToSingle(dt.Rows[0]["chaiType"] == DBNull.Value ? null : dt.Rows[0]["chaiType"]) * 3.206 + Convert.ToSingle(dt.Rows[0]["jiaType"] == DBNull.Value ? null : dt.Rows[0]["jiaType"]) * 1.375 + Convert.ToSingle(dt.Rows[0]["yiType"] == DBNull.Value ? null : dt.Rows[0]["yiType"]) * 1.913 + Convert.ToSingle(dt.Rows[0]["bingType"] == DBNull.Value ? null : dt.Rows[0]["bingType"]) * 3 + Convert.ToSingle(dt.Rows[0]["dingType"] == DBNull.Value ? null : dt.Rows[0]["dingType"]) * 3.03 + Convert.ToSingle(dt.Rows[0]["tianType"] == DBNull.Value ? null : dt.Rows[0]["tianType"]) * 2.75) / tw, 4).ToString(),
                    secondParam2 = "",
                    oiff = "",
                    addition = "",
                };
                return result;
            }
            return null;
        }

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
        public async Task SaveVesselAllAsync(VesselInfoDto vesselInfo, TotalIndicatorDto totalIndicator, PredictionDto prediction, IList<BatteryDto> batteries, IList<FlowmeterDto> flowmeters, IList<GeneratorDto> generators, IList<LiquidLevelDto> liquidLevels, IList<ShaftDto> shafts, IList<SternSealingDto> sternSealings, IList<SupplyUnitDto> supplyUnits, IList<PowerUnitDto> powerUnits)
        {
            try
            {
                await _repository.InsertAsync(ObjectMapper.Map<VesselInfoDto, VesselInfo>(vesselInfo));
                await _totalIndicatorRepository.InsertAsync(ObjectMapper.Map<TotalIndicatorDto, TotalIndicator>(totalIndicator));
                await _predictionRepository.InsertAsync(ObjectMapper.Map<PredictionDto, Prediction>(prediction));
                await _batteryRepository.InsertManyAsync(ObjectMapper.Map<IList<BatteryDto>, IList<Battery>>(batteries));
                await _flowmeterRepository.InsertManyAsync(ObjectMapper.Map<IList<FlowmeterDto>, IList<Flowmeter>>(flowmeters));
                await _generatorRepository.InsertManyAsync(ObjectMapper.Map<IList<GeneratorDto>, IList<Generator>>(generators));
                await _liquidLevelRepository.InsertManyAsync(ObjectMapper.Map<IList<LiquidLevelDto>, IList<LiquidLevel>>(liquidLevels));
                await _shaftRepository.InsertManyAsync(ObjectMapper.Map<IList<ShaftDto>, IList<Shaft>>(shafts));
                await _sternSealingRepository.InsertManyAsync(ObjectMapper.Map<IList<SternSealingDto>, IList<SternSealing>>(sternSealings));
                await _supplyUnitRepository.InsertManyAsync(ObjectMapper.Map<IList<SupplyUnitDto>, IList<SupplyUnit>>(supplyUnits));
                await _powerUnitRepository.InsertManyAsync(ObjectMapper.Map<IList<PowerUnitDto>, IList<PowerUnit>>(powerUnits));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "船舶数据存储错误=>" + MethodBase.GetCurrentMethod().DeclaringType.Namespace + "_" + MethodBase.GetCurrentMethod().Name);
                throw;
            }
        }

        public async Task SaveERAllAsync(IList<CompositeBoilerDto> CompositeBoilers, IList<CompressedAirSupplyDto> CompressedAirSupplys, IList<CoolingFreshWaterDto> CoolingFreshWaters, IList<CoolingSeaWaterDto> CoolingSeaWaters, IList<CoolingWaterDto> CoolingWaters, IList<CylinderLubOilDto> CylinderLubOils, IList<ExhaustGasDto> ExhaustGases, IList<FODto> FOs, IList<FOSupplyUnitDto> FOSupplyUnits, IList<LubOilPurifyingDto> LubOilPurifyings, IList<LubOilDto> LubOils, IList<MainGeneratorSetDto> MainGeneratorSets, IList<MainSwitchboardDto> MainSwitchboards, IList<MERemoteControlDto> MERemoteControls, IList<MiscellaneousDto> Miscellaneouses, IList<ScavengeAirDto> ScavengeAirs, IList<ShaftClutchDto> ShaftClutchs)
        {
            try
            {
                await _compositeBoilerRepository.InsertManyAsync(ObjectMapper.Map<IList<CompositeBoilerDto>, IList<CompositeBoiler>>(CompositeBoilers));
                await _compressedAirSupplyRepository.InsertManyAsync(ObjectMapper.Map<IList<CompressedAirSupplyDto>, IList<CompressedAirSupply>>(CompressedAirSupplys));
                await _coolingFreshWaterRepository.InsertManyAsync(ObjectMapper.Map<IList<CoolingFreshWaterDto>, IList<CoolingFreshWater>>(CoolingFreshWaters));
                await _coolingSeaWaterRepository.InsertManyAsync(ObjectMapper.Map<IList<CoolingSeaWaterDto>, IList<CoolingSeaWater>>(CoolingSeaWaters));
                await _coolingWaterRepository.InsertManyAsync(ObjectMapper.Map<IList<CoolingWaterDto>, IList<CoolingWater>>(CoolingWaters));
                await _cylinderLubOilRepository.InsertManyAsync(ObjectMapper.Map<IList<CylinderLubOilDto>, IList<CylinderLubOil>>(CylinderLubOils));
                await _exhaustGasRepository.InsertManyAsync(ObjectMapper.Map<IList<ExhaustGasDto>, IList<ExhaustGas>>(ExhaustGases));
                await _fORepository.InsertManyAsync(ObjectMapper.Map<IList<FODto>, IList<FO>>(FOs));
                await _fOSupplyUnitRepository.InsertManyAsync(ObjectMapper.Map<IList<FOSupplyUnitDto>, IList<FOSupplyUnit>>(FOSupplyUnits));
                await _lubOilPurifyingRepository.InsertManyAsync(ObjectMapper.Map<IList<LubOilPurifyingDto>, IList<LubOilPurifying>>(LubOilPurifyings));
                await _lubOilRepository.InsertManyAsync(ObjectMapper.Map<IList<LubOilDto>, IList<LubOil>>(LubOils));
                await _mainGeneratorSetRepository.InsertManyAsync(ObjectMapper.Map<IList<MainGeneratorSetDto>, IList<MainGeneratorSet>>(MainGeneratorSets));
                await _mainSwitchboardRepository.InsertManyAsync(ObjectMapper.Map<IList<MainSwitchboardDto>, IList<MainSwitchboard>>(MainSwitchboards));
                await _mERemoteControlRepository.InsertManyAsync(ObjectMapper.Map<IList<MERemoteControlDto>, IList<MERemoteControl>>(MERemoteControls));
                await _miscellaneousRepository.InsertManyAsync(ObjectMapper.Map<IList<MiscellaneousDto>, IList<Miscellaneous>>(Miscellaneouses));
                await _scavengeAirRepository.InsertManyAsync(ObjectMapper.Map<IList<ScavengeAirDto>, IList<ScavengeAir>>(ScavengeAirs));
                await _shaftClutchRepository.InsertManyAsync(ObjectMapper.Map<IList<ShaftClutchDto>, IList<ShaftClutch>>(ShaftClutchs));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "机舱数据存储错误=>" + MethodBase.GetCurrentMethod().DeclaringType.Namespace + "_" + MethodBase.GetCurrentMethod().Name);
                throw;
            }
        }

        public async Task SaveERADAsync(VesselInfoDto vesselInfo, IList<AssistantDecisionDto> AssistantDecisions)
        {
            try
            {
                foreach (var dto in AssistantDecisions)
                {
                    dto.ReceiveDatetime = vesselInfo.ReceiveDatetime;
                }
                if (AssistantDecisions != null)
                    await _assistantDecisionRepository.InsertManyAsync(ObjectMapper.Map<IList<AssistantDecisionDto>, IList<AssistantDecision>>(AssistantDecisions));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "机舱分析数据存储错误=>" + MethodBase.GetCurrentMethod().DeclaringType.Namespace + "_" + MethodBase.GetCurrentMethod().Name);
                throw;
            }
        }

        /// <summary>
        /// 根据采集系统序列号和接收时间获取对象
        /// </summary>
        /// <param name="number"></param>
        /// <param name="receiveDatetimeFmtDt"></param>
        /// <returns></returns>
        public async Task<VesselInfoDto> GetByNumberReceiveDatetime(string number, DateTime receiveDatetimeFmtDt)
        {
            var result = await _repository.FirstOrDefaultAsync(t => t.SN == number && t.ReceiveDatetime == receiveDatetimeFmtDt && ((t.delete_time ?? new DateTime()) == new DateTime()));
            return ObjectMapper.Map<VesselInfo, VesselInfoDto>(result);
        }
    }
}