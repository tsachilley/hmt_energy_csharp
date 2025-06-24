using hmt_energy_csharp.Energy;
using Microsoft.EntityFrameworkCore;
using System;

namespace hmt_energy_csharp.Engineroom.CoolingFreshWaters
{
    /**
     * 冷却淡水系统
     */

    [Index(nameof(Number), nameof(ReceiveDatetime), nameof(DeviceNo), IsUnique = true, Name = "UK_CoolingFreshWater_NRD")]
    public class CoolingFreshWater : BaseEnergy
    {
        //低温冷却淡水泵压力 5321
        public double? LTCFWPress { get; set; }

        //中央冷却器低温淡水出口温度 5322
        public double? CCLTCFWOutTemp { get; set; }

        //1号低温冷却淡水泵出口压力 5323C1
        public int? LTCFW1Press { get; set; }

        //2号低温冷却淡水泵出口压力 5323C2
        public int? LTCFW2Press { get; set; }

        //3号低温冷却淡水泵出口压力 5323C3
        public int? LTCFW3Press { get; set; }

        //主机缸套水冷却泵出口压力 5329c
        public int? MEJWCOutPress { get; set; }

        //上传云端标识
        public byte Uploaded { get; set; } = 0;

        public CoolingFreshWater()
        {
        }

        public CoolingFreshWater(string sentence)
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