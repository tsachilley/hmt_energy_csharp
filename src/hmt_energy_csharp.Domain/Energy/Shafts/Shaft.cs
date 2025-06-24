using Microsoft.EntityFrameworkCore;
using System;

namespace hmt_energy_csharp.Energy.Shafts
{
    /**
     * 主轴
     */

    [Index(nameof(Number), nameof(ReceiveDatetime), nameof(DeviceNo), IsUnique = true, Name = "UK_Shaft_NRD")]
    public class Shaft : BaseEnergy
    {
        //功率 kW
        public decimal? Power { get; set; }

        //转速
        public decimal? RPM { get; set; }

        //扭矩 kNm
        public decimal? Torque { get; set; }

        //推力 kN
        public decimal? Thrust { get; set; }

        //能量
        public double? Energy { get; set; }

        //转数
        public double? Revolutions { get; set; }

        //是否已上传
        [Comment("是否已上传")]
        public byte Uploaded { get; set; } = 0;

        public Shaft()
        {
        }

        public Shaft(string sentence)
        {
            try
            {
                if (sentence == null)
                    return;
                if (StringHelper.GetBCCXorCode(sentence))
                {
                    var strData = sentence.Substring(0, sentence.Length - 3);
                    string[] str = strData.Split(',');
                    RPM = Convert.ToDecimal(str[1].Trim());
                    Torque = Convert.ToDecimal(str[2].Trim());
                    Thrust = Convert.ToDecimal(str[3].Trim());
                    Power = Convert.ToDecimal(str[4].Trim());
                }
            }
            catch (Exception)
            {
            }
        }
    }
}