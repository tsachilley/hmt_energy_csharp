using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace hmt_energy_csharp.Engineroom.MERemoteControls
{
    public class MERemoteControlService : hmt_energy_csharpAppService, IMERemoteControlService
    {
        private readonly IMERemoteControlRepository _mERemoteControlRepository;

        public MERemoteControlService(IMERemoteControlRepository mERemoteControlRepository)
        {
            _mERemoteControlRepository = mERemoteControlRepository;
        }

        /// <summary>
        /// 根据采集系统序列号和时间获取列表
        /// </summary>
        /// <param name="number"></param>
        /// <param name="receviceDatetime"></param>
        /// <returns></returns>
        public async Task<IList<MERemoteControlDto>> GetListByNumberReceiveDatetimeAsync(string number, DateTime receviceDatetime)
        {
            var result = await _mERemoteControlRepository.GetListAsync(t => t.Number == number && t.ReceiveDatetime == receviceDatetime);
            return ObjectMapper.Map<IList<MERemoteControl>, IList<MERemoteControlDto>>(result);
        }
    }
}