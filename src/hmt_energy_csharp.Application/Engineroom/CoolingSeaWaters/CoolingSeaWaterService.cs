using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace hmt_energy_csharp.Engineroom.CoolingSeaWaters
{
    public class CoolingSeaWaterService : hmt_energy_csharpAppService, ICoolingSeaWaterService
    {
        private readonly ICoolingSeaWaterRepository _coolingSeaWaterRepository;

        public CoolingSeaWaterService(ICoolingSeaWaterRepository coolingSeaWaterRepository)
        {
            _coolingSeaWaterRepository = coolingSeaWaterRepository;
        }

        /// <summary>
        /// 根据采集系统序列号和时间获取列表
        /// </summary>
        /// <param name="number"></param>
        /// <param name="receviceDatetime"></param>
        /// <returns></returns>
        public async Task<IList<CoolingSeaWaterDto>> GetListByNumberReceiveDatetimeAsync(string number, DateTime receviceDatetime)
        {
            var result = await _coolingSeaWaterRepository.GetListAsync(t => t.Number == number && t.ReceiveDatetime == receviceDatetime);
            return ObjectMapper.Map<IList<CoolingSeaWater>, IList<CoolingSeaWaterDto>>(result);
        }
    }
}