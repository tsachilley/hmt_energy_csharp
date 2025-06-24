using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace hmt_energy_csharp.Engineroom.Miscellaneouses
{
    public class MiscellaneousService : hmt_energy_csharpAppService, IMiscellaneousService
    {
        private readonly IMiscellaneousRepository _miscellaneousRepository;

        public MiscellaneousService(IMiscellaneousRepository miscellaneousRepository)
        {
            _miscellaneousRepository = miscellaneousRepository;
        }

        /// <summary>
        /// 根据采集系统序列号和时间获取列表
        /// </summary>
        /// <param name="number"></param>
        /// <param name="receviceDatetime"></param>
        /// <returns></returns>
        public async Task<IList<MiscellaneousDto>> GetListByNumberReceiveDatetimeAsync(string number, DateTime receviceDatetime)
        {
            var result = await _miscellaneousRepository.GetListAsync(t => t.Number == number && t.ReceiveDatetime == receviceDatetime);
            return ObjectMapper.Map<IList<Miscellaneous>, IList<MiscellaneousDto>>(result);
        }
    }
}