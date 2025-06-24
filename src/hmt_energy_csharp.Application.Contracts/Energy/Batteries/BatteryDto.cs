using System;

namespace hmt_energy_csharp.Energy.Batteries
{
    /**
     * 电池组
     * */

    public class BatteryDto : BaseEnergyDto
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
        public byte Uploaded { get; set; } = 0;
    }
}