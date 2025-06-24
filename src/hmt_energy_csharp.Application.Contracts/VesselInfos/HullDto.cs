using System;

namespace hmt_energy_csharp.VesselInfos
{
    public class HullDto
    {
        //时间
        public DateTime ReceiveDatetime { get; set; }

        //功率
        public double? MEPower { get; set; }

        //转速
        public double? MERpm { get; set; }

        //对水航速
        public double? WaterSpeed { get; set; }

        //滑失比
        public double? Slip { get; set; }
    }
}