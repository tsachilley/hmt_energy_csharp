using hmt_energy_csharp.Energy;
using Microsoft.EntityFrameworkCore;
using System;

namespace hmt_energy_csharp.Engineroom.Miscellaneouses
{
    /**
     * 其他
     */

    [Index(nameof(Number), nameof(ReceiveDatetime), nameof(DeviceNo), IsUnique = true, Name = "UK_Miscellaneous_NRD")]
    public class Miscellaneous : BaseEnergy
    {
        //主机曲轴箱油雾浓度高 1831 1832
        public int? MECCOMHigh { get; set; }

        //主机轴向振动高 1834 1835
        public double? MEAxialVibration { get; set; }

        //主机负荷 1850
        public double? MELoad { get; set; }

        //主机增压器转速 1853
        public double? METCSpeed { get; set; }

        //上传云端标识
        public byte Uploaded { get; set; } = 0;

        public Miscellaneous()
        {
        }

        public Miscellaneous(string sentence)
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