using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace hmt_energy_csharp.Engineroom.CylinderLubOils
{
    public class CylinderLubOilService : hmt_energy_csharpAppService, ICylinderLubOilService
    {
        private readonly ICylinderLubOilRepository _cylinderLubOilRepository;

        public CylinderLubOilService(ICylinderLubOilRepository cylinderLubOilRepository)
        {
            _cylinderLubOilRepository = cylinderLubOilRepository;
        }

        /// <summary>
        /// 根据采集系统序列号和时间获取列表
        /// </summary>
        /// <param name="number"></param>
        /// <param name="receviceDatetime"></param>
        /// <returns></returns>
        public async Task<IList<CylinderLubOilDto>> GetListByNumberReceiveDatetimeAsync(string number, DateTime receviceDatetime)
        {
            var result = await _cylinderLubOilRepository.GetListAsync(t => t.Number == number && t.ReceiveDatetime == receviceDatetime);
            return ObjectMapper.Map<IList<CylinderLubOil>, IList<CylinderLubOilDto>>(result);
        }
    }
}