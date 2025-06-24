using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace hmt_energy_csharp.Engineroom.ExhaustGases
{
    public class ExhaustGasService : hmt_energy_csharpAppService, IExhaustGasService
    {
        private readonly IExhaustGasRepository _exhaustGasRepository;

        public ExhaustGasService(IExhaustGasRepository exhaustGasRepository)
        {
            _exhaustGasRepository = exhaustGasRepository;
        }

        /// <summary>
        /// 根据采集系统序列号和时间获取列表
        /// </summary>
        /// <param name="number"></param>
        /// <param name="receviceDatetime"></param>
        /// <returns></returns>
        public async Task<IList<ExhaustGasDto>> GetListByNumberReceiveDatetimeAsync(string number, DateTime receviceDatetime)
        {
            var result = await _exhaustGasRepository.GetListAsync(t => t.Number == number && t.ReceiveDatetime == receviceDatetime);
            return ObjectMapper.Map<IList<ExhaustGas>, IList<ExhaustGasDto>>(result);
        }
    }
}