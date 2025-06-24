using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace hmt_energy_csharp.Indexes
{
    public interface ICIIService : IApplicationService
    {
        /// <summary>
        /// 计算CII
        /// </summary>
        /// <param name="parameters">计算参数,json格式</param>
        /// <returns></returns>
        public Task<object> CalcCII(string parameters);

        /// <summary>
        /// 计算EEOI
        /// </summary>
        /// <param name="parameters">计算参数,json格式</param>
        /// <returns></returns>
        public Task<object> CalcEEOI(string parameters);

        /// <summary>
        /// 计算C排放量
        /// </summary>
        /// <param name="parameters">计算参数,json格式</param>
        /// <returns></returns>
        public Task<double> CalcCEmission(string parameters);

        /// <summary>
        /// 计算基于DWT的碳排放指数
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public Task<double> CalcCDWT(string parameters);
    }
}