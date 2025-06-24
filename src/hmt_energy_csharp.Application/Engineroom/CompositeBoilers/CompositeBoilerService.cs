using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace hmt_energy_csharp.Engineroom.CompositeBoilers
{
    public class CompositeBoilerService : hmt_energy_csharpAppService, ICompositeBoilerService
    {
        private readonly ICompositeBoilerRepository _compositeBoilerRepository;

        public CompositeBoilerService(ICompositeBoilerRepository compositeBoilerRepository)
        {
            _compositeBoilerRepository = compositeBoilerRepository;
        }

        /// <summary>
        /// 根据采集系统序列号和时间获取列表
        /// </summary>
        /// <param name="number"></param>
        /// <param name="receviceDatetime"></param>
        /// <returns></returns>
        public async Task<IList<CompositeBoilerDto>> GetListByNumberReceiveDatetimeAsync(string number, DateTime receviceDatetime)
        {
            var result = await _compositeBoilerRepository.GetListAsync(t => t.Number == number && t.ReceiveDatetime == receviceDatetime);
            return ObjectMapper.Map<IList<CompositeBoiler>, IList<CompositeBoilerDto>>(result);
        }
    }
}