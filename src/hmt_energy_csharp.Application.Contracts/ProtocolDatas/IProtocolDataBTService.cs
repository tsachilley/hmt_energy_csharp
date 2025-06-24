using hmt_energy_csharp.ResponseResults;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace hmt_energy_csharp.ProtocolDatas
{
    public interface IProtocolDataBTService
    {
        /// <summary>
        /// 解析
        /// </summary>
        /// <param name="number">采集系统序列号</param>
        /// <param name="sentence">语句</param>
        /// <param name="startChars">起始符</param>
        /// <param name="endChars">结束符</param>
        /// <returns></returns>
        Task<ResponseResult> DecodeAsync(string number, string sentence, string startChars, string endChars);
    }
}