using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace hmt_energy_csharp.Energy.Predictions
{
    public interface IPredictionService : IApplicationService
    {
        /// <summary>
        /// 根据采集系统序列号和时间获取对象
        /// </summary>
        /// <param name="number"></param>
        /// <param name="receviceDatetime"></param>
        /// <returns></returns>
        Task<PredictionDto> GetByNumberReceiveDatetimeAsync(string number, DateTime receviceDatetime);
    }
}