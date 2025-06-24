using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace hmt_energy_csharp.Energy.PowerUnits
{
    public class PowerUnitService : hmt_energy_csharpAppService, IPowerUnitService
    {
        private readonly IPowerUnitRepository _powerUnitRepository;

        public PowerUnitService(IPowerUnitRepository powerUnitRepository)
        {
            _powerUnitRepository = powerUnitRepository;
        }

        /// <summary>
        /// 根据采集系统序列号和时间获取列表
        /// </summary>
        /// <param name="number"></param>
        /// <param name="receviceDatetime"></param>
        /// <returns></returns>
        public async Task<IList<PowerUnitDto>> GetListByNumberReceiveDatetimeAsync(string number, DateTime receviceDatetime)
        {
            var result = await _powerUnitRepository.GetListAsync(t => t.Number == number && t.ReceiveDatetime == receviceDatetime);
            return ObjectMapper.Map<IList<PowerUnit>, IList<PowerUnitDto>>(result);
        }
    }
}