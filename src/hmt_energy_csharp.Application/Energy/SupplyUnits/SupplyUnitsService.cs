using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace hmt_energy_csharp.Energy.SupplyUnits
{
    public class SupplyUnitsService : hmt_energy_csharpAppService, ISupplyUnitsService
    {
        private readonly ISupplyUnitRepository _supplyUnitRepository;

        public SupplyUnitsService(ISupplyUnitRepository supplyUnitRepository)
        {
            _supplyUnitRepository = supplyUnitRepository;
        }

        /// <summary>
        /// 根据采集系统序列号和时间获取列表
        /// </summary>
        /// <param name="number"></param>
        /// <param name="receviceDatetime"></param>
        /// <returns></returns>
        public async Task<IList<SupplyUnitDto>> GetListByNumberReceiveDatetimeAsync(string number, DateTime receviceDatetime)
        {
            var result = await _supplyUnitRepository.GetListAsync(t => t.Number == number && t.ReceiveDatetime == receviceDatetime);
            return ObjectMapper.Map<IList<SupplyUnit>, IList<SupplyUnitDto>>(result);
        }
    }
}