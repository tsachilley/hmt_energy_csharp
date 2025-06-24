using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace hmt_energy_csharp.Energy.Shafts
{
    public class ShaftService : hmt_energy_csharpAppService, IShaftService
    {
        private readonly IShaftRepository _shaftRepository;

        public ShaftService(IShaftRepository shaftRepository)
        {
            _shaftRepository = shaftRepository;
        }

        /// <summary>
        /// 根据采集系统序列号和时间获取列表
        /// </summary>
        /// <param name="number"></param>
        /// <param name="receviceDatetime"></param>
        /// <returns></returns>
        public async Task<IList<ShaftDto>> GetListByNumberReceiveDatetimeAsync(string number, DateTime receviceDatetime)
        {
            var result = await _shaftRepository.GetListAsync(t => t.Number == number && t.ReceiveDatetime == receviceDatetime);
            return ObjectMapper.Map<IList<Shaft>, IList<ShaftDto>>(result);
        }
    }
}