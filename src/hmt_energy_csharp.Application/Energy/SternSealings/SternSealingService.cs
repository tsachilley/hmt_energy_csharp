using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace hmt_energy_csharp.Energy.SternSealings
{
    public class SternSealingService : hmt_energy_csharpAppService, ISternSealingService
    {
        private readonly ISternSealingRepository _sternSealingRepository;

        public SternSealingService(ISternSealingRepository sternSealingRepository)
        {
            _sternSealingRepository = sternSealingRepository;
        }

        /// <summary>
        /// 根据采集系统序列号和时间获取列表
        /// </summary>
        /// <param name="number"></param>
        /// <param name="receviceDatetime"></param>
        /// <returns></returns>
        public async Task<IList<SternSealingDto>> GetListByNumberReceiveDatetimeAsync(string number, DateTime receviceDatetime)
        {
            var result = await _sternSealingRepository.GetListAsync(t => t.Number == number && t.ReceiveDatetime == receviceDatetime);
            return ObjectMapper.Map<IList<SternSealing>, IList<SternSealingDto>>(result);
        }
    }
}