﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace hmt_energy_csharp.Engineroom.LubOilPurifyings
{
    public interface ILubOilPurifyingService : IApplicationService
    {
        /// <summary>
        /// 根据采集系统序列号和时间获取列表
        /// </summary>
        /// <param name="number"></param>
        /// <param name="receviceDatetime"></param>
        /// <returns></returns>
        Task<IList<LubOilPurifyingDto>> GetListByNumberReceiveDatetimeAsync(string number, DateTime receviceDatetime);
    }
}