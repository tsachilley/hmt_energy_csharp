using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace hmt_energy_csharp.Engineroom.ShaftClutches
{
    public class ShaftClutchService : hmt_energy_csharpAppService, IShaftClutchService
    {
        private readonly IShaftClutchRepository _shaftClutchRepository;

        public ShaftClutchService(IShaftClutchRepository shaftClutchRepository)
        {
            _shaftClutchRepository = shaftClutchRepository;
        }

        /// <summary>
        /// 根据采集系统序列号和时间获取列表
        /// </summary>
        /// <param name="number"></param>
        /// <param name="receviceDatetime"></param>
        /// <returns></returns>
        public async Task<IList<ShaftClutchDto>> GetListByNumberReceiveDatetimeAsync(string number, DateTime receviceDatetime)
        {
            var result = await _shaftClutchRepository.GetListAsync(t => t.Number == number && t.ReceiveDatetime == receviceDatetime);
            return ObjectMapper.Map<IList<ShaftClutch>, IList<ShaftClutchDto>>(result);
        }
    }
}