using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace hmt_energy_csharp.Engineroom.FOs
{
    public class FOService : hmt_energy_csharpAppService, IFOService
    {
        private readonly IFORepository _fORepository;

        public FOService(IFORepository fORepository)
        {
            _fORepository = fORepository;
        }

        /// <summary>
        /// 根据采集系统序列号和时间获取列表
        /// </summary>
        /// <param name="number"></param>
        /// <param name="receviceDatetime"></param>
        /// <returns></returns>
        public async Task<IList<FODto>> GetListByNumberReceiveDatetimeAsync(string number, DateTime receviceDatetime)
        {
            var result = await _fORepository.GetListAsync(t => t.Number == number && t.ReceiveDatetime == receviceDatetime);
            return ObjectMapper.Map<IList<FO>, IList<FODto>>(result);
        }
    }
}