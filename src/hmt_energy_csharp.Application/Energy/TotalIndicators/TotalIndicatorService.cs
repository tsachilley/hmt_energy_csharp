using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace hmt_energy_csharp.Energy.TotalIndicators
{
    public class TotalIndicatorService : hmt_energy_csharpAppService, ITotalIndicatorService
    {
        private readonly ITotalIndicatorRepository _totalIndicatorRepository;

        public TotalIndicatorService(ITotalIndicatorRepository totalIndicatorRepository)
        {
            _totalIndicatorRepository = totalIndicatorRepository;
        }

        /// <summary>
        /// 根据采集系统序列号和时间获取对象
        /// </summary>
        /// <param name="number"></param>
        /// <param name="receviceDatetime"></param>
        /// <returns></returns>
        public async Task<TotalIndicatorDto> GetByNumberReceiveDatetimeAsync(string number, DateTime receviceDatetime)
        {
            var result = await _totalIndicatorRepository.FirstOrDefaultAsync(t => t.Number == number && t.ReceiveDatetime == receviceDatetime);
            return ObjectMapper.Map<TotalIndicator, TotalIndicatorDto>(result);
        }
    }
}