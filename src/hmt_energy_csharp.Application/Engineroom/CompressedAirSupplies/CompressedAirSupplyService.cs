using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace hmt_energy_csharp.Engineroom.CompressedAirSupplies
{
    public class CompressedAirSupplyService : hmt_energy_csharpAppService, ICompressedAirSupplyService
    {
        private readonly ICompressedAirSupplyRepository _compressedAirSupplyRepository;

        public CompressedAirSupplyService(ICompressedAirSupplyRepository compressedAirSupplyRepository)
        {
            _compressedAirSupplyRepository = compressedAirSupplyRepository;
        }

        /// <summary>
        /// 根据采集系统序列号和时间获取列表
        /// </summary>
        /// <param name="number"></param>
        /// <param name="receviceDatetime"></param>
        /// <returns></returns>
        public async Task<IList<CompressedAirSupplyDto>> GetListByNumberReceiveDatetimeAsync(string number, DateTime receviceDatetime)
        {
            var result = await _compressedAirSupplyRepository.GetListAsync(t => t.Number == number && t.ReceiveDatetime == receviceDatetime);
            return ObjectMapper.Map<IList<CompressedAirSupply>, IList<CompressedAirSupplyDto>>(result);
        }
    }
}