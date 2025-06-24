using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace hmt_energy_csharp.Engineroom.CompressedAirSupplies
{
    public interface ICompressedAirSupplyService : IApplicationService
    {
        /// <summary>
        /// 根据采集系统序列号和时间获取列表
        /// </summary>
        /// <param name="number"></param>
        /// <param name="receviceDatetime"></param>
        /// <returns></returns>
        Task<IList<CompressedAirSupplyDto>> GetListByNumberReceiveDatetimeAsync(string number, DateTime receviceDatetime);
    }
}