using System;

namespace hmt_energy_csharp.VesselInfos
{
    public class VFCSpdDto
    {
        //对水航速
        public double? WaterSpeed { get; set; }

        //全船每海里油耗
        public double? FCPerNm { get; set; }

        //主机每海里油耗
        public double? MEFCPerNm { get; set; }

        //辅机每海里油耗
        public double? DGFCPerNm { get; set; }

        //锅炉每海里油耗
        public double? BLRFCPerNm { get; set; }

        //时间
        public DateTime ReceiveDatetime { get; set; }

        //纵倾
        public double? Trim { get; set; }

        //吃水
        public double? Draft { get; set; }

        //滑失比
        public double? Slip { get; set; }

        //功率
        public double? MEPower { get; set; }

        //转速
        public double? MERpm { get; set; }

        //风向
        public double? WindDirection { get; set; }

        //风速
        public double? WindSpeed { get; set; }
    }
}