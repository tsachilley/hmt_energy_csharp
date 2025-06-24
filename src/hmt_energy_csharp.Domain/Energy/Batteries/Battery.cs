using System.Runtime.Intrinsics.Arm;
using System;
using Volo.Abp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace hmt_energy_csharp.Energy.Batteries
{
    /**
     * 电池组
     * */

    [Index(nameof(Number), nameof(ReceiveDatetime), nameof(DeviceNo), IsUnique = true, Name = "UK_Battery_NRD")]
    public class Battery : BaseEnergy
    {
        //剩余电量
        public decimal? SOC { get; set; }

        //健康度
        public decimal? SOH { get; set; }

        //最高温度
        public decimal? MaxTEMP { get; set; }

        //最高温度箱号
        public string MaxTEMPBox { get; set; }

        //最高温度编号
        public string MaxTEMPNo { get; set; }

        //最低温度
        public decimal? MinTEMP { get; set; }

        //最低温度箱号
        public string MinTEMPBox { get; set; }

        //最低温度编号
        public string MinTEMPNo { get; set; }

        //最高电压
        public decimal? MaxVoltage { get; set; }

        //最高电压箱号
        public string MaxVoltageBox { get; set; }

        //最高电压编号
        public string MaxVoltageNo { get; set; }

        //最低电压
        public decimal? MinVoltage { get; set; }

        //最低电压箱号
        public string MinVoltageBox { get; set; }

        //最低电压编号
        public string MinVoltageNo { get; set; }

        //是否已上传
        [Comment("是否已上传")]
        public byte Uploaded { get; set; } = 0;

        public Battery()
        {
        }

        public Battery(string sentence)
        {
            try
            {
                if (sentence == null)
                    return;
                if (StringHelper.GetBCCXorCode(sentence))
                {
                    var strData = sentence.Substring(0, sentence.Length - 3);
                    string[] str = strData.Split(',');
                    SOC = Convert.ToDecimal(str[1]);
                    SOH = Convert.ToDecimal(str[2]);
                    MaxTEMP = Convert.ToDecimal(str[3]);
                    MaxTEMPBox = str[4];
                    MaxTEMPNo = str[5];
                    MinTEMP = Convert.ToDecimal(str[6]);
                    MinTEMPBox = str[7];
                    MinTEMPNo = str[8];
                    MaxVoltage = Convert.ToDecimal(str[9]);
                    MaxVoltageBox = str[10];
                    MaxVoltageNo = str[11];
                    MinVoltage = Convert.ToDecimal(str[12]);
                    MinVoltageBox = str[13];
                    MinVoltageNo = str[14];
                }
            }
            catch (Exception)
            {
            }
        }
    }
}