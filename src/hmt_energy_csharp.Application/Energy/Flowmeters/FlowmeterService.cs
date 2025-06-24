using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace hmt_energy_csharp.Energy.Flowmeters
{
    public class FlowmeterService : hmt_energy_csharpAppService, IFlowmeterService
    {
        private readonly IFlowmeterRepository _flowmeterRepository;

        public FlowmeterService(IFlowmeterRepository flowmeterRepository)
        {
            _flowmeterRepository = flowmeterRepository;
        }

        /// <summary>
        /// 根据采集系统序列号和时间获取列表
        /// </summary>
        /// <param name="number"></param>
        /// <param name="receviceDatetime"></param>
        /// <returns></returns>
        public async Task<IList<FlowmeterDto>> GetListByNumberReceiveDatetimeAsync(string number, DateTime receviceDatetime)
        {
            var result = await _flowmeterRepository.GetListAsync(t => t.Number == number && t.ReceiveDatetime == receviceDatetime);
            return ObjectMapper.Map<IList<Flowmeter>, IList<FlowmeterDto>>(result);
        }

        /// <summary>
        /// 获取某种燃油总计消耗
        /// </summary>
        /// <param name="sn"></param>
        /// <param name="receviceDatetime"></param>
        /// <param name="deviceNo"></param>
        /// <param name="fuelType"></param>
        /// <returns></returns>
        public async Task<decimal> GetTotalFCBySNRDFTAsync(string sn, DateTime receviceDatetime, string deviceNo, string fuelType)
        {
            decimal result = 0;
            try
            {
                var lstFm = await _flowmeterRepository.GetRecentlyFmAsync(sn, receviceDatetime, deviceNo, fuelType);
                if (lstFm.Count > 0)
                    result = lstFm[0].ConsAcc ?? 0;
            }
            catch (Exception)
            {
            }
            return result;
        }
    }
}