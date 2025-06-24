using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.ObjectMapping;

namespace hmt_energy_csharp.Energy.Batteries
{
    public class BatteryService : hmt_energy_csharpAppService, IBatteryService
    {
        private readonly IBatteryRepository _batteryRepository;

        public BatteryService(IBatteryRepository batteryRepository)
        {
            _batteryRepository = batteryRepository;
        }

        /// <summary>
        /// 根据采集系统序列号和时间获取列表
        /// </summary>
        /// <param name="number"></param>
        /// <param name="receviceDatetime"></param>
        /// <returns></returns>
        public async Task<IList<BatteryDto>> GetListByNumberReceiveDatetimeAsync(string number, DateTime receviceDatetime)
        {
            var result = await _batteryRepository.GetListAsync(t => t.Number == number && t.ReceiveDatetime == receviceDatetime);
            return ObjectMapper.Map<IList<Battery>, IList<BatteryDto>>(result);
        }
    }
}