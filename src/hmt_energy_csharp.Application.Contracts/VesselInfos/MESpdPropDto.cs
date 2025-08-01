using System;

namespace hmt_energy_csharp.VesselInfos
{
    public class MESpdPropDto
    {
        //时间
        public DateTime ReceiveDatetime { get; set; }

        //主机能效
        public double? MESFOC { get; set; }

        //功率
        public double? MEPower { get; set; }

        //转速
        public double? MERpm { get; set; }

        //对水航速
        public double? WaterSpeed { get; set; }

        //纵倾
        public double? Trim { get; set; }

        //吃水
        public double? Draft { get; set; }

        //滑失比
        public double? Slip { get; set; }
    }
}