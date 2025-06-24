using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace hmt_energy_csharp.Engineroom.LubOilPurifyings
{
    public class LubOilPurifyingService : hmt_energy_csharpAppService, ILubOilPurifyingService
    {
        private readonly ILubOilPurifyingRepository _lubOilPurifyingRepository;

        public LubOilPurifyingService(ILubOilPurifyingRepository lubOilPurifyingRepository)
        {
            _lubOilPurifyingRepository = lubOilPurifyingRepository;
        }

        /// <summary>
        /// 根据采集系统序列号和时间获取列表
        /// </summary>
        /// <param name="number"></param>
        /// <param name="receviceDatetime"></param>
        /// <returns></returns>
        public async Task<IList<LubOilPurifyingDto>> GetListByNumberReceiveDatetimeAsync(string number, DateTime receviceDatetime)
        {
            var result = await _lubOilPurifyingRepository.GetListAsync(t => t.Number == number && t.ReceiveDatetime == receviceDatetime);
            return ObjectMapper.Map<IList<LubOilPurifying>, IList<LubOilPurifyingDto>>(result);
        }
    }
}