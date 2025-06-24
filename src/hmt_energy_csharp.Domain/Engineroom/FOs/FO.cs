using hmt_energy_csharp.Energy;
using Microsoft.EntityFrameworkCore;
using System;

namespace hmt_energy_csharp.Engineroom.FOs
{
    /**
     * 燃油系统
     */

    [Index(nameof(Number), nameof(ReceiveDatetime), nameof(DeviceNo), IsUnique = true, Name = "UK_FO_NRD")]
    public class FO : BaseEnergy
    {
        //主机燃油进机压力 1201
        public double? MEInPressure { get; set; }

        //主机燃油进机温度 1202
        public double? MEInTemp { get; set; }

        //主机高压油管泄漏高报警 1203
        public int? MEHPOPLeakage { get; set; }

        //上传云端标识
        public byte Uploaded { get; set; } = 0;

        public FO()
        {
        }

        public FO(string sentence)
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