using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace hmt_energy_csharp.Engineroom.MainGeneratorSets
{
    public class MainGeneratorSetService : hmt_energy_csharpAppService, IMainGeneratorSetService
    {
        private readonly IMainGeneratorSetRepository _mainGeneratorSetRepository;

        public MainGeneratorSetService(IMainGeneratorSetRepository mainGeneratorSetRepository)
        {
            _mainGeneratorSetRepository = mainGeneratorSetRepository;
        }

        /// <summary>
        /// 根据采集系统序列号和时间获取列表
        /// </summary>
        /// <param name="number"></param>
        /// <param name="receviceDatetime"></param>
        /// <returns></returns>
        public async Task<IList<MainGeneratorSetDto>> GetListByNumberReceiveDatetimeAsync(string number, DateTime receviceDatetime)
        {
            var result = await _mainGeneratorSetRepository.GetListAsync(t => t.Number == number && t.ReceiveDatetime == receviceDatetime);
            return ObjectMapper.Map<IList<MainGeneratorSet>, IList<MainGeneratorSetDto>>(result);
        }
    }
}