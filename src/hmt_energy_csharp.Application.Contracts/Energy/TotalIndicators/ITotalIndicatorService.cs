using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace hmt_energy_csharp.Energy.TotalIndicators
{
    public interface ITotalIndicatorService : IApplicationService
    {
        /// <summary>
        /// 根据采集系统序列号和时间获取对象
        /// </summary>
        /// <param name="number"></param>
        /// <param name="receviceDatetime"></param>
        /// <returns></returns>
        Task<TotalIndicatorDto> GetByNumberReceiveDatetimeAsync(string number, DateTime receviceDatetime);
    }
}