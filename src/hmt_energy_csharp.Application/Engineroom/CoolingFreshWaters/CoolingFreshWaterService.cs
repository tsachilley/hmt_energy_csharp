using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace hmt_energy_csharp.Engineroom.CoolingFreshWaters
{
    public class CoolingFreshWaterService : hmt_energy_csharpAppService, ICoolingFreshWaterService
    {
        private readonly ICoolingFreshWaterRepository _coolingFreshWaterRepository;

        public CoolingFreshWaterService(ICoolingFreshWaterRepository coolingFreshWaterRepository)
        {
            _coolingFreshWaterRepository = coolingFreshWaterRepository;
        }

        /// <summary>
        /// 根据采集系统序列号和时间获取列表
        /// </summary>
        /// <param name="number"></param>
        /// <param name="receviceDatetime"></param>
        /// <returns></returns>
        public async Task<IList<CoolingFreshWaterDto>> GetListByNumberReceiveDatetimeAsync(string number, DateTime receviceDatetime)
        {
            var result = await _coolingFreshWaterRepository.GetListAsync(t => t.Number == number && t.ReceiveDatetime == receviceDatetime);
            return ObjectMapper.Map<IList<CoolingFreshWater>, IList<CoolingFreshWaterDto>>(result);
        }
    }
}