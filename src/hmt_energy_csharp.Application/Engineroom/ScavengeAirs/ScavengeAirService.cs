using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace hmt_energy_csharp.Engineroom.ScavengeAirs
{
    public class ScavengeAirService : hmt_energy_csharpAppService, IScavengeAirService
    {
        private readonly IScavengeAirRepository _scavengeAirRepository;

        public ScavengeAirService(IScavengeAirRepository scavengeAirRepository)
        {
            _scavengeAirRepository = scavengeAirRepository;
        }

        /// <summary>
        /// 根据采集系统序列号和时间获取列表
        /// </summary>
        /// <param name="number"></param>
        /// <param name="receviceDatetime"></param>
        /// <returns></returns>
        public async Task<IList<ScavengeAirDto>> GetListByNumberReceiveDatetimeAsync(string number, DateTime receviceDatetime)
        {
            var result = await _scavengeAirRepository.GetListAsync(t => t.Number == number && t.ReceiveDatetime == receviceDatetime);
            return ObjectMapper.Map<IList<ScavengeAir>, IList<ScavengeAirDto>>(result);
        }
    }
}