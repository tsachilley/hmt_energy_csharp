using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace hmt_energy_csharp.Energy.Flowmeters
{
    public interface IFlowmeterService : IApplicationService
    {
        /// <summary>
        /// 根据采集系统序列号和时间获取列表
        /// </summary>
        /// <param name="number"></param>
        /// <param name="receviceDatetime"></param>
        /// <returns></returns>
        Task<IList<FlowmeterDto>> GetListByNumberReceiveDatetimeAsync(string number, DateTime receviceDatetime);

        /// <summary>
        /// 获取某种燃油总计消耗
        /// </summary>
        /// <param name="sn"></param>
        /// <param name="receviceDatetime"></param>
        /// <param name="deviceNo"></param>
        /// <param name="fuelType"></param>
        /// <returns></returns>
        Task<decimal> GetTotalFCBySNRDFTAsync(string sn, DateTime receviceDatetime, string deviceNo, string fuelType);
    }
}