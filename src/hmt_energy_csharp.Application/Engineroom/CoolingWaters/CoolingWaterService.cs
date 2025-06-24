using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace hmt_energy_csharp.Engineroom.CoolingWaters
{
    public class CoolingWaterService : hmt_energy_csharpAppService, ICoolingWaterService
    {
        private readonly ICoolingWaterRepository _coolingWaterRepository;

        public CoolingWaterService(ICoolingWaterRepository coolingWaterRepository)
        {
            _coolingWaterRepository = coolingWaterRepository;
        }

        /// <summary>
        /// 根据采集系统序列号和时间获取列表
        /// </summary>
        /// <param name="number"></param>
        /// <param name="receviceDatetime"></param>
        /// <returns></returns>
        public async Task<IList<CoolingWaterDto>> GetListByNumberReceiveDatetimeAsync(string number, DateTime receviceDatetime)
        {
            var result = await _coolingWaterRepository.GetListAsync(t => t.Number == number && t.ReceiveDatetime == receviceDatetime);
            return ObjectMapper.Map<IList<CoolingWater>, IList<CoolingWaterDto>>(result);
        }
    }
}