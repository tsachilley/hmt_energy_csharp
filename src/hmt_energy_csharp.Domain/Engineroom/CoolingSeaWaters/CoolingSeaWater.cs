using hmt_energy_csharp.Energy;
using Microsoft.EntityFrameworkCore;
using System;

namespace hmt_energy_csharp.Engineroom.CoolingSeaWaters
{
    /**
     * 冷却海水系统
     */

    [Index(nameof(Number), nameof(ReceiveDatetime), nameof(DeviceNo), IsUnique = true, Name = "UK_CoolingSeaWater_NRD")]
    public class CoolingSeaWater : BaseEnergy
    {
        //冷却海水泵出口压力 5311
        public double? CSWOutPress { get; set; }

        //冷却海水泵出口温度 5312
        public double? CSWOutTemp { get; set; }

        //上传云端标识
        public byte Uploaded { get; set; } = 0;

        public CoolingSeaWater()
        {
        }

        public CoolingSeaWater(string sentence)
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