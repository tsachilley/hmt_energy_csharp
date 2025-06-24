using hmt_energy_csharp.Energy;
using Microsoft.EntityFrameworkCore;
using System;

namespace hmt_energy_csharp.Engineroom.CompressedAirSupplies
{
    /**
     * 主机压缩空气系统
     */

    [Index(nameof(Number), nameof(ReceiveDatetime), nameof(DeviceNo), IsUnique = true, Name = "UK_CompressedAirSupply_NRD")]
    public class CompressedAirSupply : BaseEnergy
    {
        //主机起动空气压力 1501
        public double? MEStartPress { get; set; }

        //主机控制空气进口压力 1502
        public double? MEControlPress { get; set; }

        //主机排气阀弹簧进口空气压力 1503
        public double? ExhaustValuePress { get; set; }

        //上传云端标识
        public byte Uploaded { get; set; } = 0;

        public CompressedAirSupply()
        {
        }

        public CompressedAirSupply(string sentence)
        {
            try
            {
                if (sentence == null)
                    return;
                if (StringHelper.GetBCCXorCode(sentence))
                {
                    var strData = sentence.Substring(0, sentence.Length - 3);
                    string[] str = strData.Split(',');
                }
            }
            catch (Exception)
            {
            }
        }
    }
}