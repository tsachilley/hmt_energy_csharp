using hmt_energy_csharp.Energy;
using Microsoft.EntityFrameworkCore;
using System;

namespace hmt_energy_csharp.Engineroom.ShaftClutches
{
    /**
     * 推进机械轴系 & 离合器
     */

    [Index(nameof(Number), nameof(ReceiveDatetime), nameof(DeviceNo), IsUnique = true, Name = "UK_ShaftClutch_NRD")]
    public class ShaftClutch : BaseEnergy
    {
        //尾管后轴承温度 2101
        public double? SternAftTemp { get; set; }

        //中间轴承温度 2102
        public double? InterTemp { get; set; }

        //上传云端标识
        public byte Uploaded { get; set; } = 0;

        public ShaftClutch()
        {
        }

        public ShaftClutch(string sentence)
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