﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace hmt_energy_csharp.Engineroom.CoolingFreshWaters
{
    public interface ICoolingFreshWaterService : IApplicationService
    {
        /// <summary>
        /// 根据采集系统序列号和时间获取列表
        /// </summary>
        /// <param name="number"></param>
        /// <param name="receviceDatetime"></param>
        /// <returns></returns>
        Task<IList<CoolingFreshWaterDto>> GetListByNumberReceiveDatetimeAsync(string number, DateTime receviceDatetime);
    }
}