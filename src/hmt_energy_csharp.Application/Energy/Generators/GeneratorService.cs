using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace hmt_energy_csharp.Energy.Generators
{
    public class GeneratorService : hmt_energy_csharpAppService, IGeneratorService
    {
        private readonly IGeneratorRepository _generatorRepository;

        public GeneratorService(IGeneratorRepository generatorRepository)
        {
            _generatorRepository = generatorRepository;
        }

        /// <summary>
        /// 根据采集系统序列号和时间获取列表
        /// </summary>
        /// <param name="number"></param>
        /// <param name="receviceDatetime"></param>
        /// <returns></returns>
        public async Task<IList<GeneratorDto>> GetListByNumberReceiveDatetimeAsync(string number, DateTime receviceDatetime)
        {
            var result = await _generatorRepository.GetListAsync(t => t.Number == number && t.ReceiveDatetime == receviceDatetime);
            return ObjectMapper.Map<IList<Generator>, IList<GeneratorDto>>(result);
        }
    }
}