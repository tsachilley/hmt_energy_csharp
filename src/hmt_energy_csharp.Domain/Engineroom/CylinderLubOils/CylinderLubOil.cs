using hmt_energy_csharp.Energy;
using Microsoft.EntityFrameworkCore;
using System;

namespace hmt_energy_csharp.Engineroom.CylinderLubOils
{
    /**
     * 主机气缸滑油
     */

    [Index(nameof(Number), nameof(ReceiveDatetime), nameof(DeviceNo), IsUnique = true, Name = "UK_CylinderLubOil_NRD")]
    public class CylinderLubOil : BaseEnergy
    {
        //主机气缸滑油进口温度 1361
        public double? MEInTemp { get; set; }

        //上传云端标识
        public byte Uploaded { get; set; } = 0;

        public CylinderLubOil()
        {
        }

        public CylinderLubOil(string sentence)
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