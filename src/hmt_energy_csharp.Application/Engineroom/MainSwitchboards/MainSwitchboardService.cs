using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace hmt_energy_csharp.Engineroom.MainSwitchboards
{
    public class MainSwitchboardService : hmt_energy_csharpAppService, IMainSwitchboardService
    {
        private readonly IMainSwitchboardRepository _mainSwitchboardRepository;

        public MainSwitchboardService(IMainSwitchboardRepository mainSwitchboardRepository)
        {
            _mainSwitchboardRepository = mainSwitchboardRepository;
        }

        /// <summary>
        /// 根据采集系统序列号和时间获取列表
        /// </summary>
        /// <param name="number"></param>
        /// <param name="receviceDatetime"></param>
        /// <returns></returns>
        public async Task<IList<MainSwitchboardDto>> GetListByNumberReceiveDatetimeAsync(string number, DateTime receviceDatetime)
        {
            var result = await _mainSwitchboardRepository.GetListAsync(t => t.Number == number && t.ReceiveDatetime == receviceDatetime);
            return ObjectMapper.Map<IList<MainSwitchboard>, IList<MainSwitchboardDto>>(result);
        }
    }
}