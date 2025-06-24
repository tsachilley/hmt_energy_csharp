using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace hmt_energy_csharp.Engineroom.FOSupplyUnits
{
    public class FOSupplyUnitService : hmt_energy_csharpAppService, IFOSupplyUnitService
    {
        private readonly IFOSupplyUnitRepository _fOSupplyUnitRepository;

        public FOSupplyUnitService(IFOSupplyUnitRepository fOSupplyUnitRepository)
        {
            _fOSupplyUnitRepository = fOSupplyUnitRepository;
        }

        /// <summary>
        /// 根据采集系统序列号和时间获取列表
        /// </summary>
        /// <param name="number"></param>
        /// <param name="receviceDatetime"></param>
        /// <returns></returns>
        public async Task<IList<FOSupplyUnitDto>> GetListByNumberReceiveDatetimeAsync(string number, DateTime receviceDatetime)
        {
            var result = await _fOSupplyUnitRepository.GetListAsync(t => t.Number == number && t.ReceiveDatetime == receviceDatetime);
            return ObjectMapper.Map<IList<FOSupplyUnit>, IList<FOSupplyUnitDto>>(result);
        }
    }
}