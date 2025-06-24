using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace hmt_energy_csharp.Energy.LiquidLevels
{
    public class LiquidLevelService : hmt_energy_csharpAppService, ILiquidLevelService
    {
        private readonly ILiquidLevelRepository _liquidLevelRepository;

        public LiquidLevelService(ILiquidLevelRepository liquidLevelRepository)
        {
            _liquidLevelRepository = liquidLevelRepository;
        }

        /// <summary>
        /// 根据采集系统序列号和时间获取列表
        /// </summary>
        /// <param name="number"></param>
        /// <param name="receviceDatetime"></param>
        /// <returns></returns>
        public async Task<IList<LiquidLevelDto>> GetListByNumberReceiveDatetimeAsync(string number, DateTime receviceDatetime)
        {
            var result = await _liquidLevelRepository.GetListAsync(t => t.Number == number && t.ReceiveDatetime == receviceDatetime);
            return ObjectMapper.Map<IList<LiquidLevel>, IList<LiquidLevelDto>>(result);
        }
    }
}