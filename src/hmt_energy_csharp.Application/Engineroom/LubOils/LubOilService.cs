using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace hmt_energy_csharp.Engineroom.LubOils
{
    public class LubOilService : hmt_energy_csharpAppService, ILubOilService
    {
        private readonly ILubOilRepository _lubOilRepository;

        public LubOilService(ILubOilRepository lubOilRepository)
        {
            _lubOilRepository = lubOilRepository;
        }

        /// <summary>
        /// 根据采集系统序列号和时间获取列表
        /// </summary>
        /// <param name="number"></param>
        /// <param name="receviceDatetime"></param>
        /// <returns></returns>
        public async Task<IList<LubOilDto>> GetListByNumberReceiveDatetimeAsync(string number, DateTime receviceDatetime)
        {
            var result = await _lubOilRepository.GetListAsync(t => t.Number == number && t.ReceiveDatetime == receviceDatetime);
            return ObjectMapper.Map<IList<LubOil>, IList<LubOilDto>>(result);
        }
    }
}